using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 클래스 - 플레이어 캐릭터의 동작을 관리합니다.
/// </summary>
public class Player : MonoBehaviour
{
    public Vector2 inputVector;  //입력벡터
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

    }

    /// <summary>
    /// 활성화 시 호출 - 캐릭터 설정
    /// </summary>
    void OnEnable()
    {
        //기본 속도 설정 (캐릭터별 보정)
        speed = 3f * Character.Speed;
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

        //체력 감소
        GameManager.instance.health -= Time.deltaTime * 10;

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


}
