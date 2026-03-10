using UnityEngine;

/// <summary>
/// 핸드 클래스 - 플레이어 손(무기)의 위치와 렌더링을 관리합니다.
/// </summary>
public class Hand : MonoBehaviour
{
    public enum HandType { LEFT, RIGHT, BACK }

    public HandType handType; //손 유형

    public SpriteRenderer spriter; //스프라이트 렌더러

    SpriteRenderer playerSprite; //플레이어 스프라이트

    //오른손 위치
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0f);
    //오른손 위치 (좌우 반전 시)
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0f);

    //왼손 회전
    Quaternion leftRot = Quaternion.Euler(0f, 0f, -35f);
    //왼손 회전 (좌우 반전 시)
    Quaternion leftRotReverse = Quaternion.Euler(0f, 0f, -135f);




    /// <summary>
    /// 시작 시 호출 - 플레이어 스프라이트 가져오기
    /// </summary>
    void Awake()
    {
        //부모의 스프라이트 렌더러 가져오기
        playerSprite = GetComponentsInParent<SpriteRenderer>()[1];
    }

    /// <summary>
    /// 프레임 후 처리 - 손 위치 및 방향 업데이트
    /// </summary>
    void LateUpdate()
    {
        //플레이어 스프라이트 반전 여부
        bool isReverse = playerSprite.flipX;

        if (handType == HandType.LEFT) //왼손
        {
            //회전 설정
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            //좌우 반전
            spriter.flipY = isReverse;
            //정렬 순서 설정
            spriter.sortingOrder = isReverse ? playerSprite.sortingOrder - 1 : playerSprite.sortingOrder + 1;
        }
        else if (handType == HandType.RIGHT)  //오른손/앞의 경우
        {
            //위치 설정
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            //좌우 반전
            spriter.flipX = isReverse;
            //정렬 순서 설정
            spriter.sortingOrder = isReverse ? playerSprite.sortingOrder + 1 : playerSprite.sortingOrder - 1;
        }
        else if (handType == HandType.BACK) //등 뒤의 경우
        {

            //등 뒤 위치 (기본)
            Vector3 backPos = new Vector3(-0.49f, -0.05f, 0f);
            //등 뒤 위치 (좌우 반전 시)
            Vector3 backPosReverse = new Vector3(0.49f, -0.05f, 0f);


            //미사일 위치 설정
            transform.localPosition = isReverse ? backPosReverse : backPos;


            //정렬 순서 설정
            spriter.sortingOrder = isReverse ? playerSprite.sortingOrder + 1 : playerSprite.sortingOrder - 1;
        }



    }

}
