using Godot;

namespace ShopGame.TEMP;

[GlobalClass]
internal sealed partial class MovingGirl : Sprite3D
{
  [Export] private float _moveSpeed = .1f;
  [Export] private Vector3 _moveAxis = new(0f, 0f, -1f);

  public override void _PhysicsProcess(double delta)
  {
    float moveDir = Input.GetAxis("Left", "Right");
    GlobalPosition += _moveAxis * moveDir * _moveSpeed;
  }
}
