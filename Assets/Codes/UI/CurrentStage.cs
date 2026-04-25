using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CurrentStage : MonoBehaviour
{
    public Text _titleLabel;
    public Image image;
    public Text _descLabel;
    public Button _selectButton;

    public GameObject _alertPanel;

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

        if (_alertPanel != null)
        {
            _alertPanel.transform.localScale = Vector3.zero; // 알림 패널의 크기를 원래대로 설정
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
            nextStage = DataManager.instance.playerInfo.next_stage;
            _titleLabel.text = "STAGE " + nextStage;
        }
    }

    // 선택 버튼 클릭 시 호출
    void OnSelectButtonClicked()
    {

        if (DataManager.instance.playerInfo.Energy <= 0)
        {
            if (_alertPanel != null)
            {
                _alertPanel.transform.localScale = Vector3.one; // 알림 패널의 크기를 원래대로 설정
                Invoke("CloseAlertPanel", 1f);
            }

            Debug.Log("에너지가 부족합니다!");
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

    void CloseAlertPanel()
    {
        if (_alertPanel != null)
        {
            _alertPanel.transform.localScale = Vector3.zero; // 알림 패널의 크기를 원래대로 설정
        }
    }
}
