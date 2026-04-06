using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 결과 클래스 - 게임 결과(승리/패배) 화면을 관리합니다.
/// </summary>
public class Result : MonoBehaviour
{

    public GameObject[] _titles; //결과 제목 배열 (0: 패배, 1: 승리)

    public Text _text;

    [Header("일시정지 버튼")]
    public GameObject pauseBG; //일시정지 배경 오브젝트


    /// <summary>
    /// 패배 화면 표시
    /// </summary>
    public void Lose()
    {
        //패배 제목 활성화
        _titles[0].SetActive(true);
        _text.text = $"재도전..";

        pauseBG.SetActive(false); //일시정지 배경 비활성화

    }

    /// <summary>
    /// 승리 화면 표시
    /// </summary>
    public void Win()
    {
        //승리 제목 활성화
        _titles[1].SetActive(true);

        _text.text = $"Next..";

        pauseBG.SetActive(false); //일시정지 배경 비활성화
    }


}
