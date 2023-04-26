using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridScript : MonoBehaviour
{
    [SerializeField] public Tile _tilePrefab;
    [SerializeField] public int _width, _height;
    [SerializeField] private Transform _cam;

    private GameObject _room;
    public GameObject[,] _listTiles;



    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
        _listTiles[2, 2].tag = "ClosedDoor";
    }


    //G�n�re le carr� 5x5 de room, d�finit le nom des room et place la cam�ra, pas de changement pr�vu ici si ce n'est la position de la cam�ra au cas o�
    public void GenerateGrid()
    {
        _listTiles = new GameObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // De base spawnedTile �tait un var
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y,(float)1), Quaternion.identity);
                spawnedTile.name = $"room {x} {y}";
                _room = GameObject.Find("room " + x + " " + y);
                _listTiles[x, y] = _room;
                _listTiles[x, y].tag = "ClosedDoor";

                //�quivalent de :var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                var isOffset = (x % 2 != y % 2);
                spawnedTile.Init(isOffset);

            }
        }
        
        //Camera position
        _cam.transform.position = new Vector3((float)_width / 2 + 5.5f, (float)_height / 2 - 0.5f, -10);

    }

}
