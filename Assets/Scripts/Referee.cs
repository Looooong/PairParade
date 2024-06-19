using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PairParade {
  public class Referee : MonoBehaviour {
    public event System.Action Matched;
    public event System.Action Mismatched;
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
      var session = GameSession.Create(gameplaySettings, cards);

      foreach (var cardState in session.cardStates) {
        cardState.IsMatchingChanged += isMatching => {
          if (!isMatching) {
            cardState.IsFlipped = false;
            _selectedCardState = null;
          }

          cardState.IsFlipped = true;

          if (_selectedCardState == null) {
            _selectedCardState = cardState;
          } else if (cardState.card == _selectedCardState.card) {
            cardState.IsMatched = true;
            session.MatchCount++;
            session.FlipCount++;
            Matched.Invoke();
          } else {
            session.FlipCount++;
            Mismatched.Invoke();
          }
        };
      }

      Session = session;
      StartCoroutine(MemorizationPhase());
    }

    IEnumerator MemorizationPhase() {
      yield return new WaitForSeconds(gameplaySettings.memorizationTime);

      foreach (var cardState in Session.cardStates) {
        cardState.IsFlipped = false;
      }

      StartCoroutine(CheckRemainingTime());
    }

    IEnumerator CheckRemainingTime() {
      if (float.IsInfinity(Session.RemainingTime)) yield break;

      while (Session.RemainingTime > 0f) {
        yield return null;
        Session.RemainingTime -= Time.deltaTime;
      }

      GameOver.Invoke();
    }
  }
}
