using Godot;
using ShopGame.Static;

namespace ShopGame.UI.Textbox;

[GlobalClass]
internal sealed partial class Choicebox : TextureButton
{
  [Export] private Label? _choiceLabel;
  
  public override void _Ready()
    => Visible = false;

  internal void Display(string optionText)
  {
    if (!_choiceLabel.IsValid())
      return;
    
    Visible = true;
    _choiceLabel.Text = optionText;
  }

  internal void Disable()
  {
    if (!_choiceLabel.IsValid())
      return;

    Visible = false;
    _choiceLabel.Text = "";
  }
}
