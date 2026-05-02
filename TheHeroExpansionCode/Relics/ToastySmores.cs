using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
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

    private int _extraCards = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override bool ShowCounter => true;
    public override int DisplayAmount => _extraCards;

    [SavedProperty]
    public int ExtraCards
    {
        get => _extraCards;
        set
        {
            this.AssertMutable();
            _extraCards = value;
            this.DynamicVars.Cards.BaseValue = _extraCards;
            this.InvokeDisplayAmountChanged();
            this.Status = _extraCards <= 0 ? RelicStatus.Disabled : RelicStatus.Normal;
        }
    }

    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        if (player != this.Owner || _extraCards <= 0)
            return count;
        return count + _extraCards;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (_extraCards > 0)
        {
            this.Flash();
            ExtraCards = _extraCards - 1;
        }
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is RestSiteRoom)
        {
            ExtraCards = 3;
            this.Flash();
        }
        return Task.CompletedTask;
    }
}