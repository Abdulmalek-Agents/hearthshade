# Hearthshade — Unity Project (Unity 6000.4.4f1)

Onboarding guide for the Unity prototype of **Hearthshade**, the studio's #1 niche pick (dark-cozy life-sim).
This project scaffolds the **vertical slice** described in `../GDD.md` §12: one homestead, one Autumn season,
the full **day → dusk → night → dawn** loop with the **Gloam / Light / Resolve** systems.

---

## 1. Requirements

| Tool | Version |
|---|---|
| Unity Editor | **6000.4.4f1** (Unity 6) |
| Render pipeline | **URP** (2D Renderer with 2D lights) |
| Input | **Input System** package (new) |
| Language | C# (.NET Standard 2.1) |

Packages are pinned in `Packages/manifest.json`. Open the folder in Unity Hub with **6000.4.4f1**; the editor
will resolve packages on first import.

## 2. First-time setup

1. **Unity Hub → Add → select this `unity/` folder.** Open with 6000.4.4f1.
2. Let packages resolve (URP, Input System, Cinemachine, Newtonsoft JSON via UPM).
3. Open `Assets/Scenes/Homestead.unity` (build steps in `ProjectStructure.md`; scenes are authored in-editor —
   only the manifest + script scaffolding ship in git so the repo stays diff-friendly).
4. Project Settings → Player → **Active Input Handling = Input System Package (New)**.
5. Graphics → Scriptable Render Pipeline Settings → assign the URP 2D asset (`Assets/Settings/URP_2D.asset`).
6. Press Play. The `GameBootstrap` prefab spins up the systems in order (see §4).

## 3. Architecture at a glance

```
GameBootstrap (DontDestroyOnLoad)
 ├── GameManager           // top-level state, scene flow, difficulty band
 ├── GameClock             // phase/time state machine (raises OnPhaseChanged)
 ├── GloamSystem           // tile-based dread fluid sim (grid)
 ├── LightingRegistry      // registers Lantern/Hearth/MoonWell light emitters
 ├── ResolveSystem         // player "nerve" soft-survival meter
 ├── FarmSystem            // crop growth + wither-on-gloam
 └── SaveSystem            // JSON save/load (Newtonsoft)

Scene actors
 ├── Player (PlayerController + PlayerInput)
 ├── Lantern* (prefab, Interactable, ILightEmitter)
 ├── Hearth   (prefab, ILightEmitter, RestPoint)
 ├── Villager* (prefab, Schedule + Fear)
 └── CropTile* (data-driven via FarmSystem grid)
```

**Event flow:** `GameClock` is the heartbeat. It fires `OnPhaseChanged(Phase)` and `OnNightTick()`.
`GloamSystem` advances only on night ticks; `LightingRegistry` supplies the per-tile light values the
Gloam reads; `ResolveSystem` samples light at the player's tile to drain/restore nerve. Nothing polls in
`Update()` that an event can deliver instead.

## 4. Bootstrap order

`GameBootstrap.cs` calls `IGameSystem.Init()` deterministically: `LightingRegistry → GloamSystem →
FarmSystem → ResolveSystem → GameClock → GameManager`. This avoids race conditions where the Gloam reads a
not-yet-built light grid.

## 5. Unity 6 features used

- **URP 2D Renderer + 2D Lights** for the literal cozy/dread lighting (the day→night desaturation is a global
  `Volume` color-adjust driven by `GameClock`).
- **Input System** (`PlayerControls.inputactions`) — Move/Interact/Place/Rest.
- **Cinemachine 3** virtual camera follows the player with a soft confiner.
- **Awaitable / async** used in `SaveSystem` for non-blocking disk writes (Unity 6 `Awaitable`).
- (Optional) **Unity Sentis** is *not* required for the slice; flagged in `ProjectStructure.md` as a future
  hook for villager "fear" behaviour models.

## 6. Where to start reading

`Assets/Scripts/Core/GameClock.cs` → `Systems/GloamSystem.cs` → `Gameplay/PlayerController.cs`.
Each file has a header comment mapping it to the GDD system it implements.

## 7. Build targets

PC (Windows/Mac/Linux) StandaloneicONURP first. Switch 2 / console and premium mobile are post-1.0 (see GDD §8).
