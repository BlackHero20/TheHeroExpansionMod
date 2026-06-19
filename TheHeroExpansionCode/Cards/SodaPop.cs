using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class SodaPop() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new EnergyVar(2),
        new CardsVar(1),
        new HealVar(6M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.ForEnergy(this),
            HoverTipFactory.FromCard<Soul>()
        ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SodaPop sodaPop = this;
        await CreatureCmd.TriggerAnim(sodaPop.Owner.Creature, "Cast", sodaPop.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(sodaPop.DynamicVars.Energy.BaseValue, sodaPop.Owner);
        List<Soul> souls = Soul.Create(sodaPop.Owner, 1, sodaPop.CombatState).ToList<Soul>();
        await CardPileCmd.AddGeneratedCardToCombat((CardModel) souls[0], PileType.Hand, Owner);
        if (Osty.CheckMissingWithAnim(sodaPop.Owner))
            return;
        await CreatureCmd.Heal(sodaPop.Owner.Osty, sodaPop.DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}