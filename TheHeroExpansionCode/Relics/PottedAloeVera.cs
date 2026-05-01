using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
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
[Pool(typeof(SharedRelicPool))]
public class PottedAloeVera() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RegenPower>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<RegenPower>(4M),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        PottedAloeVera pottedAloeVera = this;
        if (!(room is CombatRoom))
            return;
        if (room.RoomType != RoomType.Elite)
            return;
        pottedAloeVera.Flash();
        await PowerCmd.Apply<RegenPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), pottedAloeVera.Owner.Creature, pottedAloeVera.DynamicVars["RegenPower"].BaseValue, pottedAloeVera.Owner.Creature, (CardModel) null);
    }
}