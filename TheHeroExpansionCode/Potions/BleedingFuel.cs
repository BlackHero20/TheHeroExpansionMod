using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace TheHeroExpansion.TheHeroExpansionCode.Potions;
[Pool(typeof(SharedPotionPool))]
public class BleedingFuel : TheHeroExpansionPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        BleedingFuel bleedingFuel = this;
        CardModel card = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Exhaust.GetPile(bleedingFuel.Owner), bleedingFuel.Owner, new CardSelectorPrefs(bleedingFuel.SelectionScreenPrompt, 1))).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        await CardPileCmd.Add(card, PileType.Hand);
    }
}