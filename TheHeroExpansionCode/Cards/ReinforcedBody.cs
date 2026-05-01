using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class ReinforcedBody() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;
    
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(8, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ReinforcedBody reinforcedBody = this;
        await CreatureCmd.TriggerAnim(reinforcedBody.Owner.Creature, "Cast", reinforcedBody.Owner.Character.CastAnimDelay);
        int xValue = reinforcedBody.ResolveEnergyXValue();
        for (int i = 0; i < xValue; ++i)
        {
            await CreatureCmd.GainBlock(reinforcedBody.Owner.Creature, reinforcedBody.DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2M);
    }
}