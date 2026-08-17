# OpenDAoC restore strategy

**Status:** Discussion — adopted as the shard approach.

## Goal

Restore **Shrouded Isles + Catacombs + TOA** expansion content on a private OpenDAoC shard, using DOLSharp as **data and behavior reference**, not as a runtime.

## Why OpenDAoC (not stock DOLSharp)

OpenDAoC is a DOLSharp fork refactored for **ECS performance**. Expansion class code was **scoped to a 1.65 baseline**, not deleted.

| Area | DOLSharp | OpenDAoC |
|------|----------|----------|
| Architecture | OOP + events | ECS components, services, game loop |
| Default patch | Flexible | 1.65 Classic Module |
| Class definitions | Mixed | `GameServer/playerclasses/` |
| Performance | Baseline | Target: many players per zone |

## Three tiers of work

| Tier | Work | Delivers |
|------|------|----------|
| **1 — Config unlock** | `STARTING_CLASSES_DICT`, race stats, `EligibleRaces`, server properties | Classes selectable at creation |
| **2 — Database import** | Spells, specs, styles, startup locations | Progression and spells functional |
| **3 — ECS code fixes** | Handlers, effects, packet edge cases | Behavioral parity |

## What was disabled (four layers)

1. **`GlobalConstants.cs`** — Heretic, Warlock, Vampiir, Maulers commented out of `STARTING_CLASSES_DICT`; Frostalf, Shar, Half Ogre, Minotaur race stats commented
2. **Class files** — empty/commented `EligibleRaces`
3. **Server properties** — `disabled_expansions`, `disabled_races`, `start_as_base_class`
4. **Database** — expansion spell/style rows missing from 1.65 base dump; startup location rows missing

## Implementation order (original plan)

Heretic → Vampiir → Warlock → load-test → TOA Maulers.

See [`../complete/`](../complete/) for what has been finished.

## Porting rules

**Ports easily:** spell handler classes, DB rows, trainer/NPC scripts.

**Needs ECS review:** combat ticks, buffs/debuffs, incomplete handlers (Warlock chambers were the hardest SI/Catacombs item).

**Do not copy live modern DAoC values** — use validated Catacombs-era sources (e.g. db-public release 85).
