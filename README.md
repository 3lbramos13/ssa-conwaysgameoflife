# Conway's Game of Life — Unity

A Unity implementation of [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) built with a clean layered architecture and a fully interactive runtime UI.

---

## Features

- Classic four-rule Conway simulation on a fixed orthogonal grid
- Three seeding modes: **Random**, **Pattern**, and **Mixed**
- Runtime controls for simulation speed, cell size, fill density, and config selection
- Live generation and alive-cell counters
- Multiple `GameOfLifeConfig` ScriptableObject assets switchable at runtime
- Smooth control-panel fade-in on startup

---

## Rules

At every generation, the following transitions are applied simultaneously to every cell:

| # | Rule | Outcome |
|---|------|---------|
| 1 | Live cell with **fewer than 2** live neighbours | Dies — underpopulation |
| 2 | Live cell with **2 or 3** live neighbours | Survives to next generation |
| 3 | Live cell with **more than 3** live neighbours | Dies — overpopulation |
| 4 | Dead cell with **exactly 3** live neighbours | Becomes alive — reproduction |

Boundary handling uses **hard edges** — cells outside the grid are treated as dead (no toroidal wrapping).

---

## Architecture

The project follows a strict inward dependency hierarchy:

```
UI → Presentation → Simulation → Domain
```

```
Assets/_Project/Scripts/
├── Configuration/          GameOfLifeConfig (ScriptableObject), InitializationMode
├── Domain/
│   ├── Interfaces/         IGrid
│   ├── Models/             GridModel
│   └── Rules/              IGameOfLifeRule, ConwayRule
├── Simulation/
│   ├── Interfaces/         ISimulationService
│   └── Services/           SimulationService
├── Presentation/
│   ├── Controllers/        GameController (MonoBehaviour, composition root)
│   └── Views/              GridView (MonoBehaviour, UI renderer)
├── Services/               ServiceLocator
└── UI/                     UIController (MonoBehaviour)
```

### Layer Responsibilities

| Class | Role |
|-------|------|
| `GameOfLifeConfig` | ScriptableObject data container for all tunable settings |
| `GridModel` | `bool[,]` backing store for cell state, implements `IGrid` |
| `ConwayRule` | Encodes the four canonical Conway rules, implements `IGameOfLifeRule` |
| `SimulationService` | Runs one generation step with a double-buffer, tracks generation count and alive count |
| `GameController` | Composition root — owns all runtime instances, drives the time-based simulation loop |
| `GridView` | Renders the grid as a 2D array of UI `Image` components |
| `UIController` | Bridges Unity UI events to `GameController` commands |
| `ServiceLocator` | Type-keyed dictionary for registering and resolving service instances |

---

## Simulation Loop

`GameController.Update()` uses an accumulator to fire one or more simulation steps per frame:

```
interval = 1 / simulationSpeed
_stepTimer += Time.deltaTime

while _stepTimer >= interval:
    SimulationService.Step()   // evaluate Conway rules for every cell
    GridView.Render(_grid)     // recolor all UI Image components
    _stepTimer -= interval
```

This ensures that no steps are dropped on slow frames, and that multiple steps fire correctly on fast configurations.

---

## Initialization Modes

| Mode | Behaviour |
|------|-----------|
| **Random** | Every cell is independently set alive with probability `RandomFillDensity` |
| **Pattern** | Grid starts empty, then two gliders and one blinker are stamped in at fixed positions |
| **Mixed** | Random fill is applied first, then the pattern overlay is stamped on top |

### Seeded Patterns

**Glider** — classic 5-cell spaceship (period 4, travels diagonally):
```
. X .
. . X
X X X
```
Two gliders are placed: one at the top-left corner, one at the bottom-right corner.

**Blinker** — simplest period-2 oscillator (3 horizontal cells):
```
X X X
```
One blinker is placed at the grid centre.

---

## Configuration (ScriptableObject) (GameOfLifeConfig)

Create configuration assets via `Assets → Create → ConwayLife → GameOfLifeConfig`.

| Field | Type | Description |
|-------|------|-------------|
| `GridWidth` | `int` | Number of grid columns |
| `GridHeight` | `int` | Number of grid rows |
| `SimulationSpeed` | `float` | Initial steps per second |
| `RandomFillDensity` | `float` (0–1) | Probability each cell starts alive |
| `AliveColor` | `Color` | Color for alive cells |
| `DeadColor` | `Color` | Color for dead cells |
| `InitializationMode` | `enum` | `Random`, `Pattern`, or `Mixed` |

Multiple config assets can be loaded and switched at runtime via the config dropdown.

---

## UI Controls

| Control | Type | Action |
|---------|------|--------|
| **Play / Pause** | Button | Toggles the simulation loop on/off |
| **Speed** | Slider | Sets steps per second |
| **Cell Size** | Slider | Resizes all cell visuals and rebuilds the grid (default: 20 px) |
| **Density** | Slider | Sets the random fill probability (0–1); applies on next Randomize |
| **Randomize** | Button | Re-seeds the grid with the current density; disabled in Pattern mode |
| **Config** | Dropdown | Switches the active `GameOfLifeConfig` scritable object asset and rebuilds the grid |
| **Generation** | Label | Live display of the current generation number |
| **Alive** | Label | Live display of the current alive cell count |

---

## Version History

| Tag | Description |
|-----|-------------|
| `v1.0.0` | Initial release — core simulation, pattern seeding, config system, UI |
| `v1.1.0` | UI-driven cell size and density controls |
