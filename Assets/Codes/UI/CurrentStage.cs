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
                nextStage = PlayerPrefs.GetInt("NextStage", 1);
            }

            // Debug.Log($"현재 스테이지: {nextStage}");

            // 다음 스테이지 표시
            _titleLabel.text = "STAGE " + nextStage;
        }
    }

    // 선택 버튼 클릭 시 호출
    void OnSelectButtonClicked()
    {
        // 선택한 스테이지를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedStage", nextStage);
        PlayerPrefs.Save();

        Debug.Log("스테이지 " + nextStage + " 선택됨");

        // 게임 씬으로 이동
        SceneManager.LoadScene("GameScene");
    }
}
