using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class EdgeOfDestinyPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        EdgeOfDestinyPower power = this;
        if (cardPlay.Card is not SovereignBlade || cardPlay.Card.Owner.Creature != power.Owner)
            return Task.CompletedTask;

        int stars = power.Owner.Player.PlayerCombatState.Stars;
        decimal bonus = power.Amount * stars;
        power.GetInternalData<Data>().addedBonus = bonus;
        power.GetInternalData<Data>().target = cardPlay.Target;
        UpdateSovereignBladeVars();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        EdgeOfDestinyPower power = this;

        if (cardPlay.Card is SovereignBlade sb && cardPlay.Card.Owner.Creature == power.Owner)
        {
            decimal bonus = power.GetInternalData<Data>().addedBonus;

            if (bonus > 0)
            {
                power.Flash();
                var attackCmd = DamageCmd.Attack(bonus).FromCard(sb);
                await Cmd.Wait(0.1f);

                if (power.Owner.HasPower<SeekingEdgePower>())
                    await attackCmd.TargetingAllOpponents(power.CombatState).Execute(choiceContext);
                else if (power.GetInternalData<Data>().target != null)
                    await attackCmd.Targeting(power.GetInternalData<Data>().target).Execute(choiceContext);
            }

            power.GetInternalData<Data>().addedBonus = 0;
            power.GetInternalData<Data>().target = null;
        }

        if (cardPlay.Card.Type == CardType.Power && cardPlay.Card.Owner == Owner.Player)
            UpdateSovereignBladeVars();
    }

    private class Data
    {
        public decimal addedBonus = 0;
        public Creature? target = null;
    }

    public override Task AfterStarsGained(int amount, Player gainer)
    {
        if (gainer == Owner.Player) UpdateSovereignBladeVars();
        return Task.CompletedTask;
    }

    public override Task AfterStarsSpent(int amount, Player spender)
    {
        if (spender == Owner.Player) UpdateSovereignBladeVars();
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player) UpdateSovereignBladeVars();
        return Task.CompletedTask;
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is SovereignBlade && creator == Owner.Player)
            UpdateSovereignBladeVars();
        return Task.CompletedTask;
    }

    private void UpdateSovereignBladeVars()
    {
        if (!CombatManager.Instance.IsInProgress) return;
        int stars = Owner.Player.PlayerCombatState.Stars;
        decimal damage = stars > 0 ? (decimal)stars * Amount : 0M;
        bool hasSeekingEdge = Owner.HasPower<SeekingEdgePower>();

        foreach (var pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var card in pileType.GetPile(Owner.Player).Cards.OfType<SovereignBlade>())
            {
                try
                {
                    card.DynamicVars["EdgeOfDestinyDamage"].BaseValue = damage;
                    card.DynamicVars["HasEdgeOfDestiny"].BaseValue = 1M;
                    ((StringVar)card.DynamicVars["SeekingEdgeSuffix"]).StringValue =
                        hasSeekingEdge ? " to ALL enemies" : "";
                }
                catch { }
            }
        }
    }
}