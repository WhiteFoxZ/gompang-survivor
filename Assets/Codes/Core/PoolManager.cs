using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 오브젝트 풀 관리자 클래스 - 게임 오브젝트의 재사용을 관리합니다.
/// </summary>
public class PoolManager : MonoBehaviour
{

    //오브젝트 유형 열거형
    public enum InfoType { Enemy, Weapon1, Weapon2 }


    // 프리펩들을 보관할 변수
    public GameObject[] prefabs;

    // 풀 담당을 하는 리스트들 각 프리펩마다 하나의 리스트
    private List<GameObject>[] pools;
    private int poolSize = 10; // 각 프리펩당 기본 풀 크기

    /// <summary>
    /// 시작 시 호출 - 풀 초기화
    /// </summary>
    void Awake()
    {
        // 풀 초기화
        pools = new List<GameObject>[prefabs.Length];

        // this.Log(" pools.Length " + pools.Length);

        // 각 프리펩마다 리스트 생성
        for (int i = 0; i < prefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();

        }

    }

    /// <summary>
    /// 풀 초기화 - 미리 오브젝트 생성
    /// </summary>
    public void init()
    {
        // 각 프리펩의 풀에 기본 풀 크기만큼 오브젝트가 있는지 확인하고 부족하면 생성
        for (int i = 0; i < prefabs.Length; i++)
        {
            while (pools[i].Count < poolSize)
            {
                GameObject obj = Instantiate(prefabs[i], transform);
                obj.SetActive(false);
                pools[i].Add(obj);
            }
        }
    }

    /// <summary>
    /// 오브젝트 가져오기 - 풀에서 활성화된 오브젝트 반환
    /// </summary>
    /// <param name="prefabIndex">프리팹 인덱스</param>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject GetObject(int prefabIndex)
    {
        // 해당 프리펩의 풀에서 비활성화된 오브젝트를 찾음
        foreach (GameObject obj in pools[prefabIndex])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // 비활성화된 오브젝트가 없으면 새로 생성
        GameObject newObj = Instantiate(prefabs[prefabIndex], transform);
        pools[prefabIndex].Add(newObj);
        return newObj;
    }



}
