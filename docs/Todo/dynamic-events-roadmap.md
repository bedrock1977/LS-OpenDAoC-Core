# Dynamic events roadmap

**Status:** Todo — extends the completed pilot.

Pilot reference: [`../complete/dynamic-event-pilot.md`](../complete/dynamic-event-pilot.md)

Design reference: [`../documentation/projectwar-dynamic-events.md`](../documentation/projectwar-dynamic-events.md)

## Near term

- [ ] Enable pilot on staging; confirm spawn/reward/cooldown loop
- [ ] Tune default coordinates for Camelot Hills (or move to a quieter test zone)
- [ ] Add GM command or `/property` cheat sheet for operators

## Medium term

- [ ] **Contribution scoring** — weight XP by damage, healing, deaths, time in area
- [ ] **Multi-stage waves** — wave 2 / boss on timer via `ECSGameTimer`
- [ ] **Empty-area reset** — despawn mobs and clear state if no players for N minutes
- [ ] **Loot rewards** — item template or BP instead of flat XP only

## Long term

- [ ] **Data-driven events** — SQL or JSON defs (region, coords, mob templates, stages, rewards)
- [ ] **Realm-aware PQs** — Alb/Mid/Hib variants
- [ ] **Client feedback** — richer chat/broadcast; no native PQ UI on DAoC client

## Out of scope for OpenDAoC

- Porting ProjectWAR `PublicQuest` C# code directly
- Custom client UI for progress bars (would need client mod)
