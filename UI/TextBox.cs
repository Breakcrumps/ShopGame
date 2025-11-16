using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class TextBox : TextureRect
{
  [Export] private float _secBetweenChars = .03f;
  [Export] private RichTextLabel? _label;
  [Export] private Prompt? _proceedPrompt;
  [Export] private Prompt? _questionPrompt;
  [Export] private Timer? _timer;

  [ExportGroup("ChoiceBoxes")]
  [Export] private ChoiceBox? _lowestChoiceBox;
  [Export] private ChoiceBox? _highestChoiceBox;

  private List<Replica> _currentDialogue = [];
  private int _currentLineIndex;

  private bool _awaitingInput;

  public override void _Ready()
  {
    Visible = false;
    
    GlobalInstances.TextBox = this;

    if (!_timer.IsValid())
      return;

    _timer!.WaitTime = _secBetweenChars;
    _timer.Timeout += AddVisibleCharacter;
  }

  private void AddVisibleCharacter()
  {
    if (!_label.IsValid())
      return;

    if (_label!.VisibleCharacters < _label.GetTotalCharacterCount())
    {
      _label.VisibleCharacters++;
      return;
    }

    _timer?.Stop();

    if (_currentDialogue[_currentLineIndex].Choices is not List<string> choices)
    {
      _proceedPrompt?.Activate();
      return;
    }

    if (choices.Count == 1)
    {
      _lowestChoiceBox?.Display(choices[0]);
    }
    else
    {
      _highestChoiceBox?.Display(choices[0]);
      _lowestChoiceBox?.Display(choices[1]);
    }

    _questionPrompt?.Activate();
  }

  internal void Activate(string filename, string dialogueName)
  {
    if (!_label.IsValid())
      return;

    if (GlobalInstances.Player.IfValid() is not Player player)
      return;

    ReadNewDialogue(filename, dialogueName);

    Visible = true;
    _label!.Text = _currentDialogue[0].What;
    _label.VisibleCharacters = 0;
    _timer?.Start();
    player.CanMove = false;
    _awaitingInput = true;
  }

  private void ReadNewDialogue(string filename, string dialogueName)
  {
    string jsonFile = File.ReadAllText($"Dialogue/{filename}.json");
    var dialogueToReplicas = JsonSerializer.Deserialize<Dictionary<string, List<Replica>>>(jsonFile)!;

    if (!dialogueToReplicas.TryGetValue(dialogueName, out List<Replica>? replicas))
      return;

    if (replicas is null)
      return;

    _currentDialogue = replicas;
  }

  public override void _Input(InputEvent @event)
  {
    if (!_awaitingInput)
      return;

    if (!@event.IsActionPressed("interact"))
      return;

    if (!_timer.IsValid() || !_label.IsValid())
      return;

    if (!GlobalInstances.Player.IsValid())
      return;

    if (!_timer!.IsStopped())
    {
      _label!.VisibleCharacters = _label.GetTotalCharacterCount();
      AcceptEvent();
      return;
    }

    _label!.VisibleCharacters = 0;
    
    if (_currentLineIndex == _currentDialogue.Count - 1)
    {
      Visible = false;
      _label.Text = "";
      GlobalInstances.Player!.CanMove = true;
      _awaitingInput = false;
      _proceedPrompt?.Deactivate();
      _questionPrompt?.Deactivate();
      _currentLineIndex = 0;
      _lowestChoiceBox?.Disable();
      _highestChoiceBox?.Disable();
    }
    else
    {
      _timer?.Start();
      _label.Text = _currentDialogue[_currentLineIndex].What;
    }

    AcceptEvent();
  }
}
