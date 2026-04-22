using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public class TheWorldPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override bool ShouldTakeExtraTurn(Player player)
    {
        return player == this.Owner.Player;
    }
    
    public override async Task AfterTakingExtraTurn(Player player)
    {
        TheWorldPower power = this;
        if (player != power.Owner.Player)
            return;

        this.Flash();
        await PowerCmd.TickDownDuration(power);
    }
}