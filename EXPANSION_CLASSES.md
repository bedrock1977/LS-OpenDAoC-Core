# Expansion classes (Heretic, Warlock, Vampiir)

Catacombs / Shrouded Isles client target. Maulers and Minotaurs remain disabled.

## Code changes (Tier 1)

- `GameServer/GlobalConstants.cs` — enabled `Heretic`, `Warlock`, `Vampiir` in `STARTING_CLASSES_DICT`; enabled `Frostalf` and `Shar` race stats.
- `GameServer/playerclasses/albion/ClassHeretic.cs` — eligible races: Briton, Avalonian, Inconnu.
- `GameServer/playerclasses/midgard/ClassWarlock.cs` — eligible races: Frostalf, Kobold, Norseman.
- `GameServer/playerclasses/hibernia/ClassVampiir.cs` — eligible races: Celt, Lurikeen, Shar.
- `GameServer/spells/Warlock/ChamberSpellHandler.cs` — null-safe chamber release via `StartSpell`.

## Database (Tier 2)

Import `opendaoc-db-core/patches/expansion-classes-import.sql` from [bedrock1977/OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) after the base dump.

Also apply `expansion-startup-locations.sql` so new Heretic/Warlock/Vampiir characters spawn in capital cities instead of region 0.

Base dump already includes class specs (`classxspecialization.sql`), spell lines (`spellline.sql`), and starter gear entries for classes 33/58/59.

## Runtime server properties (Tier 1)

Ensure these `ServerProperty` values are set after first boot:

- `disabled_expansions` — empty
- `disabled_races` — empty (must not include Frostalf or Shar)
- `start_as_base_class` — **False** (otherwise Warlock/Heretic/Vampiir are saved as base classes on creation)

## Docker

The stock `docker-compose.yml` uses `ghcr.io/opendaoc/opendaoc-core:latest`. Rebuild from this fork or mount a local build to pick up expansion-class code changes.

## Verification checklist

1. Character creation shows Heretic / Warlock / Vampiir with correct races.
2. Trainer UI lists spec lines and spells advance on level.
3. Heretic: DoT, pierce magic, monster rez.
4. Vampiir: STR-based power pool, power gain on hit.
5. Warlock: chamber charge, primer, curse release.
