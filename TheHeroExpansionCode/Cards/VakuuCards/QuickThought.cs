using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

[Pool(typeof(TokenCardPool))]
public class QuickThought() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override bool ShouldGlowRedInternal => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(12M, ValueProp.Move),
        new PowerVar<TheGambitPower>(1M)
    ];
    
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != this.Owner) return true;
        CardPile pile = this.Pile;
        return (pile != null ? (pile.Type != PileType.Hand ? 1 : 0) : 1) != 0
               || card is QuickThought
               || autoPlayType != AutoPlayType.None;
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        QuickThought quickThought = this;
        await CreatureCmd.GainBlock(quickThought.Owner.Creature, quickThought.DynamicVars.Block, cardPlay);
        if (quickThought.IsUpgraded)
        {
            await PowerCmd.Apply<TheGambitPower>(choiceContext, quickThought.Owner.Creature, quickThought.DynamicVars["TheGambitPower"].BaseValue, quickThought.Owner.Creature, (CardModel) quickThought);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(23M);
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
        List<CardModel> quickThought = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            quickThought.Add((CardModel) combatState.CreateCard<QuickThought>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) quickThought, PileType.Hand, owner);
        return (IEnumerable<CardModel>) quickThought;
    }
}