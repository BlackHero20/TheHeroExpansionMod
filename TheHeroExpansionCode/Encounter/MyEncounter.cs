/*using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models.Acts;
using TheHeroExpansion.TheHeroExpansionCode.Monsters;

namespace TheHeroExpansion.TheHeroExpansionCode.Encounters;

public sealed class MyEncounter : CustomEncounterModel
{
    public MyEncounter() : base(RoomType.Monster, autoAdd: true) { }
    
    public override RoomType RoomType => RoomType.Monster;

    public override bool IsValidForAct(ActModel act) => act is Glory;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        new List<MonsterModel> { ModelDb.Monster<Hexaghost>() };

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return new List<(MonsterModel, string?)>
        {
            (ModelDb.Monster<Hexaghost>().ToMutable(), null)
        };
    }
}*/