using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class Serve() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MinionDuty>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<ServePower>(1M),
        new PowerVar<UpgradedServePower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        Serve serve = this;
        await CreatureCmd.TriggerAnim(serve.Owner.Creature, "Cast", serve.Owner.Character.CastAnimDelay);

        if (serve.IsUpgraded)
            await PowerCmd.Apply<UpgradedServePower>(choiceContext, serve.Owner.Creature,
                serve.DynamicVars["UpgradedServePower"].BaseValue, serve.Owner.Creature, serve);
        else
            await PowerCmd.Apply<ServePower>(choiceContext, serve.Owner.Creature,
                serve.DynamicVars["ServePower"].BaseValue, serve.Owner.Creature, serve);
    }
}