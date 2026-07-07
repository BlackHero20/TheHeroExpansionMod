using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(TokenCardPool))]
public class MinionDuty() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Token,
    TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(6M, ValueProp.Move),
            new BlockVar(5M, ValueProp.Move)
        ];

    protected override HashSet<CardTag> CanonicalTags =>
        [CardTag.Minion];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        MinionDuty minionDuty = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(minionDuty.DynamicVars.Damage.BaseValue)
            .FromCard(minionDuty, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(minionDuty.Owner.Creature, minionDuty.DynamicVars.Block, cardPlay);
        
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        this.DynamicVars.Block.UpgradeValueBy(2M);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState)
    {
        if (count == 0)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        List<CardModel> minionDuty = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            minionDuty.Add((CardModel) combatState.CreateCard<MinionDuty>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) minionDuty, PileType.Hand, owner);
        return (IEnumerable<CardModel>) minionDuty;
    }
}