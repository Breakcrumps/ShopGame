using Godot;

namespace ShopGame.Extensions;

internal static class MathExtensions
{
  internal static void Lerp(ref this float from, float to, float weight)
    => from = Mathf.Lerp(from, to, weight);

  internal static float Lerped(this float from, float to, float weight)
    => Mathf.Lerp(from, to, weight);

  internal static void ExpLerp(ref this float from, float to, float rate, float deltaF)
    => from.Lerp(to, 1 - Mathf.Exp(-rate * deltaF));

  internal static float ExpLerped(this float from, float to, float rate, float deltaF)
    => from.Lerped(to, 1 - Mathf.Exp(-rate * deltaF));
}
