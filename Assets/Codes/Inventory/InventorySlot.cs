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

            DataManager.instance.InventorySlots();
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
