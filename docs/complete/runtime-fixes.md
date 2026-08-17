# Runtime fixes (expansion characters)

**Completed:** during SI/TOA unlock testing.

## `start_as_base_class`

**Problem:** New expansion characters saved as **base classes** (e.g. Warlock → Mystic 36).

**Fix:**

- Code default: `StartAsBaseClass.cs` → `start_as_base_class = false`
- DB: `expansion-serverproperties.sql` sets `start_as_base_class = False`

**Rule:** Must be `False` on any shard using Heretic / Warlock / Vampiir / Mauler.

## Missing startup locations

**Problem:** New chars spawned at **region 0, coords 0,0,0** (e.g. Frostalf Warlock).

**Fix:** SQL patches add `startuplocation` rows:

- `expansion-startup-locations.sql` — classes 33, 58, 59 (IDs 1003–1011)
- `toa-startup-locations.sql` — Maulers 60/61/62 + Half Ogre Alb base classes (IDs 1012–1023)

Reload: `/refresh startuplocations` or gameserver restart.

## Warlock chamber null reference

**Fix:** `ChamberSpellHandler.cs` — null-safe chamber release via `StartSpell`.
