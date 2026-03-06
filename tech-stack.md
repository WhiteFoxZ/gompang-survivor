# Tech Stack - Undead Survivor

## Engine & Platform

| Component | Version | Notes |
|-----------|---------|-------|
| Unity | 6.3 LTS | Primary game engine |
| Scripting Backend | IL2CPP | Release builds |
| API Compatibility | .NET Standard 2.1 | |
| Target Platform | Android, PC | Primary targets |

## Unity Packages

### Core Packages
```json
{
  "com.unity.inputsystem": "1.8.1",
  "com.unity.render-pipelines.universal": "16.0.5",
  "com.unity.textmeshpro": "3.0.6"
}
```

### Input System
- **Package**: Input System (com.unity.inputsystem)
- **Version**: 1.8.1
- **Usage**: Player movement, combat controls, UI navigation
- **Configuration**: `Assets/Codes/PlayerAction.inputactions`

### Rendering
- **Pipeline**: Universal Render Pipeline (URP)
- **Version**: 16.0.5
- **2D Renderer**: Renderer2D
- **Features**: 2D lights, shadows, post-processing

## Architecture Patterns

### Design Patterns Used

1. **Singleton Pattern**
   - GameManager
   - AudioManager
   - InventoryManager
   - PoolManager

2. **ScriptableObject Pattern**
   - ItemData
   - InventoryItemData
   - PlayerInventory
   - Audio configurations

3. **Observer Pattern**
   - C# Events for decoupled communication
   - Inventory.OnInventoryChanged
   - GameManager state changes

4. **Object Pooling**
   - Bullet pooling
   - Enemy pooling
   - Particle effect pooling

### Data Flow

```
Player Input → Input System → Player Controller
                                    ↓
Combat Action → Weapon System → Pool Manager → Spawn Objects
                                    ↓
Item Pickup → Inventory Manager → Player Inventory (SO)
                                    ↓
Inventory UI ← Event System ← Inventory Changes
```

## Code Structure

### Namespace Organization
```csharp
// No explicit namespaces - flat structure for simplicity
// Classes organized by folder
```

### Folder Structure
```
Assets/
├── Codes/
│   ├── Core/           # GameManager, SceneManager
│   ├── Inventory/      # Inventory system (new)
│   ├── Player/         # Player, Character
│   ├── Combat/         # Weapon, Bullet, Hand
│   ├── Enemy/          # Enemy, Spawner, Scanner
│   ├── UI/             # HUD, LevelUp, Result
│   └── Utils/          # PoolManager, AudioManager
├── Resources/
│   └── Data/           # ScriptableObject assets
├── Prefabs/
├── Scenes/
└── Settings/
```

## Key Systems

### Inventory System

| Component | Type | Purpose |
|-----------|------|---------|
| InventoryItemData | ScriptableObject | Item definition |
| InventorySlot | Serializable class | Slot state |
| PlayerInventory | ScriptableObject | Player's inventory data |
| InventoryManager | MonoBehaviour | Runtime management |
| InventoryUI | MonoBehaviour | UI controller |
| InventorySlotUI | MonoBehaviour | Individual slot UI |
| ItemPickup | MonoBehaviour | World item pickup |

### Event System

```csharp
// Inventory events
public event Action OnInventoryChanged;
public event Action<InventorySlot> OnSlotChanged;

// Game events
public event Action<InventoryItemData> OnItemCollected;
public event Action<InventoryItemData> OnItemUsed;
```

### Save System (Planned)

| Aspect | Approach |
|--------|----------|
| Format | JSON |
| Location | Application.persistentDataPath |
| Encryption | Optional (AES) |
| Auto-save | On level complete, app pause |

## Performance Considerations

### Object Pooling
- Bullets: 50-100 instances
- Enemies: 20-30 instances
- Particles: Pool per effect type

### Optimization Techniques
- Sprite atlasing for UI
- Object culling off-screen enemies
- Lazy initialization for UI
- Coroutines for time-based operations

### Memory Management
- Dispose of events OnDisable
- Pool instead of instantiate/destroy
- Use structs where appropriate
- Avoid LINQ in Update loops

## Build Configuration

### Android
```
Minimum API Level: 24 (Android 7.0)
Target API Level: 34 (Android 14)
Scripting Backend: IL2CPP
Target Architectures: ARM64
```

### PC
```
Target Platform: Windows, Mac, Linux
Architecture: x86_64
```

## Version Control

### Git Configuration
- `.gitignore`: Unity-specific ignores
- Large files: Git LFS for assets
- Branches: main, develop, feature/*

### Important .gitignore Entries
```
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
```

## Development Tools

### IDE
- **Primary**: Visual Studio 2022 / VS Code
- **Extensions**: Unity Tools, C# Dev Kit

### Debugging
- Unity Profiler
- Frame Debugger
- Memory Profiler

### Testing
- Unity Test Framework (planned)
- Manual playtesting
- Device testing (Android)

## Third-Party Assets

### Currently Used
- Undead Survivor Asset Pack (Sprites, Audio)
- Neodgm Font (Korean)

### Potential Additions
- DOTween (animation)
- TextMeshPro (already included)

## Coding Standards

See [PROTOCOL.md](./PROTOCOL.md) for detailed coding standards.

### Quick Reference
- Unity 6.3 APIs only
- PascalCase for public members
- camelCase for private members
- No `FindObjectOfType` - use `FindFirstObjectByType`
- Proper null checking with `?.` operator
- Event unsubscription in OnDisable

## Documentation

### Code Documentation
- XML comments for public APIs
- Inline comments for complex logic
- README files for complex systems

### External Documentation
- PROTOCOL.md - Coding standards
- ACTION_PLAN.md - Development roadmap
- PRD.md - Product requirements
- This file - Technical specifications
