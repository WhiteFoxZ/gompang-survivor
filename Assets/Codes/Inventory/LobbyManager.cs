using System.Collections;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    void Start()
    {
        // this.Log("LobbyManager Start 장비정보 다운로드  시작");
        StartCoroutine(LoadDataAndStartGame());
    }

    IEnumerator LoadDataAndStartGame()
    {
        // 장비 데이터 다운로드 먼저 실행
        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(GoogleSpreadSheetManager.DownType.Equip));

        this.Log("LobbyManager playerData 장비 데이터 다운로드 먼저 실행 시작");


        PlayerData playerData = DataManager.instance.LoadData();


        if (playerData != null)
        {
            DataManager.instance.UpdateUI(playerData);
        }
       

    }

}
