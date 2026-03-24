using UnityEngine;
using UnityEngine.UI;

//스크롤뷰에서 여러이미지중에 duration 기간동안 이미지를 보여주다.특정이미지를 마지막에 보여준다.
public class AutoHorizontalStop : MonoBehaviour
{
    public ScrollRect scrollRect;

    [Header("멈출때까지시간")]
    public float duration = 15f;       // 총 걸리는 시간 (20초)

    public int targetIndex = -1;       // 멈출 이미지 번호
    public int totalImages = 0;      // 전체 이미지 개수

    [Header("속도(돌리기횟수)")]
    public int loopCount = 3;          // 20초 동안 전체를 몇 번 왕복할지 (속도감 결정)

    private float timer = 0f;
    private bool isFinished = true;


    public GameObject _itemBoxBtn;  //확인 버튼을 활성화,비활성화 위해

    public GameObject _itemImge;  //스크롤뷰가 활성화시 보이지 않케

    public GameObject _randomSelect;    //박스종류별 아이템 이미지를 가져온다.


    [Header("구매한 장비아이템 버튼들")]
    public GameObject[] _gearItemButton; //구매한 장비아이템 버튼들


    // 버튼클릭시
    public void OnClickConfirm(int totalImages, ShopItemType shopItemType)
    {
        RandomSelect random = _randomSelect.GetComponent<RandomSelect>();
        int itemIndex = 0;
        EquipmentSO gameItem = null;

        if (shopItemType == ShopItemType.ItemBoxCommon)
        {
            itemIndex = random.GetGameItemCommon();
            gameItem = random.gameItemCommon[itemIndex];
            Debug.Log($"ItemBoxCommon 시작! itemIndex : {itemIndex}");
        }
        else if (shopItemType == ShopItemType.ItemBoxRare)
        {
            itemIndex = random.GetGameItemRare();
            gameItem = random.gameItemRare[itemIndex];
            Debug.Log($"ItemBoxRare 시작! itemIndex : {itemIndex}");
        }
        else if (shopItemType == ShopItemType.ItemBoxEpic)
        {
            itemIndex = random.GetGameItemEpic();
            gameItem = random.gameItemEpic[itemIndex];
            Debug.Log($"ItemBoxEpic 시작! itemIndex : {itemIndex}");
        }
        else if (shopItemType == ShopItemType.ItemBoxLegendary)
        {
            itemIndex = random.GetGameItemLegendary();
            gameItem = random.gameItemLegendary[itemIndex];
            Debug.Log($"ItemBoxLegendary 시작! itemIndex : {itemIndex}");
        }

        targetIndex = itemIndex;
        this.totalImages = totalImages;
        timer = 0f;
        isFinished = false;

        _itemImge.SetActive(false);
        _itemBoxBtn.GetComponent<Button>().interactable = false;

        //장비덱 버튼에 추가
        foreach (GameObject button in _gearItemButton)
        {
            InventoryButton inventoryButton = button.GetComponent<InventoryButton>();

            this.Log($" 장비덱 버튼에 추가 : {inventoryButton.deckFree}");
            if (inventoryButton.deckFree)
            {
                inventoryButton.Init(gameItem);
                break;
            }

        }

    }

    void OnDisable()
    {
        timer = 0f;
        isFinished = true;

        _itemImge.SetActive(true);


        _itemBoxBtn.GetComponent<Button>().interactable = true;

        Debug.Log("OnDisable");
    }


    void Update()
    {
        if (isFinished || scrollRect == null || totalImages <= 1) return;

        timer += Time.deltaTime;
        float t = timer / duration;

        // 1. Ease-Out 큐빅 베지어 (초반엔 빠르고 끝에 아주 천천히 멈춤)
        float curve = 1f - Mathf.Pow(1f - t, 3f);

        // 2. 최종 목표 normalized 위치 (0.0 ~ 1.0)
        // 0번째 이미지는 0, 마지막 이미지는 1에 위치함
        float finalTargetPos = (float)targetIndex / (totalImages - 1);

        // 3. 총 이동해야 할 '양' (루프 횟수 + 최종 위치)
        float totalDistance = loopCount + finalTargetPos;

        // 4. 현재 위치 계산 (0~1 사이로 반복되다가 마지막에 finalTargetPos에 도달)
        float currentPos = (curve * totalDistance);

        // ScrollRect의 위치를 업데이트 (1.0을 넘어가면 나머지 값으로 순환 효과)
        scrollRect.horizontalNormalizedPosition = currentPos % 1.0001f;

        if (t >= 1f)
        {
            // 마지막에 정확한 위치로 고정
            scrollRect.horizontalNormalizedPosition = finalTargetPos;
            isFinished = true;
            _itemBoxBtn.GetComponent<Button>().interactable = true;
        }
    }

}
