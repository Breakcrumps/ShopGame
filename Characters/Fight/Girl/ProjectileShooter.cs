using System.Collections.Generic;
using Godot;

namespace ShopGame.Characters.Fight.Girl;

[GlobalClass]
internal sealed partial class ProjectileShooter : Node3D
{
  [Export] private FightGirl _girl = null!;
  [Export] private PackedScene _projectilePacked = null!;

  [Export] internal ProjectileTargetMask TargetMask { get; private set; } = ProjectileTargetMask.Enemy;

  internal Queue<Projectile> AvailableProjectiles { get; } = [];

  private Projectile GetProjectile()
  {
    if (AvailableProjectiles.TryDequeue(out Projectile? projectile))
      return projectile;

    Projectile newProjectile = _projectilePacked.Instantiate<Projectile>();
    AddChild(newProjectile);
    return newProjectile;
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!@event.IsActionPressed("Shoot", allowEcho: false))
      return;

    Vector2 inputDirection = Input.GetVector("Left", "Right", "Down", "Up").Normalized();

    if (inputDirection == Vector2.Zero)
      inputDirection = _girl.FacingDirection == FacingDirection.Right ? Vector2.Right : Vector2.Left;
    
    GetProjectile().Launch(this, new Vector3(inputDirection.X, inputDirection.Y, 0f));
  }
}
