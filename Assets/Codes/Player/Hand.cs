using UnityEngine;

/// <summary>
/// 핸드 클래스 - 플레이어 손(무기)의 위치와 렌더링을 관리합니다.
/// </summary>
public class Hand : MonoBehaviour
{
    public enum HandType { LEFT, RIGHT, BACK, FRONT }

    public bool isLeft; //왼손 여부

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

        if (isLeft) //왼손인 경우
        {
            //회전 설정
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            //좌우 반전
            spriter.flipY = isReverse;
            //정렬 순서 설정
            spriter.sortingOrder = isReverse ? playerSprite.sortingOrder - 1 : playerSprite.sortingOrder + 1;
        }
        else //오른손인 경우
        {
            //위치 설정
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            //좌우 반전
            spriter.flipX = isReverse;
            //정렬 순서 설정
            spriter.sortingOrder = isReverse ? playerSprite.sortingOrder + 1 : playerSprite.sortingOrder - 1;
        }
    }

}
