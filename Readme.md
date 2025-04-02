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

### NPCSpawner.cs
- **Purpose**: Spawns NPCs at regular intervals.
- **Key Points**:
  - Randomly spawns NPCs from either left or right.
  - Sets NPC movement direction and vertical position using SpawnZoneData.
- **Usage**: Attach to a spawner object and set the npcTemplate and SpawnZoneData reference.

### SpawnZoneData.cs
- **Purpose**: Defines vertical boundaries for NPC spawns.
- **Key Points**:
  - Uses a BoxCollider2D to calculate minY and maxY.
- **Usage**: Attach to a spawn zone object (requires a BoxCollider2D).