using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(DefectCardPool))]
public class Nanomachines() : TheHeroExpansionCard(2,
    CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        Nanomachines card = this;
        await PowerCmd.Apply<NanomachinesPower>(choiceContext, card.Owner.Creature,
            card.DynamicVars.Cards.BaseValue, card.Owner.Creature, card);
    }

    protected override void OnUpgrade() => this.DynamicVars.Cards.UpgradeValueBy(1M);
}
