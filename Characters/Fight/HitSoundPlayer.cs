using Godot;
using ShopGame.Characters.Fight.Enemies;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class HitSoundPlayer : AudioStreamPlayer3D
{
  [Export] private AudioStream? _enemyHitSound;
  [Export] private AudioStream? _dotHitSound;

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
