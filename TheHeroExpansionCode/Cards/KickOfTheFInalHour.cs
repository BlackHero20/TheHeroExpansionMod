using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class KickOfTheFinalHour() : TheHeroExpansionCard(5,
    CardType.Attack, CardRarity.Rare,
    TargetType.RandomEnemy, nameof(KickOfTheFinalHour))
{
    private const int TriggerThreshold = 11;
    private int _cardsUntilDiscount = TriggerThreshold;
    private bool _discountActive; 
    private int _pendingDiscountAmount;

    protected override bool ShouldGlowGoldInternal => this._discountActive;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12M, ValueProp.Move),
        new RepeatVar(4),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CardsUntilDiscount").WithMultiplier((card, _) =>
            ((KickOfTheFinalHour)card)._cardsUntilDiscount)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        KickOfTheFinalHour kickOfTheFinalHour = this;
        await DamageCmd.Attack(kickOfTheFinalHour.DynamicVars.Damage.BaseValue)
            .WithHitCount(kickOfTheFinalHour.DynamicVars.Repeat.IntValue)
            .FromCard(kickOfTheFinalHour, cardPlay)
            .TargetingRandomOpponents(kickOfTheFinalHour.CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        KickOfTheFinalHour kickOfTheFinalHour = this;
        if (cardPlay.Card.Owner != kickOfTheFinalHour.Owner)
            return;

        bool isSelf = cardPlay.Card == kickOfTheFinalHour;

        if (_discountActive)
        {
            if (isSelf)
            {
                _discountActive = false;
                _cardsUntilDiscount = TriggerThreshold;
            }
            else
            {
                kickOfTheFinalHour.EnergyCost.AddUntilPlayed(_pendingDiscountAmount);
                _discountActive = false;
                _pendingDiscountAmount = 0;
                _cardsUntilDiscount = TriggerThreshold;
            }
            return;
        }
        
        _cardsUntilDiscount--;
        if (_cardsUntilDiscount > 0)
            return;

        int currentCost = (int)kickOfTheFinalHour.EnergyCost.GetWithModifiers(CostModifiers.Local);
        if (currentCost <= 0)
            return;

        kickOfTheFinalHour.EnergyCost.AddUntilPlayed(-currentCost);
        _pendingDiscountAmount = currentCost;
        _discountActive = true;
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Repeat.UpgradeValueBy(1M);
    }
}