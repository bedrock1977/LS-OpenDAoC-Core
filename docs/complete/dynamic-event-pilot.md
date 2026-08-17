# Dynamic event pilot (WAR-style public quest sketch)

**Completed:** script + server properties (disabled by default).

Minimal proof-of-concept for zone events inspired by [ProjectWAR](https://github.com/Shmerrick/ProjectWAR) public quests — implemented with native OpenDAoC area triggers, not ported WAR code.

Design context: [`../documentation/projectwar-dynamic-events.md`](../documentation/projectwar-dynamic-events.md)

Future work: [`../Todo/dynamic-events-roadmap.md`](../Todo/dynamic-events-roadmap.md)

## What it does

1. Registers a circular **Area** when the gameserver starts (if enabled).
2. When a player **enters** the area and the event is off cooldown, spawns one **wave** of NPCs.
3. Tracks **participants** (players in the area at start, plus anyone credited on kills).
4. When all wave mobs die, grants a flat **XP reward** and starts a **cooldown**.

Script: `GameServer/scripts/gameevents/DynamicEventPilot.cs`

## Enable on your shard

Default is **disabled**. Set server properties (DB `serverproperty` table or `/property` GM command after first boot creates rows):

| Property | Default | Notes |
|----------|---------|--------|
| `dynamic_event_pilot_enabled` | `false` | Master switch |
| `dynamic_event_pilot_region` | `1` | Albion Camelot Hills |
| `dynamic_event_pilot_x` | `560000` | Area center |
| `dynamic_event_pilot_y` | `512000` | |
| `dynamic_event_pilot_z` | `2500` | |
| `dynamic_event_pilot_radius` | `2500` | Trigger radius |
| `dynamic_event_pilot_mob_count` | `5` | Wave size |
| `dynamic_event_pilot_mob_level` | `35` | Mob level |
| `dynamic_event_pilot_xp_reward` | `50000` | Per participant |
| `dynamic_event_pilot_cooldown_seconds` | `300` | 5 minutes |

Example SQL:

```sql
UPDATE serverproperty SET Value = 'True' WHERE [Key] = 'dynamic_event_pilot_enabled';
UPDATE serverproperty SET Value = '560000' WHERE [Key] = 'dynamic_event_pilot_x';
UPDATE serverproperty SET Value = '512000' WHERE [Key] = 'dynamic_event_pilot_y';
```

Restart the gameserver (or reload scripts if your deployment supports it) after changing `dynamic_event_pilot_enabled` or region/coordinates — the area is registered at startup.

## Testing

1. Enable properties above.
2. Rebuild/restart `ls-opendaoc-core`.
3. `/gm speed 4` to the default coords in region 1 (Camelot Hills — adjust if your zone data differs).
4. Walk into the circle; five **Restless Spirit** mobs should spawn.
5. Kill them; participants receive XP and a system message. Re-enter within cooldown — nothing happens until cooldown expires.

## Limitations (pilot scope)

- Single wave only — no stages, no contribution scoring, no loot chest.
- No reset if players wipe and leave; mobs remain until killed.
- Coordinates are static server properties, not DB-driven spawns.
- No client PQ UI (WAR banners/progress bar) — chat messages only.

## Next steps toward full public quests

See [`../Todo/dynamic-events-roadmap.md`](../Todo/dynamic-events-roadmap.md).
