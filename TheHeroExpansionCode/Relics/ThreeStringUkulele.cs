using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using TheHeroExpansion.TheHeroExpansionCode.Relics;

namespace TheHeroExpansion.TheHeroExpansionCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class ThreeStringUkulele() : TheHeroExpansionRelic
{
    private bool _activatedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool ActivatedThisCombat
    {
        get => this._activatedThisCombat;
        set
        {
            this.AssertMutable();
            this._activatedThisCombat = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        this.ActivatedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ThreeStringUkulele relic = this;
        if (!CombatManager.Instance.IsInProgress || cardPlay.Card.Owner != relic.Owner || cardPlay.Card.Type != CardType.Power || relic.ActivatedThisCombat)
            return;
        relic.Flash();
        await PlayerCmd.GainEnergy(relic.DynamicVars.Energy.BaseValue, relic.Owner);
        relic.ActivatedThisCombat = true;
    }
}