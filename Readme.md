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

### Code References 

#### Files

### NPCController.cs
- **Purpose**: Controls NPC movement and behavior.
- **Key Points**:
  - Moves NPCs at a set speed and flips sprite based on direction.
  - Destroys NPC when it goes off-screen.
  - Aligns the bottom of the sprite to a given Y-coordinate.
- **Usage**: Attach to your NPC prefab (requires a SpriteRenderer).

### SpawnZoneData.cs
- **Purpose**: Defines vertical boundaries for NPC spawns.
- **Key Points**:
  - Uses a BoxCollider2D to calculate minY and maxY.
- **Usage**: Attach to a spawn zone object (requires a BoxCollider2D).

### NPCSpawner.cs
- **Purpose**: Spawns both NPCs and Customers into the scene.
- **Key Points**:
  - Spawns NPCs at regular intervals.
  - Optionally spawns Customers manually using the `spawnCustomerDebug` flag.
  - Both NPCs and Customers are spawned from the left or right of the screen, with randomized vertical spawn positions within a spawn zone.
  - Customer spawn logic is separated from NPC logic into its own function.
- **Usage**: 
  - Attach to a spawner object.
  - Assign both `npcTemplate` and `customerTemplate` GameObjects.
  - Assign a `SpawnZoneData` component to define spawn height limits.
  - Toggle `spawnCustomerDebug` in Inspector to manually trigger a customer spawn during runtime.

### CustomerController.cs
- **Purpose**: Controls Customer movement and interaction with the counter.
- **Key Points**:
  - Moves toward a predefined counter position.
  - Displays a text bubble with "..." while walking.
  - Stops movement and disables itself upon reaching the counter.
- **Usage**: 
  - Attach to your customer prefab.
  - Include a child GameObject with a speech bubble (e.g., world-space canvas + Text).
  - Assign the bubble via the `textBubble` field.
  - Called from the spawner with `SetTargetCounterPosition()` to direct customers to a counter.



#### Audio Assets 
Audio when customer reaches counter
Audio when customer recieves order (very good , good , default , bad)
Audo when customer times out 
