using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheHeroExpansion.TheHeroExpansionCode.Relics;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
using TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;


[Pool(typeof(EventRelicPool))]
public class TheRing : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        TheRing relic = this;

        if (player != relic.Owner)
            return;

        relic.Flash();

        int roll = relic.Owner.RunState.Rng.CombatTargets.NextInt(1, 3);

        switch (roll)
        {
            case 1:
                await relic.Effect1(choiceContext);
                break;
            case 2:
                await relic.Effect2(choiceContext);
                break;
            case 3:
                await relic.Effect3(choiceContext);
                break;
            // ... up to 20
            default:
                break;
        }
    }

    private async Task Effect1(PlayerChoiceContext choiceContext)
    {
        TheRing theRing = this;
        await VakuusPrank.CreateInHand(theRing.Owner, 1, theRing.Owner.Creature.CombatState);
    }

    private async Task Effect2(PlayerChoiceContext choiceContext)
    {
        TheRing theRing = this;
        TalkCmd.Play(new LocString("powers", "THEHEROEXPANSION-MY_TURN_POWER.approval"), theRing.Owner.Creature, VfxColor.Purple);
        await PowerCmd.Apply<MyTurnPower>(choiceContext, theRing.Owner.Creature, 1M, theRing.Owner.Creature, null);
    }

    private Task Effect3(PlayerChoiceContext choiceContext)
    {
        // TODO
        return Task.CompletedTask;
    }
}