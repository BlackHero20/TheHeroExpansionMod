using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class TheSavageBlade() : TheHeroExpansionCard(0,
    CardType.Attack, CardRarity.Event,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{ 
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<DefectCardPool>();
    
    private int _zeroCostCardsPlayedThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5M, ValueProp.Move),
        new DynamicVar("Increase", 2M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheSavageBlade theSavageBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(theSavageBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theSavageBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        decimal increase = theSavageBlade.DynamicVars["Increase"].BaseValue;
        foreach (Claw claw in theSavageBlade.Owner.PlayerCombatState.AllCards.OfType<Claw>())
            claw.DynamicVars.Damage.BaseValue += increase;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side)
            _zeroCostCardsPlayedThisTurn = 0;
    }

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheSavageBlade theSavageBlade = this;
        if (cardPlay.Card.Owner != theSavageBlade.Owner)
            return;
        if (cardPlay.Card == theSavageBlade)
            return;
        if (cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.Local) != 0)
            return;

        _zeroCostCardsPlayedThisTurn++;
        if (_zeroCostCardsPlayedThisTurn % 3 != 0)
            return;
        if (theSavageBlade.Pile?.Type == PileType.Hand)
            return;

        await CardPileCmd.Add(theSavageBlade, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        this.DynamicVars["Increase"].UpgradeValueBy(1M);
    }
}