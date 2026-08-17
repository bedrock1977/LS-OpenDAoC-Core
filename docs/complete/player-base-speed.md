# Player base speed (default 232)

**Completed:** PR #1 merged to `master` (`8c09f60a5`).

## Change

- Added server property `player_base_speed` in `ServerProperties.cs`
- `GamePlayer.PlayerBaseSpeed` reads property; used on char create, load fallback, movement percent
- Default changed from live-like **191** to **232** for this shard

## Existing characters

SQL after deploy:

```sql
UPDATE dolcharacters SET MaxSpeed = 232 WHERE MaxSpeed < 232;
```

Match value to `player_base_speed` in `serverproperty`.

## Files

- `GameServer/serverproperty/ServerProperties.cs`
- `GameServer/gameobjects/GamePlayer.cs`
