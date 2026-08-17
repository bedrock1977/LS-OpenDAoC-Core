# LS OpenDAoC shard documentation

Project docs for the **bedrock1977** private shard (SI + Catacombs + TOA target).

## Layout

| Folder | Purpose |
|--------|---------|
| [`documentation/`](documentation/) | Discussions, feasibility notes, design ideas, and reference material |
| [`Todo/`](Todo/) | Work still to do — deployment, testing, and future features |
| [`complete/`](complete/) | Finished milestones with enough detail to reproduce or verify |

## Repositories

| Repo | Role |
|------|------|
| [LS-OpenDAoC-Core](https://github.com/bedrock1977/LS-OpenDAoC-Core) | Gameserver fork (this repo) |
| [OpenDAoC-Database](https://github.com/bedrock1977/OpenDAoC-Database) | SQL patches and import tools (`opendaoc-db-core/patches/`) |
| [LS-OpenDAoC-Launcher](https://github.com/bedrock1977/LS-OpenDAoC-Launcher) | Client launcher |

## Quick links

- **What's done:** [`complete/`](complete/)
- **What's next:** [`Todo/backlog.md`](Todo/backlog.md)
- **Why OpenDAoC (not Python):** [`documentation/opendaoc-restore-strategy.md`](documentation/opendaoc-restore-strategy.md)
- **Expansion class deployment:** [`complete/expansion-classes.md`](complete/expansion-classes.md)

## For agents

When resuming work on this shard, read `Todo/backlog.md` first, then scan `complete/` for context on what is already merged or patched. Do not treat `.cursor/plans/` as the source of truth — that plan file is frozen; these docs are maintained here.
