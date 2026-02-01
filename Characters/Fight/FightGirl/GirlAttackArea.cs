using System;
using Godot;
using ShopGame.Types;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class GirlAttackArea : Area3D
{
  private enum AttackDirection { Up, Down, Left, Right, UpLeft, UpRight }
  [Export] private AttackDirection _attackDirection;

  [Export] private FightGirl _fightGirl = null!;
  [Export] private HitSoundPlayer _hitSoundPlayer = null!;
  [Export] private CollisionShape3D _collider = null!;

  [Export] private int _attackStrength = 10;
  [Export] private float _pushbackMagnitude = 130f;
  [Export] private float _attackDuration = .1f;


  private float _timeLeftInAttack;
  
  public override void _Ready()
  {
    _collider.Disabled = true;
    
    BodyEntered += TryHit;
    AreaEntered += TryHit;
  }

  private void TryHit(Node3D node)
  {
    if (node is not IHitProcessor hitProcessor)
      return;

    Attack attack = new()
    {
      Strength = _attackStrength,
      PushbackMagnitude = _pushbackMagnitude,
      Attacker = _fightGirl
    };

    hitProcessor.ProcessHit(attack);
    
    Vector3 pushbackDirection = _attackDirection switch
    {
      AttackDirection.Up => Vector3.Down,
      AttackDirection.Down => Vector3.Up,
      AttackDirection.Left => Vector3.Right,
      AttackDirection.Right => Vector3.Left,
      AttackDirection.UpLeft => new Vector3(1f, -1f, 0f),
      AttackDirection.UpRight => new Vector3(-1f, -1f, 0f),
      _ => Vector3.Zero
    };

    _fightGirl.HandleOwnAttackPushback(pushbackDirection.Normalized(), pogo: _attackDirection is AttackDirection.Down);

    _hitSoundPlayer.PlayHitSound(hitProcessor);

    StopAttack();
  }


  public override void _PhysicsProcess(double delta)
  {
    if (
      _timeLeftInAttack == 0f
      && Input.IsActionJustPressed("Attack")
      && NeededDirectionPressed()
      && !_fightGirl.InAttack
    )
    {
      _timeLeftInAttack = _attackDuration;
      _fightGirl.InAttack = true;
      _collider.Disabled = false;
      return;
    }

    if (_timeLeftInAttack <= 0f)
    {
      if (_collider.Disabled)
        return;
      
      StopAttack();
      return;
    }

    _timeLeftInAttack -= (float)delta;
  }

  private bool NeededDirectionPressed() => _attackDirection switch
  {
    AttackDirection.Down => Input.IsActionPressed("Down"),
    AttackDirection.UpLeft => Input.IsActionPressed("Up") && Input.IsActionPressed("Left"),
    AttackDirection.UpRight => Input.IsActionPressed("Up") && Input.IsActionPressed("Right"),
    _ => Input.IsActionPressed(Enum.GetName(_attackDirection)!) && NothingElsePressed()
  };

  private bool NothingElsePressed()
  {
    for (int i = 0; i <= (int)AttackDirection.Right; i++)
    {
      if (i == (int)_attackDirection)
        continue;

      if (Input.IsActionPressed(Enum.GetName((AttackDirection)i)!))
        return false;
    }

    return true;
  }

  private void StopAttack()
  {
    _timeLeftInAttack = 0f;
    _collider.SetDeferred("disabled", true);
    _fightGirl.InAttack = false;
  }
}
