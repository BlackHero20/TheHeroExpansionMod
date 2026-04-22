using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(SilentCardPool))]
public class ButterflyKnife() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromCard<Shiv>(this.IsUpgraded)];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DynamicVar("Shivs", 2M),
        new CardsVar(1)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ButterflyKnife butterflyKnife = this;
        await CardPileCmd.Draw(choiceContext, butterflyKnife.DynamicVars.Cards.BaseValue, butterflyKnife.Owner);
        IEnumerable<CardModel> inHand = await Shiv.CreateInHand(butterflyKnife.Owner, butterflyKnife.DynamicVars["Shivs"].IntValue, butterflyKnife.CombatState);
        if (!butterflyKnife.IsUpgraded)
            return;
        foreach (CardModel card in inHand)
            CardCmd.Upgrade(card);
    }
}