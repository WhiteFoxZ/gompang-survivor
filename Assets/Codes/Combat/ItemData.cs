using UnityEngine;
using System;
using System.IO;

/// <summary>
/// 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    //아이템 유형 열거형
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    public ItemType itemType; //아이템 유형
    public int itemID; //아이템 ID
    public string itemName; //아이템 이름

    [TextArea(3, 10)]
    public string itemDesc; //아이템 설명
    public Sprite itemIcon; //아이템 아이콘


    [Header("# Base Data")]
    public float baseDamage; //기본 데미지
    public int baseCount; //기본 개수

    [Header("# Level Data")]
    public float[] damages; //레벨별 데미지
    public int[] counts; //레벨별 개수

    [Header("# Weapon")]
    public GameObject itemPrefab; //무기 프리팹
    public Sprite hand; //손에 들 이미지

    /// <summary>
    /// JSON 파일에서 아이템 데이터를 로드합니다.
    /// </summary>
    public static ItemData[] LoadFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ItemData");
        if (jsonFile == null)
        {
            Debug.LogError("ItemData.json 파일을 찾을 수 없습니다!");
            return null;
        }

        string jsonString = jsonFile.text;
        return JsonUtility.FromJson<JsonItemDataWrapper>("{\"items\":" + jsonString + "}").items;
    }

    /// <summary>
    /// 이 아이템 데이터의 값을 JSON 형식으로 반환합니다.
    /// </summary>
    public string ToJson()
    {
        JsonItemData jsonItem = new JsonItemData
        {
            itemType = (int)itemType,
            itemID = itemID,
            itemName = itemName,
            itemDesc = itemDesc,
            baseDamage = baseDamage,
            baseCount = baseCount,
            damages = damages,
            counts = counts
        };
        return JsonUtility.ToJson(jsonItem, true);
    }
}

/// <summary>
/// JSON 직렬화를 위한 래퍼 클래스
/// </summary>
[Serializable]
public class JsonItemDataWrapper
{
    public ItemData[] items;
}

/// <summary>
/// JSON 변환용 아이템 데이터 클래스
/// </summary>
[Serializable]
public class JsonItemData
{
    public int itemType;
    public int itemID;
    public string itemName;
    public string itemDesc;
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;
}
