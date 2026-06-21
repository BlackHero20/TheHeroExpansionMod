using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class InstantTransmission() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override int CanonicalStarCost => 2;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        InstantTransmission instantTransmission = this;
        await CreatureCmd.TriggerAnim(instantTransmission.Owner.Creature, "Cast", instantTransmission.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(instantTransmission.Owner.Creature, instantTransmission.DynamicVars.Block, cardPlay);
        CardSelectorPrefs prefs = new CardSelectorPrefs(instantTransmission.SelectionScreenPrompt, 1);
        IEnumerable<CardModel> card = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            (IReadOnlyList<CardModel>)PileType.Draw.GetPile(instantTransmission.Owner).Cards
                .OrderBy(c => c.Rarity)
                .ThenBy(c => c.Id)
                .ToList(),
            instantTransmission.Owner,
            prefs);
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3M);
    }
}