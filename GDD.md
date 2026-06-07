# Hearthshade - Game Design Document (GDD v0.1)

> **Tend the light. Mind the dark.**
> A dark-cozy life-sim where the chores that keep you safe are the same chores that keep you sane.

| | |
|---|---|
| **Working title** | Hearthshade |
| **Genre** | Dark-cozy life-sim (cozy-horror cross-genre) |
| **Sub-tags** | Farming sim, Life sim, Light horror, Atmospheric, Story-rich, Base management |
| **Engine** | Unity 6000.4.4f1 (URP) |
| **Players** | Single-player first, optional 1-4 drop-in co-op (post-1.0) |
| **Platforms** | PC (Steam) primary -> Nintendo Switch 2 / consoles, then mobile premium |
| **Price** | $19.99 premium, no microtransactions; cosmetic DLC post-launch |
| **Target rating** | ESRB Teen / PEGI 12 (dread, not gore) |

---

## 1. Vision Statement

Hearthshade is a farming-and-home life-sim set in a valley where night is not just "time the shops close" - it is a **living antagonist**. By day the player farms, cooks, decorates, and befriends a small permadeath-capable community. At dusk a creeping **Gloam** seeps in from the treeline, and the same cozy tools - lanterns, hearth fires, warm meals, kind words - become the player's defenses. The fantasy is not *survive the monster*; it is **make a home worth protecting and keep its little light burning**. We are deliberately building for the validated player who, in their own words, "finds comfort in both softness and shadow" - the audience cozy-horror has proven exists but that almost no polished, content-rich title currently serves.

**Design north star:** every system must be readable as *cozy* in daylight and *uneasy* at night using the **same** verbs. No mode switch, no combat minigame bolted on. Dread is environmental and social, never twitch-reflex.

## 2. Market Gap (why this, why now)

See `docs/MARKET_VALIDATION.md` for full sourcing. Summary of the thesis:

- **Cozy is the clearest five-year growth curve on Steam**, yet self-described cozy went from 0.4% (2022) -> 3.1% (2025) of games grossing >$100k LTD - still a *thin* supply relative to demand.
- **Cozy-horror specifically is being called "the next big gaming trend,"** with breakout proof points (Grimshire = "Overwhelmingly Positive" in Early Access) but **almost no large-scope, polished, narrative-rich entry**.
- **Horror was a top-3 indie genre by number of hit games in 2025**, and the cozy demographic is broadening (notably strong female-gamer growth).
- Analysts list **"low-stress horror experiences"** explicitly as an underserved, defensible blend.

**Positioning sentence:** *"The first content-complete dark-cozy life-sim - Stardew-deep daytime warmth fused with a dread you have to manage, not fight."*

## 3. Core Pillars

1. **One toolbox, two tones.** Lanterns, fire, food, friendship: comfort by day, survival by night.
2. **Dread is a resource, not a damage bar.** You manage *Gloam* and *Resolve*, never a health bar.
3. **Stakes that matter, kindness that pays.** Villagers can be lost permanently (opt-in). Protecting them is the emotional engine.
4. **Slow is the point.** Daily rhythm, ritual, and rest are mechanics, echoing the wellness/escapism driver behind cozy's rise.

## 4. Core Gameplay Loop

### 4.1 The Day (Cozy layer)
- **Morning:** wake at the hearth, check the **Gloam Line**, plan the day.
- **Daytime verbs:** till/plant/water/harvest; forage; cook; craft lanterns & wards; repair; talk to and gift villagers; decorate (decoration has mechanical value).
- **Economy:** sell surplus at the day market; buy seeds, oil, wax, salvage.

### 4.2 The Dusk (Transition)
- A scripted, telegraphed **"the light is going"** beat. Birds stop. Color desaturates ~15%. A soft timer to light the homestead and bank the fire.

### 4.3 The Night (Dread layer)
- The **Gloam** advances from the map edges toward lit areas. Lit tiles hold it back; unlit tiles let it pool.
- The player does **not** fight. They **maintain**: refill lantern oil, stoke the fire, calm a frightened villager, reseat a ward, or hold position in the light until dawn.
- **Whispers** (audio events) test nerve: investigating costs Resolve but can yield lore/rewards.
- **Dawn** burns the Gloam back. The further it got, the more cleanup next morning.

> The beat we are chasing: the player *wants* a bigger, prettier, more lantern-lit farm for cozy reasons - and that exact desire is what makes the farm defensible at night. **Cozy progression == survival progression.**

## 5. Systems Design

### 5.1 Gloam (the dark) - environmental antagonist
Tile-based "fluid" that spreads each night-tick along unlit, low-Warmth tiles. Counters: light radius, Warmth, Wards. Never kills the player directly; it **withers crops, frightens villagers, and erodes structures**.

### 5.2 Resolve (the player's nerve) - soft survival meter
Drains from darkness, Whispers, villager loss, hunger, no rest. Restores from warm meals, sleep, rituals, daylight. At zero the player **"loses the thread"** (melancholy soft-fail) and wakes at the hearth - **no death**.

### 5.3 Homestead & Decoration (cozy with teeth)
Furniture grants **Comfort** (homestead-wide Resolve regen + Fear resistance). Light sources have oil/wax upkeep (the core economy). **Moon-wells**: rare permanent light anchors restored over the campaign (progression milestones).

### 5.4 Villagers & Relationships
Small cast (8-12) with schedules, gift prefs, cozy questlines and night-fears. **Trust** unlocks help. **Permadeath is opt-in** (Gentle / Tended / Hollow).

### 5.5 Rituals (comfort-as-mechanic)
Short diegetic activities (banking the fire, a shared meal, a lullaby) that restore Resolve/Comfort. The game's "spells," framed as **care, not power**.

### 5.6 Seasons & Campaign Arc
Four seasons gate content and escalate the Gloam. ~15-25h main arc about *why* the valley dims, plus evergreen free-play.

## 6. Unique Selling Point (USP)

**The only cozy-horror where your defenses ARE your decorations.** Comfort and dread share one toolbox and one progression curve. No combat. No twitch skill. Just a home you love and a night you have to outlast.

## 7. Target Audience

- **Primary:** cozy-life-sim players (Stardew/Spiritfarer/Coral Island) who *also* wishlist atmospheric horror; skews broad and notably female-positive.
- **Secondary:** atmospheric/narrative horror fans seeking low-stress, high-mood experiences.
- **Tertiary:** streamers/cozy-stream audiences (the slow tension reads well on camera).

## 8. Platform & Launch Strategy

1. **Steam first.** Capsule co-tags "cozy" + "horror"; demo in **Steam Next Fest** to build wishlists.
2. **Wishlist engine:** free **Prologue** (the first autumn) as a permanent funnel.
3. **Switch 2 / console** port post-1.0 (cozy over-indexes on Nintendo).
4. **Premium mobile** later (APAC mobile-first upside), controls redesigned.

## 9. Monetization

- **$19.99 premium**, one-time. No ads, no loot, no energy timers.
- **Post-launch:** paid cosmetic/decor packs + a paid "new valley" story expansion (DLC = ~77% of indie ongoing spend).
- **No FTP.**

## 10. Market Positioning Map

```
                 HIGH DREAD
                     |
   Phasmophobia      |     [ HEARTHSHADE ]   <- empty, defensible quadrant:
   REPO / co-op      |      polished, content-rich,
   horror            |      single-player, cozy-first
                     |
 ---TWITCH/COMBAT----+----CARE/MAINTENANCE----
                     |
   Stardew Valley    |     Spiritfarer
   Coral Island      |     Strange Horticulture
                     |
                 LOW DREAD
```

## 11. Art & Audio Direction

- **Art:** hand-painted 2.5D, warm lantern palette by day; at night the same scene **loses saturation and gains long shadows** (URP 2D lights + global volume). Rule: *light = safe* must always be legible.
- **Audio:** diegetic-heavy. Day = acoustic, birds. Night = room-tone thins, sub-bass rises, Whispers are spatial. Music recedes at night so silence does the work.

## 12. Vertical Slice Scope

One homestead, one season (Autumn), 3 villagers, the full day->dusk->night->dawn loop, Gloam + Resolve + lantern-oil economy, one Moon-well restoration set piece, ~60-90 min of content. The HTML prototype models this in miniature; the Unity project is scaffolded toward it.

## 13. Risks & Mitigations

| Risk | Mitigation |
|---|---|
| Tone whiplash (cozy vs scary) | Shared-toolbox rule; dusk telegraph; difficulty bands incl. *Gentle* |
| "Cozy" tag saturation | Lead capsule with the *horror* differentiator; the blend is the moat |
| Permadeath alienates cozy core | Opt-in only; default *Tended* makes loss preventable with care |
| Scope creep | Vertical slice -> one season shipped as free Prologue first |

## 14. Deliverables in this repo

- `GDD.md` - this document
- `docs/MARKET_VALIDATION.md` - sources, traction signals, critic-cycle log
- `prototype/index.html` - self-contained HTML5 Canvas prototype of the day->night loop
- `unity/` - Unity 6000.4.4f1 project structure, core C# scripts, scene manifest, prefab specs, README
- `branding/` - logo + key-art (SVG)
