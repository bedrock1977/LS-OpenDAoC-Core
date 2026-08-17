# Fork and environment setup

**Completed:** 2026-08 (approx.)

## Repositories

| Repo | URL | Local path |
|------|-----|------------|
| Core | [bedrock1977/LS-OpenDAoC-Core](https://github.com/bedrock1977/LS-OpenDAoC-Core) | `C:\Users\Rick\Projects\LS-OpenDAoC-Core` |
| Database | [bedrock1977/OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) | `C:\Users\Rick\Projects\OpenDAoC-Database` |
| Launcher | [bedrock1977/LS-OpenDAoC-Launcher](https://github.com/bedrock1977/LS-OpenDAoC-Launcher) | — |

## Docker

- Gameserver image target: `ghcr.io/bedrock1977/ls-opendaoc-core:latest`
- MariaDB + gameserver stack running locally (compose file may differ from committed upstream)

## Branch strategy

- Core: `master` with feature work merged (expansion classes, player base speed PR #1)
- Database: `feature/expansion-classes` with SQL patches and import tools
