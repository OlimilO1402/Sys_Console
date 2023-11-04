VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Console"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit
'diverse API-Ddeklarationen
Private Declare Function AllocConsole Lib "kernel32" () As Long
Private Declare Function FreeConsole Lib "kernel32" () As Long
Private Declare Function GetStdHandle Lib "kernel32" (ByVal nStdHandle As Long) As Long
Private Declare Function ReadConsole Lib "kernel32" Alias "ReadConsoleA" (ByVal hConsoleInput As Long, ByVal lpBuffer As String, ByVal nNumberOfCharsToRead As Long, ByRef lpNumberOfCharsRead As Long, lpReserved As Long) As Long
Private Declare Function SetConsoleMode Lib "kernel32" (ByVal hConsoleOutput As Long, dwMode As Long) As Long
Private Declare Function SetConsoleTextAttribute Lib "kernel32" (ByVal hConsoleOutput As Long, ByVal wAttributes As Long) As Long
Private Declare Function SetConsoleTitle Lib "kernel32" Alias "SetConsoleTitleA" (ByVal lpConsoleTitle As String) As Long
Private Declare Function WriteConsole Lib "kernel32" Alias "WriteConsoleA" (ByVal hConsoleOutput As Long, ByVal lpBuffer As Any, ByVal nNumberOfCharsToWrite As Long, ByRef lpNumberOfCharsWritten As Long, lpReserved As Long) As Long
Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As Long) As Long

Private Const STD_INPUT_HANDLE     As Long = -10&
Private Const STD_OUTPUT_HANDLE    As Long = -11&
Private Const STD_ERROR_HANDLE     As Long = -12&

Private Const FOREGROUND_BLUE      As Long = &H1&
Private Const FOREGROUND_GREEN     As Long = &H2&
Private Const FOREGROUND_RED       As Long = &H4&
Private Const FOREGROUND_INTENSITY As Long = &H8&
Private Const BACKGROUND_BLUE      As Long = &H10&
Private Const BACKGROUND_GREEN     As Long = &H20&
Private Const BACKGROUND_RED       As Long = &H40&
Private Const BACKGROUND_INTENSITY As Long = &H80&

Private Const ENABLE_PROCESSED_INPUT As Long = &H1&
Private Const ENABLE_LINE_INPUT      As Long = &H2&
Private Const ENABLE_ECHO_INPUT      As Long = &H4&
Private Const ENABLE_WINDOW_INPUT    As Long = &H8&
Private Const ENABLE_MOUSE_INPUT     As Long = &H10&

Private Const ENABLE_PROCESSED_OUTPUT   As Long = &H1&
Private Const ENABLE_WRAP_AT_EOL_OUTPUT As Long = &H2&

Private hConsoleIn As Long
Private hConsoleOut As Long
Private hConsoleErr As Long

Private Sub Class_Initialize()
  Call AllocConsole
  Call SetConsoleTitle("VB6Win32 Console - by www.MBO-Ing.com")
  
  hConsoleIn = GetStdHandle(STD_INPUT_HANDLE)
  hConsoleOut = GetStdHandle(STD_OUTPUT_HANDLE)
  hConsoleErr = GetStdHandle(STD_ERROR_HANDLE)

End Sub

Public Sub wWrite(ByVal varValue As Variant)
'Schreibt die Textdarstellung des angegebenen Wertes in den Standardausgabestream.
Dim BytesWritten As Long
Dim szOut As String
  szOut = CStr(varValue)
  Debug.Print "Len(szOut): " & CStr(Len(szOut))
  Call WriteConsole(hConsoleOut, szOut, Len(szOut), BytesWritten, 0&)
  Debug.Print "Write BytesWritten: " & CStr(BytesWritten)
End Sub

Public Sub WriteLine(Optional varValue As Variant)
Dim BytesWritten As Long
Dim szOut As String
  szOut = CStr(varValue)
  szOut = szOut & vbCrLf
  Debug.Print "Len(szOut): " & CStr(Len(szOut))
  Call WriteConsole(hConsoleOut, szOut, Len(szOut), BytesWritten, 0&)
  Debug.Print "Write BytesWritten: " & CStr(BytesWritten)
End Sub

Public Function Read() As String
'Liest das nächste Zeichen aus dem Standardeingabestream.
Dim sUserInput As String * 1
Dim BytesRead As Long
  
  Call ReadConsole(hConsoleIn, sUserInput, Len(sUserInput), BytesRead, 0&)
  Debug.Print "Read BytesRead: " & CStr(BytesRead)
  ' - 2, um vbCrLf abzuschneiden
  'Read = AscW(Left$(sUserInput, BytesRead - 2))
  Read = Chr$(Asc(sUserInput))
End Function

Public Function ReadLine() As String
'Liest die nächste Zeile von Zeichen aus dem Standardeingabestream.
'Rückgabewerte:
'Die nächste Zeile aus dem Eingabestream oder null,
'wenn keine weiteren Zeichen verfügbar sind.
Dim sUserInput As String * 256
Dim BytesRead As Long
    
  Call ReadConsole(hConsoleIn, sUserInput, Len(sUserInput), BytesRead, 0&)
  Debug.Print "ReadLine BytesRead: " & CStr(BytesRead)
  ' - 2, um vbCrLf abzuschneiden
  If BytesRead > 2 Then
    ReadLine = Left$(sUserInput, BytesRead - 2)
  Else
    ReadLine = sUserInput
  End If
End Function

Private Sub Class_Terminate()
  Call CloseHandle(hConsoleIn)
  Call CloseHandle(hConsoleOut)
  Call CloseHandle(hConsoleErr)
  FreeConsole
End Sub

