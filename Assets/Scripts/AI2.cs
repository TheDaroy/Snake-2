using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI2 : MonoBehaviour
{
    #region Note
    // The Coroutine is an experiment to see if it improves performence, it might not make sence the way its implemented.
    #endregion

    #region Class Variables
    SnakeScript snake;

    TileScript startTile;
    TileScript targetTile;
    TileScript intermedietPathTarget; // A intermediet target for the pathfinder so that it does not have to do a long search every tick.

    List<TileScript> tilesToBeEvaluated;
    HashSet<TileScript> tilesAlreadyEvaluated;

    int longSearchTimer = 0; // Makes a long search when timer is 0.
    #endregion

    #region Awake,Sets Reference To Snake
    private void Awake()
    {       
        snake = GetComponent<SnakeScript>();
        snake.AI_active = true;
       
    }
    #endregion

    #region Pathfinder
    #region Starts Pathfinder & Sets Tick
    private void Start()
    {
        if (snake.AI_active)
        {
            longSearchTimer = snake.map.AIGroup();
            StartCoroutine(FindPath());
        }
    }
    IEnumerator FindPath() // Starts the pathfinder loop
    {
        while (!snake.isDead)
        {
            if (snake.map.tick)
            {
                startTile = snake.map.tileArray[(int)transform.position.x, (int)transform.position.y];
              
                tilesToBeEvaluated = new List<TileScript>();
                tilesAlreadyEvaluated = new HashSet<TileScript>();
                tilesToBeEvaluated.Add(startTile);
                #region Debug
                // Debug.Log("StartTile: " + startTile + " , Tick: " + tick);
                // Debug.Log("intermedietPathTarget: " + intermedietPathTarget);
                // Debug.Log((int)transform.position.x);
                #endregion

                if (startTile == targetTile)
                {
                    longSearchTimer = 0;
                }

                if (intermedietPathTarget != null && longSearchTimer != 0)
                {
                    targetTile = intermedietPathTarget;
                }
                else
                {
                    targetTile = snake.map.tileArray[(int)snake.map.appleArray[0].transform.position.x, (int)snake.map.appleArray[0].transform.position.y];
                }

                Path();
                longSearchTimer++;               
                if (longSearchTimer == 5)
                {
                    longSearchTimer = 0;
                }
            }
            yield return null;
        }        
    }
    #endregion

    #region Get Raw Path
    private void Path() // Main Loop, looks through tiles until it found a path to the target, or calls EvasiveMovements if no path can be found.
    {      
        while (tilesToBeEvaluated.Count > 0)
        {
            TileScript currentTile = tilesToBeEvaluated[0];
            
            currentTile = FindLowestF_cost(tilesToBeEvaluated, currentTile);
            #region Debug
            //Debug.Log("StartTile: " +startTile);
            //Debug.Log("TargetTile: "+targetTile);
            Debug.Log("Finding Path");
            #endregion

            tilesToBeEvaluated.Remove(currentTile);
            tilesAlreadyEvaluated.Add(currentTile);

            if (currentTile == targetTile)
            {
                Debug.Log("retracing");
                RetracePath(startTile, targetTile);
               
                return;
            }
            StartCoroutine(CheckNeiboringTiles(currentTile));            
        }

        if (tilesToBeEvaluated.Count <= 0)
        {
            Debug.Log("EvasiveManeuvers");
            EvasiveMovements();
        }
    }
    private TileScript FindLowestF_cost(List<TileScript> tilesToBeEvaluated, TileScript currentTile)
    {
        #region Cost Explanation
        // gCost = Distance fromn starting node
        // hCost = How far away from target node
        // fCost = gCost + hCost
        #endregion
        for (int i = 1; i < tilesToBeEvaluated.Count; i++)
        {
            if (tilesToBeEvaluated[i].fCost < currentTile.fCost || //
                tilesToBeEvaluated[i].fCost == currentTile.fCost && //
                tilesToBeEvaluated[i].hCost < currentTile.hCost) //
            {
                currentTile = tilesToBeEvaluated[i];
            }
        }
        return currentTile;
    }
    IEnumerator CheckNeiboringTiles(TileScript currentTile) // Finds the neiboring tiles (I know its spelled "Neighbour", but i wrote this at like 5am and i am just going with the new spelling at this point) and checks if any of them are closer. 
    {
        int i = 0;
        foreach (TileScript neigbhourTile in snake.map.GetNeighboringTiles(currentTile))
        {
            if (neigbhourTile.status == TileScript.tileStatus.Taken || tilesAlreadyEvaluated.Contains(neigbhourTile))
            {
                continue;
            }

            int newMovementCostToNeibor = currentTile.gCost + GetDistance(currentTile, neigbhourTile);
            if (newMovementCostToNeibor < neigbhourTile.gCost || !tilesToBeEvaluated.Contains(neigbhourTile)) // If new path is shorter or the neigbor is not in the ToBeEvaluated list, then set the fcost and make the currentile the parent to that node. 
            {
                neigbhourTile.gCost = newMovementCostToNeibor;
                neigbhourTile.hCost = GetDistance(neigbhourTile, targetTile);
                neigbhourTile.parent = currentTile;

                if (!tilesToBeEvaluated.Contains(neigbhourTile)) 
                {
                    tilesToBeEvaluated.Add(neigbhourTile);
                }
            }
            i++;
            
            if (i % 10 == 0)
            {
                yield return null;
            }
        }
    }
 
    int GetDistance(TileScript tileA, TileScript tileB) //Finds hCost
    {
        int dstX = Mathf.Abs((int)tileA.transform.position.x - (int)tileB.transform.position.x);
        int dstY = Mathf.Abs((int)tileA.transform.position.y - (int)tileB.transform.position.y);

        if (dstX > dstY)
        {
            return dstX + dstY;
        }
        return dstY + dstX; 
    }
    #endregion

    #region  Retracing Path For Movement
    void RetracePath(TileScript startTile, TileScript targetTile) // Retraces and reverses path for movement since its the wrong way around, also sets intermedietPath if possible.
    {
        List<TileScript> path = new List<TileScript>();
        TileScript currentTile = targetTile;
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        DebugList(path);
        path.Reverse();


        if (path.Count >= 5)
        {
            intermedietPathTarget = path[5 -1];
        }
        else 
        {
            int i = path.Count;
            intermedietPathTarget = path[i -1];
        }
        MoveTroughPath(path);
        snake.map.DrawPath(path);
    }
    #endregion
    #endregion

    #region  Movements
    void MoveTroughPath(List<TileScript> path)
    {
        //Debug.Log(path[0]);
        snake.AI_Input((int)path[0].transform.position.x, (int)path[0].transform.position.y);
    } // If path to target have been found
    void EvasiveMovements()
    {
        TileScript tile;

        //Right
        if (snake.currentX < snake.map.maxX -1)
        {
            if (snake.map.tileArray[startTile.x + 1, startTile.y].x <= snake.map.maxX + 1 &&
                        snake.map.tileArray[startTile.x + 1, startTile.y].x >= 0 &&
                        snake.map.tileArray[startTile.x + 1, startTile.y].y <= snake.map.maxY + 1 &&
                        snake.map.tileArray[startTile.x + 1, startTile.y].y >= 0 &&
                        snake.map.tileArray[startTile.x + 1, startTile.y].status != TileScript.tileStatus.Taken) //
            {
                tile = snake.map.tileArray[startTile.x + 1, startTile.y];
                snake.AI_Input((int)tile.transform.position.x, (int)tile.transform.position.y);
                return;
            }

        }

        else if (snake.currentX < snake.map.maxX -1)
        {
            //Left
            if (snake.map.tileArray[startTile.x - 1, startTile.y].x <= snake.map.maxX + 1 &&
            snake.map.tileArray[startTile.x - 1, startTile.y].x >= 0 &&
            snake.map.tileArray[startTile.x - 1, startTile.y].y <= snake.map.maxY + 1 &&
            snake.map.tileArray[startTile.x - 1, startTile.y].y >= 0 &&
            snake.map.tileArray[startTile.x - 1, startTile.y].status != TileScript.tileStatus.Taken)  //
            {
                tile = snake.map.tileArray[startTile.x - 1, startTile.y];
                snake.AI_Input((int)tile.transform.position.x, (int)tile.transform.position.y);
                return;
            }
        }
        else if (snake.currentY < snake.map.maxY -1)
        {
            //Up
            if (snake.map.tileArray[startTile.x, startTile.y + 1].x <= snake.map.maxX + 1 &&
            snake.map.tileArray[startTile.x, startTile.y + 1].x >= 0 &&
            snake.map.tileArray[startTile.x, startTile.y + 1].y <= snake.map.maxY + 1 &&
            snake.map.tileArray[startTile.x, startTile.y + 1].y >= 0 &&
            snake.map.tileArray[startTile.x, startTile.y + 1].status != TileScript.tileStatus.Taken)  //
            {
                tile = snake.map.tileArray[startTile.x, startTile.y + 1];
                snake.AI_Input((int)tile.transform.position.x, (int)tile.transform.position.y);
                return;
            }
        }

        else if (snake.currentY < snake.map.maxY-1)
        {
            //Down
            if (snake.map.tileArray[startTile.x, startTile.y - 1].x <= snake.map.maxX + 1 &&
            snake.map.tileArray[startTile.x, startTile.y - 1].x >= 0 &&
            snake.map.tileArray[startTile.x, startTile.y - 1].y <= snake.map.maxY + 1 &&
            snake.map.tileArray[startTile.x, startTile.y - 1].y >= 0 &&
            snake.map.tileArray[startTile.x, startTile.y - 1].status != TileScript.tileStatus.Taken) //
            {
                tile = snake.map.tileArray[startTile.x, startTile.y - 1];
                snake.AI_Input((int)tile.transform.position.x, (int)tile.transform.position.y);
                return;
            }
        }
    } // if path to target cant be found (EXPERIMENTAL, IGNORE, there is some stupid shit in here) 
    #endregion

    #region Misc Debug and Map Coloration (Update)
    private void Update()
    {
        if (snake.map.tick)
        {
            snake.map.PaintMapWhite();
        }

    }
    void DebugList(List<TileScript> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
           // Debug.Log(list[i]);
        }
    }
    #endregion
}