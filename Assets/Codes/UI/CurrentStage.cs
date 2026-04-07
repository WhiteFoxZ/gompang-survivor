using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CurrentStage : MonoBehaviour
{
    public Text _titleLabel;
    public Image image;
    public Text _descLabel;
    public Button _selectButton;

    private int nextStage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateStageDisplay();

        // 버튼 클릭 이벤트 등록
        if (_selectButton != null)
        {
            _selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 제거: 매 프레임마다 호출하던 코드
    }

    void UpdateStageDisplay()
    {
        if (_titleLabel != null)
        {
            // LobbyScene에서는 PlayerPrefs에서 직접 값 읽기
            // GameScene에서는 GameManager에서 값 가져오기
            if (GameManager.instance != null)
            {
                nextStage = GameManager.instance.next_stage;
            }
            else
            {
                nextStage = DataManager.instance.playerInfo.next_stage; // GameManager가 없을 때 DataManager에서 가져오기);
            }

            // Debug.Log($"현재 스테이지: {nextStage}");

            // 다음 스테이지 표시
            _titleLabel.text = "STAGE " + nextStage;
        }
    }

    // 선택 버튼 클릭 시 호출
    void OnSelectButtonClicked()
    {

        if (DataManager.instance.playerInfo.Energy <= 0)
        {
            Debug.Log("에너지가 부족합니다!");
            // 에너지 부족 시 게임 시작을 막거나, UI에서 알림을 표시할 수 있습니다.
            return;
        }

        // Energy decreases by 1 when starting a game
        DataManager.instance.playerInfo.Energy -= 1;
        if (DataManager.instance.playerInfo.Energy < 0)
        {
            DataManager.instance.playerInfo.Energy = 0;
        }

        DataManager.instance.Save(); // 에너지 업데이트 및 저장



        // 선택한 스테이지를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedStage", nextStage);
        PlayerPrefs.Save();

        Debug.Log("스테이지 " + nextStage + " 선택됨");

        // 게임 씬으로 이동
        SceneManager.LoadScene("GameScene");
    }
}
