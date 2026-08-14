using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(RegentCardPool))]
public class Fire() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self, nameof(Fire))
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MinionSheriff>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Fire fire = this;
        await CreatureCmd.TriggerAnim(fire.Owner.Creature, "Cast",
            fire.Owner.Character.CastAnimDelay);

        int count = fire.DynamicVars.Cards.IntValue;
        List<CardModel> sheriffs = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            var sheriff = fire.CombatState.CreateCard<MinionSheriff>(fire.Owner);
            if (fire.IsUpgraded)
                CardCmd.Upgrade(sheriff);
            sheriffs.Add(sheriff);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(sheriffs, PileType.Hand, fire.Owner);
    }
}