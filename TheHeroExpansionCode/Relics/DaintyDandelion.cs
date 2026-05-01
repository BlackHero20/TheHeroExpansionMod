using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;
[Pool(typeof(EventRelicPool))]
public class DaintyDandelion() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool _tookDamageThisCombat = false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        if (player != this.Owner || _tookDamageThisCombat)
            return amount;
        return amount + this.DynamicVars.Energy.BaseValue;
    }
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        _tookDamageThisCombat = false;
        this.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != this.Owner.Creature)
            return;
        if (result.UnblockedDamage <= 0)
            return;
        _tookDamageThisCombat = true;
        this.Status = RelicStatus.Disabled;
    }
}