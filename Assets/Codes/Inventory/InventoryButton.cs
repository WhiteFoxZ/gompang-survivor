using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{

    public InventoryManager _inventoryManager; // 인벤토리 관리자 참조

    [HideInInspector]
    public EquipmentSO _equipmentItem; // 테스트용 획득 아이템 배열

    private Image _buttonImage; // 버튼 이미지 컴포넌트
    private Text _buttonText; // 버튼 텍스트 컴포넌트

    private Image _darkBackground; // 아이템이 없는 경우 어두운 배경 이미지

    public bool deckFree = true;

    Button btn;


    public void Awake()
    {
        //해당버튼 클릭 못하도록 인터랙션 비활성화
        btn = GetComponent<Button>();
        btn.interactable = false;

        //버튼 이미지와 텍스트 null로 초기화
        if (_buttonImage != null)
        {
            _buttonImage.sprite = null;
        }

        if (_buttonText != null)
        {
            _buttonText.text = null;
        }
    }



    public void Init(EquipmentSO equipmentItem)
    {
        this._equipmentItem = equipmentItem;
        btn.interactable = true;

        if (_inventoryManager == null)
        {
            Debug.LogError("InventoryManager reference is missing!");
        }

        if (_equipmentItem == null)
        {
            Debug.LogError("GameItem reference is missing!");
        }
        //_equipmentItem 에 이미지를 넣어주세요. 그래야 인벤토리에 아이템이 보입니다.

        _buttonImage = GetComponentsInChildren<Image>()[1];
        _buttonText = GetComponentInChildren<Text>();

        if (_buttonImage != null)
        {
            _buttonImage.sprite = _equipmentItem.image; // 버튼 이미지 설정
        }

        if (_buttonText != null && _equipmentItem.name != null)
        {
            _buttonText.text = _equipmentItem.name; // 버튼 텍스트 설정
        }

        // 아이템이 없는 경우 어두운 배경 이미지 설정
        _darkBackground = GetComponentsInChildren<Image>()[2];
        if (_darkBackground != null)
        {
            _darkBackground.enabled = false; // 아이템이 있는경우 어두운 배경 비활성화
        }

        deckFree = false;

    }


    /// <summary>
    /// 아이템 획득 - 지정된 ID의 아이템을 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="id">아이템 ID</param>
    public void pickupItem()
    {
        //인벤토리에 아이템 추가 시도
        bool result = _inventoryManager.AddItem(_equipmentItem);

        if (result)
        {
            this.Log("item added: " + _equipmentItem.name);

            _darkBackground.enabled = true; // 아이템이 추가되면 어두운 배경 비활성화

            //해당버튼 클릭 못하도록 인터랙션 비활성화
            btn.interactable = false;

            //버튼 이미지와 텍스트 null로 초기화
            if (_buttonImage != null)
            {
                _buttonImage.sprite = null;
            }

            if (_buttonText != null)
            {
                _buttonText.text = null;
            }

            deckFree = true;
            //파일로 저장
            DataManager.instance.InventorySlots();
        }
        else
        {
            this.Log("full inventory: ");
        }
    }

}
