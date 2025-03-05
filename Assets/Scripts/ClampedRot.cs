using UnityEngine;

[ExecuteAlways]
public class ClampedRot : MonoBehaviour
{
    public GameObject target;
    public Vector2 yBounds;
    public Vector3 forward;
    private Vector3 targetVec;
    float angle;
    void Update()
    {
        targetVec = new Vector3((target.transform.position - transform.position).normalized.x,0, (target.transform.position - transform.position).normalized.z);



        transform.LookAt(targetVec, Vector3.up);
       


    }

  
}
