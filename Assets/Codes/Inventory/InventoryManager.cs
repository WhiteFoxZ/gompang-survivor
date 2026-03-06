using UnityEngine;

/// <summary>
/// 인벤토리 관리자 클래스 - 인벤토리의 아이템을 관리합니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{

    const int maxStackItems = 5; //최대 스택 개수

    public InventorySlot[] _inventorySlots; //인벤토리 슬롯 배열

    public GameObject _inventoryItemPrefabs; //인벤토리 아이템 프리팹


    /// <summary>
    /// 아이템 추가 - 인벤토리에 아이템을 추가합니다.
    /// </summary>
    /// <param name="newItem">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddItem(GameItem newItem)
    {
        //먼저 기존 아이템과 스택 가능한지 확인
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemSlot = slot.GetComponentInChildren<InventoryItem>();

            //같은 아이템이고 스택 공간이 있으면
            if (itemSlot != null
            && itemSlot.gameItem == newItem
            && itemSlot.count < maxStackItems
            && itemSlot.gameItem.stackable)
            {
                //개수 증가
                itemSlot.count++;
                //표시 갱신
                itemSlot.reflushCount();
                return true;
            }
        }


        //빈 슬롯에 새로운 아이템 추가
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemSlot = slot.GetComponentInChildren<InventoryItem>();

            //빈 슬롯이면
            if (itemSlot == null)
            {
                //새 아이템 생성
                SpawnNewItem(newItem, slot);
                return true;
            }
        }

        //인벤토리가 가득 찼음
        return false;
    }

    /// <summary>
    /// 새 아이템 생성 - 지정된 슬롯에 아이템을 생성합니다.
    /// </summary>
    /// <param name="item">생성할 아이템 데이터</param>
    /// <param name="slot">생성할 슬롯</param>
    void SpawnNewItem(GameItem item, InventorySlot slot)
    {
        //프리팹からインスタンス生成
        GameObject newItemGameObj = Instantiate(_inventoryItemPrefabs, slot.transform);
        InventoryItem inventoryItem = newItemGameObj.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }



}
