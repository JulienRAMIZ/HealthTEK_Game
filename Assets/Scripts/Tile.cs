using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] public GameObject _room;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _whiteHighlight;
    [SerializeField] private GameObject _redHighlight;
    
    public PlayerController player;
    private GridScript grid;
    private GameManager manager;
    public bool goQnA;
    public bool isClose = false;



    public void Start()
    {
        player = GameObject.Find("Character").GetComponent<PlayerController>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void Init(bool isOffset)
    {
        // .... ? ... : ... => ? = if quelque chose est vrai ?, si oui je fais ça sinon (:) je fais ça
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (player.transform.position.x - transform.position.x == -1 || player.transform.position.x - transform.position.x == 1 || player.transform.position.y - transform.position.y == -1 || player.transform.position.y - transform.position.y == 1)
        {
            if (player.transform.position.x - transform.position.x == 0 ||  player.transform.position.y - transform.position.y == 0 )
            {
                isClose = true;
                Debug.Log("en x on a : " + (player.transform.position.x - this.transform.position.x) + " Et en y on a : " + (player.transform.position.y - this.transform.position.y));
            }

        }

        if (CompareTag("OpenedDoor") == true)
        {
            _redHighlight.SetActive(false);
            _whiteHighlight.SetActive(true);
            if (isClose) { player.ableMoving = true; }
        }
        else
        {
            _redHighlight.SetActive(true);
            player.ableMoving = false;

        }

    }


    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _whiteHighlight.SetActive(false);
        if (goQnA == false) 
        { 
            _redHighlight.SetActive(false); 
        
        }
        isClose = false;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _redHighlight == true && isClose == true)
        {
            goQnA = true;
            manager.PopUpQuestion();
        }

    }
    public void CorrectAnswer()
    {
        if (manager.CorrectChoice) 
        {
            this.tag = "OpenedDoor";
            _whiteHighlight.SetActive(true);
            player.ableMoving = true;


        }
    }
}
