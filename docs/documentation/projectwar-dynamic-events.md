# ProjectWAR and dynamic zone events

**Status:** Design reference — not a port target.

## Context

[ProjectWAR](https://github.com/Shmerrick/ProjectWAR) is a **Warhammer Online** emulator (DagonUO lineage). It is **not** DOL/OpenDAoC — different protocol, database, and object model. **Code cannot be copied directly.**

## What is useful

ProjectWAR’s **Public Quest** system (`WorldServer/World/Objects/PublicQuests/`) is a good **design reference** for:

- Staged objectives (waves, bosses)
- Contribution scoring
- Zone-wide player participation
- Reset and cooldown behavior

## OpenDAoC approach

Implement similar behavior with native OpenDAoC primitives:

| Mechanism | Use |
|-----------|-----|
| `Area.Circle` + `RegisterPlayerEnter` | Trigger zone when players enter |
| `GameLivingEvent.Dying` | Track wave clears |
| `ECSGameTimer` | Timed stages, reset on empty area |
| Server properties | Enable/disable, coords, rewards |

## Pilot

A minimal pilot is implemented — see [`../complete/dynamic-event-pilot.md`](../complete/dynamic-event-pilot.md).

Future work: [`../Todo/dynamic-events-roadmap.md`](../Todo/dynamic-events-roadmap.md).
