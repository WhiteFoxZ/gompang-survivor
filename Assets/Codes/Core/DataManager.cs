using System.Collections.Generic;
using UnityEngine;

// 1.저장할 데이터가 존재
// 2.데이터를 제이슨으로 변환
// 3. 변환한 제이슨을 외부에 저장

public class DataManager : MonoBehaviour
{

    public static DataManager instance;  //싱글톤 인스턴스

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {

            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

}


public class PlayerInfo
{

    List<EquipmentSO> slot_list = new List<EquipmentSO>();   //장착중인아이템

    List<EquipmentSO> button_list = new List<EquipmentSO>();   //인벤토리에 있는아이템


}