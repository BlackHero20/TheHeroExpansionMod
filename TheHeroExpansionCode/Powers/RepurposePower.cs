using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;
public sealed class RepurposePower : TheHeroExpansionPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private decimal _advantageousAmount = 2M;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Advantageous", _advantageousAmount)
    ];

    public void SetAdvantageous(decimal amount)
    {
        this.AssertMutable();
        _advantageousAmount = amount;
        this.DynamicVars["Advantageous"].BaseValue = amount;
    }

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        RepurposePower power = this;
        if (player != power.Owner.Player)
            return;
        if (!Hook.ShouldFlush(player.Creature.CombatState, player))
            return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(power.SelectionScreenPrompt, 0, power.Amount);
        List<CardModel> selected = (await CardSelectCmd.FromHand(
            choiceContext, player, prefs,
            card => card.Enchantment == null &&
                    (card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power),
            (AbstractModel)power)).ToList();

        foreach (CardModel card in selected)
        {
            card.GiveSingleTurnRetain();
            CardCmd.Enchant<Advantageous>(card, _advantageousAmount);
        }

        await PowerCmd.Remove(power);
    }
}
