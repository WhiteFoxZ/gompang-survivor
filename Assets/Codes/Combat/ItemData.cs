using UnityEngine;

/// <summary>
/// 게임내에서 플레이어에 경험치 증가에 따른 무기아이템 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    //아이템 유형 열거형
    public enum ItemType { Melee, Range, Glove, Shoe, Heal, StamPack, Missile, MissilePack }

    [Header("# Main Info")]
    public ItemType itemType; //아이템 유형
    public int itemID; //아이템 ID
    public string itemName; //아이템 이름

    [TextArea(3, 10)]
    public string itemDesc; //아이템 설명
    public Sprite itemIcon; //아이템 아이콘


    [Header("# Base Data")]
    public float baseDamage; //기본 데미지
    public int baseCount; //기본 

    [Header("# knockBack Data")]
    public float knockBack; //넉백 세기

    [Header("# knockBackRate Data")]
    public float knockBackRate; //넉백 확율


    [Header("# Level Data")]
    public float[] damages; //레벨별 데미지
    public int[] counts; //레벨별 개수



    [Header("# Weapon")]
    public GameObject itemPrefab; //무기 프리팹
    public Sprite hand; //손에 들 이미지


}
