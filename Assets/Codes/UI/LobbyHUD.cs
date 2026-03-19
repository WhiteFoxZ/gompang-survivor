using UnityEngine;
using UnityEngine.UI;

public class LobbyHUD : MonoBehaviour
{
    //정보 유형 열거형
    public enum InfoType { LEVEL }
    public InfoType infoType;  //정보 유형

    UnityEngine.UI.Text infoText;  //정보 텍스트
    Slider expSlider;  //경험치 슬라이더


    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        infoText = GetComponent<UnityEngine.UI.Text>();
        expSlider = GetComponent<Slider>();

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
                float playerinfoLV = PlayerInfoLobby.instance.playerinfoLV;  //현재 체력
                float playerinfoMaxLV = PlayerInfoLobby.instance.playerinfoMaxLV;  //최대 체력
                expSlider.value = playerinfoLV / playerinfoMaxLV;  //슬라이더 값 
                break;
        }
    }

}
