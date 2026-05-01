using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;

[Pool(typeof(TokenCardPool))]
public class QuickThought() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(12M, ValueProp.Move),
        new PowerVar<TheGambitPower>(1M)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        QuickThought quickThought = this;
        await CreatureCmd.GainBlock(quickThought.Owner.Creature, quickThought.DynamicVars.Block, cardPlay);
        if (quickThought.IsUpgraded)
        {
            await PowerCmd.Apply<TheGambitPower>(choiceContext, quickThought.Owner.Creature, quickThought.DynamicVars["TheGambitPower"].BaseValue, quickThought.Owner.Creature, (CardModel) quickThought);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(23M);
    }
}