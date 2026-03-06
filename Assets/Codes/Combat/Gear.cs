using UnityEngine;

/// <summary>
/// 장비(Attire/Gear) 클래스 - 방어구와 악세서리 아이템을 관리합니다.
/// </summary>
public class Gear : MonoBehaviour
{

    public ItemData.ItemType type; //장비 유형
    public float rate; //장비 효과 수치

    /// <summary>
    /// 장비 초기화 - 아이템 데이터에서 장비를 설정합니다.
    /// </summary>
    /// <param name="data">아이템 데이터</param>
    public void Init(ItemData data)
    {
        name = "Gear_" + data.itemID.ToString();
        transform.parent = GameManager.instance.player.transform; //플레이어 자식으로 설정
        transform.localPosition = Vector3.zero;                    //플레이어 위치로 이동

        type = data.itemType;
        rate = data.damages[0];

        //장비 효과 적용
        ApplayGear();

    }

    /// <summary>
    /// 장비 레벨업 - 효과 수치를 업데이트합니다.
    /// </summary>
    /// <param name="newRate">새로운 효과 수치</param>
    public void LevelUp(float newRate)
    {
        rate = newRate;
        ApplayGear();
    }

    /// <summary>
    /// 장비 효과 적용 - 장비 유형에 따라 적절한 효과를 적용합니다.
    /// </summary>
    void ApplayGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                //장갑 효과: 공격 속도 증가
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                //신발 효과: 이동 속도 증가
                SpeedUp();
                break;
        }
    }

    /// <summary>
    /// 공격 속도 증가 - 장갑 장비의 효과입니다.
    /// </summary>
    void RateUp()
    {
        //플레이어의 모든 무기 가져오기
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0: //근접 무기(삽)

                    float speed = 150 * Character.WeaponSpeed;

                    weapon.speed = speed + (speed * rate);

                    break;
                default: //원거리 무기
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;

            }
        }
    }

    /// <summary>
    /// 이동 속도 증가 - 신발 장비의 효과입니다.
    /// </summary>
    void SpeedUp()
    {
        // 기본 속도에서 상대적 증가로 변경
        float baseSpeed = 3f * Character.Speed;
        float bonusRate = 1f + rate;  // rate가 양수면 증가, 음수면 감소

        GameManager.instance.player.speed = baseSpeed * bonusRate;
    }
}
