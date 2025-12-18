using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class HitDot : Area2D, IHitProcessor
{
  [Export] private CollisionShape2D? _collider;
  [Export] private Sprite2D? _sprite;

  [Export] private float _disabledTime = 3f;
  private float _disabledTimer;
  
  public void ProcessHit(Attack attack)
  {
    if (!_collider.IsValid() || !_sprite.IsValid())
      return;

    _collider.SetDeferred("disabled", true);
    _sprite.Visible = false;
    _disabledTimer = _disabledTime;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (_disabledTimer == 0f)
      return;

    if (!_collider.IsValid() || !_sprite.IsValid())
      return;

    _disabledTimer = Mathf.Max(_disabledTimer - (float)delta, 0f);

    if (_disabledTimer == 0f)
    {
      _collider.SetDeferred("disabled", false);
      _sprite.Visible = true;
    }
  }
}
