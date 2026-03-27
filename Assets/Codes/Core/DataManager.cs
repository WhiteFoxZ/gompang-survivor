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

    public static DataManager instance;  //싱글톤 인스턴스

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


    public void Save()
    {
        path = Application.persistentDataPath + "/";

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
                    playerInfo.slotItems.Add(equipmentSO.id);
            }

        }


        print(path);



        // InventorySlot[] _inventorySlots = InventoryManager.instance._inventorySlots;    //장착한아이템

        // InventoryItem itemSlot;

        // EquipmentSO equipmentSO;

        // foreach (InventorySlot slot in _inventorySlots)
        // {
        //     itemSlot = slot.GetComponentInChildren<InventoryItem>();
        //     if (itemSlot != null)
        //     {
        //         equipmentSO = itemSlot.gameItem;
        //         if (equipmentSO != null)
        //             playerInfo.slotItems.Add(equipmentSO.id);
        //     }

        // }



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

            print(json);


            // 2. 저장용 클래스로 역직렬화
            PlayerData saveData = JsonConvert.DeserializeObject<PlayerData>(json);

            // 3. 실제 게임 데이터(PlayerInfo)에 값 복사
            PlayerData info = new PlayerData();
            // info.playerinfoLV = saveData.playerinfoLV;


            // 4. 아이템 이름으로 실제 SO 에셋 로드 (Resources.Load 사용 예시)
            foreach (string itemName in saveData.slotItems)
            {
                EquipmentSO equipmentSO = Resources.Load<EquipmentSO>($"Items/{itemName}");

                //인벤토리에 slot 에 추가해야함.

                print("인벤토리에 slot 에 추가해야함.");

                this.Log($" EquipmentSO : {equipmentSO}");

                // if (so != null) info.slotItems.Add(so);
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
    public List<string> slotItems = new List<string>();
    public List<string> buttonItems = new List<string>();


}