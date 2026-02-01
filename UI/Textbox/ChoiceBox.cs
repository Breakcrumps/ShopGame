using Godot;

namespace ShopGame.UI.Textbox;

[GlobalClass]
internal sealed partial class Choicebox : TextureButton
{
  [Export] private Label _choiceLabel = null!;
  
  public override void _Ready()
    => Visible = false;

  internal void Display(string optionText)
  {
    Visible = true;
    _choiceLabel.Text = optionText;
  }

  internal void Disable()
  {
    Visible = false;
    _choiceLabel.Text = "";
  }
}
