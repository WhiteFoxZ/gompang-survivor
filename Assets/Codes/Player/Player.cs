using UnityEngine;

using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// 플레이어 클래스 - 플레이어 캐릭터의 동작을 관리합니다.
/// </summary>
public class Player : MonoBehaviour
{

    public Vector2 inputVector;  //입력벡터

    [Header("캐릭터 스탯")]
    public float defaultSpeed; //기본 이동 속도;
    public float speed; //이동 속도
    public bool isFloat = true; //부드러운 움직임 여부

    private Rigidbody2D rig2d;    //물리엔진 컴포넌트

    private SpriteRenderer spriteRenderer;  //스프라이트렌더러 컴포넌트

    private Animator animator;  //애니메이터 컴포넌트

    [HideInInspector]
    public Scanner scanner; //적 스캐너 컴포넌트

    [HideInInspector]
    public Hand[] hands; //손 배열

    //케릭터 선택시 플레이어 이미지 및 ani 컨틀로러 변경
    public RuntimeAnimatorController[] animCon;

    // StamPack effect tracking
    private bool isStampPackActive;
    private Coroutine stampEffectCoroutine;

    public bool IsStampPackActive => isStampPackActive;


    public GameObject _fireVFXObject; //(스팀팩 효과 추적용)

    PlayerData playerData;  //플레이어 장비,재능


    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    void Awake()
    {
        rig2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        //비활성화된 자식을 가져올때는 GetComponentsInChildren<Hand>(true) 사용
        hands = GetComponentsInChildren<Hand>(true);
        //장비적용
        playerData = GameManager.instance.playerData;
    }

    /// <summary>
    /// 활성화 시 호출 - 캐릭터 설정
    /// </summary>
    void OnEnable()
    {
        //기본 속도 설정 (캐릭터별 보정)
        speed = defaultSpeed * Character.Speed;

        //장비 적용
        EquipItem equipItem = playerData.GetTotalSlotStats();
        speed = speed * (1 + equipItem.moveSpeed * 0.01f);


        //애니메이션 컨트롤러 설정
        animator.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    /// <summary>
    /// 이동 입력 처리 - InputSystem 이벤트
    /// </summary>
    /// <param name="value">입력 값</param>
    void OnMove(InputValue value)   //InputSystem 이벤트 함수
    {
        //게임이 종료되었으면 입력 무시
        if (!GameManager.instance.isLive)
        {
            inputVector = Vector2.zero;
            return;
        }


        Vector2 input = value.Get<Vector2>();

        if (input != null)
        {
            inputVector = input;
        }
    }


    /// <summary>
    /// 점프 입력 처리 - 테스트용 (무기 레벨업)
    /// </summary>
    /// <param name="value">입력 값</param>
    public void OnJump(InputValue value)
    {
        if (!GameManager.instance.isLive)
            return;

        //스페이스바가 눌렸을 때
        if (value.isPressed)
        {
            //무기 데미지, 투사체 개수 증가 (테스트용)
            transform.Find("Weapon0").gameObject.GetComponent<Weapon>().LevelUp(5f, 1);
        }
    }




    /// <summary>
    /// 물리 업데이트 - 이동 처리
    /// </summary>
    void FixedUpdate()  //물리적 이동처리
    {
        if (!GameManager.instance.isLive)
            return;

        //다음 위치 계산
        Vector2 nextVector = inputVector * (speed * Time.fixedDeltaTime);

        //물리엔진을 사용한 이동
        rig2d.MovePosition(rig2d.position + nextVector);
    }



    /// <summary>
    /// 프레임 후 처리 - 애니메이션 및 스프라이트 업데이트
    /// </summary>
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (animator != null)
        {
            //애니메이터 파라미터 설정 : Speed - 이동속도로 run/idle 구분
            animator.SetFloat("Speed", inputVector.magnitude);
        }


        //좌우 방향 전환
        if (inputVector.x != 0)
        {
            spriteRenderer.flipX = inputVector.x < 0;
        }
    }


    /// <summary>
    /// 충돌 유지 시 호출 - 피격 처리
    /// </summary>
    /// <param name="collision">충돌 정보</param>
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;


        float enemyAttack = 10;

        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // 정보를 가져오는 예시
            enemyAttack = enemy.attack;
        }

        EquipItem equipItem = playerData.GetTotalSlotStats();

        float deffence = equipItem.defence;

        // 최종 데미지 = 공격력 * (100 / (100 + 방어력))
        // 방어력 100일 때: 데미지 50 % 감소(1 / 2)
        // 방어력 200일 때: 데미지 66 % 감소(1 / 3)

        float damage = enemyAttack * (100 / (100 + deffence));

        //체력 감소
        GameManager.instance.health -= Time.deltaTime * damage;

        //체력이 0 이하이면 사망
        if (GameManager.instance.health < 0)
        {
            //모든 자식 오브젝트 비활성화
            for (int index = 0; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            //사망 애니메이션 재생
            animator.SetTrigger("Dead");
            //게임 오버 처리
            GameManager.instance.GameOver();
        }


    }

    /// <summary>
    /// StampPack 효과 - 30초간 이동속도 100% 증가, 총알 간격 100% 증가
    /// </summary>
    /// <param name="duration">지속 시간</param>
    /// <returns></returns>
    public IEnumerator ApplyStampEffect(float duration)
    {
        isStampPackActive = true;

        _fireVFXObject.SetActive(true); //스팀팩 효과 오브젝트 활성화

        //이동속도 100% 증가 (2배)
        float originalSpeed = speed;
        speed = originalSpeed * 2f;

        //모든 발사무기 발사 (간격 50% 감소)
        //Melee 무기: 회전 속도 100% 증가
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        float[] originalWeaponSpeeds = new float[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            originalWeaponSpeeds[i] = weapons[i].speed;

            //무기 타입에 따라 다른 로직 적용
            if (weapons[i].id == 0)
            {
                //Melee 무기: 회전 속도 100% 증가
                weapons[i].speed = weapons[i].speed * 2f;
            }
            else
            {
                //Range/Missile 무기: 발사 간격 50% 감소 (100% 증가 = 간격 50% 감소)
                weapons[i].speed = weapons[i].speed * 0.5f;
            }
        }

        yield return new WaitForSeconds(duration);

        //원래 상태 복원
        speed = originalSpeed;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].speed = originalWeaponSpeeds[i];
        }

        isStampPackActive = false;
        _fireVFXObject.SetActive(false); //스팀팩 효과 오브젝트 비활성화

    }


}
