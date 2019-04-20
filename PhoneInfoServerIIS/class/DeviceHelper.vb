
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

Public Class DeviceHelper

    Public Shared Function HandleImeiAndUid(imsi As String, Optional imei As String = "") As String
        If imsi = "" Then Return ""
        Dim txt As String = ORALocalhost.SQLInfo("select aid from deviceTable where imsi='" & imsi & "'")
        If IsNothing(txt) = False Then
            If txt <> "" Then
                Return txt
            End If
        End If
        If imei <> "" Then
            txt = ORALocalhost.SQLInfo("select aid from deviceTable where imei='" & imei & "'")
            If IsNothing(txt) = False Then
                If txt <> "" Then
                    Return txt
                End If
            End If
        End If
        Dim aid As String = GetNewAid()
        Dim sql As String = "update deviceTable set aid='" & aid & "' where imsi='" & imsi & "'"
        ORALocalhost.SqlCMD(sql)
        Dim oracleImsi As String = "select imsi from deviceTable where aid='" & aid & "'"
        If oracleImsi = imsi Then
            Return aid
        End If
        If imei <> "" Then
            sql = "update deviceTable set aid='" & aid & "' where imei='" & imei & "'"
            ORALocalhost.SqlCMD(sql)
        End If
        Return aid
    End Function
    Public Shared Sub ChangeDeviceStatus(pi As PhoneInfo)
        If IsNothing(pi) Then Return
        If IsNothing(pi.AID) Then
            pi.AID = ""
        End If
        If pi.AID = "" Then
            pi.AID = HandleImeiAndUid(pi.IMSI, pi.IMEI)
        End If
        If pi.AID = "" Then
            Return
        End If
        Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "select * from deviceTable where aid='" & pi.AID & "'"
        Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
        'LogHelper.Log($"ChangeDeviceStatus aid={pi.AID},isExist={isExist}")
        'File.AppendAllText("d:\error", $"ChangeDeviceStatus aid={pi.AID},isExist={isExist}" & Environment.NewLine)
        If isExist Then
            sql = "update deviceTable set {0} GroupId='{1}',lastDateTime='{2}',isBusy='{3}',
                   province='{4}',city='{5}',district='{6}',lon='{7}',lat='{8}',bdlon='{9}',bdlat='{10}',gdlon='{11}',gdlat='{12}',apkVersion='{13}',imei='{14}',imsi='{15}',isOnline=1,carrier='{16}' where aid='" & pi.AID & "'"
            Dim list As New List(Of String)
            list.Add("") 'userName
            list.Add("") 'groupId
            list.Add(time)
            list.Add(0)  'isBusy
            list.Add(pi.province)
            list.Add(pi.city)
            list.Add(pi.district)
            list.Add(pi.lon)
            list.Add(pi.lat)
            list.Add(pi.bdlon)
            list.Add(pi.bdlat)
            list.Add(pi.gdlon)
            list.Add(pi.gdlat)
            list.Add(pi.apkVersion)
            list.Add(pi.IMEI)
            list.Add(pi.IMSI)
            list.Add(pi.carrier)
            sql = String.Format(sql, list.ToArray())
            Dim result As String = ORALocalhost.SqlCMD(sql)
            '  LogHelper.Log($"ChangeDeviceStatus aid={pi.AID},result={result}")
            ' File.AppendAllText("d:\error", $"ChangeDeviceStatus aid={pi.AID},result={result}" & Environment.NewLine)
        Else
            sql = "insert into deviceTable (DateTime,userName,imei,GroupId,lastDateTime,isBusy,province,city,district,lon,lat,bdlon,bdlat,gdlon,gdlat,power,PHONEMODEL,apkVersion,imsi,aid,isOnline,carrier) values ('{0}','{1}','{2}','{3}','{4}',
                    '{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',1,'{20}')"
            Dim list As New List(Of String)
            list.Add(time)
            list.Add("") 'userName
            list.Add(pi.IMEI)
            list.Add("") 'groupId
            list.Add(time)
            list.Add(0)  'isBusy
            list.Add(pi.province)
            list.Add(pi.city)
            list.Add(pi.district)
            list.Add(pi.lon)
            list.Add(pi.lat)
            list.Add(pi.bdlon)
            list.Add(pi.bdlat)
            list.Add(pi.gdlon)
            list.Add(pi.gdlat)
            list.Add(9) 'power
            list.Add(pi.phoneModel)
            list.Add(pi.apkVersion)
            list.Add(pi.IMSI)
            list.Add(GetNewAid())
            list.Add(pi.carrier)
            sql = String.Format(sql, list.ToArray())
            Dim result As String = ORALocalhost.SqlCMD(sql)
            '  LogHelper.Log($"ChangeDeviceStatus aid={pi.AID},result={result}")

            ' File.AppendAllText("d:\error", $"ChangeDeviceStatus aid={pi.AID},result={result}" & Environment.NewLine)
        End If
    End Sub
    Private Shared Function GetNewAid() As String
        While True
            Dim aid As String = System.Guid.NewGuid().ToString("N").Substring(0, 6)
            If Regex.IsMatch(aid, "[A-Za-z].*[0-9]|[0-9].*[A-Za-z]") Then
                Dim sql As String = "select id from devicelog where aid='" & aid & "'"
                If ORALocalhost.SqlIsIn(Sql) = False Then
                    Return aid
                End If
            End If
        End While
    End Function
    Public Shared Function AddMission(am As AppMission) As NormalResponse
        Dim id As String = am.id
        Dim dateTime As String = am.dateTime
        Dim userName As String = am.userName
        Dim imei As String = am.imei
        Dim phoneModel As String = am.phoneModel
        Dim groupId As String = am.groupId
        Dim deviceCount As Integer = am.deviceCount
        Dim missionType As String = am.missionType
        Dim missionRemark As String = am.missionRemark
        Dim startTime As String = am.startTime
        Dim endTime As String = am.endTime
        Dim status As String = am.status
        Dim isClosed As Integer = am.isClosed
        If imei = "" And groupId = "" Then
            Return New NormalResponse(False, "imei和GroupId不能同时为空")
        End If
        Try
            Dim taskStartTime As Date = Date.Parse(startTime)
            Dim taskEndTime As Date = Date.Parse(endTime)
            If taskEndTime <= taskStartTime Then
                Return New NormalResponse(False, "结束时间必须大于开始时间")
            End If
            If taskEndTime <= Now.AddMinutes(1) Then
                Return New NormalResponse(False, "结束时间必须大于当前时间")
            End If
            startTime = taskStartTime.ToString("yyyy-MM-dd HH:mm:ss")
            endTime = taskEndTime.ToString("yyyy-MM-dd HH:mm:ss")
        Catch ex As Exception
            Return New NormalResponse(False, "开始时间或结束时间格式非法")
        End Try
        Dim isGroup As Boolean = False
        Dim sql As String = ""
        Dim groupImeilist As New List(Of String)
        If groupId <> "" Then
            '查询groupId表
            Dim groupDt As DataTable = ORALocalhost.SqlGetDT("select * from DT_GROUP where groupid='" & groupId & "'")
            If IsNothing(groupDt) Then
                Return New NormalResponse(False, "该测试组ID不存在")
            End If
            If groupDt.Rows.Count = 0 Then
                Return New NormalResponse(False, "该测试组ID不存在")
            End If
            Dim groupRow As DataRow = groupDt.Rows(0)
            Dim IMEI_CM As String = groupRow("IMEI_CM").ToString()
            Dim IMEI_CU As String = groupRow("IMEI_CU").ToString()
            Dim IMEI_CT As String = groupRow("IMEI_CT").ToString()
            Dim userName_CM As String = groupRow("NAME_CM").ToString()
            Dim userName_CU As String = groupRow("NAME_CU").ToString()
            Dim userName_CT As String = groupRow("NAME_CT").ToString()
            Dim userNameList As New List(Of String)


            Dim list As New List(Of String)
            Dim modelList As New List(Of String)
            If IMEI_CM <> "" Then list.Add(IMEI_CM) : userNameList.Add(userName_CM) ： modelList.Add(GetPhoneModelByImei(IMEI_CM))
            If IMEI_CU <> "" Then list.Add(IMEI_CU) : userNameList.Add(userName_CU) ： modelList.Add(GetPhoneModelByImei(IMEI_CU))
            If IMEI_CT <> "" Then list.Add(IMEI_CT) : userNameList.Add(userName_CT) ： modelList.Add(GetPhoneModelByImei(IMEI_CT))
            If list.Count = 0 Then
                Return New NormalResponse(False, "任务下发失败，该测试组不存在任何测试设备")
            End If
            sql = "select * from app_mission_table where  groupId='{0}' and isClosed=0 and " &
                                                       "((StartTime between '{1}' and '{2}' or EndTime between '{1}' and '{2}') " &
                                                       "or (StartTime<='{1}' and EndTime>='{2}'))"
            sql = String.Format(sql, groupId, startTime, endTime)
            If ORALocalhost.SqlIsIn(sql) Then
                Return New NormalResponse(False, "任务下发失败，该时段测试组'" & groupId & "'正忙")
            End If

            For Each itm In list
                sql = "select * from app_mission_table where  imei like '%{0}%' and isClosed=0 and " &
                                                       "((StartTime between '{1}' and '{2}' or EndTime between '{1}' and '{2}') " &
                                                       "or (StartTime<='{1}' and EndTime>='{2}'))"
                sql = String.Format(sql, itm, startTime, endTime)
                If ORALocalhost.SqlIsIn(sql) Then
                    Return New NormalResponse(False, "任务下发失败，该时段测试组内设备'" & itm & "'正忙")
                End If

            Next
            imei = JsonConvert.SerializeObject(list)
            groupImeilist = list
            phoneModel = JsonConvert.SerializeObject(modelList)
            userName = JsonConvert.SerializeObject(userNameList)
            deviceCount = list.Count
            isGroup = True
        Else
            sql = "select * from app_mission_table where imei like '%{0}%' and isClosed=0 and " &
                                                       "((StartTime between '{1}' and '{2}' or EndTime between '{1}' and '{2}') " &
                                                       "or (StartTime<='{1}' and EndTime>='{2}'))"
            sql = String.Format(sql, imei, startTime, endTime)
            If ORALocalhost.SqlIsIn(sql) Then
                Return New NormalResponse(False, "任务下发失败，该时段设备'" & imei & "'正忙")
            End If
            If userName = "" Then userName = GetUserNameByImei(imei)
            If phoneModel = "" Then phoneModel = GetPhoneModelByImei(imei)
            deviceCount = 1
        End If

        Dim dt As New DataTable
        Dim cols As String() = GetOraTableColumns("app_mission_table")
        If IsNothing(cols) Then Return New NormalResponse(False, "数据库中任务表不存在，请联系管理员")
        For Each col In cols
            If col <> "ID" Then
                dt.Columns.Add(col)
            End If
        Next
        dateTime = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim row As DataRow = dt.NewRow
        row("dateTime".ToUpper) = dateTime
        row("userName".ToUpper) = userName
        row("imei".ToUpper) = imei
        row("phoneModel".ToUpper) = phoneModel
        row("groupId".ToUpper) = groupId
        row("deviceCount".ToUpper) = deviceCount
        row("missionType".ToUpper) = missionType
        row("missionRemark".ToUpper) = missionRemark
        row("startTime".ToUpper) = startTime
        row("endTime".ToUpper) = endTime
        row("status".ToUpper) = "新建"
        row("isClosed".ToUpper) = -2
        row("LastDateTime".ToUpper) = dateTime
        row("isGroup".ToUpper) = IIf(isGroup, 1, 0)
        If isGroup Then
            If IsNothing(groupImeilist) = False Then
                Dim list As New List(Of MissionDog.TimeAndImei)
                For Each itm In groupImeilist
                    list.Add(New MissionDog.TimeAndImei(dateTime, itm, "未开启"))
                Next
                row("LastTimeAndImei".ToUpper) = JsonConvert.SerializeObject(list)
            End If
        End If
        dt.Rows.Add(row)
        Dim result As String = ORALocalhost.SqlCMDListQuickByPara("app_mission_table", dt)
        If result = "success" Then
            Return New NormalResponse(True, "任务添加成功！")
        Else
            Return New NormalResponse(False, "任务添加失败！" + result)
        End If
    End Function

    Private Shared Function GetPhoneModelByImei(imei As String) As String
        Dim sql As String = "select * from deviceTable where imei='" & imei & "'"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return ""
        If dt.Rows.Count = 0 Then Return ""
        Dim row As DataRow = dt.Rows(0)
        Dim str As String = row("PhoneModel".ToUpper).ToString
        If IsNothing(str) Then str = ""
        Return str
    End Function

    Private Shared Function GetUserNameByImei(imei As String) As String
        Dim sql As String = "select * from deviceTable where imei='" & imei & "'"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return ""
        If dt.Rows.Count = 0 Then Return ""
        Dim row As DataRow = dt.Rows(0)
        Dim str As String = row("userName".ToUpper).ToString
        If IsNothing(str) Then str = ""
        Return str
    End Function
End Class
