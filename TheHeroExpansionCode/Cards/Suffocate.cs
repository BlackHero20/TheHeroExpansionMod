using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class Suffocate() : TheHeroExpansionCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new PowerVar<WeakPower>(1M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Suffocate suffocate = this;
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

        await DamageCmd.Attack(suffocate.DynamicVars.Damage.BaseValue)
            .FromCard(suffocate)
            .Targeting(play.Target)
            .Execute(choiceContext);

        if (play.Target.HasPower<VulnerablePower>())
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, play.Target,
                suffocate.DynamicVars["WeakPower"].BaseValue,
                suffocate.Owner.Creature, suffocate);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(1M);
        this.DynamicVars.Weak.UpgradeValueBy(1M);
    }
}