using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.Button;

public class InfiniteScrollVertical : MonoBehaviour
{
    [Header("Required")]
    [Header("-=-=-=-=-=-")]

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private RectTransform contentPanelTransform;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private GameObject selector;
    [SerializeField] private RectTransform[] items;
    [Header("-=-=-=-=-=-")]


    private float centerOffset;


    [Header("Scroll Variables")]
    [Header("-=-=-=-=-=-")]
    [SerializeField] private float moveSpeed;


    [Range(0, 10)]
    [SerializeField] private float holdSpeed;
    [Range(0, 10)]
    [SerializeField] private float initHoldDelay;
    [SerializeField] private float rushFactor;
    [Header("-=-=-=-=-=-")]
    [SerializeField] private int startIndex;
    [SerializeField] private HoverType hoverType;
    [SerializeField] private bool DissableSelectorOnMove;
    private float holdTimer;

    private int currSelect; //this is not always a valid index
    private float mouseTimer;

    [SerializeField] private bool scrollWheelInput;
    [SerializeField] private bool mouseInput;
    [SerializeField] private bool keyboardInput;
    [SerializeField] private bool staticSelector;
    [SerializeField] private bool loopBack;
    private float rectSize;
    private float targetY;
    public RectTransform[] totalItems;
    private Vector2 tempVel;
    private bool updated;
    private RectTransform scrollRectTransformRef;
    public enum HoverType
    {
        None,
        All,
        Focused
    }
    private enum MouseClickRetun
    {
        NoClick = 0,
        OutOfBounds = 1,
        AboveSelected = 2,
        BelowSelected = 3,
        OnSelected = 4
    }
    private enum MouseHoverReturn
    {
        NoHover = 0,
        OverUI = 1,
    }

    void Start()
    {
        tempVel = Vector2.zero;
        updated = false;
        scrollRectTransformRef = scrollRect.GetComponent<RectTransform>();

        rectSize = items[0].rect.height + layoutGroup.spacing;
        centerOffset = scrollRectTransformRef.rect.height - rectSize * ((viewPortTransform.rect.height / rectSize) + 0.5f) / 2;

        //Construct temp list
        List<RectTransform> totalTemp = new List<RectTransform>();
        for (int i = 0; i < items.Length; i++)
        {
            totalTemp.Add(items[i]);
        }

        if (loopBack)
        {
            int newItemCount = Mathf.CeilToInt(viewPortTransform.rect.height / (items[0].rect.height + layoutGroup.spacing));
            currSelect = startIndex + newItemCount;
            //---------------------------
            for (int i = 0; i < newItemCount; i++)
            {
                RectTransform rtRef = Instantiate(items[i % items.Length], contentPanelTransform);
                rtRef.SetAsLastSibling();

                int iInvert = items.Length - i - 1;
                while (iInvert < 0)
                {
                    iInvert += items.Length;
                }
                RectTransform rtRefi = Instantiate(items[iInvert], contentPanelTransform);
                rtRefi.SetAsFirstSibling();

                totalTemp.Insert(0, rtRefi);
                totalTemp.Add(rtRef);
            }
        }
        else
        {
            currSelect = BoundSelected(startIndex, totalTemp.Count);
        }
        totalItems = totalTemp.ToArray();
        contentPanelTransform.localPosition = GetTargetPosition(startIndex);
    }



    //Update is called once per frame
    void Update()
    {
        int tempSelected;
        //temp selected will always be the elemet at the center of the rect
        tempSelected = GetCenterElement();
        //if currently using mouse, pause snapping
        if ((Input.GetMouseButton(0) || Mathf.Abs(scrollRect.velocity.y) > 20) && mouseInput)
        {
            mouseTimer += Time.deltaTime;
            if (mouseTimer > 0.2)
            {
                //finds the item at the center of the screen
                SetCurrSelected(tempSelected);
            }
        }
        else
        {
            mouseTimer = 0;
            //Calculate speed based on distance from desired index

            float tMove = Mathf.Clamp(Mathf.Abs(tempSelected - currSelect) * rushFactor, 1, Mathf.Infinity) * moveSpeed;
            //moves contentpanel towards target position at desired speed
            contentPanelTransform.localPosition = Vector3.MoveTowards(contentPanelTransform.localPosition, GetTargetPosition(currSelect), tMove * Time.deltaTime);
        }
        scrollRect.vertical = mouseInput;
        if (mouseInput)
        {
            MouseButtonInteraction();
        }
        if (keyboardInput)
        {
            KeyboardInput();
        }
        LoopBack();
        if (!(currSelect < 0 || currSelect > totalItems.Length - 1) && !staticSelector) selector.transform.position = totalItems[currSelect].transform.position;
        else if (staticSelector) selector.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        //sets the selector active depending on if the scroll rect is moving
        //(only for mouse, since keyboard movement translates and doesnt use velocity)

        selector.SetActive(!(Mathf.Abs(scrollRect.velocity.y) > 20) || !DissableSelectorOnMove || staticSelector);
    }

    #region Loopback Magic
    public void LoopBack()
    {

        //pauses velocity updates when we do infinite scroll trickery
        if (updated)
        {
            scrollRect.velocity = tempVel;
            updated = false;
        }

        //detect upper bounds and loop around
        if (contentPanelTransform.localPosition.y < 0)
        {

            Canvas.ForceUpdateCanvases();
            tempVel = scrollRect.velocity;
            updated = true;
            UpdateCurrSelected(items.Length);

            contentPanelTransform.localPosition += new Vector3(0, items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }

        //detect lower bounds and loop around
        if (contentPanelTransform.localPosition.y > (items.Length * (items[0].rect.height + layoutGroup.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            tempVel = scrollRect.velocity;
            updated = true;
            UpdateCurrSelected(-items.Length);

            contentPanelTransform.localPosition -= new Vector3(0, items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }
    }
    #endregion

    #region keyboard Input
    private void KeyboardInput()
    {

        if (!(currSelect < 0 || currSelect > totalItems.Length - 1) && Input.GetKeyDown(KeyCode.Space))
        {
            if (totalItems[currSelect].TryGetComponent(out ScrollButton _scrollButtonRef))
            {
                _scrollButtonRef.Click();
            }
            else if (totalItems[currSelect].TryGetComponent(out UnityEngine.UI.Button _button))
            {
            }
        }
        //keyboard handling
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ForceUpdateSelected(-1);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {

            ForceUpdateSelected(1);
        }

        holdTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (holdTimer > initHoldDelay + holdSpeed)
            {
                holdTimer = initHoldDelay;
                UpdateCurrSelected(1);

            }
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (holdTimer > initHoldDelay + holdSpeed)
            {
                holdTimer = initHoldDelay;
                UpdateCurrSelected(-1);
            }
        }
        else
        {
            //neither set is being held down
            holdTimer = 0;
        }
    }
    #endregion

    #region Mouse Interactions
    void MouseButtonInteraction()
    {
        MouseClickRetun r = GetMouseClick();


        if (r == MouseClickRetun.AboveSelected)
        {
            ForceUpdateSelected(-1);
        }
        else if (r == MouseClickRetun.BelowSelected)
        {
            ForceUpdateSelected(1);
        }
        else if (r == MouseClickRetun.OnSelected)
        {
            if (totalItems[currSelect].TryGetComponent(out ScrollButton buttonRef))
            {
                buttonRef.Click();
            }
            else
            {
                Debug.LogWarning("Element " + totalItems[currSelect].name + " does not have a ScrollButtonComponent");
            }
        }

        if (Input.mouseScrollDelta.y > 0) ForceUpdateSelected(-1);
        else if (Input.mouseScrollDelta.y < 0) ForceUpdateSelected(1);

        GameObject selected;
        if (GetMouseHover(out selected) == MouseHoverReturn.OverUI)
        {
            if (selected != null)
            {
                if (selected.TryGetComponent(out ScrollButton _scrollButtonRef))
                {
                    _scrollButtonRef.hover = true;
                }
            }
        }
        for (int i = 0; i < totalItems.Length; i++)
        {
            if (totalItems[i].TryGetComponent(out ScrollButton _scrollButtonRef))
            {
                if (selected == null || totalItems[i].gameObject != selected)
                {
                    _scrollButtonRef.hover = false;
                }

            }
        }
    }

    #endregion

    #region MouseHover
    MouseHoverReturn GetMouseHover(out GameObject _selectedObject)
    {
        MouseHoverReturn res = GetMouseHover(out _, out _selectedObject);
        return res;
    }
    MouseHoverReturn GetMouseHover(out GameObject[] _allObjects)
    {
        MouseHoverReturn res = GetMouseHover(out _allObjects, out _);
        return res;
    }
    MouseHoverReturn GetMouseHover(out GameObject[] _allObjects, out GameObject _selectedObject)
    {
        RaycastResult[] raycastResults = GetMouseRaycast();
        _allObjects = new GameObject[raycastResults.Length];

        GameObject selectedRes = null;
        for (int i = 0; i < _allObjects.Length; i++)
        {
            _allObjects[i] = raycastResults[i].gameObject;
            switch (hoverType)
            {
                case HoverType.All:
                    if (!(currSelect < 0 || currSelect > totalItems.Length - 1) && raycastResults[i].gameObject.GetComponent<ScrollButton>() != null)
                    {
                        selectedRes = raycastResults[i].gameObject;
                    }
                    break;
                case HoverType.Focused:
                    if (!(currSelect < 0 || currSelect > totalItems.Length - 1) && raycastResults[i].gameObject == totalItems[currSelect].gameObject)
                    {
                        selectedRes = raycastResults[i].gameObject;
                    }
                    break;
            }
        }
        _selectedObject = selectedRes;
        return raycastResults.Length > 0 ? MouseHoverReturn.OverUI : MouseHoverReturn.NoHover;
    }
    #endregion

    #region MouseClick
    MouseClickRetun GetMouseClick()
    {
        MouseClickRetun returnType = MouseClickRetun.NoClick;

        if (!Input.GetMouseButtonDown(0)) return returnType;

        returnType = MouseClickRetun.OutOfBounds;

        RaycastResult[] rayResults = GetMouseRaycast();

        bool inBounds = false;
        if (!(currSelect < 0 || currSelect > totalItems.Length - 1))
        {
            for (int i = 0; i < rayResults.Length; i++)
            {

                if (rayResults[i].gameObject == totalItems[currSelect].gameObject)
                {
                    returnType = MouseClickRetun.OnSelected;
                }

                if (rayResults[i].gameObject == scrollRect.gameObject) inBounds = true;
            }
            if (returnType != MouseClickRetun.OnSelected && inBounds)
            {

                returnType = Input.mousePosition.y > totalItems[currSelect].position.y ? MouseClickRetun.AboveSelected : MouseClickRetun.BelowSelected;
            }
        }
        return returnType;
    }
    #endregion

    #region Helper Functions


    private Vector3 GetTargetPosition(int _inedx)
    {
        float y;
        if (loopBack)
        {
            y = rectSize * currSelect - scrollRectTransformRef.rect.height / 2 + items[0].rect.height / 2;
        }
        else
        {
            y = Mathf.Clamp(rectSize * currSelect - scrollRectTransformRef.rect.height / 2 + items[0].rect.height / 2, 0, contentPanelTransform.rect.height / 2);
            Debug.Log(y);
        }
        return Vector3.up * y;

    }
    private int GetCenterElement()
    {
        return Mathf.RoundToInt((contentPanelTransform.localPosition.y + centerOffset) / (rectSize));
    }
    private void ForceUpdateSelected(int _amount)
    {
        scrollRect.velocity = Vector2.zero;
        SetCurrSelected(currSelect + _amount);
    }
    void UpdateCurrSelected(int _amount)
    {
        SetCurrSelected(currSelect + _amount);

    }
    private void SetCurrSelected(int _index)
    {
        currSelect = _index;
        if (!(currSelect < 0 || currSelect > totalItems.Length - 1))
        {
            EventSystem.current.SetSelectedGameObject(totalItems[currSelect].gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);

        }
    }
    private RaycastResult[] GetMouseRaycast()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> rayResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, rayResults);
        return rayResults.ToArray();

    }
    #endregion

    int BoundSelected(int _index, int _mod)
    {
        if (_index >= 0)
        {
            return _index % _mod;
        }
        else
        {
            int res = _index;
            while (res < 0)
            {
                res += _mod;

            }
            return res;
        }
    }
}
