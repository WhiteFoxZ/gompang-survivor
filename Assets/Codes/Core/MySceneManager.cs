using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 필요합니다.

/// <summary>
/// 씬 관리자 클래스 - 씬 전환을 관리합니다.
/// </summary>
public class MySceneManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static MySceneManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static MySceneManager instance;


    /// <summary>
    /// 로비로 이동
    /// </summary>
    public void GoToLobby()
    {
        // 'lobby'라는 이름의 씬으로 이동합니다.
        SceneManager.LoadScene("lobby");
    }


}
