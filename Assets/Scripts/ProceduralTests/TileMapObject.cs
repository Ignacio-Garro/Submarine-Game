using UnityEngine;




public class TileMapObject : MonoBehaviour
{
    public bool UpWall;
    public bool DownWall;
    public bool LeftWall;
    public bool RightWall;
    public bool FrontWall;
    public bool BackWall;
    

    
    //Rigth->0
    //Front->1
    //Left->2
    //Back->3
    public bool GetHorizontalConnectionRotated(int face, int rotation)
    {
        face = face - rotation / 90;
        if (face < 0) face += 4;
        switch (face)
        {
            case 0: return RightWall;
            case 1: return FrontWall;
            case 2: return LeftWall;
            case 3: return BackWall;
        }
        return false;
    }
}
