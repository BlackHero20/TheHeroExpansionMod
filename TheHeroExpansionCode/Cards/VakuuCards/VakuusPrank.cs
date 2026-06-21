using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class VakuusPrank() : VakuuCard(0,
    CardType.Power, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<VakuusPrankPower>(10M),
    ];
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        VakuusPrank vakuusPrank = this;
        if (vakuusPrank.Owner.Creature.Side != side)
            return;
        if (vakuusPrank.Pile?.Type != PileType.Hand)
            return;
        TalkCmd.Play(this.GetRandomVakuuTaunt(), vakuusPrank.Owner.Creature, VfxColor.Purple);
        await Cmd.Wait(0.7f);
        await CardCmd.AutoPlay(choiceContext, vakuusPrank, null, AutoPlayType.Default);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VakuusPrank vakuusPrank = this;
        await PowerCmd.Apply<VakuusPrankPower>(choiceContext, vakuusPrank.Owner.Creature, vakuusPrank.DynamicVars["VakuusPrankPower"].BaseValue, vakuusPrank.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars["VakuusPrankPower"].UpgradeValueBy(10M);
    }
}