using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(EventCardPool))]
public class TheSecondBlade() : TheHeroExpansionCard(2,
    CardType.Attack, CardRarity.Event,
    TargetType.AnyEnemy, nameof(TheBrokenBlade))
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<RegentCardPool>();
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];
    
    public override TargetType TargetType =>
        !HasSeekingEdge ? TargetType.AnyEnemy : TargetType.AllEnemies;

    public override bool GainsBlock => GetOwnerParryAmount(this) > 0M;

    private bool HasSeekingEdge =>
        IsMutable && Owner != null && Owner.Creature.HasPower<SeekingEdgePower>();

    private static decimal GetOwnerParryAmount(CardModel card) =>
        !card.IsMutable || card.Owner == null || card.Pile == null || !card.Pile.IsCombatPile
            ? 0M
            : (decimal)card.Owner.Creature.GetPowerAmount<ParryPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => GetOwnerParryAmount(card)),
        new RepeatVar(1),
        new DamageVar("EdgeOfDestinyDamage", 0M, ValueProp.Move),
        new DynamicVar("HasEdgeOfDestiny", 0M),
        new StringVar("SeekingEdgeSuffix")
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TheSecondBlade theSecondBlade = this;
        int swordSageStacks = (int)theSecondBlade.Owner.Creature.GetPowerAmount<SwordSagePower>();
        int hitCount = theSecondBlade.DynamicVars.Repeat.IntValue + swordSageStacks;

        AttackCommand attackCommand1 = DamageCmd.Attack(theSecondBlade.DynamicVars.Damage.BaseValue)
            .FromCard(theSecondBlade, cardPlay)
            .WithHitCount(hitCount);

        AttackCommand attackCommand2;
        if (theSecondBlade.HasSeekingEdge)
        {
            attackCommand2 = attackCommand1
                .TargetingAllOpponents(theSecondBlade.CombatState)
                .BeforeDamage(() =>
                {
                    IReadOnlyList<Creature> hittableEnemies = theSecondBlade.CombatState.HittableEnemies;
                    if (hittableEnemies.Count <= 0)
                        return Task.CompletedTask;

                    NSovereignBladeVfx? vfxNode = SovereignBlade.GetVfxNode(theSecondBlade.Owner, theSecondBlade);
                    NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(hittableEnemies[0]);
                    if (vfxNode != null && creatureNode != null)
                        vfxNode.Attack(creatureNode.VfxSpawnPosition);

                    return Task.CompletedTask;
                })
                .WithHitFx("vfx/vfx_giant_horizontal_slash", tmpSfx: "slash_attack.mp3");
        }
        else
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
            attackCommand2 = attackCommand1
                .Targeting(cardPlay.Target)
                .BeforeDamage(() =>
                {
                    NSovereignBladeVfx? vfxNode = SovereignBlade.GetVfxNode(theSecondBlade.Owner, theSecondBlade);
                    NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
                    if (vfxNode != null && creatureNode != null)
                        vfxNode.Attack(creatureNode.VfxSpawnPosition);

                    return Task.CompletedTask;
                })
                .WithHitVfxNode(NBigSlashVfx.Create)
                .WithHitVfxNode(NBigSlashImpactVfx.Create);
        }

        await attackCommand2.Execute(choiceContext);

        if (GetOwnerParryAmount(theSecondBlade) > 0M)
        {
            await CreatureCmd.GainBlock(theSecondBlade.Owner.Creature,
                theSecondBlade.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target),
                theSecondBlade.DynamicVars.CalculatedBlock.Props, cardPlay);
        }
    }
    
    public override Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy)
    {
        if (card != this)
            return Task.CompletedTask;

        if (oldPileType == PileType.None || oldPileType == PileType.Exhaust)
            ForgeCmd.PlayCombatRoomForgeVfx(this.Owner, this);

        return Task.CompletedTask;
    }
    
    public void AddDamage(decimal amount)
    {
        DamageVar damage = this.DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + amount;
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}