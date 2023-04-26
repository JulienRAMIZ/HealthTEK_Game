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

    //Active la possibilité de recevoir une question
    public bool goQnA;
    //Si le pointer est proche du personnage (case adjacente) on peut activer la phase question
    public bool isClose = false;


    //Script qui agit sur chaque room crée depuis GridScript

    public void Start()
    {
        //On récupère les script dont on a besoin
        player = GameObject.Find("Character").GetComponent<PlayerController>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void Init(bool isOffset)
    {
        //permet de changer de couleur 1 room sur 2 (référence dans GridScript)
        // explication de la syntaxe .... ? ... : ... => ? = if quelque chose est vrai (?), si oui je fais ça sinon (:) je fais ça
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }


    //Lorsque la souris entre dans une room
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Limitation des mouvements aux cases adjacentes 
        if (player.transform.position.x - transform.position.x == -1 || player.transform.position.x - transform.position.x == 1 || player.transform.position.y - transform.position.y == -1 || player.transform.position.y - transform.position.y == 1)
        {
            if (player.transform.position.x - transform.position.x == 0 ||  player.transform.position.y - transform.position.y == 0 )
            {
                isClose = true;
                Debug.Log("en x on a : " + (player.transform.position.x - this.transform.position.x) + " Et en y on a : " + (player.transform.position.y - this.transform.position.y));
            }

        }
        // le tag OpenedDoor signifie que l'on a répondu correctement à la question de la room et qu'on peut donc avancer dedans, par défaut les room ont le tag ClosedDoor
        if (CompareTag("OpenedDoor") == true)
        {
            goQnA = false;
            _redHighlight.SetActive(false);
            _whiteHighlight.SetActive(true);
            if (isClose) { player.ableMoving = true; }
        }
        else
        {
            _redHighlight.SetActive(true);
            player.ableMoving = false;
            goQnA = true;

        }

    }

    //Lorsque la souris sort d'une room
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _whiteHighlight.SetActive(false);
        _redHighlight.SetActive(false);

        isClose = false;

    }

    //Lorsqu'on fait un clic gauche en étant sur la room
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _redHighlight == true && isClose == true && goQnA)
        {
            manager.tileX = (int)transform.position.x;
            manager.tileY = (int)transform.position.y;
            Debug.Log("position du tile : " + manager.tileX + " , " + manager.tileY);
            manager.PopUpQuestion();
        }

    }

}
