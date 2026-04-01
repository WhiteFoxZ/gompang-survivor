using System.Collections;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public PlayerData playerData = null;

    void Start()
    {

        this.Log("LobbyManager Start 장비정보 다운로드  시작");
        StartCoroutine(LoadDataAndStartGame());
        this.Log("LobbyManager Start 장비정보 다운로드  완료");

        this.Log("LobbyManager Start 장착한 장비정보 적용 시작");
        playerData = DataManager.instance.LoadData();
        
        if (playerData != null)
        {
            DataManager.instance.UpdateUI(playerData);
        }
        else
        {
            playerData = new PlayerData(); // 새로운 PlayerData 인스턴스 생성

            this.Log($" MaxLevel : {playerData.MaxLevel}");
            this.Log($" Level : {playerData.Level}");

            Debug.LogWarning("PlayerData를 불러오는데 실패했습니다. 새로운 PlayerData가 생성되었습니다.");
        }



    }



    IEnumerator LoadDataAndStartGame()
    {
        // 장비 데이터 다운로드 먼저 실행
        yield return StartCoroutine(GoogleSpreadSheetManager.instance.DownloadItemData(GoogleSpreadSheetManager.DownType.Equip));

    }

}
