VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "TBConsole"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit
Private WithEvents mTBInput  As TextBox
Attribute mTBInput.VB_VarHelpID = -1
Private WithEvents mTBOutput As TextBox
Attribute mTBOutput.VB_VarHelpID = -1
Private m_LastInputText As String
Private mGotIt As Boolean

Friend Sub New_(aTBInput As TextBox, ATBOutput As TextBox)
    Set mTBInput = aTBInput: Set mTBOutput = ATBOutput
    mTBOutput.Locked = True
    If Not mTBOutput.MultiLine Then MsgBox "The output textbox must be multiline!"
End Sub

Public Function ReadLine() As String
    Do Until mGotIt
        DoEvents
    Loop
    ReadLine = m_LastInputText
    mGotIt = False
End Function

Public Sub WriteLine(aLine As String)
    mTBOutput.Text = mTBOutput.Text & aLine & vbCrLf
    'scrolldown
    mTBOutput.SelStart = Len(mTBOutput.Text) '- 1
End Sub

Private Sub mTBInput_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = KeyCodeConstants.vbKeyReturn Then
        mGotIt = True
        m_LastInputText = mTBInput.Text
        mTBInput.Text = vbNullString
        WriteLine m_LastInputText
    End If
End Sub
Private Sub mTBInput_KeyPress(KeyAscii As Integer)
    If KeyAscii = KeyCodeConstants.vbKeyReturn Then KeyAscii = 0
End Sub

Private Sub mTBOutput_GotFocus()
    mTBInput.SetFocus
End Sub
