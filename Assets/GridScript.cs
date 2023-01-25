using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    [SerializeField] private GameObject room;
    [SerializeField] private int _width, _height;
    [SerializeField] private Transform _cam;


    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                room.name = $"room {x} {y}";
                Instantiate(room,new Vector3(x,y), Quaternion.identity);
                
            }
        }
        _cam.transform.position = new Vector3(_width / 2 -0.5f, _height / 2 -0.5f, -10);
    }
}
