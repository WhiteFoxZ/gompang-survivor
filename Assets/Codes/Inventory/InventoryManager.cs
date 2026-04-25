using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 관리자 클래스 - 인벤토리의 아이템을 관리합니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;  //싱글톤 인스턴스

    public const int maxStackItems = 60; //최대 스택 개수

    public GameObject _inventoryItemPrefabs; //인벤토리 아이템 프리팹

    [Header("장비장착정보")]
    public InventorySlot[] _equipSlots; //인벤토리 슬롯 배열

    [Header("구매한 장비아이템 버튼들")]
    public InventorySlot[] _inventorySlots; //구매한 장비아이템 버튼들

    // public int currentEmptyCount = 0;

    //항상 최신정보를 가질수 있도록 computed property
    public int currentEmptyCount
    {
        get
        {
            int count = 0;
            foreach (InventorySlot slot in _inventorySlots)
            {
                if (slot.GetComponentInChildren<InventoryItem>() == null)
                {
                    count++;
                }
            }
            return count;
        }
    }



    [Header("아이템구매팝업창")]
    public GameObject _shopItemPopUp;
    public Text _itemName;
    public Image _itemImge;
    public Image _priceICon;
    public Text _priceTxt;


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
    public bool AddEquipItem(EquipmentSO newItem)
    {
        //먼저 기존 아이템과 스택 가능한지 확인
        for (int i = 0; i < _equipSlots.Length; i++)
        {
            InventorySlot slot = _equipSlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

            //같은 아이템이고 스택 공간이 있으면
            if (inventoryItem != null
            && inventoryItem.gameItem == newItem
            && inventoryItem.count < maxStackItems
            )
            {
                this.Log($" 갯수증가 : {newItem.gearType}");
                //개수 증가
                inventoryItem.count++;
                //표시 갱신
                inventoryItem.reflushCount();
                return true;
            }
        }


        //빈 슬롯에 새로운 아이템 추가
        for (int i = 0; i < _equipSlots.Length; i++)
        {
            InventorySlot slot = _equipSlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

            //빈 슬롯이면
            if (inventoryItem == null)
            {
                //새 아이템 생성
                SpawnNewItem(newItem, slot);
                this.Log($"Equip 새 아이템 생성 : {newItem.gearType} 인벤토리 [ {currentEmptyCount}/{_inventorySlots.Length} ]");
                return true;
            }
        }

        //인벤토리가 가득 찼음
        this.Log("***** 인벤토리가 가득 찼음 ****");

        return false;
    }


    /// <summary>
    /// 아이템 추가 - 인벤토리에 아이템을 추가합니다.(AutoHorizontalStop 호출,DataManager.Load 호출)
    /// </summary>
    /// <param name="newItem">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddInventoryItem(EquipmentSO newItem)
    {
        //먼저 기존 아이템과 스택 가능한지 확인
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

            //같은 아이템이고 스택 공간이 있으면
            if (inventoryItem != null
            && inventoryItem.gameItem == newItem
            && inventoryItem.count < maxStackItems
            )
            {
                this.Log($" 갯수증가 : {newItem.gearType}");
                //개수 증가
                inventoryItem.count++;
                //표시 갱신
                inventoryItem.reflushCount();
                return true;
            }
        }


        //빈 슬롯에 새로운 아이템 추가
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

            //빈 슬롯이면
            if (inventoryItem == null)
            {
                //새 아이템 생성
                SpawnNewItem(newItem, slot);
                this.Log($"Equip 새 아이템 생성 : {newItem.gearType} 인벤토리 [ {currentEmptyCount}/{_inventorySlots.Length} ]");
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
        //프리팹
        GameObject newItemGameObj = Instantiate(_inventoryItemPrefabs, slot.transform);
        InventoryItem inventoryItem = newItemGameObj.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }



    /// <summary>
    /// 상점 아이템을 클릭했을 때 호출됩니다.
    /// 클릭한 shopItem 팝업창으로 보여준다. 
    /// </summary>
    /// <param name="shopItem">클릭한 상점 아이템 데이터</param>
    public void ShopItemClick(ShopItemSO shopItem)
    {

        this.shopItem = shopItem;

        this._itemName.text = shopItem.title;
        this._itemImge.sprite = shopItem.ImageItem;
        this._priceICon.sprite = shopItem.ImageAd;
        this._priceTxt.text = shopItem.price.ToString();

        if (currentEmptyCount == 0)
        {
            lackInventorySlot();
            return;
        }

        switch (shopItem.payType)
        {
            //광고클릭시 에너지,코인 은 바로충전되고, ItemBoxCommon 경우 _scrollView(램덤아이템 팝업이) 띠워진다. 
            //레어박스,에픽박스,레전드박스 선택시 _shopItemPopUp 띠워진다.
            case PayType.AD:    //광고를 보고나서
            case PayType.PAY:   //현질을 해서 코인,다이아 구매

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.Energy:
                        DataManager.instance.playerInfo.Energy += shopItem.itemCnt;
                        this.Log($" 비용 : {shopItem.price} Energy 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" Energy : {DataManager.instance.playerInfo.Energy} / {DataManager.instance.playerInfo.MaxEnergy}");

                        break;

                    case ShopItemType.Coin:
                        DataManager.instance.playerInfo.Gold += shopItem.itemCnt;
                        this.Log($" 비용 : {shopItem.price} coin 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" Gold : {DataManager.instance.playerInfo.Gold} ");

                        break;

                    case ShopItemType.Diamond:
                        DataManager.instance.playerInfo.Diamond += shopItem.itemCnt;
                        this.Log($"현질 비용 : {shopItem.price} Diamond 구매갯수 itemCnt : {shopItem.itemCnt}");
                        this.Log($" Diamond : {DataManager.instance.playerInfo.Diamond} ");

                        break;

                    case ShopItemType.ItemBoxCommon:

                        lackGold(); //레어,에픽 박스를 선택이후 다시 초기화해주기위해

                        _scrollView.SetActive(true);

                        List<EquipmentSO> items = _randomSelect.GetComponent<RandomSelect>().gameItemCommon;

                        if (nowShopItemType != ShopItemType.ItemBoxCommon)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxCommon);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxCommon;

                        _shopItemPopUp.SetActive(true);

                        //광고 일반박스일때는 아이템버튼을 비활성화
                        Transform itemBoxBtn = _shopItemPopUp.transform.Find("Panel/ContentPanel/ItemBoxBtn");
                        itemBoxBtn.gameObject.SetActive(false);

                        break;

                    default:
                        break;
                }

                break;

            default:

                this.Log($" objname : {shopItem.shopItemType}");

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.ItemBoxRare:
                    case ShopItemType.ItemBoxEpic:

                        //playerInfo 머니가 게임아이템 금액보다 적을경우 itemBoxBtn 비활성화                
                        lackGold();

                        break;

                    case ShopItemType.ItemBoxLegendary:

                        //playerInfo 다이아가 게임아이템 다이아보다 적을경우 itemBoxBtn 비활성화                
                        lackDiamond();

                        break;

                    default:
                        break;
                }

                _shopItemPopUp.SetActive(true);
                break;

        }

        DataManager.instance.Save();

    }

    //구매팝업창 클릭이후 팝업창에서 최종 구매버튼 클릭시 램덤박스View 호출해서 램덤한 아이템이 나온다.
    //Coin 으로 레어,에픽 박스구매시
    //다이아 사용으로 레전드 박스구매시
    public void ConfirmOK()
    {

        this.Log($" confirmOK shopItem.payType  {shopItem.payType} 구매 : {shopItem.shopItemType}");

        List<EquipmentSO> items;

        switch (shopItem.payType)
        {
            case PayType.COIN:

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.ItemBoxRare:
                        DataManager.instance.playerInfo.Gold -= shopItem.price;
                        this.Log($" COIN  ItemBoxRare : {DataManager.instance.playerInfo.Gold}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemRare;

                        if (nowShopItemType != ShopItemType.ItemBoxRare)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxRare);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxRare;

                        if (DataManager.instance.playerInfo.Gold < shopItem.price)
                        {
                            Transform itemBoxBtn = _shopItemPopUp.transform.Find("Panel/ContentPanel/ItemBoxBtn");
                            itemBoxBtn.gameObject.SetActive(false);
                        }

                        break;

                    case ShopItemType.ItemBoxEpic:
                        DataManager.instance.playerInfo.Gold -= shopItem.price;
                        this.Log($" COIN  ItemBoxEpic : {DataManager.instance.playerInfo.Gold}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemEpic;

                        if (nowShopItemType != ShopItemType.ItemBoxEpic)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxEpic);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxEpic;

                        if (DataManager.instance.playerInfo.Gold < shopItem.price)
                        {
                            Transform itemBoxBtn = _shopItemPopUp.transform.Find("Panel/ContentPanel/ItemBoxBtn");
                            itemBoxBtn.gameObject.SetActive(false);
                        }

                        break;
                    default:
                        break;
                }

                break;

            case PayType.DIAMOND:

                switch (shopItem.shopItemType)
                {
                    case ShopItemType.ItemBoxLegendary:
                        DataManager.instance.playerInfo.Diamond -= shopItem.price;
                        this.Log($" DIAMOND  ItemBoxLegendary : {DataManager.instance.playerInfo.Diamond}");

                        _scrollView.SetActive(true);

                        items = _randomSelect.GetComponent<RandomSelect>().gameItemLegendary;

                        if (nowShopItemType != ShopItemType.ItemBoxLegendary)
                            InitScrollViewContent(items);

                        _scrollView.GetComponent<AutoHorizontalStop>().OnClickConfirm(items.Count, ShopItemType.ItemBoxLegendary);    //이미지움직이게

                        nowShopItemType = ShopItemType.ItemBoxLegendary;

                        if (DataManager.instance.playerInfo.Diamond < shopItem.price)
                        {
                            Transform itemBoxBtn = _shopItemPopUp.transform.Find("Panel/ContentPanel/ItemBoxBtn");
                            itemBoxBtn.gameObject.SetActive(false);
                        }

                        break;
                    default:
                        break;
                }

                break;

            default:
                break;

        }

        DataManager.instance.Save();
    }


    bool lackGold()
    {
        Transform contentPanel = _shopItemPopUp.transform.Find("Panel/ContentPanel");
        contentPanel.gameObject.SetActive(true);

        Transform textMsg = _shopItemPopUp.transform.Find("Panel/TextMsg");
        textMsg.gameObject.SetActive(false);

        if (DataManager.instance.playerInfo.Gold < shopItem.price)
        {
            textMsg.GetComponent<Text>().text = "골드가 부족합니다.";
            textMsg.gameObject.SetActive(true);
            contentPanel.gameObject.SetActive(false);
            return false;
        }

        return true;
    }

    bool lackDiamond()
    {
        Transform contentPanel = _shopItemPopUp.transform.Find("Panel/ContentPanel");
        contentPanel.gameObject.SetActive(true);

        Transform textMsg = _shopItemPopUp.transform.Find("Panel/TextMsg");
        textMsg.gameObject.SetActive(false);

        if (DataManager.instance.playerInfo.Diamond < shopItem.price)
        {
            textMsg.GetComponent<Text>().text = "다이아가 부족합니다.";
            textMsg.gameObject.SetActive(true);
            contentPanel.gameObject.SetActive(false);
            return false;
        }

        return true;
    }

    void lackInventorySlot()
    {
        Transform contentPanel = _shopItemPopUp.transform.Find("Panel/ContentPanel");
        contentPanel.gameObject.SetActive(true);

        Transform textMsg = _shopItemPopUp.transform.Find("Panel/TextMsg");
        textMsg.gameObject.SetActive(false);

        textMsg.GetComponent<Text>().text = "빈 데크가 없습니다.";
        textMsg.gameObject.SetActive(true);
        contentPanel.gameObject.SetActive(false);
        _shopItemPopUp.SetActive(true);
        return;

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



    //장비덱 있는 아이템 저장
    public void EquipSlotsUpdate()
    {
        this.Log("EquipSlotsUpdate");

        InventoryItem inventoryItem;

        EquipmentSO equipmentSO;

        //장비덱인지, 인벤토리인지 체크해서 update

        DataManager.instance.playerInfo.equipItems.Clear();

        foreach (InventorySlot slot in _equipSlots)
        {
            this.Log("slot : " + slot);

            inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                this.Log(" inventoryItem " + inventoryItem);

                equipmentSO = inventoryItem.gameItem;
                if (equipmentSO != null)
                {
                    this.Log(" EquipItem.count " + inventoryItem.count);

                    EquipItem item = new EquipItem(equipmentSO, inventoryItem.count);
                    DataManager.instance.playerInfo.equipItems.Add(item);
                }
                else
                {
                    this.Log("******EquipSlots is null ****");
                }
            }
            else
            {
                this.Log("Equip inventoryItem null ");
            }
        }
    }

    //장바구니 버튼에 있는 아이템 저장
    public void InventorySlotsUpdate()
    {
        this.Log("InventorySlotsUpdate");

        InventoryItem inventoryItem;

        EquipmentSO equipmentSO;

        DataManager.instance.playerInfo.inventoryItems.Clear();

        foreach (InventorySlot slot in _inventorySlots)
        {
            this.Log(" slot " + slot);

            inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                this.Log("inventoryItem " + inventoryItem);

                equipmentSO = inventoryItem.gameItem;
                if (equipmentSO != null)
                {

                    this.Log(" inventoryItem.count " + inventoryItem.count);

                    EquipItem item = new EquipItem(equipmentSO, inventoryItem.count);
                    DataManager.instance.playerInfo.inventoryItems.Add(item);
                }
                else
                {
                    this.Log("******InventorySlots is null ****");
                }


            }
            else
            {
                this.Log("inventoryItem null ");
            }
        }

    }



}
