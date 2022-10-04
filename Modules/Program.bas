Attribute VB_Name = "Program"
Option Explicit
Public Console As Console

Sub Main()
    Set Console = New Console
    Console.IsInIDE = False
    FMain.Show
End Sub
