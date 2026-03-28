using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json; // 네임스페이스 추가
using System;


// 1.저장할 데이터가 존재
// 2.데이터를 제이슨으로 변환
// 3. 변환한 제이슨을 외부에 저장

public class DataManager : MonoBehaviour
{

    public static DataManager instance;

    PlayerData playerInfo = new PlayerData();

    string path;
    string filename = "playerinfo";

    void Awake()
    {

        // 싱글톤 구현: 인스턴스가 이미 존재하면 자신을 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }



    //장비덱 있는 아이템 저장
    public void InventorySlots()
    {
        InventorySlot[] _inventorySlots = InventoryManager.instance._inventorySlots;    //장착한아이템

        InventoryItem itemSlot;

        EquipmentSO equipmentSO;

        foreach (InventorySlot slot in _inventorySlots)
        {
            itemSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemSlot != null)
            {
                equipmentSO = itemSlot.gameItem;
                if (equipmentSO != null)
                {
                    EquipItem item = new EquipItem(equipmentSO);
                    playerInfo.slotItems.Add(item);
                }
                else
                    print("******InventorySlots is null ****");

            }
        }

        Save();

    }

    //장바구니 버튼에 있는 아이템 저장
    public void GearItemButton()
    {
        GameObject[] _gearItemButton = InventoryManager.instance._gearItemButton;    //장착한아이템

        EquipmentSO equipmentItem;

        foreach (GameObject button in _gearItemButton)
        {
            InventoryButton inventoryButton = button.GetComponent<InventoryButton>();

            if (!inventoryButton.deckFree)
            {
                equipmentItem = inventoryButton._equipmentItem;


                if (equipmentItem != null)
                {
                    EquipItem item = new EquipItem(equipmentItem);
                    playerInfo.buttonItems.Add(item);
                }
                else
                    print("******buttonItems is null ****");
            }



        }

        Save();
    }



    public void Save()
    {
        path = Application.persistentDataPath + "/";

        print(path);

        // string data = JsonUtility.ToJson(playerInfo);

        string data = JsonConvert.SerializeObject(playerInfo, Formatting.Indented);


        File.WriteAllText(path + filename, data);

    }


    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + filename;

        print(filePath);

        if (File.Exists(filePath))
        {
            // 1. JSON 파일 읽기
            string json = File.ReadAllText(filePath);

            print("*********** LoadData *********");
            this.Log(json);

            // 2. 저장용 클래스로 역직렬화
            PlayerData saveData = JsonConvert.DeserializeObject<PlayerData>(json);

            // Load all EquipmentSO and create a lookup dictionary
            EquipmentSO[] allEquipment = Resources.FindObjectsOfTypeAll<EquipmentSO>();
            Dictionary<string, EquipmentSO> equipmentDict = new Dictionary<string, EquipmentSO>();
            foreach (EquipmentSO eq in allEquipment)
            {
                if (!equipmentDict.ContainsKey(eq.id))
                {
                    this.Log($" equipmentDict : {eq.id}");
                    equipmentDict.Add(eq.id, eq);
                }
            }

            foreach (EquipItem item in saveData.slotItems)
            {
                if (equipmentDict.TryGetValue(item.id, out EquipmentSO equipmentToAdd))
                {
                    this.Log($" InventoryManager.instance.AddItem : {equipmentToAdd}");
                    InventoryManager.instance.AddItem(equipmentToAdd);
                }
                else
                {
                    Debug.LogWarning($"EquipmentSO with id {item.id} not found!");
                }
            }

            Debug.Log("데이터 로드 및 에셋 연결 완료!");
        }
    }


}

[System.Serializable]
public class PlayerData
{
    // 1. 기본 정보
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public long CurrentExp { get; set; }

    // 2. 재화 관련
    public int Gold { get; set; }
    public int Gem { get; set; }
    public int Energy { get; set; }
    public DateTime LastEnergyUpdateTime { get; set; } // 에너지 회복 계산용

    // 3. 진행도 및 스탯
    public int MaxStageReached { get; set; } // 최고 클리어 스테이지
    public Dictionary<string, int> Talents { get; set; } // 특성 ID와 강화 레벨

    // 4. 장비 , 인벤토리,SO 객체 대신 ID(이름) 리스트를 저장합니다.
    public List<EquipItem> slotItems = new List<EquipItem>();
    public List<EquipItem> buttonItems = new List<EquipItem>();


}

[System.Serializable]
public class EquipItem
{
    public string id;
    public ItemRarity itemRarity; //등급
    public GearType gearType; //착용아이템 유형

    public int level = 1;

    public float atack;  //공격력
    public float defence;    //방어력
    public float moveSpeed;    //움직임

    public float atkSpeed;     //공격스피드

    public EquipItem()
    {

    }

    public EquipItem(EquipmentSO gameItem)
    {
        this.id = gameItem.id;
        this.itemRarity = gameItem.itemRarity;
        this.gearType = gameItem.gearType;
        this.level = gameItem.level;
        this.atack = gameItem.atack;
        this.defence = gameItem.defence;
        this.moveSpeed = gameItem.moveSpeed;
        this.atkSpeed = gameItem.atkSpeed;

    }



}

