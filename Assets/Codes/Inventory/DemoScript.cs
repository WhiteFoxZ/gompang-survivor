using UnityEngine;

/// <summary>
/// 데모 스크립트 - 아이템 획득 테스트를 위한 스크립트입니다.
/// </summary>
public class DemoScript : MonoBehaviour
{

    public InventoryManager _inventoryManager; // 인벤토리 관리자 참조

    public EquipmentSO[] _gameItemsToPickup; // 테스트용 획득 아이템 배열


    /// <summary>
    /// 아이템 획득 - 지정된 ID의 아이템을 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="id">아이템 ID</param>
    public void pickupItem(int id)
    {
        //인벤토리에 아이템 추가 시도
        bool result = _inventoryManager.AddEquipItem(_gameItemsToPickup[id]);

        if (result)
        {
            this.Log("item added: " + _gameItemsToPickup[id].name);
        }
        else
        {
            this.Log("full inventory: ");
        }


    }


}
