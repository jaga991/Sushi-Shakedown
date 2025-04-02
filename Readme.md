# Sushi-Shakedown Game Reference 

## Guru's Corner

### Features Checklist
- [x] NPC Customers moving around (similar to Pixel Cafe)
- [ ] Customers requesting dishes
    - [ ] Basic dish assets (default style)
    - [ ] Request display system
- [ ] Customer patience system
    - [ ] Timer implementation
    - [ ] Visual indicator (text field above customer)
- [ ] Customers leaving when patience timer expires
- [ ] Customer states (entering, waiting, satisfied, angry)
- [ ] Simple sound effects for key events
- [ ] Basic scoring system
- [ ] Game over condition
- [ ] Customer variety (different sprites or colors)
- [ ] Tutorial/instructions screen


### 🧾 **Checkpoint 2 priority**

- [ ] Convert text bubble from `...` to show food order when customer reaches their `OrderArea`.
- [ ] Implement a **patience timer**:
  - [ ] Customer leaves when timer runs out.
  - [ ] Score is reduced when customer leaves angrily.
- [ ] Add a **BoxCollider2D** to the food bubble (for interaction detection).
- [ ] Implement a **data structure** for food orders (e.g., ScriptableObject or class with food type, ID, sprite).
- [ ] Implement **drag-and-drop** for food items:
  - [ ] Drag food from prep area.
  - [ ] Drop onto customer or their order bubble.
- [ ] Add **animation/sprite change for customer mood**:
  - [ ] Happy when given correct food.
  - [ ] Angry if time runs out or wrong food is given.
  - [ ] (Can spoof with `OnClick()` trigger for testing).

---

Let me know if you want these tied to specific files, scenes, or given deadlines for task tracking!

### Code References 

Here’s your **updated README documentation** based on the most recent changes in functionality across your files:

---

### Code References

#### Files

---

### NPCController.cs
- **Purpose**: Controls NPC movement and behavior.
- **Key Points**:
  - Moves NPCs at a set speed and flips sprite based on direction.
  - Destroys NPC when it goes off-screen.
  - Aligns the bottom of the sprite to a given Y-coordinate.
- **Usage**: Attach to your NPC prefab (requires a SpriteRenderer). No major changes.

---

### SpawnZoneData.cs
- **Purpose**: Defines vertical boundaries for NPC and Customer spawns.
- **Key Points**:
  - Uses a BoxCollider2D to calculate minY and maxY at runtime.
- **Usage**: Attach to a spawn zone object (requires a BoxCollider2D). No changes in logic.

---

### NPCSpawner.cs
- **Purpose**: Spawns both NPCs and Customers into the scene.
- **Key Points**:
  - Spawns NPCs at regular intervals.
  - Spawns Customers manually via the `spawnCustomerDebug` flag.
  - Spawns Customers only if a free `OrderArea` is available (managed by `OrderAreaGroup`).
  - Customer spawn and NPC spawn logic are separated into `SpawnCustomer()` and `SpawnNPC()`.
- **New Dependency**: Requires an `OrderAreaGroup` reference to manage available order spots.
- **Usage**:
  - Attach to a spawner object.
  - Assign references to `npcTemplate`, `customerTemplate`, `spawnZone`, and `orderAreaGroup`.
  - Toggle `spawnCustomerDebug` to manually spawn a customer.

---

### CustomerController.cs
- **Purpose**: Controls Customer movement, target assignment, and simple text bubble display.
- **Key Points**:
  - Moves toward an assigned `OrderArea` instead of a hardcoded coordinate.
  - Text bubble displays "..." while walking and "Order" when arrived.
  - Logs and updates the assigned `OrderArea` upon arrival.
- **Usage**:
  - Attach to a customer prefab.
  - Include a child GameObject with a speech bubble (with a Text component).
  - Use `SetOrderArea()` from the spawner to assign a target.

---

### OrderArea.cs
- **Purpose**: Represents a single spot at the counter where a Customer can order.
- **Key Points**:
  - Tracks if the spot is currently occupied.
  - Provides coordinates for customer navigation.
  - Contains a trigger event for customer presence.
- **Usage**:
  - Attach to counter spot GameObjects.
  - Each should have a Collider2D set to trigger.
  - Called by the spawner and customer to set availability.

---

### OrderAreaGroup.cs
- **Purpose**: Manages multiple `OrderArea` components as a pool.
- **Key Points**:
  - Provides a method to fetch the first free order spot.
  - Can reset/release an occupied spot later via `ReleaseOrderArea()`.
- **Usage**:
  - Attach to a GameObject containing multiple child `OrderArea` objects.
  - Automatically collects them on Awake().
  - Used by `NPCSpawner` to assign a valid order spot to a new customer.

---

Let me know if you’d like a diagram or prefab structure guide next!

#### Audio Assets 
Audio when customer reaches counter
Audio when customer recieves order (very good , good , default , bad)
Audo when customer times out 


