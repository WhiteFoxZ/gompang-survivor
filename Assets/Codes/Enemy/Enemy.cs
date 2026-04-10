using System.Collections;
using UnityEngine;

/// <summary>
///Enemy 클래스 - 적 캐릭터의 동작을 관리합니다.
/// </summary>
public class Enemy : MonoBehaviour
{

    public int boos; //보스 여부 (0: 일반, 1: 보스)
    public float attack; //공격력
    public float speed; //이동 속도
    public float health; //현재 체력
    public float maxHealth; //최대 체력

    public RuntimeAnimatorController[] animatorController; //애니메이션 컨트롤러 배열

    [Header("Target 참고용")]
    public Rigidbody2D target; //플레이어 리지드바디


    public bool isLive; //생존 여부

    Rigidbody2D rigid; //적 리지드바디
    Collider2D col; //콜라이더
    SpriteRenderer sprite; //스프라이트 렌더러

    Animator animator; //애니메이터

    WaitForFixedUpdate wait; //물리 업데이트 대기

    // 보스 패턴 관련 변수
    private bool bossPatternStarted = false; // 보스 패턴이 이미 시작됐는지 추적
    private float bossPatternTriggerDistance = 5.0f; // 보스 패턴이 시작될 거리 (플레이어와의 거리)


    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// 물리 업데이트 - 이동 로직
    /// </summary>
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive) return;

        //넉백을 위한 이동 중지 - 히트 애니메이션이 재생 중이면 이동하지 않도록 설정
        //일반 적은 히트 애니메이션이 재생 중이어도 이동하지 않도록 설정 (보스는 히트 상태에서 멈춤)
        if (boos == 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;
        }

        //플레이어 방향으로 이동
        Vector2 dir = (target.position - rigid.position).normalized;    //이동 방향
        Vector2 move = dir * speed * Time.fixedDeltaTime;   //다음에 움직일 위치
        rigid.MovePosition(rigid.position + move);  //이동

        //충돌 시 밀림 방지 물리적 속도 제거
        rigid.linearVelocity = Vector2.zero;


    }

    /// <summary>
    /// 프레임 후 처리 - 스프라이트 방향 전환
    /// </summary>
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive) return;

        //적 스프라이트 방향 전환
        if (target.position.x < rigid.position.x)
            sprite.flipX = true;
        else
            sprite.flipX = false;
    }

    /// <summary>
    /// 프레임 업데이트 - 보스 패턴 트리거 확인
    /// </summary>
    void Update()
    {
        if (!GameManager.instance.isLive || !isLive)
            return;

        // 보스가 아직 패턴이 시작되지 않았으며, 플레이어와 거리가 충분히 가까우면 패턴 시작
        if (boos == 1 && !bossPatternStarted)
        {
            float distanceToPlayer = Vector2.Distance(rigid.position, target.position);

            this.Log($" distanceToPlayer <= bossPatternTriggerDistance : {distanceToPlayer} <= {bossPatternTriggerDistance} ");


            if (distanceToPlayer <= bossPatternTriggerDistance)
            {
                BossPattern bossPattern = GetComponent<BossPattern>();
                if (bossPattern != null)
                {
                    bossPattern.StartDashPattern();
                    bossPatternStarted = true;
                }
            }
        }
    }

    /// <summary>
    /// 활성화 시 호출 - 적 초기화
    /// </summary>
    void OnEnable()
    {
        if (GameManager.instance.player != null)
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();


        health = maxHealth;

        isLive = true;
        col.enabled = true; //충돌 활성화
        rigid.simulated = true; //물리엔진 활성화
        sprite.sortingOrder = 1; //정렬 순서 설정
        animator.SetBool("Dead", false);
        gameObject.SetActive(true);


    }

    /// <summary>
    /// 적 초기화 - 스폰 데이터 적용
    /// </summary>
    /// <param name="data">스폰 데이터</param>
    public void Init(SpawnData data)
    {
        //애니메이션 컨트롤러 설정
        animator.runtimeAnimatorController = animatorController[data.spriteType];
        boos = data.boss;
        speed = data.speed;
        maxHealth = data.health;
        attack = data.attack;

        health = maxHealth;

    }

    /// <summary>
    /// 트리거 충돌 처리 - 총알에 맞았을 때
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;

        //총알과 충돌 시
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            // Bullet 컴포넌트가 있으면 일반 총알 처리
            if (bullet != null)
            {
                float damage = bullet.damage;

                //한 번만 확률 계산 (Bullet.cs에서 이미 계산된 값을 사용하지 않고 여기서 계산)
                //이렇게 하면 Bullet.cs와 Enemy.cs 중복 방지
                float knockback = bullet.knockback;
                float knockbackRate = bullet.knockbackRate;
                float appliedKnockback = (Random.value <= knockbackRate) ? knockback : 0f;

                if (bullet.itemType == ItemData.ItemType.Melee)
                {
                    // this.Log($"삽 Damage: {damage}, Knockback: {appliedKnockback}  ");
                }
                else if (bullet.itemType == ItemData.ItemType.Range)
                {
                    // this.Log($"총알 Damage: {damage}, Knockback: {appliedKnockback} ");
                }

                //보스는 넉백이 적용되지 않도록 설정 (보스는 넉백 면역)
                if (boos == 1)
                {
                    appliedKnockback = 0f; //보스는 넉백 면역
                    this.Log($"보스는 넉백 면역! Knockback이 0으로 적용됩니다.");
                }
                //데미지 적용 (넉백 값 포함)
                TakeDamage(damage, appliedKnockback);
            }
            else
            {
                // BulletMissle(미사일) 처리 - Bullet 컴포넌트가 없으면 미사일로 간주
                BulletMissle missile = collision.GetComponent<BulletMissle>();
                if (missile != null)
                {
                    float damage = missile.damage;
                    float impackDamage = missile.impackDamage;
                    float impackRate = missile.impackRate;
                    float appliedKnockback = (Random.value <= impackRate) ? impackDamage : 0f;

                    this.Log($"Missile Damage: {damage}, Knockback: {appliedKnockback}");

                    if (boos == 1)
                    {
                        appliedKnockback = 0f; //보스는 넉백 면역
                        this.Log($"보스는 넉백 면역! Knockback이 0으로 적용됩니다.");
                    }
                    //미사일 데미지 적용
                    TakeDamage(damage, appliedKnockback);
                }
            }
        }
    }

    /// <summary>
    /// 데미지 적용 - 적 체력 감소 및死亡 처리
    /// </summary>
    /// <param name="damage">입는 데미지</param>
    /// <param name="knockback">넉백 세기</param>
    public void TakeDamage(float damage, float knockback = 0f)
    {
        health -= damage;

        //체력이 남으면
        if (health > 0)
        {
            //넉백 코루틴 실행 (널값이면 기본값 0f 사용)
            StartCoroutine(KnockBack(knockback));

            //히트 애니메이션 재생
            animator.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.SFX.Hit);
        }
        else //사망
        {
            isLive = false;
            col.enabled = false; //충돌 비활성화
            rigid.simulated = false; //물리엔진 비활성화
            sprite.sortingOrder = 1;
            animator.SetBool("Dead", true);
            GameManager.instance.kill++; //처치 수 증가
            GameManager.instance.GetExp(1); //경험치 획득

            //게임 진행 중이면 사망 효과음
            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.SFX.Dead);
            }
        }
    }


    /// <summary>
    /// 넉백 코루틴 - 적을 뒤로 밀어내는 효과 + 근처 적들에게도 넉백 적용
    /// </summary>
    /// <param name="knockback">넉백 세기</param>
    IEnumerator KnockBack(float knockbackDamage)
    {

        if (knockbackDamage <= 0f)
        {
            yield break; //넉백 세기가 0이하이면 넉백 효과 없음
        }

        //1프레임 대기
        yield return wait;



        //플레이어 위치에서 적 위치로의 방향
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 enemyPos = transform.position;  //적 위치
        Vector3 knockDir = (enemyPos - playerPos).normalized; //넉백 방향

        //넉백 힘 가하기
        rigid.AddForce(knockDir * knockbackDamage, ForceMode2D.Impulse);

        //근처 적들에게도 넉백 적용 (Enemy-to-Enemy Knockback)
        ApplyKnockbackToNearbyEnemies(knockDir, knockbackDamage);
    }

    /// <summary>
    /// 근처 적들에게 넉백 적용 - 적과 적이 붙어있을 때 넉백이 전파되도록
    /// </summary>
    /// <param name="knockDir">넉백 방향</param>
    /// <param name="knockbackDamage">넉백 세기</param>
    void ApplyKnockbackToNearbyEnemies(Vector3 knockDir, float knockbackDamage)
    {
        //근처 적 탐지 범위
        float knockbackRange = 2.0f; //2유닛 이내의 적들에게 넉백 적용
        //근처 적에게 전달되는 넉백 세기 (원래 넉백의 50%)
        float knockbackTransferRate = 0.5f;

        //레이어 마스크: Enemy 레이어만 탐지
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        //근처의 모든 Collider2D 탐지
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, knockbackRange);

        foreach (Collider2D collider in nearbyColliders)
        {
            //자기 자신이 아니고 Enemy 태그를 가진 경우
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                //적 컴포넌트 가져오기
                Enemy nearbyEnemy = collider.GetComponent<Enemy>();
                if (nearbyEnemy != null && nearbyEnemy.isLive)
                {
                    //같은 방향으로 넉백 적용 (덜 강한 세기로)
                    Rigidbody2D nearbyRigid = collider.GetComponent<Rigidbody2D>();
                    if (nearbyRigid != null)
                    {
                        nearbyRigid.AddForce(knockDir * knockbackDamage * knockbackTransferRate, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }



    /// <summary>
    /// 사망 처리 - 오브젝트 비활성화
    /// </summary>
    void Dead()
    {
        gameObject.SetActive(false);
        GameManager.instance.kill++;
        GameManager.instance.GetExp(1);

        if (boos == 1) //보스가 죽었을 때 벽 제거
            GameManager.instance._wallSpawner.RemoveWalls();

    }




}
