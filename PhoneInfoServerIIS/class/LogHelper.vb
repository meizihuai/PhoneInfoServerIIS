Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.IO


Public Class LogHelper
        Public Shared rootPath As String = HttpContext.Current.Server.MapPath("/Logs/")
        Private Shared lc As Object = New Object()

        Public Shared Sub CheckDir()
            Dim dir As DirectoryInfo = New DirectoryInfo(rootPath)

            If Not dir.Exists Then
                dir.Create()
            End If
        End Sub

        Public Shared Sub Log(ByVal content As String, ByVal Optional tagName As String = "default")
            SyncLock lc

                Try
                    CheckDir()
                    Dim now As DateTime = DateTime.Now
                    Dim str As String = now.ToString("[HH:mm:ss] ") & "<" & tagName & "> " & content & vbCrLf
                    Dim filePath As String = rootPath & now.ToString("yyyy_MM_dd") & ".txt"
                    File.AppendAllText(filePath, str)
                Catch e As Exception
                End Try
            End SyncLock
        End Sub
    End Class

