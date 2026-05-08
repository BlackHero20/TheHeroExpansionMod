using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;


[Pool(typeof(NecrobinderCardPool))]
public class RigorMortis() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(2M),
        new CalculatedVar("CalculatedDoom").WithMultiplier((card, _) =>
        {
            Creature osty = card.Owner?.Osty;
            return osty == null || !osty.IsAlive ? 0M : (decimal)osty.CurrentHp;
        })
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        RigorMortis rigorMortis = this;
        ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));
        decimal doom = ((CalculatedVar)rigorMortis.DynamicVars["CalculatedDoom"]).Calculate(play.Target);
        
        if (doom <= 0) return;
        if (rigorMortis.Owner.Osty?.IsAlive == true)
            await CreatureCmd.TriggerAnim(rigorMortis.Owner.Osty, "attack_poke", 0.3f);
        await PowerCmd.Apply<DoomPower>(choiceContext, play.Target, doom, rigorMortis.Owner.Creature, rigorMortis);
    }
    
    protected override void OnUpgrade() => this.DynamicVars.CalculationExtra.UpgradeValueBy(1M);
}