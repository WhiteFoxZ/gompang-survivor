using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 슬롯 클래스 - 아이템을 놓을 수 있는 슬롯을 나타냅니다.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        Inventory,
        Dron,
        Trash
    }

    public SlotType slotType; //착용아이템 유형

    /// <summary>
    /// 드롭 이벤트 처리 - 아이템을 슬롯에 놓았을 때 호출됩니다.
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 아이템이 유효한지 확인
        InventoryItem inventoryItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
        if (inventoryItem == null)
        {
            return;
        }

        // 휴지통 슬롯에 드롭되면 아이템 파괴
        if (slotType == SlotType.Trash)
        {
            Destroy(inventoryItem.gameObject);

            InventoryManager.instance.EquipSlotsUpdate();
            DataManager.instance.Save();

            return;
        }

        // 이미 슬롯에 아이템이 있는지 확인
        InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

        // 같은 아이템이고 스택이 가능한지 확인 (최대 60개까지 스택 가능)
        if (existingItem != null &&
            existingItem.gameItem == inventoryItem.gameItem &&
            inventoryItem.gameItem.count < InventoryManager.maxStackItems) // maxStackItems from InventoryManager
        {
            //개수 증가
            existingItem.gameItem.count++;
            //표시 갱신
            existingItem.reflushCount();

            // 드롭된 아이템 객체 파괴 (이미 카운트에 반영되었으므로)
            Destroy(inventoryItem.gameObject);

            // 저장 및 업데이트
            InventoryManager.instance.EquipSlotsUpdate();
            DataManager.instance.Save();

            return;
        }

        // 휴지통이 아닌 슬롯에만 빈 공간이 있으면 아이템을 이동
        if (slotType != SlotType.Trash && transform.childCount == 0)
        {
            inventoryItem.parentAfterDrag = transform;
        }

    }


}
