using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

[Pool(typeof(TokenCardPool))]
public class VakuusPrank() : TheHeroExpansionCard(0,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<VakuusPrankPower>(15M),
    ];
    
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != this.Owner) return true;
        CardPile pile = this.Pile;
        return (pile != null ? (pile.Type != PileType.Hand ? 1 : 0) : 1) != 0
               || card is VakuusPrank
               || autoPlayType != AutoPlayType.None;
    }
    
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        VakuusPrank vakuusPrank = this;
        if (vakuusPrank.Owner.Creature.Side != side)
            return;
        if (vakuusPrank.Pile?.Type != PileType.Hand)
            return;
        TalkCmd.Play(new LocString("cards", "THEHEROEXPANSION-VAKUUS_PRANK.approval"), vakuusPrank.Owner.Creature, VfxColor.Purple);
        await CardCmd.AutoPlay(choiceContext, vakuusPrank, null, AutoPlayType.Default);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VakuusPrank vakuusPrank = this;
        await PowerCmd.Apply<VakuusPrankPower>(choiceContext, vakuusPrank.Owner.Creature, vakuusPrank.DynamicVars["VakuusPrankPower"].BaseValue, vakuusPrank.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars["VakuusPrankPower"].UpgradeValueBy(10M);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState)
    {
        if (count == 0)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        List<CardModel> vakuusPrank = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            vakuusPrank.Add((CardModel) combatState.CreateCard<VakuusPrank>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) vakuusPrank, PileType.Hand, owner);
        return (IEnumerable<CardModel>) vakuusPrank;
    }
}