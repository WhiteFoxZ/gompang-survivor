using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

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
            inventoryItem.parentAfterDrag = transform;

            // OnEndDrag 로직을 먼저 실행 (UI 상태 초기화 및 데이터 저장)
            PointerEventData endDragData = new PointerEventData(EventSystem.current)
            {
                position = eventData.position,
                pointerDrag = inventoryItem.gameObject
            };
            inventoryItem.OnEndDrag(endDragData);

            Destroy(inventoryItem.gameObject);

            return;
        }

        // 이미 슬롯에 아이템이 있는지 확인
        InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

        // 같은 아이템이고 스택이 가능한지 확인 (최대 60개까지 스택 가능)
        if (existingItem != null &&
            existingItem.gameItem == inventoryItem.gameItem &&
            existingItem.count < InventoryManager.maxStackItems) // maxStackItems from InventoryManager
        {
            this.Log("아이템 중복 시작");


            //개수 증가
            int tmpCnt = existingItem.count + inventoryItem.count;

            this.Log($" {tmpCnt} = {existingItem.count} + {inventoryItem.count}");

            existingItem.count = tmpCnt;

            this.Log($" existingItem.count = {existingItem.count}");

            //표시 갱신
            existingItem.reflushCount();

            // 드롭된 아이템 객체 파괴 (이미 카운트에 반영되었으므로)
            Destroy(inventoryItem.gameObject);

            return;
        }

        // 휴지통이 아닌 슬롯에만 빈 공간이 있으면 아이템을 이동
        if (slotType != SlotType.Trash && transform.childCount == 0)
        {
            this.Log("휴지통이 아닌 슬롯에만 빈 공간이 있으면 아이템을 이동");

            inventoryItem.parentAfterDrag = transform;
        }

    }


}
