using UnityEngine;
using UnityEngine.Rendering.UI;
using static UnityEngine.UI.Button;

public class ScrollButton : MonoBehaviour
{

    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private string clickTriggerName;
    [SerializeField] private string hoverBoolName;
    [SerializeField] private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();



    private bool _hover;
    public bool hover
    {
        get
        {
            return _hover;
        }
        set
        {
            if (value != _hover)
            {
                if (buttonAnimator != null) buttonAnimator.SetBool(hoverBoolName, value);
            }
            _hover = value;
        }
    }
    private int _id;
    public int id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public void ExecuteEvent()
    {
        m_OnClick.Invoke();
    }
    public void Click()
    {
        if(buttonAnimator != null) buttonAnimator.SetTrigger(clickTriggerName);
        ExecuteEvent();
    }


}
