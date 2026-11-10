# ☠️ Cursed Tides
 
A **3D third-person survival-action game** built in **Unity (C#)**. Captain Rogal must break the curse of a pirate island by returning stolen gems to their temple, fighting the undead guardians that stand in his way.
 
> Semester project at **SAE Institute Berlin** (March to July 2026). It combines three major technical pillars in one playable game: **AI**, **shaders and VFX**, and **procedural generation**, plus a custom **Unity Editor tool**.
> ---
 
## At a glance
 
| | |
|---|---|
| **Genre** | 3D third-person survival action |
| **Engine** | Unity (URP) · C# |
| **Duration** | March to July 2026 |
| **Art style** | Low-poly, three chromatic zones |
| **Key tech** | FSM enemy AI · Shader Graph · VFX Graph · Procedural terrain · Editor tooling |

---

## The story
 
The island is cursed by **the Veil**, a creeping fog. Captain **Rogal** learns that the curse comes from gems taken from an ancient temple. To end it, he must return every gem to its pedestal while undead guardians defend them.

```
 Find gems  ──►  Survive guardians  ──►  Return gems to the temple  ──►  Break the Veil
                  (timer running)             (pedestals)
```
 
---

## My work, at a glance
 
```
┌───────────────────────────────────────────────────────────────┐
│  1. ENEMY AI           FSM hierarchy, ~10 states per enemy    │
│  2. COMBAT SYSTEMS     Damage, health, hitboxes, checkpoints  │
│  3. PROCEDURAL MAP     Perlin mesh, height texture, spawning  │
│  4. SHADERS and VFX    Water, gems, coins, fire, aura         │
│  5. EDITOR TOOL        Enemy spawner window with undo         │
│  6. GAMEPLAY LOOP      Gems, pedestals, win and lose states   │
└───────────────────────────────────────────────────────────────┘
```

### 1. Enemy AI (finite state machines)
 
```
Enemy (abstract)
 ├── EnemyMelee   ~10 states
 └── EnemyRanged  ~10 states
        └── each state is its own class, stored in a state dictionary
```
 
| Feature | Implementation |
|---|---|
| Field of view | Dot product between enemy forward and direction to player |
| Group behaviour | Nearby enemies are alerted with `Physics.OverlapSphere` |
| Ranged enemies | Multi-phase alert behaviour and a `ProjectileLauncher` |
| Melee attacks | `WeaponHitbox` driven by animation events |
| Architecture | Abstract base classes, generic `BaseState`, separate states enum |
 
### 2. Combat systems
 
| System | Purpose |
|---|---|
| `IDamageable` | Common contract for anything that can take damage |
| `PlayerHealth` | Health with C# events (e.g. death event the AI listens to) |
| `Hurtbox` | Body-part damage multipliers |
| `WeaponHitbox` | Attack collision, enabled by animation events |
| `CheckpointManager` | Respawn points |
 
### 3. Procedural map generation
 
```
Perlin noise ──► Mesh vertices ──► Height-based texture gradient ──► Final shader ──► Spawn script
                                                                                     (props, gems,
                                                                                      collectibles, player)
```
 
The terrain mesh is built from scratch (vertices, triangle winding order, UVs and normals), then populated automatically at runtime.
 
### 4. Shaders and VFX
 
| Effect | Tool | Technique |
|---|---|---|
| **Water** | Shader Graph | Depth-based colour, scrolling Voronoi specular, sinusoidal waves |
| **Gems and crystals** | Shader Graph | Fresnel, pulsing HDR emission, Object Space Y gradient |
| **Coin glint** | Shader Graph | Shimmer effect |
| **Bonfire** | VFX Graph | Fire, sparks and smoke |
| **Cursed enemies** | VFX Graph | Aura |
| **Ambience** | VFX Graph | Fireflies |
| **Cannons** | VFX Graph | Fire burst |
 
### 5. Custom Editor tool: `EnemySpawnerWindow`
 
- Scans the project for enemy prefabs
- Customises character skin and attachments with radio buttons and toggles
- Full **Ctrl+Z undo** support
- Built with the **Factory** and **Command** design patterns
### 6. Gameplay loop
 
| Element | Role |
|---|---|
| `IInteractable` | Shared interface for all interactions |
| Gems (`GemType` enum) | Main collectible, tied to the story |
| Pedestals | Gems are returned here |
| `GameManager` | Win condition (all gems returned) and timer-based lose condition |
 
### 7. Player
 
- Rigidbody-based movement with camera-relative controls
- Sprint and ground check
- Third-person camera controller
- Input handled through a dedicated input reader
---

## License
 
Copyright © 2026 Andrea Evangelista. All rights reserved.
 
The source code is published for portfolio and educational viewing only.
Copying, modifying, distributing or commercial use is not permitted without
written permission. Third-party assets belong to their respective authors
and are used under their own licenses.










