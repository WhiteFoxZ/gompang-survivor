using UnityEngine;

public class PlayerInfoLobby : MonoBehaviour
{

    public static PlayerInfoLobby instance;  //싱글톤 인스턴스

    public float playerinfoLV = 1f;
    public float playerinfoMaxLV = 60f;



    [Header("장착장비")]
    public GameItem[] gameItems;


    public InventorySlot[] _inventorySlots; //인벤토리 슬롯 배열

    public GameObject _inventoryItemPrefabs; //인벤토리 아이템 프리팹





    /// <summary>
    /// 시작 시 호출 - 싱글톤 설정 및 초기화
    /// </summary>
    void Awake()
    {

        this.Log($" Awake : ************************** ");

        // 싱글톤 구현: 인스턴스가 이미 존재하면 자신을 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        this.Log($" Start : ************************** ");

        _inventorySlots = InventoryManager.instance._inventorySlots;



        // public GameObject _inventoryItemPrefabs; //인벤토리 아이템 프리팹




        //빈 슬롯에 새로운 아이템 추가
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemSlot = slot.GetComponentInChildren<InventoryItem>();

            //빈 슬롯이면
            if (itemSlot != null)
            {
                //새 아이템 생성

                this.Log($" objname : {itemSlot}");
            }
        }





    }

    // Update is called once per frame
    void Update()
    {

    }
}
