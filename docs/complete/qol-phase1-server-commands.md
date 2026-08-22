# QoL Phase 1 (server commands and achievements)

**Completed:** code on `master` (Core) and `feature/expansion-classes` (Database patch).

**Live verified:** 2026-08-17 — `/bags`, `/achievements` confirmed on production container. `/quests` and `/selljunk` (`/sellgreys`) initial testing OK; full edge-case pass pending. Level 2 unlock fires **First Steps** with `[LSACH] unlock|...` in system chat (daochook-ready).

## Player commands

| Command | Description |
|---------|-------------|
| `/bags` | Backpack slot usage, weight, sorted item list (text window) |
| `/quests` | Active quests with step + description (text window; improved from chat-only) |
| `/achievements` (`/ach`) | Achievement progress by category (text window) |
| `/sellgreys` (`/selljunk`) | Sell low-quality backpack items to targeted merchant |

## Achievements

- Tables: `Achievement`, `CharacterAchievement`
- SQL patch: `opendaoc-db-core/patches/achievements.sql`
- Level milestones: welcome (1), first_steps (2), apprentice (10), veteran (20), champion (35), hero (50)
- Unlock chat + daochook token: `[LSACH] unlock|key|Title|points`
- Server property: `achievements_enabled` (default true after patch)

## Code

- `GameServer/gameutils/AchievementMgr.cs`
- `GameServer/scripts/gameevents/AchievementTracker.cs`
- `GameServer/commands/playercommands/bags.cs`, `achievements.cs`, `sellgreys.cs`
- `CoreDatabase/Tables/DbAchievement.cs`, `DbCharacterAchievement.cs`

## Deploy

```bash
mysql -u root -p opendaoc < opendaoc-db-core/patches/achievements.sql
```

Rebuild gameserver image, restart, test `/bags` and `/achievements` in-game.

## Next (Phase 2+)

See [`../Todo/qol-ui-roadmap.md`](../Todo/qol-ui-roadmap.md) — Atlas API, daochook Lua addons.
