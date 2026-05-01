using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class PerishSongPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<DoomPower>(90M),
    ];

    public void SetDoom(decimal doom)
    {
        this.AssertMutable();
        this.DynamicVars.Doom.BaseValue = doom;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        PerishSongPower power = this;
        if (side != power.Owner.Side)
            return;

        if (power.Amount > 1)
        {
            await PowerCmd.Decrement((PowerModel)power);
        }
        else
        {
            power.Flash();

            foreach (Creature enemy in (IEnumerable<Creature>)power.CombatState.HittableEnemies)
                await PowerCmd.Apply<DoomPower>(choiceContext, enemy, power.DynamicVars.Doom.BaseValue, power.Owner, (CardModel)null);

            await PowerCmd.Apply<DoomPower>(choiceContext, power.Owner, power.DynamicVars.Doom.BaseValue, power.Owner, (CardModel)null);

            foreach (Creature ally in (IEnumerable<Creature>)power.CombatState.Allies)
            {
                if (ally != power.Owner)
                    await PowerCmd.Apply<DoomPower>(choiceContext, ally, power.DynamicVars.Doom.BaseValue, power.Owner, (CardModel)null);
            }

            await PowerCmd.Remove((PowerModel)power);
        }
    }
}