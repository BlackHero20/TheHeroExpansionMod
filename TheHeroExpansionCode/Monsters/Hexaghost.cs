/*using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Helpers;
using TheHeroExpansion.TheHeroExpansionCode.Patches;

namespace TheHeroExpansion.TheHeroExpansionCode.Monsters;

public sealed class Hexaghost : CustomMonsterModel
{
    public override int MinInitialHp =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 264, 250);
    public override int MaxInitialHp =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 264, 250);

    public override IEnumerable<string> AssetPaths =>
        new[] { "res://TheHeroExpansion/images/monsters/hexaghost.png" };

    // --- Damage values ---
    private const int SearDamage = 6;
    private int InfernoDamage =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private int FireTackleDamage =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int FireTackleCount = 2;
    private const int InfernoHits = 6;
    private const int StrengthenBlock = 12;
    private int StrengthAmount =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private int SearBurnCount =>
        AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

    // --- State ---
    private bool _burnUpgraded;
    private int _orbActiveCount;
    private int _dividerDamage;

    // --- Move IDs ---
    private const string ACTIVATE = "ACTIVATE";
    private const string DIVIDER = "DIVIDER";
    private const string TACKLE = "TACKLE";
    private const string INFLAME = "INFLAME";
    private const string SEAR = "SEAR";
    private const string INFERNO = "INFERNO";
    private const string MOVE_BRANCH = "MOVE_BRANCH";


    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var root = new NCreatureVisuals();

        var visuals = new Node2D { Name = "Visuals", UniqueNameInOwner = true };

        var texture = GD.Load<Texture2D>(
            "res://TheHeroExpansion/images/monsters/hexaghost.png");
        if (texture != null)
        {
            // For animations in the future: swap Sprite2D for AnimatedSprite2D,
            // load frames into a SpriteFrames resource, and call sprite.Play("idle")
            var sprite = new Sprite2D
            {
                Texture = texture,
                Scale = new Vector2(0.6f, 0.6f),
                Position = new Vector2(100, -250)
            };
            visuals.AddChild(sprite);
        }

        root.AddChild(visuals);
        root.AddChild(new Control
        {
            Name = "Bounds",
            UniqueNameInOwner = true,
            Size = new Vector2(400, 400),
            Position = new Vector2(-100, -450)
        });
        root.AddChild(new Marker2D
        {
            Name = "IntentPos",
            UniqueNameInOwner = true,
            Position = new Vector2(100, -500)
        });
        root.AddChild(new Marker2D
        {
            Name = "CenterPos",
            UniqueNameInOwner = true,
            Position = new Vector2(50, -125)
        });

        return root;
    }

    // --- Lifecycle ---
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        _burnUpgraded = false;
        _orbActiveCount = 0;
    }

    // --- Move State Machine ---
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var states = new List<MonsterState>();

        var activateState = new MoveState(ACTIVATE, Activate,
            new AbstractIntent[] { new UnknownIntent() });

        var dividerState = new MoveState(DIVIDER, Divider,
            new AbstractIntent[] { new DynamicMultiAttackIntent(() => _dividerDamage, 6) });

        var tackleState = new MoveState(TACKLE, Tackle,
            new AbstractIntent[] { new MultiAttackIntent(FireTackleDamage, FireTackleCount) });

        var inflameState = new MoveState(INFLAME, Inflame,
            new AbstractIntent[] { new DefendIntent(), new BuffIntent() });

        var searState = new MoveState(SEAR, Sear,
            new AbstractIntent[] { new SingleAttackIntent(SearDamage), new StatusIntent(SearBurnCount) });

        var infernoState = new MoveState(INFERNO, Inferno,
            new AbstractIntent[] { new MultiAttackIntent(InfernoDamage, InfernoHits), new DebuffIntent() });

        var moveBranch = new ConditionalBranchState(MOVE_BRANCH);
        moveBranch.AddState(infernoState, () => _orbActiveCount == 6);
        moveBranch.AddState(inflameState, () => _orbActiveCount == 3);
        moveBranch.AddState(tackleState, () => _orbActiveCount == 1 || _orbActiveCount == 4);
        moveBranch.AddState(searState, () => true);
        
        activateState.FollowUpState = dividerState;
        dividerState.FollowUpState = moveBranch;
        tackleState.FollowUpState = moveBranch;
        inflameState.FollowUpState = moveBranch;
        searState.FollowUpState = moveBranch;
        infernoState.FollowUpState = moveBranch;

        states.Add(activateState);
        states.Add(dividerState);
        states.Add(tackleState);
        states.Add(inflameState);
        states.Add(searState);
        states.Add(infernoState);
        states.Add(moveBranch);

        return new MonsterMoveStateMachine(states, activateState);
    }

    protected override bool ShouldShowMoveInBestiary(string moveStateId) =>
        moveStateId != MOVE_BRANCH;

    // --- Moves ---
    private async Task Activate(IReadOnlyList<Creature> targets)
    {
        var living = targets.Where(t => t.IsAlive).ToList();
        var avgHp = living.Count > 0 ? living.Average(t => t.CurrentHp) : 1;
        _dividerDamage = (int)(avgHp / 12) + 1;
    }

    private async Task Divider(IReadOnlyList<Creature> targets)
    {
        for (int i = 0; i < 6; i++)
        {
            await DamageCmd.Attack(_dividerDamage)
                .FromMonster(this)
                .WithAttackerFx(sfx: AttackSfx)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(null);
            await Cmd.Wait(0.05f);
        }
        _orbActiveCount = 0;
    }

    private async Task Sear(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SearDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 1f)
            .WithAttackerFx(sfx: AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await AddBurnsToDiscard(targets, SearBurnCount);
        _orbActiveCount++;
    }

    private async Task Tackle(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(FireTackleDamage)
            .WithHitCount(FireTackleCount)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 1f)
            .WithAttackerFx(sfx: AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        _orbActiveCount++;
    }

    private async Task Inflame(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, StrengthenBlock, ValueProp.Move, null);
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(), Creature, StrengthAmount, Creature, null);
        _orbActiveCount++;
    }

    private async Task Inferno(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(InfernoDamage)
            .WithHitCount(InfernoHits)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 1f)
            .WithAttackerFx(sfx: AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await UpgradeAllBurnsAndAddMore(targets);
        _burnUpgraded = true;
        _orbActiveCount = 0;
    }

    // --- Burn helpers ---
    private async Task AddBurnsToDiscard(IReadOnlyList<Creature> targets, int count)
    {
        if (!_burnUpgraded)
        {
            await CardPileCmd.AddToCombatAndPreview<Burn>(
                targets, PileType.Discard, count, null);
            return;
        }

        BurnUpgradePatch.AllowBurnUpgrade = true;
        try
        {
            foreach (var playerCreature in targets.Where(t => t.Player != null))
            {
                var player = playerCreature.Player;
                var combatState = playerCreature.CombatState;
                var statusCards = new CardPileAddResult[count];

                for (int i = 0; i < count; i++)
                {
                    var burn = combatState.CreateCard<Burn>(player);
                    burn.UpgradeInternal();
                    burn.FinalizeUpgradeInternal();
                    statusCards[i] = await CardPileCmd.AddGeneratedCardToCombat(
                        burn, PileType.Discard, null);
                }

                CardCmd.PreviewCardPileAdd(
                    (IReadOnlyList<CardPileAddResult>)statusCards,
                    style: CardPreviewStyle.HorizontalLayout);
            }
            await Cmd.Wait(1f);
        }
        finally
        {
            BurnUpgradePatch.AllowBurnUpgrade = false;
        }
    }

    private async Task UpgradeAllBurnsAndAddMore(IReadOnlyList<Creature> targets)
    {
        BurnUpgradePatch.AllowBurnUpgrade = true;
        try
        {
            foreach (var playerCreature in targets.Where(t => t.Player != null))
            {
                var player = playerCreature.Player;

                var burnsToUpgrade = player.Piles
                    .Where(p => p.Type is PileType.Draw or PileType.Discard or PileType.Hand)
                    .SelectMany(p => p.Cards)
                    .OfType<Burn>()
                    .Where(b => b.CurrentUpgradeLevel < b.MaxUpgradeLevel)
                    .ToList();

                foreach (var burn in burnsToUpgrade)
                {
                    burn.UpgradeInternal();
                    burn.FinalizeUpgradeInternal();
                }

                var combatState = playerCreature.CombatState;
                var statusCards = new CardPileAddResult[3];

                for (int i = 0; i < 3; i++)
                {
                    var burn = combatState.CreateCard<Burn>(player);
                    burn.UpgradeInternal();
                    burn.FinalizeUpgradeInternal();
                    statusCards[i] = await CardPileCmd.AddGeneratedCardToCombat(
                        burn, PileType.Discard, null);
                }

                CardCmd.PreviewCardPileAdd(
                    (IReadOnlyList<CardPileAddResult>)statusCards,
                    style: CardPreviewStyle.HorizontalLayout);
            }
            await Cmd.Wait(1f);
        }
        finally
        {
            BurnUpgradePatch.AllowBurnUpgrade = false;
        }
    }
}*/
