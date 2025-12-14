using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.UI.Textbox;

[GlobalClass]
internal sealed partial class TextBox : TextureRect
{
  [Export] private float _secBetweenChars = .03f;
  [Export] private RichTextLabel? _label;

  [ExportGroup("Prompts")]
  [Export] private Prompt? _proceedPrompt;
  [Export] private Prompt? _questionPrompt;

  [ExportGroup("Timers")]
  [Export] private Timer? _nextCharTimer;
  [Export] private Timer? _waitTimer;

  [ExportGroup("ChoiceBoxes")]
  [Export] private ChoiceBox? _lowestChoiceBox;
  [Export] private ChoiceBox? _highestChoiceBox;

  private IActionHandler? _activatorNode;
  private DialogueInitArea? _dialogueArea;

  private Dictionary<string, List<Replica>> _dialogueFile = [];
  private List<Replica> _currentReplicas = [];
  private string _currentDialogueName = "";
  private (Choice Lowest, Choice Highest) _choiceDestinations;
  private readonly List<(string Name, int LineIndex)> _dialogueSnapshots = [];
  private int _currentReplicaIndex;

  private bool _inDialogue;
  private bool _awaitingInput;
  private bool _inWait;

  private static readonly JsonSerializerOptions _dialogueDeserOpt = new() { IncludeFields = true };

  public override void _EnterTree()
    => GlobalInstances.TextBox = this;

  public override void _Ready()
  {
    ReadNewDialogueFile(GetTree().CurrentScene.Name);
    
    if (!_lowestChoiceBox.IsValid() || !_highestChoiceBox.IsValid())
      return;
    
    Visible = false;

    if (!_nextCharTimer.IsValid() || !_waitTimer.IsValid())
      return;

    _nextCharTimer.WaitTime = _secBetweenChars;
    _nextCharTimer.Timeout += AddVisibleCharacter;

    _waitTimer.OneShot = true;
    _waitTimer.Timeout += EndWait;

    _lowestChoiceBox.Pressed += () => Choose(in _choiceDestinations.Lowest);
    _highestChoiceBox.Pressed += () => Choose(in _choiceDestinations.Highest);
  }

  private void Choose(in Choice option)
  {
    if (option.Action is ChoiceAction choiceAction)
    {
      if (choiceAction.FromObject)
        _activatorNode?.HandleAction(choiceAction.Name);
      else
        DialogueActions.Actions[choiceAction.Name]();
    }

    if (option.Destination is null)
    {
      _currentReplicaIndex++;
      LoadLine(_currentReplicaIndex);
      return;
    }
    
    if (!option.NoSnapshot)
      _dialogueSnapshots.Add((_currentDialogueName, _currentReplicaIndex));

    _currentDialogueName = option.Destination;
    _currentReplicas = _dialogueFile[option.Destination];
    _currentReplicaIndex = 0;
    _awaitingInput = true;
    LoadLine(_currentReplicaIndex);

    ReadDialogueFromFile(_currentDialogueName);
  }

  private void AddVisibleCharacter()
  {
    if (!_label.IsValid())
      return;

    if (_label.VisibleCharacters < _label.GetTotalCharacterCount())
    {
      if (!_currentReplicas[_currentReplicaIndex].Waits.TryGetValue(_label.VisibleCharacters, out float waitTime))
      {
        _label.VisibleCharacters++;
        return; 
      }
      
      if (!_waitTimer.IsValid())
        return;

      _nextCharTimer?.Stop();
      
      if (waitTime == -1)
      {
        _inWait = true;
        return;
      }

      _waitTimer.WaitTime = waitTime;
      _waitTimer.Start();
      _inWait = true;
      return;
    }

    _nextCharTimer?.Stop();

    if (_currentReplicas[_currentReplicaIndex].Choices is not List<Choice> choices)
    {
      _proceedPrompt?.Activate();
      return;
    }

    if (choices.Count == 1)
    {
      _lowestChoiceBox?.Display(choices[0].What);
      _choiceDestinations.Lowest = choices[0];
    }
    else
    {
      _highestChoiceBox?.Display(choices[0].What);
      _lowestChoiceBox?.Display(choices[1].What);
      _choiceDestinations = (choices[1], choices[0]);
    }

    _questionPrompt?.Activate();
    _awaitingInput = false;
  }

  internal void Activate(Node activator, DialogueInitArea dialogueArea, int variant)
  {
    if (_inDialogue)
      return;
    
    if (!_label.IsValid())
      return;

    if (GlobalInstances.Girl.IfValid() is not Girl player)
      return;

    if (activator is IActionHandler handler)
      _activatorNode = handler;

    _dialogueArea = dialogueArea;

    ReadDialogueFromFile($"{activator.Name} {variant}");

    LoadLine(index: 0);
    player.CanMove = false;
    _awaitingInput = true;
    _inDialogue = true;
  }

  public override void _Input(InputEvent @event)
  {
    if (!_awaitingInput)
      return;

    if (!@event.IsActionPressed("Interact"))
      return;

    if (!_nextCharTimer.IsValid() || !_label.IsValid())
      return;

    if (!GlobalInstances.Girl.IsValid())
      return;

    if (_inWait)
    {
      _waitTimer?.Stop();
      EndWait();
      return;
    }

    if (!_nextCharTimer.IsStopped())
    {
      _label.VisibleCharacters = _label.GetTotalCharacterCount();
      AcceptEvent();
      return;
    }

    if (++_currentReplicaIndex != _currentReplicas.Count)
    {
      LoadLine(_currentReplicaIndex);
      AcceptEvent();
      return;
    }

    if (_dialogueSnapshots.Count == 0)
      EndDialogue();
    else
      ReadSnapshot();

    AcceptEvent();
  }

  private void LoadLine(int index)
  {
    if (!_label.IsValid())
      return;

    if (!_lowestChoiceBox.IsValid() || !_highestChoiceBox.IsValid())
      return;

    if (index >= _currentReplicas.Count)
    {
      if (_dialogueSnapshots.Count != 0)
        ReadSnapshot();
      else
        EndDialogue();
      return;
    }

    Visible = true;
    _awaitingInput = true;
    _nextCharTimer?.Start();
    _label.Text = _currentReplicas[index].Line;
    _label.VisibleCharacters = 0;
    _proceedPrompt?.Deactivate();
    _questionPrompt?.Deactivate();
    _lowestChoiceBox.Visible = false;
    _highestChoiceBox.Visible = false;
  }

  private void ReadSnapshot()
  {
    ReadDialogueFromFile(_dialogueSnapshots[^1].Name);
    _currentReplicaIndex = _dialogueSnapshots[^1].LineIndex + 1;
    _dialogueSnapshots.RemoveAt(_dialogueSnapshots.Count - 1);
    LoadLine(_currentReplicaIndex);
  }

  private void EndDialogue()
  {
    Visible = false;
    _nextCharTimer?.Stop();
    _label!.Text = "";
    _label.VisibleCharacters = 0;
    GlobalInstances.Girl!.CanMove = true;
    _awaitingInput = false;
    _inDialogue = false;
    _proceedPrompt?.Deactivate();
    _questionPrompt?.Deactivate();
    _currentReplicaIndex = 0;
    _lowestChoiceBox?.Disable();
    _highestChoiceBox?.Disable();
    _currentReplicas = [];
    _activatorNode = null;
    if (_dialogueArea.IsValid())
      _dialogueArea!.AwaitInput = true;
    _dialogueArea = null;
  }

  private void EndWait()
  {
    if (!_nextCharTimer.IsValid() || !_label.IsValid())
        return;

    _nextCharTimer.Start();
    _label.VisibleCharacters++;
    _inWait = false;
  }

  private void ReadNewDialogueFile(string filename)
  {
    string jsonFile = File.ReadAllText($"Dialogue/{filename}.json");

    _dialogueFile = JsonSerializer.Deserialize<Dictionary<string, List<Replica>>>(
      jsonFile,
      options: _dialogueDeserOpt
    ) ?? [];

    foreach (var (_, replicas) in _dialogueFile)
    {
      foreach (Replica replica in replicas)
      {
        replica.ComputeLineAndWaits();
      }
    }
  }

  private bool ReadDialogueFromFile(string dialogueName)
  {
    if (!_dialogueFile.TryGetValue(dialogueName, out List<Replica>? replicas))
      return false;

    if (replicas is null)
      return false;

    _currentReplicas = replicas;
    _currentDialogueName = dialogueName;
    return true;
  }

  internal int CountVariants(Node activator)
  {
    int variant = 1;
    
    for ( ; ReadDialogueFromFile($"{activator.Name} {variant}"); variant++);

    return variant - 1;
  }
}
