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
public class Engrave() : VakuuCard(0,
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
        CardPlay play)
    {
        Engrave engrave = this;
        
        if (engrave.IsUpgraded)
        {
            await CreatureCmd.Damage(choiceContext, engrave.Owner.Creature, engrave.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, engrave);
            foreach (CardModel card in PileType.Hand.GetPile(engrave.Owner).Cards.ToList())
            {
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
            }
        }
        else
        {
            await PowerCmd.Apply<RetainHandPower>(choiceContext, engrave.Owner.Creature, 1M, engrave.Owner.Creature, (CardModel) engrave);
        }
    }
}