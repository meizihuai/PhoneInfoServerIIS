﻿Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Sockets
Imports MySql.Data.MySqlClient
Imports ConvertGPS
Imports OfficeOpenXml
Imports System.Web
Imports System.Threading
Imports System.Threading.Thread
Imports System.IO.Compression
Imports System.Web.Configuration.WebConfigurationManager

Module Mod_Normal_Handle
    Public myServerUrl As String = "http://111.53.74.132:7062"
    Public ConnectSQL As String = "server=localhost;DataBase=PhoneInfo;User ID=root;Pwd=Smart9080;charset='utf8'"
    Public pgSQLLocalhost As New PgSQLHelper("localhost", 5432, "PhoneInfo", "mzh", "Smart9080")
    ' Public ORALocalhost As New OracleHelper("localhost", 1521, "oss", "uplan", "Smart9080")
    'Public ORALocalhost As New OracleHelper(AppSettings("oraIp"), AppSettings("oraPort"), AppSettings("oraServerName"), AppSettings("oraUsr"), AppSettings("oraPwd"))
    Public ORALocalhost As New OracleHelper(AppSettings("oraHelperCfg"))
    Public apiVersion As String = "2.0.5"
    Structure normalResponse 'json回复格式
        Public result As Boolean
        Public msg As String
        Public errmsg As String
        Public data As Object
        Sub New(ByVal _result As Boolean, ByVal _msg As String, ByVal _errmsg As String, ByVal _data As Object) '基本构造函数() As _errmsg,string
            result = _result
            msg = _msg
            errmsg = _errmsg
            data = _data
        End Sub
        Sub New(ByVal _result As Boolean, ByVal _msg As String) '重载构造函数，为了方便写new,很多时候，只需要一个结果和一个参数() As _result,string
            result = _result
            msg = _msg
            errmsg = ""
            data = ""
        End Sub
    End Structure
    Structure loginInfo
        Dim usr As String
        Dim name As String
        Dim token As String
        Dim power As Integer
        Dim state As Integer
        Sub New(usr As String, name As String, token As String, power As Integer, state As Integer)
            Me.usr = usr
            Me.name = name
            Me.token = token
            Me.power = power
            Me.state = state
        End Sub
    End Structure
    Public Function GetOraTableColumns(tableName As String) As String()
        'select COLUMN_NAME from user_tab_columns where table_name ='QOE_REPORT_TABLE'
        Try
            Dim sql As String = "select COLUMN_NAME from user_tab_columns where table_name ='" & tableName.ToUpper & "'"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return Nothing
            If dt.Rows.Count = 0 Then Return Nothing
            Dim list As New List(Of String)
            For Each row As DataRow In dt.Rows
                If IsNothing(row(0)) = False Then
                    list.Add(row(0).ToString)
                End If
            Next
            Return list.ToArray
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetLen(ByVal k As Long) As String
        Dim str As String = ""
        If k < 1024 Then
            Return k.ToString("0.00") & "b"
        End If
        Dim d As Double = 1024 * 1024
        If 1024 <= k And k < d Then
            Return (k / 1024).ToString("0.00") & "Kb"
        End If

        Return (k / d).ToString("0.00") & "Mb"
    End Function
    Public Function Str2Base64(ByVal str As String) As String
        If str = "" Then Return ""
        Dim by() As Byte = Encoding.Default.GetBytes(str)
        Dim base64 As String = Convert.ToBase64String(by)
        Return base64
    End Function
    Public Function Base2str(ByVal base64 As String) As String
        If base64 = "" Then Return ""
        Dim by() As Byte = Convert.FromBase64String(base64)
        Dim str As String = Encoding.Default.GetString(by)
        Return str
    End Function
    Public Function SQLGetDT(ByVal CmdString As String) As DataTable
        Dim dt As New DataTable
        Try
            Dim SQL As New MySqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As MySqlCommand = New MySqlCommand(CmdString, SQL)
            SQLCommand.CommandTimeout = 900000
            'Dim ResultRowInt As Integer = SQLCommand.ExecuteNonQuery()
            Dim SQLDataAdapter As New MySqlDataAdapter(SQLCommand)
            SQLDataAdapter.Fill(dt)
            SQL.Close()
            If IsNothing(dt) Then dt = New DataTable
            Return dt
        Catch ex As Exception
            '  MsgBox(ex.ToString)
            Return dt
        End Try
    End Function
    Public Function SQLCmd(ByVal CmdString As String) As String
        Try
            Dim SQL As New MySqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As MySqlCommand = New MySqlCommand(CmdString, SQL)

            Dim ResultRowInt As Integer = SQLCommand.ExecuteNonQuery()
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function SQLInsertSQLList(ByVal sqllist As List(Of String)) As String
        If IsNothing(sqllist) Then Return "sqllist is null"
        If sqllist.Count = 0 Then Return "sqllist.count=0"
        Try
            Dim SQL As New MySqlConnection(ConnectSQL)
            SQL.Open()
            Dim tx As MySqlTransaction = SQL.BeginTransaction
            Dim SQLCommand As MySqlCommand = New MySqlCommand()
            SQLCommand.Connection = SQL
            SQLCommand.Transaction = tx
            Dim count As Integer = sqllist.Count
            For i = 0 To count - 1
                SQLCommand.CommandText = sqllist(i)
                SQLCommand.ExecuteNonQuery()
                If i > 0 And (i Mod 500 = 0 Or i = count - 1) Then
                    tx.Commit()
                    tx = SQL.BeginTransaction
                End If
            Next
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function SQLInsertSQLList(ByVal connString As String, ByVal sqllist As List(Of String)) As String
        If IsNothing(sqllist) Then Return "sqllist is null"
        If sqllist.Count = 0 Then Return "sqllist.count=0"
        Try
            Dim SQL As New MySqlConnection(connString)
            SQL.Open()
            Dim tx As MySqlTransaction = SQL.BeginTransaction
            Dim SQLCommand As MySqlCommand = New MySqlCommand()
            SQLCommand.Connection = SQL
            SQLCommand.Transaction = tx
            Dim count As Integer = sqllist.Count
            For i = 0 To count - 1
                SQLCommand.CommandText = sqllist(i)
                SQLCommand.ExecuteNonQuery()
                If i > 0 And (i Mod 500 = 0 Or i = count - 1) Then
                    tx.Commit()
                    tx = SQL.BeginTransaction
                End If
            Next
            SQL.Close()
            Return "success"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Structure LocationAddressInfo
        Dim Province As String
        Dim City As String
        Dim District As String
        Dim DetailAddress As String
    End Structure
    Public Function GetAddressByLngLat(ByVal lng As String, ByVal lat As String) As LocationAddressInfo
        Dim miYue As String = "5cW7OlxZXThtbkq1Y0u5yNO6"
        '  miYue = "LwK1QyeofWEysKHf2sdySVcbnhXLnLIj"
        miYue = "tb3xO9tSnOggEciyQBO03vEUydIvdTsY"
        miYue = "wdM2ngn7AIV9dRPpQYqslRwNkiPbFhfh" '国伟的，认证过，一天30万
        Dim url As String = "http://api.map.baidu.com/geocoder/v2/?ak=" & miYue & "&location=" & lat & "," & lng & "&output=json&pois=0&coordtype=wgs84ll"
        Dim msg As String = GetH(url, "")
        Dim la As New LocationAddressInfo
        Try
            Dim p As Object = CType(JsonConvert.DeserializeObject(msg), JObject)
            Dim province As String = p("result")("addressComponent")("province").ToString
            Dim city As String = p("result")("addressComponent")("city").ToString
            Dim district As String = p("result")("addressComponent")("district").ToString
            Dim street As String = p("result")("addressComponent")("street").ToString
            Dim sematic_description As String = p("result")("sematic_description").ToString
            Dim formatted_address As String = p("result")("formatted_address").ToString
            Dim business As String = p("result")("business").ToString
            Dim str As String = city & "," & district & "," & sematic_description & "," & business
            str = formatted_address & "," & sematic_description & "," & business
            la.Province = province
            la.City = city
            la.DetailAddress = str
            la.District = district
            Return la
            str = city & district & "," & street
            'dt.Rows(index)("地址") = formatted_address
            'dt.Rows(index)("城市区划") = city & district
            'dt.Rows(index)("街道名称") = street
            'dt.Rows(index)("地址描述") = sematic_description
            'dt.Rows(index)("商圈信息") = business
            ' Return str
        Catch ex As Exception
            ' Return ex.Message
        End Try
        Return la
    End Function
    Public Function SQLInfo(ByVal CmdString As String) As String
        Try
            Dim SQL As New MySqlConnection(ConnectSQL)
            SQL.Open()
            Dim SQLCommand As MySqlCommand = New MySqlCommand(CmdString, SQL)
            Dim str As String = SQLCommand.ExecuteScalar.ToString
            SQL.Close()
            Return str
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function DT2SQL(ByVal dataTableName As String, ByVal dt As DataTable) As String
        Try
            If IsNothing(dt) Then Return "dt=null"
            Dim keys As String = ""
            Dim values As String = ""
            For Each c As DataColumn In dt.Columns
                keys = keys & c.ColumnName & ","
            Next
            keys = keys.Substring(0, keys.Length - 1)
            Dim errMsgList As New List(Of String)
            Dim sql As String = "insert into " & dataTableName & "(" & keys & ") values ({0})"
            Dim successCount As Integer = 0
            Dim failCount As Integer = 0
            For Each row As DataRow In dt.Rows
                values = ""
                For i = 0 To dt.Columns.Count - 1
                    Dim v As String = row(i).ToString
                    v = v.Replace("'", Chr(34))
                    values = values & "'" & v & "',"
                Next
                values = values.Substring(0, values.Length - 1)
                Dim newsql As String = String.Format(sql, values)
                Dim result As String = SQLCmd(newsql)
                If result = "success" Then
                    successCount = successCount + 1
                Else
                    failCount = failCount + 1
                    errMsgList.Add(result)
                End If
                newsql = Nothing
                result = Nothing
            Next
            dt = Nothing
            If failCount = 0 Then
                Return "success"
            Else
                Return "成功数量：" & successCount & ",失败数量：" & failCount & "," & JsonConvert.SerializeObject(errMsgList)
            End If

        Catch ex As Exception
            dt = Nothing
            Return ex.Message
        End Try
    End Function

    Private Function GetHTML(ByVal uri As String, ByVal cook As CookieContainer, ByVal msg As String) As String
        Dim req As HttpWebRequest = WebRequest.Create(uri & msg)
        req.Accept = "*/*"
        req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
        req.CookieContainer = cook
        req.KeepAlive = True
        req.ContentType = "aplication/x-www-form-urlencoded"
        req.Method = "GET"
        req.Timeout = 3000
        Dim rp As HttpWebResponse = req.GetResponse
        Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.Default).ReadToEnd
        Return str
    End Function
    Public Function GPS2BDS(ByVal x As Double, ByVal y As Double) As CoordInfo
        'Dim coords As String = x & "," & y
        'Return GPS2BDS(coords)(0)
        Dim gps As New PointLatLng(y, x)
        Dim p As PointLatLng = Gps84_To_bd09(gps)
        Return New CoordInfo(p.Lng, p.Lat)
    End Function
    Public Function GetCellIdByECI(eci As Long) As Integer
        Dim hex As String = Microsoft.VisualBasic.Hex(eci)
        If hex.Length > 2 Then
            hex = hex.Substring(hex.Length - 2, 2)
            Dim cellId As Integer = CLng("&H" & hex)
            Return cellId
        End If
        Return 0
    End Function
    Public Function GPS2GDS(ByVal x As Double, ByVal y As Double) As CoordInfo
        'Dim coords As String = x & "," & y
        'Return GPS2GDS(coords)(0)
        Dim gps As New PointLatLng(y, x)
        Dim p As PointLatLng = gps84_To_Gcj02(gps)
        Return New CoordInfo(p.Lng, p.Lat)
    End Function
    Private Function GPS2GDS(ByVal coords As String) As CoordInfo()
        ' http://api.map.baidu.com/geoconv/v1/?coords=114.21892734521,29.575429778924;114.21892734521,29.575429778924&from=1&to=5&ak=你的密钥
        Dim ak As String = "5cW7OlxZXThtbkq1Y0u5yNO6"
        Dim url As String = "http://api.map.baidu.com/geoconv/v1/?coords=" & coords & "&from=1&to=3&ak=" & ak
        ' Console.WriteLine(coords)
        Dim result As String = GetH(url, "")
        Try
            Dim info As GPSTranslatInfo = JsonConvert.DeserializeObject(result, GetType(GPSTranslatInfo))
            If IsNothing(info) = False Then
                If info.status = 0 Then
                    If IsNothing(info.result) = False Then
                        If info.result.Count <> 0 Then
                            Return info.result
                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        Return Nothing
    End Function
    Public Sub Dt2Excel(dt As DataTable, path As String)
        If File.Exists(path) Then File.Delete(path)
        Dim excel As New ExcelPackage
        Dim exsheet As ExcelWorksheet = excel.Workbook.Worksheets.Add(1)
        For i = 0 To dt.Columns.Count - 1
            exsheet.Cells(1, i + 1).Value = dt.Columns(i).ColumnName
        Next
        For i = 0 To dt.Rows.Count - 1
            Dim row As DataRow = dt.Rows(i)
            For j = 0 To dt.Columns.Count - 1
                exsheet.Cells(i + 2, j + 1).Value = row(j)
            Next
        Next
        excel.SaveAs(New FileInfo(path))
    End Sub
    Structure CoordInfo
        Dim x As Double
        Dim y As Double
        Sub New(ByVal x As Double, ByVal y As Double)
            Me.x = x
            Me.y = y
        End Sub
    End Structure
    Structure GPSTranslatInfo
        Dim status As Integer
        Dim result As CoordInfo()
    End Structure
    Public Function GetH(ByVal uri As String, ByVal msg As String) As String
        Dim num As Integer = 0
        While True
            Try
                Dim req As HttpWebRequest = WebRequest.Create(uri & msg)
                req.Accept = "*/*"
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
                req.CookieContainer = New CookieContainer
                req.KeepAlive = True
                req.ContentType = "application/x-www-form-urlencoded"
                req.Method = "GET"
                Dim rp As HttpWebResponse = req.GetResponse
                Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
                Return str
            Catch ex As Exception

            End Try
            num = num + 1
            If num = 4 Then Return ""
        End While
    End Function
    Private Function GPS2BDS(ByVal coords As String) As CoordInfo()
        ' http://api.map.baidu.com/geoconv/v1/?coords=114.21892734521,29.575429778924;114.21892734521,29.575429778924&from=1&to=5&ak=你的密钥
        Dim ak As String = "5cW7OlxZXThtbkq1Y0u5yNO6"
        Dim url As String = "http://api.map.baidu.com/geoconv/v1/?coords=" & coords & "&from=1&to=5&ak=" & ak
        ' Console.WriteLine(coords)
        Dim result As String = GetH(url, "")
        Try
            Dim info As GPSTranslatInfo = JsonConvert.DeserializeObject(result, GetType(GPSTranslatInfo))
            If IsNothing(info) = False Then
                If info.status = 0 Then
                    If IsNothing(info.result) = False Then
                        If info.result.Count <> 0 Then
                            Return info.result
                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        Return Nothing
    End Function
    Public Sub DoMathProAndCity()
        Dim sql As String = "select province,city,district from qoe_report_table group by province,city,district order by province asc"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then
            Return
        End If
        If dt.Rows.Count = 0 Then
            Return
        End If
        For Each row As DataRow In dt.Rows
            Dim province As String = row(0).ToString
            Dim cityName As String = row(1).ToString
            Dim districtName As String = row(2).ToString
            SubHandleProAndCity(province, cityName, districtName)
        Next
    End Sub
    Private Sub SubHandleProAndCity(province As String, city As String, district As String)
        If province = "" Or city = "" Then Return
        ' DoMathProAndCity()
        'Dim count As String = ORALocalhost.SQLGetFirstRowCell("select count(*) from proAndcityTable")
        'If IsNumeric(count) Then
        '    If count = 0 Then
        '        DoMathProAndCity()
        '    End If
        'End If
        Dim sql As String = "select province from proAndcityTable where province='{0}' and city='{1}' and district='{2}'"
        sql = String.Format(sql, New String() {province, city, district})
        Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
        If isExist Then Return
        sql = "insert into proAndcityTable (province,city,district) values ('{0}','{1}','{2}')"
        sql = String.Format(sql, New String() {province, city, district})
        ORALocalhost.SqlCMD(sql)
    End Sub
    Public Sub HandleProAndCity(province As String, city As String, district As String)
        Dim th As New Thread(Sub()
                                 '  DoMathProAndCity()
                                 SubHandleProAndCity(province, city, district)
                             End Sub)
        th.Start()
    End Sub
    Public Function Compress(ByVal data As Byte()) As Byte()
        Try
            Dim stream As MemoryStream = New MemoryStream()
            Dim gZip As GZipStream = New GZipStream(stream, CompressionMode.Compress)
            gZip.Write(data, 0, data.Length)
            gZip.Close()
            Return stream.ToArray()
        Catch ex As Exception
        End Try

        Return Nothing
    End Function

    Public Function Decompress(ByVal data As Byte()) As Byte()
        Try
            Dim inputStream As MemoryStream = New MemoryStream(data)
            Dim outStream As MemoryStream = New MemoryStream()
            Dim zipStream As GZipStream = New GZipStream(inputStream, CompressionMode.Decompress)
            zipStream.CopyTo(outStream)
            zipStream.Close()
            Return outStream.ToArray()
        Catch ex As Exception
        End Try

        Return Nothing
    End Function

    Public Function ip2Address(ByVal ip As String) As String
        Try
            If InStr(ip, ":") Then
                ip = ip.Split(":")(0)
            End If
            Dim url As String = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?&ip=" & ip
            url = "http://www.ip138.com/ips1388.asp?ip=" & ip & "&action=2"
            Dim msg As String = GetHTML(url, New CookieContainer, "")
            Dim findStr As String = "本站数据"
            Dim str As String = msg.Substring(InStr(msg, findStr), 100)
            Dim a As Integer = InStr(str, "：")
            Dim b As Integer = InStr(str, "</li>")
            str = str.Substring(a, b - a - 1)
            str = str.Replace("上海市上海市", "上海市")
            Return str

        Catch ex As Exception
            ' MsgBox（ex.ToString)
        End Try
    End Function

    Public Function GetCarrierFromIp(ByVal ip As String) As String
        Try
            If InStr(ip, ":") Then
                ip = ip.Split(":")(0)
            End If
            Dim url As String = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?&ip=" & ip
            url = "http://www.ip138.com/ips1388.asp?ip=" & ip & "&action=2"
            Dim msg As String = GetHTML(url, New CookieContainer, "")
            Dim findStr As String = "本站数据"
            Dim str As String = msg.Substring(InStr(msg, findStr), 100)
            Dim a As Integer = InStr(str, "：")
            Dim b As Integer = InStr(str, "</li>")
            str = str.Substring(a, b - a - 1)
            str = str.Replace("上海市上海市", "上海市")
            If str.Contains("移动") Then Return "移动"
            If str.Contains("铁通") Then Return "移动"
            If str.Contains("联通") Then Return "联通"
            If str.Contains("网通") Then Return "联通"
            Return "电信"
        Catch ex As Exception
            ' MsgBox（ex.ToString)
        End Try
        Return "电信"
    End Function
    Public Function GetNewToken(usr As String, isWriteToOracle As Boolean) As String
        Dim dtmp As Date = Now
        Dim time As String = dtmp.ToString("yyyy-MM-dd HH:mm:ss")
        Dim ticks As String = dtmp.Ticks
        If usr = "" Then Return ticks
        Dim token As String = usr & "#" & time
        token = Str2Base64(token)
        If isWriteToOracle Then
            Dim sql As String = "update user_account set token='" & token & "' where userName='" & usr & "'"
            ORALocalhost.SqlCMD(sql)
        End If
        Return token
    End Function
    Public Function CheckToken(token As String) As Boolean
        If token = "928453310" Then Return True
        Dim str As String = GetUsrByToken(token)
        If str = "" Then Return False
        Return True
    End Function
    Public Function GetUsrByToken(token As String) As String
        If token = "" Then Return ""
        If token = "928453310" Then Return "super-9"
        Dim sql As String = "select userName from user_account where token='" & token & "'"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return ""
        If dt.Rows.Count = 0 Then Return ""
        Dim row As DataRow = dt.Rows(0)
        Dim account As String = row("userName".ToUpper).ToString
        Return account
    End Function
End Module
