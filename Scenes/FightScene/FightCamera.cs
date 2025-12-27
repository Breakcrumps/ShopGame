using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Extensions;
using ShopGame.Static;

namespace ShopGame.Scenes.FightScene;

[GlobalClass]
internal sealed partial class FightCamera : Camera2D
{
  [Export] private FightGirl? _fightGirl;
  [Export] internal Node2D? InitPivot { get; private set; }

  [Export] private float _xFollowTime = .3f;
  [Export] private float _xStartFollowRate = 10f;
  [Export] private float _xFollowRate = 12f;
  [Export] private float _yFollowRate = 30f;
  private float _xMoveTimer;
  private float _yMoveTimer;

  [Export] private float _zoomLerpRate = 7f;
  internal Vector2 InitZoom { get; private set; }
  internal Vector2 TargetZoom { private get; set; } = new(4f, 4f);

  [Export] private float _lerpToPivotRate = 8f;
  internal Node2D? Pivot { private get; set; }

  public override void _Ready()
  {
    if (!InitPivot.IsValid())
      return;
    
    GlobalPosition = InitPivot.GlobalPosition;
    InitZoom = Zoom;
    Pivot = InitPivot;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_fightGirl.IsValid() || !Pivot.IsValid())
      return;

    Vector2 newPos = GlobalPosition;

    HandleXFollow(ref newPos, (float)delta);
    HandleYFollow(ref newPos, (float)delta);

    GlobalPosition = newPos;

    Zoom = Zoom.ExpLerpedV(to: TargetZoom, _zoomLerpRate, (float)delta);
  }

  private void HandleXFollow(ref Vector2 newPos, float deltaF)
  {
    if (
      _fightGirl!.GetRealVelocity().X.IsZeroApprox()
      && GlobalPosition.IsEqualApprox(Pivot!.GlobalPosition)
    )
    {
      _xMoveTimer = 0f;
      return;
    }

    _xMoveTimer += deltaF;

    float rate = _xMoveTimer >= _xFollowTime ? _xFollowRate : _xStartFollowRate;

    newPos.X.ExpLerp(to: Pivot!.GlobalPosition.X, rate, deltaF);
  }

  private void HandleYFollow(ref Vector2 newPos, float deltaF)
    => newPos.Y.ExpLerp(to: Pivot!.GlobalPosition.Y, _yFollowRate, deltaF);
}
