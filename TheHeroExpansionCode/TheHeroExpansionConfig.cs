using BaseLib.Config;
using BaseLib.Utils;

namespace TheHeroExpansion.TheHeroExpansionCode;

[ConfigHoverTipsByDefault()]
internal class TheHeroExpansionConfig : SimpleModConfig
{
    // ── Ironclad ──────────────────────────────────────────
    [ConfigSection("Ironclad")]
    
    public static bool TryAgain { get; set; } = true;
    public static bool Suffocate { get; set; } = true;
    public static bool Transfusion { get; set; } = true;
    public static bool SixHundredStrike { get; set; } = true;
    public static bool FromTheGrave { get; set; } = true;
    public static bool Akumetsu { get; set; } = true;
    public static bool BeastWithin { get; set; } = true;
    public static bool BetterDays { get; set; } = true;
    public static bool AllEndsInFlames { get; set; } = true;
    
    // ── Silent ────────────────────────────────────────────
    [ConfigSection("Silent")]
    
    public static bool ButterflyKnife { get; set; } = true;
    public static bool Repurpose { get; set; } = true;
    public static bool Groundwork { get; set; } = true;
    public static bool BehindYou { get; set; } = true;
    public static bool VolcanicViper { get; set; } = true;
    public static bool Cheat { get; set; } = true;
    public static bool CeaseToExist { get; set; } = true;
    public static bool VenomFlurry { get; set; } = true;
    public static bool Fma { get; set; } = true;
    public static bool MasterfulStab { get; set; } = true;

    // ── Regent ────────────────────────────────────────────
    [ConfigSection("Regent")]
    
    public static bool Work { get; set; } = true;
    public static bool StrikeTheEarth { get; set; } = true;
    public static bool BarrelRoll { get; set; } = true;
    public static bool Serve { get; set; } = true;
    public static bool JoyOfCreation { get; set; } = true;
    public static bool InstantTransmission { get; set; } = true;
    public static bool TheWorld { get; set; } = true;
    public static bool EdgeOfDestiny { get; set; } = true;
    public static bool Fire { get; set; } = true;

    // ── Necrobinder ───────────────────────────────────────
    [ConfigSection("Necrobinder")]
    
    public static bool Swipe { get; set; } = true;
    public static bool Eerie { get; set; } = true;
    public static bool AllOutAttack { get; set; } = true;
    public static bool SodaPop { get; set; } = true;
    public static bool RigorMortis { get; set; } = true;
    public static bool SixthFinger { get; set; } = true;
    public static bool PerishSong { get; set; } = true;
    public static bool GotYourBack { get; set; } = true;
    public static bool Yokai { get; set; } = true;

    // ── Defect ────────────────────────────────────────────
    [ConfigSection("Defect")]
    
    public static bool CrackedMarble { get; set; } = true;
    public static bool ShadowBoxing { get; set; } = true;
    public static bool LootBox { get; set; } = true;
    public static bool BitFlip { get; set; } = true;
    public static bool Nanomachines { get; set; } = true;
    public static bool DarkMagician { get; set; } = true;
    public static bool KillDrive { get; set; } = true;
    public static bool LightBuster { get; set; } = true;
    public static bool SelfDestruct { get; set; } = true;

    // ── Colorless ─────────────────────────────────────────
    [ConfigSection("Colorless")]
    
    public static bool KaraCancel { get; set; } = true;
    public static bool Setup { get; set; } = true;
    public static bool JustLucky { get; set; } = true;
    public static bool PatriotShield { get; set; } = true;
    public static bool CrystalFocus { get; set; } = true;
    public static bool MagicPen { get; set; } = true;
    public static bool ReinforcedBody { get; set; } = true;
    public static bool KickOfTheFinalHour { get; set; } = true;
    public static bool SuperForm { get; set; } = true;

    // ── Relics ────────────────────────────────────────────
    [ConfigSection("Relics")]
    
    public static bool SmilingMask { get; set; } = true;
    public static bool ThreeStringUkulele { get; set; } = true;
    public static bool PottetAloeVera { get; set; } = true;
    public static bool MysteriousFlashlight { get; set; } = true;
    public static bool BladeHandle { get; set; } = true;
    public static bool ToastySmores { get; set; } = true;
    public static bool PaelsBlight { get; set; } = true;
    public static bool TomeOfOrigin { get; set; } = true;
    public static bool SteelBall { get; set; } = true;
    public static bool DaintyDandelion { get; set; } = true;
    public static bool TheRing { get; set; } = true;

    // ── Potions ───────────────────────────────────────────
    [ConfigSection("Potions")]
    
    public static bool VigorPotion { get; set; } = true;
    public static bool TimeInABottle { get; set; } = true;
    public static bool BleedingFuel { get; set; } = true;
}
