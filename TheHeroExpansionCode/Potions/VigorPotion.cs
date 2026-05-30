using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace TheHeroExpansion.TheHeroExpansionCode.Potions;
[Pool(typeof(SharedPotionPool))]
public sealed class VigorPotion : TheHeroExpansionPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    
    public override TargetType TargetType => TargetType.AnyPlayer;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<VigorPower>(12M)
    ];
    
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("f5a623"));
        await PowerCmd.Apply<VigorPower>(choiceContext, target, 12M, Owner.Creature, null);
    }
}