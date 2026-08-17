# Shard vision and scope

**Status:** Discussion — long-term direction.

## Target patch band

**Shrouded Isles + Catacombs + Trials of Atlantis** (not 1.65-only).

## Primary classes (phase 1 — done in code)

| Class | ID | Realm | Expansion |
|-------|-----|-------|-----------|
| Heretic | 33 | Albion | Catacombs |
| Warlock | 59 | Midgard | Catacombs |
| Vampiir | 58 | Hibernia | Shrouded Isles |

## Phase 2 (code done; live DB may lag)

| Class | ID | Notes |
|-------|-----|-------|
| Mauler Alb/Mid/Hib | 60/61/62 | Minotaur races + eligible base races |
| Half Ogre | race | Albion base classes startup rows |

## Deferred / future

- Master Levels, Atlantis mechanics beyond Maulers
- Full WAR-style public quests (beyond pilot)
- RvR population and keep balance tuning
- Custom shard rules (XP, speed, HC, etc.)

## Success criteria (live shard)

- [ ] Catacombs client creates all expansion classes with correct races
- [ ] Trainer UI and spec progression work per class
- [ ] Warlock chambers charge and release
- [ ] Vampiir STR-based power pool without mana bar errors
- [ ] Mauler specs and Minotaur models on char screen
- [ ] No regressions to classic 1.65 classes

Track verification in [`../Todo/backlog.md`](../Todo/backlog.md).
