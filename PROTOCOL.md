# AI Coding Protocol

## Overview

This document defines the coding standards, best practices, and communication protocols for AI-assisted development of the Undead Survivor Unity project.

## Unity Version

- **Current Version**: Unity 6.3 (LTS)
- **Scripting Backend**: IL2CPP
- **API Compatibility Level**: .NET Standard 2.1

## Code Style Guidelines

### Naming Conventions

```csharp
// Classes: PascalCase
public class InventoryManager : MonoBehaviour { }

// Methods: PascalCase
public void AddItem(InventoryItemData item) { }

// Properties: PascalCase
public static InventoryManager Instance { get; private set; }

// Fields: camelCase (private), PascalCase (public)
private int currentSlot;
public int MaxSlots = 20;

// Constants: UPPER_SNAKE_CASE or PascalCase
public const int MAX_INVENTORY_SIZE = 100;

// Enums: PascalCase (type), PascalCase (values)
public enum ItemCategory { Weapon, Armor, Consumable }
```

### Unity-Specific Conventions

```csharp
// MonoBehaviour lifecycle methods
private void Awake() { }
private void Start() { }
private void Update() { }
private void OnEnable() { }
private void OnDisable() { }
private void OnDestroy() { }

// Unity 6.3 API - Use new find methods
// ✅ Correct
Player player = FindFirstObjectByType<Player>();
Player[] players = FindObjectsByType<Player>();

// ❌ Deprecated (Unity 6.3+)
Player player = FindObjectOfType<Player>();
Player[] players = FindObjectsOfType<Player>();
```

### Header Attributes

```csharp
// ✅ Correct - Header on fields only
[Header("Inventory Data")]
public PlayerInventory playerInventory;

// ❌ Incorrect - Header on events
[Header("Events")]  // This causes CS0592 error
public event Action OnItemCollected;
```

## Architecture Patterns

### Singleton Pattern

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
```

### ScriptableObject Pattern

```csharp
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite icon;
}
```

### Event-Driven Architecture

```csharp
// Publisher
public class Inventory : ScriptableObject
{
    public event Action OnInventoryChanged;
    public event Action<InventorySlot> OnSlotChanged;
    
    public void AddItem(ItemData item)
    {
        // ... add logic
        OnInventoryChanged?.Invoke();
    }
}

// Subscriber
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    
    private void OnEnable()
    {
        inventory.OnInventoryChanged += RefreshUI;
    }
    
    private void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshUI;
    }
}
```

## File Organization

```
Assets/
├── Codes/
│   ├── Core/           # Singletons, Managers
│   ├── Inventory/      # Inventory system
│   ├── Player/         # Player-related scripts
│   ├── Enemy/          # Enemy-related scripts
│   ├── UI/             # UI scripts
│   └── Utils/          # Utility classes
├── Resources/
│   └── Data/           # ScriptableObjects
├── Prefabs/
│   ├── UI/             # UI prefabs
│   └── Gameplay/       # Gameplay prefabs
└── Scenes/
    ├── GameScene.unity
    └── LobbyScene.unity
```

## Communication Protocol

### When Starting a Task

1. **Read existing code** - Always check related files first
2. **Understand context** - Review project structure and patterns
3. **Ask clarifying questions** - If requirements are unclear

### During Development

1. **Follow existing patterns** - Match the project's coding style
2. **Use Unity 6.3 APIs** - Avoid deprecated methods
3. **Handle errors gracefully** - Provide meaningful error messages
4. **Document complex logic** - Add comments for non-obvious code

### Before Completion

1. **Test compilation** - Ensure no CS errors
2. **Check for warnings** - Resolve CS0618 (deprecated) warnings
3. **Verify null checks** - Prevent NullReferenceException
4. **Review event subscriptions** - Prevent memory leaks

## Common Issues & Solutions

### CS0592: Header on Events
```csharp
// ❌ Wrong
[Header("Events")]
public event Action OnEvent;

// ✅ Correct
// Events
public event Action OnEvent;
```

### CS0618: Deprecated API
```csharp
// ❌ Deprecated
FindObjectOfType<Player>();

// ✅ Unity 6.3+
FindFirstObjectByType<Player>();
```

### CS0414: Unused Field
```csharp
// ❌ Warning
private int unusedField = 0;

// ✅ Use the field or remove it
private int usedField = 0;
public void Method() { usedField++; }
```

## Language Preference

- **Primary**: English for all code, comments, and documentation
- **User Communication**: Korean (as requested by user)

## Code Review Checklist

- [ ] Follows naming conventions
- [ ] Uses Unity 6.3 APIs
- [ ] No deprecated method warnings
- [ ] Proper null checking
- [ ] Event subscriptions properly managed
- [ ] Comments for complex logic
- [ ] Consistent with existing codebase
