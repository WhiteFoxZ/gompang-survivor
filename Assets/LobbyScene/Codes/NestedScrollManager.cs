using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;

    public Slider tabSlider;
    public RectTransform[] BtnRect, BtnImageRect;

    //컨텐츠 갯수
    const int SIZE = 5;
    float[] pos = new float[SIZE];  //각각 수평 panel의 위치값 저장
    float distance, curPos, targetPos;
    bool isDrag;
    int targetIndex;
    bool isFirstFrame = true;


    void Start()
    {
        // 거리에 따라 0~1인 panel 위치값 계산해서 pos 초기화
        distance = 1f / (SIZE - 1);

        // this.Log("distance " + distance);

        for (int i = 0; i < SIZE; i++)
        {
            pos[i] = distance * i;
            // this.Log("pos[" + i + "] = " + pos[i]);
        }

        // 3번째 컨텐츠(인덱스 2)를 기본으로 표시
        targetIndex = 2;
        targetPos = pos[2];
        scrollbar.value = pos[2];

        // 버튼 크기 즉시 설정 (애니메이션 없이)
        for (int i = 0; i < SIZE; i++)
        {
            BtnRect[i].sizeDelta = new Vector2(i == targetIndex ? 360 : 180, BtnRect[i].sizeDelta.y);

            // 버튼 아이콘 위치와 크기 즉시 설정
            Vector3 BtnTargetPos = BtnRect[i].anchoredPosition3D;
            Vector3 BtnTargetScale = Vector3.one;
            bool textActive = false;

            if (i == targetIndex)
            {
                BtnTargetPos.y = -23f;
                BtnTargetScale = new Vector3(1.2f, 1.2f, 1);
                textActive = true;
            }

            BtnImageRect[i].anchoredPosition3D = BtnTargetPos;
            BtnImageRect[i].localScale = BtnTargetScale;
            BtnImageRect[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }

    }

    float SetPos()
    {
        // 절반거리를 기준으로 가까운 위치를 반환
        for (int i = 0; i < SIZE; i++)
            if (scrollbar.value < pos[i] + distance * 0.5f
            && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        return 0;
    }


    public void OnBeginDrag(PointerEventData eventData) => curPos = SetPos();

    public void OnDrag(PointerEventData eventData) => isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        //보통속도로 마우스 이동시
        targetPos = SetPos();

        // 절반거리를 넘지 않아도 마우스를 빠르게 이동하면
        if (curPos == targetPos)
        {
            // ← 으로 가려면 목표가 하나 감소
            if (eventData.delta.x > 18 && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
            }

            // → 으로 가려면 목표가 하나 증가
            else if (eventData.delta.x < -18 && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }

        VerticalScrollUp();
    }

    void VerticalScrollUp()
    {
        // 목표가 수직스크롤이고, 옆에서 옮겨왔다면 수직스크롤을 맨 위로 올림(오른쪽을 갔다가 다시 왼쪽으로 오면 위치를 1로 변경)
        for (int i = 0; i < SIZE; i++)
            if (contentTr.GetChild(i).GetComponent<ScrollScript>() && curPos != pos[i] && targetPos == pos[i])
                contentTr.GetChild(i).GetChild(1).GetComponent<Scrollbar>().value = 1;
    }


    void Update()
    {
        // 첫 번째 프레임에는 TabSlider도 즉시 설정
        if (isFirstFrame)
        {
            tabSlider.value = targetPos;
        }
        else
        {
            tabSlider.value = scrollbar.value;
        }

        if (!isDrag)
        {
            // 첫 번째 프레임이거나 목표 위치에 도달했으면 즉시 이동
            if (isFirstFrame || Mathf.Abs(scrollbar.value - targetPos) <= 0.001f)
            {
                scrollbar.value = targetPos;
                isFirstFrame = false;
            }
            else
            {
                //부드럽게 자석처럼 이동하게
                scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
            }

            // 목표 버튼은 크기가 커짐
            for (int i = 0; i < SIZE; i++)
            {
                BtnRect[i].sizeDelta
                = new Vector2(i == targetIndex ? 360 : 180, BtnRect[i].sizeDelta.y);
            }

        }


        if (Time.time < 0.1f) return;

        for (int i = 0; i < SIZE; i++)
        {
            // 버튼 아이콘이 부드럽게 버튼의 중앙으로 이동, 크기는 1, 텍스트 비활성화
            Vector3 BtnTargetPos = BtnRect[i].anchoredPosition3D;
            Vector3 BtnTargetScale = Vector3.one;
            bool textActive = false;

            // 선택한 버튼 아이콘은 약간 위로 올리고, 크기도 키우고, 텍스트도 활성화
            if (i == targetIndex)
            {
                BtnTargetPos.y = -23f;
                BtnTargetScale = new Vector3(1.2f, 1.2f, 1);
                textActive = true;
            }

            BtnImageRect[i].anchoredPosition3D = Vector3.Lerp(BtnImageRect[i].anchoredPosition3D, BtnTargetPos, 0.25f);
            BtnImageRect[i].localScale = Vector3.Lerp(BtnImageRect[i].localScale, BtnTargetScale, 0.25f);
            BtnImageRect[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }
    }


    public void TabClick(int n)
    {
        curPos = SetPos();
        targetIndex = n;
        targetPos = pos[n];
        VerticalScrollUp();
    }
}
