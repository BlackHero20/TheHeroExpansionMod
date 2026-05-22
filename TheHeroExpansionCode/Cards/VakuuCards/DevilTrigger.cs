using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Powers.VakuuPowers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class DevilTrigger() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<TheWorldPower>(1M),
        new PowerVar<HubrisPower>(2M)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DevilTrigger devilTrigger = this;
        await PowerCmd.Apply<TheWorldPower>(choiceContext, devilTrigger.Owner.Creature, devilTrigger.DynamicVars["TheWorldPower"].BaseValue, devilTrigger.Owner.Creature, (CardModel) devilTrigger);
        if (devilTrigger.IsUpgraded)
        {
            await PowerCmd.Apply<HubrisPower>(choiceContext, devilTrigger.Owner.Creature, devilTrigger.DynamicVars["HubrisPower"].BaseValue, devilTrigger.Owner.Creature, (CardModel) devilTrigger);
        }
    }
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        DevilTrigger devilTrigger = this;
        if (devilTrigger.IsUpgraded)
        {
            if (devilTrigger.Owner.Creature.Side != side)
                return;
            if (devilTrigger.Pile?.Type != PileType.Hand)
                return;
            TalkCmd.Play(this.GetRandomVakuuTaunt(), devilTrigger.Owner.Creature, VfxColor.Purple);
            await Cmd.Wait(0.7f);
            await CardCmd.AutoPlay(choiceContext, devilTrigger, null, AutoPlayType.Default);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}
