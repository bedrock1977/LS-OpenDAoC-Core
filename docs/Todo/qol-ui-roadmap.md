# QoL and UI roadmap

**Last updated:** 2026-08-17

Phased plan for merchant/inventory helpers, quests, achievements, and [daochook](https://github.com/daochook/daochook) integration.

Design reference: [`../documentation/player-ui-and-daochook.md`](../documentation/player-ui-and-daochook.md)

---

## Phase 0 — Compatibility spike

- [ ] Download daochook; test inject against **exact** Catacombs client used on LS shard (1127–1129)
- [ ] Record client build number (e.g. `[1409]`) and document pass/fail in `client-and-deployment.md`
- [ ] If fail: contact daochook Discord / open issue for Catacombs client support
- [ ] Decide shard policy: **optional opt-in** vs required (recommend opt-in)

---

## Phase 1 — Server-only (all players, no client mod)

### Commands

- [ ] `/bags` — inventory summary via `SendCustomTextWindow`
- [ ] `/quests` — active quest list and steps
- [ ] `/achievements` — list unlocked + in-progress (after Phase 2 DB exists, stub OK first)

### NPCs

- [ ] Bulk sell NPC (greys/trash, confirm dialog)
- [ ] Optional hub merchant (buffs / supplies / vault keywords)

### Achievements — server foundation

- [ ] DB patch: `achievement`, `character_achievement` tables
- [ ] C# `AchievementService` + hooks (level up, kill, quest complete, dynamic event)
- [ ] Unlock notification chat message
- [ ] Structured token for addons: `[LSACH] unlock|key|Title|points`

---

## Phase 2 — Atlas API (companion + daochook data)

- [ ] Enable `atlas_api` on staging
- [ ] Add authenticated session endpoint (account-bound, online char only)
- [ ] `GET /me/inventory`, `/me/quests`, `/me/achievements`
- [ ] `GET /shard/achievements` (definitions)
- [ ] Rate limits and API password / JWT config in server properties

---

## Phase 3 — daochook addons (opt-in players)

Repo suggestion: `LS-OpenDAoC-Addons` or `launcher/addons/ls-shard/`

- [ ] **ls_achievements** — ImGui panel, `/lsach`, parse `[LSACH]` or poll API
- [ ] **ls_bags** — bag overlay from API or `/bags`-equivalent poll
- [ ] **ls_quests** — pinned quest tracker
- [ ] README: install via daochook `addons/` folder

---

## Phase 4 — Launcher integration

- [ ] LS-OpenDAoC-Launcher: optional “Enable daochook” checkbox
- [ ] Ship or link addon pack
- [ ] Player-facing doc page on shard website / Discord

---

## Phase 5 — Web companion (optional)

- [ ] Simple React/Vue app: achievements, quest log, bag (second monitor)
- [ ] Same API as Phase 2
- [ ] Link from Account Manager or launcher

---

## Out of scope (for now)

- Native merchant/inventory window redesign (impossible without client source)
- Custom quest journal tabs in stock UI
- Mandatory daochook before client version spike passes
