using Godot;

namespace ShopGame.Types;

internal readonly struct Attack
{
  internal required int Damage { get; init; }
  internal required float PushbackMagnitude { get; init; }
  internal required Node3D Attacker { get; init; }
}
