Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware
'Imports SecretLabs.NETMF.Hardware
'Imports SecretLabs.NETMF.Hardware.Netduino
Imports System
'Imports System.Net
'  **For file system
Imports System.IO

Module Module1

    Sub Main()

        OLED.InitializeDisplay()

        Dim FormatedDateString As String = "dddd hh:mm:ss"

        While (True)
            Dim DS As Date = Date.Now
            Dim str As String = DS.ToString(FormatedDateString)

            OLED.Write(0, 0, str, True)
            
            Thread.Sleep(1000)

        End While

    End Sub

End Module
