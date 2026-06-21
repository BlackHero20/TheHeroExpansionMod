using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Enchantments;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class Eerie() : TheHeroExpansionCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Jumpscare>();

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(7M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Eerie eerie = this;
        await CreatureCmd.GainBlock(eerie.Owner.Creature, eerie.DynamicVars.Block, null);
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(eerie.SelectionScreenPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, eerie.Owner, prefs,
            card => card.Enchantment == null && (card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power),
            (AbstractModel)eerie)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Jumpscare>(card, 1M);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Block.UpgradeValueBy(3M);
    }
}