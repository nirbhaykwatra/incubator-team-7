#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Internment.Digging.Terrain;

[ExecuteInEditMode]
[RequireComponent(typeof(Marching))]
public class ChunkBoundaryGizmo : MonoBehaviour
{
    public Color color = Color.green;
    public bool showAlways = false;

    void OnDrawGizmos()
    {
        // only draw when selected (unless showAlways)
        if (!showAlways && !Selection.Contains(gameObject))
            return;

        // pull width/height straight from your Marching script
        var marching = GetComponent<Marching>();
        if (marching == null) return;

        int w = marching.width + 1;
        int h = marching.height + 1;
        int l = marching.length + 1;

        Gizmos.color = color;
        Vector3 size = new Vector3(w, h, l);
        Vector3 center = transform.position + size * 0.5f;
        Gizmos.DrawWireCube(center, size);
    }
}
#endif