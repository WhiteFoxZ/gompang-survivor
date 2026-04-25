using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PolyAndCode.UI;

//Cell class for demo. A cell in Recyclable Scroll Rect must have a cell class inheriting from ICell.
//The class is required to configure the cell(updating UI elements etc) according to the data during recycling of cells.
//The configuration of a cell is done through the DataSource SetCellData method.
//Check RecyclableScrollerDemo class
public class StageCell : MonoBehaviour, ICell
{
    //UI
    public Text _titleLabel;
    public Image image;
    public Text _descLabel;
    public Button _selectButton;
    public GameObject _alertPanel;

    //Model
    private ContactStageInfo _contactInfo;
    private int _cellIndex;


    private void Awake()
    {
        if (_selectButton != null)
        {
            _selectButton.onClick.AddListener(OnStageSelected);
        }

        //Tag 이름보다 검색 속도가 빠르며 관리가 쉽습니다
        _alertPanel = GameObject.FindWithTag("NoEnergyAlert");
        if (_alertPanel != null)
        {
            _alertPanel.transform.localScale = Vector3.zero; // 알림 패널의 크기를 원래대로 설정
        }

    }

    private void OnStageSelected()
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

        DataManager.instance.Save("에너지저장"); // 에너지 업데이트 및 저장

        // Save selected stage (1-based for display)
        int selectedStage = _cellIndex + 1;
        PlayerPrefs.SetInt("SelectedStage", selectedStage);
        PlayerPrefs.Save();

        // Load the game scene
        SceneManager.LoadScene("GameScene");
    }

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(ContactStageInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;

        _titleLabel.text = contactInfo.id;
        _descLabel.text = contactInfo.Desc;

    }

    void CloseAlertPanel()
    {
        if (_alertPanel != null)
        {
            _alertPanel.transform.localScale = Vector3.zero; // 알림 패널의 크기를 원래대로 설정
        }
    }

}
