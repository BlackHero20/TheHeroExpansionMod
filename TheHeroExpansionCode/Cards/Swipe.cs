using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Swipe() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override bool ShouldGlowRedInternal => this.Owner.IsOstyMissing;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new OstyDamageVar(7M, ValueProp.Move),
        new SummonVar(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, this.DynamicVars.Summon)
    ];
    
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.OstyAttack];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Swipe swipe = this;
        if (Osty.CheckMissingWithAnim(swipe.Owner))
            return;
        await DamageCmd.Attack(swipe.DynamicVars.OstyDamage.BaseValue).FromOsty(swipe.Owner.Osty, (CardModel) swipe).TargetingAllOpponents(swipe.CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        foreach (Creature _ in (IEnumerable<Creature>) swipe.CombatState.HittableEnemies)
            await OstyCmd.Summon(choiceContext, swipe.Owner, swipe.DynamicVars.Summon.BaseValue, (AbstractModel) swipe);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.OstyDamage.UpgradeValueBy(2M);
        this.DynamicVars.Summon.UpgradeValueBy(1M);
        
    }
}