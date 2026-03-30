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

        //슬롯이 비어있는지 확인
        if (transform.childCount == 0)
        {
            //드래그된 아이템 가져오기
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

            //새 부모 설정
            inventoryItem.parentAfterDrag = transform;
        }
    }


}
