using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public sealed class GoldenRatio : TheHeroExpansionEnchantment
{
    public override bool IsStackable => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1) //exists for localization only
    ];

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        GoldenRatio enchantment = this;

        if (enchantment.Card == null)
            return;

        var owner = enchantment.Card.Owner;
        if (owner == null || owner != player)
            return;

        if (owner.PlayerCombatState.Energy <= 0)
            return;

        var card = enchantment.Card;

        card.SetToFreeThisTurn();
        await CardCmd.AutoPlay(choiceContext, card, null, AutoPlayType.Default);
    }
}