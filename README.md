# DOTsAlpha

Tuanjie DOTS sample using Entities, Entities Graphics, Burst jobs, and the Universal Render Pipeline (URP). The sample spawns a grid of cubes that rotate in place and smoothly change color through per-entity material properties.

## Environment

- Tuanjie Editor **1.5.3**, with recorded Unity compatibility version **2022.3.55t4** (`ProjectSettings/ProjectVersion.txt`).
- Entities **1.3.15**, Entities Graphics **1.3.2**, and URP **14.1.0** (`Packages/manifest.json`).
- TextMeshPro is embedded in `Packages/com.unity.textmeshpro`.
- Unity MCP is included as a Git dependency; package restoration requires access to its GitHub repository and the configured package registry.

## Open the project

1. Clone this repository.
2. Open the project with Tuanjie Editor **1.5.3** (recorded Unity compatibility version: **2022.3.55t4**).
3. Allow the Editor to restore packages and import assets.
4. Open `Assets/Scenes/SampleScene.scene` and allow its auto-loaded `ECSWorld` SubScene to finish importing/baking.
5. Press Play to view the rotating, color-changing cube grid.

## Sample configuration

Open `Assets/Scenes/SampleScene/ECSWorld.unity` through the SubScene and select `CubeSpawner` to edit `CubeSpawnerAuthoring`. The saved scene values differ from the component's C# defaults:

| Field | Saved scene value | Purpose |
| --- | --- | --- |
| `CubePrefab` | `Assets/Prefabs/Cube.prefab` | Renderable prefab with `CubeMovementAuthoring` |
| `Rows` / `Columns` | `50` / `50` | Spawns 2,500 cubes once per runtime world |
| `Step` | `1.5` | Spacing on the XZ plane; Y remains zero |
| `RoundSpeedRange` | `(-5, 5)` | Random angular speed in radians per second |
| `ColorChangeSpeed` | `5` | Color transition speed multiplier; `0` pauses transitions |
| `Seed` | `12345` | Seed used to initialize per-entity random state |

Each color transition has a base duration of 2–4 seconds, scaled by `ColorChangeSpeed` (approximately 0.4–0.8 seconds at the saved value of `5`). Colors interpolate with `smoothstep`, starting from the baked material color on the first transition.

The current `CubeMoveSystem` updates rotation and color only; it does not translate cubes. Its random-state write-back (`movement.Rng = random`) is currently commented out, so successive color targets are not a continuously advancing random sequence.

## Code and assets

- `Assets/Scripts/CubeSpawnerAuthoring.cs`: spawner configuration, `CubeSpawner` data, and its baker. Baking clamps rows/columns to at least 1, spacing to at least 1, color speed to at least 0, and seed to at least 1.
- `Assets/Scripts/CubeMovementAuthoring.cs`: `CubeMovement` runtime data and a baker that adds `URPMaterialPropertyBaseColor` from the prefab material's `_BaseColor` (converted to linear color, or white as a fallback).
- `Assets/Scripts/CubeSpawnSystem.cs`: reads a single `CubeSpawner`, instantiates `Rows * Columns` entities in bulk, assigns grid positions and random rotation settings, then disables itself.
- `Assets/Scripts/CubeMoveSystem.cs`: schedules a Burst-compiled parallel `IJobEntity` for rotation and color transitions, preserving the system's job dependency.
- `Assets/Materials/CubeMat.mat` and `CubeSG.shadergraph`: cube material and shader; keep `_BaseColor` compatible with the ECS material property.
- `Assets/Settings/Rendering/`: URP pipeline, renderer, and post-processing assets.

## Development and validation

From the repository root in PowerShell, set `$Editor` to the installed Tuanjie Editor executable and open the project:

```powershell
& $Editor -projectPath .
```

Unity Test Framework is available, but there are currently no project-specific tests under `Assets/Tests`. The embedded TextMeshPro package includes its own tests. Close the interactive Editor before running batch tests against this checkout:

```powershell
& $Editor -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults.xml -logFile test.log
```

Use `-testPlatform PlayMode` for runtime tests. In manual Play Mode validation, check that 2,500 cubes are spawned once, positions remain fixed, rotation and color transitions run, and the Console has no errors. Set `ColorChangeSpeed` to `0` before entering Play Mode to check that color transitions pause while rotation continues.

For a player build, add `Assets/Scenes/SampleScene.scene` in **File > Build Settings** and build for the intended platform. The build scene list is currently empty, and no custom build script is provided.

## Source control

`Assets`, `Packages`, and `ProjectSettings` are tracked, including asset `.meta` files and the embedded TextMeshPro package. Unity caches, local Editor preferences, builds, and generated IDE project files are excluded.
