//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PolyAndCode.UI
{
    /// <summary>
    /// Entry for the recycling system. Extends Unity's inbuilt ScrollRect.
    /// </summary>
    public class RecyclableScrollRect : ScrollRect, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector]
        public IRecyclableScrollRectDataSource DataSource;

        public bool IsGrid;
        //Prototype cell can either be a prefab or present as a child to the content(will automatically be disabled in runtime)
        public RectTransform PrototypeCell;
        //If true the intiziation happens at Start. Controller must assign the datasource in Awake.
        //Set to false if self init is not required and use public init API.
        public bool SelfInitialize = true;

        public enum DirectionType
        {
            Vertical,
            Horizontal
        }

        public DirectionType Direction;

        //Segments : coloums for vertical and rows for horizontal.
        public int Segments
        {
            set
            {
                _segments = Math.Max(value, 2);
            }
            get
            {
                return _segments;
            }
        }
        [SerializeField]
        private int _segments;

        private RecyclingSystem _recyclingSystem;
        private Vector2 _prevAnchoredPos;

        // NestedScrollManager 연동
        private bool _forParent;
        private NestedScrollManager _nestedManager;
        private ScrollRect _parentScrollRect;

        protected override void Start()
        {
            //defafult(built-in) in scroll rect can have both directions enabled, Recyclable scroll rect can be scrolled in only one direction.
            //setting default as vertical, Initialize() will set this again. 
            vertical = true;
            horizontal = false;

            if (!Application.isPlaying) return;

            // NestedScrollManager 찾기
            var nestedManagerObj = GameObject.FindWithTag("NestedScrollManager");
            if (nestedManagerObj != null)
            {
                _nestedManager = nestedManagerObj.GetComponent<NestedScrollManager>();
                _parentScrollRect = nestedManagerObj.GetComponent<ScrollRect>();
            }

            if (SelfInitialize) Initialize();
        }

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize()
        {
            //Contruct the recycling system.
            if (Direction == DirectionType.Vertical)
            {
                _recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            else if (Direction == DirectionType.Horizontal)
            {
                _recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            vertical = Direction == DirectionType.Vertical;
            horizontal = Direction == DirectionType.Horizontal;

            _prevAnchoredPos = content.anchoredPosition;
            onValueChanged.RemoveListener(OnValueChangedListener);
            //Adding listener after pool creation to avoid any unwanted recycling behaviour.(rare scenerio)
            StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                                                               onValueChanged.AddListener(OnValueChangedListener)
                                                              ));
        }

        /// <summary>
        /// public API for Initializing when datasource is not set in controller's Awake. Make sure selfInitalize is set to false. 
        /// </summary>
        public void Initialize(IRecyclableScrollRectDataSource dataSource)
        {
            DataSource = dataSource;
            Initialize();
        }

        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="direction">scroll direction</param>
        public void OnValueChangedListener(Vector2 normalizedPos)
        {
            Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }

        /// <summary>
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData()
        {
            ReloadData(DataSource);
        }

        /// <summary>
        /// Overloaded ReloadData with dataSource param
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData(IRecyclableScrollRectDataSource dataSource)
        {
            if (_recyclingSystem != null)
            {
                StopMovement();
                onValueChanged.RemoveListener(OnValueChangedListener);
                _recyclingSystem.DataSource = dataSource;
                StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                                                               onValueChanged.AddListener(OnValueChangedListener)
                                                              ));
                _prevAnchoredPos = content.anchoredPosition;
            }
        }

        /*
        #region Testing
        private void OnDrawGizmos()
        {
            if (_recyclableScrollRect is VerticalRecyclingSystem)
            {
                ((VerticalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

            if (_recyclableScrollRect is HorizontalRecyclingSystem)
            {
                ((HorizontalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

        }
        #endregion
        */

        // IBeginDragHandler, IDragHandler, IEndDragHandler 구현 (NestedScrollManager 연동)
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 드래그 방향 판단: 수평 이동이 수직 이동보다 크면 부모 스크롤이 드래그
            _forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            if (_forParent && _nestedManager != null)
            {
                _nestedManager.OnBeginDrag(eventData);
                if (_parentScrollRect != null)
                    _parentScrollRect.OnBeginDrag(eventData);
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_forParent && _nestedManager != null)
            {
                _nestedManager.OnDrag(eventData);
                if (_parentScrollRect != null)
                    _parentScrollRect.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_forParent && _nestedManager != null)
            {
                _nestedManager.OnEndDrag(eventData);
                if (_parentScrollRect != null)
                    _parentScrollRect.OnEndDrag(eventData);
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }
    }
}