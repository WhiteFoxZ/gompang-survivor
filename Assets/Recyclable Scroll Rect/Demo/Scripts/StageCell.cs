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

    //Model
    private ContactStageInfo _contactInfo;
    private int _cellIndex;


    private void Awake()
    {
        if (_selectButton != null)
        {
            _selectButton.onClick.AddListener(OnStageSelected);
        }


    }

    private void OnStageSelected()
    {
        Debug.Log($"Stage {_cellIndex} selected!");

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

}
