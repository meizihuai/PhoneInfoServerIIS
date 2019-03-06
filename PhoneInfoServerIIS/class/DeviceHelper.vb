
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
    Public Shared Sub ChangeDeviceStatus(pi As PhoneInfo)
        If IsNothing(pi) Then Return
        If IsNothing(pi.IMEI) Then Return
        If pi.IMEI = "" Then Return
        Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "select * from deviceTable where imei='" & pi.IMEI & "'"
        Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
        If isExist Then
            sql = "update deviceTable set {0} GroupId='{1}',lastDateTime='{2}',isBusy='{3}',
                  province='{4}',city='{5}',district='{6}',lon='{7}',lat='{8}',bdlon='{9}',bdlat='{10}',gdlon='{11}',gdlat='{12}',apkVersion='{13}' where imei='" & pi.IMEI & "'"
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
            sql = String.Format(sql, list.ToArray())
            Dim result As String = ORALocalhost.SqlCMD(sql)
        Else
            sql = "insert into deviceTable (DateTime,userName,imei,GroupId,lastDateTime,isBusy,province,city,district,lon,lat,bdlon,bdlat,gdlon,gdlat,power,PHONEMODEL,apkVersion) values ('{0}','{1}','{2}','{3}','{4}',
                    '{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')"
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
            sql = String.Format(sql, list.ToArray())
            Dim result As String = ORALocalhost.SqlCMD(sql)
        End If
        sql = "update app_mission_table set lastDateTime='" & time & "' where imei='" & pi.IMEI & "' and isClosed=0"
        ORALocalhost.SqlCMD(sql)
    End Sub
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
        Catch ex As Exception
            Return New NormalResponse(False, "开始时间或结束时间格式非法")
        End Try

        Dim sql As String = "select * from app_mission_table where  imei='{0}' and isClosed=0 and " &
                                                       "((StartTime between '{1}' and '{2}' or EndTime between '{1}' and '{2}') " &
                                                       "or (StartTime<='{1}' and EndTime>='{2}'))"
        sql = String.Format(sql, imei, startTime, endTime)
        Dim isExist As Boolean = ORALocalhost.SqlIsIn(sql)
        If isExist Then
            Return New NormalResponse(False, "该时段设备正忙")
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
        deviceCount = 1
        If groupId <> "" Then
            '查询groupId表

        End If
        Dim row As DataRow = dt.NewRow
        row("dateTime".ToUpper) = dateTime
        row("userName".ToUpper) = GetUserNameByImei(imei)
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
        row("PhoneModel".ToUpper) = GetPhoneModelByImei(imei)
        row("LastDateTime".ToUpper) = dateTime
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
