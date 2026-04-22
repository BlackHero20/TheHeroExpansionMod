
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(SilentCardPool))]
public class BehindYou() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<WeakPower>(2M),
        new PowerVar<VulnerablePower>(2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BehindYou behindYou = this;
        ArgumentNullException.ThrowIfNull((object) cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(behindYou.Owner.Creature, "Cast", behindYou.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(behindYou.Owner.Creature, "vfx/vfx_flying_slash");
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, behindYou.DynamicVars.Weak.BaseValue, behindYou.Owner.Creature, (CardModel)behindYou);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, behindYou.DynamicVars.Vulnerable.BaseValue, behindYou.Owner.Creature, (CardModel)behindYou);

    }
    
    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Sly);
    }
}