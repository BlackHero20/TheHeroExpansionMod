using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.UI;

public static class CheatDisplay
{
    private static Control? _container;
    private static Control? _hoverContainer;
    private static Player? _trackedPlayer;
    private static int _cardCount;
    private static CardPile? _subscribedPile;

    public static void Show(Player player, int cardCount)
    {
        var drawPileButton = NCombatRoom.Instance?.Ui?.DrawPile;
        if (drawPileButton == null) return;

        if (_container == null || !GodotObject.IsInstanceValid(_container))
        {
            _container = new Control();
            _container.Name = "CheatDisplay";
            _container.MouseFilter = Control.MouseFilterEnum.Ignore;
            _container.ZIndex = 100;
            NGame.Instance.AddChild(_container);
            _container.GlobalPosition = drawPileButton.GlobalPosition + new Vector2(40f, 30f);

            _trackedPlayer = player;
            _subscribedPile = PileType.Draw.GetPile(player);
            _subscribedPile.CardAddFinished += Refresh;
            _subscribedPile.CardRemoveFinished += Refresh;

            ActiveScreenContext.Instance.Updated += OnScreenContextUpdated;
            CombatManager.Instance.CombatEnded += OnCombatEnded;
            CombatManager.Instance.CombatWon += OnCombatWon;
        }

        _cardCount = cardCount;
        Refresh();
    }

    public static void UpdateCount(int newCount)
    {
        _cardCount = newCount;
        Refresh();
    }

    public static void Hide()
    {
        HideHoverCard();

        if (_subscribedPile != null)
        {
            _subscribedPile.CardAddFinished -= Refresh;
            _subscribedPile.CardRemoveFinished -= Refresh;
            _subscribedPile = null;
        }

        ActiveScreenContext.Instance.Updated -= OnScreenContextUpdated;

        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.CombatEnded -= OnCombatEnded;
            CombatManager.Instance.CombatWon -= OnCombatWon;
        }

        if (_container != null && GodotObject.IsInstanceValid(_container))
            _container.QueueFree();

        _container = null;
        _trackedPlayer = null;
        _cardCount = 0;
    }

    private static void ShowHoverCard(CardModel card, Vector2 slotGlobalPos)
    {
        HideHoverCard();

        _hoverContainer = new Control();
        _hoverContainer.ZIndex = 200;
        _hoverContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
        NGame.Instance.AddChild(_hoverContainer);

        var hoverCard = NCard.Create(card);
        if (hoverCard == null) { _hoverContainer.QueueFree(); _hoverContainer = null; return; }

        hoverCard.MouseFilter = Control.MouseFilterEnum.Ignore;
        _hoverContainer.AddChild(hoverCard);

        const float hoverScale = 0.55f;
        hoverCard.Scale = Vector2.One * hoverScale;
        hoverCard.UpdateVisuals(PileType.Draw, CardPreviewMode.Normal);

        const float miniScale = 0.2f;
        float miniCardWidth = NCard.defaultSize.X * miniScale;
        float hoverCardHeight = NCard.defaultSize.Y * hoverScale;

        _hoverContainer.GlobalPosition = new Vector2(
            slotGlobalPos.X + miniCardWidth + 15f,
            slotGlobalPos.Y - hoverCardHeight / 2f);
    }

    private static void HideHoverCard()
    {
        if (_hoverContainer != null && GodotObject.IsInstanceValid(_hoverContainer))
            _hoverContainer.QueueFree();
        _hoverContainer = null;
    }

    private static void OnCombatEnded(CombatRoom _) => Hide();
    private static void OnCombatWon(CombatRoom _) => Hide();

    private static void OnScreenContextUpdated()
    {
        if (_container == null || !GodotObject.IsInstanceValid(_container)) return;
        if (NCombatRoom.Instance == null) { Hide(); return; }
        if (_trackedPlayer?.Creature.GetPower<CheatPower>() == null) { Hide(); return; }

        bool combatIsActive = ActiveScreenContext.Instance.IsCurrent(NCombatRoom.Instance);
        _container.Visible = combatIsActive;
        if (!combatIsActive) HideHoverCard();
    }

    private static void Refresh()
    {
        HideHoverCard();

        if (_container == null || !GodotObject.IsInstanceValid(_container) || _trackedPlayer == null)
            return;

        foreach (var child in _container.GetChildren().OfType<Control>().ToList())
            child.QueueFree();

        var topCards = PileType.Draw.GetPile(_trackedPlayer).Cards
            .Take(_cardCount)
            .ToList();

        const float scale = 0.2f;
        float cardWidth = NCard.defaultSize.X * scale;
        float cardHeight = NCard.defaultSize.Y * scale;
        const float spacing = 4f;

        for (int i = 0; i < topCards.Count; i++)
        {
            var cardPos = new Vector2(-cardWidth / 2f, -(i + 1) * (cardHeight + spacing));

            var slot = new Control();
            slot.Size = new Vector2(cardWidth, cardHeight);
            slot.Position = cardPos - new Vector2(0f, cardHeight / 2f);
            _container.AddChild(slot);

            var miniCard = NCard.Create(topCards[i]);
            if (miniCard != null)
            {
                miniCard.MouseFilter = Control.MouseFilterEnum.Ignore;
                slot.AddChild(miniCard);
                miniCard.Scale = Vector2.One * scale;
                miniCard.Position = new Vector2(cardWidth / 2f, cardHeight / 2f);
                miniCard.UpdateVisuals(PileType.Draw, CardPreviewMode.Normal);
            }
            
            var cardModel = topCards[i];
            var capturedSlot = slot;
            slot.MouseEntered += () => ShowHoverCard(cardModel, capturedSlot.GlobalPosition);
            slot.MouseExited += HideHoverCard;
        }
    }
}