using Godot;
using ShopGame.Characters.Fight.Girl;
using ShopGame.Scenes.FightScene;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class CameraControlArea : Area3D
{
  [Export] private FightCameraRoot _cameraRoot = null!;
  [Export] private Node3D _pivot = null!;
  [Export] private float _targetSpringLength = 3f;

  public override void _Ready()
  {
    BodyEntered += node =>
    {
      if (node is FightGirl)
        _cameraRoot.SetTarget(_pivot, _targetSpringLength);
    };

    BodyExited += node =>
    {
      if (node is FightGirl)
        _cameraRoot.ResetTarget();
    };
  }
}
