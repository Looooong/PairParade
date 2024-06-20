using UnityEngine;

namespace PairParade {
  [CreateAssetMenu]
  public class GameplaySettingsPreset : ScriptableObject {
    public string displayName;
    public GameplaySettings settings;
  }
}
