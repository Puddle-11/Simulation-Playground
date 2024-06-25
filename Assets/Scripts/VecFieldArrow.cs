using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VecFieldArrow : MonoBehaviour
{
    private VectorField fieldRef;
    [SerializeField] private float lineDist;
    [SerializeField] private LineRenderer lineRen;
    // Start is called before the first frame update
    void Start()
    {
        fieldRef = GetComponentInParent<VectorField>();

        Vector3 normalizedPos = (transform.position - fieldRef.offset - fieldRef.transform.position);
        normalizedPos.x /= (fieldRef.gridSize.x * fieldRef.gridSpacing);
        normalizedPos.y /= (fieldRef.gridSize.y * fieldRef.gridSpacing);
        normalizedPos.z /= (fieldRef.gridSize.z * fieldRef.gridSpacing);

        Gradient gradient = new Gradient();

        GradientColorKey[] gradcolors = new GradientColorKey[]{
            new GradientColorKey(new Color(normalizedPos.x, normalizedPos.y, normalizedPos.z), 0.0f)
            };

        GradientAlphaKey[] gradalphas = new GradientAlphaKey[]{
            new GradientAlphaKey(0.2f, 0.0f)

            };
        gradient.SetKeys(gradcolors, gradalphas);
        lineRen.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateArrow();
    }
    private void UpdateArrow()
    {
        
        Vector3 sample = fieldRef != null ? fieldRef.SampleField(transform.position) : Vector3.zero;
        lineRen.positionCount = 2;
        Vector3[] points = new Vector3[2];
        points[0] = transform.position;
        points[1] = transform.position + sample * lineDist;
        lineRen.SetPositions(points);
    }
}
