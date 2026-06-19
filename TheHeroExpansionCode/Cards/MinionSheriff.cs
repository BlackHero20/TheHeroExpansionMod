using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using System.Linq;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(TokenCardPool))]
public class MinionSheriff() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Minion];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MinionSheriff card = this;

        var minionCards = PileType.Exhaust.GetPile(card.Owner).Cards
            .Where(c => c.Tags.Contains(CardTag.Minion) && c != card && c is not MinionSheriff)
            .ToList();

        bool first = true;
        foreach (var minion in minionCards)
        {
            if (card.IsUpgraded && minion.IsUpgradable)
                CardCmd.Upgrade(minion, CardPreviewStyle.None);

            await CardCmd.AutoPlay(choiceContext, minion, null, skipCardPileVisuals: false);
        }
    }

    protected override void OnUpgrade() { }
}