using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBody : MonoBehaviour
{
    private Gravity gravRef;

    [SerializeField] private int killLayer;

    private void Start()
    {
        gravRef = GetComponent<Gravity>();
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered");
        if(other.gameObject.layer == killLayer)
        {
            Destroy(gameObject);
            gravRef.SysManager.updateBodies();
        }
    }
}
