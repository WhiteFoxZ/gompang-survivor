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



    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        infoText = GetComponent<Text>();
        expSlider = GetComponent<Slider>();
    }



    /// <summary>
    /// 프레임 후 처리 - 게임 정보 업데이트
    /// </summary>
    void LateUpdate()
    {
        if (DataManager.instance.playerInfo == null)
        {
            Debug.LogWarning("PlayerData가 아직 로드되지 않았습니다.");
            return;
        }

        switch (infoType)
        {
            case InfoType.LEVEL:
                //스테이지 텍스트 업데이트
                float playerinfoLV = DataManager.instance.playerInfo.Level;  //현재 체력
                float playerinfoMaxLV = DataManager.instance.playerInfo.MaxLevel;  //최대 체력

                if (infoText != null)
                    infoText.text = playerinfoLV.ToString();  //골드 텍스트 업데이트

                if (expSlider != null)
                    expSlider.value = playerinfoLV / playerinfoMaxLV;  //슬라이더 값 
                break;

            case InfoType.GOLD:
                infoText.text = DataManager.instance.playerInfo.Gold.ToString();  //골드 텍스트 업데이트
                break;
            case InfoType.GEM:
                infoText.text = DataManager.instance.playerInfo.Gem.ToString();  //젬 텍스트 업데이트
                break;
            case InfoType.ENERGY:
                infoText.text = DataManager.instance.playerInfo.Energy.ToString();  //에너지 텍스트 업데이트
                break;
        }
    }

}
