using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;

[Serializable]
public class ItemDataWrapper
{
    public string itemType;
    public int itemID;
    public string itemName;
    public string itemDesc;
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;
    public float knockBack;
    public float knockBackRate;
    public string itemIconName;
    public string itemPrefabName;

    public ItemDataWrapper(ItemDataSO data)
    {
        itemType = data.itemType.ToString();
        itemID = data.itemID;
        itemName = data.itemName;
        itemDesc = data.itemDesc;
        baseDamage = data.baseDamage;
        baseCount = data.baseCount;
        damages = data.damages;
        counts = data.counts;
        knockBack = data.knockBack;
        knockBackRate = data.knockBackRate;
        itemIconName = data.itemIcon != null ? data.itemIcon.name : "";
        itemPrefabName = data.itemPrefab != null ? data.itemPrefab.name : "";
    }
}

[Serializable]
public class EquipmentDataWrapper
{
    public string gearType;
    public string id;
    public float atack;
    public float defence;
    public float moveSpeed;
    public float atkSpeed;
    public string itemRarity;
    public int weight;
    public int desc;

    public EquipmentDataWrapper(EquipmentSO data)
    {
        gearType = data.gearType.ToString();
        id = data.id;
        atack = data.atack;
        defence = data.defence;
        moveSpeed = data.moveSpeed;
        atkSpeed = data.atkSpeed;
        itemRarity = data.itemRarity.ToString();
        weight = data.weight;
        desc = data.desc;
    }
}

[Serializable]
public class StageDataWrapper
{
    public int id;
    public string stageName;
    public string stageDesc;
    public int[] enemyId;
    public int bossCount;

    public StageDataWrapper(StageData data)
    {
        id = data.id;
        stageName = data.stageName;
        stageDesc = data.stageDesc;
        enemyId = data.enemyId;
        bossCount = data.bossCount;
    }
}

[Serializable]
public class SpawnDataWrapper
{
    public int id;
    public int boss;
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    public float attack;

    public SpawnDataWrapper(SpawnData data)
    {
        id = data.id;
        boss = data.boss;
        spawnTime = data.spawnTime;
        spriteType = data.spriteType;
        health = data.health;
        speed = data.speed;
        attack = data.attack;
    }
}

[Serializable]
public class ItemDataList
{
    public ItemDataWrapper[] items;
}

[Serializable]
public class EquipmentDataList
{
    public EquipmentDataWrapper[] equipments;
}

[Serializable]
public class StageDataList
{
    public StageDataWrapper[] stages;
}

[Serializable]
public class SpawnDataList
{
    public SpawnDataWrapper[] spawnDatas;
}


public class ExpDataWrapper
{
    public float maxGameTime;
    public int maxGameStage;
    public int[] nextExp;
    public float maxHealth;
    public float ENEMY_speedRate;
    public float ENENy_healthRate;
    public float ENENY_attackRate;
    public float ENENY_spawnTimeRate;

    public ExpDataWrapper(float maxGameTime, int maxGameStage, int[] nextExp, float maxHealth, float enemySpeedRate, float enemyHealthRate, float enemyAttackRate, float enemySpawnTimeRate)
    {
        this.maxGameTime = maxGameTime;
        this.maxGameStage = maxGameStage;
        this.nextExp = nextExp;
        this.maxHealth = maxHealth;
        this.ENEMY_speedRate = enemySpeedRate;
        this.ENENy_healthRate = enemyHealthRate;
        this.ENENY_attackRate = enemyAttackRate;
        this.ENENY_spawnTimeRate = enemySpawnTimeRate;
    }
}

/**
1. 기본 로직 설계 (Flow)버전 체크: 게임 시작 시 서버의 최신 데이터 버전(또는 타임스탬프)을 확인합니다
  .분기점:로컬 버전 < 서버 버전: 서버에서 새 JSON 데이터를 다운로드 → 로컬 파일 갱신 → SO에 데이터 주입
  .로컬 버전 == 서버 버전: 저장된 로컬 파일을 로드 → SO에 데이터 주입
  .데이터 바인딩: 로드된 데이터를 SO의 변수들에 할당하여 게임 내에서 사용
**/

public class GoogleSpreadSheetManager : MonoBehaviour
{

    public static GoogleSpreadSheetManager instance;  //싱글톤 인스턴스

    static string ITEM = null;
    static string ENEMY = null;
    static string EXP = null;
    static string MAP = null;
    static string EQUIP = null;


    //다운로드 유형 열거형
    public enum DownType { Item, Exp, Map, Equip, Enemy, Sheet }

    const string ITEM_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&range=A2:J";

    //게임시간,MAX_STAGE
    const string EXP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1514884558&range=A2:J";

    const string MAP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=809858262&range=A2:E";

    const string EQUIP_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1723476130&range=A2:H";

    //적 데이터 URL (예시)
    const string ENEMY_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=1674237518&range=A2:G";

    const string SHEET_URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&gid=104575566&range=A2:E";

    private string SpreadSheetLastDownloadDate = "SpreadSheetLastDownloadDate";

    [Header("게임 Item Data")]
    public ItemDataSO[] itemDatas; //아이템 데이터 참조 

    [Header("장비 Item Data")]
    public EquipmentSO[] equipmentDatas; //장비 데이터 참조

    [Header("Stage Data")]
    public StageData[] stages;

    void Awake()
    {
        //싱글톤 인스턴스 설정
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

    }

    public IEnumerator DownloadItemData(DownType type)
    {
        //가장먼저 전체 데이터 다운로드 여부 체크 (DownType.Sheet) - 오늘 이미 다운로드 했는지 체크


        if (!CanDownloadToday(type) && type != DownType.Sheet)
        {
            this.Log($" objname : {type} 오늘은 이미 다운로드 했습니다.");
            yield break;
        }
        else
        {
            String URL;
            if (type == DownType.Item && ITEM.Equals("Y"))
            {
                URL = ITEM_URL;
            }
            else if (type == DownType.Exp && EXP.Equals("Y"))
            {
                URL = EXP_URL;
            }
            else if (type == DownType.Equip && EQUIP.Equals("Y"))
            {
                URL = EQUIP_URL;
            }
            else if (type == DownType.Map && MAP.Equals("Y"))
            {
                URL = MAP_URL;
            }
            else if (type == DownType.Enemy && ENEMY.Equals("Y"))
            {
                URL = ENEMY_URL;
            }
            else if (type == DownType.Sheet)
            {
                URL = SHEET_URL;
            }
            else
            {
                this.Log($" {type} 다운로드 불필요 또는 URL이 설정되지 않았습니다.");

                switch (type)
                {
                    case DownType.Item:
                        LoadItemDatas();
                        break;
                    case DownType.Equip:
                        LoadEquipmentDatas();
                        break;
                    case DownType.Exp:
                        LoadExp();
                        break;
                    case DownType.Map:
                        LoadStages();
                        break;
                    case DownType.Enemy:
                        LoadEnemy();
                        break;

                }

                yield break;
            }

            this.Log($" {type} 다운로드 시작");

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

                    switch (type)
                    {
                        case DownType.Item:
                            SetItemSO(data);
                            SaveItemDatas();
                            break;
                        case DownType.Equip:
                            SetEquipmentSO(data);
                            SaveEquipmentDatas();
                            break;
                        case DownType.Exp:
                            SetExp(data);
                            SaveExp();
                            break;
                        case DownType.Map:
                            SetMap(data);
                            SaveStages();
                            break;
                        case DownType.Enemy:
                            SetEnemy(data);
                            SaveEnemy();
                            break;
                        case DownType.Sheet:
                            SetSheet(data);
                            SaveCurrentDate(type);
                            break;
                    }
                }
                else
                {
                    Debug.LogError("아이템 데이터 다운로드 실패: " + www.error);
                }
            }
        }
    }

    void SetItemSO(string tsv)
    {
        //item type	item id	이름(name)	item Desc	공격력(base Damage)	Base Count	LevelUp Damage	LevelUp Counts	knockBack	knockBackRate

        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

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
            itemDatas[i].itemType = (ItemDataSO.ItemType)System.Enum.Parse(typeof(ItemDataSO.ItemType), column[0]);
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

        this.Log($" 다운갯수 : {rowSize} , {equipmentDatas.Length} ");


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

        GameManager.instance.maxGameTime = maxGameTime;
        GameManager.instance.maxStage = maxGameStage;
        GameManager.instance.nextExp = extExp;
        GameManager.instance.maxHealth = maxHealth;

        Spawner.speedRate = ENEMY_speedRate;
        Spawner.healthRate = ENENy_healthRate;
        Spawner.attackRate = ENENY_attackRate;
        Spawner.spawnTimeRate = ENENY_spawnTimeRate;


        //레벨당 시간 계산
        Spawner.levelTime = GameManager.instance.maxGameTime / Spawner._spawnDatas.Length;

        // this.Log(" 게임정보 다운로드 ******* ");

        // this.Log($" levelTime  : {Spawner.levelTime}  maxGameTime : {GameManager.instance.maxGameTime}  spawnDatasLength : {Spawner._spawnDatas.Length} ");
        // this.Log($" maxHealth  : {GameManager.instance.maxHealth}   ");
    }

    void SetMap(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

        stages = new StageData[rowSize];

        this.Log(" stages 다운갯수 : " + rowSize);

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            stages[i] = new StageData(
                int.Parse(column[0]),
                column[1],
                column[2],
                Array.ConvertAll(column[3].Split(','), int.Parse),
                int.Parse(column[4])
            );
        }
    }

    void SetEnemy(string tsv)
    {
        //id    boss	spawnTime	spriteType	health	speed	attack
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

        int id = 0;
        int boss = 0;
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

            id = int.Parse(column[0]);
            boss = int.Parse(column[1]);
            spawnTime = float.Parse(column[2]);
            spriteType = int.Parse(column[3]);
            health = int.Parse(column[4]);
            speed = float.Parse(column[5]);
            attack = float.Parse(column[6]);

            Spawner._spawnDatas[i] = new SpawnData
            {
                id = id,
                boss = boss,
                spawnTime = spawnTime,
                spriteType = spriteType,
                health = health,
                speed = speed,
                attack = attack
            };

        }

        //itemDatas 에 정보를 로그로 출력 (테스트용)
        // foreach (var item in Spawner._spawnDatas)
        // {
        //     Debug.Log($"다운로드 boss: {item.boss}, spriteType: {item.spriteType},health: {item.health},speed: {item.speed}, attack: {item.attack}, spawnTime: {item.spawnTime}");
        // }

    }

    void SetSheet(string tsv)
    {
        // ITEM ENEMY EXP MAP EQUIP

        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            ITEM = column[0];
            ENEMY = column[1];
            EXP = column[2];
            MAP = column[3];
            EQUIP = column[4];
        }

        this.Log($" ITEM:{ITEM}, ENEMY:{ENEMY} ,EXP:{EXP} ,MAP:{MAP} EQUIP:{EQUIP}");

    }

    bool CanDownloadToday(DownType type)
    {
        // 1. 저장된 날짜 문자열 가져오기
        string lastDateStr = PlayerPrefs.GetString(SpreadSheetLastDownloadDate + "_" + type, "");

        // 2. 기록이 없다면 한 번도 안 한 것이므로 true
        if (string.IsNullOrEmpty(lastDateStr)) return true;

        // 3. 현재 날짜와 저장된 날짜 비교 (yyyy-MM-dd 형식)
        string currentDateStr = DateTime.Now.ToString("yyyy-MM-dd");

        return !currentDateStr.Equals(lastDateStr);
    }

    void SaveCurrentDate(DownType type)
    {
        string currentDateStr = DateTime.Now.ToString("yyyy-MM-dd");

        if (type == DownType.Sheet)
        {
            PlayerPrefs.SetString(SpreadSheetLastDownloadDate + "_" + type, $"{currentDateStr}_Sheet");
        }
        else
        {
            PlayerPrefs.SetString(SpreadSheetLastDownloadDate + "_" + type, $"{currentDateStr}_{type}");
        }

        PlayerPrefs.Save();
    }

    public void SaveItemDatas()
    {
        if (itemDatas == null || itemDatas.Length == 0) return;

        var wrapper = new ItemDataWrapper[itemDatas.Length];
        for (int i = 0; i < itemDatas.Length; i++)
        {
            wrapper[i] = new ItemDataWrapper(itemDatas[i]);
        }

        var list = new ItemDataList { items = wrapper };
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.persistentDataPath, "itemDatas.json");
        File.WriteAllText(path, json);
        this.Log($"ItemDatas saved to {path}");
    }

    public void SaveEquipmentDatas()
    {
        if (equipmentDatas == null || equipmentDatas.Length == 0) return;

        var wrapper = new EquipmentDataWrapper[equipmentDatas.Length];
        for (int i = 0; i < equipmentDatas.Length; i++)
        {
            wrapper[i] = new EquipmentDataWrapper(equipmentDatas[i]);
        }

        var list = new EquipmentDataList { equipments = wrapper };
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.persistentDataPath, "equipmentDatas.json");
        File.WriteAllText(path, json);
        this.Log($"EquipmentDatas saved to {path}");
    }

    public void SaveExp()
    {
        ExpDataWrapper expData = new ExpDataWrapper(
            GameManager.instance.maxGameTime,
            GameManager.instance.maxStage,
            GameManager.instance.nextExp,
            GameManager.instance.maxHealth,
            Spawner.speedRate,
            Spawner.healthRate,
            Spawner.attackRate,
            Spawner.spawnTimeRate
        );

        string json = JsonUtility.ToJson(expData, true);
        string path = Path.Combine(Application.persistentDataPath, "exp.json");
        File.WriteAllText(path, json);
        this.Log($"Exp data saved to {path}");
    }

    public void SaveEnemy()
    {
        if (Spawner._spawnDatas == null || Spawner._spawnDatas.Length == 0) return;

        var wrapper = new SpawnDataWrapper[Spawner._spawnDatas.Length];
        for (int i = 0; i < Spawner._spawnDatas.Length; i++)
        {
            wrapper[i] = new SpawnDataWrapper(Spawner._spawnDatas[i]);
        }

        var list = new SpawnDataList { spawnDatas = wrapper };
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.persistentDataPath, "spawnDatas.json");
        File.WriteAllText(path, json);
        this.Log($"Enemy data saved to {path}");
    }

    public void SaveStages()
    {
        if (stages == null || stages.Length == 0) return;

        var wrapper = new StageDataWrapper[stages.Length];
        for (int i = 0; i < stages.Length; i++)
        {
            wrapper[i] = new StageDataWrapper(stages[i]);
        }

        var list = new StageDataList { stages = wrapper };
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.persistentDataPath, "stages.json");
        File.WriteAllText(path, json);
        this.Log($"Stages saved to {path}");
    }

    public void LoadItemDatas()
    {
        string path = Path.Combine(Application.persistentDataPath, "itemDatas.json");
        if (!File.Exists(path))
        {
            this.Log($"Item data file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var list = JsonUtility.FromJson<ItemDataList>(json);
        if (list == null || list.items == null || list.items.Length == 0)
        {
            this.Log($"Item data JSON is empty or invalid.");
            return;
        }

        for (int i = 0; i < list.items.Length; i++)
        {
            var wrapper = list.items[i];
            var item = ScriptableObject.CreateInstance<ItemDataSO>();
            item.itemType = (ItemDataSO.ItemType)System.Enum.Parse(typeof(ItemDataSO.ItemType), wrapper.itemType);
            item.itemID = wrapper.itemID;
            item.itemName = wrapper.itemName;
            item.itemDesc = wrapper.itemDesc;
            item.baseDamage = wrapper.baseDamage;
            item.baseCount = wrapper.baseCount;
            item.damages = wrapper.damages;
            item.counts = wrapper.counts;
            item.knockBack = wrapper.knockBack;
            item.knockBackRate = wrapper.knockBackRate;
            // Note: image and prefab references are not saved in the JSON, so they will be null/default.
            itemDatas[i] = item;
        }

        this.Log($"ItemDatas loaded from {path}, count: {itemDatas.Length}");
    }

    public void LoadEquipmentDatas()
    {
        string path = Path.Combine(Application.persistentDataPath, "equipmentDatas.json");
        if (!File.Exists(path))
        {
            this.Log($"Equipment data file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var list = JsonUtility.FromJson<EquipmentDataList>(json);
        if (list == null || list.equipments == null || list.equipments.Length == 0)
        {
            this.Log($"Equipment data JSON is empty or invalid.");
            return;
        }

        for (int i = 0; i < list.equipments.Length; i++)
        {
            var wrapper = list.equipments[i];
            var equipment = ScriptableObject.CreateInstance<EquipmentSO>();
            equipment.id = wrapper.id;
            // gearType
            if (Enum.TryParse(typeof(GearType), wrapper.gearType, out object gearTypeObj))
                equipment.gearType = (GearType)gearTypeObj;
            else
                equipment.gearType = GearType.BodyArmor; // fallback
            equipment.atack = wrapper.atack;
            equipment.defence = wrapper.defence;
            equipment.moveSpeed = wrapper.moveSpeed;
            equipment.atkSpeed = wrapper.atkSpeed;
            // itemRarity
            if (Enum.TryParse(typeof(ItemRarity), wrapper.itemRarity, out object rarityObj))
                equipment.itemRarity = (ItemRarity)rarityObj;
            else
                equipment.itemRarity = ItemRarity.Common;
            equipment.weight = wrapper.weight;
            equipment.desc = wrapper.desc;
            // Note: image and other fields not saved in wrapper; they will be null/default.
            equipmentDatas[i] = equipment;
        }

        this.Log($"EquipmentDatas loaded from {path}, count: {equipmentDatas.Length}");
    }

    public void LoadStages()
    {
        string path = Path.Combine(Application.persistentDataPath, "stages.json");
        if (!File.Exists(path))
        {
            this.Log($"Stage data file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var list = JsonUtility.FromJson<StageDataList>(json);
        if (list == null || list.stages == null || list.stages.Length == 0)
        {
            this.Log($"Stage data JSON is empty or invalid.");
            return;
        }

        stages = new StageData[list.stages.Length];
        for (int i = 0; i < list.stages.Length; i++)
        {
            var wrapper = list.stages[i];
            var stage = new StageData(
                wrapper.id,
                wrapper.stageName,
                wrapper.stageDesc,
                wrapper.enemyId,
                wrapper.bossCount
            );
            stages[i] = stage;
        }

        this.Log($"Stages loaded from {path}, count: {stages.Length}");
    }

    public void LoadEnemy()
    {
        string path = Path.Combine(Application.persistentDataPath, "spawnDatas.json");
        if (!File.Exists(path))
        {
            this.Log($"Enemy data file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var list = JsonUtility.FromJson<SpawnDataList>(json);
        if (list == null || list.spawnDatas == null || list.spawnDatas.Length == 0)
        {
            this.Log($"Enemy data JSON is empty or invalid.");
            return;
        }

        Spawner._spawnDatas = new SpawnData[list.spawnDatas.Length];
        for (int i = 0; i < list.spawnDatas.Length; i++)
        {
            var wrapper = list.spawnDatas[i];
            var spawnData = new SpawnData
            {
                id = wrapper.id,
                boss = wrapper.boss,
                spawnTime = wrapper.spawnTime,
                spriteType = wrapper.spriteType,
                health = wrapper.health,
                speed = wrapper.speed,
                attack = wrapper.attack
            };
            Spawner._spawnDatas[i] = spawnData;
        }

        this.Log($"Enemy data loaded from {path}, count: {Spawner._spawnDatas.Length}");
    }

    public void LoadExp()
    {
        string path = Path.Combine(Application.persistentDataPath, "exp.json");
        if (!File.Exists(path))
        {
            this.Log($"Exp data file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var expData = JsonUtility.FromJson<ExpDataWrapper>(json);
        if (expData == null)
        {
            this.Log($"Exp data JSON is empty or invalid.");
            return;
        }

        GameManager.instance.maxGameTime = expData.maxGameTime;
        GameManager.instance.maxStage = expData.maxGameStage;
        GameManager.instance.nextExp = expData.nextExp;
        GameManager.instance.maxHealth = expData.maxHealth;

        Spawner.speedRate = expData.ENEMY_speedRate;
        Spawner.healthRate = expData.ENENy_healthRate;
        Spawner.attackRate = expData.ENENY_attackRate;
        Spawner.spawnTimeRate = expData.ENENY_spawnTimeRate;

        //레벨당 시간 계산
        Spawner.levelTime = GameManager.instance.maxGameTime / Spawner._spawnDatas.Length;

        this.Log($"Exp data loaded from {path}");
    }

}
