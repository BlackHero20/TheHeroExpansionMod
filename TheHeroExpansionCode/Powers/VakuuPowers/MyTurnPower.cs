using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;
public class MyTurnPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.None;
    
    public override async Task AfterAutoPrePlayPhaseEnteredLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        MyTurnPower power = this;
        if (player != power.Owner.Player)
            return;

        power.Flash();

        ICombatState combatState = player.Creature.CombatState;

        using (CardSelectCmd.PushSelector((ICardSelector) new VakuuCardSelector()))
        {
            int cardsPlayed = 0;
            while (cardsPlayed < 13 && !CombatManager.Instance.IsOverOrEnding && !CombatManager.Instance.IsPlayerReadyToEndTurn(player))
            {
                CardModel card = PileType.Hand.GetPile(power.Owner.Player).Cards
                    .FirstOrDefault(c => c.CanPlay());
                if (card != null)
                {
                    Creature target = GetTarget(card, combatState);
                    await card.SpendResources();
                    await CardCmd.AutoPlay(choiceContext, card, target, skipXCapture: true);
                    ++cardsPlayed;
                }
                else
                    break;
            }
        }

        await PowerCmd.Remove(power);
    }

    private Creature? GetTarget(CardModel card, ICombatState combatState)
    {
        Rng rng = this.Owner.Player.RunState.Rng.CombatTargets;
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
            TargetType.AnyPlayer => this.Owner,
            TargetType.AnyAlly => rng.NextItem<Creature>(combatState.Allies.Where(c => c != null && c.IsAlive && c.IsPlayer && c != this.Owner)),
            _ => null
        };
    }
}