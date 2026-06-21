using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class JoyOfCreationPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => GetInternalData<Data>()?.cardsCreatedThisTurn ?? 0;

    private static bool _isCreating = false;

    protected override object InitInternalData() => new Data();

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        JoyOfCreationPower power = this;
        if (creator == null || creator.Creature != power.Owner) return;
        if (_isCreating) return;

        var data = power.GetInternalData<Data>();
        data.cardsCreatedThisTurn++;
        power.InvokeDisplayAmountChanged();

        if (data.cardsCreatedThisTurn != 3) return;

        power.Flash();

        _isCreating = true;
        try
        {
            for (int i = 0; i < power.Amount; i++)
            {
                await CardPileCmd.AddGeneratedCardToCombat(
                    card.CreateClone(), PileType.Hand, power.Owner.Player);
            }
        }
        finally
        {
            _isCreating = false;
        }

        data.cardsCreatedThisTurn = 0;
        power.InvokeDisplayAmountChanged();
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        GetInternalData<Data>().cardsCreatedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private class Data
    {
        public int cardsCreatedThisTurn;
    }
}