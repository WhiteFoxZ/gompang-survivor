using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// 스캐너 클래스 - 주변의 적을 감지합니다.
/// </summary>
public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위
    public LayerMask targetLayer; // 스캔할 레이어 마스크
    public RaycastHit2D[] targets; // 스캔된 대상들

    [Header("가장 가까운 타겟")]
    public Transform[] nearestTarget; // 가장 가까운 대상

    [Header("미사일 타겟")]
    public Transform missleTarget; // 미사일용 타겟



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

        //가장 가까운 타겟 순서대로 배열에 저장
        nearestTarget = FindNearestTarget(5);

        //2번째로 가까운 타겟 (미사일용)
        if (nearestTarget != null && nearestTarget.Length > 1)
            missleTarget = nearestTarget[1];
        else
            missleTarget = null;


    }


    /// <summary>
    /// n번째로 가까운 타겟 찾기
    /// </summary>
    /// <param name="count">반환할 타겟 수</param>
    /// <returns>가장 가까운 타겟 배열 (거리순 정렬)</returns>
    Transform[] FindNearestTarget(int count)
    {
        if (targets == null || targets.Length == 0)
            return null;

        // 타겟을 거리순으로 정렬
        var sortedTargets = new List<RaycastHit2D>(targets);
        sortedTargets.Sort((a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.transform.position);
            float distB = Vector2.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        // 요청한 수만큼 타겟 반환
        int returnCount = Mathf.Min(count, sortedTargets.Count);
        Transform[] result = new Transform[returnCount];
        for (int i = 0; i < returnCount; i++)
        {
            result[i] = sortedTargets[i].transform;
        }

        return result;
    }

}
