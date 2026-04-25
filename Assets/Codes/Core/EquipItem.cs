using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[System.Serializable]
public class EquipItem
{
    public string id;
    public ItemRarity itemRarity; //등급
    public GearType gearType; //착용아이템 유형

    public float atack;  //공격력
    public float defence;    //방어력
    public float moveSpeed;    //움직임
    public float atkSpeed;     //공격스피드

    public int count = 1;

    public EquipItem()
    {
    }


    public EquipItem(EquipmentSO gameItem, int count)
    {
        this.id = gameItem.id;
        this.itemRarity = gameItem.itemRarity;
        this.gearType = gameItem.gearType;
        this.atack = gameItem.atack;
        this.defence = gameItem.defence;
        this.moveSpeed = gameItem.moveSpeed;
        this.atkSpeed = gameItem.atkSpeed;
        this.count = count;
    }

    // EquipItem의 정보를 문자열로 반환
    public override string ToString()
    {
        return $"[itemRarity: {itemRarity}, gearType :{gearType}, count: {count}]";
    }

}

