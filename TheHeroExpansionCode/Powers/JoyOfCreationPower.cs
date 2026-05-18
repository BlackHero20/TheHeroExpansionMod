using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class JoyOfCreationPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        JoyOfCreationPower power = this;
        if (creator == null || creator.Creature != power.Owner)
            return;
        if (card is SovereignBlade)
            return;
        power.Flash();
        await ForgeCmd.Forge(power.Amount, power.Owner.Player, null);
    }
}