using UnityEngine;

public class AppleScript : MonoBehaviour
{
    
    public TileScript currentTile;
    private MapScript map;
    private void Start()
    {
        map = GameObject.Find("GameManager").GetComponent<MapScript>();
        UpdatePosition();
    }
    

    public void UpdatePosition() // Moves Apple and set tile status as "apple"
    {
        if (currentTile != null)
        {
            currentTile.status = TileScript.tileStatus.Taken;
        }
        int x;
        int y;
        (x, y) = map.NewApplePosition();
        transform.position = new Vector2(x, y);
        currentTile = map.tileArray[x, y];
        currentTile.currentApple = this;
        currentTile.status = TileScript.tileStatus.Apple;

        
    }
}
