using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class SacrificialDagger() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new HpLossVar(5M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        SacrificialDagger sacrificialDagger = this;
        
        if (sacrificialDagger.IsUpgraded)
        {
            await CreatureCmd.Damage(choiceContext, sacrificialDagger.Owner.Creature, sacrificialDagger.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, sacrificialDagger, cardPlay);
            foreach (CardModel card in PileType.Hand.GetPile(sacrificialDagger.Owner).Cards.ToList())
            {
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
            }
        }
        else
        {
            await PowerCmd.Apply<RetainHandPower>(choiceContext, sacrificialDagger.Owner.Creature, 1M, sacrificialDagger.Owner.Creature, (CardModel) sacrificialDagger);
        }
    }
}