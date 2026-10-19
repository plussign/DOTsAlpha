# Repository Guidelines

## Project Structure & Module Organization

DOTsAlpha is a Tuanjie **1.5.3** project (Unity compatibility **2022.3.55t4**). The manifest specifies Entities **1.3.15**, Entities Graphics **1.3.2**, and URP **14.1.0**. Read `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, and `Packages/packages-lock.json` before changing Editor or dependency versions.

- `Assets/Scripts/`: four C# files containing ECS components, authoring MonoBehaviours, bakers, and systems. There are no project-specific assembly definitions.
  - `CubeSpawnerAuthoring.cs`: `CubeSpawner`, authoring settings, and baker; requires a cube prefab and clamps basic configuration values.
  - `CubeMovementAuthoring.cs`: `CubeMovement`, authoring component, and baker; initializes the entity's `URPMaterialPropertyBaseColor` from the material's linear `_BaseColor`, with a white fallback.
  - `CubeSpawnSystem.cs`: uses a singleton spawner, bulk-instantiates `Rows * Columns` entities, assigns XZ grid positions and per-entity rotation/random settings, then disables itself.
  - `CubeMoveSystem.cs`: schedules a Burst-compiled parallel `IJobEntity` that updates rotation and smooth color transitions. Despite its name, it currently performs no positional movement.
- `Assets/Scenes/SampleScene.scene`: main scene; `Assets/Scenes/SampleScene/ECSWorld.unity`: ECS SubScene containing the spawner.
- `Assets/Prefabs/`, `Assets/Materials/`, and `Assets/Settings/Rendering/`: cube assets and rendering configuration.
- `Packages/`: dependency manifest, lockfile, and embedded `com.unity.textmeshpro`, including its package tests. Unity MCP is a Git dependency; preserve its manifest reference and lockfile resolution unless a dependency update is intended.
- `ProjectSettings/`: shared Editor configuration. `Library/`, `Temp/`, and generated IDE files are ignored.

## Current Sample Behavior

The saved `CubeSpawner` in `ECSWorld.unity` uses `Rows = 50`, `Columns = 50`, `Step = 1.5`, `RoundSpeedRange = (-5, 5)`, `ColorChangeSpeed = 5`, and `Seed = 12345`. These scene overrides differ from the C# defaults. The scene spawns 2,500 cubes at Y = 0; rotation speed is in radians per second. Keep a single spawner because `CubeSpawnSystem` calls `GetSingleton<CubeSpawner>()`.

Color transitions use `smoothstep` between the current color and a random target, with a base duration of 2–4 seconds divided by `ColorChangeSpeed`. A speed of zero pauses color interpolation. The current `movement.Rng = random` write-back is commented out, as is the old velocity code; do not document positional movement or a continuously advancing random color sequence as implemented behavior. Treat changes to those behaviors as code changes, not documentation cleanup.

## Build, Test, and Development Commands

Use the Editor version recorded in `ProjectSettings/ProjectVersion.txt`. In PowerShell, set `$Editor` to its executable path, then run from the repository root:

```powershell
& $Editor -projectPath .
& $Editor -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults.xml -logFile test.log
```

The first command opens the project; the second runs EditMode tests. Substitute `PlayMode` for runtime tests. Close the interactive Editor before running batch tests against this checkout.

For local execution, open `SampleScene.scene` and press Play. For player builds, add that scene in **File > Build Settings** and build for the intended platform; the scene list is currently empty. No custom build script is provided.

## Coding Style & Naming Conventions

Use four-space indentation and opening braces on separate lines. Match existing PascalCase types, methods, and public component fields; use camelCase locals. Follow the existing `CubeSpawnerAuthoring` / `CubeSpawnerBaker`, `CubeMovementAuthoring` / `CubeMovementBaker`, and `Cube*System` naming. Keep runtime data in `IComponentData`, baking in bakers, and simulation in systems/jobs. Preserve Burst compatibility and assign scheduled job handles to `state.Dependency`. Keep the prefab's renderer and movement authoring on the same GameObject, as assumed by the baker, and preserve the material's `_BaseColor` contract. No repository formatter or lint configuration is present.

## Testing Guidelines

Unity Test Framework/NUnit is available. No project-specific test suite or coverage threshold exists; embedded TextMeshPro tests are package tests, not coverage of the cube sample. For behavior changes that warrant automated tests, add them under `Assets/Tests/EditMode` or `Assets/Tests/PlayMode` with test assembly definitions and any runtime assembly setup needed to reference the project code. Use `*Tests.cs` and descriptive names such as `Spawn_CreatesConfiguredCount`.

In Play Mode, verify the configured entity count, one-time spawning, fixed XZ grid positions, rotation, smooth color interpolation, and no Console errors. Check that `ColorChangeSpeed = 0` pauses color changes without stopping rotation. Report automated and manual validation separately, and state explicitly when Editor tests or manual checks were not run. Documentation-only changes need content/path checks rather than an Editor test run.

## Commit & Pull Request Guidelines

Use concise imperative commit subjects; no broader commit convention is established. Keep commits focused and include asset `.meta` files. PRs should describe behavior changes, validation, related issues, and screenshots for visual changes. Review scene and package diffs; preserve unrelated local edits and exclude caches, builds, test output, and credentials. Update `README.md` when setup or sample behavior changes, and keep these guidelines aligned with the actual code and saved scene configuration.
