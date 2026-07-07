using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(SilentCardPool))]
public class VolcanicViper() : TheHeroExpansionCard(3,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13M, ValueProp.Move),
        new PowerVar<PoisonPower>(8M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VolcanicViper volcanicViper = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(volcanicViper.DynamicVars.Damage.BaseValue)
            .FromCard(volcanicViper, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target,
            volcanicViper.DynamicVars["PoisonPower"].BaseValue,
            volcanicViper.Owner.Creature, volcanicViper);
        
        var poison = cardPlay.Target.GetPower<PoisonPower>();
        if (poison != null && poison.Amount > 0)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target,
                (decimal)poison.Amount,
                ValueProp.Unblockable | ValueProp.Unpowered,
                cardPlay.Target, volcanicViper, cardPlay);
            await PowerCmd.Decrement(poison);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        this.DynamicVars["PoisonPower"].UpgradeValueBy(2M);
    }
}