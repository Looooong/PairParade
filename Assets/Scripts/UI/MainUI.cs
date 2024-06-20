using UnityEngine;
using UnityEngine.UIElements;

namespace PairParade.UI {
  public class MainUI : VisualElement {
    public new class UxmlFactory : UxmlFactory<MainUI> { }

    VisualElement Body => this.Q("body");
    Label GameState => this.Q<Label>("game-state");
    Label RemainingTime => this.Q<Label>("remaining-time");
    Label Combo => this.Q<Label>("combo");
    Label Score => this.Q<Label>("score");
    Label MatchCount => this.Q<Label>("match-count");
    Label FlipCount => this.Q<Label>("flip-count");

    Referee _referee;
    GameSession _session;

    public MainUI() {
      _referee = Object.FindAnyObjectByType<Referee>();

      if (_referee != null) {
        RegisterCallback<AttachToPanelEvent>(e => {
          _referee.SessionChanged += OnSessionChanged;
          OnSessionChanged(_referee.Session);
        });
        RegisterCallback<DetachFromPanelEvent>(e => {
          _referee.SessionChanged -= OnSessionChanged;
          OnSessionChanged(null);
        });
      }
    }

    void OnSessionChanged(GameSession session) {
      if (_session != null) {
        _session.StateChanged -= OnGameStateChanged;
        _session.RemainingTimeChanged -= OnRemainingTimeChanged;
        _session.ComboChanged -= OnComboChanged;
        _session.ScoreChanged -= OnScoreChanged;
        _session.MatchCountChanged -= OnMatchCountChanged;
        _session.FlipCountChanged -= OnFlipCountChanged;
      }

      _session = session;

      if (_session != null) {
        _session.StateChanged += OnGameStateChanged;
        _session.RemainingTimeChanged += OnRemainingTimeChanged;
        _session.ComboChanged += OnComboChanged;
        _session.ScoreChanged += OnScoreChanged;
        _session.MatchCountChanged += OnMatchCountChanged;
        _session.FlipCountChanged += OnFlipCountChanged;

        OnGameStateChanged(_session.State);
        OnRemainingTimeChanged(_session.RemainingTime);
        OnComboChanged(_session.Combo);
        OnScoreChanged(_session.Score);
        OnMatchCountChanged(_session.MatchCount);
        OnFlipCountChanged(_session.FlipCount);

        foreach (var cardState in _session.cardStates) {
          Body.Add(new CardElement(cardState, _referee));
        }
      }
    }

    void OnGameStateChanged(GameState state) => GameState.text = state switch {
      PairParade.GameState.Memorization => "Get Ready",
      PairParade.GameState.Playing => "Playing",
      PairParade.GameState.Completed => "Game Finished",
      PairParade.GameState.Failed => "Game Over",
      _ => null
    };

    void OnRemainingTimeChanged(float time) => RemainingTime.text = float.IsFinite(time) && time > 0f ? $": {time:F1}s" : null;

    void OnComboChanged(int count) => Combo.text = count > 0 ? $"Combo: +{count}" : "No Combo";

    void OnScoreChanged(int score) => Score.text = score.ToString();

    void OnMatchCountChanged(int count) => MatchCount.text = count.ToString();

    void OnFlipCountChanged(int count) => FlipCount.text = count.ToString();
  }
}
