using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansion.images.card_portraits.big;
[Pool(typeof(ColorlessCardPool))]
public class JustLucky() : TheHeroExpansionCard(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy, nameof(JustLucky))
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MomentumPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(4, ValueProp.Move),
        new BlockVar(3, ValueProp.Move),
        new PowerVar<MomentumPower>(2),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        JustLucky justLucky = this;
        await DamageCmd.Attack(justLucky.DynamicVars.Damage.BaseValue).FromCard(justLucky, cardPlay).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await CreatureCmd.GainBlock(justLucky.Owner.Creature, justLucky.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<MomentumPower>(choiceContext, justLucky.Owner.Creature, justLucky.DynamicVars["MomentumPower"].BaseValue, justLucky.Owner.Creature, (CardModel) justLucky);
        await CardPileCmd.Draw(choiceContext, justLucky.DynamicVars.Cards.BaseValue, justLucky.Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2M);
        this.DynamicVars.Damage.UpgradeValueBy(2M);
    }
}