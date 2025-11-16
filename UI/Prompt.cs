using Godot;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class Prompt : Sprite2D
{
  [Export] private AnimationPlayer? _animPlayer;

  public override void _Ready()
    => Visible = false;
  
  internal void Activate()
  {
    Visible = true;
    _animPlayer?.Play("flash");
  }
  
  internal void Deactivate()
  {
    Visible = false;
    _animPlayer?.Stop();
  }
}
