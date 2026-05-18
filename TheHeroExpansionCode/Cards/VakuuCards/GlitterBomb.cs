using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class GlitterBomb() : VakuuCard(0,
    CardType.Attack, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(10M, ValueProp.Move),
        new DynamicVar("Glam", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Glam>();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        GlitterBomb glitterBomb = this;
        VfxCmd.PlayFullScreenInCombat("vfx/vfx_dramatic_entrance_fullscreen", glitterBomb.Owner.Creature);
        
        await DamageCmd.Attack(glitterBomb.DynamicVars.Damage.BaseValue)
            .FromCard(glitterBomb)
            .TargetingAllOpponents(glitterBomb.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (glitterBomb.IsUpgraded)
        {
            await CreatureCmd.Damage(
                choiceContext,
                glitterBomb.CombatState.Allies.Where(c => c.IsHittable),
                glitterBomb.DynamicVars.Damage.BaseValue,
                ValueProp.Move,
                glitterBomb.Owner.Creature,
                glitterBomb);

            foreach (var card in PileType.Hand.GetPile(glitterBomb.Owner).Cards.ToList())
            {
                if (card.Enchantment == null && (card.Type == CardType.Attack || card.Type == CardType.Skill ||
                                                 card.Type == CardType.Power))
                    CardCmd.Enchant<Glam>(card, glitterBomb.DynamicVars["Glam"].BaseValue);
            }
        }
        else
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(glitterBomb.SelectionScreenPrompt, 1);
            CardModel card = (await CardSelectCmd.FromHand(choiceContext, glitterBomb.Owner, prefs,
                (Func<CardModel, bool>)(card => card.Enchantment == null && (card.Type == CardType.Attack ||
                                                                             card.Type == CardType.Skill ||
                                                                             card.Type == CardType.Power)),
                (AbstractModel)glitterBomb)).FirstOrDefault<CardModel>();
            if (card == null)
                return;
            CardCmd.Enchant<Glam>(card, glitterBomb.DynamicVars["Glam"].BaseValue);
        }
    }
}