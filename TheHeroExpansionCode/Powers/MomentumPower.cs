using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public class MomentumPower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return Owner != dealer || !props.IsPoweredAttack() ? 0M : (decimal)Amount;
    }
    
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != Owner)
                return 0M;
        }
        else if (Owner != target)
            return 0M;
        return !props.IsPoweredCardOrMonsterMoveBlock() ? 0M : (decimal)Amount;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
            return;
        await PowerCmd.Apply<MomentumPower>(choiceContext, Owner, -1M, Owner, null);
    }
}