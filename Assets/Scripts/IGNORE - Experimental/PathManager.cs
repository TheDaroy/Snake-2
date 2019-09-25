using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
public class PathManager : MonoBehaviour
{
    MapScript map;
    private Thread[] threadArray;
    List<Thread> threadList;
    Thread testThread;
    private void Awake()
    {
        map = GetComponent<MapScript>();
      
    }
    bool localTick = true;
    private void Update()
    {
        if (localTick)
        {
            localTick = false;
            pathFinding();
           
        }
        testCheck();
        
    }
    void testCheck()
    {
        if (map.tick)
        {
            localTick = true;
        }
    }

    void pathFinding()
    {
        int threadNR = 0;
        foreach (SnakeScript snake in map.snakeArray)
        {
            Debug.Log(snake);

        }
        threadList = new List<Thread>();
        foreach (SnakeScript snake in map.snakeArray)
        {
           

            if (snake.AI_active && !snake.isDead)
            {
                Debug.Log("Adding thread to list");
              //  testThread = new Thread(() => snake.AI.pathToTake = map.pathFinder.FindPath(snake.AI.currentTile, snake.AI.targetTile));
                threadList.Add(testThread);
            }
        }

        foreach ( Thread thread in threadList)
        {
            Debug.Log("starting thread: " + threadNR);
            thread.Start();
            threadNR++;
            


        }

        foreach (Thread thread in threadList)
        {
            thread.Join();
        }
    }




}
