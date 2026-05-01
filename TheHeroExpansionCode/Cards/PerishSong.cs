using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Powers;

namespace TheHeroExpansion.TheHeroExpansionCode.Cards;
[Pool(typeof(NecrobinderCardPool))]
public class PerishSong() : TheHeroExpansionCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("Turns", 3M),
        new PowerVar<DoomPower>(90M),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        PerishSong perishSong = this;
        await CreatureCmd.TriggerAnim(perishSong.Owner.Creature, "Cast", perishSong.Owner.Character.CastAnimDelay);
        (await PowerCmd.Apply<PerishSongPower>(choiceContext, perishSong.Owner.Creature, perishSong.DynamicVars["Turns"].BaseValue, perishSong.Owner.Creature, (CardModel)perishSong))
            .SetDoom(perishSong.DynamicVars.Doom.BaseValue);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}