using UnityEngine;
using System.Collections;
using static GoogleSpreadSheetManager;

/// <summary>
/// 스포너 클래스 - 적을 생성합니다.
/// </summary>
public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint; //스폰 위치 배열


    [Header("Emeny스폰 데이터 입력값")]
    public static SpawnData[] _spawnDatas; //스폰 데이터 배열


    [Header("디버그용 변수")]
    public float levelTime; //레벨당 시간
    public int enemyLevel = 0; //적 레벨 (0부터 시작)

    float timer = 0f; //타이머

    public static float speedRate = 0f; //적 스피드 증가율
    public static float healthRate = 0f; //적 체력 증가율
    public static float attackRate = 0f; //적 공격력 증가율
    public static float spawnTimeRate = 0f; //스폰 시간 감소율



    /// <summary>
    /// 시작 시 호출 - 초기화
    /// </summary>
    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Start()
    {
        StartCoroutine(LoadDataAndStartGame());
    }


    IEnumerator LoadDataAndStartGame()
    {
        // 아이템 데이터 다운로드 먼저 실행
        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(DownType.ENEMY));

        //레벨당 시간 계산
        levelTime = GameManager.instance.maxGameTime / _spawnDatas.Length;

    }


    /// <summary>
    /// 매 프레임 호출 - 적 스폰 관리
    /// </summary>
    void Update()
    {
        int curr_stage = GameManager.instance.curr_stage;

        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        //게임시간에 따라 Enemy 종류변경,레벨 계산
        enemyLevel = Mathf.FloorToInt(GameManager.instance.gameTime / levelTime);

        // this.Log("enemyLevel " + enemyLevel + "  timer " + timer + " levelTime " + levelTime);

        //레벨이 최대치를 넘지 않도록 제한
        enemyLevel = Mathf.Min(enemyLevel, _spawnDatas.Length - 1);

        // this.Log("level2 " + enemyLevel);


        //Random.Range(0, 2) = 0, 1 섞임
        int spawnDataIdx = Random.Range(0, enemyLevel + 1); //레벨+1까지 적 유형 증가

        // this.Log("spawnDataIdx " + spawnDataIdx, " enemyLevel " + enemyLevel);

        //스폰 시간 도달 시 적 생성
        //스테이지가 증가할수록 스폰 시간 5% 감소
        float spawnTime = _spawnDatas[spawnDataIdx].spawnTime * Mathf.Pow((1 - spawnTimeRate), enemyLevel);

        // this.Log($" {spawnTime} : {_spawnDatas[spawnDataIdx].spawnTime} * Mathf.Pow((1 - {spawnTimeRate}), {enemyLevel}) ");
        float spawnTime2 = spawnTime;

        spawnTime = spawnTime * Mathf.Pow((1 - spawnTimeRate), curr_stage); //게임 레벨에 따른 추가 스폰 시간 감소

        // this.Log($" {spawnTime} : {spawnTime2} * Mathf.Pow((1 - {spawnTimeRate}), {curr_stage}) ");

        spawnTime = Mathf.Max(spawnTime, 0.3f); //최소 스폰 시간 제한;

        // this.Log($" {spawnTime} : Mathf.Max({spawnTime}, 0.5f) ");

        if (timer > spawnTime)
        {
            timer = 0f;
            SpawnEnemy(spawnDataIdx);
        }

    }

    /// <summary>
    /// 적 스폰 - 랜덤 위치에 적 생성
    /// </summary>
    void SpawnEnemy(int spawnDataIdx)
    {

        int curr_stage = GameManager.instance.curr_stage;

        //풀매니저에서 적 오브젝트 가져오기 (프리팹 인덱스 0)
        GameObject enemy = GameManager.instance.poolManager.GetObject(0);

        this.Log("SpawnEnemy curr_stage" + curr_stage + " spawnDataIdx " + spawnDataIdx);

        //스폰 데이터 설정 - 
        SpawnData spawnData = new SpawnData
        {
            attack = _spawnDatas[spawnDataIdx].attack,

            spriteType = _spawnDatas[spawnDataIdx].spriteType,

            //스테이지별 체력,스피드 증가 

            health = Mathf.RoundToInt(_spawnDatas[spawnDataIdx].health * (1 + (curr_stage * healthRate))),

            speed = _spawnDatas[spawnDataIdx].speed * (1 + (curr_stage * speedRate))
        };

        spawnData.Print();

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
    public float attack; //공격력

    public void Print()
    {
        Debug.Log($"SpawnData  spriteType: {spriteType}, health: {health}, speed: {speed}, attack: {attack}");
    }

}
