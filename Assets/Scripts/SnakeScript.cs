using System;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    #region References & Class Variables

    //References
    [NonSerialized] public MapScript map;
    private SnakeListScript snakelist;

    //
    [NonSerialized] public bool isDead = false;
    [NonSerialized] public bool AI_active = false;

    //Current x and y position
    [NonSerialized] public int currentX;
    [NonSerialized] public int currentY;

    //Direction snake is going towards
    [NonSerialized] public int activeDirection;

    
    #endregion

    #region Awake Functions, Sets Reference For Map & SnakeList 
    private void Awake()
    {
        map = GameObject.Find("GameManager").GetComponent<MapScript>();
        snakelist = GetComponent<SnakeListScript>();
    }
    #endregion

    #region Start Functions, Create Snake Body
    private void Start()
    {
        SetSnake();
    }

    private void SetSnake()
    {
        int x;
        int y;
        (x, y) = map.StartPositionCalculator();
        currentX = x;
        currentY = y;
        transform.position = new Vector2(x, y);

        snakelist.list.CreateSnake(x, y, true);
        snakelist.list.CreateSnake(x - 1, y, false);
        snakelist.list.CreateSnake(x - 2, y, false);
    }
    #endregion

    #region Update, Looks If isDead or AI_active 
    private void Update()
    {
        Debug.Log(AI_active);
        if (!AI_active)
        {
            PlayerInput();
        }
        if (!isDead && map.tick)
        {
            SetMovementDirection();
        }
    }
    #endregion

    #region Player & AI Input
    void PlayerInput()
    {
        if (Input.GetButtonUp("Up") && activeDirection != 2)
        {
            activeDirection = 1;
        }

        if (Input.GetButtonUp("Down") && activeDirection != 1)
        {
            activeDirection = 2;
        }

        if (Input.GetButtonUp("Left") && activeDirection != 4)
        {
            activeDirection = 3;
        }
        if (Input.GetButtonUp("Right") && activeDirection != 3)
        {
            activeDirection = 4;
        }
    }
    public void AI_Input(int newX, int newY)
    {

        if (newY > currentY)
        {
            activeDirection = 1;
        }
        if (newY < currentY)
        {
            activeDirection = 2;
        }
        if (newX < currentX)
        {
            activeDirection = 3;
        }
        if (newX > currentX)
        {
            activeDirection = 4;
        }
    }
    #endregion

    #region Setting Movement Direction
    private void SetMovementDirection()
    {
        switch (activeDirection)
        {
            case 1:
                currentY = currentY + 1;
                
                snakelist.list.CollisionCheck(currentX, currentY);
                MovingSnake();

                break;
            case 2:
                currentY = currentY - 1;
                
                snakelist.list.CollisionCheck(currentX, currentY);
                MovingSnake();
                break;
            case 3:
                currentX = currentX - 1;
                
                snakelist.list.CollisionCheck(currentX, currentY);
                MovingSnake();
                break;
            case 4:
                currentX = currentX + 1;
                
                snakelist.list.CollisionCheck(currentX, currentY);
                MovingSnake();
                break;

               // Debug.Log("Moving direction: " + activeDirection);
        }
    }
    #endregion

    #region Movements
    private void MovingSnake()
    {
        if (!isDead)
        {
            snakelist.list.UpdateCoor(currentX, currentY);
            transform.position = new Vector2(currentX, currentY);
        }
        
    }

    #endregion

}




