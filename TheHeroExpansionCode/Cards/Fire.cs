using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class Fire() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MinionSheriff>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Fire fire = this;
        await CreatureCmd.TriggerAnim(fire.Owner.Creature, "Cast",
            fire.Owner.Character.CastAnimDelay);

        var sheriff = fire.CombatState.CreateCard<MinionSheriff>(fire.Owner);
        if (fire.IsUpgraded)
            CardCmd.Upgrade(sheriff);

        await CardPileCmd.AddGeneratedCardToCombat(sheriff, PileType.Hand, fire.Owner);
    }
}