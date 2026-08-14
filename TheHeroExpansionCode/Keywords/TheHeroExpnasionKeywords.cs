using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
 
namespace TheHeroExpansion.TheHeroExpansionCode.Keywords;
 

public static class TheHeroExpansionKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Sovereign;
}