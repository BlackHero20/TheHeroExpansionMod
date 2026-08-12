using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(IroncladCardPool))]
public class Transfusion() : TheHeroExpansionCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies, nameof(TheWorld))
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        new DamageVar(3M, ValueProp.Move),
        new RepeatVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Transfusion transfusion = this;
        bool hitCount = Transfusion.LostHpThisTurn(transfusion.Owner.Creature);
        if (hitCount)
        {
            await CreatureCmd.TriggerAnim(transfusion.Owner.Creature, "Cast", transfusion.Owner.Character.CastAnimDelay);
            await PowerCmd.Apply<StrengthPower>(choiceContext, transfusion.Owner.Creature, transfusion.DynamicVars["StrengthPower"].BaseValue, transfusion.Owner.Creature, (CardModel) transfusion);
        }
        VfxCmd.PlayOnCreatureCenter(transfusion.Owner.Creature, "vfx/vfx_bloody_impact");
        await DamageCmd.Attack(transfusion.DynamicVars.Damage.BaseValue).WithHitCount(transfusion.DynamicVars.Repeat.IntValue).FromCard((CardModel) transfusion, cardPlay).TargetingAllOpponents(transfusion.CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
    }
    
    private static bool LostHpThisTurn(Creature creature)
    {
        return CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Any<DamageReceivedEntry>((Func<DamageReceivedEntry, bool>) (e => e.HappenedThisTurn(creature.CombatState) && e.Receiver == creature && e.Result.UnblockedDamage > 0));
    }
}