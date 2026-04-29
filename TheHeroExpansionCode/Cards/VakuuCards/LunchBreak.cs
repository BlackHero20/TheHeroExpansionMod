using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

[Pool(typeof(TokenCardPool))]
public class LunchBreak() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override bool ShouldGlowRedInternal => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != this.Owner) return true;
        CardPile pile = this.Pile;
        return (pile != null ? (pile.Type != PileType.Hand ? 1 : 0) : 1) != 0
               || card is LunchBreak
               || autoPlayType != AutoPlayType.None;
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
        List<CardModel> lunchBreak = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            lunchBreak.Add((CardModel) combatState.CreateCard<LunchBreak>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) lunchBreak, PileType.Hand, owner);
        return (IEnumerable<CardModel>) lunchBreak;
    }
}