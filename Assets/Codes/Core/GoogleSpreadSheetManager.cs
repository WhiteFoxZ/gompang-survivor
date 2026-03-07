using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


public class GoogleSpreadSheetManager : MonoBehaviour
{

    public static GoogleSpreadSheetManager instance;  //싱글톤 인스턴스

    void Awake()
    {
        //싱글톤 인스턴스 설정
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("게임 Item Data")]
    public ItemData[] itemDatas; //아이템 데이터 참조 (추후 아이템 시스템 구현 시 사용)


    const string URL = "https://docs.google.com/spreadsheets/d/1xHjfvfPxcGE9-rDfiwzXv-iw9ZQTfBDDMpSJ1rGrRQY/export?format=tsv&range=A2:J";

    public IEnumerator DownloadItemData()
    {
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
                SetItemSO(data); //데이터 파싱 및 아이템 데이터 설정

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
            itemDatas[i].itemDesc = column[3];
            itemDatas[i].baseDamage = float.Parse(column[4]);
            itemDatas[i].baseCount = int.Parse(column[5]);
            itemDatas[i].damages = Array.ConvertAll(column[6].Split(','), float.Parse);
            itemDatas[i].counts = Array.ConvertAll(column[7].Split(','), int.Parse);
            itemDatas[i].knockBack = float.Parse(column[8]);
            itemDatas[i].knockBackRate = float.Parse(column[9]);

        }


        //itemDatas 에 정보를 로그로 출력 (테스트용)
        foreach (var item in itemDatas)
        {
            Debug.Log($" 유형: {item.itemType},아이템: {item.itemName}, 데미지: {item.baseDamage}, 개수: {item.baseCount}, 레벨업 데미지: {string.Join(",", item.damages)}, 레벨업 개수: {string.Join(",", item.counts)}, 넉백: {item.knockBack}, 넉백확률: {item.knockBackRate}");
        }

    }



}
