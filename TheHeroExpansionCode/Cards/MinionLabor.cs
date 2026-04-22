using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(TokenCardPool))]
public class MinionLabor() : TheHeroExpansionCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new StarsVar(1),
        new ForgeVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromForge();
    
    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Minion];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        MinionLabor minionLabor = this;
        await PlayerCmd.GainStars(minionLabor.DynamicVars.Stars.BaseValue, minionLabor.Owner);
        await ForgeCmd.Forge(minionLabor.DynamicVars.Forge.IntValue, minionLabor.Owner, (AbstractModel) minionLabor);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Stars.UpgradeValueBy(1);
        this.DynamicVars.Forge.UpgradeValueBy(2);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        CombatState combatState)
    {
        if (count == 0)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        List<CardModel> minionLabor = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            minionLabor.Add((CardModel) combatState.CreateCard<MinionLabor>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) minionLabor, PileType.Hand, true);
        return (IEnumerable<CardModel>) minionLabor;
    }
}