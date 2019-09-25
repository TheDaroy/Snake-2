using UnityEngine;

public class SnakeListScript : MonoBehaviour
{
    #region References & Class Variables
    public linkedList list = new linkedList();
    [System.NonSerialized] public SnakeScript snake;
    public GameObject headPart;
    public GameObject bodypart;
    #endregion
    private void Awake()  // Assigns references 
    {
        snake = GetComponent<SnakeScript>();
        list.headPart = this.headPart;
        list.bodyPart = this.bodypart;
        list.snake = this.snake;
    }
    
    public class linkedList
    {
        #region References & Class Variables

        // Prefabs
        public GameObject headPart; 
        public GameObject bodyPart; 

        // References
        public SnakeScript snake; 
        public Block head; 

        //
        private int lastX; 
        private int lastY;    
        #endregion

        public void CreateSnake(int x, int y, bool isHead)
        {
            Block currentBlock = head;
            GameObject part;
            if (isHead) { part = headPart; }
            else { part = bodyPart; }

            if (head == null)
            {
                GameObject temp = Instantiate(part, new Vector2(x, y), Quaternion.identity);
                head = new Block(x, y,  temp);
                snake.map.tileArray[x, y].status = TileScript.tileStatus.Taken;       
               // Debug.Log("X: " + head._X + ". Y: " + head._Y + ". GameObject: " + head.ToString());
            }

            else if (head != null)
            {
                while (currentBlock.next != null)
                {currentBlock = currentBlock.next;}
                GameObject temp = Instantiate(part, new Vector2(x, y), Quaternion.identity);
                currentBlock.next = new Block(x, y, temp);
                snake.map.tileArray[x, y].status = TileScript.tileStatus.Taken;
                // Debug.Log("X: " + currentBlock._X + ". Y: " + currentBlock._Y + ". GameObject: " + currentBlock.ToString());          
            }

        } // Called at instantiation to create head block & some body blocks
        public void AddBlock() // Adds Block to the back
        {
            Block currentBlock = head;
            while (currentBlock.next != null)
            { currentBlock = currentBlock.next;}

            if (currentBlock.next == null)
            {
                GameObject temp = Instantiate(bodyPart, new Vector2(lastX, lastY), Quaternion.identity);
                currentBlock.next = new Block(lastX, lastY,temp);
               
            }
        }
        public void UpdateCoor(int x, int y)
        {
            #region Note
            // Future idea, Move bottom block to head instead of moving every single block individually, 
            // might make things a bit more efficient.
            #endregion
            Block currentBlock = head;  
            int tempX = x;
            int tempY = y;
            
            while (currentBlock != null)
            {
                snake.map.tileArray[lastX, lastY].status = TileScript.tileStatus.Free;
                lastX = currentBlock.blockX;
                lastY = currentBlock.blockY;
                currentBlock.blockX = tempX;
                currentBlock.blockY = tempY;
                
                snake.map.tileArray[currentBlock.blockX, currentBlock.blockY].status = TileScript.tileStatus.Taken;
                tempX = lastX;
                tempY = lastY;
                MoveObject(currentBlock,currentBlock.blockX, currentBlock.blockY);
                currentBlock = currentBlock.next;        
            }
        } // Updates coordinates of all the blocks (Note Inside)
        public void CollisionCheck(int x, int y)
        {
            if (x > snake.map.maxX -1 || x < 0 || y > snake.map.maxY -1 || y < 0)
            {
                Debug.Log("You lose");
                snake.isDead = true;
            }
           else if ( snake.map.tileArray[x, y].status == TileScript.tileStatus.Taken)                                 //
            {
                Debug.Log("You lose");
                snake.isDead = true;
            }
           else if (snake.map.tileArray[x,y].status == TileScript.tileStatus.Apple)
            {
                snake.map.tileArray[x, y].currentApple.UpdatePosition();
                AddBlock();
            }
        } // Checks if collision has happened with either a snake, apple or map limit
        private void MoveObject(Block currentBlock, int x, int y)
        {
            //Debug.Log(x + "+" + y);
            currentBlock.snakePart.transform.position = new Vector2(x, y);
        } // Moves current block    
        public void DebugPrint()
        {
            //Block currentBlock = head;
            //Debug.Log("   Head is :" + head);
            //Debug.Log(currentBlock.next);
            //Debug.Log(currentBlock.snakePart.ToString());
            //Debug.Log(currentBlock.next.snakePart.ToString());
            //while (currentBlock != null)
            //{
            //    Debug.Log("X: " + currentBlock._X + ". Y: " + currentBlock._Y + ". GameObject: " + currentBlock.ToString());
            //    currentBlock = currentBlock.next;
            //}
        }  // Misc Debug Stuff
      
        public class Block
        {
            public Block next;
            public GameObject snakePart;
            public int blockX;
            public int blockY;

            public Block(int x, int y ,GameObject part)
            {
                blockX = x;
                blockY = y;        
                snakePart = part;               
            }
           
        }
        
    }
}

