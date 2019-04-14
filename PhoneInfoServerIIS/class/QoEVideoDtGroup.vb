Public Class QoEVideoDtGroup
    Private Shared ora As OracleHelper = ORALocalhost
    Private Shared tableName As String = "QoE_Video_Dt_Group"
    Public id As Integer
    Public dateTime As String
    Public groupId As String
    Public city As String
    Public manager As String
    Public manager_tel As String
    Public manager_email As String
    Public status As String
    Public lastDateTime As String
    Public lastDay As String
    Public isWatching As Integer

    Public Function Create() As NormalResponse
        If groupId = "" Then Return New NormalResponse("groupId不可为空")

        If IsExist(True) Then
            Return New NormalResponse("该组id已存在")
        End If


        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("dateTime", Now.ToString("yyyy-MM-dd HH:mm:ss"))
        dik.Add("groupId", groupId)
        dik.Add("city", city)
        dik.Add("manager", manager)
        dik.Add("manager_tel", manager_tel)
        dik.Add("manager_email", manager_email)
        dik.Add("status", status)
        dik.Add("lastDateTime", Now.ToString("yyyy-MM-dd HH:mm:ss"))
        dik.Add("lastDay", Now.ToString("yyyy-MM-dd"))
        dik.Add("isWatching", 0)
        Return ora.InsertByDik(tableName, dik)
    End Function

    Private Function IsExist(ByVal Optional useGroupId As Boolean = False) As Boolean
        Dim sql As String = "select id from " & tableName & " where id=" & id & ""

        If useGroupId Then
            sql = "select id from " & tableName & " where groupId=" & groupId & ""
        End If

        Return ora.SqlIsIn(sql)
    End Function

    Public Shared Function [Get](ByVal Optional id As Integer = 0, ByVal Optional where As String = "") As QoEVideoDtGroup
        If id = 0 AndAlso where = "" Then Return Nothing
        Dim sql As String = "select * from " & tableName & (If(where = "", "", " where " & where))

        If id > 0 Then
            sql = "select * from " & tableName & " where id=" & id
        End If

        Dim dt As DataTable = ora.SqlGetDT(sql)
        If dt Is Nothing Then Return Nothing
        If dt.Rows.Count = 0 Then Return Nothing
        Dim row As DataRow = dt.Rows(0)
        Dim qoe As QoEVideoDtGroup = New QoEVideoDtGroup()
        qoe.id = Integer.Parse(row("ID").ToString())
        qoe.dateTime = row("dateTime").ToString()
        qoe.groupId = row("groupId").ToString()
        qoe.city = row("city").ToString()
        qoe.manager = row("manager").ToString()
        qoe.manager_tel = row("manager_tel").ToString()
        qoe.manager_email = row("manager_email").ToString()
        qoe.status = row("status").ToString()
        qoe.lastDateTime = row("lastDateTime").ToString()
        qoe.lastDay = row("lastDay").ToString()
        Integer.TryParse(row("isWatching").ToString(), qoe.isWatching)
        Return qoe
    End Function

    Public Shared Function SelectToList(ByVal Optional where As String = "") As List(Of QoEVideoDtGroup)
        Dim list As List(Of QoEVideoDtGroup) = New List(Of QoEVideoDtGroup)()
        Dim sql As String = "select * from " & tableName & (If(where = "", "", " where " & where))
        Dim dt As DataTable = ora.SqlGetDT(sql)
        If dt Is Nothing Then Return list
        If dt.Rows.Count = 0 Then Return list

        For Each row As DataRow In dt.Rows
            Dim qoe As QoEVideoDtGroup = New QoEVideoDtGroup()
            qoe.id = Integer.Parse(row("ID").ToString())
            qoe.dateTime = row("dateTime").ToString()
            qoe.groupId = row("groupId").ToString()
            qoe.city = row("city").ToString()
            qoe.manager = row("manager").ToString()
            qoe.manager_tel = row("manager_tel").ToString()
            qoe.manager_email = row("manager_email").ToString()
            qoe.status = row("status").ToString()
            qoe.lastDateTime = row("lastDateTime").ToString()
            qoe.lastDay = row("lastDay").ToString()
            Integer.TryParse(row("isWatching").ToString(), qoe.isWatching)
            list.Add(qoe)
        Next

        Return list
    End Function

    Public Function Update() As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")

        If Not IsExist() Then
            Return New NormalResponse("该组不存在")
        Else
            Dim qoe As QoEVideoDtGroup = [Get](0, "groupId='" & groupId & "'")

            If qoe IsNot Nothing Then

                If qoe.id <> Me.id Then
                    Return New NormalResponse("该组id已被使用")
                End If
            End If
        End If


        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("groupId", groupId)
        dik.Add("city", city)
        dik.Add("manager", manager)
        dik.Add("manager_tel", manager_tel)
        dik.Add("manager_email", manager_email)
        dik.Add("isWatching", isWatching)
        Return ora.UpdateByDik(tableName, dik, id)
    End Function

    Public Function UpdateAll() As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")

        If Not IsExist() Then
            Return New NormalResponse("该组不存在")
        Else
            Dim qoe As QoEVideoDtGroup = [Get](0, "groupId='" & groupId & "'")

            If qoe IsNot Nothing Then

                If qoe.id <> Me.id Then
                    Return New NormalResponse("该组id已被使用")
                End If
            End If
        End If


        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("groupId", groupId)
        dik.Add("city", city)
        dik.Add("manager", manager)
        dik.Add("manager_tel", manager_tel)
        dik.Add("manager_email", manager_email)
        dik.Add("status", status)
        dik.Add("lastDateTime", lastDateTime)
        dik.Add("lastDay", lastDay)
        dik.Add("isWatching", isWatching)
        Return ora.UpdateByDik(tableName, dik, id)
    End Function

    Public Shared Function Delete(ByVal id As Integer) As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")

        If [Get](id) Is Nothing Then
            Return New NormalResponse("该组不存在")
        End If

        Dim sql As String = "delete from " & tableName & " where id=" & id
        Dim result As String = ora.SqlCMD(sql)
        Return New NormalResponse(result)
    End Function

    Public Shared Function GetMembers(ByVal groupId As String) As NormalResponse
        If groupId = "" Then Return New NormalResponse("groupId不可为空")

        If [Get](0, "groupId='" & groupId & "'") Is Nothing Then
            Return New NormalResponse("该组id不存在")
        End If

        Dim list As List(Of QoEVideoDtGroupMember) = QoEVideoDtGroupMember.SelectToList("groupId='" & groupId & "'")
        Return New NormalResponse(True, "", "", list)
    End Function
End Class

Public Class QoEVideoDtGroupMember
    Private Shared ora As OracleHelper = ORALocalhost
    Private Shared tableName As String = "QoE_Video_Dt_Group_Member"
    Public id As Integer
    Public AID As String
    Public dateTime As String
    Public groupId As String
    Public name As String
    Public tel As String
    Public imei As String
    Public imsi As String
    Public carrier As String
    Public status As String
    Public lastDateTime As String
    Public lastDay As String
    Public qoe_total_time As Integer
    Public qoe_total_E_time As Integer
    Public qoe_today_time As Integer
    Public qoe_today_E_time As Integer
    Public qoe_vmos_match_total As Integer
    Public qoe_vmos_match_today As Integer
    Public lastPlayVideoUrl As String
    Public unEvmosCount As Integer

    Private Function IsAIDExistInDeviceTable(ByVal aid As String) As Boolean
        Dim sql As String = "select id from deviceTable where aid='" & aid & "'"
        Return ora.SqlIsIn(sql)
    End Function

    Public Function Create() As NormalResponse
        If AID Is Nothing Then Return New NormalResponse("AID不可为空")
        If AID = "" Then Return New NormalResponse("AID不可为空")
        If groupId = "" Then Return New NormalResponse("groupId不可为空")
        If name = "" Then Return New NormalResponse("name不可为空")

        If QoEVideoDtGroup.[Get](0, "groupId='" & groupId & "'") Is Nothing Then
            Return New NormalResponse("您选择的组id不存在")
        End If

        If IsExist(True) Then
            Return New NormalResponse("该AID号已存在成员列表中")
        End If

        If Not IsAIDExistInDeviceTable(AID) Then
            Return New NormalResponse("该AID号未在设备表中注册，无法使用")
        End If

        If [Get](0, "name='" & name & "'") IsNot Nothing Then
            Return New NormalResponse("该name已存在")
        End If


        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("dateTime", Now.ToString("yyyy-MM-dd HH:mm:ss"))
        dik.Add("groupId", groupId)
        dik.Add("name", name)
        dik.Add("tel", tel)
        dik.Add("AID", AID)
        dik.Add("lastDateTime", Now.ToString("yyyy-MM-dd HH:mm:ss"))
        dik.Add("lastDay", Now.ToString("yyyy-MM-dd"))
        Return ora.InsertByDik(tableName, dik)
    End Function

    Private Function IsExist(ByVal Optional useAid As Boolean = False) As Boolean
        Dim sql As String = "select id from " & tableName & " where id=" & id & ""

        If useAid Then
            sql = "select id from " & tableName & " where aid='" & AID & "'"
        End If

        Return ora.SqlIsIn(sql)
    End Function

    Public Shared Function SelectToList(ByVal Optional where As String = "") As List(Of QoEVideoDtGroupMember)
        Dim list As List(Of QoEVideoDtGroupMember) = New List(Of QoEVideoDtGroupMember)()
        Dim sql As String = "select * from " & tableName & (If(where = "", "", " where " & where))
        Dim dt As DataTable = ora.SqlGetDT(sql)
        If dt Is Nothing Then Return list
        If dt.Rows.Count = 0 Then Return list

        For Each row As DataRow In dt.Rows
            Dim qoe As QoEVideoDtGroupMember = New QoEVideoDtGroupMember()
            qoe.id = Integer.Parse(row("ID").ToString())
            qoe.AID = row("AID").ToString()
            qoe.dateTime = row("dateTime").ToString()
            qoe.groupId = row("groupId").ToString()
            qoe.name = row("name").ToString()
            qoe.tel = row("tel").ToString()
            qoe.imei = row("imei").ToString()
            qoe.imsi = row("imsi").ToString()
            qoe.carrier = row("carrier").ToString()
            qoe.status = row("status").ToString()
            qoe.lastDateTime = row("lastDateTime").ToString()
            qoe.lastDay = row("lastDay").ToString()
            qoe.lastPlayVideoUrl = row("lastPlayVideoUrl").ToString()
            Integer.TryParse(row("qoe_total_time").ToString(), qoe.qoe_total_time)
            Integer.TryParse(row("qoe_total_E_time").ToString(), qoe.qoe_total_E_time)
            Integer.TryParse(row("qoe_today_time").ToString(), qoe.qoe_today_time)
            Integer.TryParse(row("qoe_today_E_time").ToString(), qoe.qoe_today_E_time)
            Integer.TryParse(row("qoe_vmos_match_total").ToString(), qoe.qoe_vmos_match_total)
            Integer.TryParse(row("qoe_vmos_match_today").ToString(), qoe.qoe_vmos_match_today)
            Integer.TryParse(row("unEvmosCount").ToString(), qoe.unEvmosCount)
            list.Add(qoe)
        Next

        Return list
    End Function

    Public Function Update() As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")
        If AID Is Nothing Then Return New NormalResponse("AID不可为空")
        If AID = "" Then Return New NormalResponse("AID不可为空")
        If groupId = "" Then Return New NormalResponse("groupId不可为空")
        If name = "" Then Return New NormalResponse("name不可为空")

        If QoEVideoDtGroup.[Get](0, "groupId='" & groupId & "'") Is Nothing Then
            Return New NormalResponse("您选择的组id不存在")
        End If

        If Not IsExist() Then
            Return New NormalResponse("该成员不存在")
        End If

        Dim qoe As QoEVideoDtGroupMember = [Get](0, "AID='" & AID & "'")

        If qoe IsNot Nothing Then

            If qoe.id <> Me.id Then
                Return New NormalResponse("该AID已被使用")
            End If
        End If

        If Not IsAIDExistInDeviceTable(AID) Then
            Return New NormalResponse("该AID号未在设备表中注册，无法使用")
        End If

        qoe = [Get](0, "name='" & name & "'")

        If qoe IsNot Nothing Then

            If qoe.id <> Me.id Then
                Return New NormalResponse("该name已被使用")
            End If
        End If


        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("groupId", groupId)
        dik.Add("name", name)
        dik.Add("tel", tel)
        dik.Add("aid", AID)
        Return ora.UpdateByDik(tableName, dik, id)
    End Function

    Public Function UpdateAll() As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")
        If imsi = "" Then Return New NormalResponse("imsi不可为空")
        If groupId = "" Then Return New NormalResponse("groupId不可为空")

        If QoEVideoDtGroup.[Get](0, "groupId='" & groupId & "'") Is Nothing Then
            Return New NormalResponse("您选择的组id不存在")
        End If

        If Not IsExist() Then
            Return New NormalResponse("该成员不存在")
        End If

        Dim qoe As QoEVideoDtGroupMember = [Get](0, "aid='" & AID & "'")

        If qoe IsNot Nothing Then

            If qoe.id <> Me.id Then
                Return New NormalResponse("该AID已被使用")
            End If
        End If

        qoe = [Get](0, "name='" & name & "'")

        If qoe IsNot Nothing Then

            If qoe.id <> Me.id Then
                Return New NormalResponse("该name已被使用")
            End If
        End If

        Dim dik As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
        dik.Add("groupId", groupId)
        dik.Add("name", name)
        dik.Add("tel", tel)
        dik.Add("imei", imei)
        dik.Add("imsi", imsi)
        dik.Add("aid", AID)
        dik.Add("carrier", carrier)
        dik.Add("status", status)
        dik.Add("lastDateTime", lastDateTime)
        dik.Add("lastDay", lastDay)
        dik.Add("qoe_total_time", qoe_total_time)
        dik.Add("qoe_total_E_time", qoe_total_E_time)
        dik.Add("qoe_today_time", qoe_today_time)
        dik.Add("qoe_today_E_time", qoe_today_E_time)
        dik.Add("qoe_vmos_match_total", qoe_vmos_match_total)
        dik.Add("qoe_vmos_match_today", qoe_vmos_match_today)
        dik.Add("lastPlayVideoUrl", lastPlayVideoUrl)
        dik.Add("unEvmosCount", unEvmosCount)
        Return ora.UpdateByDik(tableName, dik, id)
    End Function

    Public Shared Function [Get](ByVal Optional id As Integer = 0, ByVal Optional where As String = "") As QoEVideoDtGroupMember
        If id = 0 AndAlso where = "" Then Return Nothing
        Dim sql As String = "select * from " & tableName & (If(where = "", "", " where " & where))

        If id > 0 Then
            sql = "select * from " & tableName & " where id=" & id
        End If

        Dim dt As DataTable = ora.SqlGetDT(sql)
        If dt Is Nothing Then Return Nothing
        If dt.Rows.Count = 0 Then Return Nothing
        Dim row As DataRow = dt.Rows(0)
        Dim qoe As QoEVideoDtGroupMember = New QoEVideoDtGroupMember()
        qoe.id = Integer.Parse(row("ID").ToString())
        qoe.AID = row("AID").ToString()
        qoe.dateTime = row("dateTime").ToString()
        qoe.groupId = row("groupId").ToString()
        qoe.name = row("name").ToString()
        qoe.tel = row("tel").ToString()
        qoe.imei = row("imei").ToString()
        qoe.imsi = row("imsi").ToString()
        qoe.carrier = row("carrier").ToString()
        qoe.status = row("status").ToString()
        qoe.lastDateTime = row("lastDateTime").ToString()
        qoe.lastDay = row("lastDay").ToString()
        qoe.lastPlayVideoUrl = row("lastPlayVideoUrl").ToString()
        Integer.TryParse(row("qoe_total_time").ToString(), qoe.qoe_total_time)
        Integer.TryParse(row("qoe_total_E_time").ToString(), qoe.qoe_total_E_time)
        Integer.TryParse(row("qoe_today_time").ToString(), qoe.qoe_today_time)
        Integer.TryParse(row("qoe_today_E_time").ToString(), qoe.qoe_today_E_time)
        Integer.TryParse(row("qoe_vmos_match_total").ToString(), qoe.qoe_vmos_match_total)
        Integer.TryParse(row("qoe_vmos_match_today").ToString(), qoe.qoe_vmos_match_today)
        Integer.TryParse(row("unEvmosCount").ToString(), qoe.unEvmosCount)
        Return qoe
    End Function

    Public Shared Function Delete(ByVal id As Integer) As NormalResponse
        If id = 0 Then Return New NormalResponse("id不可为0")

        If [Get](id) Is Nothing Then
            Return New NormalResponse("该成员不存在")
        End If

        Dim sql As String = "delete from " & tableName & " where id=" & id
        Dim result As String = ora.SqlCMD(sql)
        Return New NormalResponse(result)
    End Function

    Public Shared Function GetGroupIdByImsi(ByVal imsi As String) As String
        If imsi = "" Then Return ""
        Dim qoe As QoEVideoDtGroupMember = [Get](0, "imsi='" & imsi & "'")
        If qoe Is Nothing Then Return ""
        Return qoe.groupId
    End Function

    Public Shared Sub OnUpdateQoEVideoInfo(ByVal qoe As QoEVideoInfo)
        Try
            Dim imsi As String = qoe.IMSI
            If imsi = "" Then Return
            Dim aid As String = qoe.pi.AID
            If aid = "" Then Return
            Dim mem As QoEVideoDtGroupMember = [Get](0, "aid='" & aid & "'")
            If mem Is Nothing Then Return
            Dim groupId As String = mem.groupId
            Dim group As QoEVideoDtGroup = QoEVideoDtGroup.[Get](0, "groupId='" & groupId & "'")
            If group Is Nothing Then Return

            Dim nowTime As String = now.ToString("yyyy-MM-dd HH:mm:ss")
            Dim nowDay As String = now.ToString("yyyy-MM-dd")
            Dim lastDay As String = mem.lastDay
            Dim isToday As Boolean = (lastDay = now.ToString("yyyy-MM-dd"))

            If Not isToday Then
                mem.qoe_today_E_time = 0
                mem.qoe_today_time = 0
                mem.qoe_vmos_match_today = 0
            End If

            Dim vmos_match As Integer = GetVmosMatch(qoe.EVMOS, qoe.VMOS)
            mem.qoe_total_time += 1
            mem.qoe_today_time += 1

            If qoe.EVMOS > 0 Then
                mem.qoe_vmos_match_today = (If(mem.qoe_vmos_match_today = 0, vmos_match, (mem.qoe_vmos_match_today + vmos_match) / 2))
                mem.qoe_vmos_match_total = (If(mem.qoe_vmos_match_total = 0, vmos_match, (mem.qoe_vmos_match_total + vmos_match) / 2))
                mem.qoe_total_E_time += 1
                mem.qoe_today_E_time += 1
                mem.unEvmosCount = 0
            Else
                mem.unEvmosCount += 1
            End If

            mem.lastPlayVideoUrl = qoe.FILE_NAME
            mem.imsi = qoe.IMSI
            mem.imei = qoe.IMEI
            mem.carrier = qoe.CARRIER
            mem.lastDateTime = nowTime
            mem.lastDay = nowDay
            mem.status = "正常"
            mem.UpdateAll()
            group.lastDateTime = nowTime
            group.lastDay = nowDay
            group.status = "正常"
            group.UpdateAll()
        Catch e As Exception
        End Try
    End Sub

    Private Shared Function GetVmosMatch(ByVal evmos As Integer, ByVal vmos As Integer) As Integer
        If evmos = 0 Then Return 0
        Dim vmos_match As Integer = 100 * (1 - (Math.Abs(evmos - vmos) / 4))
        Return vmos_match
    End Function
End Class