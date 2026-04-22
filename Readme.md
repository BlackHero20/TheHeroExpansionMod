# TheHeroExpansion

A content expansion mod for **Slay the Spire 2** by **BlackHero20**, built using [BaseLib](https://github.com/alchyr/sts2-baselib). Adds a new set of cards, relics, and powers to the game.

> **Version:** v0.0.1  
> **Mod ID:** `TheHeroExpansion`

---

## Requirements

- Slay the Spire 2 (early access)
- [BaseLib](https://github.com/alchyr/sts2-baselib) — required dependency
- .NET 9 SDK (for building from source)
- Godot 4.5 (for building from source)

---

## Installation

1. Install **BaseLib** if you haven't already — TheHeroExpansion depends on it.
2. Download the latest release (`.pck` + `.dll`) from the [Releases](#) page.
3. Place both files into your Slay the Spire 2 mods folder.
4. Launch the game and enable the mod from the mod loader menu.

---

## Content

### Cards (22)

| Card | Description |
|------|-------------|
| **Try Again** | Gain Block. Put an Attack from your Hand on top of your Draw Pile and Enchant it with Sharp. |
| **Six Hundred Strike** | Deal damage. Hits an extra time for each card containing "Strike" played this combat. |
| **Beast Within** | Whenever you deal single-target damage to a Vulnerable enemy, deal that much damage to ALL other enemies. |
| **Butterfly Knife** | Draw cards. Add Shivs (or Shivs+) into your Hand. |
| **Behind You** | Apply Weak and Vulnerable. |
| **Cease To Exist** | Gain Intangible. For the next X turns, you cannot use Attacks. |
| **WORK!** | Add Minion Labor(s) into your Hand. |
| **SERVE!!** | At the start of your turn, add a Minion Duty into your Hand. |
| **The World** | Take an extra turn after this one. |
| **Minion Labor** | Gain Stars. Forge. |
| **Minion Duty** | Deal damage. Gain Block. |
| **Swipe** | Osty deals damage to ALL enemies. Summon for each enemy hit. |
| **Soda Pop** | Gain Energy. Add a Soul into your Hand. Osty heals HP. |
| **Perish Song** | After X turns, apply Doom to EVERYONE. |
| **Split Shard** | Deal damage. Channel 1 Glass. |
| **Bit Flip** | Evoke all Orbs. Channel 1 Lightning for each Frost. Channel 1 Frost for each Lightning. |
| **Kill Drive** | Whenever you play a 0-cost Attack, Channel 1 Lightning. |
| **Exhume** | Put a card from your Exhaust Pile into your Hand. |
| **Concentrate** | Gain Energy. Discard cards. |
| **Remember** | Add Retain to every card in Hand (upgraded: also Upgrade them). |
| **Resent** | Choose cards from your Discard Pile and play them for free. |
| **Seek** | Put cards from your Draw Pile into your Hand. |

### Relics (4)

| Relic | Description |
|-------|-------------|
| **Smiling Mask** | The Merchant's card removal service always costs a fixed amount of Gold. |
| **Three String Ukulele** | The first time you play a Power each combat, gain Energy. |
| **Potted Aloe Vera** | Start each Elite combat with Regen. |
| **Mysterious Flashlight** | Upon pickup, obtain a card from your past. |

### Powers (6)

| Power | Description |
|-------|-------------|
| **Beast Within** | Whenever you deal single-target damage to a Vulnerable enemy, deal that much damage to ALL other enemies X times. |
| **Cease To Exist** | Cannot use Attacks for the next X turns. |
| **Serve** | At the start of your turn, add a Minion Duty into your Hand. |
| **The World** | Take an extra X turns after this one. |
| **Perish Song** | At the end of X turns, apply 100 Doom to everyone. |
| **Kill Drive** | Whenever you play a 0-cost Attack, Channel X Lightning. |

---

## Building from Source

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd TheHeroExpansion
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

   The output `.dll` and `.pck` will be located in `.godot/mono/temp/bin/Debug/`.

---

## Project Structure

```
TheHeroExpansion/
├── TheHeroExpansionCode/
│   ├── Cards/          # Card implementations
│   ├── Powers/         # Power implementations
│   ├── Relics/         # Relic implementations
│   ├── Extensions/     # Utility extensions
│   └── MainFile.cs     # Mod entry point
├── TheHeroExpansion/
│   ├── images/
│   │   ├── card_portraits/
│   │   ├── powers/
│   │   └── relics/
│   └── localization/
│       └── eng/        # English text for cards, relics, powers
├── TheHeroExpansion.json   # Mod metadata
└── TheHeroExpansion.csproj
```

---

## Credits

- **Author:** BlackHero20
- **Modding framework:** [BaseLib](https://github.com/alchyr/sts2-baselib) by alchyr
- Built with Godot 4.5, C# / .NET 9, and HarmonyLib

---

## License

This project does not currently include a license. All rights reserved by the author unless otherwise stated.
