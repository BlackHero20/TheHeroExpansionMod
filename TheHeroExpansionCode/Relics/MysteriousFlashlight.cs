using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
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

            CardModel? card = player.Character.CardPool switch
            {
                IroncladCardPool    => (CardModel?)ModelDb.Card<Exhume>(),
                SilentCardPool      => ModelDb.Card<Concentrate>(),
                DefectCardPool      => ModelDb.Card<Seek>(),
                NecrobinderCardPool => ModelDb.Card<Resent>(),
                RegentCardPool      => ModelDb.Card<Remember>(),
                _ => null
            };

            if (card == null)
                return Array.Empty<IHoverTip>();

            return new[] { HoverTipFactory.FromCard(card) };
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardName")
    ];

    public override async Task AfterObtained()
    {
        MysteriousFlashlight relic = this;
        CardModel? card = relic.Owner.Character.CardPool switch
        {
            IroncladCardPool    => relic.Owner.RunState.CreateCard(ModelDb.Card<Exhume>(), relic.Owner),
            SilentCardPool      => relic.Owner.RunState.CreateCard(ModelDb.Card<Concentrate>(), relic.Owner),
            DefectCardPool      => relic.Owner.RunState.CreateCard(ModelDb.Card<Seek>(), relic.Owner),
            NecrobinderCardPool => relic.Owner.RunState.CreateCard(ModelDb.Card<Resent>(), relic.Owner),
            RegentCardPool      => relic.Owner.RunState.CreateCard(ModelDb.Card<Remember>(), relic.Owner),
            _ => null
        };

        if (card == null)
            return;

        ((StringVar)relic.DynamicVars["CardName"]).StringValue = card.Title;
        relic._extraHoverTips = card.HoverTips.Concat(
            new[] { HoverTipFactory.FromCard(card) });

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
    }
}