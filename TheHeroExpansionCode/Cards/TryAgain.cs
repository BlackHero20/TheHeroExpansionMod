using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class TryAgain() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Sharp>((int)this.DynamicVars["Sharp"].BaseValue);
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9M, ValueProp.Move),
        new DynamicVar("Sharp", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        TryAgain tryAgain = this;
        await CreatureCmd.TriggerAnim(tryAgain.Owner.Creature, "Cast", tryAgain.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(tryAgain.Owner.Creature, tryAgain.DynamicVars.Block, cardPlay);
        CardSelectorPrefs prefs = new CardSelectorPrefs(tryAgain.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, tryAgain.Owner, prefs, (Func<CardModel, bool>) (card => card.Type == CardType.Attack && card.Enchantment == null), (AbstractModel) tryAgain)).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        CardCmd.Enchant<Sharp>(card, tryAgain.DynamicVars["Sharp"].BaseValue);
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(2M);
        this.DynamicVars["Sharp"].UpgradeValueBy(1);
    }
}