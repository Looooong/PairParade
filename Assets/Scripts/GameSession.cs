using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PairParade {
  [System.Serializable]
  public class GameSession {
    public GameplaySettings settings;
    public List<CardState> cardStates;

    public event System.Action<int> MatchCountChanged;
    public event System.Action<int> FlipCountChanged;
    public event System.Action<float> RemainingTimeChanged;

    public int MatchCount {
      get => _matchCount;
      set {
        _matchCount = value;
        MatchCountChanged?.Invoke(value);
      }
    }

    public int FlipCount {
      get => _flipCount;
      set {
        _flipCount = value;
        FlipCountChanged?.Invoke(value);
      }
    }

    public float RemainingTime {
      get => _remainingTime;
      set {
        _remainingTime = value;
        RemainingTimeChanged?.Invoke(value);
      }
    }


    [SerializeField]
    int _matchCount;
    [SerializeField]
    int _flipCount;
    [SerializeField]
    float _remainingTime;

    public static GameSession Create(GameplaySettings settings, List<Card> cards) {
      var cardCount = settings.gridSize.x * settings.gridSize.y;
      cardCount -= cardCount % 2;

      var chosenCards = new List<Card>(cards);

      for (var i = cardCount / 2; i < cards.Count; i++) {
        chosenCards.RemoveAtSwapBack(Random.Range(0, chosenCards.Count));
      }

      chosenCards.AddRange(chosenCards);

      for (var i = 0; i < cardCount; i++) {
        var j = Random.Range(0, cardCount);
        (chosenCards[i], chosenCards[j]) = (chosenCards[j], chosenCards[i]);
      }

      var session = new GameSession() {
        settings = settings,
        cardStates = new(cardCount),
        RemainingTime = settings.timeLimitPerPair * cardCount / 2,
      };

      for (var i = 0; i < cardCount; i++) {
        session.cardStates.Add(new() {
          card = chosenCards[i],
          IsFlipped = true,
        });
      }

      return session;
    }

    public static GameSession Load(string json) => JsonUtility.FromJson<GameSession>(json);

    public static string Save(GameSession session) => JsonUtility.ToJson(session);
  }
}