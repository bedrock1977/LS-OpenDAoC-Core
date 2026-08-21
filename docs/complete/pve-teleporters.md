# PvE capital teleporters

Capital hub NPCs for fast PvE travel: one **World Teleporter** and one **Dungeon Master** in each realm capital.

## Locations

Spawned automatically on server start when `pve_teleporters_enabled = true` (default):

| Realm | Region | Capital | NPCs |
|-------|--------|---------|------|
| Albion | 10 | Camelot | World Teleporter + Dungeon Master |
| Midgard | 101 | Jordheim | World Teleporter + Dungeon Master |
| Hibernia | 201 | Tir na Nog | World Teleporter + Dungeon Master |

NPCs appear near the capital center (a short walk from the default city teleporter). If an NPC with the same name already exists in that capital region, the spawner skips it.

## World Teleporter

Single flat menu on interact — no sub-menus for towns, housing, or Shrouded Isles. Whisper any bracketed destination to travel.

Covers everything a realm's standard teleporter offers:

- Frontier keeps / wharfs
- All Shrouded Isles ports for that realm
- Capital
- Housing: entrance, personal house, guild house, hearth bind
- All mainland towns from `teleport.sql`
- That realm's epic dungeon (Caer Sidi / Tuscaran Glacier / Galladoria)

Uses the same DB teleport table and portal spell as `LiveTeleporter`.

## Dungeon Master

Separate NPC beside the World Teleporter. Ports to **any** major dungeon entrance in the game:

- Full classic dungeon sets for Albion, Midgard, and Hibernia
- Epic dungeons (Caer Sidi, Tuscaran Glacier, Galladoria, Trollheim, Fomor, Avalon City, Krondon, etc.)
- Catacombs frontier dungeons (Marfach, Hall of the Corrupt, Doden's Gruva)
- Darkness Falls — `[Darkness Falls]` uses your realm's entrance; explicit keywords for all three realm entrances

Coordinates are hardcoded from `zonepoint.sql` dungeon entrances.

## Server property

| Property | Default | Description |
|----------|---------|-------------|
| `pve_teleporters_enabled` | `true` | Spawn hub NPCs in Camelot, Jordheim, and Tir na Nog |

## Code

| File | Role |
|------|------|
| `GameServer/scripts/teleporters/PvEWorldTeleporter.cs` | Flat-menu capital hub teleporter |
| `GameServer/scripts/teleporters/PvEDungeonMaster.cs` | All-realm dungeon teleporter |
| `GameServer/scripts/teleporters/PvETeleporterSpawn.cs` | `[GameServerStartedEvent]` spawner |

## Testing

1. Rebuild / redeploy the core container.
2. Confirm `pve_teleporters_enabled` is true in server properties.
3. `/mob create` or visit Camelot, Jordheim, and Tir na Nog on a character of each realm.
4. Click **World Teleporter** — full destination list should appear in one window.
5. Whisper `[Castle Sauvage]`, `[Gothwaite Harbor]`, `[Personal]`, etc.
6. Click **Dungeon Master** — full dungeon list; whisper `[Tomb of Mithra]`, `[Darkness Falls]`, `[Galladoria]`.
7. Verify combat blocks teleport and relic carriers are rejected.

## Notes

- Oceanus / Atlantis ports are intentionally omitted (require GM priv on stock teleporters).
- Battlegrounds are not included on the World Teleporter hub.
- To adjust spawn positions, edit `CapitalSpawns` in `PvETeleporterSpawn.cs`.
