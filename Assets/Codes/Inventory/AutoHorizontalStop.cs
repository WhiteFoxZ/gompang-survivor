using UnityEngine;
using UnityEngine.UI;

//스크롤뷰에서 여러이미지중에 duration 기간동안 이미지를 보여주다.특정이미지를 마지막에 보여준다.
public class AutoHorizontalStop : MonoBehaviour
{
    public ScrollRect scrollRect;

    [Header("멈출때까지시간")]
    public float duration = 20f;       // 총 걸리는 시간 (20초)

    public int targetIndex = -1;       // 멈출 이미지 번호
    public int totalImages = 0;      // 전체 이미지 개수

    [Header("속도(돌리기횟수)")]
    public int loopCount = 5;          // 20초 동안 전체를 몇 번 왕복할지 (속도감 결정)

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

            this.Log($" inventoryButton.deckFree : {inventoryButton.deckFree}");
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
        if (isFinished || scrollRect == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // 1. 속도감 연출: Ease-Out (처음엔 빠르고 끝에 느려짐)
        // t를 보정하여 초반에 훨씬 더 많이 움직이게 만듭니다.
        float curve = 1f - Mathf.Pow(1f - t, 3f);

        // 2. 전체 이동 거리 계산
        // (전체 한 바퀴 * 루프 횟수) + 최종 목표 위치
        float targetPos = (float)(targetIndex - 1) / (totalImages - 1);
        float totalMovement = loopCount + targetPos;

        // 3. 실제 적용 (NormalizedPosition은 0~1 사이여야 하므로 % 1 사용)
        float currentPos = (curve * totalMovement) % 1.0001f;
        scrollRect.horizontalNormalizedPosition = currentPos;

        if (t >= 1f)
        {
            scrollRect.horizontalNormalizedPosition = targetPos; // 정확한 위치 고정
            isFinished = true;

            _itemBoxBtn.GetComponent<Button>().interactable = true;
        }
    }
}
