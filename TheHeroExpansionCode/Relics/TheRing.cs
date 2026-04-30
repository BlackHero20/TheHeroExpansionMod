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

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        TheRing relic = this;

        if (player != relic.Owner)
            return;

        relic.Flash();

        int roll = relic.Owner.RunState.Rng.CombatTargets.NextInt(1, 9);

        switch (roll)
        {
            case 1:
                await relic.Effect1(choiceContext, combatState);
                break;
            case 2:
                await relic.Effect2(choiceContext);
                break;
            case 3:
                await relic.Effect3(choiceContext, combatState);
                break;
            case 4:
                await relic.Effect4(choiceContext, combatState);
                break;
            case 5:
                await relic.Effect5(choiceContext, combatState);
                break;
            case 6:
                await relic.Effect6(choiceContext, combatState);
                break;
            case 7:
                await relic.Effect7(choiceContext, combatState);
                break;
            case 8:
                await relic.Effect8(choiceContext, combatState);
                break;
            default:
                break;
        }
    }

    private async Task Effect1(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<VakuusPrank>(theRing.Owner, 1, combatState);
    }

    private async Task Effect2(PlayerChoiceContext choiceContext)
    {
        TheRing theRing = this;
        TalkCmd.Play(new LocString("powers", "THEHEROEXPANSION-MY_TURN_POWER.approval"), theRing.Owner.Creature, VfxColor.Purple);
        await PowerCmd.Apply<MyTurnPower>(choiceContext, theRing.Owner.Creature, 1M, theRing.Owner.Creature, null);
    }

    private async Task Effect3(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<LunchBreak>(theRing.Owner, 1, combatState);
    }
    
    private async Task Effect4(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<DemonicSurge>(theRing.Owner, 1, combatState);
    }
    
    private async Task Effect5(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<QuickThought>(theRing.Owner, 1, combatState);
    }
    
    private async Task Effect6(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<HumbleOffering>(theRing.Owner, 1, combatState);
    }
    
    private async Task Effect7(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<CognitionBlip>(theRing.Owner, 1, combatState);
    }
    
    private async Task Effect8(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        TheRing theRing = this;
        await VakuuCard.CreateInHand<Engrave>(theRing.Owner, 1, combatState);
    }
}