using Fusion;
using UnityEngine;

public struct PlayerNetworkInput : INetworkInput
{
    public Vector2 MovementDirection;
    public bool JumpPressed;
    public bool RegenPressed;
}
