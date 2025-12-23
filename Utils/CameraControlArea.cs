using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Scenes.FightScene;
using ShopGame.Static;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class CameraControlArea : Area2D
{
  [Export] private FightCamera? _camera;
  [Export] private Node2D? _pivot;
  [Export] private float _targetZoom = 4f;

  public override void _Ready()
  {
    BodyEntered += node =>
    {
      if (!_camera.IsValid() || node is not FightGirl girl)
        return;

      _camera.TargetZoom = new Vector2(_targetZoom, _targetZoom);

      if (_pivot.IsValid())
        _camera.Pivot = _pivot;
    };
    BodyExited += node =>
    {
      if (!_camera.IsValid() || node is not FightGirl girl)
        return;

      _camera.TargetZoom = _camera.InitZoom;
      _camera.Pivot = _camera.InitPivot;
    };
  }
}
