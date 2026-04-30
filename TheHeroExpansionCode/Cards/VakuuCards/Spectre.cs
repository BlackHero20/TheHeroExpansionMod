using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheHeroExpansion.TheHeroExpansionCode.Cards;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards.VakuuCards;
[Pool(typeof(TokenCardPool))]
public class Spectre() : VakuuCard(0,
    CardType.Skill, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<IntangiblePower>(1M),
        new PowerVar<WraithFormPower>(1M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Spectre spectre = this;
        await CreatureCmd.TriggerAnim(spectre.Owner.Creature, "Cast", spectre.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<IntangiblePower>(choiceContext, spectre.Owner.Creature, spectre.DynamicVars["IntangiblePower"].BaseValue, spectre.Owner.Creature, (CardModel) spectre);
        if (spectre.IsUpgraded)
        {
            await PowerCmd.Apply<WraithFormPower>(choiceContext, spectre.Owner.Creature, spectre.DynamicVars["WraithFormPower"].BaseValue, spectre.Owner.Creature, (CardModel) spectre);
        }
    }
    
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        Spectre spectre = this;
        if (spectre.IsUpgraded)
        {
            if (spectre.Owner.Creature.Side != side)
                return;
            if (spectre.Pile?.Type != PileType.Hand)
                return;
            TalkCmd.Play(new LocString("cards", "THEHEROEXPANSION-SPECTRE.approval"), spectre.Owner.Creature, VfxColor.Purple);
            await CardCmd.AutoPlay(choiceContext, spectre, null, AutoPlayType.Default);
        }
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}