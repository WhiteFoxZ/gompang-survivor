using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GoogleSpreadSheetManager;

/// <summary>
/// 게임 관리자 클래스 - 게임의 주요 로직을 관리합니다.
/// </summary>
public class GameManager : MonoBehaviour
{

    public static GameManager instance;  //싱글톤 인스턴스

    [Header("게임 Control")]
    public bool isLive = false; //게임 진행 중 여부
    public float gameTime;  //게임 시간
    public float maxGameTime = 2f * 60f;  //최대 게임 시간 (2분)
    //현재스테이지
    public int curr_stage = 1;

    public int next_stage = 1; //다음 스테이지

    public int maxStage = 100; //최대 스테이지

    [Header("플레이어 정보")]
    public int playerId; //플레이어 ID
    public float health;  //플레이어 체력
    public float maxHealth = 100;  //플레이어 최대 체력



    public int level = 0;  //현재 레벨
    public int kill; //처치한 적 수
    public int exp;  //현재 경험치
    public int[] nextExp;  //다음 레벨업에 필요한 경험치

    [Header("게임 Object")]
    public Player player;  //플레이어 참조
    public PoolManager poolManager; //오브젝트 풀 매니저 참조

    public LevelUp uiLevelup;  //레벨업 UI 참조

    public Result uiResult; //게임 결과 UI

    public Transform _uiJoy; //조이스틱

    public GameObject enemyCleaner; //적 정리기

    public PlayerData playerData;  //플레이어 장비,재능


    /// <summary>
    /// 시작 시 호출 - 싱글톤 설정 및 초기화
    /// </summary>
    void Awake()
    {
        Application.targetFrameRate = 60; //프레임 고정 (60fps)

        //저장된 스테이지 정보 불러오기 (기본값 1)
        next_stage = PlayerPrefs.GetInt("NextStage", 1);

        //싱글톤 인스턴스 설정
        if (instance == null) instance = this;
        else Destroy(gameObject);

        //장비적용
        playerData = DataManager.instance.LoadData();

    }

    /// <summary>
    /// 게임 시작 시 호출 - 데이터 로드 후 게임 시작
    /// </summary>
    void Start()
    {
        StartCoroutine(LoadDataAndStartGame());
    }

    /// <summary>
    /// 데이터 로드 후 게임 시작 코루틴
    /// </summary>
    IEnumerator LoadDataAndStartGame()
    {
        // 아이템 데이터 다운로드 먼저 실행
        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(DownType.Item));

        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(DownType.Exp));
        // 다운로드 완료 후 게임 시작
        GameStart(0);
    }

    /// <summary>
    /// 게임 시작 - 플레이어 및 게임 상태 초기화
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    public void GameStart(int playerId)
    {
        // 스테이지 선택 UI에서 선택한 스테이지 확인
        int selectedStage = PlayerPrefs.GetInt("SelectedStage", -1);
        if (selectedStage > 0)
        {
            //선택한 스테이지 사용
            curr_stage = selectedStage;
            //사용 후 선택한 스테이지 정보 삭제
            PlayerPrefs.DeleteKey("SelectedStage");
        }
        else
        {
            //기본값: 저장된 다음 스테이지 사용
            curr_stage = next_stage;
        }

        this.playerId = playerId;

        //장비 적용
        EquipItem equipItem = playerData.GetTotalSlotStats();


        maxHealth = maxHealth * (1 + equipItem.level * 0.01f);

        health = maxHealth; //체력 초기화

        player.gameObject.SetActive(true);

        uiLevelup.Select(0);
        uiLevelup.Select(1);
        uiLevelup.Select(6);

        // Energy decreases by 1 when starting a game
        if (playerData != null)
        {
            if (playerData.Energy <= 0)
            {
                Debug.Log("에너지가 부족합니다!");
                // 에너지 부족 시 게임 시작을 막거나, UI에서 알림을 표시할 수 있습니다.
                return;
            }

            playerData.Energy -= 1;
            if (playerData.Energy < 0)
                playerData.Energy = 0;
            DataManager.instance.Save();
        }

        Resume();

        //오디오 재생
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBgm(true);
            AudioManager.instance.PlaySfx(AudioManager.SFX.Select);
        }

        Debug.Log($"스테이지 {curr_stage} 시작!");
    }

    /// <summary>
    /// 매 프레임 호출 - 게임 시간 관리
    /// </summary>
    void Update()
    {
        if (!isLive)
            return;

        //게임 시간 증가
        gameTime += Time.deltaTime;

        //게임 시간 초과 시 승리
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }

    }


    /// <summary>
    /// 경험치 획득 - 레벨업 처리
    /// </summary>
    /// <param name="value">획득 경험치</param>
    public void GetExp(int value)
    {

        //게임 종료 시 경험치 얻지 않도록
        if (!isLive)
            return;

        exp += value;

        // this.Log("lv: " + level + "  " + exp + " : " + nextExp[Mathf.Min(level, nextExp.Length - 1)]);


        // 현재 레벨에 필요한 경험치 가져오기 (인덱스 초과 방지)
        int requiredExp = nextExp[Mathf.Min(level, nextExp.Length - 1)];

        // while문을 사용하여 초과된 경험치만큼 연속 레벨업 처리
        while (exp >= requiredExp)
        {
            exp -= requiredExp; // 남은 경험치 이월
            level++;
            uiLevelup.Show();

            // 다음 레벨 경험치 갱신 (최대 레벨 체크 포함)
            requiredExp = nextExp[Mathf.Min(level, nextExp.Length - 1)];
        }


    }

    /// <summary>
    /// 게임 일시정지
    /// </summary>
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; //시간 정지
        _uiJoy.localScale = Vector3.zero;    //조이스틱 숨김
    }

    /// <summary>
    /// 게임 재개
    /// </summary>
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1; //시간 정상 흐름
        _uiJoy.localScale = Vector3.one;    //조이스틱 보임
    }





    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    /// <summary>
    /// 게임 오버 코루틴
    /// </summary>
    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);

        //결과 UI 표시
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        //효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.SFX.Lose);

        // 실패 시 현재 스테이지 다시 시작
        yield return new WaitForSeconds(1.0f);
        PlayerPrefs.SetInt("SelectedStage", curr_stage);
        PlayerPrefs.Save();

    }


    /// <summary>
    /// 게임 승리 처리
    /// </summary>
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    /// <summary>
    /// 게임 승리 코루틴
    /// </summary>
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);   //경험치를 얻을수 있으므로 GetExp 수정필요

        yield return new WaitForSeconds(0.5f);

        // 승리 시 스테이지 번호 증가 및 저장
        if (next_stage < maxStage)
        {
            next_stage++;
            PlayerPrefs.SetInt("NextStage", next_stage);
            PlayerPrefs.Save();
        }

        // Increase Gold by the current stage value when winning
        if (playerData != null)
        {
            playerData.Gold += curr_stage * 100;
            DataManager.instance.Save();
        }

        //결과 UI 표시
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        //오디오 재생
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.PlaySfx(AudioManager.SFX.Win);
        }

    }



    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void GameRetry()
    {
        Time.timeScale = 1;
        // 현재 씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 데이터 초기화 (테스트용)
    /// </summary>
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        next_stage = 1;
    }

    /// <summary>
    /// 로비로 이동
    /// </summary>
    public void Lobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void SetMaxGameTime(int time)
    {
        maxGameTime = time;
    }

    public void SetMaxGameStage(int stage)
    {
        maxStage = stage;
    }

    public void SetExtExp(int[] extExp)
    {
        nextExp = extExp;
    }


}
