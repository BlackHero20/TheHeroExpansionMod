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
[Pool(typeof(NecrobinderCardPool))]
public class Yokai() : TheHeroExpansionCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<YokaiPower>(1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Yokai yokai = this;
        await CreatureCmd.TriggerAnim(yokai.Owner.Creature, "PowerUp",
            yokai.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<YokaiPower>(choiceContext, yokai.Owner.Creature, yokai.DynamicVars["YokaiPower"].BaseValue, yokai.Owner.Creature, (CardModel) yokai);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}