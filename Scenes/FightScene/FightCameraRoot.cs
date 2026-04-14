using Godot;
using ShopGame.Characters.Fight.Girl;
using ShopGame.Extensions;
using ShopGame.Static;

namespace ShopGame.Scenes.FightScene;

[GlobalClass]
internal sealed partial class FightCameraRoot : Node3D
{
  [Export] private FightGirl _fightGirl = null!;

  [Export] internal Node3D InitPivot { get; private set; } = null!;
  private Node3D _pivot = null!;

  [Export] private SpringArm3D _springArm = null!;

  [ExportGroup("Follow")]
  [Export] private float _xStartFollowRate = 11f;
  [Export] private float _xFollowRate = 14f;
  [Export] private float _xFollowRateAccel = 50f;
  [Export] private float _yFollowRate = 30f;
  [Export] private float _lerpToPivotRate = 8f;
  private float _xMoveTimer;
  private float _yMoveTimer;

  [ExportGroup("SpringArm")]
  [Export] private float _springLengthLerpRate = 7f;
  [Export] private float _initSpringLength = 3f;
  private float _targetSpringLength;

  public override void _EnterTree()
    => GlobalInstances.FightCameraRoot = this;

  public override void _Ready()
  {
    GlobalPosition = InitPivot.GlobalPosition;
    _springArm.SpringLength = _initSpringLength;
    _targetSpringLength = _initSpringLength;
    _pivot = InitPivot;
  }

  public override void _PhysicsProcess(double delta)
  {
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
      _fightGirl.GetRealVelocity().X.IsZeroApprox()
      && GlobalPosition.IsEqualApprox(_pivot.GlobalPosition)
    )
    {
      _xMoveTimer = 0f;
      return;
    }

    float rate = _xStartFollowRate.ExpLerped(to: _xFollowRate, weight: _xFollowRateAccel * _xMoveTimer);
    newPos.X.ExpLerp(to: _pivot.GlobalPosition.X, weight: rate * deltaF);
  }

  private void HandleYFollow(ref Vector3 newPos, float deltaF)
    => newPos.Y.ExpLerp(to: _pivot.GlobalPosition.Y, weight: _yFollowRate * deltaF);

  internal void SetTarget(Node3D pivot, float springLen)
  {
    _targetSpringLength = springLen;
    _pivot = pivot;
    _xMoveTimer = 0;
  }

  internal void ResetTarget()
    => SetTarget(InitPivot, _initSpringLength);
}
