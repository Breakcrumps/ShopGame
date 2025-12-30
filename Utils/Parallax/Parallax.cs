using Godot;
using ShopGame.Scenes.FightScene;
using ShopGame.Static;

namespace ShopGame.Utils.Parallax;

[GlobalClass]
internal sealed partial class Parallax : Node2D
{
  [Export] private FightCamera? _camera;
  [Export] private Vector2 _scrollScale = Vector2.Zero;

  private Vector2 _initPos;

  public override void _Ready()
    => _initPos = Position;

  public override void _Process(double delta)
  {
    if (_camera.IsValid())
      Position = _initPos - (_camera.GlobalPosition * _scrollScale / _camera.Zoom);
  }
}
