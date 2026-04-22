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
        [HoverTipFactory.FromCard<MinionDuty>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<ServePower>(1M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Serve serve = this;
        await CreatureCmd.TriggerAnim(serve.Owner.Creature, "Cast", serve.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ServePower>(serve.Owner.Creature, serve.DynamicVars["ServePower"].BaseValue, serve.Owner.Creature, (CardModel) serve);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}