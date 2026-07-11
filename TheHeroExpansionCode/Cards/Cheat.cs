using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;
using TheHeroExpansion.TheHeroExpansionCode.UI;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(SilentCardPool))]
public class Cheat() : TheHeroExpansionCard(0,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CheatPower>(2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Cheat cheat = this;
        await CreatureCmd.TriggerAnim(cheat.Owner.Creature, "PowerUp",
            cheat.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<CheatPower>(choiceContext, cheat.Owner.Creature,
            cheat.DynamicVars["CheatPower"].BaseValue, cheat.Owner.Creature, cheat);
        
        var power = cheat.Owner.Creature.GetPower<CheatPower>();
        if (power != null && LocalContext.IsMe(cheat.Owner.Creature))
            CheatDisplay.UpdateCount(power.Amount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CheatPower"].UpgradeValueBy(1M);
    }
}