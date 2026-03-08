using UnityEngine;

/// <summary>
/// 스포너 클래스 - 적을 생성합니다.
/// </summary>
public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint; //스폰 위치 배열


    [Header("스폰 데이터 입력값")]
    public SpawnData[] _spawnDatas; //스폰 데이터 배열


    [Header("디버그용 변수")]
    public float levelTime; //레벨당 시간
    public int level = 0; //현재 레벨

    float timer = 0f; //타이머





    /// <summary>
    /// 시작 시 호출 - 초기화
    /// </summary>
    void Awake()
    {

        spawnPoint = GetComponentsInChildren<Transform>();

        //레벨당 시간 계산
        levelTime = GameManager.instance.maxGameTime / _spawnDatas.Length;

    }


    /// <summary>
    /// 매 프레임 호출 - 적 스폰 관리
    /// </summary>
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        //게임시간에 따라 레벨 증가
        level = Mathf.FloorToInt(GameManager.instance.gameTime / levelTime);

        // this.Log("level " + level + "  timer " + timer + " levelTime " + levelTime);

        //레벨이 최대치를 넘지 않도록 제한
        level = Mathf.Min(level, _spawnDatas.Length - 1);

        // this.Log("level2 " + level);



        //스폰 시간 도달 시 적 생성
        if (timer > (_spawnDatas[level].spawnTime))
        {
            timer = 0f;
            SpawnEnemy();
        }

    }

    /// <summary>
    /// 적 스폰 - 랜덤 위치에 적 생성
    /// </summary>
    void SpawnEnemy()
    {



        //풀매니저에서 적 오브젝트 가져오기 (프리팹 인덱스 0)
        GameObject enemy = GameManager.instance.poolManager.GetObject(0);


        //Random.Range(0, 2) = 0, 1 섞임 (레벨 0에서는 0, 1 섞임 / 레벨 1에서는 0, 1, 2 섞임 / 레벨 2에서는 0, 1, 2, 3 섞임)
        int spawnDataIdx = Random.Range(0, level + 1); //레벨+1까지 적 유형 증가 (레벨 1에서도 0, 1 섞임)

        // this.Log("spawnDataIdx " + spawnDataIdx, " level " + level);


        //스폰 데이터 설정 - 
        SpawnData spawnData = new SpawnData
        {
            spawnTime = _spawnDatas[spawnDataIdx].spawnTime,
            spriteType = _spawnDatas[spawnDataIdx].spriteType,

            //스테이지별 체력,스피드 2%증가 + (레벨업에 따른 체력,스피드 10% 증가)
            health = Mathf.RoundToInt(_spawnDatas[spawnDataIdx].health * (1 + (level * 0.02f) + (level * 0.1f))), //레벨업에 따른 체력 증가
            speed = _spawnDatas[spawnDataIdx].speed * (1 + (level * 0.02f) + (level * 0.1f)) //레벨업에 따른 스피드 증가            
        };

        enemy.GetComponent<Enemy>().Init(spawnData);

        //0은 부모 오브젝트이므로 제외하고 랜덤 선택
        int randomIndex = Random.Range(1, spawnPoint.Length);
        Vector3 spawnPos = spawnPoint[randomIndex].position;

        //위치 설정

        enemy.transform.position = spawnPos;

        //활성화
        enemy.SetActive(true);

    }

}

/// <summary>
/// 스폰 데이터 클래스 - 적의 속성을 정의합니다.
/// </summary>
[System.Serializable]
public class SpawnData
{

    public float spawnTime; //스폰 간격
    public int spriteType;  //적의 애니메이션 유형
    public int health; //체력
    public float speed; //이동 속도


}
