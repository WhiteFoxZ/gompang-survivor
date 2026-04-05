using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


public class GoogleSpreadSheetManager : MonoBehaviour
{

    public static GoogleSpreadSheetManager instance;  //싱글톤 인스턴스

    //다운로드 유형 열거형
    public enum DownType { Item, Exp, Map, Equip, ENEMY }

    const string ITEM_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&range=A2:J";

    //게임시간,MAX_STAGE
    const string EXP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1514884558&range=A2:J";


    const string MAP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=809858262&range=A2:E";


    const string EQUIP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1723476130&range=A2:H";



    //적 데이터 URL (예시)
    const string ENEMY_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1674237518&range=A2:F";





    [Header("게임 Item Data")]
    public ItemData[] itemDatas; //아이템 데이터 참조 

    [Header("장비 Item Data")]
    public EquipmentSO[] equipmentDatas; //장비 데이터 참조

    void Awake()
    {
        //싱글톤 인스턴스 설정
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }




    public IEnumerator DownloadItemData(DownType type)
    {
        String URL;
        if (type == DownType.Item)
        {
            URL = ITEM_URL;
        }
        else if (type == DownType.Exp)
        {
            URL = EXP_URL;
        }
        else if (type == DownType.Equip)
        {
            URL = EQUIP_URL;
        }
        else if (type == DownType.Map)
        {
            URL = MAP_URL;
        }
        else if (type == DownType.ENEMY)
        {

            URL = ENEMY_URL; //적 데이터 URL로 변경 필요
        }
        else
        {
            Debug.LogError("알 수 없는 아이템 유형입니다.");
            yield break;
        }



        //구글 스프레드시트에서 아이템 데이터 다운로드 (테스트용)
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                // Debug.Log("아이템 데이터 다운로드 성공!");
                //다운로드한 데이터를 파싱하여 itemDatas 배열에 저장하는 로직 추가 필요
                string data = www.downloadHandler.text;
                // Debug.Log("다운로드한 데이터: " + data);

                if (type == DownType.Item)
                {
                    SetItemSO(data); //데이터 파싱 및 아이템 데이터 설정
                }
                else if (type == DownType.Equip)
                {
                    SetEquipmentSO(data);
                }
                else if (type == DownType.Exp)
                {
                    SetExp(data);
                }
                else if (type == DownType.Map)
                {
                    //맵 데이터 파싱 및 설정 로직 추가 필요
                    SetMap(data);
                }
                else if (type == DownType.ENEMY)
                {
                    //적 데이터 파싱 및 설정 로직 추가 필요
                    SetEnemy(data);
                }


            }
            else
            {
                Debug.LogError("아이템 데이터 다운로드 실패: " + www.error);
            }
        }
    }

    void SetItemSO(string tsv)
    {
        //item type	item id	이름(name)	item Desc	공격력(base Damage)	Base Count	LevelUp Damage	LevelUp Counts	knockBack	knockBackRate

        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int columnSize = row[0].Split('\t').Length;

        this.Log(" item 다운갯수 : " + rowSize);
        this.Log(" itemDatas : " + itemDatas.Length);

        if (itemDatas.Length != rowSize)
        {
            Debug.LogError("itemDatas 배열 크기와 다운로드한 데이터의 행 수가 일치하지 않습니다.");
            return;
        }

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');
            itemDatas[i].itemType = (ItemData.ItemType)System.Enum.Parse(typeof(ItemData.ItemType), column[0]);
            itemDatas[i].itemID = int.Parse(column[1]);
            itemDatas[i].itemName = column[2];
            itemDatas[i].itemDesc = column[3].Replace("\\n", "\n");


            itemDatas[i].baseDamage = float.Parse(column[4]);
            itemDatas[i].baseCount = int.Parse(column[5]);
            itemDatas[i].damages = Array.ConvertAll(column[6].Split(','), float.Parse);
            itemDatas[i].counts = Array.ConvertAll(column[7].Split(','), int.Parse);
            itemDatas[i].knockBack = float.Parse(column[8]);
            itemDatas[i].knockBackRate = float.Parse(column[9]);

        }


        //itemDatas 에 정보를 로그로 출력 (테스트용)
        // foreach (var item in itemDatas)
        // {
        //     Debug.Log($" 유형: {item.itemType},아이템: {item.itemName},설명: {item.itemDesc}, 데미지: {item.baseDamage}, 개수: {item.baseCount}, 레벨업 데미지: {string.Join(",", item.damages)}, 레벨업 개수: {string.Join(",", item.counts)}, 넉백: {item.knockBack}, 넉백확률: {item.knockBackRate}");
        // }

    }


    void SetEquipmentSO(string tsv)
    {

        //GearType	Id	atack	defence	moveSpeed	atkSpeed	ItemRarity	WEIGHT	DESC
        //BodyArmor   1   0   0.01    0   0   Common  1000    방어력 { 0}% 증가


        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

        this.Log(" 장비 다운갯수 : " + rowSize);
        this.Log(" equipmentDatas : " + equipmentDatas.Length);


        if (equipmentDatas.Length != rowSize)
        {
            Debug.LogError("equipmentDatas 배열 크기와 다운로드한 데이터의 행 수가 일치하지 않습니다.");
            return;
        }

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            // this.Log($" gearType : {column[0]}, id : {column[1]}, atack : {column[2]}, defence : {column[3]}, moveSpeed : {column[4]}, atkSpeed : {column[5]}, itemRarity : {column[6]}, weight : {column[7]}");

            equipmentDatas[i].gearType = (GearType)System.Enum.Parse(typeof(GearType), column[0]);
            equipmentDatas[i].id = column[1];
            equipmentDatas[i].atack = float.Parse(column[2]);
            equipmentDatas[i].defence = float.Parse(column[3]);
            equipmentDatas[i].moveSpeed = float.Parse(column[4]);
            equipmentDatas[i].atkSpeed = float.Parse(column[5]);
            equipmentDatas[i].itemRarity = (ItemRarity)System.Enum.Parse(typeof(ItemRarity), column[6]);
            equipmentDatas[i].weight = int.Parse(column[7]);


        }


        //equipmentDatas 에 정보를 로그로 출력 (테스트용)
        // foreach (var item in equipmentDatas)
        // {
        //     Debug.Log($" 장비 유형: {item.gearType},아이템: {item.id},설명: {item.desc}, 데미지: {item.atack}, 방어력: {item.defence}, 이동속도: {item.moveSpeed}, 공격속도: {item.atkSpeed}, 희귀도: {item.itemRarity}, 가중치: {item.weight}");
        // }

    }


    void SetExp(string tsv)
    {
        //item type	item id	이름(name)	item Desc	공격력(base Damage)	Base Count	LevelUp Damage	LevelUp Counts	knockBack	knockBackRate   GAME_TIME	MAX_STAGE	NEXT_EXP	MAX_HEALTH	ENEMY_speedRate	ENENy_healthRate	ENENY_attackRate	ENENY_spawnTimeRate
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int columnSize = row[0].Split('\t').Length;

        int maxGameTime = 0, maxGameStage = 0, maxHealth = 0;

        float ENEMY_speedRate = 0,
            ENENy_healthRate = 0,
            ENENY_attackRate = 0,
            ENENY_spawnTimeRate = 0;

        int[] extExp = new int[rowSize];

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            maxGameTime = int.Parse(column[0]);
            maxGameStage = int.Parse(column[1]);
            extExp = Array.ConvertAll(column[2].Split(','), int.Parse);
            maxHealth = int.Parse(column[3]);
            ENEMY_speedRate = float.Parse(column[4]);
            ENENy_healthRate = float.Parse(column[5]);
            ENENY_attackRate = float.Parse(column[6]);
            ENENY_spawnTimeRate = float.Parse(column[7]);


            // Debug.Log($"maxGameTime: {maxGameTime}, maxGameStage: {maxGameStage}, extExp: {string.Join(",", extExp)} maxHealth: {maxHealth}, ENEMY_speedRate: {ENEMY_speedRate}, ENENy_healthRate: {ENENy_healthRate}, ENENY_attackRate: {ENENY_attackRate}, ENENY_spawnTimeRate: {ENENY_spawnTimeRate}");
        }

        GameManager.instance.SetMaxGameTime(maxGameTime);
        GameManager.instance.SetMaxGameStage(maxGameStage);
        GameManager.instance.SetExtExp(extExp);
        GameManager.instance.maxHealth = maxHealth;

        Spawner.speedRate = ENEMY_speedRate;
        Spawner.healthRate = ENENy_healthRate;
        Spawner.attackRate = ENENY_attackRate;
        Spawner.spawnTimeRate = ENENY_spawnTimeRate;

    }


    void SetMap(string tsv)
    {
        //맵 데이터 파싱 및 설정 로직 추가 필요
    }


    void SetEnemy(string tsv)
    {
        //no	spawnTime	spriteType	health	speed	attack
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int columnSize = row[0].Split('\t').Length;


        float spawnTime = 0f;
        int spriteType = 0;
        int health = 0;
        float speed = 0f;
        float attack = 0f;

        Spawner._spawnDatas = new SpawnData[rowSize]; //스폰 데이터 배열 초기화''

        this.Log($" Spawner._spawnDatas : {Spawner._spawnDatas.Length}");

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            bool boss = bool.Parse(column[0]);
            spawnTime = float.Parse(column[1]);
            spriteType = int.Parse(column[2]);
            health = int.Parse(column[3]);
            speed = float.Parse(column[4]);
            attack = float.Parse(column[5]);

            Spawner._spawnDatas[i] = new SpawnData
            {
                boss = boss,
                spawnTime = spawnTime,
                spriteType = spriteType,
                health = health,
                speed = speed,
                attack = attack
            };

        }

        //itemDatas 에 정보를 로그로 출력 (테스트용)
        foreach (var item in Spawner._spawnDatas)
        {
            Debug.Log($" spriteType: {item.spriteType},health: {item.health},speed: {item.speed}, attack: {item.attack}, spawnTime: {item.spawnTime}");
        }

    }


}
