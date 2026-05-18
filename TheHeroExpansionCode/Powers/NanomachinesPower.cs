using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class NanomachinesPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner.Player || cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;
        this.GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, this.Amount);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NanomachinesPower power = this;
        if (cardPlay.Card.Owner != power.Owner.Player)
            return;
        if (!power.GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out int amount) || amount <= 0)
            return;

        power.Flash();

        for (int i = 0; i < amount; i++)
        {
            CardModel attack = PileType.Draw.GetPile(power.Owner.Player).Cards
                .Where(c => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable))
                .ToList()
                .StableShuffle(power.Owner.Player.RunState.Rng.Shuffle)
                .FirstOrDefault();

            if (attack == null) break;
            await CardCmd.AutoPlay(choiceContext, attack, null);
        }
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new();
    }
}