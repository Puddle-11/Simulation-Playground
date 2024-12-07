using UnityEngine;
[ExecuteAlways]
public class ColorSetter : MonoBehaviour
{
    [SerializeField] private Texture2D _colorAtlas;
    [SerializeField] private Vector2Int colorCoordinate;
    public void UpdateColor()
    {
        if(_colorAtlas == null)
        {
            Debug.LogWarning("Color Atlas for " + gameObject.name + " not assigned");
            return;
        }
        Color c = _colorAtlas.GetPixel(colorCoordinate.x, colorCoordinate.y);
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            mr.material.color = c;
        }
        else
        {
            Debug.LogWarning("Mesh renderer for " + gameObject.name + " not assigned");
        }
    }
    public Color GetColor()
    {
        if (colorCoordinate.x >= _colorAtlas.width && colorCoordinate.x < 0) return Color.cyan;
        if (colorCoordinate.y >= _colorAtlas.height && colorCoordinate.y < 0) return Color.cyan;

        return _colorAtlas.GetPixel(colorCoordinate.x, colorCoordinate.y);

    }

}
