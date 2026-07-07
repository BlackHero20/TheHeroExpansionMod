using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(DefectCardPool))]
public class LightBuster() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private readonly List<ModelId> _evokedOrbIds = new();

    public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Move),
        new StringVar("OrbEffects")
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Evoke)
    ];


    private void UpdateOrbEffectsVar()
    {
        if (!IsMutable) return;

        var orbNames = new Dictionary<ModelId, string>
        {
            { ModelDb.GetId<LightningOrb>(), "[gold]Lightning[/gold]" },
            { ModelDb.GetId<DarkOrb>(), "[gold]Dark[/gold]" },
            { ModelDb.GetId<GlassOrb>(), "[gold]Glass[/gold]" },
            { ModelDb.GetId<FrostOrb>(), "[gold]Frost[/gold]" },
            { ModelDb.GetId<PlasmaOrb>(), "[gold]Plasma[/gold]" },
        };

        if (_evokedOrbs.Count == 0)
        {
            ((StringVar)DynamicVars["OrbEffects"]).StringValue = "";
            return;
        }

        var parts = _evokedOrbs
            .GroupBy(o => o.OrbId)
            .Select(g => $"{(int)g.Sum(o => o.EvokeValue)} {orbNames.GetValueOrDefault(g.Key, "?")}");

        ((StringVar)DynamicVars["OrbEffects"]).StringValue = "\n(" + string.Join(", ", parts) + ")";
    }
    
    private List<(ModelId OrbId, decimal EvokeValue)> _evokedOrbs = new();

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _evokedOrbs = new List<(ModelId, decimal)>();
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _evokedOrbs.Clear();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        LightBuster lightBuster = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(lightBuster.DynamicVars.Damage.BaseValue)
            .FromCard(lightBuster, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        foreach (var (orbId, evokeValue) in lightBuster._evokedOrbs)
        {
            await ReplayEvoke(choiceContext, orbId, evokeValue);
            await Cmd.Wait(0.1f);
        }
        
        var orbQueue = lightBuster.Owner.PlayerCombatState.OrbQueue;
        if (orbQueue.Orbs.Count > 0)
        {
            var rightmost = orbQueue.Orbs.First();
            lightBuster._evokedOrbs.Add((rightmost.Id, rightmost.EvokeVal));
            lightBuster.UpdateOrbEffectsVar();
            await OrbCmd.EvokeNext(choiceContext, lightBuster.Owner);
        }
    }

    private async Task ReplayEvoke(PlayerChoiceContext ctx, ModelId orbId, decimal value)
    {
        if (orbId == ModelDb.GetId<LightningOrb>())
        {
            var target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies.ToList());
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_lightning");
            await CreatureCmd.Damage(ctx, target, value, ValueProp.Unpowered, Owner.Creature);
        }
        else if (orbId == ModelDb.GetId<DarkOrb>())
        {
            var target = CombatState.HittableEnemies.MinBy(c => c.CurrentHp);
            if (target != null)
                await CreatureCmd.Damage(ctx, target, value, ValueProp.Unpowered, Owner.Creature);
        }
        else if (orbId == ModelDb.GetId<GlassOrb>())
        {
            await CreatureCmd.Damage(ctx, CombatState.HittableEnemies, value, ValueProp.Unpowered, Owner.Creature);
        }
        else if (orbId == ModelDb.GetId<FrostOrb>())
        {
            await CreatureCmd.GainBlock(Owner.Creature, value, ValueProp.Unpowered, null, false);
        }
        else if (orbId == ModelDb.GetId<PlasmaOrb>())
        {
            await PlayerCmd.GainEnergy(value, Owner);
            
        }
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(4M);
}