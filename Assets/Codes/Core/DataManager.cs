using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 1.저장할 데이터가 존재
// 2.데이터를 제이슨으로 변환
// 3. 변환한 제이슨을 외부에 저장

public class DataManager : MonoBehaviour
{

    public static DataManager instance;  //싱글톤 인스턴스

    PlayerInfo playerInfo = new PlayerInfo();

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

        InventorySlot[] _inventorySlots = InventoryManager.instance._inventorySlots;

        InventoryItem itemSlot;

        EquipmentSO equipmentSO;

        foreach (InventorySlot slot in _inventorySlots)
        {

            itemSlot = slot.GetComponentInChildren<InventoryItem>();
            equipmentSO = itemSlot.gameItem;
            if (equipmentSO != null)
                playerInfo.addSlotList(equipmentSO);

        }


        string data = JsonUtility.ToJson(playerInfo);
        print(data);

        print(path);

        File.WriteAllText(path + filename, data);

    }

}


public class PlayerInfo
{

    public float playerinfoLV = 1f;
    public float playerinfoMaxLV = 60f;

    private List<EquipmentSO> slotList = new List<EquipmentSO>();   //장착중인아이템

    private List<EquipmentSO> buttonList = new List<EquipmentSO>();   //인벤토리에 있는아이템

    public void addSlotList(EquipmentSO equipmentSO)
    {
        slotList.Add(equipmentSO);
    }

    public void addButtonList(EquipmentSO equipmentSO)
    {
        slotList.Add(equipmentSO);
    }

    public void removeSlotList(EquipmentSO equipmentSO)
    {
        slotList.Remove(equipmentSO);
    }

    public void removeButtonList(EquipmentSO equipmentSO)
    {
        slotList.Remove(equipmentSO);
    }

    public List<EquipmentSO> GetSlotList()
    {
        return slotList;
    }

    public List<EquipmentSO> GetButtonList()
    {
        return buttonList;
    }


}