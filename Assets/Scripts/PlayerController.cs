using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

/* Script that controls the player */
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    [System.NonSerialized] public bool ableMoving, isMoving, isCalculated, closedDoor;
    public Vector3 target;
    private GridScript grid;
    public GameObject character;
    private Vector3 myTransform;
    private Vector3 calculatedTransform;

    // Unused variables
    private Vector2 target2D;
    private Tile tile;
    private GameObject[,] _openedTiles;
    private GameObject[,] _closedTiles;

    private void Start()
    {
        // We retrieve the scripts we need
        grid = GameObject.Find("GridManager").GetComponent<GridScript>();
        _openedTiles = new GameObject[grid._width, grid._height];
        _closedTiles = new GameObject[grid._width, grid._height];
        //grid._listTiles[(int)transform.position.x,(int)transform.position.y].tag = "OpenedDoor";
        grid._listTiles[(int)transform.localPosition.x,(int)transform.localPosition.y].tag = "OpenedDoor";

        myTransform = transform.position;
    }

    private void Update()
    {
        if (ableMoving == true)
        {
            // Calculate the movement to make between the player and the left click
            calculDestination();
            target.z = (float)-8.2;
            // Execute the movement if we respect one of the movement conditions 
            MoveCharacter();
        }
    }

    public void calculDestination()
    {
        if (Input.GetMouseButtonDown(0) && ableMoving == true && isMoving == false && closedDoor == false)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //target.z = transform.position.z;
            target.z = (float)-8.2;
            target2D = new Vector2(target.x, target.y);

            target.x = (float)Math.Round(target.x);
            target.y = (float)Math.Round(target.y);

            Debug.Log("x = " + target.x);
            Debug.Log("y = " + target.y);

            // If the click happens outside the maze, the player don't move at all. 
            /*if (target.x <= 0){ target.x = 0;} else if */
            if (target.x < 0 || target.x > grid._width - 1)
            {
                target.x = character.transform.position.x;

            }

            if (target.y < 0 || target.y > grid._height - 1)
            {
                target.y = character.transform.position.y;


            }
            isCalculated = true;
            calculatedTransform = target;
            ableMoving = true;
            /*else if (target.y >= grid._height - 1){ target.y = grid._height - 1; }*/
        }
    }

    public void MoveCharacter()
    {
        if (ableMoving == true && isCalculated == true)
        {
            isMoving = true;
            calculatedTransform = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            transform.position = calculatedTransform;
            
        }

        if (transform.position != target)
        {
            calculatedTransform = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            transform.position = calculatedTransform;
            if (transform.position == calculatedTransform)
            {
                isMoving = false;
            }
        }

        isCalculated = false;
    }
}
