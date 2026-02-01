using Godot;
using ShopGame.Characters.Fight.Enemies;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class HitSoundPlayer : AudioStreamPlayer3D
{
  [Export] private AudioStream _enemyHitSound = null!;
  [Export] private AudioStream _dotHitSound = null!;

  internal void PlayHitSound(IHitProcessor hitProcessor)
  {
    switch (hitProcessor)
    {
      case Enemy:
        Stream = _enemyHitSound;
        Play();
        break;
      
      case HitDot:
        Stream = _dotHitSound;
        Play();
        break;
    }
  }
}
