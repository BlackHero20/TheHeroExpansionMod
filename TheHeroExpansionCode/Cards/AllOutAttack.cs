using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class AllOutAttack() : TheHeroExpansionCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override bool ShouldGlowRedInternal => Owner.IsOstyMissing;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5M, ValueProp.Move),
        new OstyDamageVar(5M, ValueProp.Move)
    ];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.OstyAttack];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AllOutAttack card = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        bool ostyPresent = !Osty.CheckMissingWithAnim(card.Owner);
        
        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        if (ostyPresent)
            await DamageCmd.Attack(card.DynamicVars.OstyDamage.BaseValue).FromOsty(card.Owner.Osty, card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        
        if (card.IsUpgraded)
        {
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card).TargetingAllOpponents(card.CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            if (ostyPresent)
                await DamageCmd.Attack(card.DynamicVars.OstyDamage.BaseValue).FromOsty(card.Owner.Osty, card).TargetingAllOpponents(card.CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            if (ostyPresent)
                await DamageCmd.Attack(card.DynamicVars.OstyDamage.BaseValue).FromOsty(card.Owner.Osty, card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        }
    }
}