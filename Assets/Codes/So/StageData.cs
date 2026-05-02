using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[System.Serializable]
public class StageData
{

    // 스테이지ID 스테이지이름  스테이지설명 스테이지_적ID

    public int id; //스테이지ID
    public string stageName; //스테이지이름
    public string stageDesc; //스테이지설명
    public int[] enemyId; //SpawnDataId
    public int bossCount;    //보스스폰마리수



    public StageData(int id, string stageName, string stageDesc, int[] enemyId, int bossCount)
    {
        this.id = id;
        this.stageName = stageName;
        this.stageDesc = stageDesc;
        this.enemyId = enemyId;
        this.bossCount = bossCount;
    }

    // EquipItem의 정보를 문자열로 반환
    public override string ToString()
    {
        return $"[id: {id}, stageName :{stageName}, stageDesc: {stageDesc} [{string.Join(", ", this.enemyId)}] ]";
    }

}

