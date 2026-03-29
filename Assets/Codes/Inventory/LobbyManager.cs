using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public PlayerData playerData = null;

    void Start()
    {
        playerData = DataManager.instance.LoadData();
        if (playerData != null)
        {
            // EquipItem equipItem = playerData.GetTotalSlotStats();
            // this.Log(equipItem.ToString());

            DataManager.instance.UpdateUI(playerData);
        }
        else
        {
            playerData = new PlayerData(); // 새로운 PlayerData 인스턴스 생성

            this.Log($" MaxLevel : {playerData.MaxLevel}");
            this.Log($" Level : {playerData.Level}");

            Debug.LogWarning("PlayerData를 불러오는데 실패했습니다. 새로운 PlayerData가 생성되었습니다.");
        }

    }


}
