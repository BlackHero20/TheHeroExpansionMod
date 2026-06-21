using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class JoyOfCreation() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<JoyOfCreationPower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        JoyOfCreation joyOfCreation = this;
        await CreatureCmd.TriggerAnim(joyOfCreation.Owner.Creature, "PowerUp",
            joyOfCreation.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<JoyOfCreationPower>(choiceContext, joyOfCreation.Owner.Creature,
            joyOfCreation.DynamicVars["JoyOfCreationPower"].BaseValue, joyOfCreation.Owner.Creature, joyOfCreation);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}