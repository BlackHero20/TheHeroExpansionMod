using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(DefectCardPool))]
public class BitFlip() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromOrb<LightningOrb>(),
        HoverTipFactory.FromOrb<FrostOrb>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        BitFlip bitFlip = this;

        var orbs = bitFlip.Owner.PlayerCombatState.OrbQueue.Orbs.ToList();
        
        var swapped = orbs
            .Select<OrbModel, Func<Task>?>(o => o switch
            {
                LightningOrb => async () => await OrbCmd.Channel<FrostOrb>(choiceContext, bitFlip.Owner),
                FrostOrb     => async () => await OrbCmd.Channel<LightningOrb>(choiceContext, bitFlip.Owner),
                _            => null
            })
            .Where(fn => fn != null)
            .ToList();

        await CreatureCmd.TriggerAnim(bitFlip.Owner.Creature, "Cast", bitFlip.Owner.Character.CastAnimDelay);
        
        for (int i = 0; i < orbs.Count; ++i)
        {
            await OrbCmd.EvokeNext(choiceContext, bitFlip.Owner);
        }
        
        foreach (var channel in swapped)
        {
            await channel!();
        }
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}