# Product Requirements Document (PRD)

## Undead Survivor

---

## 1. Executive Summary

### Product Overview
Undead Survivor는 Vampire Survivors에서 영감을 받은 2D 탑다운 서바이벌 액션 게임입니다. 플레이어는 끝없이 몰려오는 좀비 무리 속에서 살아남아야 하며, 무기와 능력치를 업그레이드하며 점점 더 강력한 적들에게 맞서야 합니다.

### Target Audience
- **주요 타겟**: 15-35세 캐주얼 게이머
- **플랫폼**: Android (모바일), PC
- **장르**: 로그라이크, 서바이벌, 액션

### Key Value Proposition
- 짧은 세션으로 즐길 수 있는 중독성 있는 게임플레이
- 간단한 조작으로 누구나 쉽게 플레이 가능
- 다양한 무기와 능력 조합으로 전략적 깊이 제공

---

## 2. Game Overview

### Core Gameplay Loop
1. **서바이벌**: 제한된 시간 동안 좀비 무리로부터 생존
2. **전투**: 자동 공격 시스템으로 적을 처치
3. **성장**: 경험치 획득 및 레벨업
4. **선택**: 무기/능력 업그레이드 선택
5. **반복**: 더 강력한 적들과 맞서며 생존 시간 연장

### Game Modes
| Mode | Description |
|------|-------------|
| Survival | 기본 모드, 최대한 오래 생존 |
| Stage Mode | 특정 목표 달성 (예정) |
| Boss Rush | 보스 연전 (예정) |

---

## 3. Features

### 3.1 Core Features

#### Inventory System
- **Status**: 구현 완료
- **Description**: ScriptableObject 기반 아이템 관리 시스템
- **Features**:
  - 20개 슬롯 인벤토리
  - 드래그 앤 드롭 기능
  - 아이템 스택 지원
  - 카테고리 필터링
  - 희귀도 시스템

#### Combat System
- **Status**: 구현 완료
- **Features**:
  - 근접/원거리 무기
  - 자동 타겟팅
  - 다양한 공격 패턴

#### Progression System
- **Status**: 부분 구현
- **Features**:
  - 레벨업 시스템
  - 경험치 획득
  - 메타 진행 (예정)
  - 업적 시스템 (예정)

### 3.2 Planned Features

#### Save System (Phase 2)
- JSON 기반 세이브
- 진행 상황 저장
- 설정 저장
- 클라우드 저장 (옵션)

#### Equipment System (Phase 2)
- 무기/방어구 장착
- 스탯 수정자
- 장비 강화

#### Shop System (Phase 2)
- 인게임 화폐
- 아이템 구매/판매
- 랜덤 상품

#### Content Expansion (Phase 3)
- 새로운 적 종류
- 보스 전투
- 추가 무기 및 능력
- 스테이지 시스템

---

## 4. Technical Requirements

### Platform Requirements
| Platform | Minimum | Recommended |
|----------|---------|-------------|
| Android | 7.0 (API 24) | 10.0+ (API 29+) |
| Memory | 2GB RAM | 4GB+ RAM |
| Storage | 500MB | 1GB |

### Performance Targets
- 60 FPS on target devices
- Level load time < 3 seconds
- Memory usage < 200MB

### Technical Stack
- **Engine**: Unity 6.3 LTS
- **Scripting**: C# (.NET Standard 2.1)
- **Input**: Input System 1.8.1
- **Rendering**: URP 16.0.5

See [tech-stack.md](./tech-stack.md) for detailed technical specifications.

---

## 5. UI/UX Requirements

### Main Screens
1. **Main Menu**
   - 게임 시작
   - 설정
   - 인벤토리
   - 업적
   - 상점 (예정)

2. **Game HUD**
   - 체격 바
   - 경험치 바
   - 현재 레벨
   - 생존 시간
   - 킬 카운트
   - 미니맵 (선택)

3. **Inventory Screen**
   - 슬롯 그리드
   - 아이템 상세 정보
   - 카테고리 필터
   - 장착/사용 버튼

4. **Level Up Screen**
   - 3가지 선택지
   - 아이템 정보 표시
   - 선택 타이머

### Control Scheme
| Action | Input |
|--------|-------|
| Movement | WASD / Joystick |
| Pause | ESC / Back button |
| Inventory | I / Inventory button |
| Interact | Space / Tap |

---

## 6. Art & Audio Requirements

### Art Style
- **Style**: 2D Pixel Art
- **Theme**: Zombie apocalypse, dark atmosphere
- **Color Palette**: Muted colors with highlights

### Required Assets
| Category | Count | Status |
|----------|-------|--------|
| Player Characters | 4+ | Done |
| Enemies | 10+ | Done |
| Weapons | 10+ | In Progress |
| Items | 30+ | In Progress |
| UI Elements | - | In Progress |
| Effects | 10+ | In Progress |

### Audio Requirements
- **BGM**: 3-5 tracks (menu, gameplay, boss)
- **SFX**: 20+ effects (combat, UI, interactions)
- **Format**: WAV/OGG, optimized

---

## 7. Monetization

### Model
- **Primary**: Free-to-play with ads
- **Secondary**: In-app purchases (remove ads, currency)

### Ad Integration Points
- Rewarded ads: Continue after death, bonus rewards
- Interstitial: Between stages (optional)
- Banner: Bottom of menu screens (optional)

### IAP Items
| Item | Price | Description |
|------|-------|-------------|
| Remove Ads | $2.99 | Permanent ad removal |
| Starter Pack | $4.99 | Early game currency pack |
| Currency Packs | $0.99-$19.99 | In-game currency |

---

## 8. Success Metrics

### KPIs
| Metric | Target |
|--------|--------|
| Day 1 Retention | 40%+ |
| Day 7 Retention | 20%+ |
| Session Length | 10+ minutes |
| Sessions per User | 3+ per day |
| Ad Completion Rate | 30%+ |

### Quality Metrics
- Crash rate < 1%
- ANR rate < 0.5%
- User rating 4.0+

---

## 9. Release Plan

### Milestones
| Phase | Date | Deliverables |
|-------|------|--------------|
| Alpha | Week 6 | Core gameplay, basic inventory |
| Beta | Week 9 | Content complete, balance |
| Soft Launch | Week 10 | Limited release, analytics |
| Global Launch | Week 12 | Full release |

### Post-Launch Updates
- Monthly content updates
- Seasonal events
- Bug fixes and optimization

---

## 10. Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Performance issues | High | Medium | Early profiling, optimization |
| Scope creep | Medium | High | Strict milestone adherence |
| Market competition | High | Medium | Unique features, polish |
| Technical debt | Medium | Medium | Code reviews, standards |

---

## 11. Appendix

### Related Documents
- [PROTOCOL.md](./PROTOCOL.md) - Coding standards
- [ACTION_PLAN.md](./ACTION_PLAN.md) - Development roadmap
- [tech-stack.md](./tech-stack.md) - Technical specifications

### Change Log
| Date | Version | Changes |
|------|---------|---------|
| 2026-02-01 | 0.1 | Initial draft |

---

**Document Owner**: Development Team
**Last Updated**: 2026-02-01
**Status**: Draft
