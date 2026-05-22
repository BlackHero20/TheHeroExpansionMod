using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models.RelicPools;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;
[Pool(typeof(EventRelicPool))]
public class ToastySmores() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool _hadLeftoverEnergy;

    private bool HadLeftoverEnergy
    {
        get => _hadLeftoverEnergy;
        set
        {
            this.AssertMutable();
            _hadLeftoverEnergy = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return Task.CompletedTask;
        HadLeftoverEnergy = this.Owner.PlayerCombatState.Energy > 0;
        return Task.CompletedTask;
    }

    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        if (player != this.Owner || !HadLeftoverEnergy)
            return count;
        this.Flash();
        HadLeftoverEnergy = false;
        return count + this.DynamicVars.Cards.BaseValue;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        HadLeftoverEnergy = false;
        return Task.CompletedTask;
    }
}