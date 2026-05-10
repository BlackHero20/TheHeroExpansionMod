using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class EdgeOfDestiny() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<EdgeOfDestinyPower>(3M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SovereignBlade>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        EdgeOfDestiny card = this;
        await PowerCmd.Apply<EdgeOfDestinyPower>(choiceContext, card.Owner.Creature,
            card.DynamicVars["EdgeOfDestinyPower"].BaseValue, card.Owner.Creature, card);
    }

    protected override void OnUpgrade() => this.DynamicVars["EdgeOfDestinyPower"].UpgradeValueBy(2M);
}