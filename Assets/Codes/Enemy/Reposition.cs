using UnityEngine;

/// <summary>
/// 리포지션 클래스 - 타일맵과 적의 영역을 플레이어 위치에 따라 재배치합니다.
/// </summary>
public class Reposition : MonoBehaviour
{

    Collider2D col; //영역 콜라이더


    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        col = GetComponent<Collider2D>();
    }



    /// <summary>
    /// 트리거退出 처리 - 영역 밖으로 나가면 재배치
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        //영역 태그가 아니면 종료
        if (!collision.CompareTag("Area"))
            return;


        //player 가 null 인 경우 예외처리
        if (GameManager.instance.player == null)
            return;

        //플레이어의 위치
        Vector3 playerPos = GameManager.instance.player.transform.position;
        //타일맵(현재 오브젝트)의 위치
        Vector3 areaPos = transform.position;


        //태그에 따라 다른 처리
        switch (transform.tag)
        {
            case "Ground":
                //플레이어와 리스폰 지역의 상대적 위치 계산
                float diffX = (playerPos.x - areaPos.x);
                float diffY = (playerPos.y - areaPos.y);

                //플레이어의 이동 방향 계산
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;

                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);


                //거리가 더 큰 방향으로 타일맵 이동
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40); // 플레이어가 좌우로 이동 중일 때 리스폰 지역을 수평 방향으로 이동
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40); // 40*40 타일의 전체 크기
                }

                break;

            case "Enemy":
                //적이 활성화되어 있으면
                if (col.enabled)
                {
                    //플레이어 방향으로의 거리
                    Vector3 dist = playerPos - areaPos;

                    //적 리스폰 지역을 플레이어 방향으로 약간 이동 + 랜덤 오프셋 추가
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);

                    transform.Translate(ran + dist * 2);
                }


                break;
            default:
                break;

        }

    }
}
