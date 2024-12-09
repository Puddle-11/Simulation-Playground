using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
public class LentorInfiniteScroll : MonoBehaviour, IScroll
{
    [Header("Required")]
    [Header("-=-=-=-=-=-")]

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private RectTransform contentPanelTransform;
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    private bool loopBack;

    private RectTransform[] items;

    private float centerOffset;
    private float rectSize;
    private float targetY;
    private Vector2 tempVel;
    private bool updated;
    private RectTransform scrollRectTransformRef;

    private void Awake()
    {
        items = new RectTransform[contentPanelTransform.childCount];
        loopBack = true;

        int i = 0;
        foreach(RectTransform child in contentPanelTransform)
        {
            items[i++] = child;
        }

    }

    private void Start()
    {
        tempVel = Vector2.zero;
        updated = false;
        scrollRectTransformRef = scrollRect.GetComponent<RectTransform>();

        rectSize = items[0].rect.height + layoutGroup.spacing;
        centerOffset = scrollRectTransformRef.rect.height - rectSize * ((viewPortTransform.rect.height / rectSize) + 0.5f) / 2;

        List<RectTransform> totalTemp = new List<RectTransform>();
        for (int i = 0; i < items.Length; i++)
        {
            totalTemp.Add(items[i]);
        }

        if (loopBack)
        {
            int newItemCount = Mathf.CeilToInt(viewPortTransform.rect.height / (items[0].rect.height + layoutGroup.spacing));
            
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
            
        }
        items = totalTemp.ToArray();
        //contentPanelTransform.localPosition = GetTargetPosition(startIndex);
    }

    private void Update()
    {
        LoopBack();
    }
    public void DiscreteScroll(int delta)
    {
        throw new System.NotImplementedException();
    }

    public void Scroll(float delta)
    {
        print($"Scrolling:{delta}");
        contentPanelTransform.localPosition =
            new Vector3(contentPanelTransform.localPosition.x,
            contentPanelTransform.localPosition.y + delta,
            contentPanelTransform.localPosition.z);
    }

    public void ScrollToElement(int index)
    {
        throw new System.NotImplementedException();
    }
    public int CurrentElement()
    {
        throw new System.NotImplementedException();
    }

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
            //UpdateCurrSelected(items.Length);

            contentPanelTransform.localPosition += new Vector3(0, items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }

        //detect lower bounds and loop around
        if (contentPanelTransform.localPosition.y > (items.Length * (items[0].rect.height + layoutGroup.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            tempVel = scrollRect.velocity;
            updated = true;
            //UpdateCurrSelected(-items.Length);

            contentPanelTransform.localPosition -= new Vector3(0, items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }
    }
}
