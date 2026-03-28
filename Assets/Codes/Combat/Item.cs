using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 클래스 - 레벨업 시 선택지 아이템을 표시하고 관리합니다.
/// </summary>
public class Item : MonoBehaviour
{
    public ItemData data; //아이템 데이터
    public int level; //현재 레벨
    public Weapon weapon; //무기 컴포넌트
    public Gear gear; //장비 컴포넌트

    Image icon; //아이템 아이콘
    Text itemLevel; //레벨 텍스트
    Text itemName; //이름 텍스트
    Text itemDesc; //설명 텍스트




    /// <summary>
    /// 시작 시 호출 - UI 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        //자식 오브젝트에서 이미지 컴포넌트 가져오기 (아이콘)
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        //자식 오브젝트에서 텍스트 컴포넌트 가져오기
        Text[] items = GetComponentsInChildren<Text>();
        itemLevel = items[0];
        itemName = items[1];
        itemDesc = items[2];
        itemName.text = data.itemName;

    }

    /// <summary>
    /// 활성화 시 호출 - 아이템 정보 업데이트
    /// </summary>
    void OnEnable()
    {
        // this.Log("OnEnable");

        //레벨 텍스트 업데이트
        itemLevel.text = "Lv." + (level).ToString();

        //아이템 유형에 따라 설명 업데이트
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //무기: 데미지와 개수 표시
                itemDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.MissilePack:

                //장비: 효과 수치만 표시
                itemDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;

            case ItemData.ItemType.StamPack:
                //장비: 효과 수치만 표시
                itemDesc.text = string.Format(data.itemDesc, data.baseDamage);
                break;

            default:
                //기타: 설명만 표시
                itemDesc.text = string.Format(data.itemDesc, data.baseDamage);
                break;
        }

    }

    /// <summary>
    /// 아이템 클릭 시 호출 - 아이템 획득/레벨업 로직
    /// </summary>
    public void OnClickItem()
    {
        //아이템 유형에 따라 처리
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            case ItemData.ItemType.Missile:

                //무기 아이템 처리
                if (level == 0)
                {
                    //첫 획득: 새 무기 생성
                    GameObject newWebon = new GameObject();
                    weapon = newWebon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    //레벨업: 다음 레벨 데이터 계산
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);

                }

                //레벨업 처리
                levelup();

                break;


            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.MissilePack:

                //장비 아이템 처리
                if (level == 0)
                {
                    //첫 획득: 새 장비 생성
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    //레벨업: 다음 레벨 효과 수치 적용
                    float nextRate = data.damages[level];

                    gear.LevelUp(nextRate);

                }

                //레벨업 처리
                levelup();


                break;

            case ItemData.ItemType.Heal:
                //체력 회복: 최대 체력으로 복원
                GameManager.instance.health = GameManager.instance.maxHealth;

                break;

            case ItemData.ItemType.StamPack:
                //스팀팩 사용 시 체력 30% 감소
                GameManager.instance.health -= GameManager.instance.health * 0.3f;

                //StampPack 효과: 이동속도 100% 증가, 총알 간격 100% 증가
                //지속 시간은 아이템의 damage 값으로 설정
                GameManager.instance.player.StartCoroutine(
                    GameManager.instance.player.ApplyStampEffect(data.baseDamage));
                break;

        }

        //레벨업 UI 숨기기
        if (GameManager.instance.uiLevelup != null)
        {
            GameManager.instance.uiLevelup.Hide();
        }

    }


    /// <summary>
    /// 레벨업 처리 - 레벨 증가 및 최대 레벨 체크
    /// </summary>
    void levelup()
    {
        //레벨이 최대 레벨보다 작으면 증가
        if (level < data.damages.Length)
        {
            level++;

            //최대 레벨 도달 시 버튼 비활성화
            if (level == data.damages.Length)
            {
                GetComponent<Button>().interactable = false;
            }
        }

    }

}
