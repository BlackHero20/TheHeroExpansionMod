using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class SharedReserves() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<ShareEnergy>()
    ];


    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        SharedReserves sharedReserves = this;
        if (player != sharedReserves.Owner) return;
        if (combatState.RoundNumber != 1) return;

        if (!combatState.PlayerCreatures.Any(c => c.IsAlive && c.Player != sharedReserves.Owner))
            return;

        sharedReserves.Flash();

        CardModel shareCard = sharedReserves.CombatState.CreateCard<ShareEnergy>(sharedReserves.Owner);
        shareCard.SetToFreeThisTurn();

        var added = await CardPileCmd.AddGeneratedCardToCombat(
            shareCard, PileType.Hand, sharedReserves.Owner);

        CardCmd.PreviewCardPileAdd(added);
    }


    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        SharedReserves sharedReserves = this;

        if (side != CombatSide.Player) return Task.CompletedTask;
        if (!participants.Any(c => c == sharedReserves.Owner.Creature))
            return Task.CompletedTask;

        decimal leftover = sharedReserves.Owner.PlayerCombatState.Energy;
        if (leftover <= 0) return Task.CompletedTask;

        ShareEnergy? shareCard = FindShareCard();
        if (shareCard == null) return Task.CompletedTask;

        shareCard.DynamicVars.Energy.BaseValue += leftover;
        sharedReserves.Flash();

        return Task.CompletedTask;
    }


    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }


    private ShareEnergy? FindShareCard()
    {
        foreach (var pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            var found = pileType.GetPile(Owner).Cards.OfType<ShareEnergy>().FirstOrDefault();
            if (found != null) return found;
        }
        return null;
    }
}
