using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
using TheHeroExpansion.TheHeroExpansionCode.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class CheatPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        CheatDisplay.Show(Owner.Player, Amount);
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        CheatDisplay.Hide();
        return Task.CompletedTask;
    }
}