using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[System.Serializable]
public class PlayerData
{
    // 1. 기본 정보
    public string PlayerName = "";
    public float Level = 1;
    public float MaxLevel = 60;

    public int curr_stage = 1;
    public int next_stage = 1; //다음 스테이지


    // 2. 재화 관련
    public int Gold = 0;
    public int Diamond = 0;

    public int Energy = 20; // 초기 에너지
    public int MaxEnergy = 60; // 최대 에너지
    public DateTime LastEnergyUpdateTime = System.DateTime.UtcNow;// 에너지 회복 계산용 (UTC 사용)

    // 3. 진행도 및 스탯
    public int MaxStageReached = 0; // 최고 클리어 스테이지
    public Dictionary<string, int> Talents = new Dictionary<string, int>(); // 특성 ID와 강화 레벨

    // 4. 장비 , 인벤토리,SO 객체 대신 ID(이름) 리스트를 저장합니다.
    public List<EquipItem> equipItems = new List<EquipItem>();
    public List<EquipItem> inventoryItems = new List<EquipItem>();

    public EquipItem equipTotal;


    //합산된 장비정보 - 게임씬에서 사용

    public EquipItem GetTotalSlotStats()
    {
        equipTotal = new EquipItem();
        equipTotal.id = "Total";
        equipTotal.gearType = GearType.Total;

        // 1. 모든 장착 아이템의 능력치를 각 아이템 레벨 보정(1%당)을 적용해 합산
        foreach (var item in equipItems)
        {
            this.Log($" 장비 : {item}");

            equipTotal.atack += item.atack * item.count;
            equipTotal.defence += item.defence * item.count;
            equipTotal.moveSpeed += item.moveSpeed * item.count;
            equipTotal.atkSpeed += item.atkSpeed * item.count;
        }

        this.Log($" 장비 적용후합계 : {equipTotal} ");

        // 2. 최종 결과에 플레이어 레벨 보정(1%당)을 추가 적용
        // (예: 플레이어 50레벨 = 장비 총합의 1.5배)
        float playerLevelModifier = (this.Level * 0.01f);

        equipTotal.atack += playerLevelModifier;
        equipTotal.defence += playerLevelModifier;
        equipTotal.moveSpeed += playerLevelModifier;
        equipTotal.atkSpeed += playerLevelModifier;

        this.Log($"최종 결과에 플레이어 레벨 보정(1%당)을 추가 적용 : {equipTotal} ");

        return equipTotal;
    }


    public override string ToString()
    {
        if (equipItems == null || equipItems.Count == 0)
            return "슬롯이 비어 있습니다.";

        return string.Join("\n", equipItems);

    }

}