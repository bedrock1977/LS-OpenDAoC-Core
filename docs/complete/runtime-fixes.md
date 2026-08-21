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

## Unrestricted realm races per class

**Goal:** Any race in a realm can be any class in that realm (e.g. Half Ogre Wizard, Minotaur Bard).

**Fix:**

- Server property: `allow_all_realm_races_for_classes = True` (default in code)
- DB: `expansion-serverproperties.sql`
- Logic: `CharacterClassRaceRules.cs` — used on character create and trainer promotion

**Still enforced:** `disabled_races`, race gender locks (Minotaur male-only), class gender locks (Valkyrie/Bainshee female-only).

**Note:** The character creation **client UI** may still grey out some race/class pairs. The server accepts any valid realm race + realm class combo the client sends. If a combo is blocked in UI only, a client patch or daochook addon may be needed for visibility.
