using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridScript : MonoBehaviour
{
    [SerializeField] public Tile _tilePrefab;
    [SerializeField] public int _width, _height;
    [SerializeField] private Transform _cam;
    [SerializeField] private GameObject _exitRoom;

    private GameObject _room;
    public GameObject[,] _listTiles;

    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
        _listTiles[2, 2].tag = "ClosedDoor";
    }

    // Generate the 5x5 square, defines the rooms' names le nom des room and place the camera, no changes expexted here except (maybe) the camera position 
    public void GenerateGrid()
    {
        _listTiles = new GameObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Originally spawnedTile was a var
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y,(float)1), Quaternion.identity);
                spawnedTile.name = $"room {x} {y}";
                _room = GameObject.Find("room " + x + " " + y);
                _listTiles[x, y] = _room;
                _listTiles[x, y].tag = "ClosedDoor";

                //equivalent to :var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                var isOffset = (x % 2 != y % 2);
                spawnedTile.Init(isOffset);
            }
        }

        //Assign the tags for the exit room and the ones near it.
        _listTiles[_width-1, _height-1].tag = "ExitRoom";
       // _listTiles[_width - 1, _height - 2].tag = "RoomDownExitRoom";
       // _listTiles[_width - 2, _height - 1].tag = "RoomLeftExitRoom";
        _listTiles[_width - 1, _height - 1] = _exitRoom;

        //Camera position
        _cam.transform.position = new Vector3((float)_width / 2 + 5.5f, (float)_height / 2 - 0.5f, -10);


        /* Idea suspended for now 
        /*
         * //Creation of random obstacles. Obstacles are dead ends. 
            int numberObstacles = 0;
            //bool numberObstaclesReached = false;
            while (numberObstacles != 5)
            {
                int RandomX = Random.Range(0, _width-1);
                int RandomY = Random.Range(0, _height-1);
                if (RandomX != _width-1 && RandomY != _height-1 && !_listTiles[RandomX, RandomY].CompareTag("Obstacle"))
                {
                    _listTiles[RandomX, RandomY].tag = "Obstacle";
                    numberObstacles++;
                }
             }
        */
    }   
}
