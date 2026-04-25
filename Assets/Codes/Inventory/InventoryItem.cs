using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 아이템 클래스 - 인벤토리 슬롯의 아이템을 관리합니다.
/// </summary>
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("UI")]
    public Image itemImage; //아이템 이미지
    public Text _countText; //개수 텍스트
    public int count = 1;   //중복갯수

    [HideInInspector]
    public EquipmentSO gameItem; //게임 아이템 데이터


    [HideInInspector]
    public Transform parentAfterDrag; //드래그 후 부모 설정 (원래 위치로 복귀용)


    /// <summary>
    /// 아이템 초기화
    /// </summary>
    /// <param name="newitem">새로운 아이템 데이터</param>
    public void InitialiseItem(EquipmentSO newitem)
    {
        gameItem = newitem;
        itemImage.sprite = newitem.image;

        reflushCount();
    }

    /// <summary>
    /// 개수 텍스트 갱신
    /// </summary>
    public void reflushCount()
    {
        this.Log($" reflushCount  gearType : {gameItem.gearType} , count : {count}");
        //개수가 1보다 크면 텍스트 표시
        bool textActive = count > 1;
        _countText.gameObject.SetActive(textActive);

        if (count > 1)
        {
            _countText.text = count.ToString();
        }
        else
        {
            _countText.text = "";
        }
    }


    /// <summary>
    /// 드래그 시작 시 호출
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //레이캐스트 차단 비활성화 (드래agging 허용)
        itemImage.raycastTarget = false;
        //원래 부모 저장
        parentAfterDrag = transform.parent;
        //최상위 캔버스로 이동 (UI 최상단 표시)
        transform.SetParent(transform.root);

    }

    /// <summary>
    /// 드래그 중 호출
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnDrag(PointerEventData eventData)
    {
        //마우스 위치 추적
        transform.position = eventData.position;

    }

    /// <summary>
    /// 드래그 종료 시 호출
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //레이캐스트 차단 다시 활성화
        itemImage.raycastTarget = true;

        //유효한 슬롯에 드롭되지 않았으면 원래 부모로 복귀
        if (transform.parent == transform.root)
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero; //위치 초기화
        }


        // 저장 및 업데이트
        InventoryManager.instance.EquipSlotsUpdatePlayerInfo();
        InventoryManager.instance.InventorySlotsUpdatePlayerInfo();
        DataManager.instance.Save("OnEndDrag");


    }
}
