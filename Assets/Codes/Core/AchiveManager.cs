using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 업적 관리자 클래스 - 캐릭터 잠금 해제 등 업적을 관리합니다.
/// </summary>
public class AchiveManager : MonoBehaviour
{

    public GameObject[] lockCharacter; //잠긴 캐릭터 배열
    public GameObject[] unlockCharacter; //잠금 해제된 캐릭터 배열


    //업적 열거형
    enum Achive { UnlockPotato, UnlockBean }

    Achive[] achives; //업적 배열


    public GameObject uiNotice; //업적 알림 UI
    WaitForSecondsRealtime wait; //대기 시간


    /// <summary>
    /// 시작 시 호출 - 업적 시스템 초기화
    /// </summary>
    void Awake()
    {
        //열거형에서 모든 값 가져오기
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5); //5초 대기

        //저장된 데이터가 없으면 초기화
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    /// <summary>
    /// 초기화 - 첫 실행 시 기본값 설정
    /// </summary>
    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        //모든 업적 초기화 (미달성 상태)
        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }

    }

    /// <summary>
    /// 캐릭터 잠금 해제 상태 업데이트
    /// </summary>
    void UnlockCharacter()
    {
        //모든 캐릭터에 대해 잠금 상태 확인
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;

            //잠금/잠금 해제 UI 토글
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);

        }

    }

    /// <summary>
    /// 게임 시작 시 캐릭터 상태 업데이트
    /// </summary>
    void Start()
    {
        UnlockCharacter();
    }


    /// <summary>
    /// 매 프레임 후 처리 - 업적 달성 조건 체크
    /// </summary>
    void LateUpdate()
    {
        //모든 업적에 대해 달성 여부 체크
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }


    /// <summary>
    /// 업적 달성 조건 체크
    /// </summary>
    /// <param name="achive">체크할 업적</param>
    void CheckAchive(Achive achive)
    {

        bool isAchive = false;

        //업적 유형에 따라 달성 조건 확인
        switch (achive)
        {
            case Achive.UnlockPotato:
                //감자: 10마리 이상 처치
                isAchive = GameManager.instance.kill >= 10;
                break;
            case Achive.UnlockBean:
                //콩: 게임 시간 완료
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }


        //업적 달성 && 아직 달성 안됨
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            //업적 달성 저장
            PlayerPrefs.SetInt(achive.ToString(), 1);

            //해당 업적의 알림만 표시
            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }

            //알림 코루틴 시작
            StartCoroutine(NoticeRoutine());
        }

    }


    /// <summary>
    /// 알림 표시 코루틴 - 일정 시간 후 알림 숨김
    /// </summary>
    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true); //알림 표시

        AudioManager.instance.PlaySfx(AudioManager.SFX.Lose); //효과음 재생

        yield return wait; //대기

        uiNotice.SetActive(false); //알림 숨김

    }

}
