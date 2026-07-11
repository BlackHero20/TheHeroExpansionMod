using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using MegaCrit.Sts2.Core.Context;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class MysteriousFlashlight() : TheHeroExpansionRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    private IEnumerable<IHoverTip> _extraHoverTips = Array.Empty<IHoverTip>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (_extraHoverTips.Any())
                return _extraHoverTips;

            if (this.IsCanonical)
                return Array.Empty<IHoverTip>();

            var runState = RunManager.Instance.DebugOnlyGetState();
            var player = this.Owner ?? (runState != null ? LocalContext.GetMe((IPlayerCollection)runState) : null);

            if (player == null)
                return Array.Empty<IHoverTip>();

            CardModel? card = GetCardForPool(player.Character.CardPool);

            if (card == null)
                return Array.Empty<IHoverTip>();

            return new[] { HoverTipFactory.FromCard(card) };
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardName")
    ];

    private static CardModel? GetCardForPool(CardPoolModel pool)
    {
        return pool switch
        {
            IroncladCardPool    => ModelDb.Card<Exhume>(),
            SilentCardPool      => ModelDb.Card<Concentrate>(),
            DefectCardPool      => ModelDb.Card<Seek>(),
            NecrobinderCardPool => ModelDb.Card<Resent>(),
            RegentCardPool      => ModelDb.Card<Remember>(),
            _                   => ModelDb.Card<DejaVu>()
        };
    }

    public override async Task AfterObtained()
    {
        MysteriousFlashlight relic = this;

        var canonical = GetCardForPool(relic.Owner.Character.CardPool);
        if (canonical == null) return;

        var card = relic.Owner.RunState.CreateCard(canonical, relic.Owner);

        ((StringVar)relic.DynamicVars["CardName"]).StringValue = card.Title;
        relic._extraHoverTips = card.HoverTips.Concat(
            new[] { HoverTipFactory.FromCard(card) });

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
    }
}