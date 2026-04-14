using Godot;
using ShopGame.Static;

namespace ShopGame.Characters;

[GlobalClass]
internal sealed partial class Girl3D : CharacterBody3D, IGirl3D
{
  [Export] private Sprite3D _sprite = null!;
  
  [Export] private float _speed = 200f;

  [Export] private float _secsToAccelerate = 0.08f;
  private float _secsRunning;

  public bool CanMove { private get; set; } = true;

  public override void _EnterTree()
    => GlobalInstances.CurrentGirl3D = this;

  public override void _Ready()
    => AxisLockLinearY = true;

  public override void _PhysicsProcess(double delta)
  {
    if (!CanMove)
      return;
    
    Vector2 inputVector = Input.GetVector("Left", "Right", "Up", "Down");

    if (inputVector == Vector2.Zero)
    {
      _secsRunning = 0f;
      return;
    }

    if (_secsRunning < _secsToAccelerate)
      _secsRunning += (float)delta;

    Vector2 flatVelocity = inputVector.Normalized();
    flatVelocity *= _secsRunning / _secsToAccelerate * _speed;
    Velocity = new Vector3(flatVelocity.X, 0f, flatVelocity.Y);

    MoveAndSlide();
  }
}
