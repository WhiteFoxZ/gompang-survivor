using UnityEngine;

/// <summary>
/// 리포지션 클래스 - 타일맵과 적의 영역을 플레이어 위치에 따라 재배치합니다.
/// </summary>
public class Reposition : MonoBehaviour
{

    Collider2D col; //영역 콜라이더

    GameObject wallGroup; //벽 그룹 참조 (WallSpawner에서 생성된 벽 그룹), 타일맵 재배치 시 벽이 활성화되어 있으면 타일맵 재배치 방지


    /// <summary>
    /// 시작 시 호출 - 컴포넌트 가져오기
    /// </summary>
    void Awake()
    {
        col = GetComponent<Collider2D>();

        //해당오브젝트가 Ground 태그를 가지고 있다면 WallGroup을 찾아 참조
        if (transform.tag == "Ground")
        {
            wallGroup = GameObject.FindWithTag("WallGroup"); //씬에서 WallGroup 오브젝트를 찾아 참조
        }

    }



    /// <summary>
    /// 트리거退出 처리 - 영역 밖으로 나가면 재배치
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        //영역 태그가 아니면 종료
        if (!(collision.CompareTag("Area") || collision.CompareTag("AreaEnemy")))
            return;


        //player 가 null 인 경우 예외처리
        if (GameManager.instance.player == null)
            return;

        //플레이어의 위치
        Vector3 playerPos = GameManager.instance.player.transform.position;
        //타일맵(현재 오브젝트)의 위치
        Vector3 areaPos = transform.position;

        //WallGroup(벽)이 없을때만 작동, 타일맵이 영역을 벗어날 때, 플레이어와의 상대적 위치 계산, 이동 방향 결정, 타일맵 이동
        if (wallGroup != null && wallGroup.activeSelf == false)  //타일맵 컴포넌트에서만 체크, 벽이 활성화되어 있지 않을 때만 타일맵 재배치
        {
            if (transform.tag == "Ground" && collision.CompareTag("Area"))
            {
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
                    transform.Translate(Vector3.right * dirX * 60); // 플레이어가 좌우로 이동 중일 때 리스폰 지역을 수평 방향으로 이동
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 60); // 60*60 타일의 전체 크기
                }

            }

        }


        //적이 영역을 벗어날 때, 보스가 아닌 경우에만 재배치 , 플레이어에서 멀어지는 방향으로 이동, 랜덤 오프셋 추가, 0.2초 뒤에 다시 보이게
        if (transform.tag == "Enemy" && collision.CompareTag("AreaEnemy") && transform.GetComponent<Enemy>().boss == 0)
        {
            if (col.enabled)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }

                //플레이어 방향으로의 거리
                Vector3 dist = playerPos - areaPos;

                //적 리스폰 지역을 플레이어 방향으로 약간 이동 + 랜덤 오프셋 추가
                Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);

                transform.Translate(ran + dist * 2.5f); //플레이어에서 멀어지는 방향으로 이동

                //0.2초 뒤에 다시 보이게
                StartCoroutine(ShowAfterDelay(spriteRenderer));
            }

        }



    }

    /// <summary>
    /// 리포지션 후 적을 다시 보이게 하는 코루틴
    /// </summary>
    System.Collections.IEnumerator ShowAfterDelay(SpriteRenderer spriteRenderer)
    {
        yield return new WaitForSeconds(0.2f);
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }
}
