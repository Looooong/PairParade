using UnityEngine;

namespace PairParade {
  [System.Serializable]
  public class CardState {
    public Card card;
    public Coroutine revealCoroutine;

    public event System.Action<bool> IsMatchedChanged;
    public event System.Action<bool> IsMatchingChanged;
    public event System.Action<bool> IsFlippedChanged;

    public bool IsMatched {
      get => _isMatched;
      set {
        _isMatched = value;
        IsMatchedChanged?.Invoke(value);
      }
    }

    public bool IsMatching {
      get => _isMatching;
      set {
        _isMatching = value;
        IsMatchingChanged?.Invoke(value);
      }
    }

    public bool IsFlipped {
      get => _isFlipped;
      set {
        _isFlipped = value;
        IsFlippedChanged?.Invoke(value);
      }
    }

    [SerializeField]
    bool _isMatched;
    bool _isMatching;
    bool _isFlipped;
  }
}
