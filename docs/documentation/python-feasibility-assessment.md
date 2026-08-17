# Python / language rewrite feasibility

**Status:** Discussion — decision made not to pursue.

## Question

Can DOLSharp be converted to Python (or another language) for the LS shard?

## Answer

Technically yes; practically **no** for a performance-critical live shard.

DOLSharp is on the order of **~420K lines** of game-server logic. A Python port would be a **multi-year rewrite**, not a mechanical translation. The hot path (combat ticks, effects, zone updates) would suffer versus optimized C# on OpenDAoC’s ECS loop.

## Recommended path (chosen)

Stay on **OpenDAoC (C# + ECS)** and restore trimmed expansion content via:

1. Config unlock (`GlobalConstants`, class `EligibleRaces`, server properties)
2. Database import (spell lines, specs, styles, startup locations)
3. Targeted ECS fixes where handlers are incomplete (e.g. Warlock chambers)

## Hybrid option (not pursued)

Python for **tooling only** (DB import scripts, admin bots, CI) while the gameserver remains C# — already partially done in `OpenDAoC-Database/tools/`.

## Related

- [`opendaoc-restore-strategy.md`](opendaoc-restore-strategy.md) — why restoring on OpenDAoC beats rewriting
