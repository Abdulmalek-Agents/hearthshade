# Hearthshade — Unity Project Structure, Scene Manifest & Prefab Specs

## Folder layout

```
unity/
├── Packages/manifest.json          # pinned UPM packages (URP, Input System, Cinemachine, Newtonsoft)
├── ProjectSettings/ProjectVersion.txt   # 6000.4.4f1
├── README.md                       # onboarding
├── ProjectStructure.md             # this file
└── Assets/
    ├── Scripts/
    │   ├── Core/        GameBootstrap, GameManager, GameClock, IGameSystem, Phase
    │   ├── Systems/     GloamSystem, LightingRegistry, ResolveSystem, FarmSystem, SaveSystem
    │   ├── Gameplay/    PlayerController, Interactable(IInteractable), Lantern, Hearth, CropTile, Villager
    │   ├── UI/          HUDController
    │   └── Data/        PhaseData, CropData, ItemData, SaveData (ScriptableObjects + DTOs)
    ├── Scenes/          Boot.unity, Homestead.unity        (authored in-editor)
    ├── Prefabs/         (specs below)
    ├── Art/             sprites, 2D-light cookies, tilemaps
    ├── Audio/           day/night room-tone, whispers, hearth crackle
    └── Settings/        URP_2D.asset, GlobalVolume profile, PlayerControls.inputactions
```

## Scene manifest

### `Boot.unity`
- `GameBootstrap` (prefab) — only object. Instantiates the persistent systems then async-loads `Homestead`.

### `Homestead.unity` (the vertical slice)
| Object | Components | Notes |
|---|---|---|
| `Grid` | Grid, Tilemap (Ground/Soil/Water) | 15×11 logical grid mirrors the HTML prototype |
| `Player` | PlayerController, PlayerInput, Rigidbody2D (kinematic), Cinemachine target | spawn at hearth |
| `Hearth` | Hearth, ILightEmitter, Light2D, RestPoint | permanent center light, Resolve restore |
| `Lantern_01..n` | Lantern, IInteractable, ILightEmitter, Light2D | oil upkeep; refuel/place |
| `MoonWell_01` | MoonWell, ILightEmitter (restorable) | campaign set-piece / progression anchor |
| `Villager_Mira`, `Villager_Tom`, `Villager_Edda` | Villager, Schedule, FearState | 3-NPC slice cast |
| `GloamVolume` | (rendering) global Volume + GloamSystem debug overlay | desaturation driven by GameClock |
| `CM vcam1` | CinemachineCamera | soft follow + confiner |
| `HUDCanvas` | HUDController | phase, Resolve, Hold%, inventory |

## Prefab specs

**`Player.prefab`**
- Visual: 2D sprite + soft self-aura Light2D (radius 1.4, intensity 0.25) so the player is never fully black.
- Logic: `PlayerController` (grid-stepped movement, 0.12s step), reads `PlayerControls` actions.
- Collisions: kinematic Rigidbody2D + tile blocker for Water.

**`Lantern.prefab`**
- `Light2D` (Freeform/Point, radius ~1.9 tiles, color warm `#E8B06A`), intensity scales with `oil`.
- `Lantern` script: `float oil (0..6)`, `Refuel(int)`, `Burn(dt)` at night; emits to `LightingRegistry`.
- `IInteractable.Interact()` → refuel from player inventory.

**`Hearth.prefab`**
- Always-on `Light2D` (radius ~2.6), `RestPoint` (Space/Rest → Resolve +18, day only).
- Registers as the highest-priority light source; Gloam can never fully take the hearth tile.

**`MoonWell.prefab`**
- Starts dormant (no light). `Restore()` over a multi-step quest lights it permanently → expands safe zone.
- Acts as the slice's "boss-equivalent" milestone.

**`Villager.prefab`**
- `Schedule` (daytime waypoints), `FearState` (rises in gloam/darkness, falls near light + after Rituals).
- On difficulty *Hollow*, sustained max Fear → `Fade()` (opt-in permadeath). On *Gentle/Tended*, clamped.

**`CropTile`** — not a prefab; managed by `FarmSystem` as a grid cell with `CropData` (stages, grow time,
sell value, withers when local gloam > 0.6).

## Data (ScriptableObjects)

- `PhaseData` — name, duration, isNight, ambient color grade (drives the Volume).
- `CropData` — stages, growSeconds, sellValue, witherThreshold.
- `ItemData` — id, display, kind (Seed/Oil/Lantern/Food), value.

## Difficulty bands (GameManager)

| Band | Permadeath | Night length | Resolve drain |
|---|---|---|---|
| Gentle | off | short | gentle |
| Tended (default) | preventable | medium | standard |
| Hollow | on | long | harsh |
