using Godot;
using ShopGame.Static;

namespace ShopGame.Characters;

[GlobalClass]
internal sealed partial class Girl : CharacterBody2D
{
  [Export] private float _speed = 200f;
  [Export] private int _framesToAccelerate = 5;

  internal bool CanMove { private get; set; } = true;

  private int _runTime;

  public override void _EnterTree()
    => GlobalInstances.Player = this;

  public override void _PhysicsProcess(double delta)
  {
    if (!CanMove)
      return;
    
    Vector2 inputVector = Input.GetVector("Left", "Right", "Up", "Down");

    if (inputVector == Vector2.Zero)
    {
      _runTime = 0;
      return;
    }

    if (_runTime < _framesToAccelerate)
      _runTime++;

    Velocity = inputVector.Normalized() * _runTime / _framesToAccelerate * _speed;

    MoveAndSlide();
  }
}
