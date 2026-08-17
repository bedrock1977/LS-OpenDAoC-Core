# Client and deployment notes

**Status:** Reference — apply when connecting players or rebuilding the stack.

## Client patch and PacketLib

- OpenDAoC expects an **exact-match PacketLib** for the client version.
- Supported max in this fork’s testing: **1129**; **1130** rejected.
- Target client type: **Catacombs-era** (SI + Catacombs content), not 1.65-only.

## Known client mismatches

- **1127 + LoTM/NF client type** can cause character-screen issues for expansion races (models/UI).
- Recommend **Catacombs-type client** on versions **1127–1129** for Frostalf, Shar, Minotaur, etc.

## Docker

- Image target: `ghcr.io/bedrock1977/ls-opendaoc-core:latest`
- Local `docker-compose.yml` may pin MariaDB and port 3306 (local-only; not necessarily committed).
- Rebuild and push image after core code changes.

## Database

Apply patches from [OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) **`feature/expansion-classes`** branch (or merge to main when ready). See [`../Todo/deployment-checklist.md`](../Todo/deployment-checklist.md).

## Launcher

Configure [LS-OpenDAoC-Launcher](https://github.com/bedrock1977/LS-OpenDAoC-Launcher) for the Catacombs client path and server endpoint.
