using UnityEngine;
using UnityEngine.UIElements;

namespace PairParade.UI {
  public class CardElement : VisualElement {
    VisualElement FrontFace => this.Q("front-face");
    VisualElement BackFace => this.Q("back-face");

    CardState _state;

    public CardElement(CardState cardState, Referee referee) {
      _state = cardState;

      var template = Resources.Load<VisualTreeAsset>("UI/CardElement/CardElement");
      template.CloneTree(this);

      var gridSize = referee.Session.settings.gridSize;
      style.flexBasis = new Length(100f / gridSize.x, LengthUnit.Percent);
      style.height = new Length(100f / gridSize.y, LengthUnit.Percent);
      FrontFace.style.backgroundImage = new(cardState.card.frontFace);
      BackFace.style.backgroundImage = new(cardState.card.backFace);

      RegisterCallback<AttachToPanelEvent>(e => {
        referee.GameStarted += OnGameStarted;
        cardState.IsMatchedChanged += OnIsMatchedChanged;
        cardState.IsFlippedChanged += OnIsFlippedChanged;
        OnIsMatchedChanged(cardState.IsMatched);
        OnIsFlippedChanged(cardState.IsFlipped);
      });
      RegisterCallback<DetachFromPanelEvent>(e => {
        referee.GameStarted -= OnGameStarted;
        cardState.IsMatchedChanged -= OnIsMatchedChanged;
        cardState.IsFlippedChanged -= OnIsFlippedChanged;
      });
    }

    void OnGameStarted() {
      RegisterCallback<PointerDownEvent>(e => {
        _state.IsMatching = !_state.IsMatching;
      });
    }

    void OnIsMatchedChanged(bool isMatched) {
      SetEnabled(!isMatched);
    }

    void OnIsFlippedChanged(bool isFlipped) {
      FrontFace.visible = isFlipped;
      BackFace.visible = !isFlipped;
    }
  }
}
