using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PairParade.Editor {
  public static class PairParadeMenu {
    [MenuItem("Pair Parade/Log game session")]
    static void LogGameSession() {
      Debug.Log(PlayerPrefs.HasKey(nameof(GameSession)) ? PlayerPrefs.GetString(nameof(GameSession)) : $"{nameof(GameSession)} is not set");
    }

    [MenuItem("Pair Parade/Clear game session")]
    static void ClearGameSession() {
      PlayerPrefs.DeleteKey(nameof(GameSession));
      Debug.Log($"{nameof(GameSession)} cleared!");
    }

    [MenuItem("Pair Parade/Convert selected Sprites to Cards")]
    static void ConvertSelectedSpritesToCard() {
      var folder = EditorUtility.SaveFolderPanel("Choose folder to save the cards", "Assets", null);

      if (string.IsNullOrEmpty(folder)) return;

      var sprites = GetSpritesFromSelection().ToList();

      try {
        for (var i = 0; i < sprites.Count; i++) {
          var sprite = sprites[i];
          EditorUtility.DisplayProgressBar($"Converting... ({i + 1}/{sprites.Count})", sprite.name, (i + 1f) / sprites.Count);

          var card = ScriptableObject.CreateInstance<Card>();
          card.frontFace = sprite;

          var relativePath = Path.GetRelativePath(Application.dataPath, folder);
          AssetDatabase.CreateAsset(card, Path.Combine("Assets", relativePath, $"{sprite.name}.asset"));
        }
      } finally {
        EditorUtility.ClearProgressBar();
      }
    }

    [MenuItem("Pair Parade/Convert selected Sprites to Cards", true)]
    static bool ValidateConvertSelectedSpritesToCard() => GetSpritesFromSelection().Any();

    static IEnumerable<Sprite> GetSpritesFromSelection() => Selection.objects.OfType<Sprite>();
  }
}
