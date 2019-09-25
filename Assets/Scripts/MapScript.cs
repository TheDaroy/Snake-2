using UnityEngine;
using System.Collections.Generic;
using System;

public class MapScript : MonoBehaviour
{
    #region References & Class Variables
    #region IGNORE. Experimental
    public PathManager pathManager; 
    public PathFinder pathFinder;
    #endregion

    public static MapScript instance;
    public TileScript [,] tileArray;
    public GameObject tile;

    public AppleScript[] appleArray;
    public GameObject apple;
    public int nrOfApples = 1; // The AI can only look for one apple atm.

    public SnakeScript[] snakeArray;
    public GameObject snake;
    public int nrOfPlayers = 0; // Dont set to more then one atm. Yes, it could be a bool. No, i am not going to change it.
    public int nrOfAI = 0;

    // Map size 
    public int maxX = 20;
    public int maxY = 10;

    // Game Speed
    public float secondsPerTick = 0.5f; 
    float internalTimer = 0;
    [NonSerialized] public bool tick = false;


    int AI_NR = 0;
    int player_NR = 0;
    int snake_NR = 0;
    int AI_Counter = 0;

    public List<TileScript> finalPath;
    #endregion

    #region Awake Functions, Sets Map & Camera Position & Snakes
    private void Awake()
    {
        pathManager = GetComponent<PathManager>();
        pathFinder = GetComponent<PathFinder>();
        InstanceMap();
        SetCamera();
        CreateMap();
        CreateSnakes();      
    }

    private void SetCamera()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = maxX / maxY;
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = (maxY / 2) +1 ;
            Camera.main.transform.position = new Vector3(maxX / 2, maxY / 2, -1);
        }   
    } // Sets the camera to match the map size
    private void InstanceMap()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    } // Magic, dont touch. Sometimes it works, sometimes it dont.
    private void CreateMap()
    {
        tileArray = new TileScript[maxX, maxY];
        GameObject gameMap = GameObject.FindGameObjectWithTag("Gamemap");
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                GameObject temp = Instantiate(tile, new Vector2(x, y), Quaternion.identity,gameMap.transform);
                temp.name = "Tile" + x.ToString() + "." + y.ToString();
                temp.GetComponent<TileScript>().x = x;
                temp.GetComponent<TileScript>().y = y;

                tileArray[x, y] = temp.GetComponent<TileScript>();
            }

        }
    }
    private void CreateSnakes()
    {
        GameObject temp;
        snakeArray = new SnakeScript[nrOfPlayers + nrOfAI];


        for (int i = 0; i < nrOfPlayers; i++)
        {
            player_NR++;     
            temp = Instantiate(snake);
            temp.GetComponent<SnakeScript>().AI_active = false;
            temp.name = "Player Snake" + player_NR;
            snakeArray[snake_NR] = temp.GetComponent<SnakeScript>();
            snake_NR++;
        }

        for (int i = 0; i < nrOfAI; i++)
        {         
            AI_NR++;
            temp = Instantiate(snake);
            temp.GetComponent<AI2>().enabled = true;          
            temp.name = "AI Snake" + AI_NR;
            snakeArray[snake_NR] = temp.GetComponent<SnakeScript>();
            snake_NR++;
        }

        
    } // Needs a revisit in the future
    public int AIGroup() // Between 0-5, organizes the different AIs into groups so that they dont do a "long search" at the same time
    {
        if (AI_Counter >= 5)
        {
            AI_Counter = 0;
        }
        AI_Counter++;
        return AI_Counter;
        
    }
    #endregion

    #region Start, Spawns Apples
    private void Start()
    {
        SpawnApples();
    }
    private void SpawnApples()
    {
        appleArray = new AppleScript[nrOfApples];
        for (int i = 0; i < nrOfApples; i++)
        {
           GameObject temp = Instantiate(apple);
            apple.name = "Apple " + i;
            appleArray[i] = temp.GetComponent<AppleScript>();
        }
    }
    #endregion

    #region Position Calculators For Snakes & Apples
    public (int,int) StartPositionCalculator()
    {
        int startX = 0;
        int startY = 0;
        bool loop = true;
        while (loop)
        {
            startX = UnityEngine.Random.Range(1, maxX);
            startY = UnityEngine.Random.Range(1, maxY);
            if (tileArray[startX, startY].status == TileScript.tileStatus.Free)
            {
                loop = false;
            }

        }
    
        return (startX,startY);
    }
    public (int, int) NewApplePosition()
    {
        int appleX = 0;
        int appleY = 0;
        bool loop = true;
        while (loop)
        {
            appleX = UnityEngine.Random.Range(1, maxX);
            appleY = UnityEngine.Random.Range(1, maxY);
            if (tileArray[appleX, appleY].status == TileScript.tileStatus.Free)
            {
                loop = false;
            }

        }

        return (appleX, appleY);
    }
    #endregion

    #region Update, Sets Tick (GameSpeed)
    private void Update()
    {
        MovementTimer();
        Debug.Log(tick);


    }

    public void MovementTimer()
    {
        if (internalTimer <= secondsPerTick)
        {
            internalTimer += Time.deltaTime;
            tick = false;
        }
        else
        {
            tick = true;
            internalTimer = 0;
        }
    }
    #endregion

    #region Snake AI Functions
    public List<TileScript> GetNeighboringTiles(TileScript _tile)
    {
        List<TileScript> neighboringTiles = new List<TileScript>();

        int xCheck;
        int yCheck;
        //Right
        xCheck = (int)_tile.x + 1;
        yCheck = (int)_tile.y;
        if (xCheck >= 0 && xCheck < maxX)
        {
            if (yCheck >= 0 && yCheck <maxY)
            {
                neighboringTiles.Add(tileArray[xCheck, yCheck]);
                
            }
        }
        //Left
        xCheck = (int)_tile.x - 1;
        yCheck = (int)_tile.y;
        if (xCheck >= 0 && xCheck < maxX)
        {
            if (yCheck >= 0 && yCheck < maxY)
            {
                neighboringTiles.Add(tileArray[xCheck, yCheck]);
                
            }
        }
        //Up
        xCheck = (int)_tile.x;
        yCheck = (int)_tile.y + 1;
        if (xCheck >= 0 && xCheck < maxX)
        {
            if (yCheck >= 0 && yCheck < maxY)
            {
                neighboringTiles.Add(tileArray[xCheck, yCheck]);
                
            }
        }
        //Down
        xCheck = (int)_tile.x ;
        yCheck = (int)_tile.y - 1;
        if (xCheck >= 0 && xCheck < maxX)
        {
            if (yCheck >= 0 && yCheck < maxY)
            {
                neighboringTiles.Add(tileArray[xCheck, yCheck]);
                
            }
        }
       // DrawtNeighboringTiles(neighboringTiles);
        return neighboringTiles;
    }

    #region Coloring Functions, Draws AI Path
    public void DrawPath(List<TileScript> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            path[i].GetComponent<SpriteRenderer>().color = Color.black;
         
        }
    }

    public void PaintMapWhite()
    {
        foreach (TileScript tile in tileArray)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    #endregion

    #endregion
}