using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Clothsim : MonoBehaviour
{
    private Mesh m;
    [SerializeField] private VectorField VF;

    public Node[] vertices;

    public float repulsionForce;
    public float maxDist;
    public float speed;
    [System.Serializable]
    public struct Node
    {
        public GameObject Objectref;
        public GameObject[] connections;

    }
    private void Awake()
    {
        m = new Mesh();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateGeometry();
    }

    public void UpdateGeometry()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 currentforce = new Vector3();
            for (int j = 0; j < vertices[i].connections.Length; j++)
            {
                if (vertices[i].Objectref == vertices[i].connections[j]) continue;
                float dist = Vector3.Distance(vertices[i].Objectref.transform.position, vertices[i].connections[j].transform.position);
            if(dist > maxDist)
                {
                    currentforce += (vertices[j].Objectref.transform.position - vertices[i].connections[j].transform.position).normalized * (repulsionForce * (dist - maxDist));

                }
                else
                {

                   currentforce += (vertices[j].Objectref.transform.position - vertices[i].connections[j].transform.position).normalized * (repulsionForce * ((maxDist / dist) - 1)) * -1;
                }
            }
            currentforce /= vertices[i].connections.Length;
            vertices[i].Objectref.transform.position += currentforce * speed;
        }

    }
   
}
