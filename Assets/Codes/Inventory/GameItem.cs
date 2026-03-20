using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 게임 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "GearItem", menuName = "ScriptableObjects/GearItem", order = 1)]
public class GameItem : ScriptableObject
{
    [Header("Only gameplay")]
    public ItemRarity itemRarity; //등급
    public GearType gearType; //착용아이템 유형

    public int level = 1;

    [Header("%증가")]
    public float atack;  //공격력
    public float defence;    //방어력
    public float moveSpeed;    //움직임

    [Header("%감소")]
    public float atkSpeed;     //공격스피드


    public Sprite image; //아이템 이미지
    public int weight;  //가중치



    public GameItem(GameItem gameItem)
    {
        this.itemRarity = gameItem.itemRarity;
        this.gearType = gameItem.gearType;
        this.level = gameItem.level;
        this.atack = gameItem.atack;
        this.defence = gameItem.defence;
        this.moveSpeed = gameItem.moveSpeed;
        this.atkSpeed = gameItem.atkSpeed;
        this.image = gameItem.image;

    }


}

/// <summary>
/// 행동 유형 열거형
/// </summary>
public enum GearType
{
    Weapon, //무기
    Belt, //허리띠
    Gloves,//장갑
    Armor, //갑옷
    Necklace,//목거리
    Shoes//신발

}

//아이템등급
public enum ItemRarity
{
    Common, //보통템 흰색
    Rare, //레어템  파란색
    Epic, //에픽템  보라섹
    Legendary,//전설템  노락색
}

