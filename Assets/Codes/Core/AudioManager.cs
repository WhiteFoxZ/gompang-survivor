using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 오디오 관리자 클래스 - 배경음과 효과음을 관리합니다.
/// </summary>
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance; //싱글톤 인스턴스

    [Header("#BGM")]
    public AudioResource audioResourceBGM; //BGM 오디오 리소스
    AudioSource bgmPlayer; //배경음 플레이어

    public AudioClip maxLevelbgmClip; //최대 레벨 BGM 클립
    public float bgmVolumn; //BGM 볼륨

    AudioHighPassFilter bgmEffect; //배경음 고역 통과 필터

    public bool isPlaying = false; //재생 상태

    [Header("#SFX")]    //효과음
    public AudioClip[] sfxClip; //효과음 클립 배열
    public float sfxVolumn; //효과음 볼륨
    public int channels; //동시 재생 채널 수
    AudioSource[] sfxPlayers; //효과음 플레이어 배열
    int channelsIndex; //채널 인덱스

    //효과음 열거형
    public enum SFX { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }

    /// <summary>
    /// 시작 시 호출 - 오디오 시스템 초기화
    /// </summary>
    void Awake()
    {
        instance = this;
        Init();
    }

    /// <summary>
    /// 초기화 - 오디오 플레이어 설정
    /// </summary>
    void Init()
    {

        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.resource = audioResourceBGM;
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolumn;

        //카메라의 고역 통과 필터 가져오기
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("Sfxlayer");
        sfxObject.transform.parent = transform;

        sfxPlayers = new AudioSource[channels];

        //채널 수만큼 오디오 소스 생성
        for (int index = 0; index < channels; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolumn;
            sfxPlayers[index].bypassListenerEffects = true;
        }
    }


    /// <summary>
    /// 일시정지/재생 전환
    /// </summary>
    public void Pause()
    {
        if (bgmPlayer.isPlaying)
        {
            bgmPlayer.Pause();
        }
        else
        {
            bgmPlayer.UnPause();
        }

        isPlaying = bgmPlayer.isPlaying;
    }

    /// <summary>
    /// 배경음 재생/정지
    /// </summary>
    /// <param name="isPlay">재생 여부</param>
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    /// <summary>
    /// 배경음 효과 활성화/비활성화
    /// </summary>
    /// <param name="isPlay">효과 활성화 여부</param>
    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }


    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="sfx">재생할 효과음 유형</param>
    public void PlaySfx(SFX sfx)
    {
        //재생하지 않고있는 sfxPlayers 찾기

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelsIndex) % sfxPlayers.Length;

            //이미 재생 중이면 건너뛰기
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            //히트와 근접 공격은 랜덤 음성 선택
            int ranIndex = 0;
            if (sfx == SFX.Hit || sfx == SFX.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelsIndex = loopIndex;

            //효과음 클립 설정 및 재생
            sfxPlayers[loopIndex].clip = sfxClip[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }

    }


}
