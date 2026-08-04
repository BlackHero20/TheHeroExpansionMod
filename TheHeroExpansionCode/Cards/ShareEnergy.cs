using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(TokenCardPool))]
public class ShareEnergy() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Special,
    TargetType.AnyAlly)
{
    public const string StoredEnergyKey = "StoredEnergy";

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ShareEnergy shareEnergy = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        Player? targetPlayer = cardPlay.Target.Player;
        if (targetPlayer == null) return;

        decimal storedEnergy = shareEnergy.DynamicVars.Energy.BaseValue;
        if (storedEnergy <= 0) return;

        await CreatureCmd.TriggerAnim(
            shareEnergy.Owner.Creature, "Cast",
            shareEnergy.Owner.Character.CastAnimDelay);

        await PlayerCmd.GainEnergy(storedEnergy, targetPlayer);

        // Overflow (upgraded): owner also gains the stored energy
        if (shareEnergy.IsUpgraded)
            await PlayerCmd.GainEnergy(storedEnergy, shareEnergy.Owner);
    }

    protected override void OnUpgrade()
    {
        // No stat change — the upgrade effect is the Overflow behaviour in OnPlay
    }
}
