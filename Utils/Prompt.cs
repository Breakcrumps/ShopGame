using Godot;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class Prompt : Sprite2D
{
  [Export] private AnimationPlayer? _animPlayer;

  public override void _Ready()
    => Visible = false;
  
  internal void Activate()
  {
    Visible = true;
    _animPlayer?.Play("Flash");
  }
  
  internal void Deactivate()
  {
    Visible = false;
    _animPlayer?.Stop();
  }

  internal bool IsActive()
    => _animPlayer?.IsPlaying() ?? false;
}
