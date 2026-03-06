using UnityEngine;

/// <summary>
/// 스캐너 클래스 - 주변의 적을 감지합니다.
/// </summary>
public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위
    public LayerMask targetLayer; // 스캔할 레이어 마스크
    public RaycastHit2D[] targets; // 스캔된 대상들
    public Transform nearestTarget; // 가장 가까운 대상


    /// <summary>
    /// 물리 업데이트 - 적 감지
    /// </summary>
    void FixedUpdate()
    {
        //원의 범위내에 있는 모든 타겟을 감지
        //1. 캐스팅 시작위치
        //2. 원의반지름 (범위)
        //3. 방향벡터 (0,0) 고정
        //4. 거리 (0) 고정

        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0f, targetLayer);

        //가장 가까운 타겟 찾기
        nearestTarget = FindNearestTarget();

    }


    /// <summary>
    /// 가장 가까운 타겟 찾기
    /// </summary>
    /// <returns>가장 가까운 타겟 변환</returns>
    Transform FindNearestTarget()
    {
        Transform nearest = null;
        float diff = 100;   //최소한의거리

        //모든 감지된 타겟 순회
        foreach (RaycastHit2D hit in targets)
        {

            Vector3 myPos = transform.position;
            Vector3 targetPos = hit.transform.position;
            float distance = Vector2.Distance(myPos, targetPos);

            //더 가까운 타겟 발견
            if (distance < diff)
            {
                diff = distance;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

}
