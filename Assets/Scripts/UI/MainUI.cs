using UnityEngine;
using UnityEngine.UIElements;

namespace PairParade.UI {
  public class MainUI : VisualElement {
    public new class UxmlFactory : UxmlFactory<MainUI> { }

    VisualElement Body => this.Q("body");
    Label RemainingTime => this.Q<Label>("remaining-time");
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
        _session.RemainingTimeChanged -= OnRemainingTimeChanged;
        _session.MatchCountChanged -= OnMatchCountChanged;
        _session.FlipCountChanged -= OnFlipCountChanged;
      }

      _session = session;

      if (_session != null) {
        _session.RemainingTimeChanged += OnRemainingTimeChanged;
        _session.MatchCountChanged += OnMatchCountChanged;
        _session.FlipCountChanged += OnFlipCountChanged;

        OnRemainingTimeChanged(_session.RemainingTime);
        OnMatchCountChanged(_session.MatchCount);
        OnFlipCountChanged(_session.FlipCount);

        foreach (var cardState in _session.cardStates) {
          Body.Add(new CardElement(cardState, _referee));
        }
      }
    }

    void OnRemainingTimeChanged(float time) => RemainingTime.text = float.IsFinite(time) ? $"{time:F1}s" : "Unlimited";

    void OnMatchCountChanged(int count) => MatchCount.text = count.ToString();

    void OnFlipCountChanged(int count) => FlipCount.text = count.ToString();
  }
}
