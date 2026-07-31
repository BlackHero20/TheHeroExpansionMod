using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(SilentCardPool))]
public class VenomFlurry() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2M, ValueProp.Move),
        new PowerVar<PoisonPower>(1M),
        new IntVar("Replay", 1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VenomFlurry venomFlurry = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(venomFlurry.DynamicVars.Damage.BaseValue)
            .FromCard(venomFlurry, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        
        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target,
            venomFlurry.DynamicVars["PoisonPower"].BaseValue,
            venomFlurry.Owner.Creature, venomFlurry);
        
        foreach (var blade in venomFlurry.Owner.PlayerCombatState.AllCards.OfType<VenomFlurry>())
        {
            blade.BaseReplayCount += venomFlurry.DynamicVars["Replay"].IntValue;
            CardCmd.Preview(blade, venomFlurry.DynamicVars["Replay"].IntValue, CardPreviewStyle.MessyLayout);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(1M);
    }
}