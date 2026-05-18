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
public class BouncingBlade() : TheHeroExpansionCard(0,
    CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2M, ValueProp.Move),
        new PowerVar<PoisonPower>(1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        BouncingBlade bouncingBlade = this;
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        await DamageCmd.Attack(bouncingBlade.DynamicVars.Damage.BaseValue)
            .FromCard(bouncingBlade)
            .Targeting(play.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<PoisonPower>(choiceContext, play.Target,
            bouncingBlade.DynamicVars.Poison.BaseValue, bouncingBlade.Owner.Creature, bouncingBlade);

        foreach (var blade in bouncingBlade.Owner.PlayerCombatState.AllCards.OfType<BouncingBlade>())
        {
            blade.BaseReplayCount += 1;
            CardCmd.Preview(blade, 1, CardPreviewStyle.MessyLayout);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(1M);
    }
}