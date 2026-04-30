using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class HumbleOffering() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new EnergyVar(3),
        new PowerVar<PainfulOfferingPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        HumbleOffering humbleOffering = this;
        await PlayerCmd.GainEnergy(humbleOffering.DynamicVars.Energy.BaseValue, humbleOffering.Owner);
        if (humbleOffering.IsUpgraded)
        {
            await PowerCmd.Apply<PainfulOfferingPower>(choiceContext, humbleOffering.Owner.Creature, humbleOffering.DynamicVars["PainfulOfferingPower"].BaseValue, humbleOffering.Owner.Creature, (CardModel) humbleOffering);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Energy.UpgradeValueBy(4);
    }
}