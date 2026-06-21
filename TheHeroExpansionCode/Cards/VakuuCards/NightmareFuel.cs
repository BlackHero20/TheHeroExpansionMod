using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class NightmareFuel() : VakuuCard(0, CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("Corrupted", 1),
        new DynamicVar("Doomsday", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        this.IsUpgraded 
            ? HoverTipFactory.FromEnchantment<Doomsday>() 
            : HoverTipFactory.FromEnchantment<Corrupted>();
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        NightmareFuel nightmareFuel = this;

        foreach (var card in PileType.Hand.GetPile(nightmareFuel.Owner).Cards.ToList())
        {
            if (card.Enchantment == null && card.Type == CardType.Attack)
                if (nightmareFuel.IsUpgraded)
                {
                    CardCmd.Enchant<Doomsday>(card, 1M);
                }
                else
                {
                    CardCmd.Enchant<Corrupted>(card, 1M);
                }
        }
    }
}