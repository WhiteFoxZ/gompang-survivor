using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHUD : MonoBehaviour
{
    //정보 유형 열거형
    public enum InfoType { LEVEL, GOLD, GEM, ENERGY }
    public InfoType infoType;  //정보 유형

    Text infoText;  //정보 텍스트
    Slider expSlider;  //경험치 슬라이더

    public LobbyManager lobbyManager;  //로비 매니저 참조

    public PlayerData playerInfo;

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        infoText = GetComponent<Text>();
        expSlider = GetComponent<Slider>();
    }

    void Start()
    {
        StartCoroutine(WaitForLobbyManager());
    }

    IEnumerator WaitForLobbyManager()
    {
        yield return new WaitUntil(() => lobbyManager != null);

        // 인스턴스화된 후 실행할 코드
        // Debug.Log("LobbyManager 준비 완료!");
        playerInfo = lobbyManager.GetComponent<LobbyManager>().playerData;

    }

    /// <summary>
    /// 프레임 후 처리 - 게임 정보 업데이트
    /// </summary>
    void LateUpdate()
    {
        if (playerInfo == null)
        {
            Debug.LogWarning("PlayerData가 아직 로드되지 않았습니다.");
            return;
        }

        switch (infoType)
        {
            case InfoType.LEVEL:
                //스테이지 텍스트 업데이트
                float playerinfoLV = playerInfo.Level;  //현재 체력
                float playerinfoMaxLV = playerInfo.MaxLevel;  //최대 체력

                if (infoText != null)
                    infoText.text = playerinfoLV.ToString();  //골드 텍스트 업데이트

                if (expSlider != null)
                    expSlider.value = playerinfoLV / playerinfoMaxLV;  //슬라이더 값 
                break;

            case InfoType.GOLD:
                infoText.text = playerInfo.Gold.ToString();  //골드 텍스트 업데이트
                break;
            case InfoType.GEM:
                infoText.text = playerInfo.Gem.ToString();  //젬 텍스트 업데이트
                break;
            case InfoType.ENERGY:
                infoText.text = playerInfo.Energy.ToString();  //에너지 텍스트 업데이트
                break;
        }
    }

}
