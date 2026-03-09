using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// 총알 클래스 - 투사체 무기의子弹을 관리합니다.
/// </summary>
public class BulletMissle : MonoBehaviour
{

    public float damage; //총알 데미지
    public float impackDamage; //임팩트 세기
    public float impackRate; //임팩트 확률
    Rigidbody2D rig2d; //물리엔진 컴포넌트

    public Transform target;

    public float speed = 5f;
    public float rotateSpeed = 200f;


    /// <summary>
    /// 초기화 메서드 - 총알의 속성을 설정합니다.
    /// </summary>
    void Awake()
    {
        //Rigidbody2D 컴포넌트 가져오기
        rig2d = GetComponent<Rigidbody2D>();
    }


    /// <summary>
    /// 총알 초기화 - 데미지, 타겟, 방향, 넉백을 설정합니다.
    /// </summary>
    /// <param name="damage">총알 데미지</param>
    /// <param name="target">목표 타겟</param> 
    /// <param name="impackDamage">임팩트 세기</param>
    /// <param name="impackRate">임팩트 확률</param>
    public void Init(float damage, Transform target, float impackDamage = 3f, float impackRate = 1f)
    {
        this.damage = damage;
        this.target = target;
        this.impackDamage = impackDamage;
        this.impackRate = impackRate;

    }


    void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.position - rig2d.position;

        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rig2d.angularVelocity = -rotateAmount * rotateSpeed;

        rig2d.linearVelocity = transform.up * speed;
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

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 투사체 삭제 - 화면 밖으로 나갔을 때 총알을 비활성화합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        //영역 밖이 아니거나 관통력이 없는 경우 종료
        if (!collision.CompareTag("Area"))
            return;

        //총알 비활성화
        gameObject.SetActive(false);

    }


}
