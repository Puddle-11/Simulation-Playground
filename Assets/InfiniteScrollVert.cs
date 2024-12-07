using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollVert : MonoBehaviour
{

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private RectTransform contentPanelTransform;
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    public RectTransform[] items;
    private Vector2 tempVel;
    private bool updated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tempVel = Vector2.zero;
        updated = false;
        int newItemCount = Mathf.CeilToInt(viewPortTransform.rect.height / (items[0].rect.height + layoutGroup.spacing));
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

        }
        contentPanelTransform.localPosition = new Vector3
            (
            contentPanelTransform.localPosition.x,
            (items[0].rect.height + layoutGroup.spacing) * newItemCount,
            contentPanelTransform.localPosition.z
            );
    }

    //  Update is called once per frame
    void Update()
    {
        if (updated)
        {
            scrollRect.velocity = tempVel;
            updated = false;
        }

        if (contentPanelTransform.localPosition.y < 0)
        {
            Canvas.ForceUpdateCanvases();
            tempVel = scrollRect.velocity;
            updated = true;
            contentPanelTransform.localPosition += new Vector3(0,items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }
        if (contentPanelTransform.localPosition.y > (items.Length * (items[0].rect.height + layoutGroup.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            tempVel = scrollRect.velocity;
            updated = true;

            contentPanelTransform.localPosition -= new Vector3(0, items.Length * (items[0].rect.height + layoutGroup.spacing), 0);
        }
    }
}