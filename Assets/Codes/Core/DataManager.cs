using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json; // 네임스페이스 추가
using System;


// 1.저장할 데이터가 존재
// 2.데이터를 제이슨으로 변환
// 3. 변환한 제이슨을 외부에 저장

public class DataManager : MonoBehaviour
{

    public static DataManager instance;

    public PlayerData playerInfo = new PlayerData();

    string path;
    string filename = "playerinfo";

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 새로 생긴 건 바로 삭제
        }

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);


    }

    //싱글톤 패턴이고 DontDestroyOnLoad 적용되어 있기 때문에, 씬이 로드될 때마다 초기화 로직을 실행하려면 씬 로드 이벤트를 활용하는 것이 좋습니다.

    private void OnEnable()
    {
        // 씬 로드 이벤트 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출될 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"{scene.name} 씬으로 넘어왔습니다! 씬이 로드될 때마다 호출될 메서드 LoadData(),UpdateEnergy() 호출");
        // 여기서 초기화 로직 실행

        LoadData();
        UpdateEnergy();

    }


    private const int EnergyRechargeInterval = 1200; // 20 minutes in seconds

    public void UpdateEnergy()
    {
        // If energy is already at max, we update the last update time to now to avoid storing old time
        if (playerInfo.Energy >= playerInfo.MaxEnergy)
        {
            playerInfo.LastEnergyUpdateTime = DateTime.UtcNow;
            return;
        }

        TimeSpan elapsed = DateTime.UtcNow - playerInfo.LastEnergyUpdateTime;
        int elapsedSeconds = (int)elapsed.TotalSeconds;

        if (elapsedSeconds < EnergyRechargeInterval)
        {
            // Not enough time to recharge even one energy
            return;
        }

        int rechargeCount = elapsedSeconds / EnergyRechargeInterval;
        int energyToAdd = rechargeCount; // 1 energy per interval

        // Calculate new energy, but do not exceed MaxEnergy
        int newEnergy = Mathf.Min(playerInfo.Energy + energyToAdd, playerInfo.MaxEnergy);

        // Update the last energy update time: we subtract the time that was used for recharging
        // i.e., we only keep the remainder time for the next recharge.
        if (newEnergy > playerInfo.Energy)
        {
            // We actually added some energy, so we update the time by the amount that was used for recharging
            playerInfo.LastEnergyUpdateTime = playerInfo.LastEnergyUpdateTime.AddSeconds(rechargeCount * EnergyRechargeInterval);
            playerInfo.Energy = newEnergy;
        }
        else
        {
            // Energy is already at max, so we set the last update time to now to avoid storing old time
            playerInfo.LastEnergyUpdateTime = DateTime.UtcNow;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Resumed
        {
            UpdateEnergy();
        }
    }


    //장비덱 있는 아이템 저장
    public void InventorySlots()
    {
        InventorySlot[] _inventorySlots = InventoryManager.instance._inventorySlots;    //장착한아이템

        InventoryItem itemSlot;

        EquipmentSO equipmentSO;

        playerInfo.slotItems.Clear();

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



    }

    //장바구니 버튼에 있는 아이템 저장
    public void GearItemButton()
    {
        GameObject[] _gearItemButton = InventoryManager.instance._gearItemButton;    //장착한아이템

        EquipmentSO equipmentItem;

        playerInfo.buttonItems.Clear();

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
    }



    public void Save()
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        this.Log($" SAVE filePath : {filePath}");

        // string data = JsonUtility.ToJson(playerInfo);

        string data = JsonConvert.SerializeObject(playerInfo, Formatting.Indented);


        File.WriteAllText(filePath, data);

    }



    public PlayerData LoadData()
    {

        string filePath = Path.Combine(Application.persistentDataPath, filename);

        // print(filePath);

        if (File.Exists(filePath))
        {
            // 1. JSON 파일 읽기
            string json = File.ReadAllText(filePath);

            // print("*********** LoadData *********");
            // print(json);

            // 2. 저장용 클래스로 역직렬화
            PlayerData saveData = JsonConvert.DeserializeObject<PlayerData>(json);

            playerInfo = saveData; // 로드한 데이터를 playerInfo에 할당

            Debug.Log("데이터 로드 및 에셋 연결 완료!");

            return saveData;
        }

        return playerInfo;
    }


    public void UpdateUI(PlayerData saveData)
    {
        // Load all EquipmentSO and create a lookup dictionary
        EquipmentSO[] allEquipment = Resources.FindObjectsOfTypeAll<EquipmentSO>();
        Dictionary<string, EquipmentSO> equipmentDict = new Dictionary<string, EquipmentSO>();
        foreach (EquipmentSO eq in allEquipment)
        {
            if (!equipmentDict.ContainsKey(eq.id))
            {
                // this.Log($" equipmentDict : {eq.id}");
                equipmentDict.Add(eq.id, eq);
            }
        }

        foreach (EquipItem item in saveData.slotItems)
        {
            if (equipmentDict.TryGetValue(item.id, out EquipmentSO equipmentToAdd))
            {
                // this.Log($" InventoryManager.instance.AddItem : {equipmentToAdd}");
                InventoryManager.instance.AddItem(equipmentToAdd);
            }
            else
            {
                Debug.LogWarning($"EquipmentSO with id {item.id} not found!");
            }
        }

        //버튼
        foreach (EquipItem item in saveData.buttonItems)
        {
            if (equipmentDict.TryGetValue(item.id, out EquipmentSO equipmentToAdd))
            {
                // this.Log($" InventoryManager.instance.AddButtonDeck : {equipmentToAdd}");
                InventoryManager.instance.AddButtonDeck(equipmentToAdd);
            }
            else
            {
                Debug.LogWarning($"EquipmentSO with id {item.id} not found!");
            }
        }
    }


}

[System.Serializable]
public class PlayerData
{
    // 1. 기본 정보
    public string PlayerName = "";
    public float Level = 1;
    public float MaxLevel = 60;

    public int curr_stage = 1;
    public int next_stage = 1; //다음 스테이지

    // 2. 재화 관련
    public int Gold = 0;
    public int Gem = 0;
    public int Energy = 20; // 초기 에너지
    public int MaxEnergy = 60; // 최대 에너지
    public DateTime LastEnergyUpdateTime = System.DateTime.UtcNow;// 에너지 회복 계산용 (UTC 사용)

    // 3. 진행도 및 스탯
    public int MaxStageReached = 0; // 최고 클리어 스테이지
    public Dictionary<string, int> Talents = new Dictionary<string, int>(); // 특성 ID와 강화 레벨

    // 4. 장비 , 인벤토리,SO 객체 대신 ID(이름) 리스트를 저장합니다.
    public List<EquipItem> slotItems = new List<EquipItem>();
    public List<EquipItem> buttonItems = new List<EquipItem>();


    //합산된 장비정보

    public EquipItem GetTotalSlotStats()
    {
        EquipItem total = new EquipItem();
        total.id = "TotalCombinedStats";

        // 1. 모든 장착 아이템의 능력치를 각 아이템 레벨 보정(1%당)을 적용해 합산
        foreach (var item in slotItems)
        {
            if (item == null) continue;

            // 아이벨 보정치 (예: 아이템 10레벨 = 1.1배)
            float itemLevelModifier = 1f + (item.level * 0.01f);

            total.atack += item.atack * itemLevelModifier;
            total.defence += item.defence * itemLevelModifier;
            total.moveSpeed += item.moveSpeed * itemLevelModifier;
            total.atkSpeed += item.atkSpeed * itemLevelModifier;

            // this.Log(total.atack + "   " + itemLevelModifier);

        }

        // 2. 최종 결과에 플레이어 레벨 보정(1%당)을 추가 적용
        // (예: 플레이어 50레벨 = 장비 총합의 1.5배)
        float playerLevelModifier = 1f + (this.Level * 0.01f);

        total.atack *= playerLevelModifier;
        total.defence *= playerLevelModifier;
        total.moveSpeed *= playerLevelModifier;
        total.atkSpeed *= playerLevelModifier;

        return total;
    }

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


