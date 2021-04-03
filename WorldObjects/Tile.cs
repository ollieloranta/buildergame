﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum TileType {Ground, Rock, Water, Sea, Grass, Sand, Peak};

    World world;
    int x;
    int y;
    int h;
    TileType type;
    GameObject contents;

    public Tile(World world, int x, int y, int h=0)
    {
        this.world = world;
        this.x = x;
        this.y = y;
        this.h = h;
    }

    public TileType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    public int X {
        get {
            return x;
        }
    }
    
    public int Y {
        get {
            return y;
        }
    }
    
    public int H {
        get {
            return h;
        }
        set {
            h = value;
            setTileType();
        }
    }

    public GameObject Contents {
        get {
            return contents;
        }
        set {
            contents = value;
        }
    }

    void setTileType() {
        if (h < -30) type = TileType.Sea;
        else if (h < 0) type = TileType.Water;
        else if (h < 5) type = TileType.Sand;
        else if (h < 40) type = TileType.Grass;
        else if (h < 50) type = TileType.Ground;
        else if (h < 75) type = TileType.Rock;
        else type = TileType.Peak;
    }
}