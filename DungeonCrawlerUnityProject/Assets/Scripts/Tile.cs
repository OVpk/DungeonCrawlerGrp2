using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public enum TileType
    {
        Empty,
        Shop,
        Fight
    }

    public TileType type;

}
