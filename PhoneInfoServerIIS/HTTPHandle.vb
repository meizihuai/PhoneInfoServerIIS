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
Imports System.Threading.Tasks
''' <summary>
'''本类用于处理HTTP Get和Post请求，主要API都在此，通过反射来获取API处理程序
''' </summary>
'''此版本由梅子怀最后更新，版本号V3.0.0，更新时间2019-03-11 15:35:00
Public Class HTTPHandle
    Structure DtInfo '测试组信息
        Dim province As String
        Dim city As String
        Dim groupId As String
        Dim line As String
        Dim name_cm As String
        Dim imei_cm As String
        Dim phone_cm As String
        Dim name_cu As String
        Dim imei_cu As String
        Dim phone_cu As String
        Dim name_ct As String
        Dim imei_ct As String
        Dim phone_ct As String
        Dim modified_datetime As String
        Dim modified_by As String
        Dim demo As String
    End Structure
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
    Structure LTECellInfo 'LTE工参
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
    Structure ProAndCity  '省份信息 包含市列表
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
    Structure cityInfo  '城市信息  包含区列表
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


    ''设置web.config  System.Web.Configuration.WebConfigurationManager.AppSettings.Set("name", "name2");
    ''读取web.config  string name = System.Web.Configuration.WebConfigurationManager.AppSettings["name"];
    '网络测试接口
    Public Function Handle_Test(ByVal context As HttpContext) As NormalResponse '测试
        Return New NormalResponse(True, "网络测试成功！这是返回处理信息", "这里返回错误信息", "这里返回数据")
    End Function
    '获取测试组信息
    Public Function Handle_GetDtGroup(ByVal context As HttpContext) As NormalResponse '获取所有测试组信息 ,如果groupId不为空，则读取该组信息
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim groupId As String = context.Request.QueryString("groupId")
            'Stepp = 1
            'If carrier = "" Then Return New NormalResponse(False, "必须选择运营商")
            'If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then Return New NormalResponse(False, "运营商错误")

            Dim sql As String = "select groupId,line,name_cm,imei_cm,phone_cm,name_cu,imei_cu,phone_cu,name_ct,imei_ct,phone_ct from Dt_Group "

            If groupId <> "" Then sql = sql & " where groupId='" & groupId & "'  order by modified_datetime desc"
            'If province <> "" Then sql = sql & " and province='" & province & "'"
            'If city <> "" Then sql = sql & " and city='" & city & "'"

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
            dt.Columns(0).ColumnName = "groupId"            ' dt.Columns(1).ColumnName = "carrier"
            dt.Columns(1).ColumnName = "line"
            dt.Columns(2).ColumnName = "name_cm"
            dt.Columns(3).ColumnName = "imei_cm"
            dt.Columns(4).ColumnName = "phone_cm"
            dt.Columns(5).ColumnName = "name_cu"
            dt.Columns(6).ColumnName = "imei_cu"
            dt.Columns(7).ColumnName = "phone_cu"
            dt.Columns(8).ColumnName = "name_ct"
            dt.Columns(9).ColumnName = "imei_ct"
            dt.Columns(10).ColumnName = "phone_ct"

            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetDtGroup Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function
    '上传新的测试组信息
    Public Function Handle_UploadDtGroup(context As HttpContext, data As Object, token As String) As NormalResponse '保存DTGroup
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Try
                Dim DtList As List(Of DtInfo) = JsonConvert.DeserializeObject(str, GetType(List(Of DtInfo)))
                If IsNothing(DtList) Then
                    Return New NormalResponse(False, "DtList is null")
                End If
                If DtList.Count = 0 Then
                    Return New NormalResponse(False, "DtList count =0")
                End If
                Dim colList() As String = GetOraTableColumns("Dt_Group")
                Dim dt As New DataTable
                For Each col In colList
                    If col <> "ID" Then
                        dt.Columns.Add(col)
                    End If
                Next
                Dim nowTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
                For Each itm In DtList

                    If itm.groupId = "" Then
                        Return New NormalResponse(False, "不允许空白groupId")
                    End If
                    If ORALocalhost.SqlIsIn("select * from dt_group where groupid='" & itm.groupId & "'") Then
                        Return New NormalResponse(False, "groupId '" & itm.groupId & "'已存在")
                    End If
                    Dim row As DataRow = dt.NewRow
                    row("province".ToUpper) = itm.province
                    row("city".ToUpper) = itm.city
                    row("groupId".ToUpper) = itm.groupId
                    row("line".ToUpper) = itm.line
                    row("name_cm".ToUpper) = itm.name_cm
                    row("imei_cm".ToUpper) = itm.imei_cm
                    row("phone_cm".ToUpper) = itm.phone_cm
                    row("name_cu".ToUpper) = itm.name_cu
                    row("imei_cu".ToUpper) = itm.imei_cu
                    row("phone_cu".ToUpper) = itm.phone_cu
                    row("name_ct".ToUpper) = itm.name_ct
                    row("imei_ct".ToUpper) = itm.imei_ct
                    row("phone_ct".ToUpper) = itm.phone_ct
                    row("modified_datetime".ToUpper) = nowTime
                    row("modified_by".ToUpper) = itm.modified_by
                    row("demo".ToUpper) = itm.demo
                    dt.Rows.Add(row)
                Next
                Dim result As String = ORALocalhost.SqlCMDListQuickByPara("Dt_Group", dt)
                If result = "success" Then 'true
                    Dim np As New NormalResponse(True, result)
                    Return np
                Else
                    Dim np As New NormalResponse(False, result)
                    Return np
                End If
            Catch ex As Exception
                Return New NormalResponse(False, ex.ToString)
            End Try
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    '删除测试组
    Public Function Handle_DeleteDtGroup(ByVal context As HttpContext) As NormalResponse '删除测试组
        Try
            Dim groupId As String = context.Request.QueryString("groupId")
            If IsNothing(groupId) Then Return New NormalResponse(False, "groupId不可为空")
            If groupId = "" Then Return New NormalResponse(False, "groupId不可为空")
            If ORALocalhost.SqlIsIn("select * from Dt_Group where groupId='" & groupId & "'") = False Then
                Return New NormalResponse(False, "该groupId不存在")
            End If
            Dim sql As String = "delete from Dt_Group where groupId='" & groupId & "'"
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, "删除成功！")
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function

    '更新测试组信息
    Public Function Handle_UpdateDtGroup(ByVal context As HttpContext) As NormalResponse '更新测试组
        Dim Stepp As Single = 0
        Try
            'CREATE TABLE Worklog(day varchar(50) not null,name varchar(50) not null, workContent varchar(2000) default '',issue varchar(200) default '',modifiedBy varchar(50) default '',ModifiedDate varchar(50) default '')
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim groupId As String = context.Request.QueryString("groupId")
            Dim line As String = context.Request.QueryString("line")
            Dim name_cm As String = context.Request.QueryString("name_cm")
            Dim imei_cm As String = context.Request.QueryString("imei_cm")
            Dim phone_cm As String = context.Request.QueryString("phone_cm")
            Dim name_cu As String = context.Request.QueryString("name_cu")
            Dim imei_cu As String = context.Request.QueryString("imei_cu")
            Dim phone_cu As String = context.Request.QueryString("phone_cu")
            Dim name_ct As String = context.Request.QueryString("name_ct")
            Dim imei_ct As String = context.Request.QueryString("imei_ct")
            Dim phone_ct As String = context.Request.QueryString("phone_ct")
            Dim modified_datetime As String = context.Request.QueryString("modified_datetime")
            Dim modified_by As String = context.Request.QueryString("modified_by")
            Dim demo As String = context.Request.QueryString("demo")


            Stepp = 1
            province = Trim(province) : groupId = Trim(groupId)
            If groupId = "" Then Return New NormalResponse(False, "必须输入groupId")
            'If day = "" Then Return New NormalResponse(False, "必须输入日期")
            'If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then Return New NormalResponse(False, "运营商错误")
            If ORALocalhost.SqlIsIn("select * from Dt_Group where groupId='" & groupId & "'") = False Then
                Return New NormalResponse(False, "该groupId不存在")
            End If

            Dim Cond As String = "", sql As String

            Stepp = 2
            If province <> "" Or True Then Cond = " province='" & province & "'"
            If city <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " city ='" & city & "'"
            If line <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " line ='" & line & "'"
            If name_cm <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " name_cm ='" & name_cm & "'"
            If imei_cm <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " imei_cm ='" & imei_cm & "'"
            If phone_cm <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " phone_cm ='" & phone_cm & "'"

            If name_cu <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " name_cu ='" & name_cu & "'"
            If imei_cu <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " imei_cu ='" & imei_cu & "'"
            If phone_cu <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " phone_cu ='" & phone_cu & "'"

            If name_ct <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " name_ct ='" & name_ct & "'"
            If imei_ct <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " imei_ct ='" & imei_ct & "'"
            If phone_ct <> "" Or True Then Cond = Cond & IIf(Cond = "", "", " , ") & " phone_ct ='" & phone_ct & "'"

            If Cond.Length = 0 Then
                Return New NormalResponse(False, "更新条件为空", groupId, "")
            End If
            sql = "update Dt_Group set " & Cond & " where groupID='" & groupId & "'"

            Stepp = 3
            Dim result As String = ORALocalhost.SqlCMD(sql)

            If result = "success" Then
                Return New NormalResponse(True, "更新 " & Cond & " 成功！")
            Else
                Return New NormalResponse(False, result, "", sql)
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            'dt.Columns(0).ColumnName = "project"            ' dt.Columns(1).ColumnName = "carrier"
            'dt.Columns(1).ColumnName = "workContent"
            'dt.Columns(2).ColumnName = "issue"


            'Stepp = 5
            Return New NormalResponse(True, "", "", "")
        Catch ex As Exception
            Return New NormalResponse(False, "更新测试组错误 Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function
    '上传铁塔工参
    Public Function Handle_UploadTtLteCellInfo(context As HttpContext, data As Object, token As String) As NormalResponse '保存铁塔Lte工参
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
                Dim colList() As String = GetOraTableColumns("TtLte_CellInfo_Table")
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
                Dim result As String = ORALocalhost.SqlCMDListQuickByPara("TtLte_CellInfo_Table", dt)
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
            Return New NormalResponse(False, "Upload TtLteCellInfo Err Step=" & Stepp & " " & ex.ToString)
        End Try
    End Function
    '上传LTE工参
    Public Function Handle_UploadLteCellInfo(context As HttpContext, data As Object, token As String) As NormalResponse '保存运营商Lte工参
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
                Dim colList() As String = GetOraTableColumns("Lte_CellInfo_Table")
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

                Dim result As String = ORALocalhost.SqlCMDListQuickByPara("Lte_CellInfo_Table", dt)
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
            Return New NormalResponse(False, "UploadLTECellInfoErr,Step=" & Stepp & " Err=" & ex.ToString)
        End Try
    End Function
    Structure LocalTestInfo '离线数据上传信息
        Public id As Integer
        Public time As String
        Public json As String
        Public type As String
    End Structure
    '上传离线数据
    Public Function Handle_UploadofflineDatas(context As HttpContext, data As Object, token As String) As NormalResponse  ''处理离线上传包
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
                        np = Handle_UploadPhoneInfo(context, pi, token)
                    End If
                    If func = "UploadQoEVideoInfo" Then 'Video
                        Dim qoe As QoEVideoInfo = JsonConvert.DeserializeObject(jdata, GetType(QoEVideoInfo))
                        If IsNothing(qoe) Then Return New NormalResponse(False, "QoEVideoInfo格式非法")
                        qoe.ISUPLOADDATATIMELY = -1
                        qoe.DATETIME = time
                        np = Handle_UploadQoEVideoInfo(context, qoe, token)
                    End If
                    If func = "UploadQoEHTTPInfo" Then 'HTTP
                        Dim qoe As QoEHTTPInfo = JsonConvert.DeserializeObject(jdata, GetType(QoEHTTPInfo))
                        If IsNothing(qoe) Then Return New NormalResponse(False, "QoEHTTPInfo格式非法")
                        qoe.ISUPLOADDATATIMELY = -1
                        qoe.DATETIME = time
                        np = Handle_UploadQoEHTTPInfo(context, qoe, token)
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
    'Android app上传QOER数据 
    Public Function Handle_UploadPhoneInfo(context As HttpContext, data As Object, token As String) As NormalResponse
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
        row("RID") = pi.RID
        row("DateTime".ToUpper) = pi.DATETIME
        row("Day".ToUpper) = dateTime.ToString("yyyy-MM-dd")
        row("ISUPLOADDATATIMELY") = pi.ISUPLOADDATATIMELY
        If IsNothing(pi.businessType) Then
            pi.businessType = ""
        End If
        If pi.businessType = "" Then
            pi.businessType = "Report"
        End If
        If pi.apkName = "" Then
            pi.apkName = "UniQoE"
        End If
        If pi.ADJ_SIGNAL.Length > 4000 Then
            ' qoe.ADJ_SIGNAL = qoe.ADJ_SIGNAL.Substring(0, 4000)
            pi.ADJ_SIGNAL = ""
        End If
        row("MNC".ToUpper) = pi.MNC
        row("wifi_SSID".ToUpper) = pi.wifi_SSID
        row("wifi_MAC".ToUpper) = pi.wifi_MAC
        row("PING_AVG_RTT".ToUpper) = pi.PING_AVG_RTT

        row("FREQ".ToUpper) = pi.FREQ
        row("cpu".ToUpper) = pi.cpu
        row("ADJ_SIGNAL".ToUpper) = pi.ADJ_SIGNAL

        row("isScreenOn".ToUpper) = pi.isScreenOn
        row("isGPSOpen".ToUpper) = pi.isGPSOpen

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
            pi.bdlon = c.x
            pi.bdlat = c.x
            c = GPS2GDS(pi.lon, pi.lat)
            row("GDLON".ToUpper) = c.x
            row("GDLAT".ToUpper) = c.y
            pi.gdlon = c.x
            pi.gdlat = c.x
            If True Then
                Dim la As LocationAddressInfo = GetAddressByLngLat(pi.lon, pi.lat)
                If IsNothing(la) = False Then
                    row("PROVINCE".ToUpper) = la.Province
                    row("CITY".ToUpper) = la.City
                    row("DISTRICT".ToUpper) = la.District
                    row("ADDRESS".ToUpper) = la.DetailAddress
                    HandleProAndCity(la.Province, la.City, la.District)
                    pi.province = la.Province
                    pi.city = la.City
                    pi.district = la.District
                    pi.DetailAddress = la.DetailAddress
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
        row("enodebId_cellId".ToUpper) = pi.eNodeBId & "_" & pi.cellId
        row("PHONE_SCREEN_BRIGHTNESS".ToUpper) = pi.PHONE_SCREEN_BRIGHTNESS
        row("EARFCN".ToUpper) = pi.EARFCN
        row("PHONE_ELECTRIC".ToUpper) = pi.PHONE_ELECTRIC

        If pi.HTTP_URL <> "" Then
            pi.VMOS = GetQoEHttpResponseScore(pi.HTTP_RESPONSE_TIME)
        End If
        row("VMOS") = pi.VMOS
        row("HTTP_RESPONSE_TIME") = pi.HTTP_RESPONSE_TIME
        row("HTTP_URL") = pi.HTTP_URL
        row("HTTP_BUFFERSIZE") = pi.HTTP_BUFFERSIZE


        If IsNothing(pi.neighbourList) = False Then
            Dim nb As List(Of Neighbour) = pi.neighbourList
            If nb.Count > 0 Then
                Dim neighbor As Neighbour = nb(0)
                row("Adj_PCI1".ToUpper) = neighbor.PCI
                row("Adj_RSRP1".ToUpper) = neighbor.RSRP
                row("Adj_EARFCN1".ToUpper) = neighbor.EARFCN
            End If
        End If
        If IsNothing(pi.xyZaSpeed) = False Then
            Dim json As String = JsonConvert.SerializeObject(pi.xyZaSpeed)
            row("xyZaSpeed".ToUpper) = json
        End If
        Try
            '修改设备表 设备的经纬度等参数信息
            Dim th As New Thread(Sub()
                                     DeviceHelper.ChangeDeviceStatus(pi)
                                 End Sub)
            th.Start()
        Catch ex As Exception

        End Try
        Try
            '修改任务表 任务最后数据时间等，任务监控模块告警生成
            Dim th As New Thread(Sub()
                                     MissionDog.OnDeviceDataCome(pi)
                                 End Sub)
            th.Start()
        Catch ex As Exception

        End Try

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
    'Android app上传QOE_VIDEO数据 
    Public Function Handle_UploadQoEVideoInfo(context As HttpContext, data As Object, token As String) As NormalResponse 'Android 上传QoEVideo
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
        If qoe.VIDEO_BUFFER_TOTAL_TIME = 0 Then Return New NormalResponse(False, "数据采集有问题，VIDEO_BUFFER_TOTAL_TIME=0")
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
        row("isScreenOn".ToUpper) = qoe.isScreenOn
        row("SCREENRECORD_FILENAME".ToUpper) = qoe.SCREENRECORD_FILENAME
        row("isscreenrecorduploaded".ToUpper) = 0

        row("ISUPLOADDATATIMELY") = qoe.ISUPLOADDATATIMELY
        row("DATETIME".ToUpper) = qoe.DATETIME
        row("NET_TYPE".ToUpper) = qoe.NET_TYPE
        row("BUSINESS_TYPE".ToUpper) = qoe.BUSINESS_TYPE
        row("VIDEO_BUFFER_INIT_TIME".ToUpper) = qoe.VIDEO_BUFFER_INIT_TIME
        row("VIDEO_BUFFER_TOTAL_TIME".ToUpper) = qoe.VIDEO_BUFFER_TOTAL_TIME

        row("VIDEO_LOAD_SCORE".ToUpper) = qoe.VIDEO_LOAD_SCORE
        row("VIDEO_STALL_SCORE".ToUpper) = qoe.VIDEO_STALL_SCORE
        row("VIDEO_BUFFER_TOTAL_SCORE".ToUpper) = qoe.VIDEO_BUFFER_TOTAL_SCORE
        ' row("USER_SCORE".ToUpper) = qoe.USER_SCORE
        row("ECLATIRY".ToUpper) = qoe.ECLATIRY
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

        row("SINR".ToUpper) = qoe.pi.SINR
        row("PHONE_MODEL".ToUpper) = qoe.PHONE_MODEL
        row("OPERATING_SYSTEM".ToUpper) = qoe.OPERATING_SYSTEM
        row("UDID".ToUpper) = qoe.UDID
        row("IMEI".ToUpper) = qoe.IMEI
        row("IMSI".ToUpper) = qoe.IMSI
        row("USER_TEL".ToUpper) = qoe.USER_TEL
        row("PHONE_PLACE_STATE".ToUpper) = qoe.PHONE_PLACE_STATE
        row("COUNTRY".ToUpper) = qoe.COUNTRY
        row("PROVINCE".ToUpper) = qoe.PROVINCE
        If qoe.CITY = "" Then
            Try
                Dim dtGroupId As String = QoEVideoDtGroupMember.GetGroupIdByImsi(qoe.IMSI)
                Dim qoeDtGroup As QoEVideoDtGroup = QoEVideoDtGroup.Get(0, "groupId='" & dtGroupId & "'")
                If IsNothing(qoeDtGroup) = False Then
                    qoe.CITY = qoeDtGroup.city
                End If
            Catch ex As Exception

            End Try
        End If
        row("CITY".ToUpper) = qoe.CITY
        row("ADDRESS".ToUpper) = qoe.ADDRESS
        row("PHONE_ELECTRIC_START".ToUpper) = qoe.PHONE_ELECTRIC_START
        row("PHONE_ELECTRIC_END".ToUpper) = qoe.PHONE_ELECTRIC_END
        row("SCREEN_RESOLUTION_LONG".ToUpper) = qoe.SCREEN_RESOLUTION_LONG
        row("SCREEN_RESOLUTION_WIDTH".ToUpper) = qoe.SCREEN_RESOLUTION_WIDTH
        Dim LIGHT_INTENSITY As String = ""
        If IsNothing(qoe.LIGHT_INTENSITY_list) = False Then
            LIGHT_INTENSITY = JsonConvert.SerializeObject(qoe.LIGHT_INTENSITY_list)
        End If
        row("LIGHT_INTENSITY".ToUpper) = LIGHT_INTENSITY
        Dim PHONE_SCREEN_BRIGHTNESS As String = ""
        If IsNothing(qoe.PHONE_SCREEN_BRIGHTNESS_list) = False Then
            PHONE_SCREEN_BRIGHTNESS = JsonConvert.SerializeObject(qoe.PHONE_SCREEN_BRIGHTNESS_list)
        End If
        row("PHONE_SCREEN_BRIGHTNESS".ToUpper) = PHONE_SCREEN_BRIGHTNESS
        row("HTTP_RESPONSE_TIME".ToUpper) = qoe.HTTP_RESPONSE_TIME
        row("PING_AVG_RTT".ToUpper) = qoe.PING_AVG_RTT
        qoe.VIDEO_CLARITY = ORALocalhost.SQLInfo("select VIDEO_CLARITY from QOE_VIDEO_SOURCE where url='" & qoe.FILE_NAME & "'")
        qoe.VIDEO_CLARITY = qoe.VIDEO_CLARITY.Replace("P", "").Replace("p", "")
        row("VIDEO_CLARITY".ToUpper) = qoe.VIDEO_CLARITY
        If qoe.VIDEO_CODING_FORMAT = "" Then qoe.VIDEO_CODING_FORMAT = "H.264"
        row("VIDEO_CODING_FORMAT".ToUpper) = qoe.VIDEO_CODING_FORMAT
        row("VIDEO_BITRATE".ToUpper) = qoe.VIDEO_BITRATE
        row("FPS".ToUpper) = qoe.FPS
        row("VIDEO_TOTAL_TIME".ToUpper) = qoe.VIDEO_TOTAL_TIME
        If qoe.VIDEO_PLAY_TOTAL_TIME = 0 Then qoe.VIDEO_PLAY_TOTAL_TIME = qoe.VIDEO_STALL_TOTAL_TIME + qoe.VIDEO_TOTAL_TIME
        row("VIDEO_PLAY_TOTAL_TIME".ToUpper) = qoe.VIDEO_PLAY_TOTAL_TIME
        row("ISGPSOPEN".ToUpper) = qoe.pi.isGPSOpen


        row("APP_PREPARED_TIME".ToUpper) = qoe.APP_PREPARED_TIME
        row("BVRATE".ToUpper) = qoe.BVRATE
        row("STARTTIME".ToUpper) = qoe.STARTTIME
        row("FILE_SIZE".ToUpper) = qoe.FILE_SIZE
        row("FILE_NAME".ToUpper) = qoe.FILE_NAME
        If qoe.FILE_SERVER_LOCATION = "" Then
            If qoe.FILE_SERVER_IP.StartsWith("221") Then
                qoe.FILE_SERVER_LOCATION = "研究院221服务器"
            Else
                qoe.FILE_SERVER_LOCATION = "移动111服务器"
            End If
        End If
        row("FILE_SERVER_LOCATION".ToUpper) = qoe.FILE_SERVER_LOCATION
        row("FILE_SERVER_IP".ToUpper) = qoe.FILE_SERVER_IP
        row("UE_INTERNAL_IP".ToUpper) = qoe.UE_INTERNAL_IP
        row("ENVIRONMENTAL_NOISE".ToUpper) = qoe.ENVIRONMENTAL_NOISE
        qoe.VIDEO_AVERAGE_PEAK_RATE = qoe.VIDEO_AVERAGE_PEAK_RATE * 8
        row("VIDEO_AVERAGE_PEAK_RATE".ToUpper) = qoe.VIDEO_AVERAGE_PEAK_RATE
        If IsNothing(qoe.CELL_SIGNAL_STRENGTHList) = False Then
            'Dim recordValue As Integer
            'For i = 0 To qoe.CELL_SIGNAL_STRENGTHList.Count - 1
            '    If qoe.CELL_SIGNAL_STRENGTHList(i) <> 0 Then
            '        recordValue = qoe.CELL_SIGNAL_STRENGTHList(i)
            '    Else
            '        qoe.CELL_SIGNAL_STRENGTHList(i) = recordValue
            '    End If
            'Next
            row("CELL_SIGNAL_STRENGTH".ToUpper) = JsonConvert.SerializeObject(qoe.CELL_SIGNAL_STRENGTHList)
        End If
        If IsNothing(qoe.ACCELEROMETER_DATAList) = False Then
            row("ACCELEROMETER_DATA".ToUpper) = JsonConvert.SerializeObject(qoe.ACCELEROMETER_DATAList)
        End If
        If IsNothing(qoe.INSTAN_DOWNLOAD_SPEEDList) = False Then
            For i = 0 To qoe.INSTAN_DOWNLOAD_SPEEDList.Count - 1
                qoe.INSTAN_DOWNLOAD_SPEEDList(i) = qoe.INSTAN_DOWNLOAD_SPEEDList(i) * 10 * 8
            Next
            row("INSTAN_DOWNLOAD_SPEED".ToUpper) = JsonConvert.SerializeObject(qoe.INSTAN_DOWNLOAD_SPEEDList)
        End If
        Dim isNeedSetPeakSpeed As Boolean = False
        If qoe.VIDEO_PEAK_DOWNLOAD_SPEED = 0 Then
            isNeedSetPeakSpeed = True
        Else
            qoe.VIDEO_PEAK_DOWNLOAD_SPEED = qoe.VIDEO_PEAK_DOWNLOAD_SPEED * 8
        End If
        Dim Video_All_Peak_Speed As Long = 0
        If IsNothing(qoe.INSTAN_DOWNLOAD_SPEEDList) = False Then
            Dim recordValue As Long
            For i = 0 To qoe.INSTAN_DOWNLOAD_SPEEDList.Count - 1
                If qoe.INSTAN_DOWNLOAD_SPEEDList(i) <> 0 Then
                    recordValue = qoe.INSTAN_DOWNLOAD_SPEEDList(i)
                End If
                If recordValue > Video_All_Peak_Speed Then
                    Video_All_Peak_Speed = recordValue
                End If
                If isNeedSetPeakSpeed Then
                    If recordValue > qoe.VIDEO_PEAK_DOWNLOAD_SPEED Then
                        qoe.VIDEO_PEAK_DOWNLOAD_SPEED = recordValue
                    End If
                End If
            Next
        End If
        row("VIDEO_ALL_PEAK_RATE".ToUpper) = Video_All_Peak_Speed
        row("VIDEO_PEAK_DOWNLOAD_SPEED".ToUpper) = qoe.VIDEO_PEAK_DOWNLOAD_SPEED
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
            row(("LONGITUDE_1").ToUpper) = qoe.pi.lon
            row(("LATITUDE_1").ToUpper) = qoe.pi.lat
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
        'If IsNothing(qoe.ADJList) = False Then
        '    Dim recordValue As New QoEVideoInfo.ADJInfo
        '    recordValue.ECI = 0
        '    recordValue.RSRP = 0
        '    For i = 0 To 5
        '        If qoe.ADJList.Count > i Then
        '            recordValue = qoe.ADJList(i)
        '        End If
        '        row(("ADJ_ECI" & i + 1).ToUpper) = recordValue.ECI
        '        row(("ADJ_RSRP" & i + 1).ToUpper) = recordValue.RSRP
        '    Next
        'End If

        If qoe.VIDEO_STALL_NUM > 0 Then
            If qoe.VIDEO_STALL_TOTAL_TIME = 0 Then qoe.VIDEO_STALL_TOTAL_TIME = 1000
        End If
        If IsNothing(qoe.STALLlist) Then
            qoe.STALLlist = New List(Of QoEVideoInfo.STALLInfo)
        End If
        If qoe.STALLlist.Count < qoe.VIDEO_STALL_NUM Then
            Dim totalStallTime As Long = qoe.VIDEO_STALL_TOTAL_TIME
            Dim d As Double = totalStallTime / qoe.VIDEO_STALL_NUM
            Dim perTotalTime As Long = d
            Dim point As Long = 0
            For i = qoe.STALLlist.Count To qoe.VIDEO_STALL_NUM - 1

                qoe.STALLlist.Add(New QoEVideoInfo.STALLInfo(point, perTotalTime))
                point = point + perTotalTime
            Next
        End If

        If IsNothing(qoe.STALLlist) = False Then
            Dim recordValue As New QoEVideoInfo.STALLInfo
            recordValue.TIME = 0
            recordValue.POINT = 0
            For i = 0 To 9
                If qoe.STALLlist.Count > i Then
                    recordValue = qoe.STALLlist(i)
                Else
                    recordValue.TIME = 0
                    recordValue.POINT = 0
                End If
                row(("STALL_DURATION_LONG_" & i + 1).ToUpper) = recordValue.TIME
                row(("STALL_DURATION_LONG_POINT_" & i + 1).ToUpper) = recordValue.POINT
            Next
        End If
        row("VIDEO_STALL_NUM".ToUpper) = qoe.VIDEO_STALL_NUM
        row("VIDEO_STALL_TOTAL_TIME".ToUpper) = qoe.VIDEO_STALL_TOTAL_TIME
        Dim tmpdValue As Double = qoe.VIDEO_STALL_TOTAL_TIME / qoe.VIDEO_BUFFER_TOTAL_TIME
        qoe.VIDEO_STALL_DURATION_PROPORTION = (tmpdValue * 100).ToString("0.00")
        row("VIDEO_STALL_DURATION_PROPORTION".ToUpper) = qoe.VIDEO_STALL_DURATION_PROPORTION

        Dim pi As PhoneInfo = qoe.pi
        If IsNothing(pi) = False Then

            If IsNothing(pi.neighbourList) = False Then
                If pi.neighbourList.Count > 0 Then
                    row("Adj_PCI1".ToUpper) = pi.neighbourList(0).PCI
                    row("Adj_RSRP1".ToUpper) = pi.neighbourList(0).RSRP
                    row("Adj_EARFCN1".ToUpper) = pi.neighbourList(0).EARFCN
                    For i = 0 To 5
                        If pi.neighbourList.Count > i + 1 Then
                            row(("ADJ_PCI" & i + 1).ToUpper) = pi.neighbourList(i).PCI
                            row(("ADJ_RSRP" & i + 1).ToUpper) = pi.neighbourList(i).RSRP
                        End If
                    Next
                End If
            End If
        End If
        Dim NETWORK_SET As String = ""
        If IsNothing(qoe.NETWORK_TYPEList) = False Then NETWORK_SET = JsonConvert.SerializeObject(qoe.NETWORK_TYPEList)
        row("NETWORK_SET".ToUpper) = NETWORK_SET
        row("USERSCENE".ToUpper) = qoe.USERSCENE
        row("MOVE_SPEED".ToUpper) = qoe.MOVE_SPEED
        qoe.ISPLAYCOMPLETED = 1
        row("ISPLAYCOMPLETED".ToUpper) = qoe.ISPLAYCOMPLETED
        row("LOCALDATASAVETIME".ToUpper) = qoe.LOCALDATASAVETIME
        row("ISUPLOADDATATIMELY".ToUpper) = qoe.ISUPLOADDATATIMELY
        row("TASKNAME".ToUpper) = qoe.TASKNAME
        ' qoe.RECFILE = qoe.SCREENRECORD_FILENAME
        ' row("RECFILE".ToUpper) = qoe.RECFILE
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
        Dim vmos_match As Double = 0
        If qoe.EVMOS <> 0 Then
            vmos_match = 100 * （1 - (Math.Abs(qoe.EVMOS - qoe.VMOS) / 4)）
        End If
        qoe.VMOS_MATCH = vmos_match.ToString("0.00")
        row("VMOS_MATCH".ToUpper) = qoe.VMOS_MATCH
        'row("GPSSPEED".ToUpper) = qoe.GPSSPEED
        'row("BUSINESSTYPE".ToUpper) = qoe.BUSINESSTYPE
        row("PCI".ToUpper) = qoe.pi.PCI
        row("APKNAME".ToUpper) = qoe.APKNAME
        dt.Rows.Add(row)
        Task.Run(Sub()
                     HandleUserBonusPoints(qoe)
                 End Sub)
        Task.Run(Sub()
                     QoEVideoDtGroupMember.OnUpdateQoEVideoInfo(qoe)
                 End Sub)
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
    Private Sub HandleUserBonusPoints(qoe As QoEVideoInfo)
        Try

            Dim vmos As Integer, evmos As Integer, imsi As String, imei As String, url As String
            vmos = qoe.VMOS
            evmos = qoe.EVMOS
            imsi = qoe.IMSI
            imei = qoe.IMEI
            url = qoe.FILE_NAME
            ' If (vmos = 0 Or evmos = 0) Then Return
            Dim bonusPoints As Single = 1
            Dim rewordPoints As Single = 1 - (Math.Abs(evmos - vmos)) / 4
            If (vmos = 0 Or evmos = 0) Then
                rewordPoints = 0
                bonusPoints = 0
            End If
            bonusPoints = bonusPoints + rewordPoints
            ' File.WriteAllText("d:\user_bp_table.txt", String.Format("evmos={0},vmos={1},rewordPoints={2},bonusPoints={3}", evmos, vmos, rewordPoints, bonusPoints))
            Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
            Dim sql As String = "select * from user_bp_table where imsi='" & imsi & "'"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            Dim isNeedAdd As Boolean = True
            If IsNothing(dt) = False Then
                If dt.Rows.Count > 0 Then
                    isNeedAdd = False
                    Dim oldPoints As String = dt.Rows(0)("bonusPoints".ToUpper()).ToString()
                    Dim videoUrls As String = dt.Rows(0)("videoUrls".ToUpper()).ToString()
                    Dim qoe_total_time As String = dt.Rows(0)("qoe_total_time".ToUpper()).ToString()
                    Dim qoe_total_E_time As String = dt.Rows(0)("qoe_total_E_time".ToUpper()).ToString()
                    Dim qoe_today_time As String = dt.Rows(0)("qoe_today_time".ToUpper()).ToString()
                    Dim qoe_today_E_time As String = dt.Rows(0)("qoe_today_E_time".ToUpper()).ToString()
                    Dim lastDateTime As String = dt.Rows(0)("lastDateTime".ToUpper()).ToString()
                    If IsNothing(qoe_total_time) = False AndAlso IsDBNull(qoe_total_time) = False Then
                        qoe_total_time = Val(qoe_total_time) + 1
                    Else
                        qoe_total_time = 1
                    End If
                    If qoe.EVMOS > 0 Then
                        If IsNothing(qoe_total_E_time) = False AndAlso IsDBNull(qoe_total_E_time) = False Then
                            qoe_total_E_time = Val(qoe_total_E_time) + 1
                        Else
                            qoe_total_E_time = 1
                        End If
                    End If

                    Dim isToday As Boolean = False
                    If IsNothing(lastDateTime) = False AndAlso IsDBNull(lastDateTime) = False Then
                        Dim lastDay As Date = Now
                        Date.TryParse(lastDateTime, lastDay)
                        If lastDay.ToString("yyyy-MM-dd") = Now.ToString("yyyy-MM-dd") Then
                            isToday = True
                        End If
                    End If
                    If isToday Then
                        If IsNothing(qoe_today_time) = False AndAlso IsDBNull(qoe_today_time) = False Then
                            qoe_today_time = Val(qoe_today_time) + 1
                        Else
                            qoe_today_time = 1
                        End If
                        If qoe.EVMOS > 0 Then
                            If IsNothing(qoe_today_E_time) = False AndAlso IsDBNull(qoe_today_E_time) = False Then
                                qoe_today_E_time = Val(qoe_today_E_time) + 1
                            Else
                                qoe_today_E_time = 1
                            End If
                        End If
                    Else
                        qoe_today_time = 1
                        If qoe.EVMOS > 0 Then
                            qoe_today_E_time = 1
                        End If
                    End If

                    Dim videoList As List(Of String)
                    Dim oldBp As Single = 0
                    If IsNothing(oldPoints) = False AndAlso IsDBNull(oldPoints) = False Then
                        oldBp = Val(oldPoints)
                    End If
                    If IsNothing(videoUrls) = False AndAlso IsDBNull(videoUrls) = False Then
                        Try
                            videoList = JsonConvert.DeserializeObject(Of List(Of String))(videoUrls)
                        Catch ex As Exception

                        End Try
                    End If
                    If IsNothing(videoList) Then videoList = New List(Of String)
                    videoList.Add(url)
                    Dim lastBpoints As String = bonusPoints.ToString("0.0")
                    bonusPoints = bonusPoints + oldBp
                    Dim str As String = bonusPoints.ToString("0.0")
                    sql = "update user_bp_table set lastDateTime='{0}',bonusPoints={1},imei='{2}',EVMOS={3},VMOS={4},LastBonusPoints={5},LastVideoUrl='{6}',VideoUrls='{7}',
                           QOE_TOTAL_time={8},QOE_TOTAL_E_time={9},QOE_TODAY_time={10},QOE_TODAY_E_time={11},lastday='{12}'   where imsi='" & imsi & "'"
                    videoUrls = JsonConvert.SerializeObject(videoList)
                    videoUrls = ""
                    sql = String.Format(sql, time, str, imei, evmos, vmos, lastBpoints, url, videoUrls,
                                         qoe_total_time, qoe_total_E_time, qoe_today_time, qoe_today_E_time, time.Substring(0, 10))
                End If
            End If
            If isNeedAdd Then
                sql = "insert into user_bp_table(dateTime,imei,imsi,lastDateTime,bonusPoints,EVMOS,VMOS,LastBonusPoints,LastVideoUrl,VideoUrls,QOE_TOTAL_time,QOE_TOTAL_E_time,QOE_TODAY_time,QOE_TODAY_E_time,lastDay)
                       values('{0}','{1}','{2}','{3}',{4},{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}')"
                Dim str As String = bonusPoints.ToString("0.0")
                Dim list As New List(Of String)
                list.Add(url)
                Dim videoUrls As String = JsonConvert.SerializeObject(list)
                videoUrls = ""
                sql = String.Format(sql, time, imei, imsi, time, str, evmos, vmos, str, url, videoUrls,
                                    1, IIf(qoe.EVMOS > 0, 1, 0), 1, IIf(qoe.EVMOS > 0, 1, 0), time.Substring(0, 10))
            End If
            Dim result As String = ORALocalhost.SqlCMD(sql)
            ' File.WriteAllText("d:\user_bp_table.txt", Now.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf & sql & vbCrLf & result)
        Catch ex As Exception
            ' File.WriteAllText("d:\user_bp_table.txt", ex.ToString())
        End Try
    End Sub

    Private Function HandleQoEVideoInfo(qoe As QoEVideoInfo) As QoEVideoInfo
        If qoe.ISUPLOADDATATIMELY = -1 Then
            '离线数据
            qoe.ISUPLOADDATATIMELY = 0
            If qoe.DATETIME = "" Then qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
            qoe.LOCALDATASAVETIME = qoe.DATETIME
        Else
            '实时数据
            qoe.ISUPLOADDATATIMELY = 1
            qoe.DATETIME = Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If

        If IsNothing(qoe.BUSINESSTYPE) Then qoe.BUSINESSTYPE = ""
        If qoe.BUSINESSTYPE = "" Then qoe.BUSINESSTYPE = "QoEVideo"
        qoe.BUSINESS_TYPE = qoe.BUSINESSTYPE
        Dim npi As PhoneInfo = qoe.pi
        If IsNothing(npi) = False Then
            Dim lon As Double = qoe.pi.lon
            Dim lat As Double = qoe.pi.lat
            Dim bdlon, bdlat, gdlon, gdlat As Double
            If lon > 0 And lat > 0 Then
                'Dim c As CoordInfo = GPS2BDS(lon, lat)
                'bdlon = c.x
                'bdlat = c.y
                Dim c As CoordInfo = GPS2GDS(lon, lat)
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
            If qoe.ADJ_SIGNAL.Length > 4000 Then
                ' qoe.ADJ_SIGNAL = qoe.ADJ_SIGNAL.Substring(0, 4000)
                qoe.ADJ_SIGNAL = ""
            End If
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


            qoe.ECI = npi.CI
            qoe.MCC = "460"
            qoe.PLMN = qoe.MCC & qoe.MNC
        End If
        If qoe.SATELLITECOUNT >= 4 Then
            qoe.ISOUTSIDE = 1
        Else
            qoe.ISOUTSIDE = 0
        End If
        If qoe.VIDEO_TOTAL_TIME > 0 Then qoe.VIDEO_STALL_DURATION_PROPORTION = 100 * qoe.VIDEO_STALL_TOTAL_TIME / qoe.VIDEO_TOTAL_TIME
        If qoe.VIDEO_TOTAL_TIME > 0 Then qoe.BVRATE = 100 * qoe.VIDEO_BUFFER_TOTAL_TIME / qoe.VIDEO_TOTAL_TIME
        Dim Video_LOAD_Score As Single = GetVideo_LOAD_Score(qoe.VIDEO_BUFFER_INIT_TIME, qoe.VIDEO_BUFFER_TOTAL_TIME, qoe.VIDEO_TOTAL_TIME)
        Dim Video_STALL_Score As Single = GetVideo_STALL_Score(qoe.VIDEO_STALL_TOTAL_TIME, qoe.VIDEO_STALL_NUM, qoe.VIDEO_TOTAL_TIME)

        qoe.VIDEO_BUFFER_TOTAL_SCORE = GetVideo_Buffer_Total_Score(qoe.BVRATE)
        qoe.VIDEO_LOAD_SCORE = Video_LOAD_Score
        qoe.VIDEO_STALL_SCORE = Video_STALL_Score
        qoe.VMOS = GetVMOS(Video_LOAD_Score, Video_STALL_Score)
        Return qoe
    End Function
    'Android app上传QOE_HTTP数据 
    Public Function Handle_UploadQoEHTTPInfo(ByVal context As HttpContext, ByVal data As Object, token As String) As NormalResponse ''上传QOE HTTP Table
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

        row("EVMOS") = qoe.EVMOS
        row("ERESPONSETIMESCORE") = qoe.ERESPONSETIMESCORE
        row("ETOTALBUFFERTIMESCORE") = qoe.ETOTALBUFFERTIMESCORE
        row("EWHITESCREENTIMESCORE") = qoe.EWHITESCREENTIMESCORE
        row("HTTPURL") = qoe.HTTPURL

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
            If pi.ADJ_SIGNAL.Length > 4000 Then
                ' qoe.ADJ_SIGNAL = qoe.ADJ_SIGNAL.Substring(0, 4000)
                pi.ADJ_SIGNAL = ""
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
                'Dim c As CoordInfo = GPS2BDS(pi.lon, pi.lat)
                'row("BDLON".ToUpper) = c.x
                'row("BDLAT".ToUpper) = c.y
                Dim c As CoordInfo = GPS2GDS(pi.lon, pi.lat)
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

    '视频加载评分
    Private Function GetVideo_LOAD_Score(iniTime As Long, bufferTotalTime As Long, totalTime As Long) As Single
        Dim iniScore As Single = 1
        If iniTime <= 1000 Then iniScore = 5
        If 1000 < iniTime And iniTime <= 2000 Then iniScore = 4
        If iniTime < 2000 And iniTime <= 3000 Then iniScore = 3
        If iniTime < 3000 And iniTime <= 5000 Then iniScore = 2
        If iniTime > 5000 Then iniScore = 1

        Dim bvRate As Double = 100 * bufferTotalTime / totalTime
        Dim bvRateScore As Integer = GetVideo_Buffer_Total_Score(bvRate)
        Dim score As Integer = bvRateScore * 0.5 + iniScore * 0.5
        Return Math.Floor(score)
    End Function
    '视频卡顿评分
    Private Function GetVideo_STALL_Score(stallTime As Long, stallCount As Integer, totalTime As Long) As Integer
        Dim Score As Single = 1
        Dim p As Double = 100 * stallTime / totalTime
        If p = 0 Then Score = 5
        If 0 < p And p <= 0.1 Then Score = 4
        If 0.1 < p And p <= 1 Then Score = 3
        If 1 < p And p <= 5 Then Score = 2
        If 5 < p Then Score = 1

        If stallCount = 0 Then Score = (Score + 5) / 2
        If stallCount = 1 Then Score = (Score + 4) / 2
        If stallCount = 2 Then Score = (Score + 3) / 2
        If stallCount = 3 Then Score = (Score + 2) / 2
        If stallCount > 3 Then Score = (Score + 1) / 2

        Return Math.Round(Score)
    End Function
    '缓存评分
    Private Function GetVideo_Buffer_Total_Score(BVRate As Double) As Integer
        If BVRate <= 10 Then Return 5
        If BVRate <= 30 Then Return 4
        If BVRate <= 50 Then Return 3
        If BVRate <= 70 Then Return 2
        Return 1
    End Function
    'VMOS 总评分
    Private Function GetVMOS(ByVal Video_LOAD_Score As Integer, ByVal Video_STALL_Score As Integer) As Single
        Dim d As Double = 0.5 * Video_LOAD_Score + 0.5 * Video_STALL_Score
        'Dim i As Single = Math.Ceiling(d)
        Return Math.Round(d, 2)
    End Function
    'HTTP响应时间的评分
    Private Function GetQoEHttpResponseScore(responseRime As Long) As Integer
        If (responseRime > 5000) Then Return 1
        If (responseRime > 3000) Then Return 2
        If (responseRime > 2000) Then Return 3
        If (responseRime > 500) Then Return 4

        Return 5
    End Function
    'ping评分
    Private Function GetQoerPingScore(time As Single) As Integer
        If (time > 1000) Then Return 1
        If (time > 500) Then Return 2
        If (time > 200) Then Return 3
        If (time > 50) Then Return 4
        Return 5
    End Function
    '获取QOER所有入网设备信息
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
    '获取某个设备最新的测试数据点
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
    '获取QoEHTTP业务数据
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
            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,eNodeBId,CellId,Grid,VMOS,imei from QOE_HTTP_TABLE "
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

    '获取QoE视频业务数据
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
            Dim imei As String = context.Request.QueryString("imei")
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
            Dim sql As String = "select datetime,province,city,district,net_Type,GDlon,GDlat,SIGNAL_STRENGTH,SINR,eNodeBId,CellId,Grid," &
                                "VMOS,screenrecord_filename,isscreenrecorduploaded,imei from Qoe_Video_TABLE "
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

            If imei <> "" Then
                If doHaveWhere Then
                    sql = sql & " and imei='" & imei & "'"
                Else
                    sql = sql & " where imei='" & imei & "'"
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
            If imei <> "" Then

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

            For Each row In dt.Rows
                Dim screenrecord_filename As String = row("screenrecord_filename".ToUpper).ToString
                Dim isscreenrecorduploaded As String = row("isscreenrecorduploaded".ToUpper).ToString
                If IsNothing(screenrecord_filename) = False And IsNothing(isscreenrecorduploaded) = False Then
                    If IsDBNull(screenrecord_filename) = False And IsDBNull(isscreenrecorduploaded) = False Then
                        screenrecord_filename = myServerUrl & "/ScreenRecordFiles/" & screenrecord_filename
                        If isscreenrecorduploaded = "1" Then
                            row("screenrecord_filename".ToUpper) = screenrecord_filename
                        Else
                            row("screenrecord_filename".ToUpper) = ""
                        End If
                    End If
                End If
            Next
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetQoePointErr:" & ex.Message & ",Step=" & Stepp)
        End Try

    End Function
    '获取QoER的省市区结构
    Public Function Handle_GetQoeReportProAndCity(ByVal context As HttpContext) As NormalResponse '获取SDK中运营商的省、市、区信息
        Try
            Dim sb As New StringBuilder
            Dim startTime As Date = Now
            Dim sql As String = "select province,city,district from proAndcityTable order by province asc"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            sb.AppendLine("查询数据库:" & GetTimeSpan(startTime))
            startTime = Now
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
            sb.AppendLine("归类计算:" & GetTimeSpan(startTime))
            startTime = Now
            Return New NormalResponse(True, sb.ToString, "", list)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    '获得QoeReport数据
    Public Function Handle_GetQoeReportRSRPPoint(ByVal context As HttpContext) As NormalResponse '获得QoeReport数据  Old Name :Handle_GetSmartPlanRSRPPoint      New： 
        Dim Stepp As Single = 0
        Try
            Dim handleStartTime As Date = Now
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
            If carrier = "" Then Return New NormalResponse(False, "必须选择运营商")
            If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then Return New NormalResponse(False, "运营商错误")

            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,QoeR,eNodeBId,CellId,Grid,ENODEBID_CELLID,isOutSide,isGpsOpen,imei from QOE_REPORT_TABLE "

            sql = sql & " where carrier='" & carrier & "'"
            If province <> "" Then sql = sql & " and province='" & province & "'"
            If city <> "" Then sql = sql & " and city='" & city & "'"
            If district <> "" Then sql = sql & " and district='" & district & "'"
            If netType <> "" Then sql = sql & " and netType='" & netType & "'"
            If grid <> "" Then sql = sql & " and grid='" & grid & "'"

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"

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
            Dim sqlCheckStartTime As Date = Now
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            Dim sqlCheckTime As String = GetTimeSpan(sqlCheckStartTime)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' "select province,city,district,netType,GDlon,GDlat,RSRP,time,SINR,eNodeBId,CellId from SDKTABLE"
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"            ' dt.Columns(1).ColumnName = "carrier"
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
            dt.Columns(13).ColumnName = "eNodeBid_Cellid"
            'Stepp = 5
            Dim handleCheckTime As String = GetTimeSpan(handleStartTime)
            Return New NormalResponse(True, "数据库耗时:" & sqlCheckTime & ",后台处理总耗时:" & handleCheckTime, "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetQoeR RSRPPointErr:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function


    '获取UEQoEReport数据
    Public Function Handle_GetUeQoeReportRSRPPoint(ByVal context As HttpContext) As NormalResponse '获得用户QoeReport数据 
        Dim Stepp As Single = 0
        Try

            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim province As String = context.Request.QueryString("province")
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim imei As String = context.Request.QueryString("imei")
            Dim netType As String = context.Request.QueryString("netType")
            Dim grid As String = context.Request.QueryString("grid")
            Dim startTime As String = context.Request.QueryString("startTime")
            Dim endTime As String = context.Request.QueryString("endTime")
            Dim readIndex As Integer = context.Request.QueryString("readIndex")
            Dim readCount As Integer = context.Request.QueryString("readCount")
            'Stepp = 1

            If imei = "" Then Return New NormalResponse(False, "必须选择imei")
            '  If carrier = "" Then Return New NormalResponse(False, "必须选择运营商")

            ' If carrier <> "中国移动" And carrier <> "中国联通" And carrier <> "中国电信" Then Return New NormalResponse(False, "运营商错误")

            Dim sql As String = "select datetime,province,city,district,netType,GDlon,GDlat,RSRP,SINR,QoeR,eNodeBId,CellId,imei from QOE_REPORT_TABLE "

            sql = sql & " where imei='" & imei & "'"
            If carrier <> "" Then sql = sql & " and carrier='" & carrier & "'"

            If netType <> "" Then sql = sql & " and netType='" & netType & "'"
            If grid <> "" Then sql = sql & " and grid='" & grid & "'"
            If province <> "" Then sql = sql & " and province='" & province & "'"
            If city <> "" Then sql = sql & " and city='" & city & "'"
            If district <> "" Then sql = sql & " and district='" & district & "'"

            If startTime <> "" Then
                If endTime <> "" Then
                    Try
                        Dim d As Date = Date.Parse(startTime)
                        startTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        d = Date.Parse(endTime)
                        endTime = d.ToString("yyyy-MM-dd HH:mm:ss")
                        sql = sql & " and datetime>='" & startTime & "' and datetime<='" & endTime & "'"

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
            'Carrier,province,city,district,netType,GDlon,GDlat,RSRP,time,SINR
            dt.Columns(0).ColumnName = "datetime"            ' dt.Columns(1).ColumnName = "carrier"
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
            'dt.Columns(12).ColumnName = "Grid"
            'Stepp = 5
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, "GetUeQoeReportRSRPPoint Err:" & ex.Message & ",Step=" & Stepp)
        End Try
    End Function
    '获取铁塔工参
    Public Function Handle_GetTtLteCellInfo(ByVal context As HttpContext) As NormalResponse '获得运营商4G小区工参
        Dim Stepp As Integer = 0
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'中国联通','中国移动','中国电信'}
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim siteType As String = context.Request.QueryString("siteType")

            Stepp = 1

            If city = "" Then Return New NormalResponse(False, "必须选择城市")
            If carrier <> "移动" And carrier <> "联通" And carrier <> "电信" Then Return New NormalResponse(False, "运营商错误{移动,联通,电信}")
            '' Return New NormalResponse(False, "必须选择省份")carrier,
            'End If
            Dim sql As String = "select carrier,district,enodebName,enodebName_Tt,state,gdLon,gdLat,siteType,h from TtLte_CellInfo_Table "
            sql = sql & " where city='" & city & "'"
            sql = sql & " and instr(carrier,'" & carrier & "')>0 "
            If district <> "" Then sql = sql & " and district='" & district & "'"
            If siteType <> "" Then sql = sql & " and siteType='" & siteType & "'"

            'If IsNothing(readIndex) = False Then
            '    If readIndex = 0 And readCount = 0 Then
            '    Else
            '        If readIndex >= 0 Then
            '            If readCount <= 0 Then
            '                Return New NormalResponse(False, "readCount必须为正整数")
            '            End If
            '            sql = OracleSelectPage(sql, readIndex, readCount)
            '            '  sql = sql & "  limit " & readCount & " offset " & readCount
            '        End If
            '    End If
            'End If
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


    '获取LTE工参
    Public Function Handle_GetLteCellInfo(ByVal context As HttpContext) As NormalResponse '获得运营商4G小区工参
        Dim Stepp As Integer = 0
        Try
            Dim carrier As String = context.Request.QueryString("carrier") '运营商 {'联通','移动','电信'}
            Dim city As String = context.Request.QueryString("city")
            Dim district As String = context.Request.QueryString("district")
            Dim siteType As String = context.Request.QueryString("siteType")
            'Dim grid As String = context.Request.QueryString("grid")
            'Dim readIndex As Integer = context.Request.QueryString("readIndex")
            'Dim readCount As Integer = context.Request.QueryString("readCount")
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
            Dim sql As String = "select district,enodebName,ecellName,eNodeBId,cellId,gdLon,gdLat,siteType,h,amzimuth,Tac,freq,pci from Lte_CellInfo_Table "

            sql = sql & " where carrier='" & carrier & "'"
            sql = sql & " and city='" & city & "'"

            If district <> "" Then sql = sql & " and district='" & district & "'"
            If siteType <> "" Then sql = sql & " and siteType='" & siteType & "'"

            'If IsNothing(readIndex) = False Then
            '    If readIndex = 0 And readCount = 0 Then
            '    Else
            '        If readIndex >= 0 Then
            '            If readCount <= 0 Then
            '                Return New NormalResponse(False, "readCount必须为正整数")
            '            End If
            '            sql = OracleSelectPage(sql, readIndex, readCount)
            '            '  sql = sql & "  limit " & readCount & " offset " & readCount
            '        End If
            '    End If
            'End If
            Stepp = 3
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何数据", sql, "")
            End If
            Stepp = 4
            ' district,enodebName,ecellName,eNodeBId,cellId,gdLon,gdLat,siteType,h,amzimuth,Tac,freq,pci from Lte_CellInfo_Table "

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
    '获取MR网格列表
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

    '获取MR数据表的省市区结构
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
    '获取MR数据
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
            'If district = "" Then district = "化州"
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
            Dim workMsg As String = "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpan(workStartTime) & ",  apiVersion:" & apiVersion
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
    '获取某基站的性能数据
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
    'oracle分页封装
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
    Structure RunSQLInfo  '运行SQL结构信息
        Dim connStr As String
        Dim sqllist As List(Of String)
    End Structure
    '解压数据
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
    '前端执行SQL语句，！！！仅用于调试开发阶段使用！！！
    Public Function Handle_RunSQL(ByVal context As HttpContext, ByVal data As Object, token As String) As NormalResponse '按Sql来查询
        Try
            If IsNothing(data) Then Return New NormalResponse(False, "sql is null")

            Dim str As String = data.ToString
            If str = "" Then Return New NormalResponse(False, "sql is null")
            str = str.TrimStart(" ")
            Dim isSelect As Boolean = False
            Dim order As String = str.Split(" ")(0)
            If order = "" Then Return New NormalResponse(False, "sql order is null")
            order = order.ToLower
            If order = "select" Then
                isSelect = True
            End If
            Dim errList As New List(Of String)
            errList.Add("delete")
            errList.Add("insert")
            errList.Add("truncate")
            errList.Add("drop")
            errList.Add("alter")
            errList.Add("create")
            errList.Add("--")
            errList.Add("#")
            For Each itm In errList
                If order.ToLower.Contains(itm) Then
                    isSelect = False
                    Exit For
                End If
            Next
            Dim sql As String = str
            Dim logResult As String = RecordRunSqlLog(token, order, sql, isSelect)
            If isSelect Then
                Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
                If IsNothing(dt) Then Return New NormalResponse(True, logResult, "", "[]")
                If dt.Rows.Count = 0 Then Return New NormalResponse(True, logResult, "", "[]")
                Return New NormalResponse(True, logResult, "", dt)
            Else
                Return New NormalResponse(False, "不允许执行该语句", logResult, "")
                'Dim result As String = ORALocalhost.SqlCMD(sql)
                'Return New NormalResponse(True, "", "", result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    Private Function RecordRunSqlLog(token As String, SQLfunc As String, SQLContent As String, runResult As Boolean) As String
        Dim y As String = Chr(34)
        SQLfunc = SQLfunc.Replace("'", y).Replace("--", "##")
        SQLContent = SQLContent.Replace("'", y).Replace("--", "##")
        Dim usr As String = GetUsrByToken(token)
        Dim dateTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "insert into sys_runSql_log (dateTime,userName,token,SqlFunc,SqlContent,runResult) values ('{0}','{1}','{2}','{3}','{4}',{5})"
        sql = String.Format(sql, New String() {dateTime, usr, token, SQLfunc, SQLContent, IIf(runResult, 1, 0)})
        Return ORALocalhost.SqlCMD(sql)
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
    '上传MR原文件
    Public Function Handle_UploadMRFile(ByVal context As HttpContext, ByVal data As Object, token As String) As NormalResponse '上传MR文件
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
                Return New NormalResponse(True, "录入成功，总用时:" & GetTimeSpan(startTime), np.errmsg, "")
            Else
                Return np
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    '处理MR XML数据
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
            Return New NormalResponse(True, "", "行数:" & rowCount & ",入库用时:" & GetTimeSpan(startTimeTmp), "")
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
    '上传MR数据
    Public Function Handle_UploadMRData(ByVal context As HttpContext, ByVal data As Object, token As String) As NormalResponse '上传MR数据
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
                '  Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpan(startTime))
            Else
                Return New NormalResponse(False, result, "错误:数据库录入失败", "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    '删除性能数据
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
    '上传性能数据
    Public Function Handle_UploadMMYMData(ByVal context As HttpContext, ByVal data As Object, token As String) As NormalResponse '增加茂名KPI数据
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
                Return New NormalResponse(True, result, "", "行数:" & dt.Rows.Count & ",用时:" & GetTimeSpan(startTime))
            Else
                Return New NormalResponse(False, result, "错误:数据库录入失败", "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.Message)
        End Try
    End Function
    '识别栅格
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
    '获取一个随机整数
    Private Function GetAoARand() As Integer
        Return Int(Rnd() * 121) - 60
    End Function
    '处理MR表
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
    '计算耗时
    Private Function GetTimeSpan(ByVal t As Date) As String
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

    '获取首页表格数据
    Public Function Handle_GetIndexPageTable(context As HttpContext) As NormalResponse '2018-12-23 09:54:00 更新 增加首页拼接表
        Dim info As String
        Dim sb As New StringBuilder
        Try

            Dim workStartTime As Date = Now
            Dim sql As String = "select day from QOE_REPORT_TABLE GROUP BY day ORDER BY day desc"
            Dim dt As DataTable
            'sql = OracleSelectPage(sql, 0, 7)
            'Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            'If IsNothing(dt) Then Return New NormalResponse(False, "没有任何数据")
            'If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何数据")
            Dim dayList As New List(Of String)
            'For Each row In dt.Rows
            '    If IsNothing(row(0)) = False Then
            '        If row(0) <> "" Then
            '            dayList.Add(row(0))
            '        End If
            '    End If
            'Next
            Dim n As Date = Now
            For i = 0 To 6
                dayList.Add(n.AddDays(-1 * i).ToString("yyyy-MM-dd"))
            Next
            sb.AppendLine("查询最近7天时间列表:" & GetTimeSpan(workStartTime))
            workStartTime = Now
            sql = "select day from indexPageTable"
            Dim dayDt As DataTable = ORALocalhost.SqlGetDT(sql)
            Dim oldDayList As New List(Of String)
            Dim nowDay As String = Now.ToString("yyyy-MM-dd")
            If IsNothing(dayDt) = False Then
                For Each dayRow In dayDt.Rows
                    Dim day As String = dayRow("day".ToUpper)
                    If day <> nowDay Then
                        If dayList.Contains(day) Then
                            dayList.Remove(day)
                            oldDayList.Add(day)
                        End If
                    End If
                Next
            End If
            sb.AppendLine("比对历史时间列表:" & GetTimeSpan(workStartTime))
            workStartTime = Now
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
            For Each d In oldDayList
                sql = "select * from indexPageTable where day='" & d & "'"
                Dim dtmp As DataTable = ORALocalhost.SqlGetDT(sql)
                If IsNothing(dtmp) = False Then
                    If dtmp.Rows.Count > 0 Then
                        Dim row As DataRow = dtmp.Rows(0)
                        Dim rw As DataRow = resultDt.NewRow
                        rw("时间") = row("day".ToUpper)
                        rw("采样点总数(移动)") = row("total_YD".ToUpper)
                        rw("采样点总数(联通)") = row("total_LT".ToUpper)
                        rw("采样点总数(电信)") = row("total_DX".ToUpper)
                        If IsNothing(row("coverage_YD".ToUpper).ToString) = False Then rw("覆盖率(移动)") = Double.Parse(Val(row("coverage_YD".ToUpper).ToString)).ToString("0.00")
                        If IsNothing(row("coverage_LT".ToUpper).ToString) = False Then rw("覆盖率(联通)") = Double.Parse(Val(row("coverage_LT".ToUpper).ToString)).ToString("0.00")
                        If IsNothing(row("coverage_DX".ToUpper).ToString) = False Then rw("覆盖率(电信)") = Double.Parse(Val(row("coverage_DX".ToUpper).ToString)).ToString("0.00")
                        rw("QOE(移动)") = row("QOE_YD".ToUpper)
                        rw("QOE(联通)") = row("QOE_LT".ToUpper)
                        rw("QOE(电信)") = row("QOE_DX".ToUpper)
                        rw("HttpQOE(移动)") = row("HTTPQOE_YD".ToUpper)
                        rw("HttpQOE(联通)") = row("HTTPQOE_LT".ToUpper)
                        rw("HttpQOE(电信)") = row("HTTPQOE_DX".ToUpper)
                        resultDt.Rows.Add(rw)
                    End If
                End If
            Next
            sb.AppendLine("复制历史时间数据:" & GetTimeSpan(workStartTime))
            workStartTime = Now
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
                                        If Cover > 100 Then Cover = 100
                                        row("覆盖率(移动)") = Format(Cover, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),2) from Qoe_Video_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国移动'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)

                                        row("QOE(移动)") = Format(vmos, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),2) from QOE_HTTP_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国移动'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("HttpQOE(移动)") = Format(vmos, "0.00")
                                    End If
                                End If

                            End If

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
                                        If rsrp > 100 Then rsrp = 100
                                        row("覆盖率(联通)") = Format(rsrp, "0.00")
                                    End If
                                End If


                                sql = "select round(avg(vmos),2) from Qoe_Video_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国联通'"
                                info = ORALocalhost.SQLInfo(sql)
                                If IsNothing(info) = False Then
                                    If IsNumeric(info) Then
                                        Dim vmos As Single = Val(info)
                                        row("QOE(联通)") = Format(vmos, "0.00")
                                    End If
                                End If

                                sql = "select round(avg(vmos),2) from QOE_HTTP_TABLE WHERE substr(datetime,0,10)='" & itm & "' and CARRIER='中国联通'"
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
                                        If Cover > 100 Then Cover = 100
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
            sb.AppendLine("计算新时间数据:" & GetTimeSpan(workStartTime))
            workStartTime = Now
            Dim cols() As String = GetOraTableColumns("indexPageTable")
            Dim pageDt As New DataTable
            If IsNothing(cols) = False Then
                For Each d In cols
                    If d <> "ID" Then
                        pageDt.Columns.Add(d)
                    End If
                Next
            End If
            Dim dateTimeStr As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
            For Each row In resultDt.Rows
                Dim dayStr As String = row("时间")
                If dayStr = nowDay Then
                    Dim rw As DataRow = pageDt.NewRow
                    rw("day".ToUpper) = dayStr
                    rw("total_YD".ToUpper) = row("采样点总数(移动)")
                    rw("total_LT".ToUpper) = row("采样点总数(联通)")
                    rw("total_DX".ToUpper) = row("采样点总数(电信)")
                    rw("coverage_YD".ToUpper) = row("覆盖率(移动)")
                    rw("coverage_LT".ToUpper) = row("覆盖率(联通)")
                    rw("coverage_DX".ToUpper) = row("覆盖率(电信)")
                    rw("QOE_YD".ToUpper) = row("QOE(移动)")
                    rw("QOE_LT".ToUpper) = row("QOE(联通)")
                    rw("QOE_DX".ToUpper) = row("QOE(电信)")
                    rw("HTTPQOE_YD".ToUpper) = row("HttpQOE(移动)")
                    rw("HTTPQOE_LT".ToUpper) = row("HttpQOE(联通)")
                    rw("HTTPQOE_DX".ToUpper) = row("HttpQOE(电信)")
                    rw("dateTime".ToUpper) = dateTimeStr
                    pageDt.Rows.Add(rw)
                Else
                    If oldDayList.Contains(dayStr) = False Then
                        Dim rw As DataRow = pageDt.NewRow
                        rw("day".ToUpper) = dayStr
                        rw("total_YD".ToUpper) = row("采样点总数(移动)")
                        rw("total_LT".ToUpper) = row("采样点总数(联通)")
                        rw("total_DX".ToUpper) = row("采样点总数(电信)")
                        rw("coverage_YD".ToUpper) = row("覆盖率(移动)")
                        rw("coverage_LT".ToUpper) = row("覆盖率(联通)")
                        rw("coverage_DX".ToUpper) = row("覆盖率(电信)")
                        rw("QOE_YD".ToUpper) = row("QOE(移动)")
                        rw("QOE_LT".ToUpper) = row("QOE(联通)")
                        rw("QOE_DX".ToUpper) = row("QOE(电信)")
                        rw("HTTPQOE_YD".ToUpper) = row("HttpQOE(移动)")
                        rw("HTTPQOE_LT".ToUpper) = row("HttpQOE(联通)")
                        rw("HTTPQOE_DX".ToUpper) = row("HttpQOE(电信)")
                        rw("dateTime".ToUpper) = dateTimeStr
                        pageDt.Rows.Add(rw)
                    End If
                End If
            Next
            If pageDt.Rows.Count > 0 Then
                sql = "delete  from indexPageTable where day='" & nowDay & "'"
                ORALocalhost.SqlCMD(sql)
                ORALocalhost.SqlCMDListQuickByPara("indexPageTable", pageDt)
            End If
            sb.AppendLine("修改历史新时间数据:" & GetTimeSpan(workStartTime))
            workStartTime = Now
            resultDt.DefaultView.Sort = "时间 desc"
            resultDt = resultDt.DefaultView.ToTable()
            sb.AppendLine("排序:" & GetTimeSpan(workStartTime))
            workStartTime = Now
            Return New NormalResponse(True, "version=1.0 计算量 dayList.length=" & dayList.Count, sb.ToString, resultDt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString, "", sb.ToString)
        End Try
    End Function
    '获取首页表格数据 备用接口
    Public Function Handle_GetIndexPageTable0(context As HttpContext) As NormalResponse '2018-12-23 更新 增加首页拼接表
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
    '获取视频资源 旧版本
    Public Function Handle_GetQOEVideoSource(context As HttpContext) As NormalResponse '获取QOE视频源
        Try
            Dim ip As String = context.Request.UserHostAddress
            Dim carrier As String = GetCarrierFromIp(ip)
            Dim sql As String = "select * from QOE_VIDEO_SOURCE where isuse=1"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.rows.count=0")
            If carrier = "移动" Then
                For Each row As DataRow In dt.Rows
                    row("URL") = row("URL").ToString.Replace("221.238.40.153:7062", "111.53.74.132")
                Next
            End If
            Return New NormalResponse(True, "ip=" & ip, "carrier=" & carrier, dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Structure QoEVideoScouceInfo
        Dim name As String
        Dim url As String
        Dim fileSize As String
        Dim videoSecond As Long
        Dim type As String
        Dim movieName As String
        Dim movieIndex As Integer
        Dim video_clarity As String
        Dim isRand As Boolean
        Dim secondGrad As Long
        Dim wantType As String
        Dim imsi As String
        Dim qoe_total_times As Long
        Dim qoe_total_E_times As Long
        Dim qoe_today_times As Long
        Dim qoe_today_E_times As Long
    End Structure
    '获取新视频资源 新版本
    Public Function Handle_GetNewQoEVideoInfo(context As HttpContext, data As Object, token As String) As NormalResponse
        Dim startDate As Date = Now
        Try
            Dim isHaveOld As Boolean = False
            Dim str As String = ""
            If IsNothing(data) Then
                isHaveOld = False
            Else
                str = data.ToString
            End If
            If str = "" Then isHaveOld = False
            Dim qvi As QoEVideoScouceInfo
            Try
                qvi = JsonConvert.DeserializeObject(Of QoEVideoScouceInfo)(str)
                If IsNothing(qvi) = False Then
                    isHaveOld = True
                End If
            Catch ex As Exception

            End Try
            Dim ip As String = context.Request.UserHostAddress
            Dim carrier As String = GetCarrierFromIp(ip)
            Dim sql As String = "select * from QOE_VIDEO_SOURCE where isuse=1"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "dt is null", GetTimeSpan(startDate), "")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "dt.rows.count=0", GetTimeSpan(startDate), "")
            Dim realList As New List(Of QoEVideoScouceInfo)
            For Each row As DataRow In dt.Rows
                Dim tmp As New QoEVideoScouceInfo
                tmp.name = row("name".ToUpper)
                tmp.url = row("url".ToUpper)
                tmp.fileSize = row("fileSize".ToUpper)
                tmp.videoSecond = row("videoSecond".ToUpper)
                tmp.type = row("type".ToUpper)
                tmp.movieName = row("movieName".ToUpper)
                tmp.movieIndex = row("movieIndex".ToUpper)
                tmp.video_clarity = row("video_clarity".ToUpper)
                tmp.isRand = row("isRand".ToUpper)
                tmp.secondGrad = row("secondGrad".ToUpper)
                realList.Add(tmp)
            Next
            If qvi.wantType = "" Then qvi.wantType = "全部"
            Dim resultVideoInfo As New QoEVideoScouceInfo

            If isHaveOld Then
                If qvi.wantType <> "全部" Then
                    Dim tmpList As List(Of QoEVideoScouceInfo) = (From s In realList Where s.type = qvi.wantType Select s).ToList()
                    If IsNothing(tmpList) = False Then
                        realList = tmpList
                    End If
                End If
                Dim isNeedRand As Boolean = True
                If qvi.isRand = False And qvi.movieName <> "" Then
                    isNeedRand = False
                End If
                If isNeedRand Then
                    resultVideoInfo = realList(New Random().Next(realList.Count - 1))
                Else
                    Dim isFind As Boolean = False
                    For Each video In realList
                        If video.movieName = qvi.movieName Then
                            If video.movieIndex = qvi.movieIndex + 1 Then
                                isFind = True
                                resultVideoInfo = video
                                Exit For
                            End If
                        End If
                    Next
                    If isFind = False Then
                        resultVideoInfo = realList(New Random().Next(realList.Count - 1))
                    End If
                End If
            End If
            If IsNothing(resultVideoInfo) Then
                resultVideoInfo = realList(New Random().Next(realList.Count - 1))
            End If
            If carrier = "移动" Then
                resultVideoInfo.url = resultVideoInfo.url.Replace("221.238.40.153:7062", "111.53.74.132")
            End If
            If IsNothing(qvi.imsi) = False AndAlso qvi.imsi <> "" Then
                sql = "select QOE_TOTAL_time,QOE_TOTAL_E_time,QOE_TODAY_time,QOE_TODAY_E_time from user_bp_table where imsi='" & qvi.imsi & "'"
                dt = ORALocalhost.SqlGetDT(sql)
                If IsNothing(dt) = False Then
                    If dt.Rows.Count > 0 Then
                        Dim row As DataRow = dt.Rows(0)
                        Try
                            resultVideoInfo.qoe_total_times = Val(row("QOE_TOTAL_time").ToString)
                            resultVideoInfo.qoe_total_E_times = Val(row("QOE_TOTAL_E_time").ToString)
                            resultVideoInfo.qoe_today_times = Val(row("QOE_TODAY_time").ToString)
                            resultVideoInfo.qoe_today_E_times = Val(row("QOE_TODAY_E_time").ToString)
                        Catch ex As Exception

                        End Try
                    End If
                End If
                Task.Run(Sub()
                             Dim askUrl As String = resultVideoInfo.url
                             Dim imsi As String = qvi.imsi
                             Dim isExit As Boolean = ORALocalhost.SqlIsIn("select id from user_bp_table where imsi='" & imsi & "'")
                             If isExit Then
                                 sql = "update user_bp_table set lastAskvideoUrl='{0}' where imsi='{1}'"
                                 sql = String.Format(sql, askUrl, imsi)
                             Else
                                 sql = "insert into user_bp_table(dateTime,lastAskvideoUrl,imsi) values('{0}','{1}','{2}')"
                                 sql = String.Format(sql, Now.ToString("yyyy-MM-dd HH:mm:ss"), askUrl, imsi)
                             End If
                             ORALocalhost.SqlCMD(sql)
                         End Sub)
            End If
            realList = Nothing
            resultVideoInfo.imsi = qvi.imsi
            resultVideoInfo.wantType = qvi.wantType
            Return New NormalResponse(True, "ip=" & ip & ";carrier=" & carrier, GetTimeSpan(startDate), resultVideoInfo)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString, GetTimeSpan(startDate), "")
        End Try
    End Function

    '获取一键测试地址
    Public Function Handle_GetOneKeyTestUrl(context As HttpContext) As NormalResponse
        Try
            Dim ip As String = context.Request.UserHostAddress
            Dim carrier As String = GetCarrierFromIp(ip)
            Dim url As String = "http://221.238.40.153:7062/video/OneKeyTestVideo/zongyi1_30s_360p_162.mp4"
            'Dim sql As String = "select url,filesize,VIDEOSECOND from QOE_VIDEO_SOURCE WHERE FILESIZEM>2 and  FILESIZEM<3 "
            'sql = OracleSelectPage(sql, 0, 1)
            'Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            'If IsNothing(dt) = False Then
            '    If dt.Rows.Count > 0 Then
            '        Dim row As DataRow = dt.Rows(0)
            '        url = row("url".ToUpper).ToString
            '    End If
            'End If
            If carrier = "移动" Then
                url = url.Replace("221.238.40.153:7062", "111.53.74.132")
            End If
            Return New NormalResponse(True, "ip=" & ip, "carrier=" & carrier, url)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Structure OneKeyTestUrlInfo
        Dim url As String
        Dim filesize As Long
        Dim videoSecond As Integer
    End Structure
    '获取新版本,UniQoE V1.4.9以上一键测试地址
    Public Function Handle_GetNewVersionOneKeyTestUrl(context As HttpContext) As NormalResponse
        Try
            Dim ip As String = context.Request.UserHostAddress
            Dim carrier As String = GetCarrierFromIp(ip)
            Dim url As String = "http://221.238.40.153:7062/video/OneKeyTestVideo/zongyi1_30s_360p_162.mp4"
            Dim info As New OneKeyTestUrlInfo
            info.url = url
            info.filesize = 2097522
            info.videoSecond = 30
            'Dim sql As String = "select url,filesize,VIDEOSECOND from QOE_VIDEO_SOURCE WHERE FILESIZEM>2 and  FILESIZEM<3 "
            'sql = OracleSelectPage(sql, 0, 1)
            'Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            'If IsNothing(dt) = False Then
            '    If dt.Rows.Count > 0 Then
            '        Dim row As DataRow = dt.Rows(0)
            '        url = row("url".ToUpper).ToString()
            '        info.filesize = Val(row("filesize".ToString).ToString())
            '        info.videoSecond = Val(row("VIDEOSECOND".ToString).ToString())
            '    End If
            'End If
            If carrier = "移动" Then
                url = url.Replace("221.238.40.153:7062", "111.53.74.132")
            End If
            info.url = url
            Return New NormalResponse(True, "ip=" & ip, "carrier=" & carrier, info)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Structure SysModuleInfo  '系统模板信息
        Dim id As String
        Dim dateTime As String
        Dim userName As String
        Dim type As String
        Dim content As String
    End Structure
    '新增模板
    Public Function Handle_AddSysModule(context As HttpContext, data As Object, token As String) As NormalResponse '新建查询模板
        Try
            If IsNothing(data) Then Return New NormalResponse(False, "post data is null")
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim mi As SysModuleInfo = JsonConvert.DeserializeObject(str, GetType(SysModuleInfo))
            If IsNothing(mi) Then Return New NormalResponse(False, "SysModuleInfo is null,maybe json is error")
            'If mi.content.Contains(Chr(34)) Then
            '    Return New NormalResponse(False, "content不允许包含双引号")
            'End If
            ' mi.content = mi.content.Replace("'", Chr(34))
            mi.content = Str2Base64(mi.content)
            Dim sql As String = "insert into sys_module_table (dateTime,type,userName,content) values ('{0}','{1}','{2}','{3}')"
            sql = String.Format(sql, New String() {Now.ToString("yyyy-MM-dd HH:mm:ss"), mi.type, mi.userName, mi.content})
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, result)
            Else
                Return New NormalResponse(False, result, "", sql)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '删除模板
    Public Function Handle_DeleteSysModule(context As HttpContext) As NormalResponse  '删除查询模板
        Try
            Dim id As String = context.Request.QueryString("id")
            If IsNothing(id) Then Return New NormalResponse(False, "id is null")
            If id = "" Then Return New NormalResponse(False, "id is ''")
            Dim sql As String = "select * from sys_module_table where id=" & id
            Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
            If Not isExist Then
                Return New NormalResponse(False, "id does not exist")
            End If
            sql = "delete from sys_module_table where id=" & id
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, "删除成功！")
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '修改模板
    Public Function Handle_UpdateSysModule(context As HttpContext, data As Object, token As String) As NormalResponse  '修改查询模板
        Try
            If IsNothing(data) Then Return New NormalResponse(False, "post data is null")
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim mi As SysModuleInfo = JsonConvert.DeserializeObject(str, GetType(SysModuleInfo))
            If IsNothing(mi) Then Return New NormalResponse(False, "SysModuleInfo is null,maybe json is error")
            Dim sql As String = "select * from sys_module_table where id=" & mi.id
            Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
            If Not isExist Then
                Return New NormalResponse(False, "id does not exist")
            End If
            'If mi.content.Contains(Chr(34)) Then
            '    Return New NormalResponse(False, "content不允许包含双引号")
            'End If
            'mi.content = mi.content.Replace("'", Chr(34))
            mi.content = Str2Base64(mi.content)
            sql = "update sys_module_table set dateTime='{0}',userName='{1}',type='{2}',content='{3}' where id=" & mi.id
            sql = String.Format(sql, New String() {Now.ToString("yyyy-MM-dd HH:mm:ss"), mi.userName, mi.type, mi.content})
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, "修改成功！")
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '查询模板
    Public Function Handle_GetSysModule(context As HttpContext) As NormalResponse  '查询系统模板
        Try
            Dim sql As String = "select * from sys_module_table"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何数据")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何数据")
            For Each row In dt.Rows
                Dim content As String = row("CONTENT").ToString
                '   content = content.Replace(Chr(34), "'")
                content = Base2str(content)
                row("CONTENT") = content
            Next
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '查询否可以升级
    Public Function Handle_GetCanUpdate(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim obj As JObject = JObject.Parse(str)
            Dim appName As String = obj.GetValue("appName")
            Dim version As String = obj.GetValue("version")
            If appName = "" Or version = "" Then
                Return New NormalResponse(False, "appName或version为空")
            End If
            Dim rootPath As String = ""   ' "d:\soft\update\"
            Dim virtualPath As String = "update"
            rootPath = System.Web.HttpContext.Current.Server.MapPath("~/" & virtualPath & "/")
            Dim appNameWithoutExe As String = appName
            If appName.Contains(".") Then
                appNameWithoutExe = appName.Split(".")(0)
                If appNameWithoutExe = "" Then
                    Return New NormalResponse(False, "appName为空")
                End If
            End If
            Dim appPath As String = rootPath & "\" & appNameWithoutExe & "\"
            Dim appFileName As String = appPath & appName
            Dim appVersionFileName As String = appPath & "version.txt"
            If File.Exists(appVersionFileName) = False Then
                Return New NormalResponse(False, "服务器没有该app的升级信息，请联系管理员,app名称为" & appName)
            End If
            Dim appVersion As String = File.ReadAllText(appVersionFileName)
            Try
                Dim oldVersion As Version = System.Version.Parse(version)
                Dim newVersion As Version = System.Version.Parse(appVersion)
                Dim flag As Boolean = CanUpdate(version, appVersion)
                Dim nb As New JObject
                nb.Add("canUpdate", flag)
                nb.Add("serverVersion", appVersion)
                nb.Add("url", "http://221.238.40.153:7062/" & virtualPath & "/" & appNameWithoutExe & "/" & appName)
                Return New NormalResponse(True, "", "", nb)
            Catch ex As Exception
                Return New NormalResponse(False, "version版本格式非法，无法比较")
            End Try
        Catch ex As Exception

        End Try
    End Function
    '比对两个版本号的高低
    Public Function CanUpdate(ByVal oldVersion As String, ByVal newVersion As String) As Boolean
        Dim oldv As New Version(oldVersion)
        Dim newv As New Version(newVersion)
        If oldv < newv Then
            Return True
        Else
            Return False
        End If

    End Function

    '新增任务
    Public Function Handle_AddMission(context As HttpContext, data As Object, token As String) As NormalResponse
        Dim str As String
        Try
            str = JsonConvert.SerializeObject(data)
            Dim am As AppMission = JsonConvert.DeserializeObject(str, GetType(AppMission))
            If IsNothing(am) Then Return New NormalResponse(False, "AppMission is null")
            Dim np As NormalResponse = DeviceHelper.AddMission(am)
            Return np
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString, "", str)
        End Try
    End Function
    '查询在线正在执行的任务
    Public Function Handle_GetOnlineMission(context As HttpContext) As NormalResponse
        Dim endTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "select * from app_mission_table where isClosed=0 order by dateTime desc"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return New NormalResponse(False, "没有正在执行的任务")
        If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有正在执行的任务")
        Return New NormalResponse(True, "", "", dt)
    End Function
    '查询所有任务
    Public Function Handle_GetAllMission(context As HttpContext) As NormalResponse
        Dim endTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "select * from app_mission_table order by dateTime desc"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return New NormalResponse(False, "没有任何任务")
        If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何任务")
        Return New NormalResponse(True, "", "", dt)
    End Function
    '查询异常任务
    Public Function Handle_GetErrorMission(context As HttpContext) As NormalResponse
        Dim endTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "select * from app_mission_table where isClosed=0 and status='" & "异常" & "'"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return New NormalResponse(False, "没有异常任务")
        If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有异常任务")
        Return New NormalResponse(True, "", "", dt)
    End Function

    '查询设备权限
    Public Function Handle_GetDevicePermission(context As HttpContext) As NormalResponse
        Dim imei As String = context.Request.QueryString("imei")
        If imei = "" Then Return New NormalResponse(True, "", "", 0)
        Dim sql As String = "select * from deviceTable where imei='" & imei & "'"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return New NormalResponse(True, "", "", 0)
        If dt.Rows.Count = 0 Then Return New NormalResponse(True, "", "", 0)
        Dim row As DataRow = dt.Rows(0)
        Dim permission As String = row("power").ToString
        If IsNothing(permission) Then permission = 0
        If permission = "" Then permission = 0
        Return New NormalResponse(True, "", "", permission)
    End Function
    '手工刷新省市区
    Public Function Handle_RefrushProAndCity(context As HttpContext) As NormalResponse
        Try
            DoMathProAndCity()
            Return New NormalResponse(True, "success")
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Structure PiBridgeMsgInfo '测试信息  调试阶段使用
        Dim msg As String
        Dim PiId As String
    End Structure
    '上传测试信息  调试阶段使用
    Public Function Handle_UploadPiBridgeMsg(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim pm As PiBridgeMsgInfo = JsonConvert.DeserializeObject(str, GetType(PiBridgeMsgInfo))
            If IsNothing(pm) Then
                Return New NormalResponse(False, "PiBridgeMsgInfo 格式非法")
            End If
            Dim msg As String = pm.msg
            Dim PiId As String = pm.PiId
            If msg = "" Then
                Return New NormalResponse(False, "PiBridgeMsgInfo.msg不能为空")
            End If
            If PiId = "" Then
                Return New NormalResponse(False, "PiBridgeMsgInfo.PiId不能为空")
            End If
            Dim sql As String = "insert into PiBridgeTestTable(dateTime,msg,piid)values('{0}','{1}','{2}')"
            sql = String.Format(sql, New String() {Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, PiId})
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, result)
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function

    '用户登陆
    Public Function Handle_login(ByVal context As HttpContext) As NormalResponse '用户登陆
        Try
            Dim account As String = context.Request.QueryString("usr")
            Dim passWord As String = context.Request.QueryString("pwd")
            If account = "" Then Return New NormalResponse(False, "用户名为空")
            If passWord = "" Then Return New NormalResponse(False, "密码为空")
            Dim sql As String = "select password,token,userName,power,state from user_Account where userName='" & account & "' and state<>0"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "该用户不存在")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "该用户不存在")
            Dim row As DataRow = dt.Rows(0)
            Dim OraPwd As String = row("password".ToUpper).ToString
            Dim OraToken As String = row("token".ToUpper).ToString
            Dim oraName As String = row("userName".ToUpper).ToString
            Dim power As Integer = Val(row("power".ToUpper).ToString)
            Dim state As Integer = Val(row("state".ToUpper).ToString)
            If IsDBNull(OraToken) Then OraToken = ""
            If OraPwd = passWord Then
                If OraToken = "" Then
                    OraToken = GetNewToken(account, True)
                End If
                Dim linfo As New loginInfo(account, oraName, OraToken, power, state)
                Return New NormalResponse(True, "success", "", linfo)
            Else
                Return New NormalResponse(False, "用户名或密码错误", "", "")
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Public Function Handle_GetUserNameByToken(context As HttpContext) As NormalResponse
        Try
            Dim token As String = context.Request.QueryString("token")
            Dim userName As String = GetUsrByToken(token)
            Return New NormalResponse(True, "", "", userName)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    'APP使用，查询本地保存的视频文件有没有实际对应的QOE VIDEO记录,无需token
    Public Function Handle_CheckScreenRecordFile(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim fileName As String = data.ToString()
            'screenrecord_filename
            Dim sql As String = "select * from QOE_VIDEO_TABLE where screenrecord_filename='" & fileName & "' and isscreenrecorduploaded=0"
            Dim bool As Boolean = ORALocalhost.SqlIsIn(sql)
            Return New NormalResponse(True, "", "", IIf(bool, 1, 0))
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    'APP使用，上传本地保存的视频文件,无需token
    Structure ScreenRecordFileInfo
        Dim fileName As String
        Dim filelenth As Long
        Dim base64 As String
    End Structure
    Public Function Handle_UploadScreenRecordFile(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim str As String = data.ToString
            Dim scf As ScreenRecordFileInfo = JsonConvert.DeserializeObject(str, GetType(ScreenRecordFileInfo))
            Dim sql As String = "select * from QOE_VIDEO_TABLE where screenrecord_filename='" & scf.fileName & "' and isscreenrecorduploaded=0"
            Dim bool As Boolean = ORALocalhost.SqlIsIn(sql)
            If Not bool Then
                Return New NormalResponse(False, "该文件名称不在数据库记录中")
            End If

            Dim virtualPath As String = "ScreenRecordFiles"
            Dim buffer() As Byte = Convert.FromBase64String(scf.base64)
            If IsNothing(buffer) Then
                Return New NormalResponse(False, "base64内容非法")
            End If
            If buffer.Length <> scf.filelenth Then
                Return New NormalResponse(False, "文件大小与描述不符")
            End If
            Dim rootPath = System.Web.HttpContext.Current.Server.MapPath("~/" & virtualPath & "/")
            Dim filePath As String = rootPath & "/" & scf.fileName
            If File.Exists(filePath) Then File.Delete(filePath)
            File.WriteAllBytes(filePath, buffer)
            sql = "update QOE_VIDEO_TABLE set isscreenrecorduploaded=1 where screenrecord_filename='" & scf.fileName & "'"
            Dim result As String = ORALocalhost.SqlCMD(sql)
            Return New NormalResponse(True, result, "", "")
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Public Function Handle_UploadOneKeyTestInfo(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim str As String = data.ToString
            Dim okt As OneKeyTestInfo = JsonConvert.DeserializeObject(Of OneKeyTestInfo)(str)
            If IsNothing(okt) Then Return New NormalResponse(False, "OneKeyTestInfo from json is null")
            Dim rid As String = Guid.NewGuid.ToString("N")
            If IsNothing(okt.pi) = False Then
                Dim pi As PhoneInfo = okt.pi
                pi.businessType = "One Key Test"
                pi.RID = rid

                Dim np As NormalResponse = InsertPhoneInfoToOracle(pi)
                If np.result = False Then
                    rid = ""
                    pi.RID = rid
                End If

                okt.pi = pi
                ' InsertPhoneInfoToOracleAsync(pi)
                'Task.Run(Sub()
                '             InsertPhoneInfoToOracle(pi)
                '         End Sub)
            End If
            Dim dt As DataTable = ORALocalhost.GetOraTableColumnsOnDt("QOE_ONE_KEY_TEST", True)
            If IsNothing(dt) Then Return New NormalResponse(False, "表QOE_REPORT_TABLE不存在")
            Dim row As DataRow = dt.NewRow
            row("DATETIME") = Now.ToString("yyyy-MM-dd HH:mm:ss")
            row("PHONEMODEL") = okt.pi.phoneModel
            row("IMEI") = okt.pi.IMEI
            row("NETTYPE") = okt.pi.netType
            row("RSRP_SCORE") = okt.net4gStrengthScore.score
            row("SINR_SCORE") = okt.net4gQualityScore.score
            row("WIFI_STRENGTH_SCORE") = okt.wifiStrengthScore.score
            row("WIFI_QUALITY_SCORE") = okt.wifiQualityScore.score
            row("NETSPEED_SCORE") = okt.netSpeedScore.score
            row("VIDEOSPEED_SCORE") = okt.videoScore.score
            row("HTTP_SCORE") = okt.htmlPageScore.score
            row("TOTAL_SCORE") = okt.togetherScore.score
            row("NETSPEED_SPEED") = okt.netSpeedTestSpeed
            row("VIDEOSPEED_SPEED") = okt.videoTestSpeed
            row("HTTP_RESONSETIME") = okt.httpResonseTime
            row("QOER_RID") = rid

            dt.Rows.Add(row)
            Dim result As String = ORALocalhost.SqlCMDListQuickByPara("QOE_ONE_KEY_TEST", dt)
            If result = "success" Then
                Return New NormalResponse(True, "success", "", "")
            Else
                Return New NormalResponse(False, result)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    Private Function InsertPhoneInfoToOracleAsync(pi As PhoneInfo) As Task(Of NormalResponse)
        Return Task.Run(Function()
                            Return InsertPhoneInfoToOracle(pi)
                        End Function)
    End Function

    Structure ClientLogInfo
        Dim title As String
        Dim body As String
        Dim serverModuleName As String
        Dim clientIP As String
        Dim clientUserName As String
    End Structure
    '增加访问日志
    Public Function Handle_AddClientLog(context As HttpContext, data As Object, token As String) As NormalResponse
        Try
            Dim str As String = JsonConvert.SerializeObject(data)
            Dim cinfo As ClientLogInfo = JsonConvert.DeserializeObject(str, GetType(ClientLogInfo))
            If cinfo.title = "" Or cinfo.body = "" Or cinfo.serverModuleName = "" Or cinfo.clientIP = "" Or cinfo.clientUserName = "" Then
                Return New NormalResponse(False, "日志参数不完整")
            End If
            Dim usr As String = GetUsrByToken(token)
            Dim sql As String = "insert into SYS_VISIT (dateTime,title,body,serverModuleName,clientIP,clientUserName) values ('{0}','{1}','{2}','{3}','{4}','{5}')"
            Dim list As New List(Of String)
            list.Add(Now.ToString("yyyy-MM-dd HH:mm:ss"))
            list.Add(cinfo.title)
            list.Add(cinfo.body)
            list.Add(cinfo.serverModuleName)
            list.Add(cinfo.clientIP)
            list.Add(usr)
            sql = String.Format(sql, list.ToArray)
            Dim result As String = ORALocalhost.SqlCMD(sql)
            If result = "success" Then
                Return New NormalResponse(True, "新增日志成功")
            Else
                Return New NormalResponse(False, result, "", sql)
            End If
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '获取访问日志
    Public Function Handle_GetClientLog(context As HttpContext) As NormalResponse
        Try
            Dim countstr As String = context.Request.QueryString("count")
            Dim count As Integer = 0
            If IsNothing(countstr) = False Then
                If countstr <> "" Then
                    If IsNumeric(countstr) Then
                        count = Val(countstr)
                    End If
                End If
            End If
            Dim sql As String = "select * from SYS_VISIT order by dateTime desc"
            If count > 0 Then
                sql = OracleSelectPage(sql, 0, count)
            End If
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then
                Return New NormalResponse(False, "没有任何记录")
            End If
            If dt.Rows.Count = 0 Then
                Return New NormalResponse(False, "没有任何记录")
            End If
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    'APP获取设备一键测试ID列表
    Public Function Handle_GetOneKeyTestHisIdList(context As HttpContext) As NormalResponse
        Try
            Dim imei As String = context.Request.QueryString("imei")
            If IsNothing(imei) Then Return New NormalResponse(False, "设备Id为空")
            If imei = "" Then Return New NormalResponse(False, "设备Id为空")
            Dim sql As String = "select * from QOE_ONE_KEY_TEST where imei='" & imei & "' order by dateTime desc"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何记录")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何记录")
            Return New NormalResponse(True, "", "", dt)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '根据一键测试的ID来获取具体数据
    Public Function Handle_GetOneKeyTestHisData(context As HttpContext) As NormalResponse
        Try
            Dim id As String = context.Request.QueryString("id")
            If IsNothing(id) Then Return New NormalResponse(False, "id为空")
            If id = "" Then Return New NormalResponse(False, "id为空")
            Dim sql As String = "select * from QOE_ONE_KEY_TEST where id=" & id
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有任何记录")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有任何记录")
            Dim row As DataRow = dt.Rows(0)
            Return New NormalResponse(True, "", "", row)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    '用户查询积分
    Public Function Handle_GetMyBonusPoints(context As HttpContext) As NormalResponse
        Try
            Dim imsi As String = context.Request.QueryString("imsi")
            If IsNothing(imsi) Then Return New NormalResponse(False, "imsi不可为空")
            If imsi = "" Then Return New NormalResponse(False, "imsi不可为空")
            Dim sql As String = "select * from user_bp_table where imsi='" & imsi & "'"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return New NormalResponse(False, "没有您的积分记录")
            If dt.Rows.Count = 0 Then Return New NormalResponse(False, "没有您的积分记录")
            Dim row As DataRow = dt.Rows(0)
            Dim bonusPoints As String = row("bonusPoints".ToUpper()).ToString()
            Dim bp As Single = 0
            Single.TryParse(bonusPoints, bp)
            Return New NormalResponse(True, "", "", bp.ToString("0.0"))
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString())
        End Try
    End Function
    'app获取ping地址
    Public Function Handle_GetPingIP(context As HttpContext) As NormalResponse
        Try
            Dim ip As String = context.Request.UserHostAddress
            Dim carrier As String = GetCarrierFromIp(ip)
            Dim ping As String = "221.238.40.153"
            If carrier = "移动" Then
                ping = "111.53.74.132"
            End If
            Return New NormalResponse(True, "ip=" & ip, "carrier=" & carrier, ping)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    'app在登录时候获取aid
    Public Function Handle_GetAid(context As HttpContext) As NormalResponse
        Try
            Dim imsi As String = context.Request.QueryString("imsi")
            Dim imei As String = context.Request.QueryString("imei")
            If IsNothing(imsi) Then Return New NormalResponse(False, "imsi不可为空")
            If IsNothing(imei) Then Return New NormalResponse(False, "imei不可为空")
            Dim aid As String = DeviceHelper.HandleImeiAndUid(imsi)
            Return New NormalResponse(True, "", "", aid)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
    'app获取视频分类列表
    Public Function Handle_GetQoEVideoSourceTypeList(context As HttpContext) As NormalResponse
        Try
            Dim dt As DataTable = ORALocalhost.SqlGetDT("select type from QOE_VIDEO_SOURCE group by type")
            Dim list As New List(Of String)
            list.Add("全部")
            Dim tmpList = (From d In dt.AsEnumerable() Select d.Field(Of String)("type")).ToList()
            list.AddRange(tmpList)
            Return New NormalResponse(True, "", "", list)
        Catch ex As Exception
            Return New NormalResponse(False, ex.ToString)
        End Try
    End Function
End Class
