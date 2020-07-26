Imports Exiled.API.Features
Imports Exiled.Events.EventArgs
Imports UnityEngine
Imports Handlers = Exiled.Events.Handlers

Namespace AdminTools
    Public Class BreakDoorComponent
        Inherits MonoBehaviour

        Public player As Player
        Public breakAll = False
        Private unbreakableDoorNames = {"079_FIRST", "079_SECOND", "372", "914", "CHECKPOINT_ENT", "CHECKPOINT_LCZ_A", "CHECKPOINT_LCZ_B", "GATE_A", "GATE_B", "SURFACE_GATE"}

        Public Sub Awake()
            AddHandler Handlers.Player.InteractingDoor, AddressOf OnDoorInteract
            AddHandler Handlers.Player.Left, AddressOf OnLeave
            player = Player.Get(gameObject)
            player.IsBypassModeEnabled = True
        End Sub

        Private Sub OnLeave(ByVal ev As LeftEventArgs)
            If ev.Player Is player Then Destroy(Me)
        End Sub

        Private Sub OnDoorInteract(ByVal ev As InteractingDoorEventArgs)
            If ev.Player IsNot player Then Return

            If Not unbreakableDoorNames.Contains(ev.Door.DoorName) Then
                BreakDoor(ev.Door)
            ElseIf breakAll Then
                BreakDoor(ev.Door)
            End If
        End Sub

        Private Sub BreakDoor(ByVal door As Door)
            door.Networkdestroyed = True
            door.DestroyDoor(True)
            door.destroyed = True
        End Sub

        Public Sub OnDestroy()
            RemoveHandler Handlers.Player.InteractingDoor, AddressOf OnDoorInteract
            RemoveHandler Handlers.Player.Left, AddressOf OnLeave
            player.IsBypassModeEnabled = False
            breakAll = False
            Plugin.BdHubs.Remove(player)
        End Sub
    End Class
End Namespace
