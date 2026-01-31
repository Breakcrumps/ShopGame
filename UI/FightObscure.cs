using Godot;
using ShopGame.Extensions;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class FightObscure : ColorRect
{
  private const float _obscureRate = 5f;

  private const float _startFeather = 5f;
  private const float _endFeather = 2.4f;
  private float _curFeather;

  private bool _obscuring;
  private float _obscureTimer = 0f;

  private ShaderMaterial? _shaderMaterial;

  public override void _Ready()
  {
    if (Material is not ShaderMaterial shaderMaterial)
      return;
  
    _shaderMaterial = shaderMaterial;
    _shaderMaterial.SetShaderParameter("cur_feather", _startFeather);
    _obscuring = true;
  }

  public override void _PhysicsProcess(double delta)
    => Obscure((float)delta);

  private void Obscure(float deltaF)
  {
    if (!_obscuring)
      return;

    _obscureTimer += deltaF;
    _curFeather = _startFeather.ExpLerped(to: _endFeather, weight: _obscureRate * _obscureTimer);
    _shaderMaterial?.SetShaderParameter("cur_feather", _curFeather);

    if (_curFeather.IsEqualApprox(_endFeather, .00001f))
    {
      _obscuring = false;
      _obscureTimer = 0f;
    }
  }
}
