using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public (int x, int y) coordinates;

    public enum TileType
    {
        Shop,
        Fight
    }

    public TileType type;
    
    public void Initialize(TileType tileType, (int x, int y) tileCoordinates)
    {
        type = tileType;
        coordinates = tileCoordinates;
    }
    
    
}
