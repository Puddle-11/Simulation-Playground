using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class InventiryGrouping : MonoBehaviour
{
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemObject[] menuItems;
    [Range(1,4)]
    [SerializeField] private int itemCount;
   
    [SerializeField] private float distance;


    [SerializeField] private float maxTime;
    private float timer;
    [SerializeField] private AnimationCurve alphaBlend;
    private void Update()
    {
        timer += Time.deltaTime;
        float alpha = alphaBlend.Evaluate(timer / maxTime);
        if(timer >= maxTime)
        {
            //destroy
        }

    }
    public void EnableMenu()
    {
            


    }
    public void InitMenu()
    {

    }
    public void DissableMenu()
    {

    }
    public void ChangeSelected()
    {

    }






}
