# Repository Guidelines

## Project Structure & Module Organization

DOTsAlpha is a Tuanjie **1.5.3** project (Unity compatibility **2022.3.55t4**) using Entities and URP.

- `Assets/Scripts/`: C# ECS components, authoring MonoBehaviours, bakers, and systems. `CubeSpawnSystem` instantiates entities once; `CubeMoveSystem` schedules parallel movement jobs.
- `Assets/Scenes/SampleScene.scene`: main scene; `Assets/Scenes/SampleScene/ECSWorld.unity`: ECS SubScene containing the spawner.
- `Assets/Prefabs/`, `Assets/Materials/`, and `Assets/Settings/Rendering/`: cube assets and rendering configuration.
- `Packages/`: dependency manifest, lockfile, and embedded `com.unity.textmeshpro`, including its package tests.
- `ProjectSettings/`: shared Editor configuration. `Library/`, `Temp/`, and generated IDE files are ignored.

## Build, Test, and Development Commands

Use the Editor version recorded in `ProjectSettings/ProjectVersion.txt`. In PowerShell, set `$Editor` to its executable path, then run from the repository root:

```powershell
& $Editor -projectPath .
& $Editor -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults.xml -logFile test.log
```

The first command opens the project; the second runs EditMode tests. Substitute `PlayMode` for runtime tests. Close the interactive Editor before running batch tests against this checkout.

For local execution, open `SampleScene.scene` and press Play. For player builds, add that scene in **File > Build Settings** and build for the intended platform; the scene list is currently empty. No custom build script is provided.

## Coding Style & Naming Conventions

Use four-space indentation and opening braces on separate lines. Match existing PascalCase types, methods, and public component fields; use camelCase locals. Follow `Cube*Authoring`, `Cube*Baker`, and `Cube*System` naming. Keep runtime data in `IComponentData`, baking in bakers, and simulation in systems/jobs. Preserve Burst compatibility and job dependencies. No repository formatter or lint configuration is present.

## Testing Guidelines

Unity Test Framework/NUnit is available. No project-specific test suite or coverage threshold exists. Add tests under `Assets/Tests/EditMode` or `Assets/Tests/PlayMode` with test assembly definitions; use `*Tests.cs` and descriptive names such as `Spawn_CreatesConfiguredCount`. Verify spawning, movement bounds, and Console errors in Play Mode. Report automated and manual validation separately.

## Commit & Pull Request Guidelines

History contains only `Initial import of DOTsAlpha Unity project`; no broader convention is established. Use concise imperative subjects. Keep commits focused and include asset `.meta` files. PRs should describe behavior changes, validation, related issues, and screenshots for visual changes. Review scene and package diffs; preserve unrelated local edits and exclude caches, builds, and credentials.
