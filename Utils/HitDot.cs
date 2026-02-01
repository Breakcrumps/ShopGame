using Godot;
using ShopGame.Characters;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class HitDot : Area3D, IHitProcessor
{
  [Export] private CollisionShape3D _collider = null!;
  [Export] private Sprite3D _sprite = null!;

  [Export] private float _disabledTime = 3f;
  private float _disabledTimer;
  
  public void ProcessHit(Attack attack)
  {
    _collider.SetDeferred("disabled", true);
    _sprite.Visible = false;
    _disabledTimer = _disabledTime;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (_disabledTimer == 0f)
      return;

    _disabledTimer = Mathf.Max(_disabledTimer - (float)delta, 0f);

    if (_disabledTimer == 0f)
    {
      _collider.SetDeferred("disabled", false);
      _sprite.Visible = true;
    }
  }
}
