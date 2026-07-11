using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(DefectCardPool))]
public class SelfDestruct() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Attacks", 3M),
        new DynamicVar("OrbSlots", 1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<BustedCore>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SelfDestruct selfDestruct = this;
        int attackCount = DynamicVars["Attacks"].IntValue;

        for (int i = 0; i < attackCount; i++)
        {
            var attack = PileType.Draw.GetPile(Owner).Cards
                             .Where(c => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable))
                             .ToList()
                             .StableShuffle(Owner.RunState.Rng.Shuffle)
                             .FirstOrDefault()
                         ?? PileType.Draw.GetPile(Owner).Cards
                             .Where(c => c.Type == CardType.Attack)
                             .ToList()
                             .StableShuffle(Owner.RunState.Rng.Shuffle)
                             .FirstOrDefault();

            if (attack == null) break;
            await CardCmd.AutoPlay(choiceContext, attack, null);
        }
        for (int i = 0; i < 2; ++i)
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel) selfDestruct.CombatState.CreateCard<BustedCore>(selfDestruct.Owner), PileType.Discard, selfDestruct.Owner));
        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["Attacks"].UpgradeValueBy(1M);
    }
}