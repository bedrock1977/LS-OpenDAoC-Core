# Expansion classes (Heretic, Warlock, Vampiir, Mauler)

Catacombs / Shrouded Isles / TOA client target.

## Code changes (Tier 1)

- `GameServer/GlobalConstants.cs` — enabled `Heretic`, `Warlock`, `Vampiir`, `MaulerAlb`, `MaulerMid`, `MaulerHib` in `STARTING_CLASSES_DICT`; enabled `Frostalf`, `Shar`, `HalfOgre`, and all three Minotaur races.
- `GameServer/playerclasses/albion/ClassHeretic.cs` — eligible races: Briton, Avalonian, Inconnu.
- `GameServer/playerclasses/midgard/ClassWarlock.cs` — eligible races: Frostalf, Kobold, Norseman.
- `GameServer/playerclasses/hibernia/ClassVampiir.cs` — eligible races: Celt, Lurikeen, Shar.
- `GameServer/playerclasses/*/ClassMauler*.cs` — eligible races: Minotaur + Briton/Inconnu (Alb), Minotaur + Norseman/Kobold (Mid), Minotaur + Celt/Lurikeen (Hib).
- `GameServer/spells/Warlock/ChamberSpellHandler.cs` — null-safe chamber release via `StartSpell`.

## Database (Tier 2)

Import patches from [bedrock1977/OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) after the base dump:

- `expansion-classes-import.sql` — Heretic, Warlock, Vampiir spells/styles
- `toa-mauler-import.sql` — Mauler spells/styles
- `expansion-startup-locations.sql` — SI class spawns
- `toa-startup-locations.sql` — Mauler and Half Ogre spawns

Base dump already includes class specs, spell lines, and starter gear for classes 33/58/59/60/61/62.

## Runtime server properties (Tier 1)

Ensure these `ServerProperty` values are set after first boot:

- `disabled_expansions` — empty
- `disabled_races` — empty (must not include Frostalf, Shar, HalfOgre, or Minotaur races)
- `start_as_base_class` — **False** (otherwise advanced classes are saved as base classes on creation)

## Docker

Rebuild from this fork or mount a local build to pick up expansion-class code changes. The fork `docker-compose.yml` uses `ghcr.io/bedrock1977/ls-opendaoc-core:latest`.

## Verification checklist

1. Character creation shows Heretic / Warlock / Vampiir / Mauler with correct races.
2. Trainer UI lists spec lines and spells advance on level.
3. Heretic: DoT, pierce magic, monster rez.
4. Vampiir: STR-based power pool, power gain on hit.
5. Warlock: chamber charge, primer, curse release.
6. Mauler: fist wraps, aura manipulation, magnetism specs; Minotaur models load on char screen.
