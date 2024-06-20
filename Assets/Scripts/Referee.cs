using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PairParade {
  public class Referee : MonoBehaviour {
    public static event System.Action Selected;
    public static event System.Action Matched;
    public static event System.Action Mismatched;
    public static event System.Action GameStarted;
    public static event System.Action GameCompleted;
    public static event System.Action GameOver;
    public event System.Action<GameSession> SessionChanged;

    public GameplaySettingsPreset settingsPreset;
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
    bool _gameStopped;

    void Start() {
      Matched += () => {
        if (Session.cardStates.TrueForAll(s => s.IsMatched)) {
          StopAllCoroutines();
          GameCompleted?.Invoke();
        }
      };
      GameCompleted += OnGameStopped;
      GameOver += OnGameStopped;
      CardState.Selected += OnCardSelected;
      InitializeSession();
    }

    void OnDestroy() {
      CardState.Selected -= OnCardSelected;
    }

    void OnApplicationFocus(bool hasFocus) {
      if (!hasFocus && !_gameStopped) {
        PlayerPrefs.SetString(nameof(GameSession), JsonUtility.ToJson(Session));
      }
    }

    void InitializeSession() {
      if (PlayerPrefs.HasKey(nameof(GameSession))) {
        Session = JsonUtility.FromJson<GameSession>(PlayerPrefs.GetString(nameof(GameSession)));
        StartCoroutine(StartGame());

        foreach (var cardState in Session.cardStates) {
          cardState.IsFlipped = cardState.IsMatched;
        }
      } else {
        Session = GameSession.Create(settingsPreset.settings, cards);
        StartCoroutine(MemorizationPhase());
      }
    }

    void OnGameStopped() {
      _gameStopped = true;
      PlayerPrefs.DeleteKey(nameof(GameSession));
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
      yield return new WaitForSeconds(Session.settings.memorizationTime);

      foreach (var cardState in Session.cardStates) {
        cardState.IsFlipped = false;
      }

      StartCoroutine(StartGame());
    }

    IEnumerator StartGame() {
      GameStarted?.Invoke();

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
