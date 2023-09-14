using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using static UnityEngine.GraphicsBuffer;

/* Script that acts on each room from the GridScript */
public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] public GameObject _room;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _exit;
    [SerializeField] private GameObject _greenHighlight;
    [SerializeField] private GameObject _whiteHighlight;
    [SerializeField] private GameObject _redHighlight;
 
    public PlayerController player;
    // Variable that allows the possibility to receive the question
    public bool goQnA = true;
    // If the pointer is close  to the player (adjacent square), we can pop up the question
    public bool isClose = false;
    public bool goingToExitRoom = false;
    public bool isPositionned = false;
    public bool questionpoped = false;
    private Vector3 playerTarget = new Vector3();

    private GameManager manager;

    public void Start()
    {
        // We retrieve the scripts we need
        player = GameObject.Find("Character").GetComponent<PlayerController>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Update()
    {   
        playerTarget = player.target;

        if (CompareTag("Obstacle") == true)
        {
            _redHighlight.SetActive(true);
        }

        if (CompareTag("OpenedDoor") == true)
        {
            _greenHighlight.SetActive(true);
        }

        if (player.transform.position.x != (int)player.transform.position.x || player.transform.position.y != (int)player.transform.position.y)
        {
            player.ableMoving = true;
            player.isMoving = true;
        }
        else
        {
            player.isMoving = false;
        }

        if (player.isMoving == !player.isMoving)
        {
            Debug.Log("Oula changement du bool isMoving ... " + player.isMoving);

        }

    }
    public void Init(bool isOffset)
    {
        // Change the color every other square (reference in the GridScript)
        // Syntax explanation.... ? ... : ... => ? = if something is true (?), if yes I do this otherwise (:) I do that
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        if( _renderer.transform.position.x == 4 && _renderer.transform.position.y == 4)
        {
            _renderer.sprite  = _exit;

        }
    }
    // When the mouse enters in a room
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        // Limit the movements to the adjacent squares
        if (player.transform.position.x - transform.position.x == -1 || player.transform.position.x - transform.position.x == 1 || player.transform.position.y - transform.position.y == -1 || player.transform.position.y - transform.position.y == 1)
        {

            if (player.transform.position.x - transform.position.x == 0 || player.transform.position.y - transform.position.y == 0)
            {
                isClose = true;
            }    
        }

        // The OpenedDoor tag means that the player answered correctlty and that he can move to the room. The default tag is ClosedDoor.

        if (CompareTag("OpenedDoor") == true)
        {
            player.closedDoor = false;
            goQnA = false;
            _whiteHighlight.SetActive(false);
            _greenHighlight.SetActive(true);

            if (isClose && manager.questionPopped == false)
            {
                player.ableMoving = true;
                Debug.Log("Close it is !");
            }
            else
            {
                Debug.Log("Plus close du tout ! ");
                player.ableMoving = false;
            }
        }

        if (CompareTag("Obstacle") == true)
        {
            goQnA = false;
            player.ableMoving = false;
        }

        if (CompareTag("ExitRoom") == true)
        {
            goQnA = true;
            player.ableMoving = false;
        }

        if (CompareTag("ClosedDoor") == true)
        {
            player.closedDoor = true;
            _whiteHighlight.SetActive(true);
            player.ableMoving = false;
            if (isClose)
            {
                goQnA = true;
            }
        }

    }

    // When the mouse exits a room
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _greenHighlight.SetActive(false);
        _whiteHighlight.SetActive(false);
        isClose = false;
    }

    // When we do a left click while being on a room
    public void OnPointerDown(PointerEventData eventData)
    { 
        if (eventData.button == PointerEventData.InputButton.Left && _whiteHighlight == true && isClose == true && goQnA && manager.questionPopped == false) 
        {
            manager.tileX = (int)transform.position.x;
            manager.tileY = (int)transform.position.y;
            questionpoped = true;
            manager.PopUpQuestion();  
        }
        else if (eventData.button == PointerEventData.InputButton.Left && _whiteHighlight == true && isClose == false) // and tag différet de opende door
        {
            StartCoroutine(manager.ShowMessage("You can't move here.", 3));
        }
        else if (eventData.button == PointerEventData.InputButton.Left && _redHighlight == true && isClose == true) // and tag différet de opende door
        {
            StartCoroutine(manager.ShowMessage("You can't attempt again.", 3));
        }
        else if (eventData.button == PointerEventData.InputButton.Left && _redHighlight == true && isClose == false) // and tag différet de opende door
        {
            StartCoroutine(manager.ShowMessage("You can't move here.", 3));
        }
        else if (eventData.button == PointerEventData.InputButton.Left && goQnA && goingToExitRoom == true)
        {
            manager.tileX = (int)transform.position.x;
            manager.tileY = (int)transform.position.y;
            Debug.Log("position du tile : " + manager.tileX + " , " + manager.tileY);
            manager.PopUpQuestion();
            questionpoped = true;
        }
    }
}
