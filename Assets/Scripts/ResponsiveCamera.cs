using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class ResponsiveCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private (Vector3 center, float size) CalculatorOrthoSize()
    {
        var bounds = new Bounds();
        foreach (var col in FindAnyObjectByType<Collider2D>()) bounds.Encapsulate(col.bounds);
        bounds.Expand(_buffer);
        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * _campixelHeight / _cam.pixelWidth;
        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0,0,-10);

        return (center, size);
    }

}
