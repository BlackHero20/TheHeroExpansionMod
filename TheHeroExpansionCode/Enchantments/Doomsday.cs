using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class Doomsday : TheHeroExpansionEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) =>
        cardType == CardType.Attack;

    public override bool IsStackable => false;
    
    public override bool HasExtraCardText => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(2M)
        
    ];

    public override Decimal EnchantDamageMultiplicative(Decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 1M : 1.75M;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Doomsday doomsday = this;
        if (choiceContext == null || doomsday.Card?.Owner?.Creature == null)
            return;
        await CreatureCmd.LoseMaxHp(choiceContext, doomsday.Card.Owner.Creature, doomsday.DynamicVars.MaxHp.BaseValue, true);
    }
}