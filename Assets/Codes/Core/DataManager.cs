using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json; // 네임스페이스 추가
using System;
using System.Collections;


// 1.저장할 데이터가 존재
// 2.데이터를 제이슨으로 변환
// 3. 변환한 제이슨을 외부에 저장

public class DataManager : MonoBehaviour
{

    public static DataManager instance;

    public PlayerData playerInfo = new PlayerData();

    string path;

    string filename = "playerinfo";

    private bool isEquipmentDataReady = false;

    void Awake()
    {

        this.Log("********** Awake **********");

        if (instance == null)
        {
            instance = this;

            this.Log("LobbyManager 장비정보 다운로드 시작");
            StartCoroutine(LoadDataAndStartGame());

        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 새로 생긴 건 바로 삭제
        }

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);

    }


    IEnumerator LoadDataAndStartGame()
    {
        this.Log("ㅇ EquipmentSO 장비 데이터 다운로드 먼저 실행 시작");
        // 장비 데이터 다운로드 먼저 실행
        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(GoogleSpreadSheetManager.DownType.Equip));

        this.Log("LobbyManager EquipmentSO 장비 데이터 다운로드 먼저 실행 완료");

        isEquipmentDataReady = true; // Set flag when data is ready


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
        if (scene.name == "LobbyScene")
        {
            if (!isEquipmentDataReady)
            {
                StartCoroutine(WaitForEquipmentDataThenInitialize(scene));
                return;
            }

        }

    }

    private IEnumerator WaitForEquipmentDataThenInitialize(Scene scene)
    {
        while (!isEquipmentDataReady)
        {
            yield return null; // Wait one frame
        }

        // Now data is ready, initialize
        Debug.Log($"{scene.name} 씬으로 넘어왔습니다! 씬이 로드될 때마다 호출될 메서드 LoadData(),UpdateEnergy(),UpdateUI() 호출");
        // 여기서 초기화 로직 실행
        LoadData();
        UpdateEnergy();
        UpdateUI();
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

        this.Log("********** UpdateEnergy 완료 *******");

    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Resumed
        {
            UpdateEnergy();
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
            playerInfo = JsonConvert.DeserializeObject<PlayerData>(json);

            Debug.Log("데이터 로드 및 에셋 연결 완료!");

            this.Log($" LoadData playerInfo : {playerInfo} ");
        }

        return playerInfo;
    }

    /**
    아이템 장착 UI 업데이트
    **/
    public void UpdateUI()
    {
        this.Log("************ UpdateUI *******");

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

        //장착된 아이템 업데이트            
        foreach (EquipItem item in playerInfo.equipItems)
        {
            if (equipmentDict.TryGetValue(item.id, out EquipmentSO equipmentSO))
            {
                this.Log($"장착된 아이템 InventoryManager.instance.AddItem : {equipmentSO} ");

                // if (item.count > 0) equipmentSO.count = item.count;

                for (int i = 0; i < item.count; i++)
                {
                    InventoryManager.instance.AddEquipItem(equipmentSO);
                }
            }
            else
            {
                Debug.LogWarning($"EquipmentSO with id {item.id} not found!");
            }
        }

        //box 아이템 업데이트
        foreach (EquipItem item in playerInfo.inventoryItems)
        {
            if (equipmentDict.TryGetValue(item.id, out EquipmentSO equipmentSO))
            {
                // this.Log($" InventoryManager.instance.AddButtonDeck : {equipmentSO}");                

                for (int i = 0; i < item.count; i++)
                {
                    InventoryManager.instance.AddInventoryItem(equipmentSO);
                }

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
    public int Diamond = 0;

    public int Energy = 20; // 초기 에너지
    public int MaxEnergy = 60; // 최대 에너지
    public DateTime LastEnergyUpdateTime = System.DateTime.UtcNow;// 에너지 회복 계산용 (UTC 사용)

    // 3. 진행도 및 스탯
    public int MaxStageReached = 0; // 최고 클리어 스테이지
    public Dictionary<string, int> Talents = new Dictionary<string, int>(); // 특성 ID와 강화 레벨

    // 4. 장비 , 인벤토리,SO 객체 대신 ID(이름) 리스트를 저장합니다.
    public List<EquipItem> equipItems = new List<EquipItem>();
    public List<EquipItem> inventoryItems = new List<EquipItem>();


    //합산된 장비정보 - 게임씬에서 사용

    public EquipItem GetTotalSlotStats()
    {
        EquipItem total = new EquipItem();
        total.id = "TotalCombinedStats";

        // 1. 모든 장착 아이템의 능력치를 각 아이템 레벨 보정(1%당)을 적용해 합산
        foreach (var item in equipItems)
        {
            total.atack += item.atack * item.count;
            total.defence += item.defence * item.count;
            total.moveSpeed += item.moveSpeed * item.count;
            total.atkSpeed += item.atkSpeed * item.count;

            this.Log($"item id : {item.id} ,itemRarity : {item.itemRarity}, total.atack : {total.atack} = {item.atack} * {item.count} ");

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


    public override string ToString()
    {
        if (equipItems == null || equipItems.Count == 0)
            return "슬롯이 비어 있습니다.";

        return string.Join("\n", equipItems);

    }

}




