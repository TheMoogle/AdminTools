Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Exiled.API.Features
Imports Exiled.Events.EventArgs
Imports Exiled.Permissions.Extensions
Imports GameCore
Imports Grenades
Imports MEC
Imports Mirror
Imports RemoteAdmin
Imports UnityEngine
Imports Log = Exiled.API.Features.Log
Imports [Object] = UnityEngine.[Object]

Namespace AdminTools
    Public Class EventHandlers
        Private ReadOnly plugin As AdminTools.Plugin

        Public Sub New(ByVal plugin As AdminTools.Plugin)
            CSharpImpl.__Assign(Me.plugin, plugin)
        End Sub

        Public Sub OnCommand(ByVal ev As Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs)
            Dim result As Single = Nothing, result As UInteger = Nothing, result As UInteger = Nothing, x As Single = Nothing, y As Single = Nothing, z As Single = Nothing, newPos As Single = Nothing, scale As Single = Nothing, x As Single = Nothing, y As Single = Nothing, z As Single = Nothing, x As Single = Nothing, y As Single = Nothing, z As Single = Nothing, size As Single = Nothing, x As Single = Nothing, y As Single = Nothing, z As Single = Nothing, count As Integer = Nothing, role As Integer = Nothing, result As Integer = Nothing, __ As AdminTools.InstantKillComponent = Nothing, ikComponent As AdminTools.InstantKillComponent = Nothing, bdComponent As AdminTools.BreakDoorComponent = Nothing, doorBreak As AdminTools.BreakDoorComponent = Nothing, bdComponent As AdminTools.BreakDoorComponent = Nothing, doorBreaker As AdminTools.BreakDoorComponent = Nothing

            Try
                If ev.Name.Contains("REQUEST_DATA PLAYER_LIST") Then Return
                Dim appData As String = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)
                Dim scpFolder As String = System.IO.Path.Combine(appData, "SCP Secret Laboratory")
                Dim logs As String = System.IO.Path.Combine(scpFolder, "AdminLogs")
                Dim fileName As String = System.IO.Path.Combine(logs, $"command_log-{ServerConsole.Port}.txt")
                If Not System.IO.Directory.Exists(logs) Then System.IO.Directory.CreateDirectory(logs)
                If Not System.IO.File.Exists(fileName) Then System.IO.File.Create(CStr((fileName))).Close()
                Dim data As String = $"{System.DateTime.Now}: {ev.Sender.Nickname} ({ev.Sender.Id}) executed: {ev.Name} {System.Environment.NewLine}"
                System.IO.File.AppendAllText(fileName, data)
                Dim args As String() = ev.Arguments.ToArray()
                Dim sender As Exiled.API.Features.Player = ev.Sender

                Select Case ev.Name
                    Case "kick"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.kick") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        Dim reasons As System.Collections.Generic.IEnumerable(Of String) = args.Where(Function(s) Not Equals(s, args(0)) AndAlso Not Equals(s, args(1)))
                        Dim reason As String = ""

                        For Each st As String In reasons
                            reason += st
                        Next

                        Dim obj As UnityEngine.GameObject = Exiled.API.Features.Player.[Get](args(1))?.GameObject

                        If obj Is DirectCast(Nothing, Global.UnityEngine.[Object]) Then
                            ev.ReplyMessage = ("Player not found")
                            ev.Success = False
                            Return
                        End If

                        ServerConsole.Disconnect(obj, $"You have been kicked from the server: {reason}")
                        ev.ReplyMessage = ("Player was kicked.")
                        Return
                    Case "muteall"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.mute") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                            If Not player.ReferenceHub.serverRoles.RemoteAdmin Then player.IsMuted = True
                        Next

                        ev.ReplyMessage = ("All non-staff players have been muted.")
                        Return
                    Case "unmuteall"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.mute") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                            If Not player.ReferenceHub.serverRoles.RemoteAdmin Then player.IsMuted = False
                        Next

                        ev.ReplyMessage = ("All non-staff players have been muted.")
                        Return
                    Case "rocket"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.rocket") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                        If player Is DirectCast(Nothing, System.Object) AndAlso Not Equals(args(1), "*") AndAlso Not Equals(args(1), "all") Then
                            ev.ReplyMessage = ("Player not found.")
                            Return
                        End If

                        If Not Single.TryParse(args(2), result) Then
                            ev.ReplyMessage = ($"Speed argument invalid: {args(2)}")
                            Return
                        End If

                        If Equals(args(1), "*") OrElse Equals(args(1), "all") Then
                            For Each h As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                MEC.Timing.RunCoroutine(Me.DoRocket(h, result))
                            Next
                        Else
                            MEC.Timing.RunCoroutine(Me.DoRocket(player, result))
                        End If

                        ev.ReplyMessage = ("We're going on a trip, in our favorite rocketship.")
                        Return
                    Case "bc"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.bc") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        Dim thing As System.Collections.Generic.IEnumerable(Of String) = args.Skip(2)
                        Dim msg As String = ""

                        For Each s As String In thing
                            msg += $"{s} "
                        Next

                        Dim time As UInteger = UInteger.Parse(args(1))

                        For Each p As UnityEngine.GameObject In PlayerManager.players
                            p.GetComponent(Of Broadcast)().TargetAddElement(p.GetComponent(Of Scp049_2PlayerScript)().connectionToClient, msg, CUShort(time), Broadcast.BroadcastFlags.Normal)
                        Next

                        ev.ReplyMessage = ("Broadcast Sent.")
                        Exit Select
                    Case "id"
                        ev.IsAllowed = False
                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))
                        Dim id As String = If(player Is DirectCast(Nothing, System.Object), "Player not found", player.UserId)
                        ev.ReplyMessage = ($"{player.Nickname} - {id}")
                        Exit Select
                    Case "pbc"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.bc") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 4 Then
                            ev.ReplyMessage = ("You must provide a players name/id, a number in seconds to display the broadcast, and a message")
                            ev.Success = False
                            Exit Select
                        End If

                        If Not UInteger.TryParse(args(2), result) Then
                            ev.ReplyMessage = ("You must provide a valid integer for a duration.")
                            ev.Success = False
                            Exit Select
                        End If

                        Dim thing As System.Collections.Generic.IEnumerable(Of String) = args.Skip(3)
                        Dim msg As String = ""

                        For Each s As String In thing
                            msg += $"{s} "
                        Next

                        Exiled.API.Features.Player.[Get](args(1))?.Broadcast(CUShort(result), msg, Broadcast.BroadcastFlags.Normal)
                        ev.ReplyMessage = ("Message sent.")
                        Exit Select
                    Case "tut"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.tut") Then
                            ev.ReplyMessage = ("Permission denied.")
                            ev.Success = False
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("You must supply a player name or ID")
                            ev.Success = False
                            Return
                        End If

                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](String.Join(" ", args.Skip(1)))

                        If player Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ("Player not found.")
                            ev.Success = False
                            Return
                        End If

                        If player.Role <> RoleType.Tutorial Then
                            MEC.Timing.RunCoroutine(Me.DoTut(player))
                            ev.ReplyMessage = ("Player set as tutorial.")
                        Else
                            ev.ReplyMessage = ("Player unset as Tutorial (killed).")
                            player.Role = RoleType.Spectator
                        End If

                        Exit Select
                    Case "hidetags"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.tags") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List

                            If player.ReferenceHub.serverRoles.RemoteAdmin Then
                                player.BadgeHidden = True
                            End If
                        Next

                        ev.ReplyMessage = ("All staff tags hidden.")
                    Case "showtags"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.tags") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List

                            If player.ReferenceHub.serverRoles.RemoteAdmin AndAlso Not player.ReferenceHub.serverRoles.RaEverywhere Then
                                player.BadgeHidden = False
                            End If
                        Next

                        ev.ReplyMessage = ("All staff tags shown.")
                    Case "jail"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.jail") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("You must supply a player name or ID")
                            ev.Success = False
                            Return
                        End If

                        Dim array As System.Collections.Generic.IEnumerable(Of String) = args.Where(Function(a) Not Equals(a, args(0)))
                        Dim filter As String = DirectCast(Nothing, System.String)

                        For Each s As String In array
                            filter += s
                        Next

                        Dim target As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](filter)

                        If target Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ("User not found.")
                            ev.Success = False
                            Return
                        End If

                        If Me.plugin.JailedPlayers.Any(Function(j) Equals(j.Userid, target.UserId)) Then
                            MEC.Timing.RunCoroutine(Me.DoUnJail(target))
                            ev.ReplyMessage = ("User unjailed.")
                        Else
                            MEC.Timing.RunCoroutine(Me.DoJail(target))
                            ev.ReplyMessage = ("User jailed.")
                        End If

                        Exit Select
                    Case "abc"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.bc") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("You must include a duration and a message.")
                            ev.Success = False
                            Return
                        End If

                        If Not UInteger.TryParse(args(1), result) Then
                            ev.ReplyMessage = ("You must provide a valid integer for a duration.")
                            ev.Success = False
                            Exit Select
                        End If

                        Dim thing2 As System.Collections.Generic.IEnumerable(Of String) = args.Skip(2)
                        Dim msg As String = ""

                        For Each s As String In thing2
                            msg += $"{s} "
                        Next

                        For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                            If player.ReferenceHub.serverRoles.RemoteAdmin Then player.Broadcast(CUShort(result), $"{ev.Sender.Nickname}: {msg}", Broadcast.BroadcastFlags.AdminChat)
                        Next

                        ev.ReplyMessage = ("Message sent to all online staff members.")
                        Exit Select
                    Case "drop"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.items") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        Dim result As Integer

                        If args.Length <> 4 Then
                            ev.ReplyMessage = ($"Invalid arguments.{args.Length}")
                            Exit Select
                        End If

                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                        If player Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ("Player not found.")
                            Exit Select
                        End If

                        Dim item As ItemType = CType(System.[Enum].Parse(GetType(ItemType), args(2)), ItemType)

                        If Not Integer.TryParse(args(3), result) Then
                            ev.ReplyMessage = ("Not a number doufus.")
                            Exit Select
                        End If

                        If result > 200 Then
                            ev.ReplyMessage = ("Try a lower number that won't crash my servers, ty.")
                            Return
                        End If

                        For i As Integer = 0 To result - 1
                            Me.SpawnItem(item, player.Position, UnityEngine.Vector3.zero)
                        Next

                        ev.ReplyMessage = ("Done. hehexd")
                        Return
                    Case "pos"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.tp") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("You must supply a player name/ID and a subcommand.")
                            ev.Success = False
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players

                            Select Case args(CInt((2))).ToLower()
                                Case "set"

                                    If args.Length < 6 Then
                                        ev.ReplyMessage = ("You must supply x, y and z coordinated.")
                                        ev.Success = False
                                        Return
                                    End If

                                    If Not Single.TryParse(args(3), x) Then
                                        ev.ReplyMessage = ("Invalid x coordinates.")
                                        Return
                                    End If

                                    If Not Single.TryParse(args(4), y) Then
                                        ev.ReplyMessage = ("Invalid y coordinates.")
                                        Return
                                    End If

                                    If Not Single.TryParse(args(5), z) Then
                                        ev.ReplyMessage = ("Invalid z coordinates.")
                                        Return
                                    End If

                                    player.Position = New UnityEngine.Vector3(x, y, z)
                                    ev.ReplyMessage = ($"Player {player.Nickname} - {player.UserId} moved to x{x} y{y} z{z}")
                                    Exit Select
                                Case "get"
                                    Dim pos As UnityEngine.Vector3 = player.Position
                                    Dim ret As String = $"{player.Nickname} - {player.UserId} Position: x {pos.x} y {pos.y} z {pos.z}"
                                    ev.ReplyMessage = (ret)
                                    Exit Select
                                Case "add"

                                    If Not Equals(args(3), "x") AndAlso Not Equals(args(3), "y") AndAlso Not Equals(args(3), "z") Then
                                        ev.ReplyMessage = ("Invalid coordinate plane selected.")
                                        Return
                                    End If

                                    If Not Single.TryParse(args(4), newPos) Then
                                        ev.ReplyMessage = ("Invalid coordinate.")
                                        Return
                                    End If

                                    Dim pos As UnityEngine.Vector3 = player.Position

                                    Select Case args(CInt((3))).ToLower()
                                        Case "x"
                                            player.Position = New UnityEngine.Vector3(pos.x + newPos, pos.y, pos.z)
                                        Case "y"
                                            player.Position = New UnityEngine.Vector3(pos.x, pos.y + newPos, pos.z)
                                        Case "z"
                                            player.Position = New UnityEngine.Vector3(pos.x, pos.y, pos.z + newPos)
                                    End Select

                                    ev.ReplyMessage = ($"Player {player.Nickname} - {player.UserId} position changed.")
                                    Exit Select
                            End Select
                        Next

                        Exit Select
                    Case "tpx"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.tp") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("You must supply a player name/ID to teleport and a player name/ID to teleport them to.")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        Dim target As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(2))

                        If target Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ($"Player {args(2)} not found.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            player.Position = target.Position
                            ev.ReplyMessage = ($"{player.Nickname} teleported to {target.Nickname}")
                        Next

                        Exit Select
                    Case "ghost"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.ghost") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("You must supply a playername to ghost.")
                            ev.Success = False
                            Return
                        End If

                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                        If player Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ("Player not found.")
                            ev.Success = False
                            Return
                        End If

                        If player.IsInvisible Then
                            player.IsInvisible = False
                            ev.ReplyMessage = ($"{player.Nickname} removed from ghostmode.")
                            Return
                        End If

                        player.IsInvisible = True
                        ev.ReplyMessage = ($"{player.Nickname} ghosted.")
                        Return
                    Case "scale"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.size") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("You must provide a target and scale size.")
                            Return
                        End If

                        If Not Single.TryParse(args(2), scale) Then
                            ev.ReplyMessage = ("Invalid scale size selected.")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            Me.SetPlayerScale(player.GameObject, scale)
                            ev.ReplyMessage = ($"{player.Nickname} size set to {scale}")
                        Next

                        Return
                    Case "size"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.size") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 5 Then
                            ev.ReplyMessage = ("You must provide a target, x size, y size and z size.")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(2), x) Then
                            ev.ReplyMessage = ($"Invalid x size: {args(2)}")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(3), y) Then
                            ev.ReplyMessage = ($"Invalid y size: {args(3)}")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(4), z) Then
                            ev.ReplyMessage = ($"Invalid z size: {args(4)}")
                            ev.Success = False
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            Me.SetPlayerScale(player.GameObject, x, y, z)
                            ev.ReplyMessage = ($"{player.Nickname}'s size has been changed.")
                        Next

                        Return
                    Case "spawnworkbench"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.benches") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 5 Then
                            ev.ReplyMessage = ("Invalid number of arguments.")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(2), x) Then
                            ev.ReplyMessage = ($"Invalid x size: {args(2)}")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(3), y) Then
                            ev.ReplyMessage = ($"Invalid y size: {args(3)}")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(4), z) Then
                            ev.ReplyMessage = ($"Invalid z size: {args(4)}")
                            ev.Success = False
                            Return
                        End If

                        Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                        If player Is DirectCast(Nothing, System.Object) Then
                            ev.ReplyMessage = ($"Player not found: {args(1)}")
                            ev.Success = False
                            Return
                        End If

                        Me.SpawnWorkbench(player.Position + player.ReferenceHub.PlayerCameraReference.forward * 2, player.GameObject.transform.rotation.eulerAngles, New UnityEngine.Vector3(x, y, z))
                        ev.ReplyMessage = ($"Ahh, yes. Enslaved game code.")
                        Return
                    Case "drops"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.items") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 4 Then
                            ev.ReplyMessage = ("haha no, try again with correct arguments 4head")
                            Return
                        End If

                        If Not Single.TryParse(args(3), size) Then
                            ev.ReplyMessage = ("Invalid size")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        Dim item As ItemType = CType(System.[Enum].Parse(GetType(ItemType), args(2)), ItemType)

                        For Each player As Exiled.API.Features.Player In players
                            Dim yesnt As Pickup = player.Inventory.SetPickup(item, -4.656647E+11F, player.Position, UnityEngine.Quaternion.identity, 0, 0, 0)
                            Dim gameObject As UnityEngine.GameObject = yesnt.gameObject
                            gameObject.transform.localScale = UnityEngine.Vector3.one * size
                            Mirror.NetworkServer.UnSpawn(gameObject)
                            Mirror.NetworkServer.Spawn(yesnt.gameObject)
                        Next

                        ev.ReplyMessage = ($"Yay, items! With sizes!!")
                        Return
                    Case "dummy"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.dummy") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 6 Then
                            ev.ReplyMessage = ("You must supply a player, dummy role, x size, y size and z size")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        Dim role As RoleType = RoleType.None

                        Try
                            role = CType(System.[Enum].Parse(GetType(RoleType), args(2)), RoleType)
                        Catch __unusedException1__ As System.Exception
                            ev.ReplyMessage = ($"Invalid role selected: {args(2)}")
                            ev.Success = False
                            Return
                        End Try

                        If role = RoleType.None Then
                            ev.ReplyMessage = ("Cannot spawn a dummy without a role.")
                            ev.Success = False
                            Return
                        End If

                        If Not Single.TryParse(args(3), x) Then
                            ev.ReplyMessage = ("Invalid x value.")
                            Return
                        End If

                        If Not Single.TryParse(args(4), y) Then
                            ev.ReplyMessage = ("Invalid y value.")
                            Return
                        End If

                        If Not Single.TryParse(args(5), z) Then
                            ev.ReplyMessage = ("Invalid z value.")
                            Return
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            Me.SpawnDummyModel(player.Position, player.GameObject.transform.localRotation, role, x, y, z)
                        Next

                        ev.ReplyMessage = ("Dummy(s) spawned.")
                        Exit Select
                    Case "ragdoll"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.dolls") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 4 Then
                            ev.ReplyMessage = ("Try again")
                            Return
                        End If

                        If Not Integer.TryParse(args(3), count) Then
                            ev.ReplyMessage = ("Invalid number selected.")
                            Return
                        End If

                        If Not Integer.TryParse(args(2), role) Then
                            ev.ReplyMessage = ("Invalid roleID")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        ev.ReplyMessage = ("hehexd")

                        For Each player As Exiled.API.Features.Player In players
                            MEC.Timing.RunCoroutine(Me.SpawnBodies(player, role, count))
                        Next

                        Return
                    Case "config"

                        If Equals(args(CInt((1))).ToLower(), "reload") Then
                            ev.IsAllowed = False
                            ServerStatic.PermissionsHandler.RefreshPermissions()
                            GameCore.ConfigFile.ReloadGameConfigs()
                            ev.ReplyMessage = ($"Config files reloaded.")
                        End If

                        Return
                    Case "hp"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.hp") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("You must supply a player name/ID and an amount.")
                            ev.Success = False
                            Return
                        End If

                        If Not Integer.TryParse(args(2), result) Then
                            ev.ReplyMessage = ($"Invalid health amount: {args(2)}")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            If result > player.MaxHealth Then player.MaxHealth = result
                            player.Health = result
                            ev.ReplyMessage = ($"{player.Nickname} ({player.UserId}'s health has been set to {result}")
                        Next

                        Return
                    Case "cleanup"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.cleanup") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("You must supply a type of cleanup: items or ragdolls.")
                            ev.Success = False
                            Return
                        End If

                        If Equals(args(CInt((1))).ToLower(), "items") Then
                            For Each item As Pickup In UnityEngine.[Object].FindObjectsOfType(Of Pickup)()
                                item.Delete()
                            Next
                        ElseIf Equals(args(CInt((1))).ToLower(), "ragdolls") Then

                            For Each doll As Ragdoll In UnityEngine.[Object].FindObjectsOfType(Of Ragdoll)()
                                Mirror.NetworkServer.Destroy(doll.gameObject)
                            Next
                        End If

                        ev.ReplyMessage = ("Cleanup complete.")
                        Return
                    Case "grenade"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.grenade") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ($"Too few arguments. Value: {args.Length}, Expected 3")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        Select Case args(CInt((2))).ToLower()
                            Case "frag"

                                For Each player As Exiled.API.Features.Player In players
                                    Dim gm As Grenades.GrenadeManager = player.ReferenceHub.GetComponent(Of Grenades.GrenadeManager)()
                                    Dim grenade As Grenades.GrenadeSettings = gm.availableGrenades.FirstOrDefault(Function(g) g.inventoryID = ItemType.GrenadeFrag)

                                    If grenade Is DirectCast(Nothing, System.Object) Then
                                        ev.ReplyMessage = ($"Something broke that really really <b>really</b> shouldn't have.. Notify Joker with the following error code: GS-NRE")
                                        ev.Success = False
                                        Return
                                    End If

                                    Dim component As Grenades.Grenade = UnityEngine.[Object].Instantiate(grenade.grenadeInstance).GetComponent(Of Grenades.Grenade)()
                                    component.InitData(gm, UnityEngine.Vector3.zero, UnityEngine.Vector3.zero, 0F)
                                    Mirror.NetworkServer.Spawn(component.gameObject)
                                Next

                                ev.ReplyMessage = ("Tick, tick.. BOOM!")
                            Case "flash"

                                For Each player As Exiled.API.Features.Player In players
                                    Dim gm As Grenades.GrenadeManager = player.ReferenceHub.GetComponent(Of Grenades.GrenadeManager)()
                                    Dim grenade As Grenades.GrenadeSettings = gm.availableGrenades.FirstOrDefault(Function(g) g.inventoryID = ItemType.GrenadeFlash)

                                    If grenade Is DirectCast(Nothing, System.Object) Then
                                        ev.ReplyMessage = ($"Something broke that really really <b>really</b> shouldn't have.. Notify Joker with the following error code: GS-NRE")
                                        ev.Success = False
                                        Return
                                    End If

                                    Dim component As Grenades.Grenade = UnityEngine.[Object].Instantiate(grenade.grenadeInstance).GetComponent(Of Grenades.Grenade)()
                                    component.InitData(gm, UnityEngine.Vector3.zero, UnityEngine.Vector3.zero, 0F)
                                    Mirror.NetworkServer.Spawn(component.gameObject)
                                Next

                                ev.ReplyMessage = ("Don't look at the light!")
                            Case "ball"

                                For Each player As Exiled.API.Features.Player In players
                                    Dim spawnrand As UnityEngine.Vector3 = New UnityEngine.Vector3(UnityEngine.Random.Range(0F, 2F), UnityEngine.Random.Range(0F, 2F), UnityEngine.Random.Range(0F, 2F))
                                    Dim gm As Grenades.GrenadeManager = player.ReferenceHub.GetComponent(Of Grenades.GrenadeManager)()
                                    Dim ball As Grenades.GrenadeSettings = gm.availableGrenades.FirstOrDefault(Function(g) g.inventoryID = ItemType.SCP018)

                                    If ball Is DirectCast(Nothing, System.Object) Then
                                        ev.ReplyMessage = ($"TheMoogle broke something in his code that shouldn't have been.. Notify Joker with the error code: Mog's Balls don't work")
                                        ev.Success = False
                                        Return
                                    End If

                                    Dim component As Grenades.Grenade = UnityEngine.[Object].Instantiate(ball.grenadeInstance).GetComponent(Of Grenades.Scp018Grenade)()
                                    component.InitData(gm, spawnrand, UnityEngine.Vector3.zero)
                                    Mirror.NetworkServer.Spawn(component.gameObject)
                                Next

                                ev.ReplyMessage = ("The Balls started bouncing!")
                            Case Else
                                ev.ReplyMessage = ("Enter either ""frag"", ""flash"" or ""ball"".")
                        End Select

                        Exit Select
                    Case "ball"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.ball") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ($"Too few arguments. Value: {args.Length}, Expected 2")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next

                            Exiled.API.Features.Cassie.Message("pitch_1.5 xmas_bouncyballs", True, False)
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            Dim spawnrand As UnityEngine.Vector3 = New UnityEngine.Vector3(UnityEngine.Random.Range(0F, 2F), UnityEngine.Random.Range(0F, 2F), UnityEngine.Random.Range(0F, 2F))
                            Dim gm As Grenades.GrenadeManager = player.ReferenceHub.GetComponent(Of Grenades.GrenadeManager)()
                            Dim ball As Grenades.GrenadeSettings = gm.availableGrenades.FirstOrDefault(Function(g) g.inventoryID = ItemType.SCP018)

                            If ball Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ($"TheMoogle broke something in his code that shouldn't have been.. Notify Joker with the error code: Mog's Balls don't work")
                                ev.Success = False
                                Return
                            End If

                            Dim component As Grenades.Grenade = UnityEngine.[Object].Instantiate(ball.grenadeInstance).GetComponent(Of Grenades.Scp018Grenade)()
                            component.InitData(gm, spawnrand, UnityEngine.Vector3.zero)
                            Mirror.NetworkServer.Spawn(component.gameObject)
                        Next

                        ev.ReplyMessage = ("The Balls started bouncing!")
                        Exit Select
                    Case "kill"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.kill") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        Dim players As System.Collections.Generic.List(Of Exiled.API.Features.Player) = New System.Collections.Generic.List(Of Exiled.API.Features.Player)()

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then players.Add(player)
                            Next
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ("Player not found.")
                                ev.Success = False
                                Return
                            End If

                            players.Add(player)
                        End If

                        For Each player As Exiled.API.Features.Player In players
                            Dim id As Integer = player.Id
                            player.ReferenceHub.playerStats.HurtPlayer(New PlayerStats.HitInfo(119000000, ev.Sender.Nickname, DamageTypes.Wall, id), player.GameObject)
                            ev.ReplyMessage = ($"{player.Nickname} has been slayed.")
                        Next

                        Exit Select
                    Case "inv"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.inv") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 3 Then
                            ev.ReplyMessage = ("Please provide a removal command and id")
                            Return
                        End If

                        Select Case args(CInt((1))).ToLower()
                            Case "drop"

                                If Equals(args(CInt((2))).ToLower(), "*") OrElse Equals(args(CInt((2))).ToLower(), "all") Then
                                    For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                        If player.Role <> RoleType.Spectator Then player.Inventory.ServerDropAll()
                                    Next

                                    ev.ReplyMessage = ("Dropped all items in everyone's inventory")
                                Else
                                    Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(2))

                                    If player Is DirectCast(Nothing, System.Object) Then
                                        ev.ReplyMessage = ($"Player {args(2)} not found")
                                        Return
                                    End If

                                    player.Inventory.ServerDropAll()
                                    ev.ReplyMessage = ($"Dropped all items in {player.Nickname}'s inventory")
                                End If

                            Case "see"
                                Dim ply As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(2))

                                If ply Is DirectCast(Nothing, System.Object) Then
                                    ev.ReplyMessage = ($"Player {args(2)} not found")
                                    Return
                                End If

                                If ply.Inventory.items.Count <> 0 Then
                                    Dim itemLister As String = $"Player {ply.Nickname} has the following items in their inventory (in order): "

                                    For Each item As Inventory.SyncItemInfo In ply.Inventory.items
                                        itemLister += item.id & ", "
                                    Next

                                    itemLister = itemLister.Substring(0, itemLister.Count() - 2)
                                    ev.ReplyMessage = (itemLister)
                                    Return
                                End If

                                ev.ReplyMessage = ($"Player {ply.Nickname} does not have any items in their inventory")
                            Case Else
                                ev.ReplyMessage = ("Please enter either ""clear"", ""drop"", or ""see""")
                        End Select

                        Exit Select
                    Case "ik"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.ik") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("Please provide a id")
                            Return
                        End If

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List

                                If Not player.ReferenceHub.TryGetComponent(__) Then
                                    player.ReferenceHub.gameObject.AddComponent(Of AdminTools.InstantKillComponent)()
                                End If
                            Next

                            ev.ReplyMessage = ("Instant killing is on for all players now")
                        ElseIf Equals(args(CInt((1))).ToLower(), "list") Then

                            If AdminTools.Plugin.IkHubs.Count <> 0 Then
                                Dim playerLister As String = "Players with instant kill on: "

                                For Each player As Exiled.API.Features.Player In AdminTools.Plugin.IkHubs.Keys
                                    playerLister += player.Nickname & ", "
                                Next

                                playerLister = playerLister.Substring(0, playerLister.Count() - 2)
                                ev.ReplyMessage = (playerLister)
                                Return
                            End If

                            ev.ReplyMessage = ("No players currently online have instant killing on")
                        ElseIf Equals(args(CInt((1))).ToLower(), "clear") Then

                            For Each player As Exiled.API.Features.Player In AdminTools.Plugin.IkHubs.Keys
                                UnityEngine.[Object].Destroy(player.ReferenceHub.GetComponent(Of AdminTools.InstantKillComponent)())
                            Next

                            ev.ReplyMessage = ("Instant killing is off for all players now")
                        Else
                            Dim ply As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If ply Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ($"Player {args(1)} not found")
                                Return
                            End If

                            If Not ply.ReferenceHub.TryGetComponent(ikComponent) Then
                                ply.GameObject.AddComponent(Of AdminTools.InstantKillComponent)()
                                ev.ReplyMessage = ($"Instant killing is on for {ply.Nickname}")
                            Else
                                UnityEngine.[Object].Destroy(ikComponent)
                                ev.ReplyMessage = ($"Instant killing is off for {ply.Nickname}")
                            End If
                        End If

                        Exit Select
                    Case "bd"
                        ev.IsAllowed = False

                        If Not sender.CheckPermission("at.bd") Then
                            ev.ReplyMessage = ("Permission denied.")
                            Return
                        End If

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("Please provide a break command and an id (if needed) (Note: For ""list"" and ""clear"" you do not need an id)")
                            Return
                        End If

                        Select Case args.Length
                            Case 2

                                Select Case args(CInt((1))).ToLower()
                                    Case "list"

                                        If AdminTools.Plugin.BdHubs.Count <> 0 Then
                                            Dim playerLister As String = "Players with break doors permissions on: "

                                            For Each player As Exiled.API.Features.Player In AdminTools.Plugin.BdHubs.Keys
                                                playerLister += player.Nickname & ", "
                                            Next

                                            playerLister = playerLister.Substring(0, playerLister.Count() - 2)
                                            ev.ReplyMessage = (playerLister)
                                            Return
                                        End If

                                        ev.ReplyMessage = ("No players currently online have break door permissions on")
                                    Case "clear"

                                        For Each player As Exiled.API.Features.Player In AdminTools.Plugin.BdHubs.Keys
                                            UnityEngine.[Object].Destroy(player.ReferenceHub.GetComponent(Of AdminTools.BreakDoorComponent)())
                                        Next

                                        ev.ReplyMessage = ("Break door permissions is off for all players now")
                                    Case Else
                                        ev.ReplyMessage = ("Please enter either ""all"", ""clear"", ""doors"", or ""list""")
                                End Select

                            Case 3

                                Select Case args(CInt((1))).ToLower()
                                    Case "doors"

                                        If Equals(args(CInt((2))).ToLower(), "*") OrElse Equals(args(CInt((2))).ToLower(), "all") Then
                                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List

                                                If Not player.ReferenceHub.TryGetComponent(bdComponent) Then
                                                    bdComponent = player.GameObject.AddComponent(Of AdminTools.BreakDoorComponent)()
                                                    AdminTools.Plugin.BdHubs.Add(player, bdComponent)
                                                Else
                                                    bdComponent.breakAll = False
                                                End If
                                            Next

                                            ev.ReplyMessage = ("Instant breaking of doors is on for all players now")
                                            Return
                                        End If

                                        Dim ply As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(2))

                                        If ply Is DirectCast(Nothing, System.Object) Then
                                            ev.ReplyMessage = ($"Player {args(2)} not found")
                                            Return
                                        End If

                                        If Not ply.ReferenceHub.TryGetComponent(doorBreak) Then
                                            ev.ReplyMessage = ($"Instant breaking of doors is on for {ply.Nickname}")
                                            doorBreak = ply.GameObject.AddComponent(Of AdminTools.BreakDoorComponent)()
                                            AdminTools.Plugin.BdHubs.Add(ply, doorBreak)
                                            doorBreak.breakAll = False
                                        Else

                                            If doorBreak.breakAll Then
                                                ev.ReplyMessage = ($"Instant breaking of doors is on for {ply.Nickname}")
                                                doorBreak.breakAll = False
                                                Return
                                            End If

                                            ev.ReplyMessage = ($"Instant breaking of doors is off for {ply.Nickname}")
                                            UnityEngine.[Object].Destroy(doorBreak)
                                            AdminTools.Plugin.BdHubs.Remove(ply)
                                        End If

                                    Case "all"

                                        If Equals(args(CInt((2))).ToLower(), "*") OrElse Equals(args(CInt((2))).ToLower(), "all") Then
                                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List

                                                If Not player.ReferenceHub.TryGetComponent(bdComponent) Then
                                                    bdComponent = player.GameObject.AddComponent(Of AdminTools.BreakDoorComponent)()
                                                    bdComponent.breakAll = True
                                                    AdminTools.Plugin.BdHubs.Add(player, bdComponent)
                                                Else
                                                    bdComponent.breakAll = True
                                                End If
                                            Next

                                            ev.ReplyMessage = ("Instant breaking of everything is on for all players now")
                                            Return
                                        End If

                                        ply = Exiled.API.Features.Player.[Get](args(2))

                                        If ply Is DirectCast(Nothing, System.Object) Then
                                            ev.ReplyMessage = ($"Player {args(2)} not found")
                                            Return
                                        End If

                                        If Not ply.ReferenceHub.TryGetComponent(doorBreaker) Then
                                            ev.ReplyMessage = ($"Instant breaking of everything is on for {ply.Nickname}")
                                            doorBreak = ply.GameObject.AddComponent(Of AdminTools.BreakDoorComponent)()
                                            doorBreak.breakAll = True
                                            AdminTools.Plugin.BdHubs.Add(ply, doorBreak)
                                        Else

                                            If Not doorBreaker.breakAll Then
                                                ev.ReplyMessage = ($"Instant breaking of everything is on for {ply.Nickname}")
                                                doorBreaker.breakAll = True
                                                Return
                                            End If

                                            ev.ReplyMessage = ($"Instant breaking of everything is off for {ply.Nickname}")
                                            UnityEngine.[Object].Destroy(doorBreaker)
                                            AdminTools.Plugin.BdHubs.Remove(ply)
                                        End If

                                    Case Else
                                        ev.ReplyMessage = ("Please enter either ""all"", ""clear"", ""doors"", or ""list""")
                                End Select
                        End Select

                    Case "strip"
                        If Not RemoteAdmin.CommandProcessor.CheckPermissions(New RemoteAdmin.PlayerCommandSender(ev.Sender.ReferenceHub.queryProcessor), args(CInt((0))).ToUpper(), PlayerPermissions.PlayersManagement, "AdminTools", False) Then Return
                        ev.IsAllowed = False

                        If args.Length < 2 Then
                            ev.ReplyMessage = ("Syntax: strip ((id/name)/*/all)")
                            Return
                        End If

                        If Equals(args(CInt((1))).ToLower(), "*") OrElse Equals(args(CInt((1))).ToLower(), "all") Then
                            For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                                If player.Role <> RoleType.Spectator Then player.ClearInventory()
                            Next

                            ev.ReplyMessage = ("Cleared all items in everyone's inventory")
                        Else
                            Dim player As Exiled.API.Features.Player = Exiled.API.Features.Player.[Get](args(1))

                            If player Is DirectCast(Nothing, System.Object) Then
                                ev.ReplyMessage = ($"Player {args(1)} not found")
                                Return
                            End If

                            player.ClearInventory()
                            ev.ReplyMessage = ($"Cleared all items in {player.Nickname}'s inventory")
                        End If
                End Select

            Catch e As System.Exception
                Exiled.API.Features.Log.[Error]($"Handling command error: {e}")
            End Try
        End Sub

        Private Sub SpawnDummyModel(ByVal position As UnityEngine.Vector3, ByVal rotation As UnityEngine.Quaternion, ByVal role As RoleType, ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Dim obj As UnityEngine.GameObject = UnityEngine.[Object].Instantiate(Mirror.NetworkManager.singleton.spawnPrefabs.FirstOrDefault(Function(p) Equals(p.gameObject.name, "Player")))
            Dim ccm As CharacterClassManager = obj.GetComponent(Of CharacterClassManager)()
            If ccm Is DirectCast(Nothing, Global.UnityEngine.[Object]) Then Exiled.API.Features.Log.[Error]("CCM is null, doufus. You need to do this the harder way.")
            ccm.CurClass = role
            ccm.RefreshPlyModel()
            obj.GetComponent(Of NicknameSync)().Network_myNickSync = "Dummy"
            obj.GetComponent(Of RemoteAdmin.QueryProcessor)().PlayerId = 9999
            obj.GetComponent(Of RemoteAdmin.QueryProcessor)().NetworkPlayerId = 9999
            obj.transform.localScale = New UnityEngine.Vector3(x, y, z)
            obj.transform.position = position
            obj.transform.rotation = rotation
            Mirror.NetworkServer.Spawn(obj)
        End Sub

        Private Iterator Function SpawnBodies(ByVal player As Exiled.API.Features.Player, ByVal role As Integer, ByVal count As Integer) As System.Collections.Generic.IEnumerator(Of Single)
            For i As Integer = 0 To count - 1
                player.GameObject.GetComponent(Of RagdollManager)().SpawnRagdoll(player.Position + UnityEngine.Vector3.up * 5, UnityEngine.Quaternion.identity, UnityEngine.Vector3.zero, role, New PlayerStats.HitInfo(1000F, player.UserId, DamageTypes.Falldown, player.Id), False, "SCP-343", "SCP-343", 0)
                Yield MEC.Timing.WaitForSeconds(0.15F)
            Next
        End Function

        Public Sub SpawnWorkbench(ByVal position As UnityEngine.Vector3, ByVal rotation As UnityEngine.Vector3, ByVal size As UnityEngine.Vector3)
            Dim bench As UnityEngine.GameObject = UnityEngine.[Object].Instantiate(Mirror.NetworkManager.singleton.spawnPrefabs.Find(Function(p) Equals(p.gameObject.name, "Work Station")))
            Dim offset As Offset = New Offset()
            offset.position = position
            offset.rotation = rotation
            offset.scale = UnityEngine.Vector3.one
            bench.gameObject.transform.localScale = size
            Mirror.NetworkServer.Spawn(bench)
            bench.GetComponent(Of WorkStation)().Networkposition = offset
            bench.AddComponent(Of WorkStationUpgrader)()
        End Sub

        Public Sub SpawnItem(ByVal type As ItemType, ByVal pos As UnityEngine.Vector3, ByVal rot As UnityEngine.Vector3)
            PlayerManager.localPlayer.GetComponent(Of Inventory)().SetPickup(type, -4.656647E+11F, pos, UnityEngine.Quaternion.Euler(rot), 0, 0, 0)
        End Sub

        Private Iterator Function DoTut(ByVal player As Exiled.API.Features.Player) As System.Collections.Generic.IEnumerator(Of Single)
            If player.IsOverwatchEnabled Then player.IsOverwatchEnabled = False
            player.Role = RoleType.Tutorial
            Yield MEC.Timing.WaitForSeconds(1F)
            Dim d As Door() = UnityEngine.[Object].FindObjectsOfType(Of Door)()

            For Each door As Door In d

                If Equals(door.DoorName, "SURFACE_GATE") Then
                    player.Position = door.transform.position + UnityEngine.Vector3.up * 2
                    Exit For
                End If
            Next

            player.ReferenceHub.serverRoles.CallTargetSetNoclipReady(player.ReferenceHub.characterClassManager.connectionToClient, True)
            player.ReferenceHub.serverRoles.NoclipReady = True
        End Function

        Public Sub SetPlayerScale(ByVal target As UnityEngine.GameObject, ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Try
                Dim identity As Mirror.NetworkIdentity = target.GetComponent(Of Mirror.NetworkIdentity)()
                target.transform.localScale = New UnityEngine.Vector3(1 * x, 1 * y, 1 * z)
                Dim destroyMessage As Mirror.ObjectDestroyMessage = New Mirror.ObjectDestroyMessage()
                destroyMessage.netId = identity.netId

                For Each player As UnityEngine.GameObject In PlayerManager.players
                    Dim playerCon As Mirror.NetworkConnection = player.GetComponent(Of Mirror.NetworkIdentity)().connectionToClient
                    If player IsNot target Then playerCon.Send(destroyMessage, 0)
                    Dim parameters As Object() = New Object() {identity, playerCon}
                    GetType(Mirror.NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters)
                Next

            Catch e As System.Exception
                Exiled.API.Features.Log.Info($"Set Scale error: {e}")
            End Try
        End Sub

        Public Sub SetPlayerScale(ByVal target As UnityEngine.GameObject, ByVal scale As Single)
            Try
                Dim identity As Mirror.NetworkIdentity = target.GetComponent(Of Mirror.NetworkIdentity)()
                target.transform.localScale = UnityEngine.Vector3.one * scale
                Dim destroyMessage As Mirror.ObjectDestroyMessage = New Mirror.ObjectDestroyMessage()
                destroyMessage.netId = identity.netId

                For Each player As UnityEngine.GameObject In PlayerManager.players
                    If player Is target Then Continue For
                    Dim playerCon As Mirror.NetworkConnection = player.GetComponent(Of Mirror.NetworkIdentity)().connectionToClient
                    playerCon.Send(destroyMessage, 0)
                    Dim parameters As Object() = New Object() {identity, playerCon}
                    GetType(Mirror.NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters)
                Next

            Catch e As System.Exception
                Exiled.API.Features.Log.Info($"Set Scale error: {e}")
            End Try
        End Sub

        Public Iterator Function DoRocket(ByVal player As Exiled.API.Features.Player, ByVal speed As Single) As System.Collections.Generic.IEnumerator(Of Single)
            Const maxAmnt As Integer = 50
            Dim amnt As Integer = 0

            While player.Role <> RoleType.Spectator
                player.Position = player.Position + UnityEngine.Vector3.up * speed
                amnt += 1

                If amnt >= maxAmnt Then
                    player.IsGodModeEnabled = False
                    player.ReferenceHub.playerStats.HurtPlayer(New PlayerStats.HitInfo(1000000, "WORLD", DamageTypes.Grenade, 0), player.GameObject)
                End If

                Yield MEC.Timing.WaitForOneFrame
            End While
        End Function

        Public Iterator Function DoJail(ByVal player As Exiled.API.Features.Player, ByVal Optional skipadd As Boolean = False) As System.Collections.Generic.IEnumerator(Of Single)
            Dim items As System.Collections.Generic.List(Of ItemType) = New System.Collections.Generic.List(Of ItemType)()

            For Each item As Inventory.SyncItemInfo In player.Inventory.items
                items.Add(item.id)
            Next

            If Not skipadd Then Me.plugin.JailedPlayers.Add(New AdminTools.Jailed With {
                .Health = player.Health,
                .Position = player.Position,
                .Items = items,
                .Name = player.Nickname,
                .Role = player.Role,
                .Userid = player.UserId
            })
            If player.IsOverwatchEnabled Then player.IsOverwatchEnabled = False
            Yield MEC.Timing.WaitForSeconds(1F)
            player.Role = RoleType.Tutorial
            player.Position = New UnityEngine.Vector3(53F, 1020F, -44F)
            player.Inventory.items.Clear()
        End Function

        Private Iterator Function DoUnJail(ByVal player As Exiled.API.Features.Player) As System.Collections.Generic.IEnumerator(Of Single)
            Dim jail As AdminTools.Jailed = Me.plugin.JailedPlayers.Find(Function(j) Equals(j.Userid, player.UserId))
            player.Role = jail.Role

            For Each item As ItemType In jail.Items
                player.Inventory.AddNewItem(item)
            Next

            Yield MEC.Timing.WaitForSeconds(1.5F)
            player.Health = jail.Health
            player.Position = jail.Position
            Me.plugin.JailedPlayers.Remove(jail)
        End Function

        Public Sub OnPlayerJoin(ByVal ev As Exiled.Events.EventArgs.JoinedEventArgs)
            Try
                If Me.plugin.JailedPlayers.Any(Function(j) Equals(j.Userid, ev.Player.UserId)) Then MEC.Timing.RunCoroutine(Me.DoJail(ev.Player, True))

                If System.IO.File.ReadAllText(CStr((Me.plugin.OverwatchFilePath))).Contains(ev.Player.UserId) Then
                    Exiled.API.Features.Log.Debug($"Putting {ev.Player.UserId} into overwatch.")
                    ev.Player.IsOverwatchEnabled = True
                End If

                If System.IO.File.ReadAllText(CStr((Me.plugin.HiddenTagsFilePath))).Contains(ev.Player.UserId) Then
                    Exiled.API.Features.Log.Debug($"Hiding {ev.Player.UserId}'s tag.")
                    ev.Player.BadgeHidden = True
                End If

            Catch e As System.Exception
                Exiled.API.Features.Log.[Error]($"Player Join: {e}")
            End Try
        End Sub

        Public Sub OnRoundEnd(ByVal ev As Exiled.Events.EventArgs.RoundEndedEventArgs)
            Try
                Dim overwatchRead As System.Collections.Generic.List(Of String) = System.IO.File.ReadAllLines(Me.plugin.OverwatchFilePath).ToList()
                Dim tagsRead As System.Collections.Generic.List(Of String) = System.IO.File.ReadAllLines(Me.plugin.HiddenTagsFilePath).ToList()

                For Each player As Exiled.API.Features.Player In Exiled.API.Features.Player.List
                    Dim userId As String = player.UserId

                    If player.IsOverwatchEnabled AndAlso Not overwatchRead.Contains(userId) Then
                        overwatchRead.Add(userId)
                    ElseIf Not player.IsOverwatchEnabled AndAlso overwatchRead.Contains(userId) Then
                        overwatchRead.Remove(userId)
                    End If

                    If player.ReferenceHub.serverRoles._hideLocalBadge AndAlso Not tagsRead.Contains(userId) Then
                        tagsRead.Add(userId)
                    ElseIf Not player.ReferenceHub.serverRoles._hideLocalBadge AndAlso tagsRead.Contains(userId) Then
                        tagsRead.Remove(userId)
                    End If
                Next

                For Each s As String In overwatchRead
                    Exiled.API.Features.Log.Debug($"{s} is in overwatch.")
                Next

                For Each s As String In tagsRead
                    Exiled.API.Features.Log.Debug($"{s} has their tag hidden.")
                Next

                System.IO.File.WriteAllLines(Me.plugin.OverwatchFilePath, overwatchRead)
                System.IO.File.WriteAllLines(Me.plugin.HiddenTagsFilePath, tagsRead)
            Catch e As System.Exception
                Exiled.API.Features.Log.[Error]($"Round End: {e}")
            End Try
        End Sub

        Public Sub OnTriggerTesla(ByVal ev As Exiled.Events.EventArgs.TriggeringTeslaEventArgs)
            If ev.Player.IsGodModeEnabled Then ev.IsTriggerable = False
        End Sub

        Public Sub OnSetClass(ByVal ev As Exiled.Events.EventArgs.ChangingRoleEventArgs)
            If Me.plugin.Config.GodTuts Then ev.Player.IsGodModeEnabled = ev.NewRole = RoleType.Tutorial
        End Sub

        Public Sub OnWaitingForPlayers()
            AdminTools.Plugin.IkHubs.Clear()
            AdminTools.Plugin.BdHubs.Clear()
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
