using Godot;

namespace ShopGame.Characters.DialogueSprites;

[GlobalClass]
internal sealed partial class DialogueSprite : Sprite2D
{
  internal void LoadCharacter(string charName)
    => Texture = ResourceLoader.Load<Texture2D>($"res://Characters/DialogueSprites/{charName}.png");
}
