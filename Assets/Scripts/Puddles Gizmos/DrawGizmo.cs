
using UnityEditor;
using UnityEngine;


public class DrawGizmo : MonoBehaviour
{
    public Type drawType;


    public bool onSelect;
    public bool wire;
    public Color color;

    [HideInInspector] public bool localScale;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] public Vector3 size;
    [HideInInspector] public Mesh collMesh;
    [HideInInspector] public Vector3 rotation;
    [HideInInspector] public Collider coll;
    public enum Type
    {
        cube,
        sphere,
        circle,
        hemisphere,
        mesh,
        collider,

    }
    public void OnDrawGizmos()
    {
        if (!onSelect)
        {
            Draw();
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (onSelect)
        {
            Draw();
        }
    }
    private void Draw()
    {
        Gizmos.color = color;
        switch (drawType)
        {
            case Type.cube:
                if (wire) Gizmos.DrawWireCube(transform.position + offset, size);
                else Gizmos.DrawCube(transform.position + offset, size);
                break;
            case Type.sphere:
                if (wire) Gizmos.DrawWireSphere(transform.position + offset, size.x);
                else Gizmos.DrawSphere(transform.position + offset, size.x);
                break;
            case Type.circle:
                GizmoDrawWireCircle(transform.position, size.x, rotation, offset);
                break;

            case Type.hemisphere:
                GizmoDrawWireCircle(transform.position, size.x, new Vector3(90,0,90) , new Vector2(0, Mathf.PI));
                GizmoDrawWireCircle(transform.position, size.x, new Vector3(0, 0, 90), new Vector2(0, Mathf.PI));
                GizmoDrawWireCircle(transform.position, size.x, new Vector3(90, 0, 0));

                break;
            case Type.mesh:
                if (collMesh != null)
                {
                    Vector3 nsize = size;
                    if (localScale)
                    {
                        nsize = new Vector3(size.x * transform.lossyScale.x, size.y * transform.lossyScale.y, size.z * transform.lossyScale.z);
                    }
                    
                    if (wire) Gizmos.DrawWireMesh(collMesh, 0, transform.position + offset, Quaternion.Euler(rotation), nsize);
                    else Gizmos.DrawMesh(collMesh, 0, transform.position + offset, Quaternion.Euler(rotation), nsize);
                }
                break;
            case Type.collider:
                if (coll != null)
                {
                    if (coll.TryGetComponent(out BoxCollider BcollRef))
                    {

                        Vector3 nsize = new Vector3(BcollRef.size.x * transform.lossyScale.x, BcollRef.size.y * transform.lossyScale.y, BcollRef.size.z * transform.lossyScale.z);
                        
                        Vector3 center = new Vector3(BcollRef.center.x * transform.lossyScale.x, BcollRef.center.y * transform.lossyScale.y, BcollRef.center.z * transform.lossyScale.z) + transform.position;
                        if (wire) Gizmos.DrawWireCube( center, nsize);
                        else Gizmos.DrawCube(center, size);
                    }
                    else if (coll.TryGetComponent(out CapsuleCollider CcollRef))
                    {
                        float largerScale = transform.lossyScale.x > transform.lossyScale.z ? transform.lossyScale.x : transform.lossyScale.z;

                        Vector3 size = new Vector3(CcollRef.radius * 2 * largerScale, (CcollRef.radius * 2 <  CcollRef.height ? CcollRef.height : CcollRef.radius * 2) * transform.lossyScale.y, CcollRef.radius * 2 * largerScale);
                        Vector3 center = new Vector3(CcollRef.center.x * transform.lossyScale.x, CcollRef.center.y * transform.lossyScale.y, CcollRef.center.z * transform.lossyScale.z) + transform.position;

                        if (wire) Gizmos.DrawWireCube(center, size);
                        else Gizmos.DrawCube(center, size);

                    }
                    else if (coll.TryGetComponent(out MeshCollider McollRef))
                    {
    
                        if (wire) Gizmos.DrawWireMesh(McollRef.sharedMesh, 0, transform.position, transform.rotation, transform.lossyScale);
                        else Gizmos.DrawMesh(McollRef.sharedMesh, 0, transform.position, transform.rotation, transform.lossyScale);
                    }
                    else if (coll.TryGetComponent(out SphereCollider ScollRef))
                    {
                        Vector3 center = new Vector3(ScollRef.center.x * transform.lossyScale.x, ScollRef.center.y * transform.lossyScale.y, ScollRef.center.z * transform.lossyScale.z) + transform.position;
                        //------------------------------------------------
                        //Get largest scale value
                        float largestScale = 
                            transform.lossyScale.x > transform.lossyScale.y 
                                ? 
                                transform.lossyScale.x > transform.lossyScale.z ? transform.lossyScale.x : transform.lossyScale.z 
                                :
                                transform.lossyScale.y > transform.lossyScale.z ? transform.lossyScale.y : transform.lossyScale.z;
                        //------------------------------------------------
                        float size = ScollRef.radius * largestScale;

                        if (wire) Gizmos.DrawWireSphere(center, size);
                        else Gizmos.DrawSphere(center, size);
                        

                    }
                    else if (coll.TryGetComponent(out TerrainCollider TcollRef))
                    {

                        //Terrain Collider not supported
                    }
                    else if (coll.TryGetComponent(out WheelCollider WcollRef))
                    {
                        //Wheel not supported

                    }
                }
                break;
        }
    }


    public static void GizmoDrawWireCircle(Vector3 _origin, float _radius, Vector3 _rotation, Vector3 _offset = default(Vector3), float _segmentStep = 0.1f)
    {
        GizmoDrawWireCircle(_origin, _radius, _rotation, new Vector3(0, Mathf.PI * 2), _offset, _segmentStep);

    }

    public static void GizmoDrawWireCircle(Vector3 _origin, float _radius, Vector3 _rotation, Vector2 minMaxAngle, Vector3 _offset = default(Vector3), float _segmentStep = 0.1f)
    {
        _segmentStep = Mathf.Clamp(_segmentStep, 0.01f, Mathf.Infinity);
        

          float step = minMaxAngle.x;

        Vector3 prevPos = new Vector2(Mathf.Sin(step), Mathf.Cos(step));
        while (step < minMaxAngle.y)
        {
            step = Mathf.Clamp(step + _segmentStep, minMaxAngle.x, minMaxAngle.y);
            Vector2 dcirc = new Vector2(Mathf.Sin(step), Mathf.Cos(step));
            Gizmos.DrawLine(Rotate(_offset + (Vector3)dcirc * _radius, _offset, _rotation) + _origin, Rotate(_offset + (Vector3)prevPos * _radius, _offset, _rotation) + _origin);
            prevPos = dcirc;
        }
    }
    public static Vector3 Rotate(Vector3 _pos, Vector3 _pivot, Vector3 _rot, bool rad = false)
    {
        if (!rad)
        {
            _rot = Mathf.Deg2Rad * _rot;
        }
        Vector3 p0 = _pos - _pivot;
        //Initialize


        //X Rotation
        Vector3 p1 = Vector3.zero;

        p1.x = p0.x;
        p1.y = p0.y * Mathf.Cos(_rot.x) - p0.z * Mathf.Sin(_rot.x);
        p1.z = p0.y * Mathf.Sin(_rot.x) + p0.z * Mathf.Cos(_rot.x);
        //Y Rotation
        Vector3 p2 = Vector3.zero;

        p2.x = p1.x * Mathf.Cos(_rot.y) + p1.z * Mathf.Sin(_rot.y);
        p2.y = p1.y;
        p2.z = -1 * p1.x * Mathf.Sin(_rot.y) + p1.z * Mathf.Cos(_rot.y);
        //Z Rotation
        Vector3 p3 = Vector3.zero;

        p3.x = p2.x * Mathf.Cos(_rot.z) - p2.y * Mathf.Sin(_rot.z);
        p3.y = p2.x * Mathf.Sin(_rot.z) + p2.y * Mathf.Cos(_rot.z);
        p3.z = p2.z;
        return p3 + _pivot;
    }
}
