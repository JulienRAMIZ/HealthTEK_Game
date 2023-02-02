using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;

    public void Init(bool isOffset)
    {
        // .... ? ... : ... => ? = if quelque chose est vrai ?, si oui je fais �a sinon (:) je fais �a
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
}
