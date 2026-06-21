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
public class TimeGun() : VakuuCard(0,
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
        TimeGun timeGun = this;
        await PowerCmd.Apply<TheWorldPower>(choiceContext, timeGun.Owner.Creature, timeGun.DynamicVars["TheWorldPower"].BaseValue, timeGun.Owner.Creature, (CardModel) timeGun);
        if (timeGun.IsUpgraded)
        {
            await PowerCmd.Apply<HubrisPower>(choiceContext, timeGun.Owner.Creature, timeGun.DynamicVars["HubrisPower"].BaseValue, timeGun.Owner.Creature, (CardModel) timeGun);
        }
    }
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        TimeGun timeGun = this;
        if (timeGun.IsUpgraded)
        {
            if (timeGun.Owner.Creature.Side != side)
                return;
            if (timeGun.Pile?.Type != PileType.Hand)
                return;
            TalkCmd.Play(this.GetRandomVakuuTaunt(), timeGun.Owner.Creature, VfxColor.Purple);
            await Cmd.Wait(0.7f);
            await CardCmd.AutoPlay(choiceContext, timeGun, null, AutoPlayType.Default);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}
