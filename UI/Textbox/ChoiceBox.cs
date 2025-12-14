using Godot;
using ShopGame.Static;

namespace ShopGame.UI.Textbox;

[GlobalClass]
internal sealed partial class ChoiceBox : TextureButton
{
  [Export] private Label? _choiceLabel;
  
  public override void _Ready()
    => Visible = false;

  internal void Display(string choiceOption)
  {
    if (!_choiceLabel.IsValid())
      return;
    
    Visible = true;
    _choiceLabel.Text = choiceOption;
  }

  internal void Disable()
  {
    if (!_choiceLabel.IsValid())
      return;

    Visible = false;
    _choiceLabel.Text = "";
  }
}
