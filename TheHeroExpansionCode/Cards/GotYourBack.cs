using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class GotYourBack() : TheHeroExpansionCard(3,
    CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowRedInternal => this.Owner.IsOstyMissing;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("Summon").WithMultiplier((card, _) =>
        {
            Creature osty = card.Owner?.Osty;
            return osty == null || !osty.IsAlive ? 0M : (decimal)osty.MaxHp;
        })
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, this.DynamicVars["Summon"])
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        GotYourBack gotYourBack = this;
        if (Osty.CheckMissingWithAnim(gotYourBack.Owner)) return;

        decimal summon = ((CalculatedVar)gotYourBack.DynamicVars["Summon"]).Calculate(null);

        await PowerCmd.Apply<GotYourBackPower>(choiceContext, gotYourBack.Owner.Creature,
            1M, gotYourBack.Owner.Creature, gotYourBack);
        await CreatureCmd.TriggerAnim(gotYourBack.Owner.Creature, "Cast", gotYourBack.Owner.Character.CastAnimDelay);
        await OstyCmd.Summon(choiceContext, gotYourBack.Owner, summon, gotYourBack);
    }
    
    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}