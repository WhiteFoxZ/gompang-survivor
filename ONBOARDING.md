# Undead Survivor - 온보딩 문서

## 📋 목차

1. [프로젝트 개요](#1-프로젝트-개요)
2. [개발 환경 설정](#2-개발-환경-설정)
3. [프로젝트 구조](#3-프로젝트-구조)
4. [주요 시스템 설명](#4-주요-시스템-설명)
5. [코딩 컨벤션](#5-코딩-컨벤션)
6. [개발 워크플로우](#6-개발-워크플로우)
7. [문제 해결 가이드](#7-문제-해결-가이드)

---

## 1. 프로젝트 개요

### 1.1 게임 소개

**Undead Survivor**는 Vampire Survivors에서 영감을 받은 2D 탑다운 서바이벌 액션 게임입니다.

- **플랫폼**: Android, PC
- **장르**: 로그라이크, 서바이벌, 액션
- **Unity 버전**: 6.3 LTS
- **渲染**: URP (Universal Render Pipeline)
- **스크립트**: C# (.NET Standard 2.1)

### 1.2 핵심 게임 루프

```
1. 서바이벌 → 2. 전투 (자동 공격) → 3. 성장 (레벨업) → 4. 선택 (업그레이드) → 5. 반복
```

### 1.3 현재 개발 상태

| 구분 | 상태 |
|------|------|
| 코어 게임 시스템 | ✅ 완료 |
| 인벤토리 시스템 | ✅ 완료 |
| 레벨업 시스템 | ✅ 완료 |
| 세이브 시스템 | ⏳ 개발 예정 |
| 장비 시스템 | ⏳ 개발 예정 |
| 상점 시스템 | ⏳ 개발 예정 |

---

## 2. 개발 환경 설정

### 2.1 필수 요구사항

- **Unity**: 6.3 LTS 이상
- **Visual Studio**: 2022 이상
- **.NET**: 8.0 이상

### 2.2 프로젝트 클론 및 실행

```bash
# 1. 프로젝트 클론
git clone [repository-url]

# 2. Unity Hub에서 프로젝트 열기
# Unity Hub → Add → 프로젝트 폴더 선택

# 3. Packages 복원
# Unity가 자동으로 Packagesfolder의 package-lock.json을 기반으로 복원
```

### 2.3 Unity 패키지 종속성

프로젝트에서 사용 중인 주요 패키지:

| 패키지 | 버전 | 용도 |
|--------|------|------|
| Input System | 1.8.1 | 플레이어 입력 처리 |
| URP | 16.0.5 | 2D 렌더링 |
| Recyclable Scroll Rect | - | 인벤토리 스크롤 (Asset) |

### 2.4 빌드 설정

- **플랫폼**: Android (모바일), PC
- **타겟 프레임레이트**: 60 FPS
- **메모리 목표**: 200MB 이하

---

## 3. 프로젝트 구조

```
TitleMap_Cinemar/
├── Assets/
│   ├── Codes/                 # 모든 C# 스크립트
│   │   ├── Combat/            # 전투 시스템 (무기, 총알, 아이템)
│   │   ├── Core/              # 핵심 시스템 (GameManager, Audio, Pool)
│   │   ├── Enemy/             # 적 AI (스폰, 스캐너, 리포지션)
│   │   ├── Inventory/         # 인벤토리 시스템
│   │   ├── Player/            # 플레이어 컨트롤
│   │   ├── UI/                # UI 시스템 (HUD, 레벨업, 결과)
│   │   └── Utils/             # 유틸리티
│   ├── Scenes/                # 유니티 씬 파일
│   ├── Prefabs/               # 프리팹 에셋
│   ├── Items/                 # 게임 아이템 데이터 (ScriptableObject)
│   ├── LobbyScene/            # 로비 관련 에셋
│   ├── Settings/             # 빌드 및 프로젝트 설정
│   └── Resources/             # 리소스 폴더
├── .vscode/                   # VSCode 설정
├── PRD.md                     # 产品要求文档
└── ACTION_PLAN.md             # 개발 로드맵
```

### 3.1 Scripts 디렉토리 상세 구조

```
Assets/Codes/
├── Combat/
│   ├── Bullet.cs          # 총알 로직
│   ├── Gear.cs            # 장비 시스템
│   ├── Item.cs            # 아이템 기본 클래스
│   ├── ItemData.cs        # 아이템 데이터
│   └── Weapon.cs          # 무기 시스템
│
├── Core/
│   ├── AchiveManager.cs   # 업적 시스템
│   ├── AudioManager.cs    # 오디오 관리 (BGM, SFX)
│   ├── GameManager.cs     # 게임 핵심 관리 (싱글톤)
│   ├── MySceneManager.cs  # 씬 관리 유틸리티
│   └── PoolManager.cs     # 오브젝트 풀링
│
├── Enemy/
│   ├── Enemy.cs            # 적 기본 클래스
│   ├── Reposition.cs      # 적 위치 재설정
│   ├── Scanner.cs         # 타겟 스캐닝
│   └── Spawner.cs          # 적 스폰 관리
│
├── Inventory/
│   ├── GameItem.cs         # 게임 아이템 데이터
│   ├── InventoryItem.cs    # 인벤토리 슬롯 아이템
│   ├── InventoryManager.cs # 인벤토리 관리 (싱글톤)
│   └── InventorySlot.cs    # 인벤토리 슬롯
│
├── Player/
│   ├── Character.cs        # 캐릭터 스탯
│   ├── Hand.cs             # 무기 손 위치
│   └── Player.cs           # 플레이어 컨트롤러
│
├── UI/
│   ├── HUD.cs              # 게임 내 HUD
│   ├── LevelUp.cs          # 레벨업 선택 UI
│   └── Result.cs           # 게임 결과 UI
│
└── Utils/
    └── Debug2.cs           # 디버그 유틸리티
```

---

## 4. 주요 시스템 설명

### 4.1 GameManager (핵심 시스템)

**위치**: [`Assets/Codes/Core/GameManager.cs`](Assets/Codes/Core/GameManager.cs)

게임의 핵심을 담당하는 싱글톤 매니저입니다.

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager instance;  //싱글톤 인스턴스

    // 게임 상태
    public bool isLive = false;
    public float gameTime;           // 현재 게임 시간
    public float maxGameTime = 2f * 60f;  // 최대 게임 시간 (2분)

    // 플레이어 정보
    public float health;             // 플레이어 체력
    public float maxHealth = 100;   // 최대 체력
    public int level = 0;            // 현재 레벨
    public int kill;                 // 킬 수
    public int exp;                  // 현재 경험치
    public int[] nextExp;            // 레벨업 필요 경험치

    // 현재 스테이지
    public int curr_stage = 1;
    public int next_stage = 1;
    public int maxStage = 100;
}
```

**주요 기능**:
- 게임 상태 관리 (시작, 일시정지, 종료)
- 레벨업 및 경험치 관리
- 스테이지 진행 관리
- 게임 오버/승리 처리

### 4.2 Player (플레이어)

**위치**: [`Assets/Codes/Player/Player.cs`](Assets/Codes/Player/Player.cs)

플레이어Movement와 입력을 처리합니다.

```csharp
public class Player : MonoBehaviour
{
    public Vector2 inputVector;   // 입력 벡터
    public float speed;           // 이동 속도

    // 컴포넌트
    private Rigidbody2D rig2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Scanner scanner;       // 적 탐지
    public Hand[] hands;          // 무기 손
}
```

**주요 기능**:
- WASD/조이스틱 입력 처리
- 물리 기반 이동 (`FixedUpdate`)
- 스프라이트 방향 전환
- 애니메이션 제어

#### Health 이동 (피격 시스템)

플레이어가 적과 충돌 시 체력이 지속적으로 감소하는 로직입니다.

```csharp
// 피격 - OnCollisionStay2D: 적과 충돌 중일 때마다 호출
void OnCollisionStay2D(Collision2D collision)
{
    if (!GameManager.instance.isLive)
        return;

    // 충돌 중일 때마다 체력 감소 (1초당 10 데미지)
    GameManager.instance.health -= Time.deltaTime * 10;

    if (GameManager.instance.health < 0)
    {
        // 자식 오브젝트 모두 비활성화 (무기, 효과 등)
        for (int index = 0; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(false);
        }

        // 사망 애니메이션 재생
        animator.SetTrigger("Dead");
        
        // 게임 오버 처리
        GameManager.instance.GameOver();
    }
}
```

**주요 포인트**:
- `OnCollisionStay2D`: 적과 충돌하는 동안 매 프레임 호출
- `Time.deltaTime * 10`: 초당 10 데미지 (프레임 독립적)
- 체력이 0 이하가 되면 게임 오버

### 4.3 Enemy (적 AI)

**위치**: [`Assets/Codes/Enemy/Enemy.cs`](Assets/Codes/Enemy/Enemy.cs)

적의 기본 동작을 담당합니다.

```csharp
public class Enemy : MonoBehaviour
{
    public float speed;           // 이동 속도
    public float health;          // 현재 체력
    public float maxHealth;       // 최대 체력
    public Rigidbody2D target;     // 추적 대상 (플레이어)
}
```

**주요 기능**:
- 플레이어 추적 및 이동
- 피격 및 넉백 처리
- 사망 시 경험치 부여
- 다양한 애니메이션 상태 관리

### 4.4 Combat System (전투 시스템)

**위치**: [`Assets/Codes/Combat/Weapon.cs`](Assets/Codes/Combat/Weapon.cs)

무기 및 공격 로직을 처리합니다.

```csharp
public class Weapon : MonoBehaviour
{
    public int id;                // 무기 ID
    public float damage;          // 공격력
    public int count;             // 투사체 수 (관통력)
    public float speed;           // 공격 속도
}
```

**주요 기능**:
- 자동 타겟팅 (가장 가까운 적)
- 근접/원거리 무기 지원
- 무기 레벨업
- 무기별 특수 동작 (예: 삽 - 회전 공격)

### 4.5 Inventory System (인벤토리)

**위치**: [`Assets/Codes/Inventory/InventoryManager.cs`](Assets/Codes/Inventory/InventoryManager.cs)

아이템 관리 시스템을 담당합니다.

```csharp
public class InventoryManager : MonoBehaviour
{
    const int maxStackItems = 5;  // 최대 스택 수

    public InventorySlot[] _inventorySlots;
    public GameObject _inventoryItemPrefabs;

    public bool AddItem(GameItem newItem);  // 아이템 추가
    void SpawnNewItem(GameItem item, InventorySlot slot);  // 아이템 생성
}
```

**주요 기능**:
- 아이템 추가 (스택 지원)
- 슬롯 관리
- 드래그 앤 드롭 UI
- 카테고리 필터링

### 4.6 AudioManager (오디오)

**위치**: [`Assets/Codes/Core/AudioManager.cs`](Assets/Codes/Core/AudioManager.cs)

BGM 및 효과음을 관리합니다.

```csharp
public class AudioManager : MonoBehaviour
{
    public enum SFX { Select, Win, Lose, Hit, Dead, Range, Melee }
    
    public void PlayBgm(bool isPlay);    // BGM 재생
    public void PlaySfx(SFX sfx);         // 효과음 재생
}
```

### 4.7 PoolManager (오브젝트 풀링)

**위치**: [`Assets/Codes/Core/PoolManager.cs`](Assets/Codes/Core/PoolManager.cs)

게임 오브젝트의 효율적인 재사용을 관리합니다.

```csharp
public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;    // 풀링할 프리팹 배열
    
    public GameObject GetObject(int prefabId);  // 오브젝트 가져오기
    public void ReturnObject(GameObject obj);   // 오브젝트 반환
}
```

---

## 5. 코딩 컨벤션

### 5.1命名规则

| 类型 | 规则 | 示例 |
|------|------|------|
| 클래스 | PascalCase | `GameManager`, `InventoryManager` |
| 메소드 | PascalCase | `GameStart()`, `AddItem()` |
| 변수 | camelCase | `gameTime`, `playerId` |
| 상수 | PascalCase | `MAX_STACK_ITEMS` |
|_PRIVATE 변수 | _camelCase | `_inventorySlots` |

### 5.2 코드 주석

```csharp
// 주석은 한국어로 작성
// 구분선 사용 (可选)

// TODO: 구현 필요 사항
// FIXME: 수정 필요 사항
```

### 5.3 접근 제어자

- **public**: 다른 클래스에서 접근 가능
- **private**: 해당 클래스 내에서만 접근
- **[HideInInspector]**: Inspector 숨김 (편집 불가)
- **readonly**: 읽기 전용 변수

### 5.4 Unity 특이 사항

```csharp
// 게임 오브젝트 접근은 GetComponent 사용
Rigidbody2D rig2d = GetComponent<Rigidbody2D>();

// 자식 오브젝트 접근 (비활성화 포함)
hands = GetComponentsInChildren<Hand>(true);

// 싱글톤 접근
GameManager.instance.GameStart(0);

// 로그 출력 (디버그 유틸리티)
this.Log("로그 메시지");
```

---

## 6. 개발 워크플로우

### 6.1 기능 개발流程

```
1. 브랜치 생성 (feature/기능명)
   └─ git checkout -b feature/new-weapon

2. 기능 구현
   └─ 코드 작성 및 테스트

3. 커밋
   └─ git add . → git commit -m "feat: 새 무기 추가"

4. 풀 리퀘스트
   └─ 코드 리뷰 요청

5. 머지
   └─ 메인 브랜치에 병합
```

### 6.2 주요 브랜치

| 브랜치 | 용도 |
|--------|------|
| main | 프로덕션 브랜치 |
| develop | 개발 브랜치 |
| feature/* | 기능 개발 |
| fix/* | 버그 수정 |

### 6.3 Commit Message 규칙

```
feat:     새 기능 추가
fix:      버그 수정
refactor: 코드 리팩토링
docs:     문서 수정
style:    코드 스타일 변경
```

---

## 7. 문제 해결 가이드

### 7.1 흔한 문제들

#### Q: 게임이 실행되지 않아요
```
1. Unity 프로젝트가 열려있는지 확인
2. Console 창 에러 확인
3. Packagesfolder 복원 시도
```

#### Q: 인벤토리가 작동하지 않아요
```
1. InventoryManager 연결 확인
2. InventorySlot 프리팹 확인
3. Console 에러 확인
```

#### Q: 적들이 생성되지 않아요
```
1. Spawner 설정 확인
2. Enemy 프리팹 확인
3. SpawnData 설정 확인
```

### 7.2 디버깅 팁

```csharp
// 로그 출력
Debug.Log("메시지");

// 조건부 로그 (개발 시)
#if UNITY_EDITOR
    Debug.Log("개발 모드 로그");
#endif

// Inspector에서 변수 값 확인
// 게임 오브젝트 비활성화하여 테스트
```

### 7.3 유용한 단축키

| 동작 | 단축키 |
|------|--------|
| 실행 | Ctrl + P |
| 일시정지 | Ctrl + Shift + P |
| 단계 실행 | Ctrl + Alt + P |
| 빌드 | Ctrl + B |

---

## 📞 지원 및 참고 자료

### 내부 문서
- [PRD.md](./PRD.md) - 产品要求 문서
- [ACTION_PLAN.md](./ACTION_PLAN.md) - 개발 로드맵

### 외부 참고
- [Unity 공식 문서](https://docs.unity3d.com/)
- [URP 문서](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/)
- [Input System 문서](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest/)

---

**문서 작성일**: 2026-02-28  
**최종 수정일**: 2026-02-28  
**문서 소유자**: Development Team
