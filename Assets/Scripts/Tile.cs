using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public GameObject _room;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    
    private PlayerController player;
    private GridScript grid;


    public void Start()
    {
        player = GameObject.Find("Character").GetComponent<PlayerController>();
    }
    public void Init(bool isOffset)
    {
        // .... ? ... : ... => ? = if quelque chose est vrai ?, si oui je fais ça sinon (:) je fais ça
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);

        if (this.CompareTag("OpenedDoor") == true)
        {   
            Debug.Log("test");
            player.ableMoving = true;
        }
        else
        {
            //player.ableMoving = false;
        }
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        if (player.isMoving == false)
        {
            player.ableMoving = false;
        }

    }
}
