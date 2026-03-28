using UnityEngine;
using UnityEngine.UI;

public class ItemBoxUi : MonoBehaviour
{

    public ShopItemSO _shopItem;

    private Text[] titleTxt;
    private Image ImageItem;

    private Image ImageAd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        // public ShopItemType shopItemType; //상점 아이템 유형
        // public PayType payType; //결제 유형


        // public string title;
        // public int price; //가격
        // public int itemCnt; //충천갯수    

        // this.Log($" _shopItem : {_shopItem} ");
        // 자식중에 text 컴포넌트중 2번째 text 에 _shopItem에 itemCnt을 보여준다
        titleTxt = GetComponentsInChildren<Text>();


        //자신하위에 이미지를 이름으로 찾는다
        Transform imageItemTransform = transform.Find("ImageItem");
        if (imageItemTransform != null)
        {
            ImageItem = imageItemTransform.GetComponent<Image>();
        }

        Transform imageAdTransform = transform.Find("PanelAD/ImageAd");
        if (imageAdTransform != null)
        {
            ImageAd = imageAdTransform.GetComponent<Image>();
        }


        // 이미지 컴포넌트가 존재하는 경우에만 스프라이트 설정
        if (ImageItem != null && _shopItem.ImageItem != null)
        {
            ImageItem.sprite = _shopItem.ImageItem;
        }

        if (ImageAd != null && _shopItem.ImageAd != null)
        {
            ImageAd.sprite = _shopItem.ImageAd;
        }


        switch (_shopItem.payType)
        {
            case PayType.PAY:


                titleTxt[0].text = _shopItem.itemCnt.ToString();
                break;

            case PayType.AD:
            case PayType.COIN:
            case PayType.DIAMOND:

                titleTxt[0].text = _shopItem.title;
                break;


        }

        titleTxt[1].text = _shopItem.price == 0 ? "무료" : _shopItem.price.ToString();





    }

    // Update is called once per frame
    void Update()
    {

    }
}
