Imports Exiled.API.Features
Imports Exiled.Events.EventArgs
Imports UnityEngine
Imports Handlers = Exiled.Events.Handlers

Namespace AdminTools
    Public Class InstantKillComponent
        Inherits MonoBehaviour

        Public player As Player

        Public Sub Awake()
            player = Player.Get(gameObject)
            AddHandler Handlers.Player.Hurting, AddressOf RunWhenPlayerIsHurt
            AddHandler Handlers.Player.Left, AddressOf OnLeave
            Plugin.IkHubs.Add(player, Me)
        End Sub

        Private Sub OnLeave(ByVal ev As LeftEventArgs)
            If ev.Player Is player Then Destroy(Me)
        End Sub

        Public Sub OnDestroy()
            RemoveHandler Handlers.Player.Hurting, AddressOf RunWhenPlayerIsHurt
            RemoveHandler Handlers.Player.Left, AddressOf OnLeave
            Plugin.IkHubs.Remove(player)
        End Sub

        Public Sub RunWhenPlayerIsHurt(ByVal ev As HurtingEventArgs)
            If ev.Attacker IsNot ev.Target AndAlso ev.Attacker Is player Then ev.Amount = Integer.MaxValue
        End Sub
    End Class
End Namespace
