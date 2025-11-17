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

  [ExportGroup("Prompts")]
  [Export] private Prompt? _proceedPrompt;
  [Export] private Prompt? _questionPrompt;

  [ExportGroup("Timers")]
  [Export] private Timer? _timer;
  [Export] private Timer? _waitEscapeTimer;

  [ExportGroup("ChoiceBoxes")]
  [Export] private ChoiceBox? _lowestChoiceBox;
  [Export] private ChoiceBox? _highestChoiceBox;

  private Dictionary<string, List<Replica>> _dialogueToReplicas = [];
  private List<Replica> _currentDialogue = [];
  private string _currentDialogueName = "";
  private (string? Lowest, string? Highest) _choiceDestinations;
  private readonly List<(string Name, int LineIndex)> _dialogueSnapshots = [];
  private int _currentLineIndex;

  private bool _inDialogue;
  private bool _awaitingInput;
  private bool _inWait;

  private readonly JsonSerializerOptions _dialogueDeserOpt = new() { IncludeFields = true };

  public override void _Ready()
  {
    if (!_lowestChoiceBox.IsValid() || !_highestChoiceBox.IsValid())
      return;
    
    Visible = false;
    
    GlobalInstances.TextBox = this;

    if (!_timer.IsValid() || !_waitEscapeTimer.IsValid())
      return;

    _timer!.WaitTime = _secBetweenChars;
    _timer.Timeout += AddVisibleCharacter;

    _waitEscapeTimer!.Timeout += EndWait;

    _lowestChoiceBox!.Pressed += () => Choose(_choiceDestinations.Lowest);
    _highestChoiceBox!.Pressed += () => Choose(_choiceDestinations.Highest);
  }

  private void Choose(string? option)
  {
    if (_label is null)
      return;
    
    if (option is null)
    {
      _currentLineIndex++;
      LoadLine(_currentLineIndex);
      return;
    }
    
    _dialogueSnapshots.Add((_currentDialogueName, _currentLineIndex));
    _currentDialogueName = option;
    _currentDialogue = _dialogueToReplicas[option];
    _currentLineIndex = 0;
    _awaitingInput = true;
    LoadLine(_currentLineIndex);

    ReadDialogueFromFile(_currentDialogueName);
  }

  private void AddVisibleCharacter()
  {
    if (!_label.IsValid())
      return;

    if (_label!.VisibleCharacters < _label.GetTotalCharacterCount())
    {
      if (_currentDialogue[_currentLineIndex].Waits.TryGetValue(_label.VisibleCharacters, out float waitTime))
      {
        if (!_waitEscapeTimer.IsValid())
          return;

        _timer?.Stop();
        
        if (waitTime == -1)
        {
          _inWait = true;
          return;
        }

        _waitEscapeTimer!.WaitTime = waitTime;
        _waitEscapeTimer.Start();
        _inWait = true;
        return;
      }
      
      _label.VisibleCharacters++;
      return;
    }

    _timer?.Stop();

    if (_currentDialogue[_currentLineIndex].Choices is not List<Choice> choices)
    {
      _proceedPrompt?.Activate();
      return;
    }

    if (choices.Count == 1)
    {
      _lowestChoiceBox?.Display(choices[0].What);
      _choiceDestinations.Lowest = choices[0].Destination;
    }
    else
    {
      _highestChoiceBox?.Display(choices[0].What);
      _lowestChoiceBox?.Display(choices[1].What);
      _choiceDestinations = (choices[1].Destination, choices[0].Destination);
    }

    _questionPrompt?.Activate();
    _awaitingInput = false;
  }

  internal void Activate(string filename, string dialogueName)
  {
    if (_inDialogue)
      return;
    
    if (!_label.IsValid())
      return;

    if (GlobalInstances.Player.IfValid() is not Player player)
      return;

    ReadNewDialogueFile(filename, dialogueName);

    LoadLine(index: 0);
    player.CanMove = false;
    _awaitingInput = true;
    _inDialogue = true;
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

    if (_inWait)
    {
      _waitEscapeTimer?.Stop();
      EndWait();
      return;
    }

    if (!_timer!.IsStopped())
    {
      _label!.VisibleCharacters = _label.GetTotalCharacterCount();
      AcceptEvent();
      return;
    }

    if (++_currentLineIndex != _currentDialogue.Count)
    {
      LoadLine(_currentLineIndex);
      AcceptEvent();
      return;
    }

    if (_dialogueSnapshots.Count == 0)
    {
      EndDialogue();
    }
    else
    {
      ReadDialogueFromFile(_dialogueSnapshots[^1].Name);
      _currentLineIndex = _dialogueSnapshots[^1].LineIndex + 1;
      _dialogueSnapshots.RemoveAt(_dialogueSnapshots.Count - 1);
      LoadLine(_currentLineIndex);
    }

    AcceptEvent();
  }

  private void LoadLine(int index)
  {
    if (!_label.IsValid())
      return;

    if (!_lowestChoiceBox.IsValid() || !_highestChoiceBox.IsValid())
      return;

    if (index >= _currentDialogue.Count)
    {
      EndDialogue();
      return;
    }

    Visible = true;
    _awaitingInput = true;
    _timer?.Start();
    _currentDialogue[index].ComputeLineAndWaits();
    _label!.Text = _currentDialogue[index].Line;
    _label.VisibleCharacters = 0;
    _proceedPrompt?.Deactivate();
    _questionPrompt?.Deactivate();
    _lowestChoiceBox!.Visible = false;
    _highestChoiceBox!.Visible = false;
  }

  private void EndDialogue()
  {
    Visible = false;
    _timer?.Stop();
    _label!.Text = "";
    _label.VisibleCharacters = 0;
    GlobalInstances.Player!.CanMove = true;
    _awaitingInput = false;
    _inDialogue = false;
    _proceedPrompt?.Deactivate();
    _questionPrompt?.Deactivate();
    _currentLineIndex = 0;
    _lowestChoiceBox?.Disable();
    _highestChoiceBox?.Disable();
    _choiceDestinations.Lowest = null;
    _choiceDestinations.Highest = null;
    _dialogueToReplicas = [];
    _currentDialogue = [];
  }

  private void EndWait()
  {
    if (!_timer.IsValid() || !_label.IsValid())
        return;

    _timer!.Start();
    _label!.VisibleCharacters++;
    _inWait = false;
  }

  private void ReadNewDialogueFile(string filename, string dialogueName)
  {
    string jsonFile = File.ReadAllText($"Dialogue/{filename}.json");

    _dialogueToReplicas = JsonSerializer.Deserialize<Dictionary<string, List<Replica>>>(
      jsonFile,
      options: _dialogueDeserOpt
    ) ?? [];

    ReadDialogueFromFile(dialogueName);
  }

  private void ReadDialogueFromFile(string dialogueName)
  {
    if (!_dialogueToReplicas.TryGetValue(dialogueName, out List<Replica>? replicas))
      return;

    if (replicas is null)
      return;

    _currentDialogue = replicas;
    _currentDialogueName = dialogueName;
  }
}
