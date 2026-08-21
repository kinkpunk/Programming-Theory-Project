# Design Document / Project Brief

**Project Title:** RoboFleet — Warehouse Automation Simulator
**Author:** Yury M.
**Pathway:** Unity Junior Programmer — Apply Object-Oriented Principles
**Submission:** Programming Theory in Action
**Version:** 2.0

---

## 1. High-Level Concept

**RoboFleet** is a small prototype simulating a fleet of delivery robots in an automated warehouse. Star-shaped packages spawn at random locations on the warehouse floor, and the robots autonomously find the nearest package, pick it up, and deliver it to the central **Warehouse** drop-off point.

The goal of this prototype is **not visual polish**, but a clean object-oriented architecture that demonstrates all four OOP pillars: **Inheritance, Polymorphism, Encapsulation, and Abstraction**.

### Scope & Visual Style
- Prototype-grade visuals using primitive shapes:
  - Floor → `Plane`
  - Packages → `Star` prefabs (or cubes with a `TrailRenderer` for flair)
  - Robots → colored `Cubes` / `Cylinders`
  - Warehouse → a marked zone on the floor
- A text/console log is used to report robot decisions and actions.

---

## 2. Pillar 1 — Inheritance (Choosing the Objects)

All robots share a common base, but each subtype has unique movement and cargo behavior.

### Class Hierarchy

- **Base (parent) class:** `Robot`
- **Derived (child) classes:**
  - `WheeledBot` — ground robot; moves along X/Z plane only.
  - `FlyingDrone` — aerial unit; moves in full 3D space, flies over obstacles.
  - `HeavyLoader` — slow ground unit; can carry multiple packages at once.

### Other Objects
- `Package` — a collectible item with weight and position.
- `PackageSpawner` — periodically spawns packages at random positions on the floor.
- `Warehouse` — the drop-off zone that receives packages.
- `GameManager` — the dispatcher that coordinates the fleet.

---

## 3. Pillar 2 — Polymorphism (Choosing the Behaviors)

### Method Overriding (runtime polymorphism)
All robots respond to the same high-level commands, but the behavior "morphs" depending on the subtype:

- `MoveTo(Vector3 destination)`
  - `WheeledBot`: moves along X and Z only, keeps Y constant.
  - `FlyingDrone`: moves along X, Y, and Z; ascends to fly over obstacles.
  - `HeavyLoader`: ground movement at reduced speed while carrying cargo.
- `DeliverNearestPackage()` — each subtype finds and transports a package using its own movement logic.

### Method Overloading ("many forms")
- `Pickup()` — picks up the nearest available package.
- `Pickup(Package target)` — picks up a specific package.

---

## 4. Pillar 3 — Encapsulation (Choosing the Data)

Critical robot state is hidden behind properties and validated methods:

| Data | Exposure | Validation Rule |
|---|---|---|
| `_batteryLevel` | public property `BatteryLevel` | clamped to 0–100; robot sleeps at 0 |
| `_currentCargo` | private field; changed only via `AddCargo()` | can never exceed `MaxCapacity` |
| `_serialNumber` | public property `SerialNumber` (get only) | immutable after `Initialize()` |
| `speed` | protected field; read-only property for outsiders | subclasses tune it, external code cannot |

---

## 5. Pillar 4 — Abstraction (Choosing the Functions)

The `Robot` base class exposes a simple, high-level API. The `GameManager` never needs to know about raycasts, physics, or battery math — it just issues commands:

- `StartShift()` — powers the robot on and resets its battery.
- `FindAndDeliverPackage()` — abstract command; the full "find → go → pick up → go to warehouse → drop off" routine is hidden inside each subtype.
- `EnterSleepMode()` — powers the robot off.

---

## 6. Implementation Plan in Unity

1. **Scene setup** — add a `Plane` floor, a `Warehouse` zone at the center, and several robot prefabs around the edges.
2. **Scripts** — create `Robot.cs` (abstract base), `WheeledBot.cs`, `FlyingDrone.cs`, `HeavyLoader.cs`, `Package.cs`, `PackageSpawner.cs`, `Warehouse.cs`, and `GameManager.cs`.
3. **Spawning** — `PackageSpawner` creates star packages at random positions on the Plane every few seconds.
4. **Dispatching** — `GameManager` calls `FindAndDeliverPackage()` on each robot; each robot independently finds the nearest package and delivers it using its own movement style.
5. **Scoring** — `Warehouse` tracks how many packages have been delivered and logs the total.

---

## 7. Submission Checklist

- [x] **Inheritance** — `Robot` → `WheeledBot` / `FlyingDrone` / `HeavyLoader`
- [x] **Polymorphism** — overridden `MoveTo()` / `FindAndDeliverPackage()`; overloaded `Pickup()`
- [x] **Encapsulation** — private fields with validated properties (battery, cargo, serial number)
- [x] **Abstraction** — high-level API (`StartShift`, `FindAndDeliverPackage`, `EnterSleepMode`) hides implementation details
- [x] **Prototype scope** — primitive shapes / console output only, no art assets required
