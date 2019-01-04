Public Class QoEHTTPInfo
    Public DATETIME As String
    Public VMOS As Integer
    Public RESPONSETIMESCORE As Integer
    Public TOTALBUFFERTIMESCORE As Integer
    Public DNSTIMESCORE As Integer
    Public DOWNLOADSPEEDSCORE As Integer
    Public WHITESCREENTIMESCORE As Integer
    Public RESPONSETIME As Long
    Public TOTALBUFFERTIME As Long
    Public DNSTIME As Long
    Public DOWNLOADSPEED As Long
    Public WHITESCREENTIME As Long
    Public ISUPLOADDATATIMELY As Integer
    Public HTTPTESTRESULTLIST As List(Of HTTPTestInfo)
    Public pi As PhoneInfo
    Sub New()

    End Sub
    Public Class HTTPTestInfo
        Public URL As String
        Public RESPONSETIME As Long
        Public TOTALBUFFERTIME As Long
        Public DNSTIME As Long
        Public DOWNLOADSPEED As Long
        Public HTMLBUFFERSIZE As Long
        Public WHITESCREENTIME As Long
    End Class
End Class
