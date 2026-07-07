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
public class UnbreakableStrength() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<UnbreakableStrengthPower>(1M),
        new PowerVar<FragileStrengthPower>(2M)
        
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        UnbreakableStrength unbreakableStrength = this;
        if (!unbreakableStrength.IsUpgraded)
        {
            await PowerCmd.Apply<UnbreakableStrengthPower>(choiceContext, unbreakableStrength.Owner.Creature, unbreakableStrength.DynamicVars["UnbreakableStrengthPower"].BaseValue, unbreakableStrength.Owner.Creature, (CardModel) unbreakableStrength);
        }
        else
        {
            await PowerCmd.Apply<FragileStrengthPower>(choiceContext, unbreakableStrength.Owner.Creature, unbreakableStrength.DynamicVars["FragileStrengthPower"].BaseValue, unbreakableStrength.Owner.Creature, (CardModel) unbreakableStrength);
        }
    }
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        UnbreakableStrength unbreakableStrength = this;
        if (unbreakableStrength.IsUpgraded)
        {
            if (unbreakableStrength.Owner.Creature.Side != side)
                return;
            if (unbreakableStrength.Pile?.Type != PileType.Hand)
                return;
            TalkCmd.Play(this.GetRandomVakuuTaunt(), unbreakableStrength.Owner.Creature, VfxColor.Purple);
            await Cmd.Wait(0.7f);
            await CardCmd.AutoPlay(choiceContext, unbreakableStrength, null, AutoPlayType.Default);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}