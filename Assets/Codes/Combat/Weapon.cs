using UnityEngine;

/// <summary>
/// 무기 클래스 - 플레이어의 무기를 관리합니다.
/// </summary>
public class Weapon : MonoBehaviour
{

    public int id; //무기 ID
    public int prefabId; //프리팹 ID
    public float damage; //무기 데미지
    public int count; //투사체 개수
    public float speed; //공격 속도/회전 속도
    public float knockback; //넉백 세기
    public float knockbackRate; //넉백 확률

    private float timer; //타이머 (원거리 무기용)

    Player player; //플레이어 참조

    /// <summary>
    /// 시작 시 호출 - 플레이어 참조 가져오기
    /// </summary>
    void Awake()
    {
        player = GameManager.instance.player;
    }


    /// <summary>
    /// 매 프레임 호출 - 무기 동작 처리
    /// </summary>
    void Update()
    {
        //게임이 종료된 경우 실행 안함
        if (!GameManager.instance.isLive)
            return;

        //무기 ID에 따른 동작
        switch (id)
        {
            case 0: //삽 (근접 무기)
                //지속적으로 회전
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;


            case 1:
                //원거리 무기: 타이머로 발사 주기 조절
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    Fire();
                }
                break;

            case 6: //missle 무기
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    MissleFire();
                }
                break;

            default:
                break;


        }
    }

    /// <summary>
    /// 무기 초기화 - 아이템 데이터에서 무기 설정
    /// </summary>
    /// <param name="itemData">아이템 데이터</param>
    public void Init(ItemData itemData)
    {
        //이름 설정
        name = "Weapon_" + itemData.itemID.ToString();
        transform.parent = player.transform;    //플레이어 자식으로 설정
        transform.localPosition = Vector3.zero; //플레이어 위치로 이동

        //속성 초기화
        id = itemData.itemID;
        damage = itemData.baseDamage * Character.Damage;
        count = itemData.baseCount + Character.Count;
        knockback = itemData.knockBack;
        knockbackRate = itemData.knockBackRate;


        //프리팹 ID 찾기
        for (int index = 0; index < GameManager.instance.poolManager.prefabs.Length; index++)
        {
            if (GameManager.instance.poolManager.prefabs[index] == itemData.itemPrefab)
            {
                prefabId = index;
                break;
            }
        }


        Hand hand = null; //손 참조


        //무기 유형에 따른 초기화
        switch (itemData.itemID)
        {
            case 0: //삽 (근접 무기)
                speed = 150 * Character.WeaponSpeed;
                Plcae(); //무기 배치

                hand = player.hands[0]; //근접 무기는 왼손
                break;
            case 1: //원거리 무기
                speed = 0.3f * Character.WeaponRate;    //1초에 3번발사
                hand = player.hands[1]; //원거리 무기는 오른손
                break;

            case 6: //missle 무기
                speed = 2.0f; // 미사일 발사 간격 (2초당 1회)
                hand = player.hands[2]; //미사일은 왼손
                break;

            default:
                break;
        }

        //손 적용 - 손에 무기 이미지 표시

        hand.spriter.sprite = itemData.hand;
        hand.gameObject.SetActive(true);


        //장비 효과 갱신
        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }

    /// <summary>
    /// 무기 배치 - 근접 무기의 위치와 회전을 설정합니다.
    /// </summary>
    void Plcae()
    {
        //투사체 개수만큼 배치
        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            //기존 자식이 있으면 재사용 ,없으면 새로 생성
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                //새 오브젝트 생성
                bullet = GameManager.instance.poolManager.GetObject(prefabId).transform;
                bullet.parent = transform;  //무기 오브젝트의 자식으로 설정

            }

            bullet.localPosition = Vector3.zero;    //초기화(player 위치로 이동 방지)  
            bullet.localRotation = Quaternion.identity;

            //위치 회전 - 원형으로 배치
            Vector3 rotVec = Vector3.forward * (360f / count) * i;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.3f, Space.World); //무기에서 약간 떨어진 위치에 배치

            //무기 프로퍼티 초기화 - 관통력 -100 (관통 없음), 넉백 적용
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero, knockback, knockbackRate);

        }


    }



    /// <summary>
    /// 무기 레벨업 - 데미지와 개수 증가
    /// </summary>
    /// <param name="damage">추가 데미지</param>
    /// <param name="count">추가 개수</param>
    public void LevelUp(float damage, int count)
    {
        this.damage += damage * Character.Damage;
        this.count += count;

        //삽 무기인 경우 다시 배치
        if (id == 0)
        {
            Plcae(); //다시 배치
        }

        //장비 효과 갱신
        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }


    /// <summary>
    /// 총알 발사 - 원거리 무기의 투사체를 생성합니다.
    /// </summary>
    void Fire()
    {
        //총알 발사 로직

        //타겟이 없으면 발사 안함
        if (player.scanner.nearestTarget == null || player.scanner.nearestTarget.Length == 0)
            return;



        Vector3 targetPos = player.scanner.nearestTarget[0].position;  //가장 가까운 타겟 위치
        Vector3 fireDir = (targetPos - transform.position).normalized; //발사방향

        //투사체 가져오기
        Transform bullet = GameManager.instance.poolManager.GetObject(prefabId).transform;
        bullet.position = transform.position;

        bullet.rotation = Quaternion.FromToRotation(Vector3.up, fireDir); //총알 회전

        //총알 초기화 - 관통 count 만큼 설정, 넉백 적용
        bullet.GetComponent<Bullet>().Init(damage, count, fireDir, knockback, knockbackRate);

        //발사 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.SFX.Range);

    }

    void MissleFire()
    {
        //미사일 발사 로직

        //타겟이 없으면 발사 안함
        if (!player.scanner.missleTarget)
            return;

        //투사체 가져오기
        Transform bullet = GameManager.instance.poolManager.GetObject(prefabId).transform;
        bullet.position = transform.position;

        float impackDamage = this.knockback; //넉백 세기
        float impackRate = this.knockbackRate; //넉백 확률


        //총알 초기화 - 관통 count 만큼 설정, 넉백 적용
        bullet.GetComponent<BulletMissle>().Init(damage, player.scanner.nearestTarget[0], impackDamage, impackRate);



        //발사 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.SFX.Range);





    }


}
