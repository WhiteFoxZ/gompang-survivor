using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public PlayerData playerData;

    void Start()
    {
        playerData = DataManager.instance.LoadData();
        if (playerData != null)
        {
            EquipItem equipItem = playerData.GetTotalSlotStats();
            this.Log(equipItem.ToString());

            DataManager.instance.UpdateUI(playerData);
        }

    }


}
