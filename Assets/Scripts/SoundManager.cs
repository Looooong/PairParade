using UnityEngine;

namespace PairParade {
  [RequireComponent(typeof(AudioSource))]
  public class SoundManager : MonoBehaviour {
    public AudioClip Select;
    public AudioClip Match;
    public AudioClip Mismatch;
    public AudioClip GameOver;

    AudioSource _audioSource;

    void Awake() {
      _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable() {
      Referee.Selected += OnSelected;
      Referee.Matched += OnMatched;
      Referee.Mismatched += OnMismatched;
      Referee.GameOver += OnGameOver;
    }

    void OnDisable() {
      Referee.Selected -= OnSelected;
      Referee.Matched -= OnMatched;
      Referee.Mismatched -= OnMismatched;
      Referee.GameOver -= OnGameOver;
    }

    void OnSelected() => _audioSource.PlayOneShot(Select);

    void OnMatched() => _audioSource.PlayOneShot(Match);

    void OnMismatched() => _audioSource.PlayOneShot(Mismatch);

    void OnGameOver() => _audioSource.PlayOneShot(GameOver);
  }
}
