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
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class BottledDynamism() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<VigorPower>(9M),
        new PowerVar<CeaseToExistPower>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip> { HoverTipFactory.FromPower<VigorPower>() };
            if (this.IsUpgraded)
                tips.Add(HoverTipFactory.FromPower<CeaseToExistPower>());
            return tips;
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BottledDynamism bottledDynamism = this;
        await CreatureCmd.TriggerAnim(bottledDynamism.Owner.Creature, "Cast", bottledDynamism.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<VigorPower>(choiceContext, bottledDynamism.Owner.Creature, bottledDynamism.DynamicVars["VigorPower"].BaseValue, bottledDynamism.Owner.Creature, (CardModel) bottledDynamism);
        if (bottledDynamism.IsUpgraded)
        {
            await PowerCmd.Apply<CeaseToExistPower>(choiceContext, bottledDynamism.Owner.Creature, bottledDynamism.DynamicVars["CeaseToExistPower"].BaseValue, bottledDynamism.Owner.Creature, (CardModel) bottledDynamism);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars["VigorPower"].UpgradeValueBy(6M);
    }
}