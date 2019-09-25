using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using System.Threading;

public class AI3 : MonoBehaviour 
{
    SnakeScript snake;
    public TileScript currentTile;
    public List<TileScript> pathToTake = new List<TileScript>();
    
    int tick = 0;
    private void Awake()
    {
        // gCost = Distance fromn starting node
        // hCost = How far away from target node
        // fCost = gCost + hCost
        snake = GetComponent<SnakeScript>();
        snake.AI_active = true;

    }
    private void Start()
    {
        tick = snake.map.AIGroup();
       
    }
    private void Update()
    {
        currentTile = snake.map.tileArray[snake.currentX, snake.currentY];
        targetTile = snake.map.appleArray[0].currentTile;
        if (snake.map.tick)
        {
            GetPath();
            MoveTroughPath();
        }   
    }


    public TileScript targetTile;
    public TileScript intermedietTile;
   
    private void GetPath()
    {         
        Debug.Log("Path to take count: "+ pathToTake.Count);          
            if (tick >= 5)
            {
                tick = 0;
            }      
    }

    public void MoveTroughPath()
    {
        //Debug.Log(path[0]);
        if (pathToTake == null)
        {
            Debug.Log("Path is Null");

        }
        snake.map.DrawPath(pathToTake);
        Debug.Log((int)pathToTake[0].transform.position.x + " " + (int)pathToTake[0].transform.position.y);
        snake.AI_Input((int)pathToTake[0].transform.position.x, (int)pathToTake[0].transform.position.y);
    }

}

