using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(SilentCardPool))]
public class CeaseToExist() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{

    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<IntangiblePower>(1M),
        new PowerVar<CeaseToExistPower>(3M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<IntangiblePower>()];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CeaseToExist ceaseToExist = this;
        await PowerCmd.Apply<IntangiblePower>(ceaseToExist.Owner.Creature, ceaseToExist.DynamicVars["IntangiblePower"].BaseValue, ceaseToExist.Owner.Creature, (CardModel) ceaseToExist);
        await PowerCmd.Apply<CeaseToExistPower>(ceaseToExist.Owner.Creature, ceaseToExist.DynamicVars["CeaseToExistPower"].BaseValue, ceaseToExist.Owner.Creature, (CardModel) ceaseToExist);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["CeaseToExistPower"].UpgradeValueBy(-1M);
    }
}