// System.Console
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

/// <summary>Stellt die Standardstreams für Eingabe, Ausgabe und Fehler bei Konsolenanwendungen dar.Die Klasse kann nicht geerbt werden.</summary>
/// <filterpriority>1</filterpriority>
public static class Console
{
	[Flags]
	internal enum ControlKeyState
	{
		RightAltPressed = 0x1,
		LeftAltPressed = 0x2,
		RightCtrlPressed = 0x4,
		LeftCtrlPressed = 0x8,
		ShiftPressed = 0x10,
		NumLockOn = 0x20,
		ScrollLockOn = 0x40,
		CapsLockOn = 0x80,
		EnhancedKey = 0x100
	}

	internal sealed class ControlCHooker : CriticalFinalizerObject
	{
		private bool _hooked;

		[SecurityCritical]
		private Win32Native.ConsoleCtrlHandlerRoutine _handler;

		[SecurityCritical]
		internal ControlCHooker()
		{
			_handler = BreakEvent;
		}

		~ControlCHooker()
		{
			Unhook();
		}

		[SecuritySafeCritical]
		internal void Hook()
		{
			if (!_hooked)
			{
				if (!Win32Native.SetConsoleCtrlHandler(_handler, addOrRemove: true))
				{
					__Error.WinIOError();
				}
				_hooked = true;
			}
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal void Unhook()
		{
			if (_hooked)
			{
				if (!Win32Native.SetConsoleCtrlHandler(_handler, addOrRemove: false))
				{
					__Error.WinIOError();
				}
				_hooked = false;
			}
		}
	}

	private sealed class ControlCDelegateData
	{
		internal ConsoleSpecialKey ControlKey;

		internal bool Cancel;

		internal bool DelegateStarted;

		internal ManualResetEvent CompletionEvent;

		internal ConsoleCancelEventHandler CancelCallbacks;

		internal ControlCDelegateData(ConsoleSpecialKey controlKey, ConsoleCancelEventHandler cancelCallbacks)
		{
			ControlKey = controlKey;
			CancelCallbacks = cancelCallbacks;
			CompletionEvent = new ManualResetEvent(initialState: false);
		}
	}

	private const int DefaultConsoleBufferSize = 256;

	private const short AltVKCode = 18;

	private const int NumberLockVKCode = 144;

	private const int CapsLockVKCode = 20;

	private const int MinBeepFrequency = 37;

	private const int MaxBeepFrequency = 32767;

	private const int MaxConsoleTitleLength = 24500;

	private static readonly UnicodeEncoding StdConUnicodeEncoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);

	private static volatile TextReader _in;

	private static volatile TextWriter _out;

	private static volatile TextWriter _error;

	private static volatile ConsoleCancelEventHandler _cancelCallbacks;

	private static volatile ControlCHooker _hooker;

	[SecurityCritical]
	private static Win32Native.InputRecord _cachedInputRecord;

	private static volatile bool _haveReadDefaultColors;

	private static volatile byte _defaultColors;

	private static volatile bool _isOutTextWriterRedirected = false;

	private static volatile bool _isErrorTextWriterRedirected = false;

	private static volatile Encoding _inputEncoding = null;

	private static volatile Encoding _outputEncoding = null;

	private static volatile bool _stdInRedirectQueried = false;

	private static volatile bool _stdOutRedirectQueried = false;

	private static volatile bool _stdErrRedirectQueried = false;

	private static bool _isStdInRedirected;

	private static bool _isStdOutRedirected;

	private static bool _isStdErrRedirected;

	private static volatile object s_InternalSyncObject;

	private static volatile object s_ReadKeySyncObject;

	private static volatile IntPtr _consoleInputHandle;

	private static volatile IntPtr _consoleOutputHandle;

	private static object InternalSyncObject
	{
		get
		{
			if (s_InternalSyncObject == null)
			{
				object value = new object();
				Interlocked.CompareExchange<object>(ref s_InternalSyncObject, value, (object)null);
			}
			return s_InternalSyncObject;
		}
	}

	private static object ReadKeySyncObject
	{
		get
		{
			if (s_ReadKeySyncObject == null)
			{
				object value = new object();
				Interlocked.CompareExchange<object>(ref s_ReadKeySyncObject, value, (object)null);
			}
			return s_ReadKeySyncObject;
		}
	}

	private static IntPtr ConsoleInputHandle
	{
		[SecurityCritical]
		get
		{
			if (_consoleInputHandle == IntPtr.Zero)
			{
				_consoleInputHandle = Win32Native.GetStdHandle(-10);
			}
			return _consoleInputHandle;
		}
	}

	private static IntPtr ConsoleOutputHandle
	{
		[SecurityCritical]
		get
		{
			if (_consoleOutputHandle == IntPtr.Zero)
			{
				_consoleOutputHandle = Win32Native.GetStdHandle(-11);
			}
			return _consoleOutputHandle;
		}
	}

	public static bool IsInputRedirected
	{
		[SecuritySafeCritical]
		get
		{
			if (_stdInRedirectQueried)
			{
				return _isStdInRedirected;
			}
			lock (InternalSyncObject)
			{
				if (_stdInRedirectQueried)
				{
					return _isStdInRedirected;
				}
				_isStdInRedirected = IsHandleRedirected(ConsoleInputHandle);
				_stdInRedirectQueried = true;
				return _isStdInRedirected;
			}
		}
	}

	public static bool IsOutputRedirected
	{
		[SecuritySafeCritical]
		get
		{
			if (_stdOutRedirectQueried)
			{
				return _isStdOutRedirected;
			}
			lock (InternalSyncObject)
			{
				if (_stdOutRedirectQueried)
				{
					return _isStdOutRedirected;
				}
				_isStdOutRedirected = IsHandleRedirected(ConsoleOutputHandle);
				_stdOutRedirectQueried = true;
				return _isStdOutRedirected;
			}
		}
	}

	public static bool IsErrorRedirected
	{
		[SecuritySafeCritical]
		get
		{
			if (_stdErrRedirectQueried)
			{
				return _isStdErrRedirected;
			}
			lock (InternalSyncObject)
			{
				if (_stdErrRedirectQueried)
				{
					return _isStdErrRedirected;
				}
				IntPtr stdHandle = Win32Native.GetStdHandle(-12);
				_isStdErrRedirected = IsHandleRedirected(stdHandle);
				_stdErrRedirectQueried = true;
				return _isStdErrRedirected;
			}
		}
	}

	/// <summary>Ruft den Standardeingabestream ab.</summary>
	/// <returns>Ein <see cref="T:System.IO.TextReader" />, der den Standardeingabestream darstellt.</returns>
	/// <filterpriority>1</filterpriority>
	public static TextReader In
	{
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, UI = true)]
		get
		{
			if (_in == null)
			{
				lock (InternalSyncObject)
				{
					if (_in == null)
					{
						Stream stream = OpenStandardInput(256);
						TextReader @in;
						if (stream == Stream.Null)
						{
							@in = StreamReader.Null;
						}
						else
						{
							Encoding inputEncoding = InputEncoding;
							@in = TextReader.Synchronized(new StreamReader(stream, inputEncoding, detectEncodingFromByteOrderMarks: false, 256, leaveOpen: true));
						}
						Thread.MemoryBarrier();
						_in = @in;
					}
				}
			}
			return _in;
		}
	}

	/// <summary>Ruft den Standardausgabestream ab.</summary>
	/// <returns>Ein <see cref="T:System.IO.TextWriter" />, der den Standardausgabestream darstellt.</returns>
	/// <filterpriority>1</filterpriority>
	public static TextWriter Out
	{
		[HostProtection(SecurityAction.LinkDemand, UI = true)]
		get
		{
			if (_out == null)
			{
				InitializeStdOutError(stdout: true);
			}
			return _out;
		}
	}

	/// <summary>Ruft den Standard-Fehlerausgabestream ab.</summary>
	/// <returns>Ein <see cref="T:System.IO.TextWriter" />, der den Standard-Fehlerausgabestream darstellt.</returns>
	/// <filterpriority>1</filterpriority>
	public static TextWriter Error
	{
		[HostProtection(SecurityAction.LinkDemand, UI = true)]
		get
		{
			if (_error == null)
			{
				InitializeStdOutError(stdout: false);
			}
			return _error;
		}
	}

	/// <summary>Ruft die Codierung ab, die die Konsole verwendet, um die Eingabe zu lesen, oder legt diese fest. </summary>
	/// <returns>Die Codierung, die verwendet wird, um die Konsoleneingabe zu lesen.</returns>
	/// <exception cref="T:System.ArgumentNullException">Der Eigenschaftswert in einer Set-Operation ist null.</exception>
	/// <exception cref="T:System.PlatformNotSupportedException">Die Set-Operation für diese Eigenschaft wird unter Windows 98, Windows 98 Zweite Ausgabe und Windows Millennium Edition nicht unterstützt.</exception>
	/// <exception cref="T:System.IO.IOException">Während der Ausführung dieser Operation ist ein Fehler aufgetreten.</exception>
	/// <exception cref="T:System.Security.SecurityException">Die Anwendung hat keine Berechtigung, diese Operation auszuführen.</exception>
	/// <filterpriority>1</filterpriority>
	public static Encoding InputEncoding
	{
		[SecuritySafeCritical]
		get
		{
			if (_inputEncoding != null)
			{
				return _inputEncoding;
			}
			lock (InternalSyncObject)
			{
				if (_inputEncoding != null)
				{
					return _inputEncoding;
				}
				uint consoleCP = Win32Native.GetConsoleCP();
				_inputEncoding = Encoding.GetEncoding((int)consoleCP);
				return _inputEncoding;
			}
		}
		[SecuritySafeCritical]
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			lock (InternalSyncObject)
			{
				if (!IsStandardConsoleUnicodeEncoding(value))
				{
					uint codePage = (uint)value.CodePage;
					if (!Win32Native.SetConsoleCP(codePage))
					{
						__Error.WinIOError();
					}
				}
				_inputEncoding = (Encoding)value.Clone();
				_in = null;
			}
		}
	}

	/// <summary>Ruft die Codierung ab, die die Konsole verwendet, um die Ausgabe zu schreiben, oder legt diese fest. </summary>
	/// <returns>Die Codierung, die verwendet wird, um die Konsolenausgabe zu schreiben.</returns>
	/// <exception cref="T:System.ArgumentNullException">Der Eigenschaftswert in einer Set-Operation ist null.</exception>
	/// <exception cref="T:System.PlatformNotSupportedException">Die Set-Operation für diese Eigenschaft wird unter Windows 98, Windows 98 Zweite Ausgabe und Windows Millennium Edition nicht unterstützt.</exception>
	/// <exception cref="T:System.IO.IOException">Während der Ausführung dieser Operation ist ein Fehler aufgetreten.</exception>
	/// <exception cref="T:System.Security.SecurityException">Die Anwendung hat keine Berechtigung, diese Operation auszuführen.</exception>
	/// <filterpriority>1</filterpriority>
	public static Encoding OutputEncoding
	{
		[SecuritySafeCritical]
		get
		{
			if (_outputEncoding != null)
			{
				return _outputEncoding;
			}
			lock (InternalSyncObject)
			{
				if (_outputEncoding != null)
				{
					return _outputEncoding;
				}
				uint consoleOutputCP = Win32Native.GetConsoleOutputCP();
				_outputEncoding = Encoding.GetEncoding((int)consoleOutputCP);
				return _outputEncoding;
			}
		}
		[SecuritySafeCritical]
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			lock (InternalSyncObject)
			{
				if (_out != null && !_isOutTextWriterRedirected)
				{
					_out.Flush();
					_out = null;
				}
				if (_error != null && !_isErrorTextWriterRedirected)
				{
					_error.Flush();
					_error = null;
				}
				if (!IsStandardConsoleUnicodeEncoding(value))
				{
					uint codePage = (uint)value.CodePage;
					if (!Win32Native.SetConsoleOutputCP(codePage))
					{
						__Error.WinIOError();
					}
				}
				_outputEncoding = (Encoding)value.Clone();
			}
		}
	}

	/// <summary>Ruft die Hintergrundfarbe der Konsole ab oder legt diese fest.</summary>
	/// <returns>Eine <see cref="T:System.ConsoleColor" />, die die Hintergrundfarbe der Konsole, d. h. die hinter jedem Zeichen angezeigte Farbe angibt.Die Standardeinstellung ist schwarz.</returns>
	/// <exception cref="T:System.ArgumentException">Die in einer Set-Operation angegebene Farbe ist kein gültiger Member von <see cref="T:System.ConsoleColor" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static ConsoleColor BackgroundColor
	{
		[SecuritySafeCritical]
		get
		{
			bool succeeded;
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo(throwOnNoConsole: false, out succeeded);
			if (!succeeded)
			{
				return ConsoleColor.Black;
			}
			Win32Native.Color c = (Win32Native.Color)(bufferInfo.wAttributes & 0xF0);
			return ColorAttributeToConsoleColor(c);
		}
		[SecuritySafeCritical]
		set
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			Win32Native.Color color = ConsoleColorToColorAttribute(value, isBackground: true);
			bool succeeded;
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo(throwOnNoConsole: false, out succeeded);
			if (succeeded)
			{
				short wAttributes = bufferInfo.wAttributes;
				wAttributes = (short)(wAttributes & -241);
				wAttributes = (short)((ushort)wAttributes | (ushort)color);
				Win32Native.SetConsoleTextAttribute(ConsoleOutputHandle, wAttributes);
			}
		}
	}

	/// <summary>Ruft die Vordergrundfarbe der Konsole ab oder legt diese fest.</summary>
	/// <returns>Eine <see cref="T:System.ConsoleColor" />, die die Vordergrundfarbe der Konsole angibt, d. h. die Farbe, in der alle Zeichen angezeigt werden.Die Standardeinstellung ist grau.</returns>
	/// <exception cref="T:System.ArgumentException">Die in einer Set-Operation angegebene Farbe ist kein gültiger Member von <see cref="T:System.ConsoleColor" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static ConsoleColor ForegroundColor
	{
		[SecuritySafeCritical]
		get
		{
			bool succeeded;
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo(throwOnNoConsole: false, out succeeded);
			if (!succeeded)
			{
				return ConsoleColor.Gray;
			}
			Win32Native.Color c = (Win32Native.Color)(bufferInfo.wAttributes & 0xF);
			return ColorAttributeToConsoleColor(c);
		}
		[SecuritySafeCritical]
		set
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			Win32Native.Color color = ConsoleColorToColorAttribute(value, isBackground: false);
			bool succeeded;
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo(throwOnNoConsole: false, out succeeded);
			if (succeeded)
			{
				short wAttributes = bufferInfo.wAttributes;
				wAttributes = (short)(wAttributes & -16);
				wAttributes = (short)((ushort)wAttributes | (ushort)color);
				Win32Native.SetConsoleTextAttribute(ConsoleOutputHandle, wAttributes);
			}
		}
	}

	/// <summary>Ruft die Höhe des Pufferbereichs ab oder legt diese fest.</summary>
	/// <returns>Die aktuelle Höhe des Pufferbereichs in Zeilen.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der für eine Set-Operation angegebene Wert ist kleiner oder gleich 0 (null).- oder - Der für eine Set-Operation angegebene Wert ist größer oder gleich <see cref="F:System.Int16.MaxValue" />.- oder - Der Wert in einer Set-Operation ist kleiner als <see cref="P:System.Console.WindowTop" /> + <see cref="P:System.Console.WindowHeight" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	public static int BufferHeight
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().dwSize.Y;
		}
		set
		{
			SetBufferSize(BufferWidth, value);
		}
	}

	/// <summary>Ruft die Breite des Pufferbereichs ab oder legt diese fest.</summary>
	/// <returns>Die aktuelle Breite des Pufferbereichs in Spalten.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der für eine Set-Operation angegebene Wert ist kleiner oder gleich 0 (null).- oder - Der für eine Set-Operation angegebene Wert ist größer oder gleich <see cref="F:System.Int16.MaxValue" />.- oder - Der Wert in einer Set-Operation ist kleiner als <see cref="P:System.Console.WindowLeft" /> + <see cref="P:System.Console.WindowWidth" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	public static int BufferWidth
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().dwSize.X;
		}
		set
		{
			SetBufferSize(value, BufferHeight);
		}
	}

	/// <summary>Ruft die Höhe des Konsolenfensterbereichs ab oder legt diese fest.</summary>
	/// <returns>Die Höhe des Konsolenfensters in Spalten.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der Wert der <see cref="P:System.Console.WindowWidth" />-Eigenschaft oder der Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft ist kleiner oder gleich 0.- oder -Die Summe aus dem Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft und dem Wert der <see cref="P:System.Console.WindowTop" />-Eigenschaft ist größer oder gleich <see cref="F:System.Int16.MaxValue" />.- oder -Der Wert der <see cref="P:System.Console.WindowWidth" />-Eigenschaft oder der Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft ist größer als die maximale Fensterbreite oder -höhe für die aktuelle Bildschirmauflösung und Konsolenschriftart.</exception>
	/// <exception cref="T:System.IO.IOException">Fehler beim Lesen oder Schreiben von Informationen.</exception>
	/// <filterpriority>1</filterpriority>
	public static int WindowHeight
	{
		[SecuritySafeCritical]
		get
		{
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
			return bufferInfo.srWindow.Bottom - bufferInfo.srWindow.Top + 1;
		}
		set
		{
			SetWindowSize(WindowWidth, value);
		}
	}

	/// <summary>Ruft die Breite des Konsolenfensters ab oder legt diese fest.</summary>
	/// <returns>Die Breite des Konsolenfensters in Spalten.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der Wert der <see cref="P:System.Console.WindowWidth" />-Eigenschaft oder der Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft ist kleiner oder gleich 0.- oder -Die Summe aus dem Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft und dem Wert der <see cref="P:System.Console.WindowTop" />-Eigenschaft ist größer oder gleich <see cref="F:System.Int16.MaxValue" />.- oder -Der Wert der <see cref="P:System.Console.WindowWidth" />-Eigenschaft oder der Wert der <see cref="P:System.Console.WindowHeight" />-Eigenschaft ist größer als die maximale Fensterbreite oder -höhe für die aktuelle Bildschirmauflösung und Konsolenschriftart.</exception>
	/// <exception cref="T:System.IO.IOException">Fehler beim Lesen oder Schreiben von Informationen.</exception>
	/// <filterpriority>1</filterpriority>
	public static int WindowWidth
	{
		[SecuritySafeCritical]
		get
		{
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
			return bufferInfo.srWindow.Right - bufferInfo.srWindow.Left + 1;
		}
		set
		{
			SetWindowSize(value, WindowHeight);
		}
	}

	/// <summary>Ruft die entsprechend der aktuellen Schriftart und Bildschirmauflösung größtmögliche Anzahl von Konsolenfensterspalten ab.</summary>
	/// <returns>Die Breite des größtmöglichen Konsolenfensters in Spalten.</returns>
	/// <filterpriority>1</filterpriority>
	public static int LargestWindowWidth
	{
		[SecuritySafeCritical]
		get
		{
			return Win32Native.GetLargestConsoleWindowSize(ConsoleOutputHandle).X;
		}
	}

	/// <summary>Ruft die entsprechend der aktuellen Schriftart und Bildschirmauflösung größtmögliche Anzahl von Konsolenfensterzeilen ab.</summary>
	/// <returns>Die Höhe des größtmöglichen Konsolenfensters in Spalten.</returns>
	/// <filterpriority>1</filterpriority>
	public static int LargestWindowHeight
	{
		[SecuritySafeCritical]
		get
		{
			return Win32Native.GetLargestConsoleWindowSize(ConsoleOutputHandle).Y;
		}
	}

	/// <summary>Ruft die am weitesten links stehende Position des Konsolenfensterbereich im Verhältnis zum Bildschirmpuffer ab oder legt diese fest.</summary>
	/// <returns>Die am weitesten links stehende Konsolenfensterposition in Spalten.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der in einem Set-Vorgang zuzuweisende Wert ist kleiner als 0 (null).- oder -Als Ergebnis der Zuweisung würde <see cref="P:System.Console.WindowLeft" /> plus <see cref="P:System.Console.WindowWidth" /><see cref="P:System.Console.BufferWidth" /> überschreiten. </exception>
	/// <exception cref="T:System.IO.IOException">Fehler beim Lesen oder Schreiben von Informationen.</exception>
	/// <filterpriority>1</filterpriority>
	public static int WindowLeft
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().srWindow.Left;
		}
		set
		{
			SetWindowPosition(value, WindowTop);
		}
	}

	/// <summary>Ruft die oberste Position des Konsolenfensterbereich im Verhältnis zum Bildschirmpuffer ab oder legt diese fest.</summary>
	/// <returns>Die oberste Konsolenfensterposition in Zeilen.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der in einem Set-Vorgang zuzuweisende Wert ist kleiner als 0 (null).- oder -Als Ergebnis der Zuweisung würde <see cref="P:System.Console.WindowTop" /> plus <see cref="P:System.Console.WindowHeight" /><see cref="P:System.Console.BufferHeight" /> überschreiten.</exception>
	/// <exception cref="T:System.IO.IOException">Fehler beim Lesen oder Schreiben von Informationen.</exception>
	/// <filterpriority>1</filterpriority>
	public static int WindowTop
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().srWindow.Top;
		}
		set
		{
			SetWindowPosition(WindowLeft, value);
		}
	}

	/// <summary>Ruft die Spaltenposition des Cursors im Pufferbereich ab oder legt diese fest.</summary>
	/// <returns>Die aktuelle Position des Cursors in Spalten.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der Wert in einer Set-Operation ist kleiner als 0 (null).- oder - Der für eine Set-Operation angegebene Wert ist größer oder gleich <see cref="P:System.Console.BufferWidth" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	public static int CursorLeft
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().dwCursorPosition.X;
		}
		set
		{
			SetCursorPosition(value, CursorTop);
		}
	}

	/// <summary>Ruft die Zeilenposition des Cursors im Pufferbereich ab oder legt diese fest.</summary>
	/// <returns>Die aktuelle Position des Cursors in Zeilen.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der Wert in einer Set-Operation ist kleiner als 0 (null).- oder - Der für eine Set-Operation angegebene Wert ist größer oder gleich <see cref="P:System.Console.BufferHeight" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	public static int CursorTop
	{
		[SecuritySafeCritical]
		get
		{
			return GetBufferInfo().dwCursorPosition.Y;
		}
		set
		{
			SetCursorPosition(CursorLeft, value);
		}
	}

	/// <summary>Ruft die Höhe des Cursors innerhalb einer Zeichenzelle ab oder legt diese fest.</summary>
	/// <returns>Die Größe des Cursors in Prozent der Höhe einer Zeichenzelle.Der Eigenschaftswert liegt zwischen 1 und 100.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der in einer Set-Operation angegebene Wert ist kleiner als 1 oder größer als 100. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static int CursorSize
	{
		[SecuritySafeCritical]
		get
		{
			IntPtr consoleOutputHandle = ConsoleOutputHandle;
			if (!Win32Native.GetConsoleCursorInfo(consoleOutputHandle, out var cci))
			{
				__Error.WinIOError();
			}
			return cci.dwSize;
		}
		[SecuritySafeCritical]
		set
		{
			if (value < 1 || value > 100)
			{
				throw new ArgumentOutOfRangeException("value", value, Environment.GetResourceString("ArgumentOutOfRange_CursorSize"));
			}
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			IntPtr consoleOutputHandle = ConsoleOutputHandle;
			if (!Win32Native.GetConsoleCursorInfo(consoleOutputHandle, out var cci))
			{
				__Error.WinIOError();
			}
			cci.dwSize = value;
			if (!Win32Native.SetConsoleCursorInfo(consoleOutputHandle, ref cci))
			{
				__Error.WinIOError();
			}
		}
	}

	/// <summary>Ruft einen Wert ab, der angibt, ob der Cursor sichtbar ist, oder legt diesen fest.</summary>
	/// <returns>true, wenn der Cursor sichtbar ist, andernfalls false.</returns>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static bool CursorVisible
	{
		[SecuritySafeCritical]
		get
		{
			IntPtr consoleOutputHandle = ConsoleOutputHandle;
			if (!Win32Native.GetConsoleCursorInfo(consoleOutputHandle, out var cci))
			{
				__Error.WinIOError();
			}
			return cci.bVisible;
		}
		[SecuritySafeCritical]
		set
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			IntPtr consoleOutputHandle = ConsoleOutputHandle;
			if (!Win32Native.GetConsoleCursorInfo(consoleOutputHandle, out var cci))
			{
				__Error.WinIOError();
			}
			cci.bVisible = value;
			if (!Win32Native.SetConsoleCursorInfo(consoleOutputHandle, ref cci))
			{
				__Error.WinIOError();
			}
		}
	}

	/// <summary>Ruft den auf der Konsolentitelleiste anzuzeigenden Titel ab oder legt diesen fest.</summary>
	/// <returns>Die Zeichenfolge, die auf der Titelleiste der Konsole angezeigt werden soll.Die maximale Länge der Titelzeichenfolge beträgt 24500 Zeichen.</returns>
	/// <exception cref="T:System.InvalidOperationException">Der in einer Get-Operation abgerufene Titel ist länger als 24500 Zeichen. </exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Der in einer Set-Operation angegebene Titel ist länger als 24500 Zeichen. </exception>
	/// <exception cref="T:System.ArgumentNullException">Der in einer Set-Operation angegebene Titel ist null. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static string Title
	{
		[SecuritySafeCritical]
		get
		{
			string s = null;
			int outTitleLength = -1;
			int titleNative = GetTitleNative(JitHelpers.GetStringHandleOnStack(ref s), out outTitleLength);
			if (titleNative != 0)
			{
				__Error.WinIOError(titleNative, string.Empty);
			}
			if (outTitleLength > 24500)
			{
				throw new InvalidOperationException(Environment.GetResourceString("ArgumentOutOfRange_ConsoleTitleTooLong"));
			}
			return s;
		}
		[SecuritySafeCritical]
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length > 24500)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_ConsoleTitleTooLong"));
			}
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			if (!Win32Native.SetConsoleTitle(value))
			{
				__Error.WinIOError();
			}
		}
	}

	/// <summary>Ruft einen Wert ab, der angibt, ob ein Tastendruck im Eingabestream vorhanden ist.</summary>
	/// <returns>true, wenn ein Tastendruck vorhanden ist, andernfalls false.</returns>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.InvalidOperationException">Die Standardeingabe wird statt an die Tastatur in eine Datei umgeleitet. </exception>
	/// <filterpriority>1</filterpriority>
	public static bool KeyAvailable
	{
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, UI = true)]
		get
		{
			if (_cachedInputRecord.eventType == 1)
			{
				return true;
			}
			Win32Native.InputRecord buffer = default(Win32Native.InputRecord);
			int numEventsRead = 0;
			while (true)
			{
				if (!Win32Native.PeekConsoleInput(ConsoleInputHandle, out buffer, 1, out numEventsRead))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 6)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ConsoleKeyAvailableOnFile"));
					}
					__Error.WinIOError(lastWin32Error, "stdin");
				}
				if (numEventsRead == 0)
				{
					return false;
				}
				if (IsKeyDownEvent(buffer) && !IsModKey(buffer))
				{
					break;
				}
				if (!Win32Native.ReadConsoleInput(ConsoleInputHandle, out buffer, 1, out numEventsRead))
				{
					__Error.WinIOError();
				}
			}
			return true;
		}
	}

	/// <summary>Ruft einen Wert ab, der angibt, ob die NUM-Tastaturumschalttaste aktiviert oder deaktiviert ist.</summary>
	/// <returns>true, wenn die NUM-TASTE aktiviert ist, false, wenn die NUM-TASTE deaktiviert ist.</returns>
	/// <filterpriority>1</filterpriority>
	public static bool NumberLock
	{
		[SecuritySafeCritical]
		get
		{
			short keyState = Win32Native.GetKeyState(144);
			return (keyState & 1) == 1;
		}
	}

	/// <summary>Ruft einen Wert ab, der angibt, ob die FESTSTELLTASTE-Tastaturumschalttaste aktiviert oder deaktiviert ist.</summary>
	/// <returns>true, wenn die FESTSTELLTASTE aktiviert ist, false, wenn die FESTSTELLTASTE deaktiviert ist.</returns>
	/// <filterpriority>1</filterpriority>
	public static bool CapsLock
	{
		[SecuritySafeCritical]
		get
		{
			short keyState = Win32Native.GetKeyState(20);
			return (keyState & 1) == 1;
		}
	}

	/// <summary>Ruft einen Wert ab oder legt diesen fest, der angibt, ob die Kombination der <see cref="F:System.ConsoleModifiers.Control" />-Modifizierertaste und der <see cref="F:System.ConsoleKey.C" />-Konsolentaste (STRG+C) als normale Eingabe oder als vom Betriebssystem zu behandelnde Unterbrechung behandelt wird.</summary>
	/// <returns>true, wenn STRG+C als normale Eingabe behandelt wird, andernfalls false.</returns>
	/// <exception cref="T:System.IO.IOException">Der Eingabemodus des Konsoleneingabepuffers kann nicht abgerufen oder festgelegt werden. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static bool TreatControlCAsInput
	{
		[SecuritySafeCritical]
		get
		{
			IntPtr consoleInputHandle = ConsoleInputHandle;
			if (consoleInputHandle == Win32Native.INVALID_HANDLE_VALUE)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
			}
			int mode = 0;
			if (!Win32Native.GetConsoleMode(consoleInputHandle, out mode))
			{
				__Error.WinIOError();
			}
			return (mode & 1) == 0;
		}
		[SecuritySafeCritical]
		set
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			IntPtr consoleInputHandle = ConsoleInputHandle;
			if (consoleInputHandle == Win32Native.INVALID_HANDLE_VALUE)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
			}
			int mode = 0;
			bool consoleMode = Win32Native.GetConsoleMode(consoleInputHandle, out mode);
			mode = ((!value) ? (mode | 1) : (mode & -2));
			if (!Win32Native.SetConsoleMode(consoleInputHandle, mode))
			{
				__Error.WinIOError();
			}
		}
	}

	/// <summary>Tritt ein, wenn die <see cref="F:System.ConsoleModifiers.Control" />-Modifizierertaste (STRG) und die <see cref="F:System.ConsoleKey.C" />-Konsolentaste (C) gleichzeitig gedrückt werden (STRG+C).</summary>
	/// <filterpriority>1</filterpriority>
	public static event ConsoleCancelEventHandler CancelKeyPress
	{
		[SecuritySafeCritical]
		add
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			lock (InternalSyncObject)
			{
				_cancelCallbacks = (ConsoleCancelEventHandler)Delegate.Combine(_cancelCallbacks, value);
				if (_hooker == null)
				{
					_hooker = new ControlCHooker();
					_hooker.Hook();
				}
			}
		}
		[SecuritySafeCritical]
		remove
		{
			new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
			lock (InternalSyncObject)
			{
				_cancelCallbacks = (ConsoleCancelEventHandler)Delegate.Remove(_cancelCallbacks, value);
				if (_hooker != null && _cancelCallbacks == null)
				{
					_hooker.Unhook();
				}
			}
		}
	}

	[SecuritySafeCritical]
	private static bool IsHandleRedirected(IntPtr ioHandle)
	{
		SafeFileHandle handle = new SafeFileHandle(ioHandle, ownsHandle: false);
		int fileType = Win32Native.GetFileType(handle);
		if ((fileType & 2) != 2)
		{
			return true;
		}
		int mode;
		bool consoleMode = Win32Native.GetConsoleMode(ioHandle, out mode);
		return !consoleMode;
	}

	[SecuritySafeCritical]
	private static void InitializeStdOutError(bool stdout)
	{
		lock (InternalSyncObject)
		{
			if ((!stdout || _out == null) && (stdout || _error == null))
			{
				TextWriter textWriter = null;
				Stream stream = ((!stdout) ? OpenStandardError(256) : OpenStandardOutput(256));
				if (stream == Stream.Null)
				{
					textWriter = TextWriter.Synchronized(StreamWriter.Null);
				}
				else
				{
					Encoding outputEncoding = OutputEncoding;
					StreamWriter streamWriter = new StreamWriter(stream, outputEncoding, 256, leaveOpen: true);
					streamWriter.HaveWrittenPreamble = true;
					streamWriter.AutoFlush = true;
					textWriter = TextWriter.Synchronized(streamWriter);
				}
				if (stdout)
				{
					_out = textWriter;
				}
				else
				{
					_error = textWriter;
				}
			}
		}
	}

	private static bool IsStandardConsoleUnicodeEncoding(Encoding encoding)
	{
		UnicodeEncoding unicodeEncoding = encoding as UnicodeEncoding;
		if (unicodeEncoding == null)
		{
			return false;
		}
		if (StdConUnicodeEncoding.CodePage == unicodeEncoding.CodePage)
		{
			return StdConUnicodeEncoding.bigEndian == unicodeEncoding.bigEndian;
		}
		return false;
	}

	private static bool GetUseFileAPIs(int handleType)
	{
		switch (handleType)
		{
		case -10:
			if (IsStandardConsoleUnicodeEncoding(InputEncoding))
			{
				return IsInputRedirected;
			}
			return true;
		case -11:
			if (IsStandardConsoleUnicodeEncoding(OutputEncoding))
			{
				return IsOutputRedirected;
			}
			return true;
		case -12:
			if (IsStandardConsoleUnicodeEncoding(OutputEncoding))
			{
				return IsErrorRedirected;
			}
			return true;
		default:
			return true;
		}
	}

	[SecuritySafeCritical]
	private static Stream GetStandardFile(int stdHandleName, FileAccess access, int bufferSize)
	{
		IntPtr stdHandle = Win32Native.GetStdHandle(stdHandleName);
		SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, ownsHandle: false);
		if (safeFileHandle.IsInvalid)
		{
			safeFileHandle.SetHandleAsInvalid();
			return Stream.Null;
		}
		if (stdHandleName != -10 && !ConsoleHandleIsWritable(safeFileHandle))
		{
			return Stream.Null;
		}
		bool useFileAPIs = GetUseFileAPIs(stdHandleName);
		return new __ConsoleStream(safeFileHandle, access, useFileAPIs);
	}

	[SecuritySafeCritical]
	private unsafe static bool ConsoleHandleIsWritable(SafeFileHandle outErrHandle)
	{
		byte b = 65;
		int numBytesWritten;
		int num = Win32Native.WriteFile(outErrHandle, &b, 0, out numBytesWritten, IntPtr.Zero);
		return num != 0;
	}

	/// <summary>Gibt den Sound eines Signaltons auf dem Konsolenlautsprecher wieder.</summary>
	/// <exception cref="T:System.Security.HostProtectionException">Diese Methode wurde auf einem Server wie SQL Server ausgeführt, der keinen Zugriff auf eine Benutzeroberfläche gestattet.</exception>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Beep()
	{
		Beep(800, 200);
	}

	/// <summary>Gibt den Sound eines Signaltons mit einer angegebenen Frequenz und Dauer auf dem Konsolenlautsprecher wieder.</summary>
	/// <param name="frequency">Die Frequenz des Signaltons zwischen 37 und 32767 Hertz.</param>
	/// <param name="duration">Die Dauer des Signaltons in Millisekunden.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="frequency" /> ist kleiner als 37 oder größer als 32767 Hertz.- oder -<paramref name="duration" /> ist kleiner oder gleich 0 (null).</exception>
	/// <exception cref="T:System.Security.HostProtectionException">Diese Methode wurde auf einem Server wie SQL Server ausgeführt, der keinen Zugriff auf die Konsole gestattet.</exception>
	/// <filterpriority>1</filterpriority>
	[SecuritySafeCritical]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Beep(int frequency, int duration)
	{
		if (frequency < 37 || frequency > 32767)
		{
			throw new ArgumentOutOfRangeException("frequency", frequency, Environment.GetResourceString("ArgumentOutOfRange_BeepFrequency", 37, 32767));
		}
		if (duration <= 0)
		{
			throw new ArgumentOutOfRangeException("duration", duration, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
		}
		Win32Native.Beep(frequency, duration);
	}

	/// <summary>Löscht die Anzeigeinformationen aus dem Konsolenpuffer und dem entsprechenden Konsolenfenster.</summary>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	[SecuritySafeCritical]
	public static void Clear()
	{
		Win32Native.COORD cOORD = default(Win32Native.COORD);
		IntPtr consoleOutputHandle = ConsoleOutputHandle;
		if (consoleOutputHandle == Win32Native.INVALID_HANDLE_VALUE)
		{
			throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
		}
		Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
		int num = bufferInfo.dwSize.X * bufferInfo.dwSize.Y;
		int pNumCharsWritten = 0;
		if (!Win32Native.FillConsoleOutputCharacter(consoleOutputHandle, ' ', num, cOORD, out pNumCharsWritten))
		{
			__Error.WinIOError();
		}
		pNumCharsWritten = 0;
		if (!Win32Native.FillConsoleOutputAttribute(consoleOutputHandle, bufferInfo.wAttributes, num, cOORD, out pNumCharsWritten))
		{
			__Error.WinIOError();
		}
		if (!Win32Native.SetConsoleCursorPosition(consoleOutputHandle, cOORD))
		{
			__Error.WinIOError();
		}
	}

	[SecurityCritical]
	private static Win32Native.Color ConsoleColorToColorAttribute(ConsoleColor color, bool isBackground)
	{
		if (((uint)color & 0xFFFFFFF0u) != 0)
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_InvalidConsoleColor"));
		}
		Win32Native.Color color2 = (Win32Native.Color)color;
		if (isBackground)
		{
			color2 = (Win32Native.Color)((int)color2 << 4);
		}
		return color2;
	}

	[SecurityCritical]
	private static ConsoleColor ColorAttributeToConsoleColor(Win32Native.Color c)
	{
		if ((c & Win32Native.Color.BackgroundMask) != 0)
		{
			c = (Win32Native.Color)((int)c >> 4);
		}
		return (ConsoleColor)c;
	}

	/// <summary>Legt die Vordergrund- und Hintergrundkonsolenfarben auf die entsprechenden Standardwerte fest.</summary>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public static void ResetColor()
	{
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		bool succeeded;
		Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo(throwOnNoConsole: false, out succeeded);
		if (succeeded)
		{
			short attributes = _defaultColors;
			Win32Native.SetConsoleTextAttribute(ConsoleOutputHandle, attributes);
		}
	}

	/// <summary>Kopiert einen angegebenen Quellbereich des Bildschirmpuffers in einen angegebenen Zielbereich.</summary>
	/// <param name="sourceLeft">Die am weitesten links stehende Spalte des Quellbereichs. </param>
	/// <param name="sourceTop">Die oberste Zeile des Quellbereichs. </param>
	/// <param name="sourceWidth">Die Anzahl der Spalten im Quellbereich. </param>
	/// <param name="sourceHeight">Die Anzahl der Zeilen im Quellbereich. </param>
	/// <param name="targetLeft">Die am weitesten links stehende Spalte des Zielbereichs. </param>
	/// <param name="targetTop">Die oberste Zeile des Zielbereichs. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Mindestens einer der angegebenen Parameter ist kleiner als 0 (null).- oder - <paramref name="sourceLeft" /> oder <paramref name="targetLeft" /> ist größer oder gleich <see cref="P:System.Console.BufferWidth" />.- oder - <paramref name="sourceTop" /> oder <paramref name="targetTop" /> ist größer oder gleich <see cref="P:System.Console.BufferHeight" />.- oder - <paramref name="sourceTop" /> + <paramref name="sourceHeight" /> ist größer oder gleich <see cref="P:System.Console.BufferHeight" />.- oder - <paramref name="sourceLeft" /> + <paramref name="sourceWidth" /> ist größer oder gleich <see cref="P:System.Console.BufferWidth" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
	{
		MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, ' ', ConsoleColor.Black, BackgroundColor);
	}

	/// <summary>Kopiert einen angegebenen Quellbereich des Bildschirmpuffers in einen angegebenen Zielbereich.</summary>
	/// <param name="sourceLeft">Die am weitesten links stehende Spalte des Quellbereichs. </param>
	/// <param name="sourceTop">Die oberste Zeile des Quellbereichs. </param>
	/// <param name="sourceWidth">Die Anzahl der Spalten im Quellbereich. </param>
	/// <param name="sourceHeight">Die Anzahl der Zeilen im Quellbereich. </param>
	/// <param name="targetLeft">Die am weitesten links stehende Spalte des Zielbereichs. </param>
	/// <param name="targetTop">Die oberste Zeile des Zielbereichs. </param>
	/// <param name="sourceChar">Das zum Ausfüllen des Quellbereichs verwendete Zeichen. </param>
	/// <param name="sourceForeColor">Die zum Ausfüllen des Quellbereichs verwendete Vordergrundfarbe. </param>
	/// <param name="sourceBackColor">Die zum Ausfüllen des Quellbereichs verwendete Hintergrundfarbe. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Mindestens einer der angegebenen Parameter ist kleiner als 0 (null).- oder - <paramref name="sourceLeft" /> oder <paramref name="targetLeft" /> ist größer oder gleich <see cref="P:System.Console.BufferWidth" />.- oder - <paramref name="sourceTop" /> oder <paramref name="targetTop" /> ist größer oder gleich <see cref="P:System.Console.BufferHeight" />.- oder - <paramref name="sourceTop" /> + <paramref name="sourceHeight" /> ist größer oder gleich <see cref="P:System.Console.BufferHeight" />.- oder - <paramref name="sourceLeft" /> + <paramref name="sourceWidth" /> ist größer oder gleich <see cref="P:System.Console.BufferWidth" />. </exception>
	/// <exception cref="T:System.ArgumentException">Einer oder beide der Farbparameter ist kein Member der <see cref="T:System.ConsoleColor" />-Enumeration. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public unsafe static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
	{
		if (sourceForeColor < ConsoleColor.Black || sourceForeColor > ConsoleColor.White)
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_InvalidConsoleColor"), "sourceForeColor");
		}
		if (sourceBackColor < ConsoleColor.Black || sourceBackColor > ConsoleColor.White)
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_InvalidConsoleColor"), "sourceBackColor");
		}
		Win32Native.COORD dwSize = GetBufferInfo().dwSize;
		if (sourceLeft < 0 || sourceLeft > dwSize.X)
		{
			throw new ArgumentOutOfRangeException("sourceLeft", sourceLeft, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (sourceTop < 0 || sourceTop > dwSize.Y)
		{
			throw new ArgumentOutOfRangeException("sourceTop", sourceTop, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (sourceWidth < 0 || sourceWidth > dwSize.X - sourceLeft)
		{
			throw new ArgumentOutOfRangeException("sourceWidth", sourceWidth, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (sourceHeight < 0 || sourceTop > dwSize.Y - sourceHeight)
		{
			throw new ArgumentOutOfRangeException("sourceHeight", sourceHeight, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (targetLeft < 0 || targetLeft > dwSize.X)
		{
			throw new ArgumentOutOfRangeException("targetLeft", targetLeft, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (targetTop < 0 || targetTop > dwSize.Y)
		{
			throw new ArgumentOutOfRangeException("targetTop", targetTop, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (sourceWidth == 0 || sourceHeight == 0)
		{
			return;
		}
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		Win32Native.CHAR_INFO[] array = new Win32Native.CHAR_INFO[sourceWidth * sourceHeight];
		dwSize.X = (short)sourceWidth;
		dwSize.Y = (short)sourceHeight;
		Win32Native.COORD bufferCoord = default(Win32Native.COORD);
		Win32Native.SMALL_RECT readRegion = default(Win32Native.SMALL_RECT);
		readRegion.Left = (short)sourceLeft;
		readRegion.Right = (short)(sourceLeft + sourceWidth - 1);
		readRegion.Top = (short)sourceTop;
		readRegion.Bottom = (short)(sourceTop + sourceHeight - 1);
		bool flag;
		fixed (Win32Native.CHAR_INFO* pBuffer = array)
		{
			flag = Win32Native.ReadConsoleOutput(ConsoleOutputHandle, pBuffer, dwSize, bufferCoord, ref readRegion);
		}
		if (!flag)
		{
			__Error.WinIOError();
		}
		Win32Native.COORD cOORD = default(Win32Native.COORD);
		cOORD.X = (short)sourceLeft;
		Win32Native.Color color = ConsoleColorToColorAttribute(sourceBackColor, isBackground: true);
		color |= ConsoleColorToColorAttribute(sourceForeColor, isBackground: false);
		short wColorAttribute = (short)color;
		for (int i = sourceTop; i < sourceTop + sourceHeight; i++)
		{
			cOORD.Y = (short)i;
			if (!Win32Native.FillConsoleOutputCharacter(ConsoleOutputHandle, sourceChar, sourceWidth, cOORD, out var pNumCharsWritten))
			{
				__Error.WinIOError();
			}
			if (!Win32Native.FillConsoleOutputAttribute(ConsoleOutputHandle, wColorAttribute, sourceWidth, cOORD, out pNumCharsWritten))
			{
				__Error.WinIOError();
			}
		}
		Win32Native.SMALL_RECT writeRegion = default(Win32Native.SMALL_RECT);
		writeRegion.Left = (short)targetLeft;
		writeRegion.Right = (short)(targetLeft + sourceWidth);
		writeRegion.Top = (short)targetTop;
		writeRegion.Bottom = (short)(targetTop + sourceHeight);
		fixed (Win32Native.CHAR_INFO* buffer = array)
		{
			flag = Win32Native.WriteConsoleOutput(ConsoleOutputHandle, buffer, dwSize, bufferCoord, ref writeRegion);
		}
	}

	[SecurityCritical]
	private static Win32Native.CONSOLE_SCREEN_BUFFER_INFO GetBufferInfo()
	{
		bool succeeded;
		return GetBufferInfo(throwOnNoConsole: true, out succeeded);
	}

	[SecuritySafeCritical]
	private static Win32Native.CONSOLE_SCREEN_BUFFER_INFO GetBufferInfo(bool throwOnNoConsole, out bool succeeded)
	{
		succeeded = false;
		IntPtr consoleOutputHandle = ConsoleOutputHandle;
		if (consoleOutputHandle == Win32Native.INVALID_HANDLE_VALUE)
		{
			if (!throwOnNoConsole)
			{
				return default(Win32Native.CONSOLE_SCREEN_BUFFER_INFO);
			}
			throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
		}
		if (!Win32Native.GetConsoleScreenBufferInfo(consoleOutputHandle, out var lpConsoleScreenBufferInfo))
		{
			bool consoleScreenBufferInfo = Win32Native.GetConsoleScreenBufferInfo(Win32Native.GetStdHandle(-12), out lpConsoleScreenBufferInfo);
			if (!consoleScreenBufferInfo)
			{
				consoleScreenBufferInfo = Win32Native.GetConsoleScreenBufferInfo(Win32Native.GetStdHandle(-10), out lpConsoleScreenBufferInfo);
			}
			if (!consoleScreenBufferInfo)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 6 && !throwOnNoConsole)
				{
					return default(Win32Native.CONSOLE_SCREEN_BUFFER_INFO);
				}
				__Error.WinIOError(lastWin32Error, null);
			}
		}
		if (!_haveReadDefaultColors)
		{
			_defaultColors = (byte)((uint)lpConsoleScreenBufferInfo.wAttributes & 0xFFu);
			_haveReadDefaultColors = true;
		}
		succeeded = true;
		return lpConsoleScreenBufferInfo;
	}

	/// <summary>Legt die Höhe und die Breite des Bildschirmpufferbereichs auf die angegebenen Werte fest.</summary>
	/// <param name="width">Die Breite des Pufferbereichs in Spalten. </param>
	/// <param name="height">Die Höhe des Pufferbereichs in Zeilen. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="height" /> oder <paramref name="width" /> ist kleiner oder gleich 0 (null).- oder - <paramref name="height" /> oder <paramref name="width" /> ist größer oder gleich <see cref="F:System.Int16.MaxValue" />.- oder - <paramref name="width" /> ist kleiner als <see cref="P:System.Console.WindowLeft" /> + <see cref="P:System.Console.WindowWidth" />.- oder - <paramref name="height" /> ist kleiner als <see cref="P:System.Console.WindowTop" /> + <see cref="P:System.Console.WindowHeight" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public static void SetBufferSize(int width, int height)
	{
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		Win32Native.SMALL_RECT srWindow = GetBufferInfo().srWindow;
		if (width < srWindow.Right + 1 || width >= 32767)
		{
			throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferLessThanWindowSize"));
		}
		if (height < srWindow.Bottom + 1 || height >= 32767)
		{
			throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferLessThanWindowSize"));
		}
		Win32Native.COORD size = default(Win32Native.COORD);
		size.X = (short)width;
		size.Y = (short)height;
		if (!Win32Native.SetConsoleScreenBufferSize(ConsoleOutputHandle, size))
		{
			__Error.WinIOError();
		}
	}

	/// <summary>Legt die Höhe und die Breite des Konsolenfensters auf die angegebenen Werte fest.</summary>
	/// <param name="width">Die Breite des Konsolenfensters in Spalten. </param>
	/// <param name="height">Die Höhe des Konsolenfensters in Spalten. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="width" /> oder <paramref name="height" /> ist kleiner oder gleich 0 (null).- oder - <paramref name="width" /> plus <see cref="P:System.Console.WindowLeft" /> oder <paramref name="height" /> plus <see cref="P:System.Console.WindowTop" /> ist größer oder gleich <see cref="F:System.Int16.MaxValue" />. - oder -<paramref name="width" /> oder <paramref name="height" /> ist größer als die größtmögliche Fensterbreite oder -höhe bei der aktuellen Bildschirmauflösung und Konsolenschriftart.</exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public unsafe static void SetWindowSize(int width, int height)
	{
		if (width <= 0)
		{
			throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
		}
		if (height <= 0)
		{
			throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
		}
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
		bool flag = false;
		Win32Native.COORD size = default(Win32Native.COORD);
		size.X = bufferInfo.dwSize.X;
		size.Y = bufferInfo.dwSize.Y;
		if (bufferInfo.dwSize.X < bufferInfo.srWindow.Left + width)
		{
			if (bufferInfo.srWindow.Left >= 32767 - width)
			{
				throw new ArgumentOutOfRangeException("width", Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowBufferSize"));
			}
			size.X = (short)(bufferInfo.srWindow.Left + width);
			flag = true;
		}
		if (bufferInfo.dwSize.Y < bufferInfo.srWindow.Top + height)
		{
			if (bufferInfo.srWindow.Top >= 32767 - height)
			{
				throw new ArgumentOutOfRangeException("height", Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowBufferSize"));
			}
			size.Y = (short)(bufferInfo.srWindow.Top + height);
			flag = true;
		}
		if (flag && !Win32Native.SetConsoleScreenBufferSize(ConsoleOutputHandle, size))
		{
			__Error.WinIOError();
		}
		Win32Native.SMALL_RECT srWindow = bufferInfo.srWindow;
		srWindow.Bottom = (short)(srWindow.Top + height - 1);
		srWindow.Right = (short)(srWindow.Left + width - 1);
		if (!Win32Native.SetConsoleWindowInfo(ConsoleOutputHandle, absolute: true, &srWindow))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (flag)
			{
				Win32Native.SetConsoleScreenBufferSize(ConsoleOutputHandle, bufferInfo.dwSize);
			}
			Win32Native.COORD largestConsoleWindowSize = Win32Native.GetLargestConsoleWindowSize(ConsoleOutputHandle);
			if (width > largestConsoleWindowSize.X)
			{
				throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowSize_Size", largestConsoleWindowSize.X));
			}
			if (height > largestConsoleWindowSize.Y)
			{
				throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowSize_Size", largestConsoleWindowSize.Y));
			}
			__Error.WinIOError(lastWin32Error, string.Empty);
		}
	}

	/// <summary>Legt die Position des Konsolenfensters relativ zum Bildschirmpuffer fest.</summary>
	/// <param name="left">Die Spaltenposition der linken oberen Ecke des Konsolenfensters. </param>
	/// <param name="top">Die Zeilenposition der linken oberen Ecke des Konsolenfensters. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="left" /> oder <paramref name="top" /> ist kleiner als 0 (null).- oder - <paramref name="left" /> + <see cref="P:System.Console.WindowWidth" /> ist größer als <see cref="P:System.Console.BufferWidth" />.- oder - <paramref name="top" /> + <see cref="P:System.Console.WindowHeight" /> ist größer als <see cref="P:System.Console.BufferHeight" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public unsafe static void SetWindowPosition(int left, int top)
	{
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
		Win32Native.SMALL_RECT srWindow = bufferInfo.srWindow;
		int num = left + srWindow.Right - srWindow.Left + 1;
		if (left < 0 || num > bufferInfo.dwSize.X || num < 0)
		{
			throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowPos"));
		}
		int num2 = top + srWindow.Bottom - srWindow.Top + 1;
		if (top < 0 || num2 > bufferInfo.dwSize.Y || num2 < 0)
		{
			throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowPos"));
		}
		srWindow.Bottom -= (short)(srWindow.Top - top);
		srWindow.Right -= (short)(srWindow.Left - left);
		srWindow.Left = (short)left;
		srWindow.Top = (short)top;
		if (!Win32Native.SetConsoleWindowInfo(ConsoleOutputHandle, absolute: true, &srWindow))
		{
			__Error.WinIOError();
		}
	}

	/// <summary>Legt die Position des Cursors fest.</summary>
	/// <param name="left">Die Spaltenposition des Cursors. </param>
	/// <param name="top">Die Zeilenposition des Cursors. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="left" /> oder <paramref name="top" /> ist kleiner als 0 (null).- oder - <paramref name="left" /> ist größer oder gleich <see cref="P:System.Console.BufferWidth" />.- oder - <paramref name="top" /> ist größer oder gleich <see cref="P:System.Console.BufferHeight" />. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Benutzer besitzt keine Berechtigung zum Ausführen dieser Aktion. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten.</exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="SafeTopLevelWindows" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	public static void SetCursorPosition(int left, int top)
	{
		if (left < 0 || left >= 32767)
		{
			throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		if (top < 0 || top >= 32767)
		{
			throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
		}
		new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
		IntPtr consoleOutputHandle = ConsoleOutputHandle;
		Win32Native.COORD cursorPosition = default(Win32Native.COORD);
		cursorPosition.X = (short)left;
		cursorPosition.Y = (short)top;
		if (!Win32Native.SetConsoleCursorPosition(consoleOutputHandle, cursorPosition))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			Win32Native.CONSOLE_SCREEN_BUFFER_INFO bufferInfo = GetBufferInfo();
			if (left < 0 || left >= bufferInfo.dwSize.X)
			{
				throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
			}
			if (top < 0 || top >= bufferInfo.dwSize.Y)
			{
				throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
			}
			__Error.WinIOError(lastWin32Error, string.Empty);
		}
	}

	[DllImport("QCall", CharSet = CharSet.Ansi)]
	[SecurityCritical]
	[SuppressUnmanagedCodeSecurity]
	private static extern int GetTitleNative(StringHandleOnStack outTitle, out int outTitleLength);

	/// <summary>Ruft die nächste vom Benutzer gedrückte Zeichen- oder Funktionstaste ab.Die gedrückte Taste wird im Konsolenfenster angezeigt.</summary>
	/// <returns>Das <see cref="T:System.ConsoleKeyInfo" />-Objekt, das die <see cref="T:System.ConsoleKey" />-Konstante und ggf. das Unicode-Zeichen beschreibt, die der gedrückten Konsolentaste entsprechen.Das <see cref="T:System.ConsoleKeyInfo" />-Objekt beschreibt außerdem in einer bitweisen Kombination von <see cref="T:System.ConsoleModifiers" />-Werten, ob eine oder mehrere der Modifizierertasten UMSCHALTTASTE, ALT oder STRG gleichzeitig mit der Konsolentaste gedrückt wurden.</returns>
	/// <exception cref="T:System.InvalidOperationException">Die <see cref="P:System.Console.In" />-Eigenschaft wird von einem Stream, der nicht die Konsole ist, umgeleitet.</exception>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static ConsoleKeyInfo ReadKey()
	{
		return ReadKey(intercept: false);
	}

	[SecurityCritical]
	private static bool IsAltKeyDown(Win32Native.InputRecord ir)
	{
		return (ir.keyEvent.controlKeyState & 3) != 0;
	}

	[SecurityCritical]
	private static bool IsKeyDownEvent(Win32Native.InputRecord ir)
	{
		if (ir.eventType == 1)
		{
			return ir.keyEvent.keyDown;
		}
		return false;
	}

	[SecurityCritical]
	private static bool IsModKey(Win32Native.InputRecord ir)
	{
		short virtualKeyCode = ir.keyEvent.virtualKeyCode;
		if ((virtualKeyCode < 16 || virtualKeyCode > 18) && virtualKeyCode != 20 && virtualKeyCode != 144)
		{
			return virtualKeyCode == 145;
		}
		return true;
	}

	/// <summary>Ruft die nächste vom Benutzer gedrückte Zeichen- oder Funktionstaste ab.Die gedrückte Taste wird optional im Konsolenfenster angezeigt.</summary>
	/// <returns>Das <see cref="T:System.ConsoleKeyInfo" />-Objekt, das die <see cref="T:System.ConsoleKey" />-Konstante und ggf. das Unicode-Zeichen beschreibt, die der gedrückten Konsolentaste entsprechen.Das <see cref="T:System.ConsoleKeyInfo" />-Objekt beschreibt außerdem in einer bitweisen Kombination von <see cref="T:System.ConsoleModifiers" />-Werten, ob eine oder mehrere der Modifizierertasten UMSCHALTTASTE, ALT oder STRG gleichzeitig mit der Konsolentaste gedrückt wurden.</returns>
	/// <param name="intercept">Bestimmt, ob die gedrückte Taste im Konsolenfenster angezeigt werden soll.true, wenn die gedrückte Taste nicht angezeigt werden soll, andernfalls false.</param>
	/// <exception cref="T:System.InvalidOperationException">Die <see cref="P:System.Console.In" />-Eigenschaft wird von einem Stream, der nicht die Konsole ist, umgeleitet.</exception>
	/// <filterpriority>1</filterpriority>
	[SecuritySafeCritical]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static ConsoleKeyInfo ReadKey(bool intercept)
	{
		int numEventsRead = -1;
		Win32Native.InputRecord buffer;
		lock (ReadKeySyncObject)
		{
			if (_cachedInputRecord.eventType == 1)
			{
				buffer = _cachedInputRecord;
				if (_cachedInputRecord.keyEvent.repeatCount == 0)
				{
					_cachedInputRecord.eventType = -1;
				}
				else
				{
					_cachedInputRecord.keyEvent.repeatCount--;
				}
			}
			else
			{
				while (true)
				{
					if (!Win32Native.ReadConsoleInput(ConsoleInputHandle, out buffer, 1, out numEventsRead) || numEventsRead == 0)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ConsoleReadKeyOnFile"));
					}
					short virtualKeyCode = buffer.keyEvent.virtualKeyCode;
					if ((!IsKeyDownEvent(buffer) && virtualKeyCode != 18) || (buffer.keyEvent.uChar == '\0' && IsModKey(buffer)))
					{
						continue;
					}
					ConsoleKey consoleKey = (ConsoleKey)virtualKeyCode;
					if (!IsAltKeyDown(buffer))
					{
						break;
					}
					if (consoleKey < ConsoleKey.NumPad0 || consoleKey > ConsoleKey.NumPad9)
					{
						switch (consoleKey)
						{
						case ConsoleKey.Clear:
						case ConsoleKey.PageUp:
						case ConsoleKey.PageDown:
						case ConsoleKey.End:
						case ConsoleKey.Home:
						case ConsoleKey.LeftArrow:
						case ConsoleKey.UpArrow:
						case ConsoleKey.RightArrow:
						case ConsoleKey.DownArrow:
						case ConsoleKey.Insert:
							continue;
						}
						break;
					}
				}
				if (buffer.keyEvent.repeatCount > 1)
				{
					buffer.keyEvent.repeatCount--;
					_cachedInputRecord = buffer;
				}
			}
		}
		ControlKeyState controlKeyState = (ControlKeyState)buffer.keyEvent.controlKeyState;
		bool shift = (controlKeyState & ControlKeyState.ShiftPressed) != 0;
		bool alt = (controlKeyState & (ControlKeyState.RightAltPressed | ControlKeyState.LeftAltPressed)) != 0;
		bool control = (controlKeyState & (ControlKeyState.RightCtrlPressed | ControlKeyState.LeftCtrlPressed)) != 0;
		ConsoleKeyInfo result = new ConsoleKeyInfo(buffer.keyEvent.uChar, (ConsoleKey)buffer.keyEvent.virtualKeyCode, shift, alt, control);
		if (!intercept)
		{
			Write(buffer.keyEvent.uChar);
		}
		return result;
	}

	private static bool BreakEvent(int controlType)
	{
		if (controlType == 0 || controlType == 1)
		{
			ConsoleCancelEventHandler cancelCallbacks = _cancelCallbacks;
			if (cancelCallbacks == null)
			{
				return false;
			}
			ConsoleSpecialKey controlKey = ((controlType != 0) ? ConsoleSpecialKey.ControlBreak : ConsoleSpecialKey.ControlC);
			ControlCDelegateData controlCDelegateData = new ControlCDelegateData(controlKey, cancelCallbacks);
			WaitCallback callBack = ControlCDelegate;
			if (!ThreadPool.QueueUserWorkItem(callBack, controlCDelegateData))
			{
				return false;
			}
			TimeSpan timeout = new TimeSpan(0, 0, 30);
			controlCDelegateData.CompletionEvent.WaitOne(timeout, exitContext: false);
			if (!controlCDelegateData.DelegateStarted)
			{
				return false;
			}
			controlCDelegateData.CompletionEvent.WaitOne();
			controlCDelegateData.CompletionEvent.Close();
			return controlCDelegateData.Cancel;
		}
		return false;
	}

	private static void ControlCDelegate(object data)
	{
		ControlCDelegateData controlCDelegateData = (ControlCDelegateData)data;
		try
		{
			controlCDelegateData.DelegateStarted = true;
			ConsoleCancelEventArgs consoleCancelEventArgs = new ConsoleCancelEventArgs(controlCDelegateData.ControlKey);
			controlCDelegateData.CancelCallbacks(null, consoleCancelEventArgs);
			controlCDelegateData.Cancel = consoleCancelEventArgs.Cancel;
		}
		finally
		{
			controlCDelegateData.CompletionEvent.Set();
		}
	}

	/// <summary>Ruft den Standardfehlerstream ab.</summary>
	/// <returns>Der Standardfehlerstream.</returns>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardError()
	{
		return OpenStandardError(256);
	}

	/// <summary>Ruft den Standardfehlerstream ab, der auf eine angegebene Puffergröße festgelegt wird.</summary>
	/// <returns>Der Standardfehlerstream.</returns>
	/// <param name="bufferSize">Die Größe des internen Streampuffers. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="bufferSize" /> ist kleiner oder gleich 0 (null). </exception>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardError(int bufferSize)
	{
		if (bufferSize < 0)
		{
			throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
		}
		return GetStandardFile(-12, FileAccess.Write, bufferSize);
	}

	/// <summary>Ruft den Standardeingabestream ab.</summary>
	/// <returns>Der Standardeingabestream.</returns>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardInput()
	{
		return OpenStandardInput(256);
	}

	/// <summary>Ruft den Standardeingabestream ab, der auf eine angegebene Puffergröße festgelegt wird.</summary>
	/// <returns>Der Standardeingabestream.</returns>
	/// <param name="bufferSize">Die Größe des internen Streampuffers. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="bufferSize" /> ist kleiner oder gleich 0 (null). </exception>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardInput(int bufferSize)
	{
		if (bufferSize < 0)
		{
			throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
		}
		return GetStandardFile(-10, FileAccess.Read, bufferSize);
	}

	/// <summary>Ruft den Standardausgabestream ab.</summary>
	/// <returns>Der Standardausgabestream.</returns>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardOutput()
	{
		return OpenStandardOutput(256);
	}

	/// <summary>Ruft den Standardausgabestream ab, der auf eine angegebene Puffergröße festgelegt wird.</summary>
	/// <returns>Der Standardausgabestream.</returns>
	/// <param name="bufferSize">Die Größe des internen Streampuffers. </param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="bufferSize" /> ist kleiner oder gleich 0 (null). </exception>
	/// <filterpriority>1</filterpriority>
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static Stream OpenStandardOutput(int bufferSize)
	{
		if (bufferSize < 0)
		{
			throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
		}
		return GetStandardFile(-11, FileAccess.Write, bufferSize);
	}

	/// <summary>Legt die <see cref="P:System.Console.In" />-Eigenschaft auf den angegebenen <see cref="T:System.IO.TextReader" /> fest.</summary>
	/// <param name="newIn">Ein <see cref="T:System.IO.TextReader" />-Stream, der die neue Standardeingabe darstellt. </param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="newIn" /> hat den Wert null. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Aufrufer verfügt nicht über die erforderliche Berechtigung. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void SetIn(TextReader newIn)
	{
		if (newIn == null)
		{
			throw new ArgumentNullException("newIn");
		}
		new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
		newIn = TextReader.Synchronized(newIn);
		lock (InternalSyncObject)
		{
			_in = newIn;
		}
	}

	/// <summary>Legt die <see cref="P:System.Console.Out" />-Eigenschaft auf den angegebenen <see cref="T:System.IO.TextWriter" /> fest.</summary>
	/// <param name="newOut">Ein <see cref="T:System.IO.TextWriter" />-Stream, der die neue Standardausgabe darstellt. </param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="newOut" /> hat den Wert null. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Aufrufer verfügt nicht über die erforderliche Berechtigung. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void SetOut(TextWriter newOut)
	{
		if (newOut == null)
		{
			throw new ArgumentNullException("newOut");
		}
		new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
		_isOutTextWriterRedirected = true;
		newOut = TextWriter.Synchronized(newOut);
		lock (InternalSyncObject)
		{
			_out = newOut;
		}
	}

	/// <summary>Legt die <see cref="P:System.Console.Error" />-Eigenschaft auf den angegebenen <see cref="T:System.IO.TextWriter" /> fest.</summary>
	/// <param name="newError">Ein <see cref="T:System.IO.TextWriter" />-Stream, der die neue Standardfehlerausgabe darstellt. </param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="newError" /> hat den Wert null. </exception>
	/// <exception cref="T:System.Security.SecurityException">Der Aufrufer verfügt nicht über die erforderliche Berechtigung. </exception>
	/// <filterpriority>1</filterpriority>
	/// <PermissionSet>
	///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
	/// </PermissionSet>
	[SecuritySafeCritical]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void SetError(TextWriter newError)
	{
		if (newError == null)
		{
			throw new ArgumentNullException("newError");
		}
		new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
		_isErrorTextWriterRedirected = true;
		newError = TextWriter.Synchronized(newError);
		lock (InternalSyncObject)
		{
			_error = newError;
		}
	}

	/// <summary>Liest das nächste Zeichen aus dem Standardeingabestream.</summary>
	/// <returns>Das nächste Zeichen aus dem Eingabestream, bzw. -1, wenn derzeit keine weiteren Zeichen gelesen werden können.</returns>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static int Read()
	{
		return In.Read();
	}

	/// <summary>Liest die nächste Zeile von Zeichen aus dem Standardeingabestream.</summary>
	/// <returns>Die nächste Zeile von Zeichen aus dem Eingabestream oder null, wenn keine weiteren Zeilen verfügbar sind.</returns>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.OutOfMemoryException">Es ist nicht genügend Speicherplatz vorhanden, um einen Puffer für die zurückgegebene Zeichenfolge zu reservieren. </exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">Die Anzahl der Zeichen in der nächsten Zeile von Zeichen ist größer als <see cref="F:System.Int32.MaxValue" />.</exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static string ReadLine()
	{
		return In.ReadLine();
	}

	/// <summary>Schreibt das aktuelle Zeichen für den Zeilenabschluss in den Standardausgabestream.</summary>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine()
	{
		Out.WriteLine();
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen booleschen Werts, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(bool value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt das angegebene Unicode-Zeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(char value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt das angegebenen Array von Unicode-Zeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="buffer">Ein Array von Unicode-Zeichen. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(char[] buffer)
	{
		Out.WriteLine(buffer);
	}

	/// <summary>Schreibt das angegebene Unterarray von Unicode-Zeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="buffer">Ein Array von Unicode-Zeichen. </param>
	/// <param name="index">Die Anfangsposition in <paramref name="buffer" />. </param>
	/// <param name="count">Die Anzahl der zu schreibenden Zeichen. </param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="buffer" /> hat den Wert null. </exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="index" /> oder <paramref name="count" /> ist kleiner als 0 (null). </exception>
	/// <exception cref="T:System.ArgumentException">Die Summe von <paramref name="index" /> und <paramref name="count" /> bezeichnet eine Position außerhalb von <paramref name="buffer" />. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(char[] buffer, int index, int count)
	{
		Out.WriteLine(buffer, index, count);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen <see cref="T:System.Decimal" />-Werts, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(decimal value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Gleitkommazahl mit doppelter Genauigkeit, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(double value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Gleitkommazahl mit einfacher Genauigkeit, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(float value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 32-Bit-Ganzzahl mit Vorzeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(int value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 32-Bit-Ganzzahl ohne Vorzeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(uint value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 64-Bit-Ganzzahl mit Vorzeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(long value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 64-Bit-Ganzzahl ohne Vorzeichen, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(ulong value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Objekts, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(object value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt den angegebenen Zeichenfolgenwert, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string value)
	{
		Out.WriteLine(value);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Objekts, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string format, object arg0)
	{
		Out.WriteLine(format, arg0);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Objekte, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das erste mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg1">Das zweite mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string format, object arg0, object arg1)
	{
		Out.WriteLine(format, arg0, arg1);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Objekte, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das erste mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg1">Das zweite mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg2">Das dritte mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string format, object arg0, object arg1, object arg2)
	{
		Out.WriteLine(format, arg0, arg1, arg2);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
	{
		ArgIterator argIterator = new ArgIterator(__arglist);
		int num = argIterator.GetRemainingCount() + 4;
		object[] array = new object[num];
		array[0] = arg0;
		array[1] = arg1;
		array[2] = arg2;
		array[3] = arg3;
		for (int i = 4; i < num; i++)
		{
			array[i] = TypedReference.ToObject(argIterator.GetNextArg());
		}
		Out.WriteLine(format, array);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Arrays von Objekten, gefolgt vom aktuellen Zeichen für den Zeilenabschluss, unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg">Ein mit <paramref name="format" /> zu schreibendes Array von Objekten. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> oder <paramref name="arg" /> ist null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void WriteLine(string format, params object[] arg)
	{
		if (arg == null)
		{
			Out.WriteLine(format, null, null);
		}
		else
		{
			Out.WriteLine(format, arg);
		}
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Objekts unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string format, object arg0)
	{
		Out.Write(format, arg0);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Objekte unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das erste mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg1">Das zweite mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string format, object arg0, object arg1)
	{
		Out.Write(format, arg0, arg1);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Objekte unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg0">Das erste mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg1">Das zweite mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <param name="arg2">Das dritte mit <paramref name="format" /> zu schreibende Objekt. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> hat den Wert null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string format, object arg0, object arg1, object arg2)
	{
		Out.Write(format, arg0, arg1, arg2);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
	{
		ArgIterator argIterator = new ArgIterator(__arglist);
		int num = argIterator.GetRemainingCount() + 4;
		object[] array = new object[num];
		array[0] = arg0;
		array[1] = arg1;
		array[2] = arg2;
		array[3] = arg3;
		for (int i = 4; i < num; i++)
		{
			array[i] = TypedReference.ToObject(argIterator.GetNextArg());
		}
		Out.Write(format, array);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Arrays von Objekten unter Verwendung der angegebenen Formatinformationen in den Standardausgabestream.</summary>
	/// <param name="format">Eine kombinierte Formatierungszeichenfolge. </param>
	/// <param name="arg">Ein mit <paramref name="format" /> zu schreibendes Array von Objekten. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="format" /> oder <paramref name="arg" /> ist null. </exception>
	/// <exception cref="T:System.FormatException">Die Formatangabe in <paramref name="format" /> ist ungültig. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string format, params object[] arg)
	{
		if (arg == null)
		{
			Out.Write(format, null, null);
		}
		else
		{
			Out.Write(format, arg);
		}
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen booleschen Werts in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(bool value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt das angegebene Unicode-Zeichen in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(char value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt das angegebene Array von Unicode-Zeichen in den Standardausgabestream.</summary>
	/// <param name="buffer">Ein Array von Unicode-Zeichen. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(char[] buffer)
	{
		Out.Write(buffer);
	}

	/// <summary>Schreibt das angegebene Unterarray von Unicode-Zeichen in den Standardausgabestream.</summary>
	/// <param name="buffer">Ein Array von Unicode-Zeichen. </param>
	/// <param name="index">Die Anfangsposition in <paramref name="buffer" />. </param>
	/// <param name="count">Die Anzahl der zu schreibenden Zeichen. </param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="buffer" /> hat den Wert null. </exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="index" /> oder <paramref name="count" /> ist kleiner als 0 (null). </exception>
	/// <exception cref="T:System.ArgumentException">Die Summe von <paramref name="index" /> und <paramref name="count" /> bezeichnet eine Position außerhalb von <paramref name="buffer" />. </exception>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(char[] buffer, int index, int count)
	{
		Out.Write(buffer, index, count);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Gleitkommazahl mit doppelter Genauigkeit in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(double value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen <see cref="T:System.Decimal" />-Werts in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(decimal value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen Gleitkommazahl mit einfacher Genauigkeit in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(float value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 32-Bit-Ganzzahl mit Vorzeichen in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(int value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 32-Bit-Ganzzahl ohne Vorzeichen in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(uint value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 64-Bit-Ganzzahl mit Vorzeichen in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(long value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung der angegebenen 64-Bit-Ganzzahl ohne Vorzeichen in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[CLSCompliant(false)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(ulong value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die Textdarstellung des angegebenen Objekts in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert oder null. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(object value)
	{
		Out.Write(value);
	}

	/// <summary>Schreibt die angegebene Zeichenfolge in den Standardausgabestream.</summary>
	/// <param name="value">Der zu schreibende Wert. </param>
	/// <exception cref="T:System.IO.IOException">Ein E/A-Fehler ist aufgetreten. </exception>
	/// <filterpriority>1</filterpriority>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[HostProtection(SecurityAction.LinkDemand, UI = true)]
	public static void Write(string value)
	{
		Out.Write(value);
	}
}
