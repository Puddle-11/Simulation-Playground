using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class VectorField : MonoBehaviour
{
    [SerializeField] private GameObject vecArrow;
    public Vector3Int gridSize;
    public float gridSpacing;
    [SerializeField] private float strength = 1;
    [HideInInspector] public Vector3 offset;
    [SerializeField] private float noiseScale;

    [SerializeField] private float scrollSpeed;
    [SerializeField] private Vector3 scrollDir;
    private float timerS;
    [SerializeField] private bool visualize;
    // Start is called before the first frame update
    private void Awake()
    {
        offset = new Vector3(gridSize.x, gridSize.y, gridSize.z) * gridSpacing / 2 * -1;
    }
    void Start()
    {
        if (visualize)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Instantiate(vecArrow, new Vector3(x, y, z) * gridSpacing + transform.position + offset, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
    private void Update()
    {
        if(scrollSpeed != 0 && scrollDir != Vector3.zero)
        {
            timerS += Time.deltaTime * scrollSpeed;

        }
    }


    public Vector3 SampleField(Vector3 _pos)
    {
        _pos += scrollDir * timerS;
        return new Vector3(GetNoise(_pos + Vector3.one * 100), GetNoise(_pos), GetNoise(_pos + Vector3.one * 500)) * strength;
    }
    private float GetNoise(Vector3 _pos)
    {
        return noise.snoise(_pos / noiseScale);
    }
}
