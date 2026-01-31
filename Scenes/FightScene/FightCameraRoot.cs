using System;
using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Extensions;
using ShopGame.Static;

namespace ShopGame.Scenes.FightScene;

[GlobalClass]
internal sealed partial class FightCameraRoot : Node3D
{
  [Export] private FightGirl? _fightGirl;

  [Export] internal Node3D? InitPivot { get; private set; }
  private Node3D? _pivot;


  [Export] private SpringArm3D? _springArm;

  [Export] private float _xStartFollowRate = 11f;
  [Export] private float _xFollowRate = 14f;
  [Export] private float _xFollowRateAccel = 50f;
  [Export] private float _yFollowRate = 30f;
  private float _xMoveTimer;
  private float _yMoveTimer;

  [Export] private float _springLengthLerpRate = 7f;
  private float _initSpringLength;
  private float _targetSpringLength;

  [Export] private float _lerpToPivotRate = 8f;

  public override void _EnterTree()
    => GlobalInstances.FightCamera = this;

  public override void _ExitTree()
    => GlobalInstances.FightCamera = null;

  public override void _Ready()
  {
    if (!InitPivot.IsValid() || !_springArm.IsValid())
      return;
    
    GlobalPosition = InitPivot.GlobalPosition;
    _initSpringLength = _springArm.SpringLength;
    _targetSpringLength = _initSpringLength;
    _pivot = InitPivot;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_fightGirl.IsValid() || !_pivot.IsValid() || !_springArm.IsValid())
      return;

    Vector3 newPos = GlobalPosition;

    HandleXFollow(ref newPos, (float)delta);
    HandleYFollow(ref newPos, (float)delta);

    GlobalPosition = newPos;

    _springArm.SpringLength = _springArm.SpringLength.ExpLerped(
      to: _targetSpringLength,
      weight: _springLengthLerpRate * (float)delta
    );
  }

  private void HandleXFollow(ref Vector3 newPos, float deltaF)
  {
    if (
      _fightGirl!.GetRealVelocity().X.IsZeroApprox()
      && GlobalPosition.IsEqualApprox(_pivot!.GlobalPosition)
    )
    {
      _xMoveTimer = 0f;
      return;
    }

    float rate = _xStartFollowRate.ExpLerped(to: _xFollowRate, weight: _xFollowRateAccel * _xMoveTimer);

    newPos.X.ExpLerp(to: _pivot!.GlobalPosition.X, weight: rate * deltaF);
  }

  private void HandleYFollow(ref Vector3 newPos, float deltaF)
    => newPos.Y.ExpLerp(to: _pivot!.GlobalPosition.Y, weight: _yFollowRate * deltaF);

  internal void SetTarget(Node3D? pivot, float springLen)
  {
    _targetSpringLength = springLen;

    if (pivot.IsValid())
      _pivot = pivot;

    _xMoveTimer = 0;
  }

  internal void ResetTarget()
    => SetTarget(InitPivot, _initSpringLength);
}
