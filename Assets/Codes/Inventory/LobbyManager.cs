using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    DataManager dataManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataManager.instance.LoadData();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
