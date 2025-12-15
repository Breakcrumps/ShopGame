using Godot;
using ShopGame.Characters.Fight;

namespace ShopGame.Scenes.FightScene;

[GlobalClass]
internal sealed partial class FightCamera : Camera2D
{
  [Export] private FightGirl? _fightGirl;
  
  public override void _Process(double delta)
    => GlobalPosition = new Vector2(_fightGirl?.GlobalPosition.X ?? 0f, GlobalPosition.Y);
}
