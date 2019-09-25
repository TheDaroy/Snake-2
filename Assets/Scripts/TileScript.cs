using UnityEngine;

public class TileScript : MonoBehaviour
{
    public enum tileStatus {Free, Taken, Apple}
    public tileStatus status = tileStatus.Free;

    // Current x,y coordinates
    public int x;
    public int y;

    public AppleScript currentApple;

    // PathFinder Variables
    public int gCost;
    public int hCost;
    public TileScript parent;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    

   
}
