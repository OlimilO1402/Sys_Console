VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Console"
Attribute VB_GlobalNameSpace = True
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit
'https://github.com/Planet-Source-Code/gridrun-cap-build-real-console-apps-with-vb6__1-38590/blob/master/basDOSConsole.bas

'from Module MPtr we just need LongPtr, so no need for including MPtr if you just activate this:
'#If VBA7 = 0 Then
'    Public Enum LongPtr
'        [_]
'    End Enum
'#End If

Public Enum ConsoleKey
    ConsoleKey_Backspace = 8      ' Die RÜCKTASTE
    ConsoleKey_Tab = 9            ' The TAB key
    ConsoleKey_Clear = 12         ' Die CLEAR-TASTE
    ConsoleKey_Enter = 13         ' Die EINGABETASTE
    ConsoleKey_Pause = 19         ' Die PAUSE-TASTE
    
    'CapsLock is here selfdefined maybe officially defined in another place???
    ConsoleKey_CapsLock = 20       ' Die CapsLock Taste
    
    ConsoleKey_Escape = 27        ' Die ESC-TASTE
    ConsoleKey_Spacebar = &H20    ' Die LEERTASTE
    ConsoleKey_PageUp = 33        ' Die BILD-AUF-TASTE
    ConsoleKey_PageDown = 34      ' The PAGE DOWN key
    ConsoleKey_End = 35           ' The END key
    ConsoleKey_Home = 36          ' The HOME key
    ConsoleKey_LeftArrow = 37     ' Die NACH-LINKS-TASTE
    ConsoleKey_UpArrow = 38       ' Die NACH-OBEN-TASTE
    ConsoleKey_RightArrow = 39    ' Die NACH-RECHTS-TASTE
    ConsoleKey_DownArrow = 40     ' Die NACH-UNTEN-TASTE
    ConsoleKey_Select = 41        ' Die SELECT-TASTE
    ConsoleKey_Print = 42         ' Die DRUCKTASTE
    ConsoleKey_Execute = 43       ' Die EXECUTE-TASTE
    ConsoleKey_PrintScreen = 44   ' Die DRUCK-TASTE
    ConsoleKey_Insert = 45        ' Die EINFG-TASTE
    ConsoleKey_Delete = 46        ' Die ENTF-TASTE
    ConsoleKey_Help = 47          ' The HELP key
    ConsoleKey_D0 = 48            ' The 0 key
    ConsoleKey_D1 = 49            ' The 1 key
    ConsoleKey_D2 = 50            ' The 2 key
    ConsoleKey_D3 = 51            ' The 3 key
    ConsoleKey_D4 = 52            ' The 4 key
    ConsoleKey_D5 = 53            ' The 5 key
    ConsoleKey_D6 = 54            ' The 6 key
    ConsoleKey_D7 = 55            ' The 7 key
    ConsoleKey_D8 = 56            ' The 8 key
    ConsoleKey_D9 = 57            ' The 9 key
    ConsoleKey_A = 65             ' Die A-TASTE
    ConsoleKey_B = 66             ' Die B-TASTE
    ConsoleKey_C = 67             ' Die C-TASTE
    ConsoleKey_D = 68             ' Die D-TASTE
    ConsoleKey_E = 69             ' Die E-TASTE
    ConsoleKey_F = 70             ' Die F-TASTE
    ConsoleKey_G = 71             ' Die G-TASTE
    ConsoleKey_H = 72             ' Die H-TASTE
    ConsoleKey_I = 73             ' Die I-TASTE
    ConsoleKey_J = 74             ' Die J-TASTE
    ConsoleKey_K = 75             ' Die K-TASTE
    ConsoleKey_L = 76             ' Die L-TASTE
    ConsoleKey_M = 77             ' Die M-TASTE
    ConsoleKey_N = 78             ' Die N-TASTE
    ConsoleKey_O = 79             ' Die O-TASTE
    ConsoleKey_P = 80             ' Die P-TASTE
    ConsoleKey_Q = 81             ' Die Q-TASTE
    ConsoleKey_R = 82             ' Die R-TASTE
    ConsoleKey_s = 83             ' Die S-TASTE
    ConsoleKey_T = 84             ' Die T-TASTE
    ConsoleKey_U = 85             ' Die U-TASTE
    ConsoleKey_V = 86             ' Die V-TASTE
    ConsoleKey_W = 87             ' Die W-TASTE
    ConsoleKey_X = 88             ' Die X-TASTE
    ConsoleKey_Y = 89             ' Die Y-TASTE
    ConsoleKey_Z = 90             ' Die Z-TASTE
    ConsoleKey_LeftWindows = 91   ' Die linke WINDOWS-TASTE (Microsoft Natural Keyboard)
    ConsoleKey_RightWindows = 92  ' Die rechte WINDOWS-TASTE (Microsoft Natural Keyboard)
    ConsoleKey_Applications = 93  ' Die ANWENDUNGSTASTE (Microsoft Natural Keyboard).
    ConsoleKey_Sleep = 95         ' Die Standbytaste des Computers
    ConsoleKey_NumPad0 = 96       ' The 0 key on the numeric keypad
    ConsoleKey_NumPad1 = 97       ' Die 1-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad2 = 98       ' Die 2-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad3 = 99       ' Die 3-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad4 = 100      ' Die 4-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad5 = 101      ' Die 5-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad6 = 102      ' Die 6-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad7 = 103      ' Die 7-TASTE auf der Zehnertastatur
    ConsoleKey_NumPad8 = 104      ' The 8 key on the numeric keypad
    ConsoleKey_NumPad9 = 105      ' The 9 key on the numeric keypad
    ConsoleKey_Multiply = 106     ' Die MULTIPLIKATIONSTASTE
    ConsoleKey_Add = 107          ' Die ADDITIONSTASTE
    ConsoleKey_Separator = 108    ' Die TRENNZEICHENTASTE
    ConsoleKey_Subtract = 109     ' Die SUBTRAKTIONSTASTE
    ConsoleKey_Decimal = 110      ' Die KOMMATASTE
    ConsoleKey_Divide = 111       ' Die DIVISIONSTASTE
    ConsoleKey_F1 = 112           ' The F1 key
    ConsoleKey_F2 = 113           ' The F2 key
    ConsoleKey_F3 = 114           ' The F3 key
    ConsoleKey_F4 = 115           ' The F4 key
    ConsoleKey_F5 = 116           ' The F5 key
    ConsoleKey_F6 = 117           ' The F6 key
    ConsoleKey_F7 = 118           ' The F7 key
    ConsoleKey_F8 = 119           ' The F8 key
    ConsoleKey_F9 = 120           ' The F9 key
    ConsoleKey_F10 = 121          ' >The F10 key
    ConsoleKey_F11 = 122          ' The F11 key
    ConsoleKey_F12 = 123          ' The F12 key
    ConsoleKey_F13 = 124          ' The F13 key
    ConsoleKey_F14 = 125          ' The F14 key
    ConsoleKey_F15 = 126          ' The F15 key
    ConsoleKey_F16 = &H7F         ' The F16 key
    ConsoleKey_F17 = &H80         ' The F17 key
    ConsoleKey_F18 = 129          ' The F18 key
    ConsoleKey_F19 = 130          ' The F19 key
    ConsoleKey_F20 = 131          ' The F20 key
    ConsoleKey_F21 = 132          ' The F21 key
    ConsoleKey_F22 = 133          ' The F22 key
    ConsoleKey_F23 = 134          ' The F23 key
    ConsoleKey_F24 = 135          ' The F24 key
    
    ConsoleKey_NumLock = 144
    
    ConsoleKey_BrowserBack = 166       ' Die BROWSER-ZURÜCK-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserForward = 167    ' Die BROWSER-VORWÄRTS-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserRefresh = 168    ' Die BROWSER-AKTUALISIEREN-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserStop = 169       ' Die BROWSER-ABBRECHEN-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserSearch = 170     ' Die BROWSER-SUCHEN-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserFavorites = 171  ' Die BROWSER-FAVORITEN-TASTE (Windows 2000 oder höher)
    ConsoleKey_BrowserHome = 172       ' Die BROWSER-STARTSEITE-TASTE (Windows 2000 oder höher)
    ConsoleKey_VolumeMute = 173        ' Die Taste zum Stummschalten (Microsoft Natural Keyboard, Windows 2000 oder höher)
    ConsoleKey_VolumeDown = 174        ' Die Taste zum Verringern der Lautstärke (Microsoft Natural Keyboard, Windows 2000 oder höher)
    ConsoleKey_VolumeUp = 175          ' Die Taste zum Erhöhen der Lautstärke (Microsoft Natural Keyboard, Windows 2000 oder höher)
    ConsoleKey_MediaNext = 176         ' Die Playertaste für den nächsten Titel (Windows 2000 oder höher)
    ConsoleKey_MediaPrevious = 177     ' Die Playertaste für den vorherigen Titel (Windows 2000 oder höher)
    ConsoleKey_MediaStop = 178         ' Die Playertaste für das Beenden der Wiedergabe (Windows 2000 oder höher)
    ConsoleKey_MediaPlay = 179         ' Die Playertaste für Wiedergabe und Pause (Windows 2000 oder höher)
    ConsoleKey_LaunchMail = 180        ' Die MAILTASTE (Microsoft Natural Keyboard, Windows 2000 oder höher)
    ConsoleKey_LaunchMediaSelect = 181 ' Die Taste für die Medienauswahl (Microsoft Natural Keyboard, Windows 2000 oder höher).
    ConsoleKey_LaunchApp1 = 182        ' The Start Application 1 key (Microsoft Natural Keyboard, Windows 2000 or later)
    ConsoleKey_LaunchApp2 = 183        ' Die ANWENDUNGSSTARTTASTE 2 (Microsoft Natural Keyboard, Windows 2000 oder höher)
    ConsoleKey_Oem1 = 186              ' The OEM 1 key (OEM specific)
    ConsoleKey_OemPlus = 187           ' Die OEM-abhängige PLUSTASTE auf Tastaturen beliebiger Länder/Regionen (Windows 2000 oder höher)
    ConsoleKey_OemComma = 188          ' >Die OEM-abhängige KOMMATASTE auf Tastaturen beliebiger Länder/Regionen (Windows 2000 oder höher)
    ConsoleKey_OemMinus = 189          ' Die OEM-abhängige MINUSTASTE auf Tastaturen beliebiger Länder/Regionen (Windows 2000 oder höher)
    ConsoleKey_OemPeriod = 190         ' Die OEM-abhängige PUNKTTASTE auf Tastaturen beliebiger Länder/Regionen (Windows 2000 oder höher)
    ConsoleKey_Oem2 = 191              ' >The OEM 2 key (OEM specific)
    ConsoleKey_Oem3 = 192              ' The OEM 3 key (OEM specific)
    ConsoleKey_Oem4 = 219              ' The OEM 4 key (OEM specific)
    ConsoleKey_Oem5 = 220              ' Die OEM-abhängige 5-TASTE (OEM-spezifisch)
    ConsoleKey_Oem6 = 221              ' The OEM 6 key (OEM specific)
    ConsoleKey_Oem7 = 222              ' The OEM 7 key (OEM specific)
    ConsoleKey_Oem8 = 223              ' The OEM 8 key (OEM specific)
    ConsoleKey_Oem102 = 226            ' Die OEM-abhängige 102. Taste (OEM-spezifisch)
    ConsoleKey_Process = 229           ' Die Taste für die IME-Verarbeitung
    ConsoleKey_Packet = 231            ' Die PACKET-TASTE (zum Übergeben von Unicode-Zeichen mit Tastaturanschlägen)
    ConsoleKey_Attention = 246         ' The ATTN key.
    ConsoleKey_CrSel = 247             ' Die CRSEL-TASTE (Cursor auswählen)
    ConsoleKey_ExSel = 248             ' Die EXSEL-TASTE (Auswahl erweitern)
    ConsoleKey_EraseEndOfFile = 249    ' Die ERASE EOF-TASTE
    ConsoleKey_Play = 250              ' The PLAY key
    ConsoleKey_Zoom = 251              ' The ZOOM key
    ConsoleKey_NoName = 252            ' Eine für die zukünftige Verwendung reservierte Konstante
    ConsoleKey_Pa1 = 253               ' Die PA1-TASTE
    ConsoleKey_OemClear = 254          ' Die CLEAR-TASTE (OEM-spezifisch)
End Enum

Private Const DefaultConsoleBufferSize As Long = 256
Private Const AltVKCode                As Integer = 18
Private Const NumberLockVKCode         As Long = 144
Private Const CapsLockVKCode           As Long = 20
Private Const MinBeepFrequency         As Long = 37
Private Const MaxBeepFrequency         As Long = 32767
Private Const MaxConsoleTitleLength    As Long = 24500 '<64000

' Handle constants
Private Const STD_INPUT_HANDLE     As Long = -10&
Private Const STD_OUTPUT_HANDLE    As Long = -11&
Private Const STD_ERROR_HANDLE     As Long = -12&

Private Const INVALID_HANDLE_VALUE As Long = -1

' Color constants
'Private Const FOREGROUND_BLUE      As Long = &H1&
'Private Const FOREGROUND_GREEN     As Long = &H2&
'Private Const FOREGROUND_RED       As Long = &H4&
'Private Const FOREGROUND_INTENSITY As Long = &H8&
'Private Const BACKGROUND_BLUE      As Long = &H10&
'Private Const BACKGROUND_GREEN     As Long = &H20&
'Private Const BACKGROUND_RED       As Long = &H40&
'Private Const BACKGROUND_INTENSITY As Long = &H80&

' For SetConsoleMode (input)
Private Const ENABLE_PROCESSED_INPUT As Long = &H1&
Private Const ENABLE_LINE_INPUT      As Long = &H2&
Private Const ENABLE_ECHO_INPUT      As Long = &H4&
Private Const ENABLE_WINDOW_INPUT    As Long = &H8&
Private Const ENABLE_MOUSE_INPUT     As Long = &H10&

' For SetConsoleMode (output)
Private Const ENABLE_PROCESSED_OUTPUT   As Long = &H1&
Private Const ENABLE_WRAP_AT_EOL_OUTPUT As Long = &H2&

'https://learn.microsoft.com/de-de/windows/console/input-record-str
Private Const KEY_EVENT                As Integer = &H1  ' Das Ereignismember enthält eine KEY_EVENT_RECORD-Struktur mit Informationen zu einem Tastaturereignis.
Private Const MOUSE_EVENT              As Integer = &H2  ' Der Event-Member enthält eine MOUSE_EVENT_RECORD-Struktur mit Informationen zu einer Mausbewegung oder einem Schaltflächen-Press-Ereignis.
Private Const WINDOW_BUFFER_SIZE_EVENT As Integer = &H4  ' Das Ereignismember enthält eine WINDOW_BUFFER_SIZE_RECORD Struktur mit Informationen zur neuen Größe des Konsolenbildschirmpuffers.
Private Const MENU_EVENT               As Integer = &H8  ' Der Event-Member enthält eine MENU_EVENT_RECORD-Struktur . Diese Ereignisse werden intern verwendet und sollten ignoriert werden.
Private Const FOCUS_EVENT              As Integer = &H10 ' Der Event-Member enthält eine FOCUS_EVENT_RECORD Struktur. Diese Ereignisse werden intern verwendet und sollten ignoriert werden.

Private Enum ControlKeyState
    RightAltPressed = &H1
    LeftAltPressed = &H2
    RightCtrlPressed = &H4
    LeftCtrlPressed = &H8
    ShiftPressed = &H10
    NumLockOn = &H20
    ScrollLockOn = &H40
    CapsLockOn = &H80
    EnhancedKey = &H100
End Enum

Public Enum ConsoleColor
    ConsoleColor_Black = &H0       'Die Farbe Schwarz.
    ConsoleColor_DarkBlue = &H1    'Die Farbe Dunkelblau.
    ConsoleColor_DarkGreen = &H2   'Die Farbe Dunkelgrün.
    ConsoleColor_DarkCyan = &H3    'Die Farbe Dunkelzyan (dunkles Blaugrün).
    ConsoleColor_DarkRed = &H4     'Die Farbe Dunkelrot.
    ConsoleColor_DarkMagenta = &H5 'Die Farbe Dunkelmagenta (dunkles, purpurähnliches Rot).
    ConsoleColor_DarkYellow = &H6  'Die Farbe Dunkelgelb (Ocker).
    ConsoleColor_Gray = &H7        'Die Farbe Grau.
    ConsoleColor_DarkGray = &H8    'Die Farbe Dunkelgrau.
    ConsoleColor_Blue = &H9        'Die Farbe Blau.
    ConsoleColor_Green = &HA       'Die Farbe Grün.
    ConsoleColor_Cyan = &HB        'Die Farbe Zyan (blaugrün).
    ConsoleColor_Red = &HC         'Die Farbe Rot.
    ConsoleColor_Magenta = &HD     'Das Farbe Magenta (purpurähnliches Rot).
    ConsoleColor_Yellow = &HE      'Die Farbe Gelb.
    ConsoleColor_White = &HF       'Die Farbe Weiß.
End Enum

Public Enum ConsoleSpecialKey
    ConsoleSpecialKey_ControlC      '/// <summary>Die Modifizierertaste <see cref="F:System.ConsoleModifiers.Control" /> und die Konsolentaste <see cref="F:System.ConsoleKey.C" />.</summary>
    ConsoleSpecialKey_ControlBreak  '/// <summary>Die Modifizierertaste <see cref="F:System.ConsoleModifiers.Control" /> und die Konsolentaste UNTBR.</summary>
End Enum

Private Type SMALL_RECT
    Left   As Integer ' 2
    Top    As Integer ' 2
    Right  As Integer ' 2
    Bottom As Integer ' 2
End Type         ' Sum: 8

Private Type COORD
    X As Integer ' 2
    Y As Integer ' 2
End Type    ' Sum: 4

Private Type LngCOORD
    Value As Long
End Type

Private Type KeyEventRecord
    KeyDown         As Long    ' 4 'bool 'Boolean
    repeatCount     As Integer ' 2 'short
    virtualKeyCode  As Integer ' 2 'short
    virtualScanCode As Integer ' 2 'short
    uChar           As Integer ' 2 'char
    ControlKeyState As Long    ' 4 'int
End Type                 ' Sum: 16
Private Type MouseEventRecord
    dwMousePosition   As COORD ' 4
    dwButtonState     As Long  ' 4
    dwControlKeyState As Long  ' 4
    dwEventFlags      As Long  ' 4
End Type                 ' Sum: 16

Private Type WindowBufferSizeRecord
    dwSize As COORD ' 4
End Type

Private Type MenuEventRecord
    dwCommandId As Long
End Type

Private Type FocusEventRecord
    bSetFocus As Long ' 4 ' BOOL
End Type

Private Type InputRecord
    EventType As Integer 'short
'  union {
'    KEY_EVENT_RECORD          KeyEvent;
'    MOUSE_EVENT_RECORD        MouseEvent;
'    WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
'    MENU_EVENT_RECORD         MenuEvent;
'    FOCUS_EVENT_RECORD        FocusEvent;
'  } Event;
    keyEvent  As KeyEventRecord
End Type

Private Type CONSOLE_SCREEN_BUFFER_INFO
    dwSize           As COORD      ' 4
    dwCursorPosition As COORD      ' 4
    wAttributes      As Integer    ' 2
    srWindow         As SMALL_RECT ' 8
    dwMaxWindowSize  As COORD      ' 4
End Type                     ' Sum: 22 'überprüft OK

Private Type CONSOLE_CURSOR_INFO
    dwSize   As Long
    bVisible As Boolean
End Type

'Console related API-deklarations
'https://learn.microsoft.com/de-de/windows/console/allocconsole
Private Declare Function AllocConsole Lib "kernel32" () As Long
'https://learn.microsoft.com/de-de/windows/console/freeconsole
Private Declare Function FreeConsole Lib "kernel32" () As Long
'https://learn.microsoft.com/de-de/windows/console/getstdhandle
Private Declare Function GetStdHandle Lib "kernel32" (ByVal nStdHandle As Long) As LongPtr
'https://learn.microsoft.com/de-de/windows-hardware/drivers/debugger/-closehandle--close-handle-
Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As LongPtr) As Long
'https://learn.microsoft.com/de-de/windows/console/writeconsole
Private Declare Function WriteConsoleW Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal lpbuffer As LongPtr, ByVal nNumberOfCharsToWrite As Long, ByRef lpNumberOfCharsWritten As Long, lpReserved As Long) As Long
'https://learn.microsoft.com/de-de/windows/console/readconsole
Private Declare Function ReadConsoleW Lib "kernel32" (ByVal hConsoleInput As LongPtr, ByVal lpbuffer As LongPtr, ByVal nNumberOfCharsToRead As Long, ByRef lpNumberOfCharsRead As Long, lpReserved As Long) As Long
'https://learn.microsoft.com/de-de/windows/console/setconsolemode
Private Declare Function SetConsoleMode Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal dwMode As Long) As Long
'https://learn.microsoft.com/de-de/windows/console/getconsolemode
Private Declare Function GetConsoleMode Lib "kernel32" (ByVal hConsoleHandle As LongPtr, lpMode As Long) As Long
'https://learn.microsoft.com/de-de/windows/console/setconsoletitle
Private Declare Function SetConsoleTitleW Lib "kernel32" (ByVal lpConsoleTitle As LongPtr) As Long
'https://learn.microsoft.com/en-us/windows/console/getconsoletitle
Private Declare Function GetConsoleTitleW Lib "kernel32" (ByVal lpConsoleTitle As LongPtr, ByVal nSize As Long) As Long
'https://learn.microsoft.com/de-de/windows/console/setconsoletextattribute
Private Declare Function SetConsoleTextAttribute Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal wAttributes As Integer) As Long

'https://learn.microsoft.com/de-de/windows/console/getconsolescreenbufferinfo
Private Declare Function GetConsoleScreenBufferInfo Lib "kernel32" (ByVal hConsoleOutput As LongPtr, lpConsoleScreenBufferInfo As CONSOLE_SCREEN_BUFFER_INFO) As Long
'https://learn.microsoft.com/de-de/windows/console/setconsolescreenbuffersize
Private Declare Function SetConsoleScreenBufferSize Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal dwSize As Long) As Long  'BOOL

'https://learn.microsoft.com/de-de/windows/console/peekconsoleinput
Private Declare Function PeekConsoleInput Lib "kernel32" (ByVal hConsoleInput As LongPtr, lpbuffer As InputRecord, ByVal nLength As Long, ByRef lpNumberOfEventsRead_out As Long) As Long  ' BOOL
'https://learn.microsoft.com/en-us/windows/console/readconsoleinput
Private Declare Function ReadConsoleInput Lib "kernel32" (ByVal hConsoleInput As LongPtr, lpbuffer As InputRecord, ByVal numInputRecords_UseOne As Long, ByRef lpNumberOfEventsRead As Long) As Long

'https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeystate
Private Declare Function GetKeyState Lib "user32" (ByVal virtualKeyCode As Long) As Integer

'https://learn.microsoft.com/de-de/windows/console/setconsolecursorposition
Private Declare Function SetConsoleCursorPosition Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal cursorPosition As Long) As Long 'bool
'https://learn.microsoft.com/de-de/windows/console/getconsolecursorinfo
Private Declare Function GetConsoleCursorInfo Lib "kernel32" (ByVal hConsoleOutput As LongPtr, cci As CONSOLE_CURSOR_INFO) As Long  'bool
'https://learn.microsoft.com/de-de/windows/console/setconsolecursorinfo
Private Declare Function SetConsoleCursorInfo Lib "kernel32" (ByVal hConsoleOutput As LongPtr, cci As CONSOLE_CURSOR_INFO) As Long  'bool

'https://learn.microsoft.com/de-de/windows/console/getlargestconsolewindowsize
Private Declare Function GetLargestConsoleWindowSize Lib "kernel32" (ByVal hConsoleOutput As LongPtr) As Long 'COORD

'https://learn.microsoft.com/de-de/windows/console/setconsolewindowinfo
Private Declare Function SetConsoleWindowInfo Lib "kernel32" (ByVal hConsoleOutput As LongPtr, ByVal absolute As Long, consoleWindow As SMALL_RECT) As Long ' bool


' File IO functions
'https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-readfile
Private Declare Function ReadFile Lib "kernel32" (ByVal hFile As LongPtr, ByVal lpbuffer As LongPtr, ByVal nNumberOfBytesToRead As Long, lpNumberOfBytesRead As Long, lpOverlapped As Any) As Long
'https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-writefile
Private Declare Function WriteFile Lib "kernel32" (ByVal hFile As LongPtr, ByVal lpbuffer As LongPtr, ByVal nNumberOfBytesToWrite As Long, lpNumberOfBytesWritten As Long, lpOverlapped As Any) As Long

'https://learn.microsoft.com/en-us/windows/win32/api/utilapiset/nf-utilapiset-beep
Private Declare Function Intern_Beep Lib "kernel32" Alias "Beep" (ByVal dwFreq As Long, ByVal dwDuration As Long) As Long

Private m_cachedInputRecord     As InputRecord
Private m_haveReadDefaultColors As Boolean
Private m_defaultColors         As Byte

Private m_isOutTextWriterRedirected   As Boolean
Private m_isErrorTextWriterRedirected As Boolean

'Private m_inputEncoding               As Long
'Private m_outputEncoding              As Long

Private m_stdInRedirectQueried  As Boolean
Private m_stdOutRedirectQueried As Boolean
Private m_stdErrRedirectQueried As Boolean

Private m_isStdInRedirected  As Boolean
Private m_isStdOutRedirected As Boolean
Private m_isStdErrRedirected As Boolean

Private m_InternalSyncObject As Object

'or maybe Enum?
Public Enum ConsoleIOMode
    IOModeStandard
    IOModeFile
End Enum

Private m_IOMode    As ConsoleIOMode

Private m_cci       As CONSOLE_CURSOR_INFO

' Handles
Private hConsoleIn  As LongPtr
Private hConsoleOut As LongPtr
Private hConsoleErr As LongPtr
Private bInIDE      As Boolean

Private Sub Class_Initialize()
    bInIDE = InIDE
    'If Not bInIDE Then
    Dim bOK As Boolean: bOK = AllocConsole
    'If Not bOK Then MsgBox "Could not allocate Console for this process"
    Me.OpenStandardInput
    Me.OpenStandardOutput
    Me.OpenStandardError
    'hConsoleIn = GetStdHandle(STD_INPUT_HANDLE)
    'hConsoleOut = GetStdHandle(STD_OUTPUT_HANDLE)
    'hConsoleErr = GetStdHandle(STD_ERROR_HANDLE)
End Sub

Private Sub Class_Terminate()
    'If Not bInIDE Then
        CloseHandle hConsoleIn
        CloseHandle hConsoleOut
        CloseHandle hConsoleErr
        FreeConsole
    'endif
End Sub

Private Function InIDE() As Boolean
Try: On Error GoTo Catch
    Debug.Print 1 / 0
    InIDE = False
    Exit Function
Catch:
    InIDE = True
End Function
Public Property Get IsInIDE() As Boolean
    IsInIDE = bInIDE
End Property
Public Property Let IsInIDE(ByVal Value As Boolean)
    'set this for debugging purposes otherwise it's possible to use the debug-windows as a Console
    bInIDE = Value
End Property
Public Property Get IsHandlesValid() As Boolean
    IsHandlesValid = 3 < hConsoleIn:   If Not IsHandlesValid Then Exit Property
    IsHandlesValid = 7 < hConsoleOut:  If Not IsHandlesValid Then Exit Property
    IsHandlesValid = 11 < hConsoleErr: If Not IsHandlesValid Then Exit Property
End Property
Public Property Let IOMode(ByVal Value As ConsoleIOMode)
    m_IOMode = Value
End Property
Public Property Get IOMode() As ConsoleIOMode
    IOMode = m_IOMode
End Property

Public Property Let Title(ByVal Value As String)
    Dim b As Boolean: b = SetConsoleTitleW(StrPtr(Value))
End Property
Public Property Get Title() As String
    Dim s As String: s = String$(MaxConsoleTitleLength, vbNullChar)
    Dim l As Long: l = GetConsoleTitleW(StrPtr(s), MaxConsoleTitleLength)
    Title = Left(s, l)
End Property

Public Property Get BackgroundColorToStr() As String
    BackgroundColorToStr = ConsoleColorToStr(Me.BackgroundColor)
End Property
Public Property Get BackgroundColor() As ConsoleColor
    Dim succeeded As Boolean
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo2(False, succeeded)
    If Not succeeded Then
        BackgroundColor = ConsoleColor.ConsoleColor_Black
        Exit Property
    End If
    Const ColorBackgroundMask As Long = &HF0&
    Dim c As Long: c = bufferInfo.wAttributes And ColorBackgroundMask
    'If (c And ColorBackgroundMask) <> 0 Then c = c \ 16
    BackgroundColor = ColorAttributeToConsoleColor(c)
End Property
Public Property Let BackgroundColor(ByVal Value As ConsoleColor)
    Dim color As Long: color = ConsoleColorToColorAttribute(Value, True)
    Dim succeeded As Boolean
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo2(False, succeeded)
    If Not succeeded Then Exit Property
    Dim wAttribs As Integer: wAttribs = bufferInfo.wAttributes
    wAttribs = CInt(CLng(wAttribs) And &HFFFFFF0F)
    wAttribs = CInt(CLng(wAttribs) Or color)
    SetConsoleTextAttribute hConsoleOut, wAttribs
End Property

Public Sub Beep()
    BeepF 800, 200
End Sub
Public Sub BeepF(ByVal frequency As Long, ByVal duration As Long)
    If (frequency < 37 Or 32767 < frequency) Then
        'throw new ArgumentOutOfRangeException("frequency", frequency, Environment.GetResourceString("ArgumentOutOfRange_BeepFrequency", 37, 32767));
    End If
    If (duration <= 0) Then
        'throw new ArgumentOutOfRangeException("duration", duration, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
    End If
    Intern_Beep frequency, duration
End Sub

Public Property Get BufferHeight() As Long
    BufferHeight = GetBufferInfo().dwSize.Y
End Property
Public Property Let BufferHeight(ByVal Value As Long)
    SetBufferSize BufferWidth, Value
End Property

Public Property Get BufferWidth() As Long
    BufferWidth = GetBufferInfo().dwSize.X
End Property
Public Property Let BufferWidth(ByVal Value As Long)
    SetBufferSize Value, BufferHeight
End Property

Public Sub SetBufferSize(ByVal Width As Long, ByVal Height As Long)
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    Dim srWindow As SMALL_RECT: srWindow = GetBufferInfo().srWindow
    If (Width < srWindow.Right + 1 Or 32767 <= Width) Then
        'throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferLessThanWindowSize"));
    End If
    If (Height < srWindow.Bottom + 1 Or 32767 <= Height) Then
        'throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferLessThanWindowSize"));
    End If
    Dim size As COORD
    size.X = CInt(Width)
    size.Y = CInt(Height)
    Dim lsize As LngCOORD: LSet lsize = size
    Dim bOK As Boolean: bOK = SetConsoleScreenBufferSize(hConsoleOut, lsize.Value)
    If Not bOK Then
        '__Error.WinIOError();
    End If
End Sub

Public Property Get CapsLock() As Boolean
    Dim keyState As Integer: keyState = GetKeyState(20) 'ConsoleKey.ConsoleKey_CapsLock) 'self defined
    CapsLock = (keyState And 1) = 1
End Property

Private Property Get ConsoleInputHandle() As LongPtr
    If hConsoleIn = 0 Then Me.OpenStandardInput
    ConsoleInputHandle = hConsoleIn
End Property
Private Property Get ConsoleOutputHandle() As LongPtr
    If hConsoleOut = 0 Then Me.OpenStandardOutput
    ConsoleOutputHandle = hConsoleOut
End Property

Public Property Get CursorLeft() As Long
    CursorLeft = GetBufferInfo().dwCursorPosition.X
End Property
Public Property Let CursorLeft(ByVal Value As Long)
    SetCursorPosition Value, Me.CursorTop
End Property

Public Property Get CursorTop() As Long
    CursorTop = GetBufferInfo().dwCursorPosition.Y
End Property
Public Property Let CursorTop(ByVal Value As Long)
    SetCursorPosition Me.CursorLeft, Value
End Property

Public Property Get CursorSize() As Long
    'IntPtr consoleOutputHandle = ConsoleOutputHandle;
    Dim bOK As Boolean: bOK = GetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError();
    End If
    CursorSize = m_cci.dwSize
End Property
Public Property Let CursorSize(ByVal Value As Long)
    If (Value < 1 Or 100 < Value) Then
        'throw new ArgumentOutOfRangeException("value", value, Environment.GetResourceString("ArgumentOutOfRange_CursorSize"));
    End If
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    'IntPtr consoleOutputHandle = ConsoleOutputHandle;
    Dim bOK As Boolean: bOK = GetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError();
        Exit Property
    End If
    m_cci.dwSize = Value
    bOK = SetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError();
    End If
End Property

Public Sub SetCursorPosition(ByVal Left As Long, ByVal Top As Long)
    If (Left < 0 Or 32767 <= Left) Then
        'throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
        Exit Sub
    End If
    If (Top < 0 Or 32767 <= Top) Then
        'throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
        Exit Sub
    End If
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    'IntPtr consoleOutputHandle = ConsoleOutputHandle;
    Dim cursorPosition As COORD
    cursorPosition.X = CInt(Left)
    cursorPosition.Y = CInt(Top)
    Dim lCursPos As LngCOORD: LSet lCursPos = cursorPosition
    Dim bOK As Boolean: bOK = SetConsoleCursorPosition(hConsoleOut, lCursPos.Value)
    If Not bOK Then
        Dim lastWin32Error As Long: lastWin32Error = Err.LastDllError  'Marshal.GetLastWin32Error()
        Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo()
        If (Left < 0 Or bufferInfo.dwSize.X <= Left) Then
            'throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
            Exit Sub
        End If
        If (Top < 0 Or bufferInfo.dwSize.Y <= Top) Then
            'throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleBufferBoundaries"));
            Exit Sub
        End If
        '__Error.WinIOError(lastWin32Error, string.Empty);
    End If
End Sub

Public Property Get CursorVisible() As Boolean
    Dim bOK As Boolean: bOK = GetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError();
    End If
    CursorVisible = m_cci.bVisible
End Property

Public Property Let CursorVisible(ByVal Value As Boolean)
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    Dim bOK As Boolean: bOK = GetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError();
    End If
    m_cci.bVisible = Value
    bOK = SetConsoleCursorInfo(hConsoleOut, m_cci)
    If Not bOK Then
        '__Error.WinIOError()
    End If
End Property

Public Property Get ForegroundColorToStr()
    ForegroundColorToStr = ConsoleColorToStr(Me.ForegroundColor)
End Property
Public Property Get ForegroundColor() As ConsoleColor
    Dim succeeded As Boolean
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo2(False, succeeded)
    If Not succeeded Then
        ForegroundColor = ConsoleColor.ConsoleColor_Gray
        Exit Property
    End If
    Const ColorBackgroundMask As Long = &HF0&
    Dim c As Long: c = (bufferInfo.wAttributes & &HF&)
    'If (c And ColorBackgroundMask) <> 0 Then c = c \ 16
    ForegroundColor = ColorAttributeToConsoleColor(c)
End Property
Public Property Let ForegroundColor(ByVal Value As ConsoleColor)
    Dim color As Long: color = ConsoleColorToColorAttribute(Value, False)
    Dim succeeded As Boolean
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo2(False, succeeded)
    If Not succeeded Then Exit Property
    Dim wAttribs As Integer: wAttribs = bufferInfo.wAttributes
    wAttribs = CInt(CLng(wAttribs) And &HFFFFFFF0)
    wAttribs = CInt(CLng(wAttribs) Or color)
    SetConsoleTextAttribute hConsoleOut, wAttribs
End Property
Private Function ColorAttributeToConsoleColor(ByVal c As Long) As ConsoleColor
    Const ColorBackgroundMask As Long = &HF0&
    If (c And ColorBackgroundMask) <> 0 Then
        c = c \ 16
    End If
    ColorAttributeToConsoleColor = c
End Function
Private Function ConsoleColorToColorAttribute(ByVal color As ConsoleColor, ByVal isBackground As Boolean) As Long
    If (color And &HFFFFFFF0) <> 0 Then
        'throw new ArgumentException(Environment.GetResourceString("Arg_InvalidConsoleColor"));
    End If
    Dim color2 As Long: color2 = color
    If isBackground Then
        color2 = color2 * 16
    End If
    ConsoleColorToColorAttribute = color2
End Function
Public Function ConsoleColorToStr(c As ConsoleColor) As String
    Dim s As String
    Select Case c
    Case ConsoleColor.ConsoleColor_Black:       s = "Black"       'Die Farbe Schwarz.
    Case ConsoleColor.ConsoleColor_DarkBlue:    s = "DarkBlue"    'Die Farbe Dunkelblau.
    Case ConsoleColor.ConsoleColor_DarkGreen:   s = "DarkGreen"   'Die Farbe Dunkelgrün.
    Case ConsoleColor.ConsoleColor_DarkCyan:    s = "DarkCyan"    'Die Farbe Dunkelzyan (dunkles Blaugrün).
    Case ConsoleColor.ConsoleColor_DarkRed:     s = "DarkRed"     'Die Farbe Dunkelrot.
    Case ConsoleColor.ConsoleColor_DarkMagenta: s = "DarkMagenta" 'Die Farbe Dunkelmagenta (dunkles, purpurähnliches Rot).
    Case ConsoleColor.ConsoleColor_DarkYellow:  s = "DarkYellow"  'Die Farbe Dunkelgelb (Ocker).
    Case ConsoleColor.ConsoleColor_Gray:        s = "Gray"        'Die Farbe Grau.
    Case ConsoleColor.ConsoleColor_DarkGray:    s = "DarkGray"    'Die Farbe Dunkelgrau.
    Case ConsoleColor.ConsoleColor_Blue:        s = "Blue"        'Die Farbe Blau.
    Case ConsoleColor.ConsoleColor_Green:       s = "Green"       'Die Farbe Grün.
    Case ConsoleColor.ConsoleColor_Cyan:        s = "Cyan"        'Die Farbe Zyan (blaugrün).
    Case ConsoleColor.ConsoleColor_Red:         s = "Red"         'Die Farbe Rot.
    Case ConsoleColor.ConsoleColor_Magenta:     s = "Magenta"     'Das Farbe Magenta (purpurähnliches Rot).
    Case ConsoleColor.ConsoleColor_Yellow:      s = "Yellow"      'Die Farbe Gelb.
    Case ConsoleColor.ConsoleColor_White:       s = "White"       'Die Farbe Weiß.
    End Select
    ConsoleColorToStr = s
End Function

Private Function GetBufferInfo() As CONSOLE_SCREEN_BUFFER_INFO
    Dim succeeded As Boolean
    GetBufferInfo = GetBufferInfo2(True, succeeded)
End Function

Private Function GetBufferInfo2(ByVal throwOnNoConsole As Boolean, ByRef succeeded As Boolean) As CONSOLE_SCREEN_BUFFER_INFO
    succeeded = False
    If (hConsoleOut = INVALID_HANDLE_VALUE) Then
        If Not throwOnNoConsole Then
            'return default(Win32Native.CONSOLE_SCREEN_BUFFER_INFO)
            Exit Function
        End If
        'throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"))
        Exit Function
    End If
    Dim lpConsoleScreenBufferInfo As CONSOLE_SCREEN_BUFFER_INFO
    Dim bOK As Boolean: bOK = GetConsoleScreenBufferInfo(hConsoleOut, lpConsoleScreenBufferInfo)
    If Not bOK Then
        Dim consoleScreenBufferInfo As Boolean: consoleScreenBufferInfo = GetConsoleScreenBufferInfo(GetStdHandle(-12), lpConsoleScreenBufferInfo)
        If Not consoleScreenBufferInfo Then
            consoleScreenBufferInfo = GetConsoleScreenBufferInfo(GetStdHandle(-10), lpConsoleScreenBufferInfo)
        End If
        If Not consoleScreenBufferInfo Then
            Dim lastWin32Error As Long ': lastWin32Error = GetLastWin32Error()
            If (lastWin32Error = 6 And Not throwOnNoConsole) Then
                'return default(Win32Native.CONSOLE_SCREEN_BUFFER_INFO)
                Exit Function
            End If
            '__Error.WinIOError(lastWin32Error, null)
        End If
    End If
    If (Not m_haveReadDefaultColors) Then
        m_defaultColors = CByte(lpConsoleScreenBufferInfo.wAttributes And &HFF)
        m_haveReadDefaultColors = True
    End If
    succeeded = True
    GetBufferInfo2 = lpConsoleScreenBufferInfo
End Function

Public Property Get IsErrorRedirected() As Boolean
    'If m_stdInRedirectQueried Then
        IsErrorRedirected = m_isStdErrRedirected
    '    Exit Property
    'End If
'    Lock InternalSyncObject
'        If m_stdErrRedirectQueried Then
'            IsErrorRedirected = m_isStdErrRedirected
'        End If
'        IntPtr stdHandle = Win32Native.GetStdHandle(-12)
'        m_isStdErrRedirected = IsHandleRedirected(stdHandle)
'        m_stdErrRedirectQueried = True
'        IsErrorRedirected = m_isStdErrRedirected
'    End Lock
End Property
Public Property Get IsInputRedirected() As Boolean
    'If m_isStdInRedirected Then
        IsInputRedirected = m_isStdInRedirected
    '    Exit Property
    'End If
'    Lock (InternalSyncObject)
'        If m_stdInRedirectQueried Then
'            IsInputRedirected = m_isStdInRedirected
'            Exit Property
'        End If
'        m_isStdInRedirected = IsHandleRedirected(ConsoleInputHandle)
'        m_stdInRedirectQueried = True
'        IsInputRedirected = m_isStdInRedirected
'    End Lock
End Property
Public Property Get IsOutputRedirected() As Boolean
    'If m_stdOutRedirectQueried Then
        IsOutputRedirected = m_isStdOutRedirected
    '    Exit Property
    'End If
'    Lock (InternalSyncObject)
'        If m_stdOutRedirectQueried Then
'            IsOutputRedirected = m_isStdOutRedirected
'            Exit Property
'        End If
'        m_isStdOutRedirected = IsHandleRedirected(ConsoleOutputHandle)
'        m_stdOutRedirectQueried = True
'        IsOutputRedirected = m_isStdOutRedirected
'    End Lock
End Property

Public Property Get KeyAvailable() As Boolean
    If m_cachedInputRecord.EventType = KEY_EVENT Then
        KeyAvailable = True: Exit Property
    End If
    Dim buffer As InputRecord
    Dim nEventsRead As Long
    Do While True
        Dim bOK As Boolean: bOK = PeekConsoleInput(hConsoleIn, buffer, 1, nEventsRead)
        If Not bOK Then
            Dim lastWin32Error As Long: lastWin32Error = Err.LastDllError '= Marshal.GetLastWin32Error();
            If lastWin32Error = 6 Then
            '    'throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ConsoleKeyAvailableOnFile"))
                Exit Property
            End If
            Err.Raise lastWin32Error, , "stdin"
        End If
        If nEventsRead = 0 Then
            KeyAvailable = False
            Exit Property
        End If
        If IsKeyDownEvent(buffer) And Not IsModKey(buffer) Then
            Exit Do
        End If
        bOK = ReadConsoleInput(hConsoleIn, buffer, 1, nEventsRead)
        If Not bOK Then
            'Err.Raise 10000, , "WinIOError"
            Exit Property
        End If
    Loop
    KeyAvailable = True
End Property

Private Function IsKeyDownEvent(ir As InputRecord) As Boolean
    If ir.EventType = KEY_EVENT Then
        IsKeyDownEvent = ir.keyEvent.KeyDown
        Exit Function
    End If
    IsKeyDownEvent = False
End Function

Private Function IsModKey(ir As InputRecord) As Boolean
    Dim virtualKeyCode  As Integer: virtualKeyCode = ir.keyEvent.virtualKeyCode
    If (virtualKeyCode < 16 Or 18 < virtualKeyCode) And (virtualKeyCode <> 20 And virtualKeyCode <> 144) Then
        IsModKey = (virtualKeyCode = 145)
        Exit Function
    End If
    IsModKey = True
End Function

Public Property Get LargestWindowHeight() As Long
    Dim lsz As LngCOORD: lsz.Value = GetLargestConsoleWindowSize(hConsoleOut)
    Dim sz  As COORD: LSet sz = lsz
    LargestWindowHeight = sz.Y
End Property

Public Property Get LargestWindowWidth() As Long
    Dim lsz As LngCOORD: lsz.Value = GetLargestConsoleWindowSize(hConsoleOut)
    Dim sz  As COORD: LSet sz = lsz
    LargestWindowWidth = sz.X
End Property

Public Property Get NumberLock() As Boolean
    Dim keyState As Integer: keyState = GetKeyState(144) 'ConsoleKey.ConsoleKey_NumLock) ' self defined
    NumberLock = (keyState And 1) = 1
End Property

Public Property Get TreatControlCAsInput() As Boolean
    'IntPtr consoleInputHandle = ConsoleInputHandle;
    If (hConsoleIn = INVALID_HANDLE_VALUE) Then
        'throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
        Exit Property
    End If
    Dim mode As Long ': mode = 0
    Dim bOK As Boolean: bOK = GetConsoleMode(hConsoleIn, mode)
    If Not bOK Then
        Exit Property
        '__Error.WinIOError();
    End If
    TreatControlCAsInput = (mode And 1) = 0
End Property

Public Property Let TreatControlCAsInput(ByVal Value As Boolean)
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    'IntPtr consoleInputHandle = ConsoleInputHandle;
    If (hConsoleIn = INVALID_HANDLE_VALUE) Then
        'throw new IOException(Environment.GetResourceString("IO.IO_NoConsole"));
        Exit Property
    End If
    Dim mode As Long '= 0
    Dim consoleMode As Boolean: consoleMode = GetConsoleMode(ConsoleInputHandle, mode)
    mode = IIf(Not Value, mode Or 1, mode And -2)
    Dim bOK As Boolean: bOK = SetConsoleMode(hConsoleIn, mode)
    If Not bOK Then
        '__Error.WinIOError();
        Exit Property
    End If
End Property

Public Property Get WindowLeft() As Long
    WindowLeft = GetBufferInfo().srWindow.Left
End Property
Public Property Let WindowLeft(ByVal Value As Long)
    SetWindowPosition Value, WindowTop
End Property

Public Property Get WindowTop() As Long
    WindowTop = GetBufferInfo().srWindow.Top
End Property
Public Property Let WindowTop(ByVal Value As Long)
    SetWindowPosition WindowLeft, Value
End Property

Public Property Get WindowWidth() As Long
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo()
    WindowWidth = bufferInfo.srWindow.Right - bufferInfo.srWindow.Left + 1
End Property
Public Property Let WindowWidth(ByVal Value As Long)
    SetWindowSize Value, WindowHeight
End Property

Public Property Get WindowHeight() As Long
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo
    WindowHeight = bufferInfo.srWindow.Bottom - bufferInfo.srWindow.Top + 1
End Property
Public Property Let WindowHeight(ByVal Value As Long)
    SetWindowSize WindowWidth, Value
End Property

Public Sub SetWindowSize(ByVal Width As Long, ByVal Height As Long)
    If (Width <= 0) Then
        'throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
        Exit Sub
    End If
    If (Height <= 0) Then
        'throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
        Exit Sub
    End If
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo
    Dim flag As Boolean '= false
    Dim size As COORD
    size.X = bufferInfo.dwSize.X
    size.Y = bufferInfo.dwSize.Y
    If (bufferInfo.dwSize.X < bufferInfo.srWindow.Left + Width) Then
        If (bufferInfo.srWindow.Left >= 32767 - Width) Then
            'throw new ArgumentOutOfRangeException("width", Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowBufferSize"));
            Exit Sub
        End If
        size.X = CInt(bufferInfo.srWindow.Left + Width)
        flag = True
    End If
    If (bufferInfo.dwSize.Y < bufferInfo.srWindow.Top + Height) Then
        If (bufferInfo.srWindow.Top >= 32767 - Height) Then
            'throw new ArgumentOutOfRangeException("height", Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowBufferSize"));
            Exit Sub
        End If
        size.Y = CInt(bufferInfo.srWindow.Top + Height)
        flag = True
    End If
    Dim lsz As LngCOORD: LSet lsz = size
    Dim bOK As Boolean: bOK = SetConsoleScreenBufferSize(ConsoleOutputHandle, lsz.Value)
    If (flag And Not bOK) Then
        '__Error.WinIOError();
    End If
    Dim srWindow As SMALL_RECT: srWindow = bufferInfo.srWindow
    srWindow.Bottom = CInt(srWindow.Top + Height - 1)
    srWindow.Right = CInt(srWindow.Left + Width - 1)
    bOK = SetConsoleWindowInfo(ConsoleOutputHandle, True, srWindow)
    If (Not bOK) Then
        'int lastWin32Error = Marshal.GetLastWin32Error();
        If (flag) Then
            LSet lsz = bufferInfo.dwSize
            SetConsoleScreenBufferSize hConsoleOut, lsz.Value
            'Exit Sub
        End If
        Dim largestConsoleWindowSize As COORD: lsz.Value = GetLargestConsoleWindowSize(hConsoleOut): LSet largestConsoleWindowSize = lsz
        If (Width > largestConsoleWindowSize.X) Then
            'throw new ArgumentOutOfRangeException("width", width, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowSize_Size", largestConsoleWindowSize.X))
            Exit Sub
        End If
        If (Height > largestConsoleWindowSize.Y) Then
            'throw new ArgumentOutOfRangeException("height", height, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowSize_Size", largestConsoleWindowSize.Y))
            Exit Sub
        End If
        '__Error.WinIOError(lastWin32Error, string.Empty)
    End If
End Sub

Public Sub SetWindowPosition(ByVal Left As Long, ByVal Top As Long)
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo()
    Dim srWindow   As SMALL_RECT: srWindow = bufferInfo.srWindow
    Dim num As Long: num = Left + srWindow.Right - srWindow.Left + 1
    If (Left < 0 Or num > bufferInfo.dwSize.X Or num < 0) Then
        'throw new ArgumentOutOfRangeException("left", left, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowPos"))
        Exit Sub
    End If
    Dim num2 As Long: num2 = Top + srWindow.Bottom - srWindow.Top + 1
    If (Top < 0 Or num2 > bufferInfo.dwSize.Y Or num2 < 0) Then
        'throw new ArgumentOutOfRangeException("top", top, Environment.GetResourceString("ArgumentOutOfRange_ConsoleWindowPos"))
        Exit Sub
    End If
    srWindow.Bottom = srWindow.Bottom - CInt(srWindow.Top - Top)
    srWindow.Right = srWindow.Right - CInt(srWindow.Left - Left)
    srWindow.Left = CInt(Left)
    srWindow.Top = CInt(Top)
    Dim bOK As Boolean: bOK = SetConsoleWindowInfo(ConsoleOutputHandle, True, srWindow)
    If Not bOK Then
        '__Error.WinIOError();
    End If
End Sub

'Public Shared Sub SetError(ByVal newError As System.IO.TextWriter)
Public Sub SetError(ByVal newError) 'As System.IO.TextWriter)
    '
End Sub

'Public Shared Sub SetIn(ByVal newIn As System.IO.TextReader)
Public Sub SetIn(ByVal newIn) 'As System_IO.TextReader)
    '
End Sub

'Public Shared Sub SetOut(ByVal newOut As System.IO.TextWriter)
Public Sub SetOut(ByVal newOut) 'As System_IO.TextWriter)
    '
End Sub

'Public Shared Sub Write(ByVal value As Boolean)
Public Sub WWrite(ByVal varValue As Variant, Optional ByVal arg0 As Object, Optional ByVal arg1 As Object, Optional ByVal arg2 As Object)
Attribute WWrite.VB_Description = "Schreibt die Textdarstellung des angegebenen Wertes in den Standardausgabestream. Parameter:\r\nformat: Die Formatzeichenfolge.  \r\narg0: Das erste mit format zu schreibende Objekt.  \r\narg1: Das zweite mit format zu schreibende Objekt.  \r\narg2: Das dritte m"
    Dim BytesWritten As Long
    Dim szOut As String
    If IsObject(varValue) Then
        szOut = varValue.ToString
    Else
        szOut = CStr(varValue)
    End If
    If bInIDE Then
        Debug.Print szOut
    Else
        Call WriteConsoleW(hConsoleOut, StrPtr(szOut), Len(szOut), BytesWritten, 0&)
    End If
End Sub

'Public Shared Sub WriteLine()
Public Sub WriteLine(ByVal Value As String)
    WriteLineO Value '& vbCrLf
End Sub

Public Sub WriteLineO(Optional varValue As Variant, Optional ByVal arg0 As Variant, Optional ByVal arg1 As Variant, Optional ByVal arg2 As Variant)
    Dim szOut As String
    'Dim scOut As New cString
    Dim szIn As String
    If IsObject(varValue) Then
        szIn = varValue.ToStr 'ing
    Else
        szIn = CStr(varValue)
    End If
    If Len(szIn) > 0 Then
        'szOut = scOut.fFormat(Nothing, szIn, arg0, arg1, arg2) & vbCrLf
        If bInIDE Then
            szOut = szIn
        Else
            szOut = szIn & vbCrLf
        End If
    Else
        szOut = vbNullString & vbCrLf
    End If
    Dim BytesWritten As Long
    If bInIDE Then
        Debug.Print szOut
    Else
        Dim bOK As Boolean
        If Me.IOMode = IOModeStandard Then
            bOK = WriteConsoleW(hConsoleOut, StrPtr(szOut), Len(szOut), BytesWritten, 0&)
        Else
            bOK = WriteFile(hConsoleOut, StrPtr(szOut), Len(szOut), BytesWritten, 0&)
        End If
    End If
End Sub

'Public Shared Function OpenStandardError() As System.IO.Stream
Public Function OpenStandardError(Optional ByVal bufferSize As Long) As LongPtr 'As System.IO.Stream
    hConsoleErr = GetStdHandle(STD_ERROR_HANDLE)
    OpenStandardError = hConsoleErr
End Function

'Public Shared Function OpenStandardInput() As System.IO.Stream
Public Function OpenStandardInput(Optional ByVal bufferSize As Long) As LongPtr 'As System.IO.Stream
    hConsoleIn = GetStdHandle(STD_INPUT_HANDLE)
    OpenStandardInput = hConsoleIn
End Function

'Public Shared Function OpenStandardOutput(ByVal bufferSize As Integer) As System.IO.Stream
Public Function OpenStandardOutput(Optional ByVal bufferSize As Long) As LongPtr 'As System.IO.Stream
    hConsoleOut = GetStdHandle(STD_OUTPUT_HANDLE)
    OpenStandardOutput = hConsoleOut
End Function

'Public Shared Function Read() As Integer
Public Function Read() As String 'cInteger
Attribute Read.VB_Description = "Liest das nächste Zeichen aus dem Standardeingabestream.   \r\nRückgabewerte: Das nächste Zeichen aus dem Eingabestream, bzw. -1, wenn keine weiteren Zeichen verfügbar sind."
    Dim sUserInput As String * 1 '256
    
    'Set Read = New cInteger
    'If bInIDE Then
        'wie soll man vom Debugwindow etwas einlesen?
    '    Read = AscW("j")
    'Else
    If Me.IOMode = IOModeStandard Then
        Dim BytesRead As Long
        Dim bOK As Boolean
        bOK = ReadConsoleW(hConsoleIn, StrPtr(sUserInput), Len(sUserInput), BytesRead, 0&)
        ' - 2, um vbCrLf abzuschneiden
        'Read = AscW(Left$(sUserInput, BytesRead - 2))
        'Read = AscW(sUserInput)
        Read = sUserInput
    Else
        bOK = ReadFile(hConsoleIn, StrPtr(sUserInput), Len(sUserInput), BytesRead, 0&)
    End If
End Function

'Public Shared Function ReadLine() As String
Public Function ReadLine() As String 'cString
Attribute ReadLine.VB_Description = "Liest die nächste Zeile von Zeichen aus dem Standardeingabestream. Rückgabewerte: Die nächste Zeile aus dem Eingabestream oder null, wenn keine weiteren Zeichen verfügbar sind."
    'SetConsoleMode hConsoleIn, ENABLE_PROCESSED_INPUT Or ENABLE_LINE_INPUT
    Dim sUserInput As String '* 256
    Dim BytesRead As Long
    If bInIDE Then
        'how to Read from VB-IDE Debug-Window?
        'ReadLine = "j"
    Else
        Dim bOK As Boolean
        sUserInput = String$(256, vbNullChar)
        If m_IOMode = IOModeStandard Then
            bOK = ReadConsoleW(hConsoleIn, StrPtr(sUserInput), ByVal Len(sUserInput), BytesRead, 0&)
        Else
            bOK = ReadFile(hConsoleIn, StrPtr(sUserInput), ByVal Len(sUserInput), BytesRead, 0&)
        End If
        If BytesRead > 2 Then
            ReadLine = Left$(sUserInput, BytesRead - 2) ' - 2, um vbCrLf abzuschneiden
        Else
            ReadLine = sUserInput
        End If
    End If
End Function

Public Sub ResetColor()
    'new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
    Dim succeeded As Boolean
    Dim bufferInfo As CONSOLE_SCREEN_BUFFER_INFO: bufferInfo = GetBufferInfo2(False, succeeded)
    If succeeded Then
        Dim attributes As Integer: attributes = m_defaultColors
        SetConsoleTextAttribute hConsoleOut, attributes
    End If
End Sub

'Public Shared ReadOnly Property [Error]() As System.IO.TextWriter
Public Property Get EError() 'As System_IO.TextWriter
    '
End Property

'Public Shared ReadOnly Property [In]() As System.IO.TextReader
Public Property Get IIn() 'As System_IO.TextReader
    '
End Property

'Public Shared ReadOnly Property Out() As System.IO.TextWriter
Public Property Get Out() 'As System_IO.TextWriter
    '
End Property
