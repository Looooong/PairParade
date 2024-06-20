using UnityEngine;

namespace PairParade {
  [System.Serializable]
  public struct GameplaySettings {
    public Vector2Int gridSize;
    public float memorizationTime;
    public float timeLimitPerPair;
  }
}
