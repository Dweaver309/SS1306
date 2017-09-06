

' Code is from adafruit origianlly https://github.com/adafruit/Adafruit_SSD1306/
' Translated to VB class library by David Weaver

' OLED Driver has a very small memory footprint that requires about 2.5K

Imports System
Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware

Public Class OLED

    Private Const BufferSize As Integer = 1024
    Private Const Width As Integer = 128
    Private Const Height As Integer = 64

    Private Shared I2CAddress As UShort = 60  '  (&H3C)
    Private Shared Con As New I2CDevice.Configuration(I2CAddress, 400)
    Private Shared I2C As New I2CDevice(Con)

    Public Shared DisplayBuffer As Byte() = New Byte(BufferSize - 1) {}

    '  SSD1306 Commands
    Enum Cmds As Byte
        DisplayOff = &HAE
        DisplayClockDiv = &HD5
        DisplayRatio = &H80
        Multiplex = &HA8
        DisplayOffSet = &HD3
        NoOffSet = &H0
        StartLine = &H40
        ChargePump = &H8D
        VCCState = &H14
        MemoryMode = &H20
        LowColumn = &H0
        SegRemap = &HA1
        ComScanDec = &HC8
        SetComPins = &HDA
        DisableLRRemap = &H12
        SetContrast = &H81
        NoExternalVcc = &HCF
        PreCharge = &HD9
        InternalDC = &HF1
        ComDetect = &HD8
        SetComDetect = &H40
        DisplayResume = &HA4
        NormalDisplay = &HA6
        DeactivateScroll = &H2E
        DisplayOn = &HAF
        ColumnAddress = &H21
        Reset = &H0
        PageAddress = &H22
        PageEndAddress = &H37
    End Enum

    ''' <summary>
    ''' Set SSD1306 Defaults for I2c
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub InitializeDisplay()

        Command(Cmds.DisplayOff)
        Command(Cmds.DisplayClockDiv)
        Command(Cmds.DisplayRatio)
        Command(Cmds.Multiplex)
        Command(Height - 1)
        Command(Cmds.DisplayOffSet)
        Command(Cmds.NoOffSet)
        Command(Cmds.StartLine)
        Command(Cmds.ChargePump)
        Command(Cmds.VCCState)
        Command(Cmds.MemoryMode)
        Command(Cmds.LowColumn)
        Command(Cmds.SegRemap)
        Command(Cmds.ComScanDec)
        Command(Cmds.SetComPins)
        Command(Cmds.DisableLRRemap)
        Command(Cmds.SetContrast)
        Command(Cmds.NoExternalVcc)
        Command(Cmds.PreCharge)
        Command(Cmds.InternalDC)
        Command(Cmds.ComDetect)
        Command(Cmds.SetComDetect)
        Command(Cmds.DisplayResume)
        Command(Cmds.NormalDisplay)
        Command(Cmds.DeactivateScroll)
        Command(Cmds.DisplayOn)

    End Sub

    ''' <summary>
    ''' Puts one font character in the display buffer
    ''' At the horizonal and Line location
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Line"></param>
    ''' <param name="ASCII"></param>
    ''' <remarks></remarks>
    Private Shared Sub DrawCharacter(Horizonal As Integer, Line As Integer, ASCII As Integer)

        For i As Integer = 0 To 4
            'Display'im int As Integer = CInt(Strings.AscW(c))
            DisplayBuffer(Horizonal + (Line * 128)) = Font((ASCII * 5) + i)
            Horizonal += 1
        Next

    End Sub

    ''' <summary>
    ''' Called from optional Center in sub Write
    ''' </summary>
    ''' <param name="Str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CenterString(Str As String) As String

        Try
            Dim MaxStringLength As Integer = 21
            Dim spacesNeeded As Integer = CInt((MaxStringLength - Str.Length) / 2)
            Dim Spaces As String = String.Empty

            For i = 1 To spacesNeeded
                Spaces += " "
            Next

            Return Spaces & Str

        Catch ex As Exception
            Debug.Print("CenterString Error")
        End Try

        Return Str

    End Function

    ''' <summary>
    ''' Adds the string to the display
    ''' Calls DrawCharacter for each character
    ''' </summary>
    ''' <param name="Horizonal"></param>
    ''' <param name="line"></param>
    ''' <param name="Str"></param>
    ''' <remarks></remarks>
    Public Shared Sub Write(Horizonal As Integer, line As Integer, Str As String, Optional Center As Boolean = False)

        If Center Then Str = CenterString(Str)

        For Counter As Integer = 0 To Str.Length - 1

            'Dim c As Char = CChar(str.Substring(Counter, 1))
            Dim Asc As Integer = Strings.AscW(Str.Substring(Counter, 1))
            DrawCharacter(Horizonal, line, Asc)

            Horizonal += 6
            '  6 Pixels wide
            If Horizonal + 6 >= Width Then
                Horizonal = 0
                '  New Line
                line += 1
            End If

            '  Max lines
            If line >= Height / 8 Then

                Return
            End If
        Next

        Display()


    End Sub

    ''' <summary>
    ''' Clears the display
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ClearScreen()
        DisplayBuffer(0) = 0
        DisplayBuffer(1) = 0
        DisplayBuffer(2) = 0
        DisplayBuffer(3) = 0
        DisplayBuffer(4) = 0
        DisplayBuffer(5) = 0
        DisplayBuffer(6) = 0
        DisplayBuffer(7) = 0
        DisplayBuffer(8) = 0
        DisplayBuffer(9) = 0
        DisplayBuffer(10) = 0
        DisplayBuffer(11) = 0
        DisplayBuffer(12) = 0
        DisplayBuffer(13) = 0
        DisplayBuffer(14) = 0
        DisplayBuffer(15) = 0

        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 16, 16)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 32, 32)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 64, 64)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 128, 128)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 256, 256)
        Array.Copy(DisplayBuffer, 0, DisplayBuffer, 512, 512)

        Display()

    End Sub

    ''' <summary>
    ''' Creates the I2c transaction for the display setup
    ''' </summary>
    ''' <param name="Cmd"></param>
    ''' <remarks></remarks>
    Private Shared Sub Command(Cmd As Integer)

        Dim xActions As I2CDevice.I2CTransaction() = New I2CDevice.I2CTransaction(0) {}

        '   create write buffer (we need one byte)
        Dim Buff As Byte() = New Byte(1) {}

        Buff(1) = CByte(Cmd)

        xActions(0) = I2CDevice.CreateWriteTransaction(Buff)

        If I2C.Execute(xActions, 1000) = 0 Then
            Debug.Print("Failed to perform I2C transaction")
        End If

    End Sub

    ''' <summary>
    ''' Sends the Write and Clear I2c transactions to the display
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub Display()

        Command(Cmds.ColumnAddress)

        Command(Cmds.Reset)

        Command(Width - 1)

        Command(Cmds.PageAddress)

        Command(Cmds.Reset)

        Command(Cmds.PageEndAddress)

        Dim img As Byte() = New Byte((1024)) {}


        img(0) = Cmds.StartLine

        Array.Copy(DisplayBuffer, 0, img, 1, 1024)

        Dim xActions As I2CDevice.I2CTransaction() = New I2CDevice.I2CTransaction(0) {}

        xActions(0) = I2CDevice.CreateWriteTransaction(img)

        If I2C.Execute(xActions, 1000) = 0 Then
            Debug.Print("Failed to perform I2C transaction")
        End If

    End Sub

    ''' <summary>
    '''  5x7 font
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly Font As Byte() = New Byte() {&H0, &H0, &H0, &H0, &H0, &H3E, _
             &H5B, &H4F, &H5B, &H3E, &H3E, &H6B, _
             &H4F, &H6B, &H3E, &H1C, &H3E, &H7C, _
             &H3E, &H1C, &H18, &H3C, &H7E, &H3C, _
             &H18, &H1C, &H57, &H7D, &H57, &H1C, _
             &H1C, &H5E, &H7F, &H5E, &H1C, &H0, _
             &H18, &H3C, &H18, &H0, &HFF, &HE7, _
             &HC3, &HE7, &HFF, &H0, &H18, &H24, _
             &H18, &H0, &HFF, &HE7, &HDB, &HE7, _
             &HFF, &H30, &H48, &H3A, &H6, &HE, _
             &H26, &H29, &H79, &H29, &H26, &H40, _
             &H7F, &H5, &H5, &H7, &H40, &H7F, _
             &H5, &H25, &H3F, &H5A, &H3C, &HE7, _
             &H3C, &H5A, &H7F, &H3E, &H1C, &H1C, _
             &H8, &H8, &H1C, &H1C, &H3E, &H7F, _
             &H14, &H22, &H7F, &H22, &H14, &H5F, _
             &H5F, &H0, &H5F, &H5F, &H6, &H9, _
             &H7F, &H1, &H7F, &H0, &H66, &H89, _
             &H95, &H6A, &H60, &H60, &H60, &H60, _
             &H60, &H94, &HA2, &HFF, &HA2, &H94, _
             &H8, &H4, &H7E, &H4, &H8, &H10, _
             &H20, &H7E, &H20, &H10, &H8, &H8, _
             &H2A, &H1C, &H8, &H8, &H1C, &H2A, _
             &H8, &H8, &H1E, &H10, &H10, &H10, _
             &H10, &HC, &H1E, &HC, &H1E, &HC, _
             &H30, &H38, &H3E, &H38, &H30, &H6, _
             &HE, &H3E, &HE, &H6, &H0, &H0, _
             &H0, &H0, &H0, &H0, &H0, &H5F, _
             &H0, &H0, &H0, &H7, &H0, &H7, _
             &H0, &H14, &H7F, &H14, &H7F, &H14, _
             &H24, &H2A, &H7F, &H2A, &H12, &H23, _
             &H13, &H8, &H64, &H62, &H36, &H49, _
             &H56, &H20, &H50, &H0, &H8, &H7, _
             &H3, &H0, &H0, &H1C, &H22, &H41, _
             &H0, &H0, &H41, &H22, &H1C, &H0, _
             &H2A, &H1C, &H7F, &H1C, &H2A, &H8, _
             &H8, &H3E, &H8, &H8, &H0, &H80, _
             &H70, &H30, &H0, &H8, &H8, &H8, _
             &H8, &H8, &H0, &H0, &H60, &H60, _
             &H0, &H20, &H10, &H8, &H4, &H2, _
             &H3E, &H51, &H49, &H45, &H3E, &H0, _
             &H42, &H7F, &H40, &H0, &H72, &H49, _
             &H49, &H49, &H46, &H21, &H41, &H49, _
             &H4D, &H33, &H18, &H14, &H12, &H7F, _
             &H10, &H27, &H45, &H45, &H45, &H39, _
             &H3C, &H4A, &H49, &H49, &H31, &H41, _
             &H21, &H11, &H9, &H7, &H36, &H49, _
             &H49, &H49, &H36, &H46, &H49, &H49, _
             &H29, &H1E, &H0, &H0, &H14, &H0, _
             &H0, &H0, &H40, &H34, &H0, &H0, _
             &H0, &H8, &H14, &H22, &H41, &H14, _
             &H14, &H14, &H14, &H14, &H0, &H41, _
             &H22, &H14, &H8, &H2, &H1, &H59, _
             &H9, &H6, &H3E, &H41, &H5D, &H59, _
             &H4E, &H7C, &H12, &H11, &H12, &H7C, _
             &H7F, &H49, &H49, &H49, &H36, &H3E, _
             &H41, &H41, &H41, &H22, &H7F, &H41, _
             &H41, &H41, &H3E, &H7F, &H49, &H49, _
             &H49, &H41, &H7F, &H9, &H9, &H9, _
             &H1, &H3E, &H41, &H41, &H51, &H73, _
             &H7F, &H8, &H8, &H8, &H7F, &H0, _
             &H41, &H7F, &H41, &H0, &H20, &H40, _
             &H41, &H3F, &H1, &H7F, &H8, &H14, _
             &H22, &H41, &H7F, &H40, &H40, &H40, _
             &H40, &H7F, &H2, &H1C, &H2, &H7F, _
             &H7F, &H4, &H8, &H10, &H7F, &H3E, _
             &H41, &H41, &H41, &H3E, &H7F, &H9, _
             &H9, &H9, &H6, &H3E, &H41, &H51, _
             &H21, &H5E, &H7F, &H9, &H19, &H29, _
             &H46, &H26, &H49, &H49, &H49, &H32, _
             &H3, &H1, &H7F, &H1, &H3, &H3F, _
             &H40, &H40, &H40, &H3F, &H1F, &H20, _
             &H40, &H20, &H1F, &H3F, &H40, &H38, _
             &H40, &H3F, &H63, &H14, &H8, &H14, _
             &H63, &H3, &H4, &H78, &H4, &H3, _
             &H61, &H59, &H49, &H4D, &H43, &H0, _
             &H7F, &H41, &H41, &H41, &H2, &H4, _
             &H8, &H10, &H20, &H0, &H41, &H41, _
             &H41, &H7F, &H4, &H2, &H1, &H2, _
             &H4, &H40, &H40, &H40, &H40, &H40, _
             &H0, &H3, &H7, &H8, &H0, &H20, _
             &H54, &H54, &H78, &H40, &H7F, &H28, _
             &H44, &H44, &H38, &H38, &H44, &H44, _
             &H44, &H28, &H38, &H44, &H44, &H28, _
             &H7F, &H38, &H54, &H54, &H54, &H18, _
             &H0, &H8, &H7E, &H9, &H2, &H18, _
             &HA4, &HA4, &H9C, &H78, &H7F, &H8, _
             &H4, &H4, &H78, &H0, &H44, &H7D, _
             &H40, &H0, &H20, &H40, &H40, &H3D, _
             &H0, &H7F, &H10, &H28, &H44, &H0, _
             &H0, &H41, &H7F, &H40, &H0, &H7C, _
             &H4, &H78, &H4, &H78, &H7C, &H8, _
             &H4, &H4, &H78, &H38, &H44, &H44, _
             &H44, &H38, &HFC, &H18, &H24, &H24, _
             &H18, &H18, &H24, &H24, &H18, &HFC, _
             &H7C, &H8, &H4, &H4, &H8, &H48, _
             &H54, &H54, &H54, &H24, &H4, &H4, _
             &H3F, &H44, &H24, &H3C, &H40, &H40, _
             &H20, &H7C, &H1C, &H20, &H40, &H20, _
             &H1C, &H3C, &H40, &H30, &H40, &H3C, _
             &H44, &H28, &H10, &H28, &H44, &H4C, _
             &H90, &H90, &H90, &H7C, &H44, &H64, _
             &H54, &H4C, &H44, &H0, &H8, &H36, _
             &H41, &H0, &H0, &H0, &H77, &H0, _
             &H0, &H0, &H41, &H36, &H8, &H0, _
             &H2, &H1, &H2, &H4, &H2, &H3C, _
             &H26, &H23, &H26, &H3C, &H1E, &HA1, _
             &HA1, &H61, &H12, &H3A, &H40, &H40, _
             &H20, &H7A, &H38, &H54, &H54, &H55, _
             &H59, &H21, &H55, &H55, &H79, &H41, _
             &H21, &H54, &H54, &H78, &H41, &H21, _
             &H55, &H54, &H78, &H40, &H20, &H54, _
             &H55, &H79, &H40, &HC, &H1E, &H52, _
             &H72, &H12, &H39, &H55, &H55, &H55, _
             &H59, &H39, &H54, &H54, &H54, &H59, _
             &H39, &H55, &H54, &H54, &H58, &H0, _
             &H0, &H45, &H7C, &H41, &H0, &H2, _
             &H45, &H7D, &H42, &H0, &H1, &H45, _
             &H7C, &H40, &HF0, &H29, &H24, &H29, _
             &HF0, &HF0, &H28, &H25, &H28, &HF0, _
             &H7C, &H54, &H55, &H45, &H0, &H20, _
             &H54, &H54, &H7C, &H54, &H7C, &HA, _
             &H9, &H7F, &H49, &H32, &H49, &H49, _
             &H49, &H32, &H32, &H48, &H48, &H48, _
             &H32, &H32, &H4A, &H48, &H48, &H30, _
             &H3A, &H41, &H41, &H21, &H7A, &H3A, _
             &H42, &H40, &H20, &H78, &H0, &H9D, _
             &HA0, &HA0, &H7D, &H39, &H44, &H44, _
             &H44, &H39, &H3D, &H40, &H40, &H40, _
             &H3D, &H3C, &H24, &HFF, &H24, &H24, _
             &H48, &H7E, &H49, &H43, &H66, &H2B, _
             &H2F, &HFC, &H2F, &H2B, &HFF, &H9, _
             &H29, &HF6, &H20, &HC0, &H88, &H7E, _
             &H9, &H3, &H20, &H54, &H54, &H79, _
             &H41, &H0, &H0, &H44, &H7D, &H41, _
             &H30, &H48, &H48, &H4A, &H32, &H38, _
             &H40, &H40, &H22, &H7A, &H0, &H7A, _
             &HA, &HA, &H72, &H7D, &HD, &H19, _
             &H31, &H7D, &H26, &H29, &H29, &H2F, _
             &H28, &H26, &H29, &H29, &H29, &H26, _
             &H30, &H48, &H4D, &H40, &H20, &H38, _
             &H8, &H8, &H8, &H8, &H8, &H8, _
             &H8, &H8, &H38, &H2F, &H10, &HC8, _
             &HAC, &HBA, &H2F, &H10, &H28, &H34, _
             &HFA, &H0, &H0, &H7B, &H0, &H0, _
             &H8, &H14, &H2A, &H14, &H22, &H22, _
             &H14, &H2A, &H14, &H8, &HAA, &H0, _
             &H55, &H0, &HAA, &HAA, &H55, &HAA, _
             &H55, &HAA, &H0, &H0, &H0, &HFF, _
             &H0, &H10, &H10, &H10, &HFF, &H0, _
             &H14, &H14, &H14, &HFF, &H0, &H10, _
             &H10, &HFF, &H0, &HFF, &H10, &H10, _
             &HF0, &H10, &HF0, &H14, &H14, &H14, _
             &HFC, &H0, &H14, &H14, &HF7, &H0, _
             &HFF, &H0, &H0, &HFF, &H0, &HFF, _
             &H14, &H14, &HF4, &H4, &HFC, &H14, _
             &H14, &H17, &H10, &H1F, &H10, &H10, _
             &H1F, &H10, &H1F, &H14, &H14, &H14, _
             &H1F, &H0, &H10, &H10, &H10, &HF0, _
             &H0, &H0, &H0, &H0, &H1F, &H10, _
             &H10, &H10, &H10, &H1F, &H10, &H10, _
             &H10, &H10, &HF0, &H10, &H0, &H0, _
             &H0, &HFF, &H10, &H10, &H10, &H10, _
             &H10, &H10, &H10, &H10, &H10, &HFF, _
             &H10, &H0, &H0, &H0, &HFF, &H14, _
             &H0, &H0, &HFF, &H0, &HFF, &H0, _
             &H0, &H1F, &H10, &H17, &H0, &H0, _
             &HFC, &H4, &HF4, &H14, &H14, &H17, _
             &H10, &H17, &H14, &H14, &HF4, &H4, _
             &HF4, &H0, &H0, &HFF, &H0, &HF7, _
             &H14, &H14, &H14, &H14, &H14, &H14, _
             &H14, &HF7, &H0, &HF7, &H14, &H14, _
             &H14, &H17, &H14, &H10, &H10, &H1F, _
             &H10, &H1F, &H14, &H14, &H14, &HF4, _
             &H14, &H10, &H10, &HF0, &H10, &HF0, _
             &H0, &H0, &H1F, &H10, &H1F, &H0, _
             &H0, &H0, &H1F, &H14, &H0, &H0, _
             &H0, &HFC, &H14, &H0, &H0, &HF0, _
             &H10, &HF0, &H10, &H10, &HFF, &H10, _
             &HFF, &H14, &H14, &H14, &HFF, &H14, _
             &H10, &H10, &H10, &H1F, &H0, &H0, _
             &H0, &H0, &HF0, &H10, &HFF, &HFF, _
             &HFF, &HFF, &HFF, &HF0, &HF0, &HF0, _
             &HF0, &HF0, &HFF, &HFF, &HFF, &H0, _
             &H0, &H0, &H0, &H0, &HFF, &HFF, _
             &HF, &HF, &HF, &HF, &HF, &H38, _
             &H44, &H44, &H38, &H44, &H7C, &H2A, _
             &H2A, &H3E, &H14, &H7E, &H2, &H2, _
             &H6, &H6, &H2, &H7E, &H2, &H7E, _
             &H2, &H63, &H55, &H49, &H41, &H63, _
             &H38, &H44, &H44, &H3C, &H4, &H40, _
             &H7E, &H20, &H1E, &H20, &H6, &H2, _
             &H7E, &H2, &H2, &H99, &HA5, &HE7, _
             &HA5, &H99, &H1C, &H2A, &H49, &H2A, _
             &H1C, &H4C, &H72, &H1, &H72, &H4C, _
             &H30, &H4A, &H4D, &H4D, &H30, &H30, _
             &H48, &H78, &H48, &H30, &HBC, &H62, _
             &H5A, &H46, &H3D, &H3E, &H49, &H49, _
             &H49, &H0, &H7E, &H1, &H1, &H1, _
             &H7E, &H2A, &H2A, &H2A, &H2A, &H2A, _
             &H44, &H44, &H5F, &H44, &H44, &H40, _
             &H51, &H4A, &H44, &H40, &H40, &H44, _
             &H4A, &H51, &H40, &H0, &H0, &HFF, _
             &H1, &H3, &HE0, &H80, &HFF, &H0, _
             &H0, &H8, &H8, &H6B, &H6B, &H8, _
             &H36, &H12, &H36, &H24, &H36, &H6, _
             &HF, &H9, &HF, &H6, &H0, &H0, _
             &H18, &H18, &H0, &H0, &H0, &H10, _
             &H10, &H0, &H30, &H40, &HFF, &H1, _
             &H1, &H0, &H1F, &H1, &H1, &H1E, _
             &H0, &H19, &H1D, &H17, &H12, &H0, _
             &H3C, &H3C, &H3C, &H3C, &H0, &H0, _
             &H0, &H0, &H0}
End Class
