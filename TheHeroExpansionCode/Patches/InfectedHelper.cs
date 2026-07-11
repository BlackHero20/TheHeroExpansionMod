using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;

namespace TheHeroExpansion.TheHeroExpansionCode.Enchantments;

public static class InfectedHelper
{
    public static readonly Dictionary<CardModel, (CardModel? left, CardModel? right)> CardNeighborCache = new();
}