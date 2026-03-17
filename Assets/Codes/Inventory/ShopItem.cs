using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// 게임 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/ShopItem", order = 1)]
public class ShopItem : ScriptableObject
{

    public ShopItemType shopItemType; //상점 아이템 유형
    public PayType payType; //결제 유형
    public int price; //가격
    public int itemCnt; //충천갯수    




}

/// <summary>
/// 상점아이템타입
/// </summary>
public enum ShopItemType
{
    Energy, //에너지
    Coin, //코인
    Diamond,    //다이아
    ItemBoxSmall, //작은 아이템 박스
    ItemBoxMedium, //중간 아이템 박스
    ItemBoxLarge //큰 아이템 박스
}


public enum PayType
{
    AD, //광고
    COIN, //코인
    DIAMOND,//다이아몬드
    PAY //코인
}

