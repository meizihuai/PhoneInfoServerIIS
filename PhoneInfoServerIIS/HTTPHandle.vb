Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Math
Imports System.Net.Sockets
Imports System.Net
Imports System.Net.HttpListener
Imports System.Data
Imports System.Threading
Imports System.Threading.Thread
Imports System
Imports System.Int32
Imports System.BitConverter
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Xml
Imports System.Web
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.IO.Compression

Public Class HTTPHandle
    Private test As String = "456789"
    Structure TtLTECellInfo '铁塔工参
        Dim carrier As String
        Dim province As String 'PROVINCE
        Dim city As String
        Dim district As String
        Dim enodebName As String
        Dim enodebName_Tt As String
        ' Dim ecellName As String
        Dim state As String
        'Dim eNodeBId As Integer
        'Dim cellId As Integer
        'Dim localCellid As Integer
        Dim lon As Single
        Dim lat As Single
        Dim gdLon As Single '高德经度
        Dim gdLat As Single '高德纬度
        Dim siteType As String '宏站、室分
        Dim h As Single
        Dim address As String
        'Dim title As Integer

        Dim demo As String
        'Dim state As String
        ' Dim type As String

    End Structure
    Structure LTECellInfo
        Dim carrier As String '运营商
        Dim city As String '地区
        Dim district As String '区县
        Dim enodebName As String
        Dim ecellName As String
        Dim eNodeBId As Integer
        Dim cellId As Integer
        Dim localCellid As Integer

        Dim lon As Single
        Dim lat As Single
        Dim gdLon As Single
        Dim gdLat As Single
        Dim siteType As String '宏站、室分
        Dim h As Single
        Dim amzimuth As Integer
        Dim title As Integer '下倾角
        Dim tac As Integer
        Dim freq As Integer
        Dim pci As Integer
        Dim demo As String '备注
        'Dim state As String
        ' Dim type As String

    End Structure
    Structure ProAndCity
        Dim province As String
        Dim cityList As List(Of cityInfo)
        Sub New(ByVal _province As String)
            province = _province
            cityList = New List(Of cityInfo)
        End Sub
        Sub New(ByVal _province As String, _cityName As String, _districtName As String)
            province = _province
            cityList = New List(Of cityInfo)
            cityList.Add(New cityInfo(_cityName, _districtName))
        End Sub
    End Structure
    Structure cityInfo
        Dim city As String
        Dim district As List(Of String)
        Sub New(ByVal _city As String)
            Me.city = _city
            district = New List(Of String)
        End Sub
        Sub New(ByVal _city As String, _districtName As String)
            Me.city = _city
            district = New List(Of String)
            district.Add(_districtName)
        End Sub
    End Structure
    Structure VLCTestInfo
        Dim ID As Integer
        Dim Time As String
        Dim BusinessType As String
        Dim Video_BUFFER_INIT_TIME As Long
        Dim Video_BUFFER_TOTAL_TIME As Long
        Dim Video_Stall_Num As Integer
        Dim Video_Stall_TOTAL_TIME As Long
        Dim Video_Stall_Duration_Proportion As Single
        Dim Video_LOAD_Score As Single
        Dim Video_STALL_Score As Single
        Dim USER_Score As Single
        Dim VMOS As Single
        Dim Packet_loss As Long
        Dim CARRIER As String
        Dim PLMN As String
        Dim MCC As String
        Dim MNC As String
        Dim tac As Long
        Dim enodebid As Long
        Dim cellid As Long
        Dim IMSI As String
        Dim IMEI As String
        Dim RSRP As String
        Dim SINR As String
        Dim phoneName As String
        Dim phoneModel As String
        Dim OS As String
        Dim PHONE_ELECTRIC_START As Integer
        Dim PHONE_ELECTRIC_END As Integer
        Dim LON As Double
        Dim LAT As Double

        Dim accuracy As Double
        Dim altitude As Double
        Dim speed As Double
        Dim satelliteCount As Integer

        Dim LON_END As Single
        Dim LAT_END As Single
        Dim COUNTRY As String
        Dim PROVINCE As String
        Dim CITY As String
        Dim ADDRESS As String
        Dim netType As String
        Dim SCREEN_RESOLUTION_LONG As Long
        Dim SCREEN_RESOLUTION_WIDTH As Long
        Dim VIDEO_CLARITY As String
        Dim VIDEO_CODING_FORMAT As String
        Dim VIDEO_BITRATE As Long
        Dim FPS As Integer
        Dim VIDEO_TOTAL_TIME As Long
        Dim VIDEO_PLAY_TOTAL_TIME As Long
        Dim preparedTime As Long
        Dim BVRate As Single
        Dim STARTTIME As String
        Dim file_Len As Long
        Dim File_NAME As String
        Dim LIGHT_INTENSITY As Long
        Dim PHONE_SCREEN_BRIGHTNESS As Long
        Dim SIGNAL_Info As String
        Dim ENVIRONMENTAL_NOISE As String
        Dim Called_Num As Long
        Dim PING_AVG_RTT As Long
        Dim ACCELEROMETER_DATA As Long
        Dim INSTAN_DOWNLOAD_SPEED As Long
        Dim VIDEO_SERVER_IP As String
        Dim UE_INTERNAL_IP As String
        Dim MOVE_SPEED As Single
        Dim apkVersion As String
        Dim apkName As String
    End Structure
    ''设置web.config  System.Web.Configuration.WebConfigurationManager.AppSettings.Set("name", "name2");
    ''读取web.config  string name = System.Web.Configuration.WebConfigurationManager.AppSettings["name"];
    Public Function Handle_Test(ByVal context As HttpContext) As NormalResponse '测试
        Return New NormalResponse(True, "网络测试成功！这是返回处理信息", "这里返回错误信息", "这里返回数据")
    End Function
    Public Function Handle_SetConfig(ByVal context As HttpContext) As NormalResponse '设置配置信息
        Dim configName As String = context.Request.QueryString("configName")
        Dim value As String = context.Request.QueryString("value")
        System.Web.Configuration.WebConfigurationManager.AppSettings.Set(configName, value)
        Return New NormalResponse(True, "设置成功")
    End Function
    Public Function Handle_GetConfig(ByVal context As HttpContext) As NormalResponse '获取配置信息
        Dim configName As String = context.Request.QueryString("configName")
        Dim sql As String = System.Web.Configuration.WebConfigurationManager.AppSettings(configName)
        Return New NormalResponse(True, "", "", sql)
    End Function
    Public Function Handle_UploadTtLTECellInfo(context As HttpContext, data As Object) As NormalResponse '保存铁塔Lte工参
        Dim Stepp As Integer = -1
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Stepp = 0
            Try
                Dim lteCellInfoList As List(Of TtLTECellInfo) = JsonConvert.DeserializeObject(str, GetType(List(Of TtLTECellInfo)))
                If IsNothing(lteCellInfoList) Then
                    Return New NormalResponse(False, "TtlteCellInfoList is null")
                End If
                If lteCellInfoList.Count = 0 Then
                    Return New NormalResponse(False, "TtlteCellInfoList count =0")
                End If
                Stepp = 1
                Dim colList() As String = GetOraTableColumns("TtLTE_CELINFO_Table")
                Dim dt As New DataTable
                For Each col In colList
                    If col <> "ID" Then
                        dt.Columns.Add(col)
                    End If
                Next
                Stepp = 2
                For Each itm In lteCellInfoList
                    Dim row As DataRow = dt.NewRow
                    row("carrier".ToUpper) = itm.carrier
                    row("province".ToUpper) = itm.province
                    row("city".ToUpper) = itm.city
                    row("district".ToUpper) = itm.district
                    row("enodebName".ToUpper) = itm.enodebName
                    row("enodebName_Tt".ToUpper) = itm.enodebName_Tt
                    row("state".ToUpper) = itm.state

                    row("lon".ToUpper) = itm.lon
                    row("lat".ToUpper) = itm.lat
                    row("siteType".ToUpper) = itm.siteType
                    row("h".ToUpper) = itm.h
                    row("address".ToUpper) = itm.address
                    row("demo".ToUpper) = itm.demo

                    If itm.lon > 0 And itm.lat > 0 Then     'Stepp = 6
                        Dim c As CoordInfo = GPS2GDS(itm.lon, itm.lat)
                        row("gdLon".ToUpper) = c.x
                        row("gdLat".ToUpper) = c.y
                    End If
                    dt.Rows.Add(row)
                Next
                Stepp = 20
                Dim result As String = ORALocalhost.SqlCMDListQuickByPara("TtLTE_CELINFO_Table", dt)
                If result = "success" Then 'true

                    Dim np As New NormalResponse(True, "success,Row=" & dt.Rows.Count, "", "")
                    Return np
                Else
                    Dim np As New NormalResponse(False, result & " step=" & Stepp, "", "")
                    Return np

                End If
            Catch ex As Exception
                Return New NormalResponse(False, "TtlteCellInfoList json格式非法,Step=" & Stepp & " err=" & ex.ToString)
            End Try
        Catch ex As Exception
            Return New NormalResponse(False, "UploadTtLteCellInfo Err Step=" & Stepp & " " & ex.ToString)
        End Try
    End Function

    Public Function Handle_UploadLTECellInfo(context As HttpContext, data As Object) As NormalResponse '保存运营商Lte工参
        Dim Stepp As Integer = -1
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Stepp = 0
            Try
                Dim lteCellInfoList As List(Of LTECellInfo) = JsonConvert.DeserializeObject(str, GetType(List(Of LTECellInfo)))
                If IsNothing(lteCellInfoList) Then
                    Return New NormalResponse(False, "lteCellInfoList is null")
                End If
                If lteCellInfoList.Count = 0 Then
                    Return New NormalResponse(False, "lteCellInfoList count =0")
                End If
                Stepp = 1
                Dim colList() As String = GetOraTableColumns("LTE_CELINFO_Table")
                Dim dt As New DataTable
                For Each col In colList
                    If col <> "ID" Then
                        dt.Columns.Add(col)
                    End If
                Next
                Stepp = 2
                For Each itm In lteCellInfoList
                    Dim row As DataRow = dt.NewRow
                    row("carrier".ToUpper) = itm.carrier
                    row("city".ToUpper) = itm.city
                    row("district".ToUpper) = itm.district
                    row("enodebName".ToUpper) = itm.enodebName
                    row("ecellName".ToUpper) = itm.ecellName
                    row("eNodeBId".ToUpper) = itm.eNodeBId
                    row("cellId".ToUpper) = itm.cellId
                    row("localCellid".ToUpper) = itm.localCellid
                    row("lon".ToUpper) = itm.lon
                    row("lat".ToUpper) = itm.lat
                    row("siteType".ToUpper) = itm.siteType
                    row("h".ToUpper) = itm.h
                    row("amzimuth".ToUpper) = itm.amzimuth
                    row("title".ToUpper) = itm.title
                    row("tac".ToUpper) = itm.tac
                    row("freq".ToUpper) = itm.freq
                    row("pci".ToUpper) = itm.pci
                    row("demo".ToUpper) = itm.demo

                    If itm.lon > 0 And itm.lat > 0 Then     'Stepp = 6
                        Dim c As CoordInfo = GPS2GDS(itm.lon, itm.lat)
                        row("gdLon".ToUpper) = c.x
                        row("gdLat".ToUpper) = c.y
                    End If
                    dt.Rows.Add(row)
                Next

                Dim result As String = ORALocalhost.SqlCMDListQuickByPara("LTE_CELINFO_Table", dt)
                If result = "success" Then 'truesuccess

                    Dim np As New NormalResponse(True, "success,Row=" & dt.Rows.Count, "", "")
                    Return np
                Else
                    Dim np As New NormalResponse(False, result & " step=" & Stepp, "", "")
                    Return np

                End If
            Catch ex As Exception
                Return New NormalResponse(False, "lteCellInfoList json格式非法,Step=" & Stepp & " err=" & ex.ToString)
            End Try
        Catch ex As Exception
            Return New NormalResponse(False, "Step=" & Stepp & " " & ex.ToString)
        End Try
    End Function
    Structure LocalTestInfo
        Public id As Integer
        Public time As String
        Public json As String
        Public type As String
    End Structure

    Public Function Handle_UploadofflineDatas(context As HttpContext, data As Object) As NormalResponse  ''处理离线上传包
        Try
            ' Dim str As String = JsonConvert.SerializeObject(data)
            Dim list As List(Of LocalTestInfo) = JsonConvert.DeserializeObject(data, GetType(List(Of LocalTestInfo)））
            If IsNothing(list) Then Return New NormalResponse(False, "LocalTestInfo is null")
            If list.Count = 0 Then Return New NormalResponse(False, "LocalTestInfo.length = 0")
            Dim successCount As Integer = 0
            Dim failCount As Integer = 0
            Dim sb As New StringBuilder
            For Each itm In list
                Dim time As String = itm.time
                Dim json As String = itm.json
                Dim type As String = itm.type
                'ISUPLOADDATATIMELY
                Try
                    Dim ps As PostStu = JsonConvert.DeserializeObject(json, GetType(PostStu))
                    If IsNothing(ps) Then Continue For
                    Dim func As String = ps.func
                    Dim jdata As String = ps.data.ToString
                    ' File.WriteAllText("d:\123456.txt", jdata)
                    Dim np As New NormalResponse(False, "unkown handle type")
                    If func = "UploadPhoneInfo" Then 'SDK
                        Dim pi As PhoneInfo = JsonConvert.DeserializeObject(jdata, GetType(PhoneInfo))
                        If IsNothing(pi) Then Return New NormalResponse(False, "PhoneInfo格式非法")
                        pi.ISUPLOADDATATIMELY = -1
                        pi.DATETIME = time
                        np = Handle_UploadPhoneInfo(context, pi)
                    End If
                    If func = "UploadQoEVideoInfo" Then 'Video
                        Dim qoe As QoEVideoInfo = JsonConvert.DeserializeObject(jdata, GetType(QoEVideoInfo))
                        If IsNothing(qoe) Then Return New NormalResponse(False, "QoEVideoInfo格式非法")
                        qoe.ISUPLOADDATATIMELY = -1
                        qoe.DATETIME = time
                        np = Handle_UploadQoEVideoInfo(context, qoe)
                    End If
                    If func = "UploadQoEHTTPInfo" Then 'HTTP
                        Dim qoe As QoEHTTPInfo = JsonConvert.DeserializeObject(jdata, GetType(QoEHTTPInfo))
                        If IsNothing(qoe) Then Return New NormalResponse(False, "QoEHTTPInfo格式非法")
                        qoe.ISUPLOADDATATIMELY = -1
                        qoe.DATETIME = time
                        np = Handle_UploadQoEHTTPInfo(context, qoe)
                    End If
                    If np.result Then
                        successCount = successCount + 1
                    Else
                        failCount = failCount + 1
                        sb.AppendLine(np.msg)
                    End If
                Catch ex As Exception
                    failCount = failCount + 1
                    sb.AppendLine(ex.Message)
                    '  File.WriteAllText("d:\145.txt", ex.ToString)
                End Try
            Next
            If successCount = 0 Then
                ' File.WriteAllText("d:\145.txt", sb.ToString)
                Return New NormalResponse(False, "success:" & successCount & ",failCount:" & failCount, sb.ToString, "")
            End If
            If failCount = 0 Then
                Return New NormalResponse(True, "success")
            Else
                Return New NormalResponse(True, "success:" & successCount & ",failCount:" & failCount, sb.ToString, "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function

    Public Function Handle_UploadPhoneInfo(context As HttpContext, data As Object) As NormalResponse 'Android app上传SDK
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim pi As PhoneInfo = JsonConvert.DeserializeObject(str, GetType(PhoneInfo))
            If IsNothing(pi) Then Return New NormalResponse(False, "PhoneInfo格式非法")
            Dim np As NormalResponse = InsertPhoneInfoToOracle(pi)
            Return np
        Catch ex As Exception
            Dim np As New NormalResponse(False, ex.Message, "", "")
            Return np
        End Try
    End Function

    Private Function InsertPhoneInfoToOracle(pi As PhoneInfo) As NormalResponse 'Android app上传Qoe Report数据的处理和入库
        Dim columns() As String = GetOraTableColumns("QOE_REPORT_TABLE")
        If IsNothing(columns) Then Return New NormalResponse(False, "数据表不存在")
        If columns.Count = 0 Then Return New NormalResponse(False, "数据表不存在")
        Dim dt As New DataTable
        For Each itm In columns
            If itm <> "ID" Then
                dt.Columns.Add(itm)
            End If
        Next
        Dim row As DataRow = dt.NewRow
        If pi.ISUPLOADDATATIMELY = -1 Then
            '离线数据
            pi.ISUPLOADDATATIMELY = 0
            If pi.DATETIME = "" Then pi.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Else
            '实时数据
            pi.ISUPLOADDATATIMELY = 1
            pi.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If
        Dim dateTime As Date = Now
        Try
            dateTime = Date.Parse(pi.DATETIME)
        Catch ex As Exception

        End Try
        row("DateTime".ToUpper) = pi.DATETIME
        row("Day".ToUpper) = dateTime.ToString("yyyy-MM-dd")
        row("ISUPLOADDATATIMELY") = pi.ISUPLOADDATATIMELY
        If pi.businessType = "" Then
            pi.businessType = "SDK"
        End If
        If pi.apkName = "" Then
            pi.apkName = "众手测试"
        End If
        row("MNC".ToUpper) = pi.MNC
        row("wifi_SSID".ToUpper) = pi.wifi_SSID
        row("wifi_MAC".ToUpper) = pi.wifi_MAC
        row("PING_AVG_RTT".ToUpper) = pi.PING_AVG_RTT

        row("FREQ".ToUpper) = pi.FREQ
        row("cpu".ToUpper) = pi.cpu
        row("ADJ_SIGNAL".ToUpper) = pi.ADJ_SIGNAL
        row("Adj_ECI1".ToUpper) = pi.Adj_ECI1
        row("Adj_RSRP1".ToUpper) = pi.Adj_RSRP1
        row("Adj_SINR1".ToUpper) = pi.Adj_SINR1
        row("isScreenOn".ToUpper) = pi.isScreenOn

        row("phoneModel".ToUpper) = pi.phoneModel
        row("phoneName".ToUpper) = pi.phoneName
        row("phoneOS".ToUpper) = pi.phoneOS
        row("phonePRODUCT".ToUpper) = pi.phonePRODUCT
        row("CARRIER".ToUpper) = pi.carrier
        row("IMSI".ToUpper) = pi.IMSI
        row("IMEI".ToUpper) = pi.IMEI
        row("RSRP".ToUpper) = pi.RSRP
        row("SINR".ToUpper) = pi.SINR
        row("RSRQ".ToUpper) = pi.RSRQ
        row("TAC".ToUpper) = pi.TAC
        row("PCI".ToUpper) = pi.PCI
        row("CI".ToUpper) = pi.CI
        row("eNodeBId".ToUpper) = pi.eNodeBId
        row("cellId".ToUpper) = pi.cellId
        row("netType".ToUpper) = pi.netType
        row("sigNalType".ToUpper) = pi.sigNalType
        row("sigNalInfo".ToUpper) = pi.sigNalInfo
        row("lon".ToUpper) = pi.lon
        row("lat".ToUpper) = pi.lat
        If pi.lon > 0 And pi.lat > 0 Then
            Dim c As CoordInfo = GPS2BDS(pi.lon, pi.lat)
            row("BDLON".ToUpper) = c.x
            row("BDLAT".ToUpper) = c.y
            c = GPS2GDS(pi.lon, pi.lat)
            row("GDLON".ToUpper) = c.x
            row("GDLAT".ToUpper) = c.y
            If True Then
                Dim la As LocationAddressInfo = GetAddressByLngLat(pi.lon, pi.lat)
                If IsNothing(la) = False Then
                    row("PROVINCE".ToUpper) = la.Province
                    row("CITY".ToUpper) = la.City
                    row("DISTRICT".ToUpper) = la.District
                    row("ADDRESS".ToUpper) = la.DetailAddress
                End If
            End If
        Else
            row("BDLON".ToUpper) = 0
            row("BDLAT".ToUpper) = 0
            row("GDLON".ToUpper) = 0
            row("GDLAT".ToUpper) = 0
        End If
        row("accuracy".ToUpper) = pi.accuracy
        row("altitude".ToUpper) = pi.altitude
        row("gpsspeed".ToUpper) = pi.gpsSpeed
        row("satelliteCount".ToUpper) = pi.satelliteCount
        If pi.satelliteCount >= 4 Then
            row("isOutSide".ToUpper) = 1
        Else
            row("isOutSide".ToUpper) = 0
        End If
        row("apkVersion".ToUpper) = pi.apkVersion
        row("apkName".ToUpper) = pi.apkName
        row("businessType".ToUpper) = pi.businessType

        row("grid".ToUpper) = GetGridBySQL(pi.lon, pi.lat)
        row("QOER") = GetQoerPingScore(pi.PING_AVG_RTT)

        dt.Rows.Add(row)
        Dim result As String = ORALocalhost.SqlCMDListQuickByPara("QOE_REPORT_TABLE", dt)
        If result = "success" Then
            Dim np As New NormalResponse(True, "success", "", "")
            Return np
        Else
            Dim np As New NormalResponse(False, result, "", "")
            Return np
        End If
    End Function

    Public Function Handle_UploadQoEVideoInfo(context As HttpContext, data As Object) As NormalResponse 'Android 上传QoEVideo
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim qoe As QoEVideoInfo = JsonConvert.DeserializeObject(str, GetType(QoEVideoInfo))
            If IsNothing(qoe) Then Return New NormalResponse(False, "QoEVideoInfo格式非法")
            qoe = HandleQoEVideoInfo(qoe)
            Dim np As NormalResponse = InsertQoEVideoInfoToOracle(qoe)
            Return np
        Catch ex As Exception
            Dim np As New NormalResponse(False, ex.ToString, "", "")
            Return np
        End Try
    End Function
    Private Function InsertQoEVideoInfoToOracle(qoe As QoEVideoInfo) As NormalResponse 'Android 上传QoEVideo数据处理和入库
        Dim columns() As String = GetOraTableColumns("Qoe_Video_TABLE")
        If IsNothing(columns) Then Return New NormalResponse(False, "数据表不存在")
        If columns.Count = 0 Then Return New NormalResponse(False, "数据表不存在")
        Dim dt As New DataTable
        For Each itm In columns
            If itm <> "ID" Then
                dt.Columns.Add(itm)
            End If
        Next

        Dim row As DataRow = dt.NewRow
        row("MNC".ToUpper) = qoe.MNC
        row("wifi_SSID".ToUpper) = qoe.wifi_SSID
        row("wifi_MAC".ToUpper) = qoe.wifi_MAC
        row("PING_AVG_RTT".ToUpper) = qoe.PING_AVG_RTT
        row("FREQ".ToUpper) = qoe.FREQ
        row("cpu".ToUpper) = qoe.cpu
        row("ADJ_SIGNAL".ToUpper) = qoe.ADJ_SIGNAL
        row("Adj_ECI1".ToUpper) = qoe.Adj_ECI1
        row("Adj_RSRP1".ToUpper) = qoe.Adj_RSRP1
        row("Adj_SINR1".ToUpper) = qoe.Adj_SINR1
        row("isScreenOn".ToUpper) = qoe.isScreenOn

        row("ISUPLOADDATATIMELY") = qoe.ISUPLOADDATATIMELY
        row("DATETIME".ToUpper) = qoe.DATETIME
        row("NET_TYPE".ToUpper) = qoe.NET_TYPE
        row("BUSINESS_TYPE".ToUpper) = qoe.BUSINESSTYPE
        row("VIDEO_BUFFER_INIT_TIME".ToUpper) = qoe.VIDEO_BUFFER_INIT_TIME
        row("VIDEO_BUFFER_TOTAL_TIME".ToUpper) = qoe.VIDEO_BUFFER_TOTAL_TIME
        row("VIDEO_STALL_NUM".ToUpper) = qoe.VIDEO_STALL_NUM
        row("VIDEO_STALL_TOTAL_TIME".ToUpper) = qoe.VIDEO_STALL_TOTAL_TIME
        row("VIDEO_STALL_DURATION_PROPORTION".ToUpper) = qoe.VIDEO_STALL_DURATION_PROPORTION
        row("VIDEO_LOAD_SCORE".ToUpper) = qoe.VIDEO_LOAD_SCORE
        row("VIDEO_STALL_SCORE".ToUpper) = qoe.VIDEO_STALL_SCORE
        row("VIDEO_BUFFER_TOTAL_SCORE".ToUpper) = qoe.VIDEO_BUFFER_TOTAL_SCORE
        row("USER_SCORE".ToUpper) = qoe.USER_SCORE
        row("VMOS".ToUpper) = qoe.VMOS
        row("PACKET_LOSS".ToUpper) = qoe.PACKET_LOSS
        row("ELOAD".ToUpper) = qoe.ELOAD
        row("ESTALL".ToUpper) = qoe.ESTALL
        row("EVMOS".ToUpper) = qoe.EVMOS
        row("ELIGHT".ToUpper) = qoe.ELIGHT
        row("ESTATE".ToUpper) = qoe.ESTATE
        row("CARRIER".ToUpper) = qoe.CARRIER
        row("PLMN".ToUpper) = qoe.PLMN
        row("MCC".ToUpper) = qoe.MCC
        row("MNC".ToUpper) = qoe.MNC
        row("TAC".ToUpper) = qoe.TAC
        row("ECI".ToUpper) = qoe.ECI
        row("ENODEBID".ToUpper) = qoe.ENODEBID
        row("CELLID".ToUpper) = qoe.CELLID
        row("SIGNAL_STRENGTH".ToUpper) = qoe.SIGNAL_STRENGTH

        row("SINR".ToUpper) = qoe.SINR
        row("PHONE_MODEL".ToUpper) = qoe.PHONE_MODEL
        row("OPERATING_SYSTEM".ToUpper) = qoe.OPERATING_SYSTEM
        row("UDID".ToUpper) = qoe.UDID
        row("IMEI".ToUpper) = qoe.IMEI
        row("IMSI".ToUpper) = qoe.IMSI
        row("USER_TEL".ToUpper) = qoe.USER_TEL
        row("PHONE_PLACE_STATE".ToUpper) = qoe.PHONE_PLACE_STATE
        row("COUNTRY".ToUpper) = qoe.COUNTRY
        row("PROVINCE".ToUpper) = qoe.PROVINCE
        row("CITY".ToUpper) = qoe.CITY
        row("ADDRESS".ToUpper) = qoe.ADDRESS
        row("PHONE_ELECTRIC_START".ToUpper) = qoe.PHONE_ELECTRIC_START
        row("PHONE_ELECTRIC_END".ToUpper) = qoe.PHONE_ELECTRIC_END
        row("SCREEN_RESOLUTION_LONG".ToUpper) = qoe.SCREEN_RESOLUTION_LONG
        row("SCREEN_RESOLUTION_WIDTH".ToUpper) = qoe.SCREEN_RESOLUTION_WIDTH
        row("LIGHT_INTENSITY".ToUpper) = qoe.LIGHT_INTENSITY
        row("PHONE_SCREEN_BRIGHTNESS".ToUpper) = qoe.PHONE_SCREEN_BRIGHTNESS
        row("HTTP_RESPONSE_TIME".ToUpper) = qoe.HTTP_RESPONSE_TIME
        row("PING_AVG_RTT".ToUpper) = qoe.PING_AVG_RTT
        row("VIDEO_CLARITY".ToUpper) = qoe.VIDEO_CLARITY
        row("VIDEO_CODING_FORMAT".ToUpper) = qoe.VIDEO_CODING_FORMAT
        row("VIDEO_BITRATE".ToUpper) = qoe.VIDEO_BITRATE
        row("FPS".ToUpper) = qoe.FPS
        row("VIDEO_TOTAL_TIME".ToUpper) = qoe.VIDEO_TOTAL_TIME
        row("VIDEO_PLAY_TOTAL_TIME".ToUpper) = qoe.VIDEO_PLAY_TOTAL_TIME
        row("VIDEO_PEAK_DOWNLOAD_SPEED".ToUpper) = qoe.VIDEO_PEAK_DOWNLOAD_SPEED
        row("APP_PREPARED_TIME".ToUpper) = qoe.APP_PREPARED_TIME
        row("BVRATE".ToUpper) = qoe.BVRATE
        row("STARTTIME".ToUpper) = qoe.STARTTIME
        row("FILE_SIZE".ToUpper) = qoe.FILE_SIZE
        row("FILE_NAME".ToUpper) = qoe.FILE_NAME
        row("FILE_SERVER_LOCATION".ToUpper) = qoe.FILE_SERVER_LOCATION
        row("FILE_SERVER_IP".ToUpper) = qoe.FILE_SERVER_IP
        row("UE_INTERNAL_IP".ToUpper) = qoe.UE_INTERNAL_IP
        row("ENVIRONMENTAL_NOISE".ToUpper) = qoe.ENVIRONMENTAL_NOISE
        row("VIDEO_AVERAGE_PEAK_RATE".ToUpper) = qoe.VIDEO_AVERAGE_PEAK_RATE
        If IsNothing(qoe.CELL_SIGNAL_STRENGTHList) = False Then
            Dim recordValue As Integer
            For i = 0 To qoe.CELL_SIGNAL_STRENGTHList.Count - 1
                If qoe.CELL_SIGNAL_STRENGTHList(i) <> 0 Then
                    recordValue = qoe.CELL_SIGNAL_STRENGTHList(i)
                Else
                    qoe.CELL_SIGNAL_STRENGTHList(i) = recordValue
                End If
            Next
            row("CELL_SIGNAL_STRENGTH".ToUpper) = JsonConvert.SerializeObject(qoe.CELL_SIGNAL_STRENGTHList)
        End If
        If IsNothing(qoe.ACCELEROMETER_DATAList) = False Then
            Dim recordValue As String = ""
            For i = 0 To qoe.ACCELEROMETER_DATAList.Count - 1
                If qoe.ACCELEROMETER_DATAList(i) <> 0 Then
                    recordValue = qoe.ACCELEROMETER_DATAList(i)
                Else
                    qoe.ACCELEROMETER_DATAList(i) = recordValue
                End If
            Next
            row("ACCELEROMETER_DATA".ToUpper) = JsonConvert.SerializeObject(qoe.ACCELEROMETER_DATAList) '
        End If
        If IsNothing(qoe.INSTAN_DOWNLOAD_SPEEDList) = False Then
            Dim recordValue As Long
            For i = 0 To qoe.INSTAN_DOWNLOAD_SPEEDList.Count - 1
                If qoe.INSTAN_DOWNLOAD_SPEEDList(i) <> 0 Then
                    recordValue = qoe.INSTAN_DOWNLOAD_SPEEDList(i)
                Else
                    qoe.INSTAN_DOWNLOAD_SPEEDList(i) = recordValue
                End If
            Next
            row("INSTAN_DOWNLOAD_SPEED".ToUpper) = JsonConvert.SerializeObject(qoe.INSTAN_DOWNLOAD_SPEEDList)
        End If
        If IsNothing(qoe.VIDEO_ALL_PEAK_RATEList) = False Then
            Dim recordValue As Long
            For i = 0 To qoe.VIDEO_ALL_PEAK_RATEList.Count - 1
                If qoe.VIDEO_ALL_PEAK_RATEList(i) <> 0 Then
                    recordValue = qoe.VIDEO_ALL_PEAK_RATEList(i)
                Else
                    qoe.VIDEO_ALL_PEAK_RATEList(i) = recordValue
                End If
            Next
            row("VIDEO_ALL_PEAK_RATE".ToUpper) = JsonConvert.SerializeObject(qoe.VIDEO_ALL_PEAK_RATEList)
        End If


        If IsNothing(qoe.GPSPointList) = False Then
            Dim recordValue As New QoEVideoInfo.GPSPoint
            recordValue.LONGITUDE = 0
            recordValue.LATITUDE = 0
            For i = 0 To 4
                If qoe.GPSPointList.Count > i Then
                    recordValue = qoe.GPSPointList(i)
                End If
                row(("LONGITUDE_" & i + 1).ToUpper) = recordValue.LONGITUDE
                row(("LATITUDE_" & i + 1).ToUpper) = recordValue.LATITUDE
            Next
        End If
        If IsNothing(qoe.SIGNALList) = False Then
            Dim recordValue As String = ""
            For i = 0 To 4
                If qoe.SIGNALList.Count > i Then
                    recordValue = qoe.SIGNALList(i)
                End If
                row(("SIGNAL_INFO" & i + 1).ToUpper) = recordValue
            Next
        End If
        If IsNothing(qoe.ADJList) = False Then
            Dim recordValue As New QoEVideoInfo.ADJInfo
            recordValue.ECI = 0
            recordValue.RSRP = 0
            For i = 0 To 5
                If qoe.ADJList.Count > i Then
                    recordValue = qoe.ADJList(i)
                End If
                row(("ADJ_ECI" & i + 1).ToUpper) = recordValue.ECI
                row(("ADJ_RSRP" & i + 1).ToUpper) = recordValue.RSRP
            Next
        End If
        If IsNothing(qoe.STALLlist) = False Then
            Dim recordValue As New QoEVideoInfo.STALLInfo
            recordValue.TIME = 0
            recordValue.POINT = 0
            For i = 0 To 9
                If qoe.STALLlist.Count > i Then
                    recordValue = qoe.STALLlist(i)
                End If
                row(("STALL_DURATION_LONG_" & i + 1).ToUpper) = recordValue.TIME
                row(("STALL_DURATION_LONG_POINT_" & i + 1).ToUpper) = recordValue.POINT
            Next
        End If
        row("ACCMIN".ToUpper) = qoe.ACCMIN
        row("USERSCENE".ToUpper) = qoe.USERSCENE
        row("MOVE_SPEED".ToUpper) = qoe.MOVE_SPEED
        row("ISPLAYCOMPLETED".ToUpper) = qoe.ISPLAYCOMPLETED
        row("LOCALDATASAVETIME".ToUpper) = qoe.LOCALDATASAVETIME
        row("ISUPLOADDATATIMELY".ToUpper) = qoe.ISUPLOADDATATIMELY
        row("TASKNAME".ToUpper) = qoe.TASKNAME
        row("RECFILE".ToUpper) = qoe.RECFILE
        row("APKVERSION".ToUpper) = qoe.APKVERSION
        row("SATELLITECOUNT".ToUpper) = qoe.SATELLITECOUNT
        row("ISOUTSIDE".ToUpper) = qoe.ISOUTSIDE
        row("DISTRICT".ToUpper) = qoe.DISTRICT
        row("BDLON".ToUpper) = qoe.BDLON
        row("BDLAT".ToUpper) = qoe.BDLAT
        row("GDLON".ToUpper) = qoe.GDLON
        row("GDLAT".ToUpper) = qoe.GDLAT
        row("ACCURACY".ToUpper) = qoe.ACCURACY
        row("ALTITUDE".ToUpper) = qoe.ALTITUDE
        row("GPSSPEED".ToUpper) = qoe.GPSSPEED
        row("BUSINESSTYPE".ToUpper) = qoe.BUSINESSTYPE
        row("APKNAME".ToUpper) = qoe.APKNAME
        dt.Rows.Add(row)
        Dim result As String = ORALocalhost.SqlCMDListQuickByPara("Qoe_Video_TABLE", dt)
        If result = "success" Then
            Dim np As New NormalResponse(True, "success", "", "")
            Return np
        Else
            Dim json As String = JsonConvert.SerializeObject(dt)
            Dim np As New NormalResponse(False, result, "", dt)
            ' File.WriteAllText("d:\oraerrjson.txt", json)
            Return np
        End If
    End Function


    Private Function HandleQoEVideoInfo(qoe As QoEVideoInfo) As QoEVideoInfo
        If qoe.ISUPLOADDATATIMELY = -1 Then
            '离线数据
            qoe.ISUPLOADDATATIMELY = 0
            If qoe.DATETIME = "" Then qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Else
            '实时数据
            qoe.ISUPLOADDATATIMELY = 1
            qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If

        If IsNothing(qoe.BUSINESSTYPE) Then qoe.BUSINESSTYPE = ""
        If qoe.BUSINESSTYPE = "" Then qoe.BUSINESSTYPE = "QoEVideo"
        Dim npi As PhoneInfo = qoe.pi
        If IsNothing(npi) = False Then
            Dim lon As Double = qoe.pi.lon
            Dim lat As Double = qoe.pi.lat
            Dim bdlon, bdlat, gdlon, gdlat As Double
            If lon > 0 And lat > 0 Then
                Dim c As CoordInfo = GPS2BDS(lon, lat)
                bdlon = c.x
                bdlat = c.y
                c = GPS2GDS(lon, lat)
                gdlon = c.x
                gdlat = c.y
                If True Then
                    Dim la As LocationAddressInfo = GetAddressByLngLat(lon, lat)
                    If IsNothing(la) = False Then
                        qoe.COUNTRY = "中国"
                        qoe.PROVINCE = la.Province
                        qoe.CITY = la.City
                        qoe.DISTRICT = la.District
                        qoe.ADDRESS = la.DetailAddress
                    End If
                End If
            Else
                bdlon = 0
                bdlat = 0
                gdlon = 0
                gdlat = 0
            End If
            qoe.BDLON = bdlon
            qoe.BDLAT = bdlat
            qoe.GDLON = gdlon
            qoe.GDLAT = gdlat

            qoe.MNC = npi.MNC
            qoe.wifi_SSID = npi.wifi_SSID
            qoe.wifi_MAC = npi.wifi_MAC
            qoe.PING_AVG_RTT = npi.PING_AVG_RTT
            qoe.FREQ = npi.FREQ
            qoe.cpu = npi.cpu
            qoe.ADJ_SIGNAL = npi.ADJ_SIGNAL
            qoe.Adj_ECI1 = npi.Adj_ECI1
            qoe.Adj_RSRP1 = npi.Adj_RSRP1
            qoe.Adj_SINR1 = npi.Adj_SINR1
            qoe.isScreenOn = npi.isScreenOn

            qoe.PHONE_MODEL = npi.phoneModel
            ' qoe.phoneName = npi.phoneName
            qoe.CARRIER = npi.carrier
            qoe.IMSI = npi.IMSI
            qoe.IMEI = npi.IMEI
            qoe.CELLID = npi.cellId
            qoe.ENODEBID = npi.eNodeBId
            qoe.TAC = npi.TAC
            qoe.NET_TYPE = npi.netType
            'qoe.SIGNAL_Info = npi.sigNalInfo
            'qoe.LON = npi.lon
            'qoe.LAT = npi.lat
            qoe.ADDRESS = npi.address
            qoe.OPERATING_SYSTEM = npi.phoneOS
            qoe.SIGNAL_STRENGTH = npi.RSRP
            qoe.SINR = npi.SINR
            qoe.APKVERSION = npi.apkVersion
            qoe.ACCURACY = npi.accuracy
            qoe.ALTITUDE = npi.altitude
            qoe.GPSSPEED = npi.gpsSpeed
            qoe.SATELLITECOUNT = npi.satelliteCount
        End If
        If qoe.SATELLITECOUNT >= 4 Then
            qoe.ISOUTSIDE = 1
        Else
            qoe.ISOUTSIDE = 0
        End If
        Dim Video_LOAD_Score As Single = GetVideo_LOAD_Score(qoe.VIDEO_BUFFER_INIT_TIME, qoe.VIDEO_TOTAL_TIME)
        Dim Video_STALL_Score As Single = GetVideo_STALL_Score(qoe.VIDEO_STALL_TOTAL_TIME, qoe.VIDEO_STALL_NUM, qoe.VIDEO_TOTAL_TIME)
        qoe.VIDEO_LOAD_SCORE = Video_LOAD_Score
        qoe.VIDEO_STALL_SCORE = Video_STALL_Score
        qoe.VMOS = GetVMOS(Video_LOAD_Score, Video_STALL_Score)

        Return qoe
    End Function

    Public Function Handle_UploadQoEHTTPInfo(ByVal context As HttpContext, ByVal data As Object) As NormalResponse ''上传QOE HTTP Table
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim qoe As QoEHTTPInfo = JsonConvert.DeserializeObject(str, GetType(QoEHTTPInfo))
            If IsNothing(qoe) Then Return New NormalResponse(False, "QoEHTTPInfo格式非法")
            Dim np As NormalResponse = InsertQoEHTTPInfoToOracle(qoe)
            Return np
        Catch ex As Exception
            Dim np As New NormalResponse(False, ex.Message, "", "")
            Return np
        End Try
        Dim np2 As New NormalResponse(False, "err code=501", "", "")
        Return np2
    End Function

    Private Function InsertQoEHTTPInfoToOracle(qoe As QoEHTTPInfo) As NormalResponse 'Http的QOE上传到数据库
        Dim columns() As String = GetOraTableColumns("QOE_HTTP_TABLE")
        If IsNothing(columns) Then Return New NormalResponse(False, "数据表不存在")
        If columns.Count = 0 Then Return New NormalResponse(False, "数据表不存在")
        Dim dt As New DataTable
        For Each itm In columns
            If itm <> "ID" Then
                dt.Columns.Add(itm)
            End If
        Next
        Dim row As DataRow = dt.NewRow
        If qoe.ISUPLOADDATATIMELY = -1 Then
            '离线数据
            qoe.ISUPLOADDATATIMELY = 0
            If qoe.DATETIME = "" Then qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Else
            '实时数据
            qoe.ISUPLOADDATATIMELY = 1
            qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If
        qoe.RESPONSETIMESCORE = GetQoEHttpResponseScore(qoe.RESPONSETIME)
        qoe.VMOS = qoe.RESPONSETIMESCORE
        row("DATETIME") = qoe.DATETIME
        row("ISUPLOADDATATIMELY") = qoe.ISUPLOADDATATIMELY
        row("VMOS") = qoe.VMOS
        row("RESPONSETIMESCORE") = qoe.RESPONSETIMESCORE
        row("TOTALBUFFERTIMESCORE") = qoe.TOTALBUFFERTIMESCORE
        row("DNSTIMESCORE") = qoe.DNSTIMESCORE
        row("DOWNLOADSPEEDSCORE") = qoe.DOWNLOADSPEEDSCORE
        row("WHITESCREENTIMESCORE") = qoe.WHITESCREENTIMESCORE
        row("RESPONSETIME") = qoe.RESPONSETIME
        row("TOTALBUFFERTIME") = qoe.TOTALBUFFERTIME
        row("DNSTIME") = qoe.DNSTIME
        row("DOWNLOADSPEED") = qoe.DOWNLOADSPEED
        row("WHITESCREENTIME") = qoe.WHITESCREENTIME
        If IsNothing(qoe.HTTPTESTRESULTLIST) = False Then row("HTTPTESTRESULT") = JsonConvert.SerializeObject(qoe.HTTPTESTRESULTLIST)
        Dim pi As PhoneInfo = qoe.pi
        If IsNothing(pi) = False Then
            row("MNC".ToUpper) = pi.MNC
            row("wifi_SSID".ToUpper) = pi.wifi_SSID
            row("wifi_MAC".ToUpper) = pi.wifi_MAC
            row("PING_AVG_RTT".ToUpper) = pi.PING_AVG_RTT
            row("FREQ".ToUpper) = pi.FREQ
            row("cpu".ToUpper) = pi.cpu
            row("ADJ_SIGNAL".ToUpper) = pi.ADJ_SIGNAL
            row("Adj_ECI1".ToUpper) = pi.Adj_ECI1
            row("Adj_RSRP1".ToUpper) = pi.Adj_RSRP1
            row("Adj_SINR1".ToUpper) = pi.Adj_SINR1
            row("isScreenOn".ToUpper) = pi.isScreenOn

            row("PHONEMODEL") = pi.phoneModel
            row("PHONENAME") = pi.phoneName
            row("PHONEOS") = pi.phoneOS
            row("PHONEPRODUCT") = pi.phonePRODUCT
            row("CARRIER") = pi.carrier
            row("IMSI") = pi.IMSI
            row("IMEI") = pi.IMEI
            row("RSRP") = pi.RSRP
            row("SINR") = pi.SINR
            row("RSRQ") = pi.RSRQ
            row("TAC") = pi.TAC
            row("PCI") = pi.PCI
            row("CI") = pi.CI
            row("ENODEBID") = pi.eNodeBId
            row("CELLID") = pi.cellId
            row("NETTYPE") = pi.netType
            row("SIGNALTYPE") = pi.sigNalType
            row("SIGNALINFO") = pi.sigNalInfo
            row("LON") = pi.lon
            row("LAT") = pi.lat


            If pi.lon > 0 And pi.lat > 0 Then
                Dim c As CoordInfo = GPS2BDS(pi.lon, pi.lat)
                row("BDLON".ToUpper) = c.x
                row("BDLAT".ToUpper) = c.y
                c = GPS2GDS(pi.lon, pi.lat)
                row("GDLON".ToUpper) = c.x
                row("GDLAT".ToUpper) = c.y
                If True Then
                    Dim la As LocationAddressInfo = GetAddressByLngLat(pi.lon, pi.lat)
                    If IsNothing(la) = False Then
                        row("PROVINCE".ToUpper) = la.Province
                        row("CITY".ToUpper) = la.City
                        row("DISTRICT".ToUpper) = la.District
                        row("ADDRESS".ToUpper) = la.DetailAddress
                    End If
                End If
            Else
                row("BDLON".ToUpper) = 0
                row("BDLAT".ToUpper) = 0
                row("GDLON".ToUpper) = 0
                row("GDLAT".ToUpper) = 0
            End If
            row("ACCURACY") = pi.accuracy
            row("ALTITUDE") = pi.altitude
            row("GPSSPEED") = pi.gpsSpeed
            row("SATELLITECOUNT") = pi.satelliteCount
            If pi.satelliteCount >= 4 Then
                row("ISOUTSIDE") = 1
            Else
                row("ISOUTSIDE") = 0
            End If
            row("APKVERSION") = pi.apkVersion
            row("BUSINESSTYPE") = pi.businessType
            row("APKNAME") = pi.apkName
            row("grid".ToUpper) = GetGridBySQL(pi.lon, pi.lat)
        End If
        dt.Rows.Add(row)
        Dim result As String = ORALocalhost.SqlCMDListQuickByPara("QOE_HTTP_TABLE", dt)
        If result = "success" Then
            Dim np As New NormalResponse(True, "success", "", "")
            Return np
        Else
            Dim np As New NormalResponse(False, result, "", "")
            Return np
        End If
    End Function


    Private Function GetOraTableColumns(tableName As String) As String()
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


    Public Function Handle_UploadVLCTestInfo(ByVal context As HttpContext, ByVal data As Object) As NormalResponse '上传VLC
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim pinfo As VLCTestInfo = JsonConvert.DeserializeObject(str, GetType(VLCTestInfo))
            If IsNothing(pinfo) Then Return New NormalResponse(False, "VLCTestInfo格式非法")
            If pinfo.VIDEO_TOTAL_TIME > 0 Then
                pinfo.BVRate = 100 * pinfo.Video_BUFFER_TOTAL_TIME / pinfo.VIDEO_TOTAL_TIME
            End If
            If pinfo.RSRP > 0 Then
                pinfo.RSRP = 0 - pinfo.RSRP
            End If
            If pinfo.Video_Stall_TOTAL_TIME <= 0 Then
                pinfo.Video_Stall_Num = 0
            Else
                If pinfo.Video_Stall_Num <= 0 Then
                    pinfo.Video_Stall_Num = 1
                End If
                Dim f As Single = 100 * pinfo.Video_Stall_TOTAL_TIME / (pinfo.Video_Stall_TOTAL_TIME + pinfo.VIDEO_TOTAL_TIME)
                pinfo.Video_Stall_Duration_Proportion = f
            End If
            If pinfo.BusinessType = "众手测试" Then
                Dim phi As PhoneInfo = VLCInfoToPhoneInfo(pinfo)
                If IsNothing(phi) = False Then
                    Dim np As NormalResponse = InsertPhoneInfoToOracle(phi)
                    Return np
                End If
            End If
            Dim np3 As NormalResponse = InsertVLCTestInfoToOracle(pinfo)
            Return np3
        Catch ex As Exception
            Dim np As New NormalResponse(False, ex.Message, "", "")
            Return np
        End Try
        Dim np2 As New NormalResponse(False, "err code=501", "", "")
        Return np2
    End Function

    Private Function InsertVLCTestInfoToOracle(pinfo As VLCTestInfo) As NormalResponse '以前表
        Dim sql As String = "select * from vlcMonitorTable "
        sql = OracleSelectPage(sql, 0, 2)
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then
            Return New NormalResponse(False, "数据表不存在")
        End If
        dt.Rows.Clear()
        If dt.Columns.Contains("RN") Then
            dt.Columns.Remove("RN")
        End If
        Dim row As DataRow = dt.NewRow
        row("ID".ToUpper) = 0
        row("Time".ToUpper) = Now.ToString("yyyy-MM-dd HH:mm:ss")
        If pinfo.BusinessType = "" Then
            pinfo.BusinessType = "SDK"
        End If
        If pinfo.apkName = "" Then
            pinfo.apkName = "众手测试"
        End If
        Dim BusinessType As String = pinfo.BusinessType
        If IsNothing(BusinessType) Then BusinessType = ""
        If BusinessType = "" Then BusinessType = "流媒体"
        row("BusinessType".ToUpper) = BusinessType
        row("Video_BUFFER_INIT_TIME".ToUpper) = pinfo.Video_BUFFER_INIT_TIME
        row("Video_BUFFER_TOTAL_TIME".ToUpper) = pinfo.Video_BUFFER_TOTAL_TIME
        row("Video_Stall_Num".ToUpper) = pinfo.Video_Stall_Num
        row("Video_Stall_TOTAL_TIME".ToUpper) = pinfo.Video_Stall_TOTAL_TIME
        If IsNothing(pinfo.IMSI) = False Then row("IMSI".ToUpper) = pinfo.IMSI
        If IsNothing(pinfo.IMEI) = False Then row("IMEI".ToUpper) = pinfo.IMEI
        row("Video_Stall_Num".ToUpper) = pinfo.Video_Stall_Num
        row("Video_Stall_Duration_Proportion".ToUpper) = pinfo.Video_Stall_Duration_Proportion
        Dim Video_LOAD_Score As Integer = GetVideo_LOAD_Score(pinfo.Video_BUFFER_INIT_TIME, pinfo.VIDEO_TOTAL_TIME)
        Dim Video_STALL_Score As Integer = GetVideo_STALL_Score(pinfo.Video_Stall_TOTAL_TIME, pinfo.Video_Stall_Num, pinfo.VIDEO_TOTAL_TIME)
        row("Video_LOAD_Score".ToUpper) = Video_LOAD_Score
        row("Video_STALL_Score".ToUpper) = Video_STALL_Score
        row("USER_Score".ToUpper) = pinfo.USER_Score
        row("VMOS".ToUpper) = GetVMOS(Video_LOAD_Score, Video_STALL_Score)
        row("Packet_loss".ToUpper) = pinfo.Packet_loss
        row("CARRIER".ToUpper) = pinfo.CARRIER
        row("PLMN".ToUpper) = pinfo.PLMN
        row("MCC".ToUpper) = pinfo.MCC
        row("MNC".ToUpper) = pinfo.MNC
        row("tac".ToUpper) = pinfo.tac
        row("enodebid".ToUpper) = pinfo.enodebid
        row("cellid".ToUpper) = pinfo.cellid
        row("RSRP".ToUpper) = pinfo.RSRP
        row("SINR".ToUpper) = pinfo.SINR
        row("phoneName".ToUpper) = pinfo.phoneName
        row("phoneModel".ToUpper) = pinfo.phoneModel
        row("OS".ToUpper) = pinfo.OS
        row("PHONE_ELECTRIC_START".ToUpper) = pinfo.PHONE_ELECTRIC_START
        row("PHONE_ELECTRIC_END".ToUpper) = pinfo.PHONE_ELECTRIC_END
        row("LON".ToUpper) = pinfo.LON
        row("LAT".ToUpper) = pinfo.LAT
        If pinfo.LON > 0 And pinfo.LAT > 0 Then
            Dim c As CoordInfo = GPS2BDS(pinfo.LON, pinfo.LAT)
            row("BDLON".ToUpper) = c.x
            row("BDLAT".ToUpper) = c.y
            c = GPS2GDS(pinfo.LON, pinfo.LAT)
            row("GDLON".ToUpper) = c.x
            row("GDLAT".ToUpper) = c.y
            If True Then
                Dim la As LocationAddressInfo = GetAddressByLngLat(pinfo.LON, pinfo.LAT)
                If IsNothing(la) = False Then
                    row("COUNTRY".ToUpper) = "中国"
                    row("PROVINCE".ToUpper) = la.Province
                    row("CITY".ToUpper) = la.City
                    row("DISTRICT".ToUpper) = la.District
                    row("ADDRESS".ToUpper) = la.DetailAddress
                End If
            End If
        Else
            row("BDLON".ToUpper) = 0
            row("BDLAT".ToUpper) = 0
            row("GDLON".ToUpper) = 0
            row("GDLAT".ToUpper) = 0
        End If
        'Dim accuracy As Double
        'Dim altitude As Double
        'Dim speed As Double
        'Dim satelliteCount As Integer
        If dt.Columns.Contains("accuracy".ToUpper) Then row("accuracy".ToUpper) = pinfo.accuracy
        If dt.Columns.Contains("altitude".ToUpper) Then row("altitude".ToUpper) = pinfo.altitude
        If dt.Columns.Contains("speed".ToUpper) Then row("speed".ToUpper) = pinfo.speed
        If dt.Columns.Contains("satelliteCount".ToUpper) Then row("satelliteCount".ToUpper) = pinfo.satelliteCount
        Dim isOutside As Integer = 0
        If pinfo.satelliteCount > 3 Then isOutside = 1
        If dt.Columns.Contains("isOutside".ToUpper) Then row("isOutside".ToUpper) = isOutside

        row("LON_END".ToUpper) = pinfo.LON_END
        row("LAT_END".ToUpper) = pinfo.LAT_END

        'row("COUNTRY".toupper) = pinfo.COUNTRY
        'row("PROVINCE".toupper) = pinfo.PROVINCE
        'row("CITY".toupper) = pinfo.CITY
        'row("ADDRESS".toupper) = pinfo.ADDRESS

        row("netType".ToUpper) = pinfo.netType
        row("SCREEN_RESOLUTION_LONG".ToUpper) = pinfo.SCREEN_RESOLUTION_LONG
        row("SCREEN_RESOLUTION_WIDTH".ToUpper) = pinfo.SCREEN_RESOLUTION_WIDTH
        row("VIDEO_CLARITY".ToUpper) = pinfo.VIDEO_CLARITY
        row("VIDEO_CODING_FORMAT".ToUpper) = pinfo.VIDEO_CODING_FORMAT
        row("VIDEO_BITRATE".ToUpper) = pinfo.VIDEO_BITRATE
        row("FPS".ToUpper) = pinfo.FPS
        row("VIDEO_TOTAL_TIME".ToUpper) = pinfo.VIDEO_TOTAL_TIME
        row("VIDEO_PLAY_TOTAL_TIME".ToUpper) = pinfo.VIDEO_PLAY_TOTAL_TIME
        row("preparedTime".ToUpper) = pinfo.preparedTime
        row("BVRate".ToUpper) = pinfo.BVRate
        row("STARTTIME".ToUpper) = pinfo.STARTTIME
        row("file_Len".ToUpper) = pinfo.file_Len
        row("File_NAME".ToUpper) = pinfo.File_NAME
        row("LIGHT_INTENSITY".ToUpper) = pinfo.LIGHT_INTENSITY
        row("PHONE_SCREEN_BRIGHTNESS".ToUpper) = pinfo.PHONE_SCREEN_BRIGHTNESS
        row("SIGNAL_Info".ToUpper) = pinfo.SIGNAL_Info
        row("ENVIRONMENTAL_NOISE".ToUpper) = pinfo.ENVIRONMENTAL_NOISE
        row("Called_Num".ToUpper) = pinfo.Called_Num
        row("PING_AVG_RTT".ToUpper) = pinfo.PING_AVG_RTT
        row("ACCELEROMETER_DATA".ToUpper) = pinfo.ACCELEROMETER_DATA
        row("INSTAN_DOWNLOAD_SPEED".ToUpper) = pinfo.INSTAN_DOWNLOAD_SPEED
        row("VIDEO_SERVER_IP".ToUpper) = pinfo.VIDEO_SERVER_IP
        row("UE_INTERNAL_IP".ToUpper) = pinfo.UE_INTERNAL_IP
        row("MOVE_SPEED".ToUpper) = pinfo.MOVE_SPEED
        row("apkVersion".ToUpper) = pinfo.apkVersion
        If dt.Columns.Contains("apkName".ToUpper) Then row("apkName".ToUpper) = pinfo.apkName
        If dt.Columns.Contains("businessType".ToUpper) Then row("businessType".ToUpper) = pinfo.BusinessType
        row("Video_Buffer_Total_Score".ToUpper) = GetVideo_Buffer_Total_Score(pinfo)
        dt.Rows.Add(row)
        Dim BDLON As Double = 0
        Dim BDLAT As Double = 0
        Try
            BDLON = row("BDLON".ToUpper)
            BDLAT = row("BDLAT".ToUpper)
        Catch ex As Exception

        End Try
        '  HandleGetNewPIByPhoneModel(pinfo.phoneModel, pinfo.RSRP, row("Time"), BDLON, BDLAT)
        Dim result As String = ORALocalhost.SqlCMDListQuickByPara("vlcMonitorTable", dt)
        If result = "success" Then
            Dim np As New NormalResponse(True, "success", "", "")
            Return np
        Else
            Dim np As New NormalResponse(False, result, "", "")
            Return np
        End If
    End Function
    Private Function VLCInfoToPhoneInfo(vlc As VLCTestInfo) As PhoneInfo
        If IsNothing(vlc) Then Return Nothing
        Dim pi As New PhoneInfo
        pi.businessType = vlc.BusinessType
        pi.phoneModel = vlc.phoneModel
        pi.phoneName = vlc.phoneName
        pi.phoneOS = vlc.OS
        pi.phonePRODUCT = ""
        pi.carrier = vlc.CARRIER
        pi.IMSI = vlc.IMSI
        pi.IMEI = vlc.IMEI
        pi.RSRP = vlc.RSRP
        pi.SINR = vlc.SINR
        pi.RSRQ = 0
        pi.TAC = vlc.tac
        pi.PCI = 0
        pi.CI = 0
        pi.eNodeBId = vlc.enodebid
        pi.cellId = vlc.cellid
        pi.netType = vlc.netType
        pi.sigNalType = vlc.netType
        pi.sigNalInfo = vlc.SIGNAL_Info
        pi.lon = vlc.LON
        pi.lat = vlc.LAT
        pi.accuracy = vlc.accuracy
        pi.altitude = vlc.altitude
        pi.gpsSpeed = vlc.speed
        pi.satelliteCount = vlc.satelliteCount
        pi.address = vlc.ADDRESS
        pi.apkVersion = vlc.apkVersion
        pi.apkName = vlc.apkName
        Return pi
    End Function
    Private Sub HandleGetNewPIByPhoneModel(phoneModel As String, RSRP As Double, time As String, lon As Double, lat As Double)
        Dim th As New Thread(Sub()
                                 Try
                                     If IsNothing(GetNewPiHelper.getNewPIByModelListLock) Then
                                         GetNewPiHelper.getNewPIByModelListLock = New Object
                                     End If
                                     Dim count As Integer = 0
                                     SyncLock GetNewPiHelper.getNewPIByModelListLock
                                         Dim getNewPIByModelList As List(Of GetNewPointByPhoneModelHelper) = GetNewPiHelper.getNewPIByModelList
                                         If IsNothing(getNewPIByModelList) Then Return
                                         If getNewPIByModelList.Count = 0 Then Return
                                         For i = getNewPIByModelList.Count - 1 To 0 Step -1
                                             Try
                                                 Dim itm As GetNewPointByPhoneModelHelper = getNewPIByModelList(i)
                                                 If itm.phoneModel = phoneModel Then
                                                     itm.Reply(RSRP, time, lon, lat)
                                                     getNewPIByModelList.RemoveAt(i)
                                                 End If
                                             Catch ex As Exception

                                             End Try
                                         Next
                                         GetNewPiHelper.getNewPIByModelList = getNewPIByModelList
                                         count = getNewPIByModelList.Count
                                     End SyncLock
                                     '  File.WriteAllText("d:\iisCount.txt", count)
                                 Catch ex As Exception
                                     ' File.WriteAllText("d:\iiserror.txt", ex.ToString)
                                 End Try
                             End Sub)
        th.Start()

    End Sub

    '视频加载分
    Private Function GetVideo_LOAD_Score(loadTime As Long, totalTime As Long) As Single
        Dim Score As Single = 1
        If loadTime <= 100 Then Score = 5
        If loadTime <= 1000 Then Score = 4
        If loadTime <= 3000 Then Score = 3
        If loadTime <= 5000 Then Score = 2

        If totalTime <= 1000 Then Score = (5 + Score) / 2
        If totalTime <= 3000 Then Score = (4 + Score) / 2
        If totalTime <= 5000 Then Score = (3 + Score) / 2
        If totalTime <= 6000 Then
            Score = (2 + Score) / 2
        Else
            Score = (1 + Score) / 2
        End If
        Return Score
    End Function

    Private Function GetVideo_STALL_Score(stallTime As Long, stallCount As Integer, totalTime As Long) As Integer
        Dim Score As Single = 1
        Dim p As Double = 100 * stallTime / totalTime
        If p = 0 Then Score = 5
        If p < 0.1 Then Score = 4
        If p < 1 Then Score = 3
        If p < 5 Then Score = 2

        If stallCount = 0 Then Score = (Score + 5) / 2
        If stallCount = 1 Then Score = (Score + 4) / 2

        If stallCount = 2 Then Score = (Score + 3) / 2

        If stallCount = 3 Then Score = (Score + 2) / 2

        If stallCount > 3 Then Score = (Score + 1) / 2

        Return Score
    End Function

    Private Function GetVideo_Buffer_Total_Score(ByVal pinfo As VLCTestInfo) As Integer
        If pinfo.BVRate < 10 Then Return 5
        If pinfo.BVRate < 30 Then Return 4
        If pinfo.BVRate < 50 Then Return 3
        If pinfo.BVRate < 70 Then Return 2
        Return 1
    End Function
    Private Function GetVMOS(ByVal Video_LOAD_Score As Integer, ByVal Video_STALL_Score As Integer) As Single
        Dim d As Double = 0.5 * Video_LOAD_Score + 0.5 * Video_STALL_Score
        'Dim i As Single = Math.Ceiling(d)
        Return d
    End Function
    Private Function GetQoEHttpResponseScore(responseRime As Long) As Integer
        If (responseRime > 5000) Then Return 1
        If (responseRime > 3000) Then Return 2
        If (responseRime > 2000) Then Return 3
        If (responseRime > 500) Then Return 4

        Return 5
    End Function

    Private Function GetQoerPingScore(time As Single) As Integer
        If (time > 1000) Then Return 1
        If (time > 500) Then Return 2
        If (time > 200) Then Return 3
        If (time > 50) Then Return 4

        Return 5
    End Function

    Public Function Handle_GetQoeReportDevices(ByVal context As HttpContext) As NormalResponse '获取手机型号
        Try
            Dim sql As String = "SELECT phoneModel from QOE_REPORT_TABLE GROUP BY phoneModel"
            Dim dt As DataTable = SQLGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何数据")
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Public Function Handle_GetNewPointByPhoneModel(ByVal context As HttpContext) As NormalResponse '按手机型号获取点
        Try
            Dim phoneModel As String = context.Request.QueryString("phoneModel")
            If IsNothing(GetNewPiHelper.getNewPIByModelListLock) Then
                GetNewPiHelper.getNewPIByModelListLock = New Object
            End If
            Dim count As Integer = 0
            SyncLock GetNewPiHelper.getNewPIByModelListLock
                Dim getNewPIByModelList As List(Of GetNewPointByPhoneModelHelper) = GetNewPiHelper.getNewPIByModelList
                If IsNothing(getNewPIByModelList) Then getNewPIByModelList = New List(Of GetNewPointByPhoneModelHelper)
                getNewPIByModelList.Add(New GetNewPointByPhoneModelHelper(phoneModel, context))
                GetNewPiHelper.getNewPIByModelList = getNewPIByModelList
                count = getNewPIByModelList.Count
            End SyncLock
            Dim th As New Thread(Sub()
                                     Try
                                         myResponse(context, New NormalResponse(False, "46"))
                                         'context.ApplicationInstance.CompleteRequest()
                                         'HttpContext.Current.ApplicationInstance.CompleteRequest()
                                     Catch ex As Exception
                                         '  MsgBox(ex.ToString)
                                     End Try
                                 End Sub)
            th.Start()
            Sleep(3000)
            SyncLock GetNewPiHelper.getNewPIByModelListLock
                Try
                    If IsNothing(GetNewPiHelper.getNewPIByModelListLock) Then
                        GetNewPiHelper.getNewPIByModelListLock = New Object
                    End If
                    SyncLock GetNewPiHelper.getNewPIByModelListLock
                        Dim getNewPIByModelList As List(Of GetNewPointByPhoneModelHelper) = GetNewPiHelper.getNewPIByModelList
                        If IsNothing(getNewPIByModelList) = False Then
                            For i = getNewPIByModelList.Count - 1 To 0 Step -1
                                Try
                                    Dim itm As GetNewPointByPhoneModelHelper = getNewPIByModelList(i)
                                    If itm.phoneModel = phoneModel Then
                                        getNewPIByModelList.RemoveAt(i)
                                    End If

                                Catch ex As Exception

                                End Try
                            Next
                        End If
                        GetNewPiHelper.getNewPIByModelList = getNewPIByModelList
                    End SyncLock
                Catch ex As Exception

                End Try
            End SyncLock
            Return New NormalResponse(False, "没有最新消息", "", count)
        Catch ex As Exception
            Dim str As String = ex.ToString
            '   File.WriteAllLines("d:\iiserror.txt", str)
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function

    Public Function Handle_GetQoeHttpPoint(ByVal context As HttpContext) As NormalResponse '获得Http Qoe数据  Old Name :Handle_GetHttpQoePoint      New： 
        Dim Stepp As Single = 0
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim netType As String = context.Request.QueryString("netType")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            Dim grid As String = context.Request.QueryString("grid")
            Stepp = 1

            If carrier = "" Then
                Return New NormalResponse(False, "必须选择运营商")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,eNodeBId,CellId,Grid,VMOS from QOE_HTTP_TABLE "
            Dim doHaveWhere As Boolean = False
            If province <> "" Then
                sql = sql & " where province='" & province & "'"
                doHaveWhere = True
            End If
            If carrier <> "" Then
                If doHaveWhere Then
                    sql = sql & " and carrier='" & carrier & "'"
                Else
                    sql = sql & " where carrier='" & carrier & "'"
                    doHaveWhere = True
                End If
            End If
            If city <> "" Then
                If doHaveWhere Then
                    sql = sql & " and city='" & city & "'"
                Else
                    sql = sql & " where city='" & city & "'"
                    doHaveWhere = True
                End If
            End If
            If district <> "" Then
                If doHaveWhere Then
                    sql = sql & " and district='" & district & "'"
                Else
                    sql = sql & " where district='" & district & "'"
                    doHaveWhere = True
                End If
            End If
            If netType <> "" Then
                If doHaveWhere Then
                    sql = sql & " and net_Type='" & netType & "'"
                Else
                    sql = sql & " where net_Type='" & netType & "'"
                    doHaveWhere = True
                End If
            End If

            If grid <> "" Then
                If doHaveWhere Then
                    sql = sql & " and grid='" & grid & "'"
                Else
                    sql = sql & " where grid='" & grid & "'"
                    doHaveWhere = True
                End If
            End If

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        If doHaveWhere Then
                            sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                        Else
                            sql = sql & " where datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                            doHaveWhere = True
                        End If
                    Catch ex As Exception
                        Return New NormalResponse(False, "起始时间或结束时间格式非法")
                    End Try
                Else
                    Return New NormalResponse(False, "缺少结束时间")
                End If
            End If
            sql = sql & "  order by datetime desc "
            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "province"
            dt.Columns(2).ColumnName = "city"
            dt.Columns(3).ColumnName = "district"
            dt.Columns(4).ColumnName = "netType"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "RSRP"
            dt.Columns(8).ColumnName = "SINR"

            dt.Columns(9).ColumnName = "eNodeBId"
            dt.Columns(10).ColumnName = "CellId"
            dt.Columns(11).ColumnName = "Grid"
            dt.Columns(12).ColumnName = "QOE"

            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetQoeHttpPointErr:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function


    Public Function Handle_GetQoeVideoPoint(ByVal context As HttpContext) As NormalResponse '获得Qoe数据  Old Name :Handle_GetQoePoint      New： Handle_GetQoeVideoPoint
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim netType As String = context.Request.QueryString("netType")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            Dim grid As String = context.Request.QueryString("grid")
            Stepp = 1

            If carrier = "" Then
                Return New NormalResponse(False, "必须选择运营商")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select datetime,province,city,district,net_Type,GDlon,GDlat,SIGNAL_STRENGTH,SINR,eNodeBId,CellId,Grid,VMOS from Qoe_Video_TABLE "
            Dim doHaveWhere As Boolean = False
            If province <> "" Then
                sql = sql & " where province='" & province & "'"
                doHaveWhere = True
            End If
            If carrier <> "" Then
                If doHaveWhere Then
                    sql = sql & " and carrier='" & carrier & "'"
                Else
                    sql = sql & " where carrier='" & carrier & "'"
                    doHaveWhere = True
                End If
            End If
            If city <> "" Then
                If doHaveWhere Then
                    sql = sql & " and city='" & city & "'"
                Else
                    sql = sql & " where city='" & city & "'"
                    doHaveWhere = True
                End If
            End If
            If district <> "" Then
                If doHaveWhere Then
                    sql = sql & " and district='" & district & "'"
                Else
                    sql = sql & " where district='" & district & "'"
                    doHaveWhere = True
                End If
            End If
            If netType <> "" Then
                If doHaveWhere Then
                    sql = sql & " and net_Type='" & netType & "'"
                Else
                    sql = sql & " where net_Type='" & netType & "'"
                    doHaveWhere = True
                End If
            End If

            If grid <> "" Then
                If doHaveWhere Then
                    sql = sql & " and grid='" & grid & "'"
                Else
                    sql = sql & " where grid='" & grid & "'"
                    doHaveWhere = True
                End If
            End If

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        If doHaveWhere Then
                            sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                        Else
                            sql = sql & " where datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                            doHaveWhere = True
                        End If
                    Catch ex As Exception
                        Return New NormalResponse(False, "起始时间或结束时间格式非法")
                    End Try
                Else
                    Return New NormalResponse(False, "缺少结束时间")
                End If
            End If
            sql = sql & "  order by datetime desc "
            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"
            'dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "province"
            dt.Columns(2).ColumnName = "city"
            dt.Columns(3).ColumnName = "district"
            dt.Columns(4).ColumnName = "netType"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "RSRP"
            dt.Columns(8).ColumnName = "SINR"

            dt.Columns(9).ColumnName = "eNodeBId"
            dt.Columns(10).ColumnName = "CellId"
            dt.Columns(11).ColumnName = "Grid"
            dt.Columns(12).ColumnName = "QOE"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetQoePointErr:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function




    Public Function Handle_GetQoeReportProAndCity(ByVal context As HttpContext) As NormalResponse '获取SDK中运营商的省、市、区信息
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {‘中国联通’,'中国移动','中国电信'}

            If carrier.Length = 0 Then
                Return New NormalResponse(False, "必须指定运营商，eg：‘中国联通’,'中国移动','中国电信'")
            End If

            Dim sql As String = "select province,city,district from QOE_REPORT_TABLE where carrier='" & carrier & "' GROUP BY province,city,district order by province asc"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            Dim list As New List(Of ProAndCity)
            For Each row As DataRow In dt.Rows
                Dim province As String = row(0).ToString
                Dim cityName As String = row(1).ToString
                Dim districtName As String = row(2).ToString
                If province <> "" Then
                    Dim isfindPro As Boolean = False
                    For i = 0 To list.Count - 1
                        Dim pro As ProAndCity = list(i)
                        If pro.province = province Then
                            isfindPro = True
                            Dim isfindCity As Boolean = False
                            For j = 0 To pro.cityList.Count - 1
                                Dim city As cityInfo = pro.cityList(j)
                                If city.city = cityName Then
                                    isfindCity = True
                                    city.district.Add(districtName)
                                    pro.cityList(j) = city
                                End If
                            Next
                            If isfindCity = False Then
                                pro.cityList.Add(New cityInfo(cityName, districtName))
                            End If
                            list(i) = pro
                        End If
                    Next
                    If isfindPro = False Then
                        list.Add(New ProAndCity(province, cityName, districtName))
                    End If
                End If
            Next

            Return New NormalResponse(True, "", "", list)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function


    Public Function Handle_GetQoeReportRSRPPoint(ByVal context As HttpContext) As NormalResponse '获得QoeReport数据  Old Name :Handle_GetSmartPlanRSRPPoint      New： 
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim netType As String = context.Request.QueryString("netType")
            Dim grid As String = context.Request.QueryString("grid")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            'Stepp = 1

            If carrier = "" Then
                Return New NormalResponse(False, "必须选择运营商")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,QoeR,eNodeBId,CellId,Grid from QOE_REPORT_TABLE "
            Dim doHaveWhere As Boolean = False
            If province <> "" Then
                sql = sql & " where province='" & province & "'"
                doHaveWhere = True
            End If
            If carrier <> "" Then
                If doHaveWhere Then
                    sql = sql & " and carrier='" & carrier & "'"
                Else
                    sql = sql & " where carrier='" & carrier & "'"
                    doHaveWhere = True
                End If
            End If
            If city <> "" Then
                If doHaveWhere Then
                    sql = sql & " and city='" & city & "'"
                Else
                    sql = sql & " where city='" & city & "'"
                    doHaveWhere = True
                End If
            End If
            If district <> "" Then
                If doHaveWhere Then
                    sql = sql & " and district='" & district & "'"
                Else
                    sql = sql & " where district='" & district & "'"
                    doHaveWhere = True
                End If
            End If
            If netType <> "" Then
                If doHaveWhere Then
                    sql = sql & " and netType='" & netType & "'"
                Else
                    sql = sql & " where netType='" & netType & "'"
                    doHaveWhere = True
                End If
            End If

            If grid <> "" Then
                If doHaveWhere Then
                    sql = sql & " and grid='" & grid & "'"
                Else
                    sql = sql & " where grid='" & grid & "'"
                    doHaveWhere = True
                End If
            End If

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        If doHaveWhere Then
                            sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                        Else
                            sql = sql & " where datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                            doHaveWhere = True
                        End If
                    Catch ex As Exception
                        Return New NormalResponse(False, "起始时间或结束时间格式非法")
                    End Try
                Else
                    Return New NormalResponse(False, "缺少结束时间")
                End If
            End If
            sql = sql & "  order by datetime desc "
            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "province"
            dt.Columns(2).ColumnName = "city"
            dt.Columns(3).ColumnName = "district"
            dt.Columns(4).ColumnName = "netType"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "RSRP"
            dt.Columns(8).ColumnName = "SINR"
            dt.Columns(9).ColumnName = "Qoe"
            dt.Columns(10).ColumnName = "eNodeBId"
            dt.Columns(11).ColumnName = "CellId"
            dt.Columns(12).ColumnName = "Grid"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetQoeR RSRPPointErr:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function



    Public Function Handle_GetUeQoeReportRSRPPoint(ByVal context As HttpContext) As NormalResponse '获得用户QoeReport数据 
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            'Dim province As String = context.Request.QueryString("province")
            'Dim city As String = context.Request.QueryString("city")
            'Dim district As String = context.Request.QueryString("district")
            Dim imei As String = context.Request.QueryString("imei")
            Dim netType As String = context.Request.QueryString("netType")
            Dim grid As String = context.Request.QueryString("grid")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            'Stepp = 1

            If imei = "" Then
                Return New NormalResponse(False, "必须选择imei")
                ' Return New NormalResponse(False, "必须选择imei")
            End If
            If carrier = "" Then
                Return New NormalResponse(False, "必须选择运营商")
            End If
            If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,QoeR,eNodeBId,CellId,Grid from QOE_REPORT_TABLE "
            Dim doHaveWhere As Boolean = False
            'If province <> "" Then
            '    sql = sql & " where province='" & province & "'"
            '    doHaveWhere = True
            'End If
            sql = sql & " where imei='" & imei & "'"
            doHaveWhere = True


            sql = sql & " and carrier='" & carrier & "'"


            If netType <> "" Then
                If doHaveWhere Then
                    sql = sql & " and netType='" & netType & "'"
                Else
                    sql = sql & " where netType='" & netType & "'"
                    doHaveWhere = True
                End If
            End If

            If grid <> "" Then
                If doHaveWhere Then
                    sql = sql & " and grid='" & grid & "'"
                Else
                    sql = sql & " where grid='" & grid & "'"
                    doHaveWhere = True
                End If
            End If

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        If doHaveWhere Then
                            sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                        Else
                            sql = sql & " where datetime>='" & startTime & "' and datetime<='" & endTime & "'"
                            doHaveWhere = True
                        End If
                    Catch ex As Exception
                        Return New NormalResponse(False, "起始时间或结束时间格式非法")
                    End Try
                Else
                    Return New NormalResponse(False, "缺少结束时间")
                End If
            End If
            sql = sql & "  order by datetime desc "
            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "province"
            dt.Columns(2).ColumnName = "city"
            dt.Columns(3).ColumnName = "district"
            dt.Columns(4).ColumnName = "netType"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "RSRP"
            dt.Columns(8).ColumnName = "SINR"
            dt.Columns(9).ColumnName = "Qoe"
            dt.Columns(10).ColumnName = "eNodeBId"
            dt.Columns(11).ColumnName = "CellId"
            dt.Columns(12).ColumnName = "Grid"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetUeQoeReportRSRPPoint Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function
    Public Function Handle_GetTtLteCellInfo(ByVal context As HttpContext) As NormalResponse '获得运营商4G小区工参
        Dim Stepp As Integer = 0
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim siteType As String = context.Request.QueryString("siteType")
            'Dim grid As String = context.Request.QueryString("grid")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            Stepp = 1

            If city = "" Then
                Return New NormalResponse(False, "必须选择城市")
            End If

            If carrier <> "移动" And carrier <> "联通" And carrier <> "电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select carrier,district,enodebName,enodebName_Tt,state,gdLon,gdLat,siteType,h from TtLTE_CELINFO_Table "
            sql = sql & " where city='" & city & "'"
            sql = sql & " and instr(carrier,'" & carrier & "')>0 "
            If district <> "" Then sql = sql & " and district='" & district & "'"
            If siteType <> "" Then sql = sql & " and siteType='" & siteType & "'"

            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4

            'carrier,district,enodebName,enodebName_Tt,state,gdLon,gdLat,siteType,h

            dt.Columns(0).ColumnName = "carrier"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "district"
            dt.Columns(2).ColumnName = "enodebName"
            dt.Columns(3).ColumnName = "enodebName_Tt"
            dt.Columns(4).ColumnName = "state"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "siteType"
            dt.Columns(8).ColumnName = "h"

            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetTtLteCellInfo Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function



    Public Function Handle_GetLteCellInfo(ByVal context As HttpContext) As NormalResponse '获得运营商4G小区工参
        Dim Stepp As Integer = 0
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'联通','移动','电信'}
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim siteType As String = context.Request.QueryString("siteType")
            'Dim grid As String = context.Request.QueryString("grid")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            Stepp = 1

            If carrier = "" Then
                Return New NormalResponse(False, "必须选择运营商")
            End If
            If city = "" Then
                Return New NormalResponse(False, "必须选择城市")
            End If

            If carrier <> "移动" And carrier <> "联通" And carrier <> "电信" Then
                Return New NormalResponse(False, "运营商错误")
                ' Return New NormalResponse(False, "必须选择省份")carrier,
            End If
            Dim sql As String = "select district,enodebName,ecellName,eNodeBId,cellId,gdLon,gdLat,siteType,h,amzimuth,Tac,freq,pci from LTE_CELINFO_Table "

            sql = sql & " where carrier='" & carrier & "'"
            sql = sql & " and city='" & city & "'"

            If district <> "" Then sql = sql & " and district='" & district & "'"
            If siteType <> "" Then sql = sql & " and siteType='" & siteType & "'"

            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                        '  sql = sql & "  limit " & readCount & " offset " & readCount
                    End If
                End If
            End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' district,enodebName,ecellName,eNodeBId,cellId,gdLon,gdLat,siteType,h,amzimuth,Tac,freq,pci from LTE_CELINFO_Table "

            ' "Select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "district"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "enodebName"
            dt.Columns(2).ColumnName = "ecellName"
            dt.Columns(3).ColumnName = "eNodeBId"
            dt.Columns(4).ColumnName = "cellId"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "siteType"
            dt.Columns(8).ColumnName = "h"
            dt.Columns(9).ColumnName = "amzimuth"
            dt.Columns(10).ColumnName = "Tac"
            dt.Columns(11).ColumnName = "freq"
            dt.Columns(12).ColumnName = "pci"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetLteCellInfo Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function

    Public Function Handle_GetMRGridlist(ByVal context As HttpContext) As NormalResponse  '获取MR的网格列表
        Try
            Dim sql As String = "select grid from maomingMRTable GROUP BY grid"
            Dim dt As DataTable = pgSQLLocalhost.SQLGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            Dim list As New List(Of String)
            For Each row As DataRow In dt.Rows
                If IsNothing(row(0)) = False Then
                    Dim grid As String = row(0).ToString
                    list.Add(grid)
                End If
            Next
            Return New NormalResponse(True, "", "", list)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function


    Public Function Handle_GetMRProAndCity(ByVal context As HttpContext) As NormalResponse '获取茂名MR省、市、区
        Try
            Dim sql As String = "select province,city,district from maomingMRTable GROUP BY province,city,district"
            Dim dt As DataTable = SQLGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据")
            End If
            Dim list As New List(Of ProAndCity)
            For Each row As DataRow In dt.Rows
                Dim province As String = row(0).ToString
                Dim cityName As String = row(1).ToString
                Dim districtName As String = row(2).ToString
                If province <> "" Then
                    Dim isfindPro As Boolean = False
                    For i = 0 To list.Count - 1
                        Dim pro As ProAndCity = list(i)
                        If pro.province = province Then
                            isfindPro = True
                            Dim isfindCity As Boolean = False
                            For j = 0 To pro.cityList.Count - 1
                                Dim city As cityInfo = pro.cityList(j)
                                If city.city = cityName Then
                                    isfindCity = True
                                    city.district.Add(districtName)
                                    pro.cityList(j) = city
                                End If
                            Next
                            If isfindCity = False Then
                                pro.cityList.Add(New cityInfo(cityName, districtName))
                            End If
                            list(i) = pro
                        End If
                    Next
                    If isfindPro = False Then
                        list.Add(New ProAndCity(province, cityName, districtName))
                    End If
                End If
            Next

            Return New NormalResponse(True, "", "", list)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    Public Function Handle_GetMRPoint(ByVal context As HttpContext) As NormalResponse  '给前端返回MR数据 主要业务
        Try
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim grid As String = context.Request.QueryString("grid")
            Dim eNodeBId As String = context.Request.QueryString("eNodeBId")
            Dim cellId As String = context.Request.QueryString("cellId")
            Dim ECI As String = context.Request.QueryString("ECI")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim siteType As String = context.Request.QueryString("siteType")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As String = context.Request.QueryString("readIndex")
            Dim readCount As String = context.Request.QueryString("readCount")
            Dim isDayTime As String = context.Request.QueryString("isDayTime")

            If province = "" Then
                'Return New NormalResponse(False, "必须选择省份")
            End If
            Dim sql As String = "select UEGDlon,UEGDlat,RSRP,SINR,Grid,siteType,eNodeBId,eci,cellid,startTime from UP_MMMRTABLE "
            Dim doHaveWhere As Boolean = False
            If province <> "" Then
                sql = sql & " where province='" & province & "'"
                doHaveWhere = True
            End If
            If city <> "" Then
                If doHaveWhere Then
                    sql = sql & " and city='" & city & "'"
                Else
                    sql = sql & " where city='" & city & "'"
                    doHaveWhere = True
                End If
            End If
            If district <> "" Then
                If doHaveWhere Then
                    sql = sql & " and district='" & district & "'"
                Else
                    sql = sql & " where district='" & district & "'"
                    doHaveWhere = True
                End If
            End If
            If grid <> "" Then
                If doHaveWhere Then
                    sql = sql & " and grid=" & grid & ""
                Else
                    sql = sql & " where grid=" & grid & ""
                    doHaveWhere = True
                End If
            End If
            If eNodeBId <> "" Then
                If doHaveWhere Then
                    sql = sql & " and eNodeBId='" & eNodeBId & "'"
                Else
                    sql = sql & " where eNodeBId='" & eNodeBId & "'"
                    doHaveWhere = True
                End If
            End If
            If cellId <> "" Then
                If doHaveWhere Then
                    sql = sql & " and cellId='" & cellId & "'"
                Else
                    sql = sql & " where cellId='" & cellId & "'"
                    doHaveWhere = True
                End If
            End If
            If ECI <> "" Then
                If doHaveWhere Then
                    sql = sql & " and ECI='" & ECI & "'"
                Else
                    sql = sql & " where ECI='" & ECI & "'"
                    doHaveWhere = True
                End If
            End If
            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        If doHaveWhere Then
                            sql = sql & " and startTime>='" & startTime & "' and startTime<='" & endTime & "'"
                        Else
                            sql = sql & " where startTime>='" & startTime & "' and startTime<='" & endTime & "'"
                            doHaveWhere = True
                        End If
                    Catch ex As Exception
                        Return New NormalResponse(False, "起始时间或结束时间格式非法")
                    End Try
                Else
                    Return New NormalResponse(False, "缺少结束时间")
                End If
            End If
            If siteType <> "" Then
                If IsNumeric(siteType) Then
                    If siteType >= 0 Then
                        If siteType = 0 Or siteType = 1 Then
                            If doHaveWhere Then
                                sql = sql & " and  siteType=" & siteType
                            Else
                                sql = sql & " where siteType=" & siteType
                                doHaveWhere = True
                            End If
                        End If
                    End If
                End If
            End If
            'isDayTime
            If isDayTime <> "" Then '1是白天，0是晚上，为空则全量
                If IsNumeric(isDayTime) Then
                    If isDayTime <> 0 And isDayTime <> 1 Then isDayTime = 1
                    If doHaveWhere Then
                        sql = sql & " and  isDayTime=" & isDayTime
                    Else
                        sql = sql & " where isDayTime=" & isDayTime
                        doHaveWhere = True
                    End If
                End If
            End If

            If IsNothing(readIndex) = False Then
                If readIndex = 0 And readCount = 0 Then
                Else
                    If readIndex >= 0 Then
                        If readCount <= 0 Then
                            Return New NormalResponse(False, "readCount必须为正整数")
                        End If
                        sql = OracleSelectPage(sql, readIndex, readCount)
                    End If
                End If
            End If
            Dim workStartTime As Date = Now
            Dim dt As DataTable
            dt = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Dim workMsg As String = "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpam(workStartTime) & ",  apiVersion:" & apiVersion
            'dt.Columns(3).ColumnName = "lon"
            'dt.Columns(4).ColumnName = "lat"
            'UEGDlon, UEGDlat, RSRP, SINR, grid
            dt.Columns(0).ColumnName = "UEGDlon"
            dt.Columns(1).ColumnName = "UEGDlat"
            dt.Columns(2).ColumnName = "RSRP"
            dt.Columns(3).ColumnName = "SINR"
            dt.Columns(4).ColumnName = "Grid"
            dt.Columns(5).ColumnName = "siteType"
            dt.Columns(6).ColumnName = "eNodeBId"
            dt.Columns(7).ColumnName = "ECI"
            dt.Columns(8).ColumnName = "cellId"
            dt.Columns(9).ColumnName = "startTime"

            Return New NormalResponse(True, workMsg, sql, dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    Public Function Handle_GetENodeBIdYMData(context As HttpContext) As NormalResponse '获取Enodebid、cellid性能指标
        Try
            Dim eNodeBId As String = context.Request.QueryString("eNodeBId")
            Dim cellId As String = context.Request.QueryString("cellId")
            Dim time As String = context.Request.QueryString("time")
            Dim getCount As Integer = context.Request.QueryString("getCount")

            If time <> "" Then
                Try
                    Dim dtmp As Date = Date.Parse(time)
                    time = dtmp.ToString("yyyy-MM-dd HH:mm:ss")
                Catch ex As Exception
                    Return New NormalResponse(False, "时间格式非法")
                End Try
            End If
            If eNodeBId = "" Then
                Return New NormalResponse(False, "eNodeBId不可为空")
            End If
            Dim sql As String = "select * from UP_YM_MM_AQ_ALLCELL "
            Dim doHaveWhere As Boolean = False
            If eNodeBId <> "" Then
                If doHaveWhere Then
                    sql = sql & " and eNodeBId='" & eNodeBId & "'"
                Else
                    sql = sql & " where eNodeBId='" & eNodeBId & "'"
                    doHaveWhere = True
                End If
            End If
            If cellId <> "" Then
                If doHaveWhere Then
                    sql = sql & " and cellId='" & cellId & "'"
                Else
                    sql = sql & " where cellId='" & cellId & "'"
                    doHaveWhere = True
                End If
            End If
            If time <> "" Then
                If doHaveWhere Then
                    sql = sql & " and time='" & time & "'"
                Else
                    sql = sql & " where time='" & time & "'"
                    doHaveWhere = True
                End If
            Else
                Dim isGetCount As Boolean = False
                If IsNothing(getCount) = False Then
                    If IsNumeric(getCount) Then
                        If getCount > 0 Then
                            isGetCount = True
                        End If
                    End If
                End If
                If Not isGetCount Then
                    getCount = 20
                End If
            End If
            sql = sql & " order by time desc "
            If IsNothing(getCount) = False Then
                If IsNumeric(getCount) Then
                    If getCount > 0 Then
                        sql = OracleSelectPage(sql, 0, getCount)
                    End If
                End If
            End If
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.row.count=0")
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Private Function OracleSelectPage(sql As String, startIndex As Long, count As Long) As String
        Dim sb As New StringBuilder
        sb.Append("select * from ( ")
        sb.Append("select A.*, Rownum RN from ( ")
        sb.Append(sql)
        sb.Append(") A ")
        sb.Append(" where Rownum<=" & startIndex + count & " )")
        sb.Append("where RN>=" & startIndex)
        Return sb.ToString
    End Function
    Structure RunSQLInfo
        Dim connStr As String
        Dim sqllist As List(Of String)
    End Structure
    Private Function Decompress(ByVal data() As Byte) As Byte()
        Dim stream As MemoryStream = New MemoryStream
        Dim gZip As New GZipStream(New MemoryStream(data), CompressionMode.Decompress)
        Dim n As Integer = 0
        While True
            Dim by(409600) As Byte
            n = gZip.Read(by, 0, by.Length)
            If n = 0 Then Exit While
            stream.Write(by, 0, n)
        End While
        gZip.Close()
        Return stream.ToArray
    End Function
    Public Function Handle_RunSQL(ByVal context As HttpContext, ByVal data As Object) As NormalResponse '按Sql来查询
        Try
            Dim str As String = data.ToString
            Dim by() As Byte = Convert.FromBase64String(str)
            Dim realBy() As Byte = Decompress(by)
            str = Encoding.Default.GetString(realBy)
            Dim runSqlInfo As RunSQLInfo = JsonConvert.DeserializeObject(str, GetType(RunSQLInfo))
            If IsNothing(runSqlInfo) Then
                Return New NormalResponse(False, "RunSQLInfo格式非法")
            End If
            If runSqlInfo.sqllist.Count = 0 Then
                Return New NormalResponse(False, "RunSQLInfo.SqlList.count=0")
            End If
            Dim conn As String = runSqlInfo.connStr
            Dim failcount As Integer = 0
            Dim successcount As Integer = 0
            Dim startTime As Date = Now
            Dim result As String = SQLInsertSQLList(conn, runSqlInfo.sqllist)
            'Dim th As New Thread(Sub()
            '                         Dim conn As String = runSqlInfo.connStr
            '                         Dim failCount As Integer = 0
            '                         Dim successCount As Integer = 0
            '                         For Each itm In runSqlInfo.sqllist
            '                             If SQLCmdWithConn(conn, itm) Then
            '                                 successCount = successCount + 1
            '                             Else
            '                                 failCount = failCount + 1
            '                             End If
            '                         Next
            '                     End Sub)
            'th.Start()
            Dim np As NormalResponse
            If result = "success" Then
                np = New NormalResponse(True, result, "", "提交SQL总行数:" & runSqlInfo.sqllist.Count & ",耗时:" & GetTimeSpam(startTime))
            Else
                np = New NormalResponse(False, result, "", "提交SQL总行数:" & runSqlInfo.sqllist.Count & ",耗时:" & GetTimeSpam(startTime))
            End If
            Return np
            'If failCount = 0 Then
            '    Return New NormalResponse(True, "success")
            'Else
            '    Return New NormalResponse(True, "成功:" & successCount & ",失败:" & failCount)
            'End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Structure GZfileInfo
        Dim cellInfo As cellInfo
        Dim eNodeBId As String
        Dim base64 As String
        Sub New(cellinfo As cellInfo, eNodeBId As String, base64 As String)
            Me.cellInfo = cellinfo
            Me.eNodeBId = eNodeBId
            Me.base64 = base64
        End Sub
    End Structure
    Public Function Handle_UploadMRFile(ByVal context As HttpContext, ByVal data As Object) As NormalResponse '上传MR文件
        Try
            Dim startTime As Date = Now
            Dim str As String = data.ToString
            Dim gzi As GZfileInfo = JsonConvert.DeserializeObject(str, GetType(GZfileInfo))
            If IsNothing(gzi) Then Return New NormalResponse(False, "gzi is null")
            Dim cellinfo As cellInfo = gzi.cellInfo
            Dim eNodeBId As String = gzi.eNodeBId
            Dim by() As Byte = Convert.FromBase64String(gzi.base64)
            Dim realBuffer() As Byte = Decompress(by)
            Dim np As NormalResponse = HandleXMLfile(Encoding.UTF8.GetString(realBuffer), eNodeBId, cellinfo)
            If IsNothing(np) Then
                Return New NormalResponse(False, "HandleXMLfile result is null")
            End If
            If np.result Then
                Return New NormalResponse(True, "录入成功，总用时:" & GetTimeSpam(startTime), np.errmsg, "")
            Else
                Return np
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Private Function HandleXMLfile(xmlstring As String, eNodeBIdInsert As String, cellinfoInsert As cellInfo) As NormalResponse '处理XML文件
        ' If File.Exists(filePath) = False Then Return

        Dim doc As New XmlDocument()
        ' doc.Load(filePath)
        doc.LoadXml(xmlstring)
        Dim startTime As String = doc.GetElementsByTagName("fileHeader")(0).Attributes("startTime").Value
        startTime = startTime.Replace("T", " ").Split(".")(0)
        Dim eNodebId As String = doc.GetElementsByTagName("eNB")(0).Attributes("id").Value
        If eNodebId <> eNodeBIdInsert Then
            Return New NormalResponse(False, "eNodebId <> eNodeBIdInsert")
        End If
        Dim measurementNode As XmlNode = doc.GetElementsByTagName("measurement")(0)
        Dim xmlHeaders As String = doc.GetElementsByTagName("smr")(0).InnerText
        Dim heads As String() = xmlHeaders.Split(" ")
        Dim dt As New DataTable
        Dim headCount As Integer = 0
        For Each itm In heads
            If itm <> "" Then
                itm = itm.Replace("MR.", "")
                dt.Columns.Add(itm)
            End If
        Next
        headCount = dt.Columns.Count
        dt.Columns.Add("startTime")
        dt.Columns.Add("eNodebId")
        dt.Columns.Add("ECI")
        dt.Columns.Add("MmeUeS1apId")
        dt.Columns.Add("MmeGroupId")
        dt.Columns.Add("MmeCode")
        dt.Columns.Add("lon")
        dt.Columns.Add("lat")
        dt.Columns.Add("province")
        dt.Columns.Add("city")
        dt.Columns.Add("district")
        dt.Columns.Add("grid")
        dt.Columns.Add("siteType")
        dt.Columns.Add("cellId")
        For Each node As XmlNode In measurementNode.ChildNodes
            Try
                If node.Name = "object" Then
                    Dim ECI As String = node.Attributes("id").Value
                    Dim MmeUeS1apId As String = node.Attributes("MmeUeS1apId").Value
                    Dim MmeGroupId As String = node.Attributes("MmeGroupId").Value
                    Dim MmeCode As String = node.Attributes("MmeCode").Value
                    For Each son As XmlNode In node.ChildNodes
                        If son.Name = "v" Then
                            Dim str As String = son.InnerText
                            Dim st() As String = str.Split(" ")
                            Dim vList As New List(Of String)
                            For Each itm In st
                                If itm <> "" Then
                                    vList.Add(itm)
                                End If
                            Next
                            If vList.Count = headCount Then
                                Dim row As DataRow = dt.NewRow
                                For i = 0 To headCount - 1
                                    row(i) = vList(i)
                                Next
                                row("startTime") = startTime
                                row("eNodebId") = eNodebId
                                row("ECI") = ECI
                                row("cellId") = GetCellIdByECI(ECI)
                                row("MmeUeS1apId") = MmeUeS1apId
                                row("MmeGroupId") = MmeGroupId
                                row("MmeCode") = MmeCode
                                Dim cellInfo As cellInfo = cellinfoInsert
                                If IsNothing(cellInfo) = False Then
                                    row("lon") = cellInfo.lon
                                    row("lat") = cellInfo.lat
                                    row("province") = cellInfo.province
                                    row("city") = cellInfo.city
                                    row("district") = cellInfo.district
                                    row("grid") = cellInfo.grid
                                    row("siteType") = cellInfo.siteType
                                End If
                                dt.Rows.Add(row)
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                Console.WriteLine(ex.ToString)
            End Try
        Next
        doc = Nothing
        xmlstring = Nothing
        If IsNothing(dt) Then
            Return New NormalResponse(False, "doc--> dt is null")
        End If
        dt = HandleMRDt(dt)
        Dim rowCount As Integer = dt.Rows.Count
        Dim startTimeTmp As Date = Now
        Dim result2 As String = ORALocalhost.SqlCMDListQuickByPara("up_mmmrTable", dt)
        dt = Nothing
        If result2 = "success" Then
            Return New NormalResponse(True, "", "行数:" & rowCount & ",入库用时:" & GetTimeSpam(startTimeTmp), "")
        Else
            Return New NormalResponse(False, result2)
        End If

    End Function
    Structure cellInfo
        Dim lon As Double
        Dim lat As Double
        Dim district As String
        Dim province As String
        Dim city As String
        Dim grid As String
        Dim siteType As Integer
    End Structure


    Public Function Handle_UploadMRData(ByVal context As HttpContext, ByVal data As Object) As NormalResponse '上传MR数据
        Try
            Dim str As String = data.ToString
            Dim by() As Byte = Convert.FromBase64String(str)
            Dim realBy() As Byte = Decompress(by)
            str = Encoding.Default.GetString(realBy)
            Dim dt As DataTable = JsonConvert.DeserializeObject(str, GetType(DataTable))
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.row.count=0")

            '   dt = HandleMRDt(dt)
            'Dim th As New Thread(Sub()
            '                         Try
            '                             pgSQLLocalhost.DT2SQL("maomingmrtable", dt)
            '                         Catch ex As Exception
            '                             '  File.WriteAllText("d:\ORALocalhostErr.txt", ex.ToString)
            '                         End Try
            '                     End Sub)
            'th.Start()
            Dim startTime As Date = Now
            Dim result2 As String = "success"
            ' result2 = ORALocalhost.SqlCMDListQuickByPara("up_mmmrTable", dt)
            Dim th As New Thread(Sub()
                                     result2 = ORALocalhost.SqlCMDListQuickByPara("up_mmmrTable", dt)
                                 End Sub)
            th.Start()
            Dim result As String = "success"
            result = result2
            If result = "success" Then
                Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count)
                '  Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpam(startTime))
            Else
                Return New NormalResponse(False, result, "错误:数据库录入失败", "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Public Function Handle_DeleteMMYMDataByTime(ByVal context As HttpContext) As NormalResponse '通过时间条件 删除茂名KPI数据
        Try
            Dim time As String = context.Request.QueryString("time")
            Try
                Dim dtmp As Date = Date.Parse(time)
                time = dtmp.ToString("yyyy-MM-dd HH:mm:ss")
            Catch ex As Exception
                Return New NormalResponse(False, "时间格式非法")
            End Try
            Dim sql As String = "delete from UP_YM_MM_AQ_ALLCELL where time='" & time & "'"
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, "删除 " & time & " 成功！")
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Public Function Handle_UploadMMYMData(ByVal context As HttpContext, ByVal data As Object) As NormalResponse '增加茂名KPI数据
        Try
            Dim str As String = data.ToString
            Dim by() As Byte = Convert.FromBase64String(str)
            Dim realBy() As Byte = Decompress(by)
            str = Encoding.Default.GetString(realBy)
            Dim dt As DataTable = JsonConvert.DeserializeObject(str, GetType(DataTable))
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.row.count=0")
            Dim startTime As Date = Now
            Dim result2 As String = ORALocalhost.SqlCMDListQuickByPara("UP_YM_MM_AQ_ALLCELL", dt)
            Dim result As String = result2
            If result = "success" Then
                ' Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count)
                Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpam(startTime))
            Else
                Return New NormalResponse(False, result, "错误:数据库录入失败", "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function

    Private Function GetGridBySQL(lon As Double, lat As Double) As Integer
        Dim sql As String = "select id from grid_Table where startlon<=" & lon & " and stoplon>=" & lon & " and startlat<=" & lat & " and stoplat>=" & lat
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return 0
        If dt.Rows.Count = 0 Then Return 0
        Dim row As DataRow = dt.Rows(0)
        Dim str As String = row(0)
        If IsNothing(str) Then Return 0
        If str = "" Then Return 0
        If IsNumeric(str) = False Then Return 0
        Return str
    End Function
    Private Function GetAoARand() As Integer
        Return Int(Rnd() * 121) - 60
    End Function
    Private Function HandleMRDt(dt As DataTable) As DataTable
        If IsNothing(dt) Then Return dt
        'Dim sqlList As New List(Of String)
        dt.Columns.Add("UElon")
        dt.Columns.Add("UElat")
        dt.Columns.Add("UEdis")
        dt.Columns.Add("RSRP")
        dt.Columns.Add("SINR")
        dt.Columns.Add("BDlon")
        dt.Columns.Add("BDlat")
        dt.Columns.Add("UEBDlon")
        dt.Columns.Add("UEBDlat")
        dt.Columns.Add("GDlon")
        dt.Columns.Add("GDlat")
        dt.Columns.Add("UEGDlon")
        dt.Columns.Add("UEGDlat")
        'dt.Columns.Add("Grid")

        For i = dt.Rows.Count - 1 To 0 Step -1
            Dim row As DataRow = dt.Rows(i)
            If IsNothing(row("LteScTadv")) Then Continue For
            If IsNothing(row("LteScAOA")) Then Continue For
            If IsNothing(row("LteScTadv")) Then Continue For
            If IsNothing(row("LteScRSRP")) Then Continue For
            If IsNothing(row("LteScSinrUL")) Then Continue For
            If IsNothing(row("lon")) Then Continue For
            If IsNothing(row("lat")) Then Continue For
            If row("lon") = "NIL" Then Continue For
            If row("lat") = "NIL" Then Continue For
            If row("LteScTadv") = "NIL" Then Continue For
            If row("LteScAOA") = "NIL" Then Continue For
            If row("LteScTadv") = "NIL" Then Continue For
            If row("LteScRSRP") = "NIL" Then Continue For
            If row("LteScSinrUL") = "NIL" Then Continue For

            Dim eNodebId As String = row("eNodebId")
            Dim adv As Double = Val(row("LteScTadv"))
            Dim aoa As Double = Val(row("LteScAOA"))
            adv = adv * 78.12
            aoa = 360 - (aoa / 2)
            Dim oldAoa As Double = aoa
            aoa = aoa + GetAoARand()
            If aoa < 0 Or aoa > 360 Then aoa = oldAoa
            Dim lon As Double = Val(row("lon"))
            Dim lat As Double = Val(row("lat"))
            Dim UElon As Double = lon + (adv * Sin(aoa)) / (111000 * Cos(lat * PI / 180))
            Dim UElat As Double = lat + (adv * Cos(aoa * PI / 180)) / 111000
            Dim dis As Double = GetDistance(lat, lon, UElat, UElon)
            Dim RSRP As Double = row("LteScRSRP")
            Dim SINR As Double = row("LteScSinrUL")
            RSRP = RSRP - 140
            Dim grid As Integer = GetGridBySQL(UElon, UElat)
            If grid = 0 Then
                dt.Rows(i).Delete()
                Continue For
            End If
            row("UElon") = UElon
            row("UElat") = UElat
            row("UEdis") = dis
            row("RSRP") = RSRP
            row("SINR") = SINR

            row("Grid") = grid
            Dim BDlon, BDlat, BDUElon, BDUElat, GDlon, GDlat, GDUElon, GDUElat As Double
            Dim BDxt1 As CoordInfo = GPS2BDS(lon, lat)
            Dim BDxt2 As CoordInfo = GPS2BDS(UElon, UElat)
            If IsNothing(BDxt1) = False And IsNothing(BDxt2) = False Then
                BDlon = BDxt1.x
                BDlat = BDxt1.y
                BDUElon = BDxt2.x
                BDUElat = BDxt2.y
                row("BDlon") = BDlon
                row("BDlat") = BDlat
                row("UEBDlon") = BDUElon
                row("UEBDlat") = BDUElat
            End If
            Dim GDxt1 As CoordInfo = GPS2GDS(lon, lat)
            Dim GDxt2 As CoordInfo = GPS2GDS(UElon, UElat)
            If IsNothing(GDxt1) = False And IsNothing(GDxt2) = False Then
                GDlon = GDxt1.x
                GDlat = GDxt1.y
                GDUElon = GDxt2.x
                GDUElat = GDxt2.y
                row("GDlon") = GDlon
                row("GDlat") = GDlat
                row("UEGDlon") = GDUElon
                row("UEGDlat") = GDUElat
            End If
        Next
        Return dt
    End Function
    Private Function rad(ByVal d As Double) As Double
        rad = d * PI / 180
    End Function
    Private Function GetDistance(ByVal lat1 As Double, ByVal lng1 As Double, ByVal lat2 As Double, ByVal lng2 As Double) As Double
        Dim radlat1 As Double, radlat2 As Double
        Dim a As Double, b As Double, s As Double, Temp As Double
        radlat1 = rad(lat1)
        radlat2 = rad(lat2)
        a = radlat1 - radlat2
        b = rad(lng1) - rad(lng2)
        Temp = Sqrt(Sin(a / 2) ^ 2 + Cos(radlat1) * Cos(radlat2) * Sin(b / 2) ^ 2)
        s = 2 * Atan(Temp / Sqrt(-Temp * Temp + 1))
        s = s * 6378.137
        Return Math.Round(s * 1000, 2)
    End Function
    Private Function GetTimeSpam(ByVal t As Date) As String
        Dim endTime As Date = Now
        Dim ts As TimeSpan = endTime - t
        Dim str As String = ts.Hours.ToString("00") & ":" & ts.Minutes.ToString("00") & ":" & ts.Seconds.ToString("00") & "." & ts.Milliseconds.ToString("000")
        Return str
    End Function
    Private Function SQLCmdWithConn(ByVal conn As String, ByVal CmdString As String) As Boolean
        Try
            Dim SQL As New MySqlConnection(conn)
            SQL.Open()
            Dim SQLCommand As MySqlCommand = New MySqlCommand(CmdString, SQL)
            Dim ResultRowInt As Integer = SQLCommand.ExecuteNonQuery()
            SQL.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function Handle_GetIndexPageTable(context As HttpContext) As NormalResponse '2018-12-23 09:54:00 更新 增加首页拼接表
        Dim info As String
        Try
            'Dim count As String = context.Request.QueryString("count")
            'If count = "" Then count = 10
            'If IsNumeric(count) = False Then
            '    Return New NormalResponse(False, "count 非数字")
            'End If
            Dim sql As String = "select day,count(day) from QOE_REPORT_TABLE GROUP BY day ORDER BY day desc"
            sql = OracleSelectPage(sql, 0, 7)
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何数据")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何数据")
            Dim dayList As New List(Of String)
            For Each row In dt.Rows
                If IsNothing(row(0)) = False Then
                    If row(0) <> "" Then
                        dayList.Add(row(0))
                    End If
                End If
            Next
            Dim resultDt As New DataTable
            resultDt.Columns.Add("时间")
            resultDt.Columns.Add("采样点总数(移动)")
            resultDt.Columns.Add("采样点总数(联通)")
            resultDt.Columns.Add("采样点总数(电信)")
            'resultDt.Columns.Add("RSRP(移动)")
            'resultDt.Columns.Add("RSRP(联通)")
            'resultDt.Columns.Add("RSRP(电信)")
            resultDt.Columns.Add("覆盖率(移动)")
            resultDt.Columns.Add("覆盖率(联通)")
            resultDt.Columns.Add("覆盖率(电信)")
            resultDt.Columns.Add("QOE(移动)")
            resultDt.Columns.Add("QOE(联通)")
            resultDt.Columns.Add("QOE(电信)")
            resultDt.Columns.Add("HttpQOE(移动)")
            resultDt.Columns.Add("HttpQOE(联通)")
            resultDt.Columns.Add("HttpQOE(电信)")
            For Each itm In dayList
                Dim row As DataRow = resultDt.NewRow
                row("时间") = itm
                sql = "select CARRIER,COUNT(CARRIER) from QOE_REPORT_TABLE WHERE day='" & itm & "' GROUP BY CARRIER"
                dt = Nothing
                dt = ORALocalhost.SqlGetDT(sql)
                If IsNothing(dt) = False Then
                    If dt.Rows.Count > 0 Then
                        For Each rw As DataRow In dt.Rows
                            If IsNothing(rw(0)) Then Continue For
                            If IsDBNull(rw(0)) Then Continue For
                            Dim carrier As String = rw(0)
                            Dim carrierCount As Integer = rw(1)
                            If carrier = "中国移动" Then
                                row("采样点总数(移动)") = carrierCount
                                'sql = "select round(avg(rsrp),0) from SDKTABLE WHERE day='" & itm & "' and CARRIER='中国移动' and rsrp<0"
                                'Dim info As String = ORALocalhost.SQLInfo(sql)
                                'If IsNothing(info) = False Then
                                '    If IsNumeric(info) Then
                                '        Dim rsrp As Integer = Val(info)
                                '        row("RSRP(移动)") = rsrp
                                '    End If
                                'End If

                                sql = "select count(rsrp) from QOE_REPORT_TABLE WHERE day='" & itm & "' and CARRIER='中国移动' and rsrp>-110 and rsrp<0"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim Cover As Single = 100 * Val(info) / carrierCount
                                        row("覆盖率(移动)") = Format(Cover, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),0) from Qoe_Video_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国移动'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("QOE(移动)") = Format(vmos, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),0) from QOE_HTTP_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国移动'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("HttpQOE(移动)") = Format(vmos, "0.00")
                                    End If
                                End If

                            End If

                            'MsgBox.show()
                            If carrier = "中国联通" Then
                                row("采样点总数(联通)") = carrierCount
                                'sql = "select round(avg(rsrp),0) from QOE_REPORT_TABLE WHERE day='" & itm & "' and CARRIER='中国联通' and rsrp<0"
                                'Dim info As String = ORALocalhost.SQLInfo(sql)
                                ''File.WriteAllText("d:\hhhhhSQLGetFirstRowCell.txt", info)
                                'If IsNothing(info) = False Then
                                '    If IsNumeric(info) Then
                                '        Dim rsrp As Integer = Math.Floor(Val(info))
                                '        row("RSRP(联通)") = rsrp
                                '    End If
                                'End If

                                sql = "select count(rsrp) from QOE_REPORT_TABLE WHERE day='" & itm & "' and CARRIER='中国联通' and rsrp>-110 and rsrp<0"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim rsrp As Single = 100 * Val(info) / carrierCount
                                        row("覆盖率(联通)") = Format(rsrp, "0.00")
                                    End If
                                End If


                                sql = "select round(avg(vmos),0) from Qoe_Video_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国联通'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("QOE(联通)") = Format(vmos, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),0) from QOE_HTTP_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国联通'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("HttpQOE(联通)") = Format(vmos, "0.00")
                                    End If
                                End If
                            End If

                            If carrier = "中国电信" Then
                                row("采样点总数(电信)") = carrierCount
                                'sql = "select round(avg(rsrp),0) from QOE_REPORT_TABLE WHERE day='" & itm & "' and CARRIER='中国电信' and rsrp<0"
                                'Dim info As String = ORALocalhost.SQLInfo(sql)
                                'If IsNothing(info) = False Then
                                '    If IsNumeric(info) Then
                                '        Dim rsrp As Integer = Math.Floor(Val(info))
                                '        row("RSRP(电信)") = rsrp
                                '    End If
                                'End If

                                sql = "select count(rsrp) from QOE_REPORT_TABLE WHERE day='" & itm & "' and CARRIER='中国电信' and rsrp>-110 and rsrp<0"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim Cover As Single = 100 * Val(info) / carrierCount
                                        row("覆盖率(电信)") = Format(Cover, "0.00")
                                    End If
                                End If


                                sql = "select round(avg(vmos),2) from Qoe_Video_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国电信'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("QOE(电信)") = Format(vmos, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),2) from QOE_HTTP_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国电信'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("HttpQOE(电信)") = Format(vmos, "0.00")
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
                resultDt.Rows.Add(row)
            Next

            Return New NormalResponse(True, "", "", resultDt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    Public Function Handle_GetAccount(ByVal context As HttpContext) As NormalResponse '获得用户账号 
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim userName As String = context.Request.QueryString("userName")
            Dim passWord As String = context.Request.QueryString("passWord")

            Stepp = 1

            If userName = "" Then Return New NormalResponse(False, "用户名为空错误")
            If passWord = "" Then Return New NormalResponse(False, "密码为空错误")


            'If carrier = "" Then
            '    Return New NormalResponse(False, "必须选择运营商")
            'End If
            'If carrier <> "移动" And carrier <> "联通" And carrier <> "电信" Then
            '    Return New NormalResponse(False, "运营商错误")
            '    ' Return New NormalResponse(False, "必须选择省份")carrier,
            'End If
            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,QoeR,eNodeBId,CellId,Grid from QOE_REPORT_TABLE "
            Dim doHaveWhere As Boolean = False
            'If province <> "" Then
            '    sql = sql & " where province='" & province & "'"
            '    doHaveWhere = True
            'End If
            sql = sql & " where userName='" & userName & "' and passWord='" & passWord & "'"
            doHaveWhere = True


            sql = sql & " and carrier='" & carrier & "'"


            If carrier <> "" Then sql = sql & " and carrier='" & carrier & "'"

            If province <> "" Then sql = sql & " and province='" & province & "'"
            If city <> "" Then sql = sql & " and city='" & city & "'"





            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"
            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "province"
            dt.Columns(2).ColumnName = "city"
            dt.Columns(3).ColumnName = "district"
            dt.Columns(4).ColumnName = "netType"
            dt.Columns(5).ColumnName = "lon"
            dt.Columns(6).ColumnName = "lat"
            dt.Columns(7).ColumnName = "RSRP"
            dt.Columns(8).ColumnName = "SINR"
            dt.Columns(9).ColumnName = "Qoe"
            dt.Columns(10).ColumnName = "eNodeBId"
            dt.Columns(11).ColumnName = "CellId"
            dt.Columns(12).ColumnName = "Grid"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetUeQoeReportRSRPPoint Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function

    Public Function Handle_GetIndexPageTable0(context As HttpContext) As NormalResponse '2018-12-23 09:54:00 更新 增加首页拼接表
        Try
            'Dim count As String = context.Request.QueryString("count")
            'If count = "" Then count = 10
            'If IsNumeric(count) = False Then
            '    Return New NormalResponse(False, "count 非数字")
            'End If
            Dim sql As String = "select substr(datetime,1,10),4*count(substr(datetime,1,10)) from Qoe_Video_TABLE GROUP BY substr(datetime,1,10) ORDER BY substr(datetime,1,10) desc"
            sql = OracleSelectPage(sql, 0, 7)
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何数据")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何数据")
            Dim dayList As New List(Of String)
            For Each row In dt.Rows
                If IsNothing(row(0)) = False Then
                    If row(0) <> "" Then
                        dayList.Add(row(0))
                    End If
                End If
            Next
            Dim resultDt As New DataTable
            resultDt.Columns.Add("时间")
            resultDt.Columns.Add("采样点总数(移动)")
            resultDt.Columns.Add("采样点总数(联通)")
            resultDt.Columns.Add("采样点总数(电信)")
            resultDt.Columns.Add("RSRP(移动)")
            resultDt.Columns.Add("RSRP(联通)")
            resultDt.Columns.Add("RSRP(电信)")
            For Each itm In dayList
                Dim row As DataRow = resultDt.NewRow
                row("时间") = itm
                sql = "select CARRIER,4*COUNT(CARRIER) from Qoe_Video_TABLE WHERE substr(datetime,1,10)='" & itm & "' GROUP BY CARRIER"
                dt = Nothing
                dt = ORALocalhost.SqlGetDT(sql)
                If IsNothing(dt) = False Then
                    If dt.Rows.Count > 0 Then
                        For Each rw As DataRow In dt.Rows
                            If IsNothing(rw(0)) Then Continue For
                            If IsDBNull(rw(0)) Then Continue For
                            Dim carrier As String = rw(0)
                            Dim carrierCount As Integer = rw(1)
                            If carrier = "中国移动" Then
                                row("采样点总数(移动)") = carrierCount
                                sql = "select sum(SIGNAL_STRENGTH) from Qoe_Video_TABLE WHERE substr(datetime,1,10)='" & itm & "' and CARRIER='中国移动' and SIGNAL_STRENGTH<0"
                                Dim info As String = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim rsrp As Integer = Math.Floor(Val(info) / carrierCount * 4)
                                        row("RSRP(移动)") = rsrp
                                    End If
                                End If
                            End If
                            If carrier = "中国联通" Then
                                row("采样点总数(联通)") = carrierCount
                                sql = "select sum(SIGNAL_STRENGTH) from Qoe_Video_TABLE WHERE substr(datetime,1,10)='" & itm & "' and CARRIER='中国联通' and SIGNAL_STRENGTH<0"
                                Dim info As String = ORALocalhost.SQLInfo(sql)
                                File.WriteAllText("d:\hhhhhSQLGetFirstRowCell.txt", info)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim rsrp As Integer = Math.Floor(Val(info) / carrierCount * 4)
                                        row("RSRP(联通)") = rsrp
                                    End If
                                End If
                            End If

                            If carrier = "中国电信" Then
                                row("采样点总数(电信)") = carrierCount
                                sql = "select sum(SIGNAL_STRENGTH) from Qoe_Video_TABLE WHERE substr(datetime,1,10)='" & itm & "' and CARRIER='中国电信' and SIGNAL_STRENGTH<0"
                                Dim info As String = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim rsrp As Integer = Math.Floor(Val(info) / carrierCount * 4)
                                        row("RSRP(电信)") = rsrp
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
                resultDt.Rows.Add(row)
            Next
            Return New NormalResponse(True, "", "", resultDt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Public Function Handle_GetQOEVideoSource(context As HttpContext) As NormalResponse
        Try
            Dim sql As String = "select * from QOE_VIDEO_SOURCE where isuse=1"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.rows.count=0")
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
End Class
