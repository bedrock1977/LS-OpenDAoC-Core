# Shard backlog

**Last updated:** 2026-08-17 (QoL Phase 1 live smoke test)

Priority-ordered work remaining for the LS OpenDAoC shard.

## P0 — Live deployment

- [ ] Apply DB patches on **production** database if not already run:
  - `toa-mauler-import.sql`
  - `toa-startup-locations.sql`
  - `expansion-serverproperties.sql` (confirm `start_as_base_class = False`, `player_base_speed = 232`)
- [x] Rebuild and deploy `ghcr.io/bedrock1977/ls-opendaoc-core:latest` with latest `master` (build `b454edc87+`, deployed 2026-08-17)
- [ ] `UPDATE dolcharacters SET MaxSpeed = 232` for existing characters (if using new default speed)

## P1 — Verification

- [ ] Smoke-test **fresh** characters for classes **33, 58, 59, 60, 61, 62**
- [ ] Confirm expansion races spawn in correct regions (not 0,0,0)
- [ ] Warlock: chamber charge, primer, curse release
- [ ] Vampiir: STR power pool, power on hit
- [ ] Mauler: specs visible, Minotaur models on char screen
- [ ] Document client setup for players ([`../documentation/client-and-deployment.md`](../documentation/client-and-deployment.md))

## P2 — Repository hygiene

- [ ] Merge `OpenDAoC-Database` branch `feature/expansion-classes` → `main`
- [ ] Optional: commit local `docker-compose.yml` changes if desired for team use

## P3 — Dynamic events

- [ ] Enable `dynamic_event_pilot_enabled` on test/staging
- [ ] Walk-test pilot in region 1; tune coords if zone data differs
- [ ] See [`dynamic-events-roadmap.md`](dynamic-events-roadmap.md) for full PQ system

## P4 — Future content

- [ ] TOA beyond Maulers (ML, Atlantis zones/mechanics)
- [ ] Contribution-based PQ rewards and multi-stage events
- [ ] Shard scope tuning (PvE vs RvR, population targets)

## P5 — QoL and UI (daochook + server)

See [`qol-ui-roadmap.md`](qol-ui-roadmap.md). Phase 1 server commands **done in code** — apply [`achievements.sql`](https://github.com/bedrock1977/LS-OpenDAoC-Database/blob/feature/expansion-classes/opendaoc-db-core/patches/achievements.sql) on live DB.

- [x] Apply `achievements.sql` on production database (achievements working in-game)
- [x] Rebuild/deploy gameserver with Phase 1 commands
- [x] Live smoke: `/bags`, `/achievements` verified; `/quests`, `/selljunk` initial OK
- [x] Achievement unlock at level 2 — chat shows `[LSACH] unlock|first_steps|...` token
- [ ] Full `/sellgreys` edge cases (empty bag, no merchant target, protected items)
- [ ] **PvE capital teleporters** — code done; rebuild/deploy and smoke-test in Camelot / Jordheim / Tir na Nog ([`../complete/pve-teleporters.md`](../complete/pve-teleporters.md))
- [ ] Phase 0: test [daochook](https://github.com/daochook/daochook) against Catacombs client
- [ ] Phase 2–4: Atlas API, Lua addons, launcher integration

## Completed (see [`../complete/`](../complete/))

Fork setup, SI classes (Heretic/Warlock/Vampiir), TOA Maulers code+DB patches, player base speed, dynamic event pilot script, runtime fixes (`start_as_base_class`, startup locations), QoL Phase 1 (`/bags`, `/achievements`, `/sellgreys`), PvE capital teleporters (World Teleporter + Dungeon Master).
