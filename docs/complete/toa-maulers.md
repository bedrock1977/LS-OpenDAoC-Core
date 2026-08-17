# TOA Maulers and races

**Completed:** code + database patches on `feature/expansion-classes`.

## Code (LS-OpenDAoC-Core)

- `GlobalConstants.cs` — MaulerAlb/Mid/Hib + HalfOgre + Minotaur races enabled
- `ClassMaulerAlb.cs`, `ClassMaulerMid.cs`, `ClassMaulerHib.cs` — eligible races set

| Class | ID | Eligible races |
|-------|-----|----------------|
| Mauler Alb | 60 | Korazh (Minotaur), Briton, Inconnu |
| Mauler Mid | 61 | Deifrang (Minotaur), Kobold, Norseman |
| Mauler Hib | 62 | Graoch (Minotaur), Celt, Lurikeen |

## Database (OpenDAoC-Database)

- `toa-mauler-import.sql` — 139 spells, 36 linexspell, 114 styles (from db-public via `tools/import_toa_maulers.py`)
- `toa-startup-locations.sql` — spawn rows for 60/61/62 and Half Ogre Alb base classes

## Verification

- Mauler specs and fist wraps in trainer UI
- Minotaur models on character screen (Catacombs client required)
- See [`expansion-classes.md`](expansion-classes.md) checklist item 6
