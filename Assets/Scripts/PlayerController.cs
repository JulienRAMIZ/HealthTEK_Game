using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 50f;
    [System.NonSerialized] public bool ableMoving, isMoving;
    private Vector3 target;
    private Vector2 target2D;
    private GridScript grid;
    private Tile tile;

    private GameObject[,] _openedTiles;
    private GameObject[,] _closedTiles;


    private void Start()
    {
        //On récupère les script dont on a besoin
        grid = GameObject.Find("GridManager").GetComponent<GridScript>();
        _openedTiles = new GameObject[grid._width, grid._height];
        _closedTiles = new GameObject[grid._width, grid._height];
        grid._listTiles[(int)transform.position.x,(int)transform.position.y].tag = "OpenedDoor";

        

    }

    private void Update()
    {
        
        if (ableMoving == true)
        {
            //Calcul le déplacement à faire entre le personnage et le clic gauche
            calculDestination();
            //Exécute le mouvement si on est dans une condition de déplacement
            MoveCharacter();
        }


    }

        
    


    public void calculDestination()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            target2D = new Vector2(target.x, target.y);

            target.x = (float)Math.Round(target.x);
            target.y = (float)Math.Round(target.y);

            //Si le clic est à l'extérieur du maze, le perso va à la case la plus proche

            if (target.x <= 0)
            {
                target.x = 0;
            }
            else if (target.x >= grid._width - 1)
            {
                target.x = grid._width - 1;
            }

            if (target.y <= 0)
            {
                target.y = 0;
            }
            else if (target.y >= grid._height - 1)
            {
                target.y = grid._height - 1;
            }


        }
        

    }

    public void MoveCharacter()
    {
        if (ableMoving == true )
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        }

    }

}
