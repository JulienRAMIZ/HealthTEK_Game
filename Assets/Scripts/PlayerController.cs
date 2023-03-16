using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    Vector3 mousePos = new Vector3();
    public float speed = 50f;
    private Vector3 target;
    private Vector2 target2D;
    private GridScript grid;


    private void Start()
    {
        grid = GameObject.Find("GridManager").GetComponent<GridScript>();
    }

    private void Update()
    {
        //Le perso se déplace via clic gauche, sa position ira au centre de la case voulu
        if (Input.GetMouseButtonDown(0))
        {

            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            target2D = new Vector2(target.x, target.y);

            //Debug.Log("target.x =  " + target.x);
            //Debug.Log("target.y =  " + target.y);
            //Debug.Log("grid largeur   " + grid._width);
            //Debug.Log("grid hauteur   " + grid._height);
          
            target.x = (float)Math.Round(target.x);
            target.y = (float)Math.Round(target.y);

        }

        //Si le clic est à l'extérieur du maze, le perso va à la case la plus proche

        if (target.x <= 0)
        {
            target.x = 0;
        }
        else if (target.x >= grid._width-1)
        {
            target.x = grid._width-1;
        }

        if (target.y <= 0)
        {
            target.y = 0;
        }
        else if (target.y >= grid._height-1)
        {
            target.y = grid._height-1;
        }

        transform.position = Vector3.MoveTowards(transform.position,target, speed * Time.deltaTime);

            //DetectObjectWithRaycast();



        

    }

    //Raycast2D ne marche pas super, je le garde au cas où ça pourrait servir
    public void DetectObjectWithRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition,Vector2.right);

            //If the collider of the object hit is not NUll
            if (hit.collider != null)
            {
                //Hit something, print the tag of the object
                Debug.Log("Hitting: " + hit.collider.tag);
            }
        }
    }

}
