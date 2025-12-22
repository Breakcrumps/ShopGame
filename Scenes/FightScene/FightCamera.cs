using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Extensions;
using ShopGame.Static;

namespace ShopGame.Scenes.FightScene;

[GlobalClass]
internal sealed partial class FightCamera : Camera2D
{
  [Export] private FightGirl? _fightGirl;
  [Export] private Node2D? _cameraPivot;

  [Export] private float _xFollowTime = .3f;
  [Export] private float _xStartFollowRate = 10f;
  [Export] private float _xFollowRate = 12f;
  [Export] private float _yFollowRate = 30f;
  private float _xMoveTimer;
  private float _yMoveTimer;

  public override void _Ready()
  {
    if (!_cameraPivot.IsValid())
      return;
    
    GlobalPosition = _cameraPivot.GlobalPosition;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_fightGirl.IsValid() || !_cameraPivot.IsValid())
      return;

    Vector2 newPos = GlobalPosition;

    HandleXFollow(ref newPos, (float)delta);
    HandleYFollow(ref newPos, (float)delta);

    GlobalPosition = newPos;
  }

  private void HandleXFollow(ref Vector2 newPos, float deltaF)
  {
    if (_fightGirl!.Velocity.X.IsZeroApprox())
    {
      _xMoveTimer = 0f;
      return;
    }

    _xMoveTimer += deltaF;

    float rate = _xMoveTimer >= _xFollowTime ? _xFollowRate : _xStartFollowRate;

    newPos.X.ExpLerp(to: _cameraPivot!.GlobalPosition.X, rate, deltaF);
  }

  private void HandleYFollow(ref Vector2 newPos, float deltaF)
    => newPos.Y.ExpLerp(to: _cameraPivot!.GlobalPosition.Y, _yFollowRate, deltaF);
}
