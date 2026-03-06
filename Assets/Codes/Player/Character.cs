using UnityEngine;

/// <summary>
/// 캐릭터 클래스 - 플레이어 캐릭터의 스탯을 정의합니다.
/// </summary>
public class Character : MonoBehaviour
{

    /// <summary>
    /// 이동 속도 - 플레이어 ID에 따라 다른 속도 반환
    /// </summary>
    public static float Speed
    {
        get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    }



    /// <summary>
    /// 무기 속도 - 플레이어 ID에 따라 다른 속도 반환
    /// </summary>
    public static float WeaponSpeed
    {
        get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    }

    /// <summary>
    /// 무기 공격 속도 - 플레이어 ID에 따라 다른 속도 반환
    /// </summary>
    public static float WeaponRate
    {
        get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }

    }

    /// <summary>
    /// 데미지 - 플레이어 ID에 따라 다른 데미지 반환
    /// </summary>
    public static float Damage
    {
        get { return GameManager.instance.playerId == 2 ? 1.2f : 1f; }
    }

    /// <summary>
    /// 투사체 개수 - 플레이어 ID에 따라 다른 개수 반환
    /// </summary>
    public static int Count
    {
        get { return GameManager.instance.playerId == 3 ? 1 : 0; }
    }

}
