using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Orbs;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public class KillDrivePower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        KillDrivePower killDrivePower = this;
        
        if (cardPlay.Card.Owner.Creature != this.Owner)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        if (cardPlay.Resources.EnergyValue > 0)
            return;

        this.Flash();
        for (int i = 0; i < killDrivePower.Amount; ++i)
            await OrbCmd.Channel<LightningOrb>(choiceContext, this.Owner.Player);
    }
}