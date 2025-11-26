using System;
using Godot;

[GlobalClass]
internal sealed partial class HandZone : Control
{
  [Export] internal TurnOrientation TurnOrientation { get; private set; }

  internal event Action<TurnOrientation>? HandEntered;

  public override void _Ready()
    => MouseEntered += () => HandEntered?.Invoke(TurnOrientation);
}
