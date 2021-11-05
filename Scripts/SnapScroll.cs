using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BallGame
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(EventTrigger))]
    public class SnapScroll : MonoBehaviour
    {
        public float Spacing;
        public float SnapSpeed;
        public float MaxScaleDictance;
        public float ScaleOffset;
        public float MinScale;
        public float AutoSelectVelocity;

        private ScrollRect scrollRect;
        private RectTransform contentRect;
        private Vector2 contentVector;
        private int selectedIndex;
        private bool isScrolling;

        private List<GameObject> pans = new List<GameObject>();
        private List<Vector2> panPos = new List<Vector2>();
        private List<Vector2> panScale = new List<Vector2>();

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            contentRect = scrollRect.content;

            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((eventData) => { Scrolling(true); });
            trigger.triggers.Add(entry);
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((eventData) => { Scrolling(false); });
            trigger.triggers.Add(entry);
        }
        
        void Update()
        {
            if (pans.Count > 0)
            {
                if (contentRect.anchoredPosition.x >= -panPos[0].x && !isScrolling || contentRect.anchoredPosition.x <= -panPos[panPos.Count - 1].x && !isScrolling)
                {
                    // isScrolling = false;
                    scrollRect.inertia = false;
                }

                float minDistance = float.MaxValue;
                for (int i = 0; i < pans.Count; i++)
                {
                    float distance = Mathf.Abs(contentRect.anchoredPosition.x + panPos[i].x);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedIndex = i;

                    }
                    //오브젝트 크기//
                    float scale;
                    if (MaxScaleDictance != 0)
                        scale = Mathf.Clamp(ScaleOffset * ((MaxScaleDictance - distance) / MaxScaleDictance), MinScale, ScaleOffset);
                    else
                        scale = ScaleOffset;
                    //Vector2 nowPanScale;
                    //nowPanScale.x = Mathf.SmoothStep(pans[i].transform.localScale.x, scale, ScaleSpeed * Time.fixedDeltaTime);
                    //nowPanScale.y = Mathf.SmoothStep(pans[i].transform.localScale.y, scale, ScaleSpeed * Time.fixedDeltaTime);
                    pans[i].transform.localScale = new Vector2(scale, scale);
                }

                float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);
                if (scrollVelocity < AutoSelectVelocity && !isScrolling)
                {
                    scrollRect.inertia = false;
                }
                if (isScrolling || scrollVelocity > AutoSelectVelocity) return;
                contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, -panPos[selectedIndex].x, SnapSpeed * Time.deltaTime);
                contentRect.anchoredPosition = contentVector;
            }
        }

        public void AddPan(GameObject pan)
        {
            pan.transform.parent = contentRect.transform;
            pan.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            pan.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            pan.GetComponent<RectTransform>().localPosition = new Vector3(pans.Count * Spacing, 0);
            panPos.Add(pan.GetComponent<RectTransform>().localPosition);
            pans.Add(pan);
        }

        public void Scrolling(bool scroll)//스크롤링 체크 
        {
            isScrolling = scroll; //터치를 땠을때 false가 댐
            if (scroll) scrollRect.inertia = true;
        }

        public int GetSelectIndex()
        {
            return selectedIndex;
        }
    }
}
