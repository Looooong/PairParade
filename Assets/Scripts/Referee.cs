using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PairParade {
  public class Referee : MonoBehaviour {
    public event System.Action Selected;
    public event System.Action Matched;
    public event System.Action Mismatched;
    public event System.Action GameStarted;
    public event System.Action GameCompleted;
    public event System.Action GameOver;
    public event System.Action<GameSession> SessionChanged;

    public GameplaySettings gameplaySettings;
    public List<Card> cards;

    public GameSession Session {
      get => _session;
      private set {
        _session = value;
        SessionChanged?.Invoke(value);
      }
    }

    [SerializeField]
    GameSession _session;
    CardState _selectedCardState;

    void Start() {
      Matched += () => {
        if (Session.cardStates.TrueForAll(s => s.IsMatched)) {
          StopAllCoroutines();
          GameCompleted?.Invoke();
        }
      };
      CardState.Selected += OnCardSelected;
      Session = GameSession.Create(gameplaySettings, cards);
      StartCoroutine(MemorizationPhase());
    }

    void OnDestroy() {
      CardState.Selected -= OnCardSelected;
    }

    void OnCardSelected(CardState cardState) {
      if (cardState.revealCoroutine != null) {
        StopCoroutine(cardState.revealCoroutine);
        cardState.revealCoroutine = null;
      }

      if (_selectedCardState == null) {
        cardState.IsFlipped = true;
        _selectedCardState = cardState;
        Selected?.Invoke();
      } else if (_selectedCardState == cardState) {
        cardState.IsFlipped = false;
        _selectedCardState = null;
        Selected?.Invoke();
      } else if (_selectedCardState.card == cardState.card) {
        cardState.IsFlipped = true;
        _selectedCardState.IsMatched = cardState.IsMatched = true;
        _selectedCardState = null;
        Session.MatchCount++;
        Session.FlipCount++;
        Matched?.Invoke();
      } else {
        RevealMismatch(cardState);
        RevealMismatch(_selectedCardState);
        _selectedCardState = null;
        Session.FlipCount++;
        Mismatched?.Invoke();
      }
    }

    void RevealMismatch(CardState cardState) {
      cardState.revealCoroutine = StartCoroutine(RevealMemorizeHide(cardState));
    }

    IEnumerator RevealMemorizeHide(CardState cardState) {
      cardState.IsFlipped = true;
      yield return new WaitForSeconds(Session.settings.memorizationTime);
      cardState.IsFlipped = false;
      cardState.revealCoroutine = null;
    }

    IEnumerator MemorizationPhase() {
      yield return new WaitForSeconds(gameplaySettings.memorizationTime);

      foreach (var cardState in Session.cardStates) {
        cardState.IsFlipped = false;
      }

      GameStarted?.Invoke();
      StartCoroutine(CheckRemainingTime());
    }

    IEnumerator CheckRemainingTime() {
      if (float.IsInfinity(Session.RemainingTime)) yield break;

      while (Session.RemainingTime > 0f) {
        yield return null;
        Session.RemainingTime -= Time.deltaTime;
      }

      StopAllCoroutines();
      GameOver?.Invoke();
    }
  }
}
