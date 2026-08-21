# Design Document / Project Brief

**Project Title:** RoboFleet — Warehouse Automation Simulator
**Author:** Yury M.
**Pathway:** Unity Junior Programmer — Apply Object-Oriented Principles
**Submission:** Programming Theory in Action
**Version:** 1.0

---

## 1. High-Level Concept

**RoboFleet** is a small prototype that simulates a fleet of delivery robots working in an automated warehouse. A dispatcher (the `GameManager`) issues high-level commands, and different robot types execute those commands in their own unique ways.

The goal of this prototype is **not visual polish**, but a clean object-oriented architecture that demonstrates all four OOP pillars: **Inheritance, Polymorphism, Encapsulation, and Abstraction**.

### Scope & Visual Style
- Fully prototype-grade, using primitive shapes only:
  - Floor → `Plane`
  - Walls / obstacles → `Cubes`
  - Packages → `Spheres`
  - Robots → colored `Cubes` / `Cylinders`
- Assets from *Create with Code* prototypes (e.g., the player cube from the Jumper prototype) may be repurposed where convenient.
- A console/text-based version is acceptable: robot behavior can be reported via `Debug.Log` in the Unity Console.

---

## 2. Pillar 1 — Inheritance (Choosing the Objects)

**Idea:** All robots on the warehouse floor share common traits (speed, battery, serial number), while specialized models derive from the generic robot and extend it with unique capabilities.

### Class Hierarchy

- **Base (parent) class:** `Robot`
- **Derived (child) classes:**
  - `WheeledBot` — ground robot; moves along the X/Z plane and must steer around obstacles.
  - `FlyingDrone` — aerial unit; moves in full 3D space and flies over obstacles in a straight line.
  - `HeavyLoader` — slow ground unit; can carry multiple packages at once.

```
                 +----------------------+
                 |   Robot (abstract)   |
                 +----------------------+
                            ^
          +-----------------+-----------------+
          |                 |                 |
  +--------------+  +---------------+  +--------------+
  |  WheeledBot  |  |  FlyingDrone  |  | HeavyLoader  |
  +--------------+  +---------------+  +--------------+
```

### Prototype Code

```csharp
// Robot.cs — abstract base (parent) class
public abstract class Robot : MonoBehaviour
{
    protected float speed;
    protected float batteryLevel;
    protected string serialNumber;
}

// Child classes derive shared state and behavior from Robot
public class WheeledBot  : Robot { }
public class FlyingDrone : Robot { }
public class HeavyLoader : Robot { }
```

---

## 3. Pillar 2 — Polymorphism (Choosing the Behaviors)

### Method Overriding (runtime polymorphism)

All robots respond to the same commands, but the behavior "morphs" depending on the subtype:

- `MoveToTarget(Vector3 destination)`
  - `WheeledBot`: moves along X and Z only, keeps Y constant, steers around walls.
  - `FlyingDrone`: moves along X, Y, and Z; ascends to fly over obstacles.
  - `HeavyLoader`: ground movement at reduced speed while carrying cargo.
- `DeliverPackage(Vector3 dropOffPoint)` — each subtype performs the delivery routine in its own way.

### Method Overloading ("many forms")

- `Pickup()` — picks up the nearest available package.
- `Pickup(int maxWeight)` — picks up the nearest package only if its weight is ≤ `maxWeight`.
- `Pickup(GameObject package)` — attempts to pick up one specific package.

Because of polymorphism, the `GameManager` can store a `List<Robot>` and call the **same** method on every unit — each subtype responds with its **own** behavior.

### Prototype Code

```csharp
public abstract class Robot : MonoBehaviour
{
    // Overriding: each subtype provides its own movement
    public abstract void MoveToTarget(Vector3 destination);

    // Overloading: same name, different parameter lists
    public bool Pickup() { /* nearest package */ return true; }
    public bool Pickup(int maxWeight) { /* nearest package with weight <= maxWeight */ return true; }
    public bool Pickup(GameObject package) { /* one specific package */ return true; }
}

public class WheeledBot : Robot
{
    public override void MoveToTarget(Vector3 destination)
    {
        // Ground-only movement: flatten Y, steer around walls
        destination.y = transform.position.y;
        transform.position = Vector3.MoveTowards(
            transform.position, destination, speed * Time.deltaTime);
    }
}

public class FlyingDrone : Robot
{
    public override void MoveToTarget(Vector3 destination)
    {
        // Full 3D movement: straight line, flying over obstacles
        transform.position = Vector3.MoveTowards(
            transform.position, destination, speed * Time.deltaTime);
    }
}
```

---

## 4. Pillar 3 — Encapsulation (Choosing the Data)

Critical robot state is hidden behind properties and validated methods so that external code can never corrupt it.

| Data | Exposure | Validation Rule |
|---|---|---|
| `_batteryLevel` | public property `BatteryLevel` (get / set) | clamped to 0–100; robot sleeps automatically at 0 |
| `_currentCargo` | private field; changed only via `AddCargo()` | can never exceed `MaxCapacity` |
| `_serialNumber` | public property `SerialNumber` (get only) | immutable after `Initialize()` |
| `speed` | protected field; read-only property for outsiders | subclasses may tune it, external code may not |

### Prototype Code

```csharp
public abstract class Robot : MonoBehaviour
{
    [SerializeField] private float _batteryLevel = 100f;
    [SerializeField] private int _currentCargo = 0;

    public float MaxCapacity { get; protected set; } = 1;
    public string SerialNumber { get; private set; }

    public void Initialize(string serial)
    {
        SerialNumber = serial; // set once, never changed again
    }

    // Battery is protected from invalid values
    public float BatteryLevel
    {
        get => _batteryLevel;
        set
        {
            _batteryLevel = Mathf.Clamp(value, 0f, 100f);
            if (_batteryLevel <= 0f) EnterSleepMode();
        }
    }

    // Cargo can only change through validated methods
    public bool AddCargo(int amount)
    {
        if (_currentCargo + amount > MaxCapacity)
        {
            Debug.LogWarning($"{SerialNumber}: cargo capacity exceeded!");
            return false;
        }
        _currentCargo += amount;
        return true;
    }
}
```

---

## 5. Pillar 4 — Abstraction (Choosing the Functions)

The `Robot` base class exposes a simple, high-level API. The `GameManager` (dispatcher) never needs to know about raycasts, physics, or battery math — it just issues commands:

- `StartShift()` — powers the robot on and resets its battery.
- `DeliverPackage(Vector3 dropOffPoint)` — abstract command; the full "go → pick up → drop off" routine is hidden inside each subtype.
- `EnterSleepMode()` — powers the robot off.

### Prototype Code

```csharp
public abstract class Robot : MonoBehaviour
{
    // High-level command: the "how" is hidden inside each subtype
    public abstract void DeliverPackage(Vector3 dropOffPoint);

    public void StartShift()
    {
        BatteryLevel = 100f;
        gameObject.SetActive(true);
        Debug.Log($"{SerialNumber} started its shift.");
    }

    public void EnterSleepMode()
    {
        gameObject.SetActive(false);
        Debug.Log($"{SerialNumber} entered sleep mode.");
    }
}

// GameManager.cs — interacts with robots ONLY through the high-level API
public class GameManager : MonoBehaviour
{
    private List<Robot> fleet = new List<Robot>();

    void Start()
    {
        // ... instantiate/spawn robots and add them to the fleet ...

        foreach (Robot robot in fleet)
        {
            robot.StartShift();
            robot.DeliverPackage(new Vector3(10f, 0f, 10f));
        }
        // WheeledBot drives around the walls, FlyingDrone flies straight
        // over them — same call, different behavior.
    }
}
```

---

## 6. Implementation Plan in Unity (Prototype Steps)

1. **Scene setup** — add a `Plane` floor, a few `Cube` walls, and several `Sphere` packages.
2. **Scripts** — create `Robot.cs` (abstract base), `WheeledBot.cs`, `FlyingDrone.cs`, `HeavyLoader.cs`, and `GameManager.cs`.
3. **Spawning** — the `GameManager` builds a `List<Robot>` in `Start()` (via prefab `Instantiate` for a visual version, or plain objects for a console-only version).
4. **Dispatching** — call `StartShift()` and `DeliverPackage(...)` on each robot and observe the polymorphic behavior in the Scene view / Console.
5. **Optional polish** — add `Rigidbody` components and move via `MovePosition()`; color-code robots by type; log battery levels to the Console.

---

## 7. Submission Checklist

- [x] **Inheritance** — `Robot` → `WheeledBot` / `FlyingDrone` / `HeavyLoader`
- [x] **Polymorphism** — overridden `MoveToTarget()` / `DeliverPackage()`; overloaded `Pickup()`
- [x] **Encapsulation** — private fields with validated properties (battery, cargo, serial number)
- [x] **Abstraction** — high-level API (`StartShift`, `DeliverPackage`, `EnterSleepMode`) hides implementation details
- [x] **Prototype scope** — primitive shapes / console output only, no art assets required
