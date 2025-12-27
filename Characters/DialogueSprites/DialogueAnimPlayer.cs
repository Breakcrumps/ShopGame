using System.Collections.Generic;
using Godot;
using ShopGame.Static;

namespace ShopGame.Characters.DialogueSprites;

internal enum DialogueCharState { Hidden, Unfocused, Focused }

[GlobalClass]
internal sealed partial class DialogueAnimPlayer : AnimationPlayer
{
  [Export] private DialogueSprite? _sprite;
  
  internal string CurrentSprite = "";
  private DialogueCharState _charState = DialogueCharState.Hidden;
  
  internal List<DialogueCharState> TargetStates { get; private set; } = [];
  private List<string> _queuedImageNames = [];
  private List<DialogueCharState> _currentActionQueue = [];

  public override void _Ready()
    => PlayBackwards("Appear");

  public override void _Process(double delta)
  {
    if (IsPlaying())
      return;

    if (!_sprite.IsValid())
      return;

    if (_currentActionQueue.Count != 0)
    {
      ExecuteAction(_currentActionQueue[0]);
      _currentActionQueue.RemoveAt(0);
      return;
    }

    if (TargetStates.Count == 0)
      return;

    ExecuteAction(TargetStates[0]);
    TargetStates.RemoveAt(0);
  }

  private void ExecuteAction(DialogueCharState nextState)
  {
    switch (_charState)
    {
      case DialogueCharState.Hidden:
        TransitionFromHidden(nextState);
        break;

      case DialogueCharState.Unfocused:
        TransitionFromUnfocused(nextState);
        break;

      case DialogueCharState.Focused:
        TransitionFromFocused(nextState);
        break;
    }
  }

  private void TransitionFromHidden(DialogueCharState nextState)
  {
    string imageName = _queuedImageNames[0];
    _sprite!.LoadCharacter(imageName);
    _queuedImageNames.RemoveAt(0);

    switch (nextState)
    {
      case DialogueCharState.Unfocused:
      {
        Play("Appear");
        _charState = DialogueCharState.Unfocused;
        CurrentSprite = imageName;
        break;
      }

      case DialogueCharState.Focused:
      {
        Play("Appear");
        _charState = DialogueCharState.Unfocused;
        CurrentSprite = imageName;
        _currentActionQueue.Add(DialogueCharState.Focused);
        break;
      }
    }
  }

  private void TransitionFromUnfocused(DialogueCharState nextState)
  {    
    switch (nextState)
    {
      case DialogueCharState.Hidden:
      {
        PlayBackwards("Appear");
        CurrentSprite = "";
        _charState = DialogueCharState.Hidden;
        break;
      }
      
      case DialogueCharState.Focused:
      {
        Play("FadeIn");
        _charState = DialogueCharState.Focused;
        break;
      }
    }
  }
  
  private void TransitionFromFocused(DialogueCharState nextState)
  {
    switch (nextState)
    {
      case DialogueCharState.Hidden:
      {
        PlayBackwards("FadeIn");
        _charState = DialogueCharState.Unfocused;
        _currentActionQueue.Add(DialogueCharState.Hidden);
        break;
      }

      case DialogueCharState.Unfocused:
      {
        PlayBackwards("FadeIn");
        _charState = DialogueCharState.Unfocused;
        break;
      }
    }
  }

  internal void QueueAppear(string charName, bool intoFocus)
  {
    if (charName == "")
    {
      TargetStates.Add(DialogueCharState.Hidden);
      return;
    }
    
    TargetStates.Add(intoFocus ? DialogueCharState.Focused : DialogueCharState.Unfocused);
    _queuedImageNames.Add(charName);
  }

  internal void FinishUp()
  {
    if (!_sprite.IsValid())
      return;
    
    _charState = DialogueCharState.Hidden;
    CurrentSprite = "";
    TargetStates = [];
    _currentActionQueue = [];
    _queuedImageNames = [];
    PlayBackwards("Appear");
  }
}
