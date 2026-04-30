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
public class CognitionBlip() : VakuuCard(0,
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
        CognitionBlip cognitionBlip = this;
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, (Decimal) (CardPile.MaxCardsInHand - cognitionBlip.Owner.PlayerCombatState.Hand.Cards.Count), cognitionBlip.Owner);
        if (cognitionBlip.IsUpgraded)
        {
            await PowerCmd.Apply<ConfusedPower>(choiceContext, cognitionBlip.Owner.Creature, cognitionBlip.DynamicVars["ConfusedPower"].BaseValue, cognitionBlip.Owner.Creature, (CardModel) cognitionBlip);
        }
    }
}