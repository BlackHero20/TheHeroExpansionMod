using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Monsters;
using System.Linq;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class SixthFinger() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private int _handAddedThisTurn = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(2M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SixthFinger sixthFinger = this;
        await CreatureCmd.TriggerAnim(sixthFinger.Owner.Creature,
            Necrobinder.GetSummonAnimIfApplicable(sixthFinger.Owner.Character),
            Necrobinder.GetSummonDelayIfApplicable(sixthFinger.Owner.Character));
        await OstyCmd.Summon(choiceContext, sixthFinger.Owner, sixthFinger.DynamicVars.Summon.BaseValue, (AbstractModel) sixthFinger);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side)
            _handAddedThisTurn = 0;
    }

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        SixthFinger sixthFinger = this;
        if (cardPlay.Card.Owner != sixthFinger.Owner) return;

        int ostyHits = CombatManager.Instance.History.Entries
            .OfType<CreatureAttackedEntry>()
            .Where(e => e.Actor == sixthFinger.Owner.Osty && e.HappenedThisTurn(sixthFinger.CombatState))
            .Sum(e => e.DamageResults.Count);

        int threshold = ostyHits / 3;
        if (threshold > _handAddedThisTurn)
        {
            _handAddedThisTurn = threshold;
            if (sixthFinger.Pile?.Type != PileType.Hand)
                await CardPileCmd.Add(sixthFinger, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(2M);
    }
}