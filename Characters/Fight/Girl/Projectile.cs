using System;
using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Characters.Fight.Girl;

[GlobalClass]
internal sealed partial class Projectile : Area3D
{
  private bool _isActive;
  
  private const uint WorldPhysLayer = 4;
  
  [Export] private Sprite3D _sprite = null!;
  [Export] private AnimationPlayer _animPlayer = null!;
  
  [Export] private int _damage = 10;
  [Export] private float _pushbackMagnitude = 10f;
  [Export] private float _speed = 1f;
  [Export] private float _lifeTime = 2f;
  
  private ProjectileShooter _projectileShooter = null!;

  private Vector3 _direction;
  private float _lifeTimer;

  public override void _Ready()
  {
    TopLevel = true;
    Deactivate();

    BodyEntered += node =>
    {
      if (node is IHitProcessor hitProcessor)
      {
        hitProcessor.ProcessHit(new Attack
        {
          Damage = _damage,
          PushbackMagnitude = _pushbackMagnitude,
          Attacker = this
        });
      }

      Deactivate();
      _projectileShooter.AvailableProjectiles.Enqueue(this);
    };
  }

  internal void Launch(ProjectileShooter shooter, Vector3 direction)
  {
    _isActive = true;
    GlobalPosition = shooter.GlobalPosition;
    _projectileShooter = shooter;
    _direction = direction;
    Visible = true;
    ProcessMode = ProcessModeEnum.Inherit;
    _lifeTimer = _lifeTime;
    _sprite.LookAt(GlobalInstances.FightCameraRoot.GlobalPosition);
    _sprite.Rotation = new Vector3(0f, 0f, MathF.Atan2(direction.Y, direction.X));
    _animPlayer.Play("Init");
    CollisionMask = WorldPhysLayer | (uint)shooter.TargetMask;
  }

  private void Deactivate()
  {
    if (!_isActive)
      return;
    
    Visible = false;
    _animPlayer.Stop();
    SetDeferred(PropertyName.ProcessMode, (int)ProcessModeEnum.Disabled);
  }

  public override void _PhysicsProcess(double delta)
  {
    GlobalPosition += _direction * _speed * (float)delta;

    _lifeTimer -= (float)delta;

    if (_lifeTimer <= 0f)
    {
      Deactivate();
      _projectileShooter.AvailableProjectiles.Enqueue(this);
    }
  }
}
