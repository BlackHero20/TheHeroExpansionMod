using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class TheBrokenBlade() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    //look at extensions for cross mod compatability
    
    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    private int _playsRemaining = 5;

    [SavedProperty]
    public int PlaysRemaining
    {
        get => _playsRemaining;
        set
        {
            AssertMutable();
            _playsRemaining = value;
            DynamicVars["PlaysRemaining"].BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1M, ValueProp.Move),
        new IntVar("PlaysRemaining", _playsRemaining)
    ];

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("BladeName", GetBladeName());
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (this.IsCanonical || this.Owner == null)
                return Array.Empty<IHoverTip>();

            CardModel? card = GetBladeInstanceForCharacter(this.Owner.Character);
            return card == null ? Array.Empty<IHoverTip>() : new[] { HoverTipFactory.FromCard(card, this.IsUpgraded) };
        }
    }

    private string GetBladeName()
    {
        if (this.IsCanonical || this.Owner == null)
            return "a character specific blade";

        CardModel? card = GetBladeInstanceForCharacter(this.Owner.Character);
        return card?.Title ?? "character specific blade";
    }
    private static CardModel? GetBladeInstanceForCharacter(CharacterModel character)
    {
        switch (character)
        {
            case Ironclad: return ModelDb.Card<TheScorchedBlade>();
            case Silent: return ModelDb.Card<TheTaintedBlade>();
            case Regent: return ModelDb.Card<TheSecondBlade>();
            case Necrobinder: return ModelDb.Card<TheMarrowBlade>();
            case Defect: return ModelDb.Card<TheSavageBlade>();
        }

        if (CustomBrokenBladeExtension.CustomBladeCards.TryGetValue(
                ((AbstractModel)character).Id, out ModelId customCardId))
            return SaveUtil.CardOrDeprecated(customCardId);

        return ModelDb.Card<TheFinishedBlade>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheBrokenBlade theBrokenBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(theBrokenBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theBrokenBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        if (theBrokenBlade.PlaysRemaining == 0)
            return;

        theBrokenBlade.PlaysRemaining--;

        if (theBrokenBlade.DeckVersion is TheBrokenBlade deckVersion)
            deckVersion.PlaysRemaining = theBrokenBlade.PlaysRemaining;
    }

    public override async Task BeforeCombatRewardOffered(RewardsSet rewardsSet, CombatRoom room)
    {
        TheBrokenBlade theBrokenBlade = this;
        if (theBrokenBlade.PlaysRemaining > 0)
            return;
        if (theBrokenBlade.Pile?.Type != PileType.Deck)
            return;

        bool shouldUpgradeResult = theBrokenBlade.IsUpgraded;
        
        CardModel canonicalTarget = GetBladeInstanceForCharacter(theBrokenBlade.Owner.Character)
            ?? ModelDb.Card<TheFinishedBlade>();
        CardModel cardForTransform = theBrokenBlade.Owner.RunState.CreateCard(canonicalTarget, theBrokenBlade.Owner);

        if (shouldUpgradeResult)
            CardCmd.Upgrade(cardForTransform);

        await CardCmd.Transform(theBrokenBlade, cardForTransform);
        await Cmd.Wait(2.3f);
    }
}