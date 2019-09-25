using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    MapScript map;
    private void Awake()
    {
        // gCost = Distance fromn starting node
        // hCost = How far away from target node
        // fCost = gCost + hCost
        map = GetComponent<MapScript>();
    }
    private readonly static object testLock = new object();

    TileScript targetTile;


    List<TileScript> tilesToBeEvaluated;
    HashSet<TileScript> tilesAlreadyEvaluated;
    List<TileScript> finalPath;
    bool foundPath = false;
    
    public List<TileScript> FindPath(TileScript startTile, TileScript targetTile)
    {

        tilesToBeEvaluated = new List<TileScript>();
        tilesAlreadyEvaluated = new HashSet<TileScript>();
        finalPath = new List<TileScript>();
        this.targetTile = targetTile;
        tilesToBeEvaluated.Add(startTile);

        Path();

        if (foundPath)
        {
            finalPath = RetracePath(startTile, targetTile);
        }
        
        
        return finalPath;

    }

    private void Path()
    {
        while (tilesToBeEvaluated.Count > 0)
        {
            TileScript currentTile = tilesToBeEvaluated[0];

            currentTile = FindLowestF_cost(tilesToBeEvaluated, currentTile);

            //Debug.Log("StartTile: " +startTile);
            //Debug.Log("TargetTile: "+targetTile);


            tilesToBeEvaluated.Remove(currentTile);
            tilesAlreadyEvaluated.Add(currentTile);

            if (currentTile == targetTile)
            {
                foundPath = true;

                return;
            }


           // CheckNeiboringTiles(tilesAlreadyEvaluated, currentTile);
        }

    }


    private TileScript FindLowestF_cost(List<TileScript> tilesToBeEvaluated, TileScript currentTile)
    {
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
   private void CheckNeiboringTiles(HashSet<TileScript> tilesAlreadyEvaluated, TileScript currentTile, List<TileScript> testThingy)
    {
        
            foreach (TileScript neigbhourTile in map.GetNeighboringTiles(currentTile))
            {
                if (neigbhourTile.status == TileScript.tileStatus.Taken || tilesAlreadyEvaluated.Contains(neigbhourTile))
                {
                    continue;
                }

                
                int newMovementCostToNeibor = currentTile.gCost + GetDistance(currentTile, neigbhourTile);
                if (newMovementCostToNeibor < neigbhourTile.gCost || !tilesToBeEvaluated.Contains(neigbhourTile))
                {
                    neigbhourTile.gCost = newMovementCostToNeibor;
                    neigbhourTile.hCost = GetDistance(neigbhourTile, targetTile);
                    neigbhourTile.parent = currentTile;

                    if (!tilesToBeEvaluated.Contains(neigbhourTile))
                    {
                        tilesToBeEvaluated.Add(neigbhourTile);               

                    }
            }
           
            }
        
        
        
    }
    
    int GetDistance(TileScript tileA, TileScript tileB)
    {
        int dstX = Mathf.Abs(tileA.x - tileB.x);
        int dstY = Mathf.Abs(tileA.y - tileB.y);

        if (dstX > dstY)
        {
            return 1;
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
    List<TileScript> RetracePath(TileScript startTile, TileScript targetTile)
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
        
        //  map.DrawPath(path);
        return path;
    }


    void DebugList(List<TileScript> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            // Debug.Log(list[i]);
        }
    }

    

}

