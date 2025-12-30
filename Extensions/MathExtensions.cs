using Godot;

namespace ShopGame.Extensions;

internal static class MathExtensions
{
  #region Lerp
  internal static void Lerp(this ref float from, float to, float weight)
    => from = Mathf.Lerp(from, to, weight);

  internal static float Lerped(this float from, float to, float weight)
    => Mathf.Lerp(from, to, weight);

  internal static void ExpLerp(this ref float from, float to, float rate, float param)
    => from.Lerp(to, 1 - Mathf.Exp(-rate * param));

  internal static float ExpLerped(this float from, float to, float rate, float param)
    => from.Lerped(to, 1 - Mathf.Exp(-rate * param));

  internal static void ExpLerpVec2(this ref Vector2 from, Vector2 to, float rate, float param)
    => from = from.Lerp(to, 1 - Mathf.Exp(-rate * param));

  internal static Vector2 ExpLerpedVec2(this Vector2 from, Vector2 to, float rate, float param)
    => from.Lerp(to, 1 - Mathf.Exp(-rate * param));

  internal static void ExpLerpVec3(this ref Vector3 from, Vector3 to, float rate, float param)
    => from = from.Lerp(to, 1 - Mathf.Exp(-rate * param));

  internal static Vector3 ExpLerpedVec3(this Vector3 from, Vector3 to, float rate, float param)
    => from.Lerp(to, 1 - Mathf.Exp(-rate * param));
  #endregion

  #region Equality
  internal static bool IsEqualApprox(this float a, float b)
    => Mathf.IsEqualApprox(a, b);

  internal static bool IsZeroApprox(this float a)
    => Mathf.IsZeroApprox(a);
  #endregion
}
