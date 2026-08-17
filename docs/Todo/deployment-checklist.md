# Deployment checklist

Run against the **live** MariaDB used by the gameserver.

## Core image

```bash
# Build/push from LS-OpenDAoC-Core (your CI or local Docker)
# Target: ghcr.io/bedrock1977/ls-opendaoc-core:latest
```

Restart gameserver container after image update.

## Database patches

From [OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) checkout (`feature/expansion-classes` or merged main):

```bash
mysql -u root -p opendaoc < opendaoc-db-core/patches/expansion-classes-import.sql
mysql -u root -p opendaoc < opendaoc-db-core/patches/expansion-serverproperties.sql
mysql -u root -p opendaoc < opendaoc-db-core/patches/expansion-startup-locations.sql
mysql -u root -p opendaoc < opendaoc-db-core/patches/toa-mauler-import.sql
mysql -u root -p opendaoc < opendaoc-db-core/patches/toa-startup-locations.sql
mysql -u root -p opendaoc < opendaoc-db-core/patches/achievements.sql
```

Skip any patch already applied (check `serverproperty`, `startuplocation`, spell counts).

## Post-patch server properties

Confirm in `serverproperty`:

| Key | Expected |
|-----|----------|
| `disabled_expansions` | empty |
| `disabled_races` | empty |
| `start_as_base_class` | `False` |
| `player_base_speed` | `232` (or your chosen value) |

## Existing characters

```sql
UPDATE dolcharacters SET MaxSpeed = 232 WHERE MaxSpeed < 232;
```

Adjust value to match `player_base_speed`.

## In-game reload

After startup location patches:

```
/refresh startuplocations
```

Or full gameserver restart.

## Verification pointer

See [`backlog.md`](backlog.md) P1 checklist.
