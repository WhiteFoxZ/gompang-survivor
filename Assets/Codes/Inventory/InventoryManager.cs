using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 관리자 클래스 - 인벤토리의 아이템을 관리합니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;  //싱글톤 인스턴스

    const int maxStackItems = 5; //최대 스택 개수

    [Header("장비장착정보")]
    public InventorySlot[] _inventorySlots; //인벤토리 슬롯 배열

    public GameObject _inventoryItemPrefabs; //인벤토리 아이템 프리팹

    [Header("구매한 장비아이템 버튼들")]
    public GameObject[] _gearItemButton; //구매한 장비아이템 버튼들



    [Header("아이템구매팝업창")]
    public GameObject _shopItemPopUp;
    public Text _itemName;
    public Image _itemImge;
    public Image _priceICon;
    public Text _priceTxt;



    [Header("코인")]
    public int coin = 0;
    public int diamond = 0;
    public int energy = 0;
    public int maxEnergy = 0;


    ShopItemSO shopItem;

    [Header("장비아이템_스크롤뷰")]
    public GameObject _scrollView; //장비아이템 팝업창에서 구매확인 클릭시 이미지는 숨기고 스크롧뷰이미지를 보여주기위해

    [Header("장비아이템_스크롤뷰_컨텐츠")]
    public GameObject _scrollViewContent;   //확인버튼 클릭시 아이템박스 종류에 따른 이미지를 추가(램덤박스로부터 가져온다.)

    public GameObject _randomSelect;    //박스종류별 아이템 이미지를 가져온다.


    [Header("장비아이템_이미지 Prefab")]
    public GameObject _scrollViewItemImagePrefab;    // 아이템 이미지를 Prefab.


    ShopItemType nowShopItemType;

    /// <summary>
    /// 시작 시 호출 - 싱글톤 설정 및 초기화
    /// </summary>
    void Awake()
    {

        // 싱글톤 구현: 인스턴스가 이미 존재하면 자신을 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

    }


    /// <summary>
    /// 아이템 추가 - 장비덱에 아이템을 추가합니다.
    /// </summary>
    /// <param name="newItem">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddItem(EquipmentSO newItem)
    {
        //먼저 기존 아이템과 스택 가능한지 확인
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemSlot = slot.GetComponentInChildren<InventoryItem>();

            //같은 아이템이고 스택 공간이 있으면
            if (itemSlot != null
            && itemSlot.gameItem == newItem
            && itemSlot.count < maxStackItems
            )
            {
                //개수 증가
                itemSlot.count++;
                //표시 갱신
                itemSlot.reflushCount();
                return true;
            }
        }


        //빈 슬롯에 새로운 아이템 추가
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemSlot = slot.GetComponentInChildren<InventoryItem>();

            //빈 슬롯이면
            if (itemSlot == null)
            {
                //새 아이템 생성
                SpawnNewItem(newItem, slot);
                return true;
            }
        }

        //인벤토리가 가득 찼음
        this.Log("***** 인벤토리가 가득 찼음 ****");

        return false;
    }

    /// <summary>
    /// 새 아이템 생성 - 지정된 슬롯에 아이템을 생성합니다.
    /// </summary>
    /// <param name="item">생성할 아이템 데이터</param>
    /// <param name="slot">생성할 슬롯</param>
    void SpawnNewItem(EquipmentSO item, InventorySlot slot)
    {
        //프리팹からインスタンス生成
        GameObject newItemGameObj = Instantiate(_inventoryItemPrefabs, slot.transform);
        InventoryItem inventoryItem = newItemGameObj.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }



    public void ShopItemClick(ShopItemSO shopItem)
    {

        this.shopItem = shopItem;

        this._itemName.text = shopItem.title;
        this._itemImge.sprite = shopItem.ImageItem;
        this._priceICon.sprite = shopItem.ImageAd;
        this._priceTxt.text = shopItem.price.ToString();


        switch (shopItem.payType)
        {
            case PayType.AD:
                switch (shopItem.shopItemType)
                {
                    case ShopItemType.Coin:
                        this.coin += shopItem.itemCnt;
                        this.Log($" 비용 : {shopItem.price} coin 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" coin : {this.coin} ");


                        break;

                    case ShopItemType.Energy:
                        this.energy += shopItem.itemCnt;
                        this.Log($" 비용 : {shopItem.price} Energy 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" Energy : {this.energy} / {this.maxEnergy}");

                        break;

                    case ShopItemType.ItemBoxCommon:

                        this.Log($" 비용 : {shopItem.price} ItemBoxSmall 구매갯수 itemCnt : {shopItem.itemCnt}");

                        _scrollView.SetActive(true);

                        List<EquipmentSO> items = _randomSelect.GetComponent<RandomSelect>().gameItemCommon;

                        if (nowShopItemType != ShopItemType.ItemBoxCommon)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxCommon);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxCommon;

                        break;

                    default:
                        break;
                }
                break;

            default:

                _shopItemPopUp.SetActive(true);
                break;

        }
    }

    //구매팝업창 클릭시 램덤박스를 호출해서 램덤한 아이템이 나온다.
    public void ConfirmOK()
    {
        this.Log($" confirmOK shopItem.payType  {shopItem.payType} 구매 : {shopItem.shopItemType}");

        List<EquipmentSO> items;

        switch (shopItem.payType)
        {
            case PayType.PAY:   //코인,다이아

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.Coin:
                        this.coin += shopItem.itemCnt;

                        this.Log($"현질 비용 : {shopItem.price} Coin 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" coin : {this.coin} ");

                        break;

                    case ShopItemType.Diamond:
                        this.diamond += shopItem.itemCnt;
                        this.Log($"현질 비용 : {shopItem.price} Diamond 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" Diamond : {this.diamond} ");

                        break;

                    default:
                        break;
                }

                break;


            case PayType.COIN:

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.ItemBoxRare:
                        this.coin -= shopItem.itemCnt;
                        this.Log($" COIN  ItemBoxRare : {this.coin}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemRare;

                        if (nowShopItemType != ShopItemType.ItemBoxRare)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxRare);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxRare;

                        break;

                    case ShopItemType.ItemBoxEpic:
                        this.coin -= shopItem.itemCnt;
                        this.Log($" COIN  ItemBoxEpic : {this.coin}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemEpic;

                        if (nowShopItemType != ShopItemType.ItemBoxEpic)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxEpic);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxEpic;

                        break;
                    default:
                        break;
                }
                break;

            case PayType.DIAMOND:

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.ItemBoxLegendary:
                        this.diamond -= shopItem.itemCnt;
                        this.Log($" DIAMOND  ItemBoxLegendary : {this.diamond}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemLegendary;

                        if (nowShopItemType != ShopItemType.ItemBoxLegendary)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxLegendary);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxLegendary;

                        break;
                    default:
                        break;
                }
                break;


            default:
                break;

        }
    }

    /**
    장비아이템 구매 확인 팝업창에서 스크롤뷰를 초기화 한다. RandomSelect 로부턴 아이템박스 종류별로 가져온다.
    **/
    private void InitScrollViewContent(List<EquipmentSO> items)
    {
        foreach (Transform child in _scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }


        foreach (EquipmentSO item in items)
        {
            GameObject obj = Instantiate(_scrollViewItemImagePrefab, _scrollViewContent.transform);
            obj.GetComponent<Image>().sprite = item.image;
        }

    }

}
