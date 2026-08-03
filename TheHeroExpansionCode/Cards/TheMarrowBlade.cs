using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class TheMarrowBlade() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Event,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<NecrobinderCardPool>();
    
    protected override bool ShouldGlowRedInternal => this.Owner.IsOstyMissing;
 
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(5M, ValueProp.Move),
        new RepeatVar(3)
    ];
 
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];
 
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.OstyAttack];
 
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheMarrowBlade theMarrowBlade = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        if (Osty.CheckMissingWithAnim(theMarrowBlade.Owner))
            return;
 
        AttackCommand attackCommand = await DamageCmd.Attack(theMarrowBlade.DynamicVars.OstyDamage.BaseValue)
            .WithHitCount(theMarrowBlade.DynamicVars.Repeat.IntValue)
            .FromOsty(theMarrowBlade.Owner.Osty, theMarrowBlade, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        
        foreach (DamageResult result in attackCommand.Results.SelectMany(r => r))
        {
            if (result.TotalDamage <= 0)
                continue;
 
            await PowerCmd.Apply<DoomPower>(choiceContext, cardPlay.Target,
                (decimal)result.TotalDamage, theMarrowBlade.Owner.Creature, theMarrowBlade);
        }
    }
 
    protected override void OnUpgrade()
    {
        this.DynamicVars.OstyDamage.UpgradeValueBy(1M);
    }
}
