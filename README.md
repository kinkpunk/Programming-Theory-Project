# 🤖 RoboFleet: Warehouse Automation Simulator

A prototype of an automated warehouse simulator developed as part of the **Unity Junior Programmer** pathway (module: *Apply Object-Oriented Principles*). 

The main goal of this project is not visual polish, but a clean code architecture that clearly demonstrates the application of the four pillars of Object-Oriented Programming (OOP) in a practical game context.

---

## 🏛 OOP Implementation

### 1. Inheritance
A clear class hierarchy was created: an abstract base class `Robot` and three specialized child classes:
* `WheeledBot` (ground robot)
* `FlyingDrone` (aerial drone)
* `HeavyLoader` (heavy cargo loader)

### 2. Polymorphism
* **Overriding:** Each robot type overrides the `MoveTo()` method to implement its own movement logic (the drone flies in 3D space, while ground bots move along the XZ plane and avoid obstacles).
* **Overloading:** The package pickup method `Pickup()` has multiple forms (e.g., picking up the nearest package vs. a specific target).

### 3. Encapsulation
Critical data is protected from incorrect external modification. For example, the delivered packages counter (`TotalDelivered`) and the robot's cargo capacity (`MaxCapacity`) are read-only or can only be modified through validating methods.

### 4. Abstraction
Complex logic such as pathfinding, obstacle avoidance (via Raycasting), and routing is hidden inside the classes. The Game Manager interacts with the robots solely through the high-level `StartDelivery()` method, without needing to know the underlying implementation details.

---

## 🎮 Gameplay Features

* **Automation:** Packages (Stars) spawn randomly across the warehouse floor.
* **Smart Navigation:** Ground robots use Raycasts to detect and navigate around obstacles. Drones ignore ground obstacles and fly over them.
* **Collision Handling:** Robots correctly stop at the warehouse drop-off zone without clipping through its collider.
* **Score System:** A real-time UI counter displays the total number of packages successfully delivered to the warehouse.

---

##  How to Run the Project

1. Open the project in **Unity** (Unity 2022.3 LTS or newer is recommended).
2. Open the main scene.
3. Ensure all robot prefabs have a `Rigidbody` (with **Is Kinematic** checked) and a `Collider` attached.
4. Ensure all obstacle objects are assigned to the **`Obstacles`** layer.
5. Hit **Play**!

---

## 📂 Script Structure

* `Robot.cs` — Abstract base class containing shared logic and properties.
* `WheeledBot.cs`, `FlyingDrone.cs`, `HeavyLoader.cs` — Concrete implementations of the robots.
* `Package.cs`, `PackageSpawner.cs` — Logic for the collectible items and their random spawning.
* `Warehouse.cs` — The drop-off zone that processes and destroys delivered packages.
* `ScoreUI.cs` — Updates the on-screen UI counter.
* `GameManager.cs` — Initializes the scene and starts the simulation loop.

---
*Developed for educational purposes. Assets: Unity Primitives & Standard Assets.*
