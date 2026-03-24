using UnityEngine;

public class PlayerInfoLobby : MonoBehaviour
{

    public static PlayerInfoLobby instance;  //싱글톤 인스턴스

    public float playerinfoLV = 1f;
    public float playerinfoMaxLV = 60f;




    /// <summary>
    /// 시작 시 호출 - 싱글톤 설정 및 초기화
    /// </summary>
    void Awake()
    {

        this.Log($" Awake : ************************** ");

        // 싱글톤 구현: 인스턴스가 이미 존재하면 자신을 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

    }
}
