using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD 클래스 - 게임 화면의_heads-up Display 정보를 표시합니다.
/// </summary>
public class HUD : MonoBehaviour
{

    //정보 유형 열거형
    public enum InfoType { Level, Kill, Exp, Time, Health, Stage }
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
        //GameManager의 인스턴스가 존재하지 않으면 업데이트 중지
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스가 존재하지 않습니다. HUD 업데이트를 중지합니다.");
            return;
        }

        switch (infoType)
        {

            case InfoType.Exp:
                //경험치 슬라이더 업데이트
                float currExp = GameManager.instance.exp;  //현재 경험치
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];  //다음 경험치
                expSlider.value = currExp / maxExp;  //슬라이더 값 
                break;

            case InfoType.Level:
                //레벨 텍스트 업데이트
                infoText.text = string.Format("LV.{0:F0}", GameManager.instance.level);
                break;

            case InfoType.Kill:
                //처치 수 텍스트 업데이트
                infoText.text = GameManager.instance.kill.ToString();
                break;

            case InfoType.Time:
                //남은시간을 보여줌
                float remainingTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                if (remainingTime < 0f) remainingTime = 0f;

                //분과 초로 표시
                int min = Mathf.FloorToInt(remainingTime / 60f);
                int sec = Mathf.FloorToInt(remainingTime % 60f);

                infoText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;

            case InfoType.Health:
                //체력 슬라이더 업데이트
                float currHealth = GameManager.instance.health;  //현재 체력
                float maxHealth = GameManager.instance.maxHealth;  //최대 체력

                // this.Log($"Health : {expSlider.value}  = {currHealth}  / {maxHealth} ");

                expSlider.value = currHealth / maxHealth;  //슬라이더 값 
                break;

            case InfoType.Stage:
                //스테이지 텍스트 업데이트
                infoText.text = DataManager.instance.playerInfo.curr_stage.ToString();
                break;
        }
    }




}
