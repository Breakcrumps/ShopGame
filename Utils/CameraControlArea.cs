using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Scenes.FightScene;
using ShopGame.Static;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class CameraControlArea : Area3D
{
  [Export] private FightCameraRoot? _cameraRoot;
  [Export] private Node3D? _pivot;
  [Export] private float _targetSpringLength = 3f;

  public override void _Ready()
  {
    BodyEntered += node =>
    {
      if (!_cameraRoot.IsValid() || node is not FightGirl girl)
        return;

      _cameraRoot.SetTarget(_pivot, _targetSpringLength);
    };

    BodyExited += node =>
    {
      if (!_cameraRoot.IsValid() || node is not FightGirl girl)
        return;

      _cameraRoot.ResetTarget();
    };
  }
}
