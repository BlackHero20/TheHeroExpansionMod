using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class StygianFlame() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<ConfusedPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        this.IsUpgraded ? [HoverTipFactory.FromPower<ConfusedPower>()] : [];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        StygianFlame stygianFlame = this;
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, (Decimal) (CardPile.MaxCardsInHand - stygianFlame.Owner.PlayerCombatState.Hand.Cards.Count), stygianFlame.Owner);
        if (stygianFlame.IsUpgraded)
        {
            await PowerCmd.Apply<ConfusedPower>(choiceContext, stygianFlame.Owner.Creature, stygianFlame.DynamicVars["ConfusedPower"].BaseValue, stygianFlame.Owner.Creature, (CardModel) stygianFlame);
        }
    }
}