using Godot;

[GlobalClass]
internal sealed partial class HandZone : Control
{
  [Export] internal TurnOrientation TurnOrientation { get; private set; }
}
