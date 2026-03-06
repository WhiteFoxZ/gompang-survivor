# Action Plan - Undead Survivor

## Project Overview

A 2D top-down survival action game inspired by Vampire Survivors, built with Unity 6.3.

## Current Status

### Completed Features ✅

1. **Core Game Systems**
   - Game Manager (Singleton)
   - Player movement and controls
   - Enemy spawning and AI
   - Combat system (melee & ranged)
   - Level up system
   - Audio management

2. **Inventory System** (Newly Implemented)
   - ScriptableObject-based item data
   - Inventory slot management
   - Player inventory (SO)
   - Inventory manager (Singleton)
   - UI with drag & drop
   - Item pickup system
   - Test tools

### In Progress 🚧

- [ ] UI/UX Polish
- [ ] Save/Load system
- [ ] Additional weapon types

## Phase 1: Foundation (Completed)

### Week 1-2: Core Mechanics
- [x] Player controller
- [x] Basic enemy AI
- [x] Weapon system
- [x] Health/damage system
- [x] Game loop

### Week 3: Systems Integration
- [x] Game manager
- [x] Audio manager
- [x] Pool manager
- [x] Scene management

## Phase 2: Content Expansion (Current)

### Week 4: Inventory System ✅
- [x] Design item data structure
- [x] Implement inventory backend
- [x] Create UI system
- [x] Add item pickup mechanics
- [x] Testing & debugging

### Week 5: Save System
- [ ] Design save data structure
- [ ] Implement JSON serialization
- [ ] Player progress persistence
- [ ] Settings save/load
- [ ] Cloud save integration (optional)

### Week 6: Equipment System
- [ ] Equipment slots (weapon, armor)
- [ ] Stat modifiers
- [ ] Equipment UI
- [ ] Equipment effects

## Phase 3: Content & Polish

### Week 7-8: New Content
- [ ] Additional enemy types
- [ ] Boss encounters
- [ ] New weapons and abilities
- [ ] Power-ups and items

### Week 9: Progression
- [ ] Achievement system
- [ ] Unlockable characters
- [ ] Upgrade system
- [ ] Meta-progression

### Week 10: UI/UX
- [ ] Main menu redesign
- [ ] HUD improvements
- [ ] Settings menu
- [ ] Tutorial system

## Phase 4: Optimization & Release Prep

### Week 11: Performance
- [ ] Profile and optimize
- [ ] Memory management
- [ ] Mobile optimization
- [ ] Loading time reduction

### Week 12: Polish
- [ ] Visual effects
- [ ] Sound effects
- [ ] Bug fixes
- [ ] Balance adjustments

## Immediate Next Steps

### Priority 1: Save System
```
Estimated Time: 3-4 days
Dependencies: Inventory system ✅
```

1. Create SaveData ScriptableObject
2. Implement SaveManager
3. JSON serialization
4. Auto-save functionality
5. Load game flow

### Priority 2: Equipment System
```
Estimated Time: 4-5 days
Dependencies: Inventory system ✅
```

1. Equipment slot definitions
2. Equipment UI
3. Stat calculation
4. Visual updates on equip

### Priority 3: Shop System
```
Estimated Time: 3-4 days
Dependencies: Inventory, Save system
```

1. Currency system
2. Shop UI
3. Item purchasing
4. Price balancing

## Technical Debt

### To Address
- [ ] Refactor enemy AI for better performance
- [ ] Optimize object pooling
- [ ] Improve event system architecture
- [ ] Add comprehensive error handling

### Code Quality
- [ ] Unit tests for inventory system
- [ ] Integration tests
- [ ] Documentation updates
- [ ] Code review all new features

## Resources Needed

### Art
- Item icons (various categories)
- Equipment sprites
- UI elements (borders, buttons)
- Effect particles

### Audio
- UI sounds (hover, click, equip)
- Item pickup sounds
- New weapon sounds
- Ambient sounds

### Design
- Item database
- Balance spreadsheet
- Progression curve
- Economy design

## Milestones

| Milestone | Target Date | Status |
|-----------|-------------|--------|
| Core Gameplay | Week 2 | ✅ Complete |
| Inventory System | Week 4 | ✅ Complete |
| Save System | Week 5 | 🚧 Planned |
| Equipment System | Week 6 | 📋 Backlog |
| Content Complete | Week 9 | 📋 Backlog |
| Release Ready | Week 12 | 📋 Backlog |

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Save system complexity | High | Start early, test thoroughly |
| Performance on mobile | Medium | Profile early, optimize |
| Scope creep | Medium | Strict milestone adherence |
| Asset delivery delays | Low | Use placeholders |

## Success Criteria

- Smooth 60fps gameplay
- Intuitive inventory management
- Satisfying progression loop
- No critical bugs
- Positive playtest feedback
