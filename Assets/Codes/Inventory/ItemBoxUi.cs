using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxUi : MonoBehaviour
{

    public ShopItem _shopItem;

    private Text[] buttonText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 자식중에 text 컴포넌트중 2번째 text 에 _shopItem에 itemCnt을 보여준다
        buttonText = GetComponentsInChildren<Text>();


        this.Log($"ssss: {buttonText[1]}");


        buttonText[1].text = _shopItem.price.ToString();

        if (_shopItem.payType == PayType.PAY)
        {
            buttonText[0].text = _shopItem.itemCnt.ToString();

        }



    }

    // Update is called once per frame
    void Update()
    {

    }
}
