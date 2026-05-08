using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class JoyOfCreation() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ForgeVar(4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        JoyOfCreation card = this;
        await PowerCmd.Apply<JoyOfCreationPower>(choiceContext, card.Owner.Creature,
            card.DynamicVars.Forge.BaseValue, card.Owner.Creature, card);
    }

    protected override void OnUpgrade() => this.DynamicVars.Forge.UpgradeValueBy(2M);
}