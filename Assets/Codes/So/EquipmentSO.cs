using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 게임 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
///
// Common(일반): 가중치 60 (매우 흔함)
// Rare (희귀): 가중치 30 (조금 드묾)
// Epic (에픽): 가중치 8 (매우 드묾)
// Legendary (전설): 가중치 2 (최상위 희귀도)
// 합계 : 100
/// </summary>
[CreateAssetMenu(fileName = "EquipmentSO", menuName = "ScriptableObjects/EquipmentSO", order = 1)]
public class EquipmentSO : ScriptableObject
{
    [Header("Only gameplay")]
    public string id;
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

    [Header("확율")]
    public int weight;  //가중치


    [Header("설명")]
    public int desc;  //가중치




    public EquipmentSO(EquipmentSO gameItem)
    {
        this.id = gameItem.id;
        this.itemRarity = gameItem.itemRarity;
        this.gearType = gameItem.gearType;
        this.level = gameItem.level;
        this.atack = gameItem.atack;
        this.defence = gameItem.defence;
        this.moveSpeed = gameItem.moveSpeed;
        this.atkSpeed = gameItem.atkSpeed;
        this.image = gameItem.image;
        this.weight = gameItem.weight;
        this.desc = gameItem.desc;
    }


}

/// <summary>
/// 행동 유형 열거형
/// </summary>
public enum GearType
{
    Dron, //드론
    Ring,
    Belt, //허리띠
    Gloves,//장갑
    BodyArmor, //갑옷
    Necklace,//목거리
    Shoes,//신발
    Pants
}

//아이템등급
public enum ItemRarity
{
    Common, //보통템 흰색
    Rare, //레어템  파란색
    Epic, //에픽템  보라섹
    Legendary,//전설템  노락색
}


