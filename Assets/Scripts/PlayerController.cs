using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

/* Script that controls the player */
public class PlayerController : MonoBehaviour
{
    public float speed = 50f;
    [System.NonSerialized] public bool ableMoving, isMoving;
    private Vector3 target;
    private GridScript grid;

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
        grid._listTiles[(int)transform.position.x,(int)transform.position.y].tag = "OpenedDoor";
    }

    private void Update()
    {
        if (ableMoving == true)
        {
            // Calculate the movement to make between the player and the left click
            calculDestination();
            // Execute the movement if we respect one of the movement conditions 
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

            // If the click happens inside the maze, the player moves to the closest square. 
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
