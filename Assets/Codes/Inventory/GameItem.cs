using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// 게임 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "GearItem", menuName = "ScriptableObjects/GearItem", order = 1)]
public class GameItem : ScriptableObject
{
    [Header("Only gameplay")]
    public TileBase tile; //타일
    public ItemType type; //아이템 유형
    public ActionType actionType; //행동 유형
    public Vector2Int range = new Vector2Int(5, 4); //범위


    [Header("Only UI")]
    public bool stackable = true; //스택 가능 여부

    [Header("Both")]
    public Sprite image; //아이템 이미지


}

/// <summary>
/// 행동 유형 열거형
/// </summary>
public enum ActionType
{
    Dig, //땅 파기
    Mine //채굴

}

/// <summary>
/// 아이템 유형 열거형
/// </summary>
public enum ItemType
{
    BuildingBlock, //건축 블록
    Tool //도구
}
