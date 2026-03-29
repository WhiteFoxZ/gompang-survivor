using UnityEngine;
using UnityEngine.UI;

public class LobbyHUD : MonoBehaviour
{
    //정보 유형 열거형
    public enum InfoType { LEVEL, GOLD, GEM, ENERGY }
    public InfoType infoType;  //정보 유형

    UnityEngine.UI.Text infoText;  //정보 텍스트
    Slider expSlider;  //경험치 슬라이더

    public LobbyManager lobbyManager;  //로비 매니저 참조

    public PlayerData playerInfo;

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        infoText = GetComponent<UnityEngine.UI.Text>();
        expSlider = GetComponent<Slider>();

        

        playerInfo = lobbyManager.GetComponent<LobbyManager>().playerData;  //로비 매니저에서 플레이어 데이터 가져오기    


    }

    /// <summary>
    /// 프레임 후 처리 - 게임 정보 업데이트
    /// </summary>
    void LateUpdate()
    {

        switch (infoType)
        {
            case InfoType.LEVEL:
                //스테이지 텍스트 업데이트
                float playerinfoLV = playerInfo.Level;  //현재 체력
                float playerinfoMaxLV = playerInfo.MaxLevel;  //최대 체력
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
