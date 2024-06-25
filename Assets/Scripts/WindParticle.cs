using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed;
    [SerializeField] private VectorField vf;
    [SerializeField] private MeshRenderer m;
    // Update is called once per frame
    void Update()
    {
           Vector3 normalizedPos = (transform.position - vf.offset - vf.transform.position);
        normalizedPos.x /= (vf.gridSize.x * vf.gridSpacing);
        normalizedPos.y /= (vf.gridSize.y * vf.gridSpacing);
        normalizedPos.z /= (vf.gridSize.z * vf.gridSpacing);
        m.material.color = new Color(normalizedPos.x, normalizedPos.y, normalizedPos.z);

        transform.position += vf.SampleField(transform.position) * speed * Time.deltaTime;
    }
}
