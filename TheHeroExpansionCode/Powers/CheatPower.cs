using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.UI;
using System.Threading.Tasks;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class CheatPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (LocalContext.IsMe(Owner))
            CheatDisplay.Show(Owner.Player, Amount);
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        if (LocalContext.IsMe(oldOwner))
            CheatDisplay.Hide();
        return Task.CompletedTask;
    }
}