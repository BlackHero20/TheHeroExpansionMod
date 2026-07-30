using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Powers;

public sealed class YokaiPower : TheHeroExpansionPower
{

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _isReplaying = false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal)
    {
        YokaiPower power = this;
        if (card.Owner.Creature != power.Owner) return;
        if (card.Type == CardType.Curse || card.Type == CardType.Status || card.Type == CardType.Quest) return;
        if (!card.Keywords.Contains(CardKeyword.Ethereal)) return;
        if (_isReplaying) return;

        power.Flash();
        _isReplaying = true;

        try
        {
            int stacks = (int)power.Amount;
            if (card.Type == CardType.Power)
            {
                for (int i = 0; i < stacks; i++)
                {
                    CardModel clone = card.CreateClone();
                    await CardCmd.AutoPlay(choiceContext, clone, null);
                }
            }
            else
            {
                for (int i = 0; i < stacks; i++)
                {
                    await CardCmd.AutoPlay(choiceContext, card, null);
                }

                if (card.Pile?.Type != PileType.Exhaust)
                    await CardPileCmd.Add(card, PileType.Exhaust, CardPilePosition.Random, null, skipVisuals: true);
            }
        }
        finally
        {
            _isReplaying = false;
        }
    }
}