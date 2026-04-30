using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class DemonicSurge() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<DemonicSurgePower>(1M),
        new PowerVar<DemonicCripplePower>(2M)
        
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DemonicSurge demonicSurge = this;
        if (!demonicSurge.IsUpgraded)
        {
            await PowerCmd.Apply<DemonicSurgePower>(choiceContext, demonicSurge.Owner.Creature, demonicSurge.DynamicVars["DemonicSurgePower"].BaseValue, demonicSurge.Owner.Creature, (CardModel) demonicSurge);
        }
        else
        {
            await PowerCmd.Apply<DemonicCripplePower>(choiceContext, demonicSurge.Owner.Creature, demonicSurge.DynamicVars["DemonicCripplePower"].BaseValue, demonicSurge.Owner.Creature, (CardModel) demonicSurge);
        }
    }
    
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        DemonicSurge demonicSurge = this;
        if (demonicSurge.IsUpgraded)
        {
            if (demonicSurge.Owner.Creature.Side != side)
                return;
            if (demonicSurge.Pile?.Type != PileType.Hand)
                return;
            TalkCmd.Play(new LocString("cards", "THEHEROEXPANSION-DEMONIC_SURGE.approval"), demonicSurge.Owner.Creature, VfxColor.Purple);
            await CardCmd.AutoPlay(choiceContext, demonicSurge, null, AutoPlayType.Default);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}