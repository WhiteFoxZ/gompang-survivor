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


    }

    private void OnStageSelected()
    {
        if (DataManager.instance.playerInfo.Energy <= 0)
        {

            _alertPanel.SetActive(true);
            //0.5초이후 알림 패널 닫기
            Invoke("CloseAlertPanel", 1f);

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
        _alertPanel.SetActive(false);
    }

}
