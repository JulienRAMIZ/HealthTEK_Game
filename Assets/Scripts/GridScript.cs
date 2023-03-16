using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridScript : MonoBehaviour
{
    [SerializeField] public Tile _tilePrefab;
    [SerializeField] public int _width, _height;
    [SerializeField] private Transform _cam;
    public Dictionary<Vector2, Tile> _tiles;


    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
    }


    public void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y,(float)1), Quaternion.identity);
                spawnedTile.name = $"room {x} {y}";

                //var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                var isOffset = (x % 2 != y % 2);
                spawnedTile.Init(isOffset);


                //_tiles[new Vector2(x, y)] = spawnedTile;

            }
        }
        
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

}
