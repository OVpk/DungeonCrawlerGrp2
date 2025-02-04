using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile rightNeighbor;
    public Tile leftNeighbor;
    public Tile upNeighbor;
    public Tile downNeighbor;

    public enum TileType
    {
        Shop,
        Fight
    }

    public TileType type;
    
    public void Initialize(TileType tileType, Tile right = null, Tile left = null, Tile up = null, Tile down = null)
    {
        type = tileType;
        rightNeighbor = right;
        leftNeighbor = left;
        upNeighbor = up;
        downNeighbor = down;
    }

    public void SetNeighbor(Tile newNeighbor, MovementBetweenTile.Directions direction)
    {
        switch (direction)
        {
            case MovementBetweenTile.Directions.Up :
                upNeighbor = newNeighbor; break;
            case MovementBetweenTile.Directions.Down :
                downNeighbor = newNeighbor; break;
            case MovementBetweenTile.Directions.Left :
                leftNeighbor = newNeighbor; break;
            case MovementBetweenTile.Directions.Right :
                rightNeighbor = newNeighbor; break;
        }
    }
}
