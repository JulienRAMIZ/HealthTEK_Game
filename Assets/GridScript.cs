using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridScript : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private int _width, _height;
    [SerializeField] private Transform _cam;


    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }


    public void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"room {x} {y}";


                //var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                var isOffset = (x % 2 != y % 2);
                spawnedTile.Init(isOffset);

            }
        }
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

    }
}
