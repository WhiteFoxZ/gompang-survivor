using UnityEngine;

/// <summary>
/// 총알 클래스 - 투사체 무기의子弹을 관리합니다.
/// </summary>
public class Bullet : MonoBehaviour
{

    public string name; //무기이름
    public ItemData.ItemType itemType;
    public float damage; //총알 데미지
    public float per; //총알 관통력
    public float knockback; //넉백 세기
    public float knockbackRate; //넉백 확률
    Rigidbody2D rig2d; //물리엔진 

    PlayerData playerData;

    /// <summary>
    /// 초기화 메서드 - 총알의 속성을 설정합니다.
    /// </summary>
    void Awake()
    {
        //Rigidbody2D 컴포넌트 가져오기
        rig2d = GetComponent<Rigidbody2D>();

        //장비적용
        playerData = GameManager.instance.playerData;
    }



    /// <summary>
    /// 총알 초기화 - 데미지, 관통력, 방향, 넉백을 설정합니다.
    /// </summary>
    /// <param name="damage">총알 데미지</param>
    /// <param name="per">관통력 수치</param>
    /// <param name="dir">이동 방향</param>
    /// <param name="knockback">넉백 세기</param>
    /// <param name="knockbackRate">넉백 확률</param>
    public void Init(float damage, float per, Vector3 dir, float knockback = 3f, float knockbackRate = 1f)
    {

        this.damage = damage;
        this.per = per;
        this.knockback = knockback;
        this.knockbackRate = knockbackRate;

        //장비적용
        EquipItem equipItemTotal = playerData.GetTotalSlotStats();
        this.damage = this.damage * (1 + equipItemTotal.atack * 0.01f);

        if (per >= 0)   //원거리무기인경우
        {
            //관통력이 있을때 속도 증가
            rig2d.linearVelocity = dir * 15f;
        }

    }

    /// <summary>
    /// 트리거 충돌 처리 - 적과 충돌했을 때의 로직
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이 아니면 종료
        if (!collision.CompareTag("Enemy"))
            return;

        // 관통력이 있는 경우(per >= 0)만 관통 처리
        if (per >= 0)
        {
            //관통력 1 감소
            per--;

            //관통력이 0미만이면 총알 비활성화
            if (per < 0)
            {
                rig2d.linearVelocity = Vector2.zero;
                //관통력이 0이되면 총알 비활성화
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 투사체 삭제 - 화면 밖으로 나갔을 때 총알을 비활성화합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        //영역 밖이 아니거나 관통력이 없는 경우 종료
        if (!collision.CompareTag("Area") || per == -100)
            return;

        //총알 비활성화
        gameObject.SetActive(false);

    }


}
