using UnityEngine;

namespace PairParade
{
  [CreateAssetMenu]
  public class GameplaySettings : ScriptableObject
  {
    public Vector2Int gridSize;
    public float memorizationTime;
    public float timeLimitPerPair;

    public void CopyFrom(GameplaySettings preset)
    {
      gridSize = preset.gridSize;
      memorizationTime = preset.memorizationTime;
      timeLimitPerPair = preset.timeLimitPerPair;
    }
  }
}
