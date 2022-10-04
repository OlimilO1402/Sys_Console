VERSION 5.00
Begin VB.Form FMain 
   Caption         =   "Form1"
   ClientHeight    =   3015
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   4560
   LinkTopic       =   "Form1"
   ScaleHeight     =   3015
   ScaleWidth      =   4560
   StartUpPosition =   3  'Windows-Standard
   Begin VB.CommandButton BtnTestConsoleBeep 
      Caption         =   "Test Console.Beep"
      Height          =   375
      Left            =   120
      TabIndex        =   2
      Top             =   1080
      Width           =   2295
   End
   Begin VB.CommandButton BtnTestConsoleColors 
      Caption         =   "Test Console.Colors"
      Height          =   375
      Left            =   120
      TabIndex        =   1
      Top             =   600
      Width           =   2295
   End
   Begin VB.CommandButton BtnTestConsoleTitle 
      Caption         =   "Test Console.Title"
      Height          =   375
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   2295
   End
   Begin VB.CommandButton BtnTestConsoleReadWrite 
      Caption         =   "Test Console.Read+Write"
      Height          =   375
      Left            =   120
      TabIndex        =   4
      Top             =   2040
      Width           =   2295
   End
   Begin VB.CommandButton BtnTestConsoleCursor 
      Caption         =   "Test Console.Cursor"
      Height          =   375
      Left            =   120
      TabIndex        =   3
      Top             =   1560
      Width           =   2295
   End
End
Attribute VB_Name = "FMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub Form_Load()
    Me.Caption = App.EXEName & " v" & App.Major & "." & App.Minor & "." & App.Revision
End Sub

Private Sub BtnTestConsoleTitle_Click()
    TestConsoleTitle
End Sub

Private Sub BtnTestConsoleColors_Click()
    TestConsoleColors
End Sub

Private Sub BtnTestConsoleBeep_Click()
    TestConsoleBeep
End Sub

Private Sub BtnTestConsoleCursor_Click()
    TestConsoleCursor
End Sub

Private Sub BtnTestConsoleReadWrite_Click()
    TestConsoleReadWrite
End Sub

Sub TestConsoleTitle()
    MsgBox "Console.Title=" & Console.Title
    Console.Title = "VB64free .now empowered by MBO-Ing.com"
    MsgBox "Console.Title=" & Console.Title
    MsgBox "All Handles valid? " & Console.IsHandlesValid
End Sub
Sub TestConsoleColors()
    Dim s As String: s = " Dings "
    Console.BackgroundColor = ConsoleColor_Black
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkBlue
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkCyan
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkGray
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkGreen
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkMagenta
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkRed
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_DarkYellow
    Console.WWrite s
    
    Console.BackgroundColor = ConsoleColor_Blue
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Cyan
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Gray
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Green
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Magenta
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Red
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_White
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Yellow
    Console.WWrite s
    
    Console.WriteLine ""
    
    Console.BackgroundColor = ConsoleColor_White
    Console.ForegroundColor = ConsoleColor_Black
    Console.WWrite s
    Console.BackgroundColor = ConsoleColor_Black
    Console.ForegroundColor = ConsoleColor_DarkBlue
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkCyan
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkGray
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkGreen
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkMagenta
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkRed
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_DarkYellow
    Console.WWrite s
    
    Console.ForegroundColor = ConsoleColor_Blue
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Cyan
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Gray
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Green
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Magenta
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Red
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_White
    Console.WWrite s
    Console.ForegroundColor = ConsoleColor_Yellow
    Console.WWrite s
    
    Console.WriteLine ""

    Dim i As Integer
    For i = 0 To 15
        Console.BackgroundColor = i
        Console.ForegroundColor = 15 - i
        Console.WWrite s
    Next
    
End Sub
Sub TestConsoleBeep()
    Console.Beep
    Console.BeepF 220, 100
    Console.BeepF 440, 100
    Console.BeepF 880, 100
    Console.BeepF 1760, 100
    Console.BeepF 1760, 100
    Console.BeepF 880, 100
    Console.BeepF 440, 100
    Console.BeepF 220, 100
End Sub
Sub TestConsoleCursor()
    TestGetCursor
    TestSetCursor
    TestGetCursor
End Sub
Sub TestGetCursor()
    Dim l As Long:      l = Console.CursorLeft
    Dim t As Long:      t = Console.CursorTop
    Dim sz As Long:    sz = Console.CursorSize
    Dim vs As Boolean: vs = Console.CursorVisible
    MsgBox "Cursor{Left: " & l & "; Top: " & t & "; size: " & sz & "; Visible: " & vs & "}"
End Sub
Sub TestSetCursor()
    Console.CursorLeft = 30
    Console.CursorTop = 10
End Sub
Sub TestConsoleReadWrite()
    Console.WWrite "Frage Ja oder Nein? (Ja/Nein): "
    Dim s As String
    s = Console.ReadLine
    Console.WriteLine "Deine Antwort war: " & s
    Console.WWrite "Nenne eine Zahl zwischen 1 und 10: "
    s = Console.ReadLine
    Console.WriteLine "Deine Antwort war: " & s
End Sub
