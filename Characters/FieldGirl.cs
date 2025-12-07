using Godot;

namespace ShopGame.Characters;

[GlobalClass]
internal sealed partial class FieldGirl : CharacterBody2D
{
  [Export] private float _walkSpeed = 150f;
  [Export] private float _runSpeed = 250f;
  
  public override void _PhysicsProcess(double delta)
  {
    float speed = Input.IsActionPressed("Run") ? _runSpeed : _walkSpeed;

    Velocity = new(Input.GetAxis("Left", "Right") * speed, GetVertVelocity());

    MoveAndSlide();
  }

  private float GetVertVelocity()
    => IsOnFloor() ? 0f : Velocity.Y + 10f;
}
