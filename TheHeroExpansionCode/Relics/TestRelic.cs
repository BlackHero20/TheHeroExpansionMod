using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;
[Pool(typeof(EventRelicPool))]
public class TestRelic : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        TestRelic relic = this;
        if (!(room is CombatRoom))
            return;
        relic.Flash();
        await PowerCmd.Apply<DexterityPower>(
            (PlayerChoiceContext)new ThrowingPlayerChoiceContext(),
            relic.Owner.Creature,
            relic.DynamicVars.Dexterity.BaseValue,
            relic.Owner.Creature,
            (CardModel)null);
    }
}