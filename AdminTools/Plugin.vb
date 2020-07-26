Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Exiled.API.Features
Imports Handlers = Exiled.Events.Handlers

Namespace AdminTools
    Public Class Plugin
        Inherits Plugin(Of Config)

        Public Overrides ReadOnly Property Author As String = "Galaxy119"
        Public Overrides ReadOnly Property Name As String = "Admin Tools"
        Public Overrides ReadOnly Property Prefix As String = "AT"
        Public Overrides ReadOnly Property Version As Version = New Version(1, 4, 0)
        Public Overrides ReadOnly Property RequiredExiledVersion As Version = New Version(2, 0, 0)
        Public EventHandlers As EventHandlers
        Public JailedPlayers As List(Of Jailed) = New List(Of Jailed)()
        Public Shared IkHubs As Dictionary(Of Player, InstantKillComponent) = New Dictionary(Of Player, InstantKillComponent)()
        Public Shared BdHubs As Dictionary(Of Player, BreakDoorComponent) = New Dictionary(Of Player, BreakDoorComponent)()
        Public OverwatchFilePath As String
        Public HiddenTagsFilePath As String

        Public Overrides Sub OnEnabled()
            Try
                Dim appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                Dim pluginPath = IO.Path.Combine(appData, "Plugins")
                Dim path = IO.Path.Combine(pluginPath, "AdminTools")
                Dim overwatchFileName = IO.Path.Combine(path, "AdminTools-Overwatch.txt")
                Dim hiddenTagFileName = IO.Path.Combine(path, "AdminTools-HiddenTags.txt")
                If Not Directory.Exists(path) Then Directory.CreateDirectory(path)
                If Not File.Exists(overwatchFileName) Then File.Create(overwatchFileName).Close()
                If Not File.Exists(hiddenTagFileName) Then File.Create(hiddenTagFileName).Close()
                OverwatchFilePath = overwatchFileName
                HiddenTagsFilePath = hiddenTagFileName
                EventHandlers = New EventHandlers(Me)
                AddHandler Handlers.Server.SendingRemoteAdminCommand, AddressOf EventHandlers.OnCommand
                AddHandler Handlers.Player.Joined, AddressOf EventHandlers.OnPlayerJoin
                AddHandler Handlers.Server.RoundEnded, AddressOf EventHandlers.OnRoundEnd
                AddHandler Handlers.Player.TriggeringTesla, AddressOf EventHandlers.OnTriggerTesla
                AddHandler Handlers.Player.ChangingRole, AddressOf EventHandlers.OnSetClass
                AddHandler Handlers.Server.WaitingForPlayers, AddressOf EventHandlers.OnWaitingForPlayers
            Catch e As Exception
                Log.Error($"Loading error: {e}")
            End Try
        End Sub

        Public Overrides Sub OnDisabled()
            RemoveHandler Handlers.Server.SendingRemoteAdminCommand, AddressOf EventHandlers.OnCommand
            RemoveHandler Handlers.Player.Joined, AddressOf EventHandlers.OnPlayerJoin
            RemoveHandler Handlers.Server.RoundEnded, AddressOf EventHandlers.OnRoundEnd
            RemoveHandler Handlers.Player.TriggeringTesla, AddressOf EventHandlers.OnTriggerTesla
            RemoveHandler Handlers.Player.ChangingRole, AddressOf EventHandlers.OnSetClass
            RemoveHandler Handlers.Server.WaitingForPlayers, AddressOf EventHandlers.OnWaitingForPlayers
            EventHandlers = Nothing
        End Sub

        Public Overrides Sub OnReloaded()
        End Sub
    End Class
End Namespace
