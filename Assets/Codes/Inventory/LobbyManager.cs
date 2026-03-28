using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerData saveData = DataManager.instance.LoadData();
        if (saveData != null)
        {
            EquipItem equipItem = saveData.GetTotalSlotStats();
            this.Log(equipItem.ToString());

            DataManager.instance.UpdateUI(saveData);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
