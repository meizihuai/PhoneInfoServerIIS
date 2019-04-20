Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Sockets
Imports System.Web
Imports System.Threading
Imports System.Threading.Thread
Public Class MissionDog
    Private Shared mThread As Thread
    Private Shared sleepSecond As Integer = 5
    Private Shared appMissionTableLock As New Object
    '说明 app_mission_table isClosed =-2(新建)  -1(未开启)  0(正常)  1(结束）
    Public Shared Sub StartWatching()
        StopWatching()
        Try
            mThread = New Thread(AddressOf Watching)
            mThread.Start()
        Catch ex As Exception

        End Try
    End Sub
    Public Shared Sub StopWatching()
        If IsNothing(mThread) = False Then
            Try
                mThread.Abort()
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Shared Sub Watching()
        While True
            Try
                WatchingWork()
            Catch ex As Exception
                'Dim path As String = "d:\WatchingDogErr.txt"
                'File.WriteAllText(path, ex.ToString)
            End Try
            Sleep(1000 * sleepSecond)
        End While
    End Sub
    Private Shared Sub WatchingWork()
        Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim lastDateTime As String = Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")
        '将endTime到期的任务标注为 结束
        Dim sql As String = "update app_mission_table set isClosed=1,status='结束' where endTime<='" & time & "' and isClosed=0 "
        ORALocalhost.SqlCMD(sql)
        '将startTime还没到时间点的任务标注为 未开启
        sql = "update app_mission_table set status='未开启',isClosed=-1 where  startTime>'" & time & "'"
        ORALocalhost.SqlCMD(sql)
        '将正在执行的任务任务标注为 正常  
        sql = "update app_mission_table set status='正常',isClosed=0 where startTime<='" & time & "' and endTime>='" & time & "'"
        ORALocalhost.SqlCMD(sql)
        '将设备库更改设备是否繁忙 isbusy
        sql = "select imei,isGroup from app_mission_table where isClosed=0"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) = False Then
            If dt.Rows.Count > 0 Then
                Dim imeiList As New List(Of String)
                For Each row As DataRow In dt.Rows
                    Dim imei As String = row("IMEI").ToString
                    Dim isGroup As Integer = Val(row("isGroup".ToUpper).ToString)
                    If IsNothing(imei) = False Then
                        If imei <> "" Then
                            If isGroup Then
                                Try
                                    Dim list As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(imei)
                                    If IsNothing(list) = False Then
                                        For Each itm In list
                                            If Not imeiList.Contains(itm) Then
                                                imeiList.Add(itm)
                                            End If
                                        Next
                                    End If
                                Catch ex As Exception

                                End Try
                            Else
                                imeiList.Add(imei)
                            End If
                        End If
                    End If
                Next
                If imeiList.Count > 0 Then
                    sql = "select imei from deviceTable"
                    Dim imeiDt As DataTable = ORALocalhost.SqlGetDT(sql)
                    If IsNothing(imeiDt) = False Then
                        If imeiDt.Rows.Count > 0 Then
                            For Each row In imeiDt.Rows
                                Dim imei As String = row("IMEI").ToString
                                If imeiList.Contains(imei) Then
                                    sql = "update deviceTable set isBusy=1 where imei='" & imei & "' and isBusy=0"
                                    ORALocalhost.SqlCMD(sql)
                                Else
                                    sql = "update deviceTable set isBusy=0 where imei='" & imei & "' and isBusy=1"
                                    ORALocalhost.SqlCMD(sql)
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        End If
        '下面将对正在执行的任务进行监控
        WatchingSingleDeviceMission(lastDateTime)  '监控单个设备的任务
        WatchingGroupMission(lastDateTime)
    End Sub
    '监控单个设备的任务
    Private Shared Sub WatchingSingleDeviceMission(lastDateTime As String)
        Dim sql As String = "update app_mission_table set lastDateTime=dateTime where lastDateTime is null "
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='异常' where isClosed=0 and lastDateTime<='" & lastDateTime & "'   and isGroup=0"
        ORALocalhost.SqlCMD(sql)
        sql = "update app_mission_table set status='正常' where isClosed=0 and lastDateTime>'" & lastDateTime & "'   and isGroup=0"
        ORALocalhost.SqlCMD(sql)
    End Sub
    '监控测试组的任务
    Private Shared Sub WatchingGroupMission(lastDateTime As String)
        Dim sql As String = "select * from app_mission_table where isClosed=0 and isGroup=1"
        Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
        If IsNothing(dt) Then Return
        If dt.Rows.Count = 0 Then Return
        For Each row As DataRow In dt.Rows
            Dim id As String = row("id".ToUpper).ToString
            Dim status As String = row("status".ToUpper).ToString
            Dim oraLastDataTime As String = row("lastDateTime".ToUpper).ToString
            Dim laseTime As String = row("LastTimeAndImei".ToUpper).ToString
            Dim totalStatus As String = "正常"
            If IsNothing(laseTime) = False Then
                If IsDBNull(laseTime) = False Then
                    If laseTime <> "" Then
                        Try
                            Dim list As List(Of TimeAndImei) = JsonConvert.DeserializeObject(Of List(Of TimeAndImei))(laseTime)
                            If IsNothing(list) = False Then
                                For i = 0 To list.Count - 1
                                    Dim itm As TimeAndImei = list(i)
                                    If itm.time <= lastDateTime Then
                                        itm.status = "异常"
                                        totalStatus = "异常"
                                    Else
                                        itm.status = "正常"
                                    End If
                                    list(i) = itm
                                Next
                            End If
                            laseTime = JsonConvert.SerializeObject(list)
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If
            sql = "update app_mission_table set status='{0}' , LastTimeAndImei='{1}'  where id=" & id
            sql = String.Format(sql, totalStatus, laseTime)
            ORALocalhost.SqlCMD(sql)
        Next
    End Sub
    Public Structure TimeAndImei
        Dim time As String
        Dim imei As String
        Dim status As String
        Sub New(time As String, imei As String, status As String)
            Me.time = time
            Me.imei = imei
            Me.status = status
        End Sub
    End Structure
    Public Shared Sub OnDeviceDataCome(pi As PhoneInfo)
        Dim time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim sql As String = "update app_mission_table set lastDateTime='" & time & "' where imei='" & pi.IMEI & "' and isClosed=0 and isgroup=0"
        ORALocalhost.SqlCMD(sql)
        SyncLock appMissionTableLock
            sql = "select * from app_mission_table where isClosed=0 and isgroup=1"
            Dim dt As DataTable = ORALocalhost.SqlGetDT(sql)
            If IsNothing(dt) Then Return
            If dt.Rows.Count = 0 Then Return
            For Each row As DataRow In dt.Rows
                Dim imei As String = row("IMEI").ToString
                Dim id As String = row("ID").ToString
                If IsNothing(imei) = False Then
                    Try
                        Dim list As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(imei)
                        If IsNothing(list) = False Then
                            If list.Contains(pi.IMEI) Then
                                Dim laseTime As String = row("LastTimeAndImei").ToString()
                                If IsDBNull(laseTime) = False Then
                                    If laseTime <> "" Then
                                        Try
                                            Dim timeList As List(Of TimeAndImei) = JsonConvert.DeserializeObject(Of List(Of TimeAndImei))(laseTime)
                                            Dim isAdd As Boolean = True
                                            If IsNothing(timeList) = False Then
                                                For i = 0 To timeList.Count - 1
                                                    Dim itm As TimeAndImei = timeList(i)
                                                    If itm.imei = pi.IMEI Then
                                                        itm.time = time
                                                        'itm.status = "正常"
                                                        isAdd = False
                                                        timeList(i) = itm
                                                        Exit For
                                                    End If
                                                Next
                                            Else
                                                timeList = New List(Of TimeAndImei)
                                            End If
                                            If isAdd Then timeList.Add(New TimeAndImei(time, pi.IMEI, "正常"))
                                            laseTime = JsonConvert.SerializeObject(timeList)
                                        Catch ex As Exception

                                        End Try
                                    End If
                                End If
                                sql = "update app_mission_table set lastDateTime='" & time & "',LastTimeAndImei='" & laseTime & "' where id=" & id
                                Dim result As String = ORALocalhost.SqlCMD(sql)

                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Next
        End SyncLock
    End Sub
End Class
