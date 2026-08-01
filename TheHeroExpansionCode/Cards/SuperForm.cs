using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(ColorlessCardPool))]
public class SuperForm() : TheHeroExpansionCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self, nameof(SuperForm))
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<MomentumPower>(6),
        new PowerVar<SuperFormPower>(3),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MomentumPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SuperForm superForm = this;
        await CreatureCmd.TriggerAnim(superForm.Owner.Creature, "PowerUp", superForm.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<MomentumPower>(choiceContext, superForm.Owner.Creature, superForm.DynamicVars["MomentumPower"].BaseValue, superForm.Owner.Creature, (CardModel) superForm);
        await PowerCmd.Apply<SuperFormPower>(choiceContext, superForm.Owner.Creature, superForm.DynamicVars["SuperFormPower"].BaseValue, superForm.Owner.Creature, (CardModel) superForm);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["MomentumPower"].UpgradeValueBy(2M);
        this.DynamicVars["SuperFormPower"].UpgradeValueBy(1M);
    }
}