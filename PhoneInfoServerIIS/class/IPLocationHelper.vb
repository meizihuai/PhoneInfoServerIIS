Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class IPLocationHelper
    Public Class IPlocationResult
        Public code As Integer
        Public data As IPLocation
    End Class
    Public Class IPLocation
        Public area As String
        Public area_id As String
        Public city As String
        Public city_id As String
        Public country As String
        Public country_id As String
        Public county As String
        Public county_id As String
        Public ip As String
        Public isp As String
        Public isp_id As String
        Public region As String
        Public region_id As String
    End Class
    Public Shared Function Ip2IPLocation(iP As String) As IPLocation
        If iP = "" Then Return Nothing
        Dim url As String = "http://ip.taobao.com/service/getIpInfo2.php?ip=" & iP
        Dim result As String = GetHTML(url, New CookieContainer(), "")
        If result = "" Then Return Nothing
        Dim ir As IPlocationResult = JsonConvert.DeserializeObject(result, GetType(IPlocationResult))
        If IsNothing(ir) Then Return Nothing
        If ir.code = 1 Then Return Nothing
        Return ir.data
    End Function
    Private Shared Function GetHTML(ByVal uri As String, ByVal cook As CookieContainer, ByVal msg As String) As String
        Try
            Dim req As HttpWebRequest = WebRequest.Create(uri & msg)
            req.Accept = "*/*"
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
            req.CookieContainer = cook
            req.KeepAlive = True
            req.ContentType = "aplication/x-www-form-urlencoded"
            req.Method = "GET"
            req.Timeout = 10000
            Dim rp As HttpWebResponse = req.GetResponse
            Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.Default).ReadToEnd
            Return str
        Catch ex As Exception

        End Try
        Return ""
    End Function

End Class
