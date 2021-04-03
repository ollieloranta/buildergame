using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    int width;
    int length;

    public int Width {
        get {
            return width;
        }
    }

    public int Length {
        get {
            return length;
        }
    }

    public World(int width=100, int length=100)
    {
        this.width = width;
        this.length = length;

        tiles = new Tile[width, length];

        for (int x = 0; x < width; x++){
            for (int y = 0; y < length; y++){
                tiles[x,y] = new Tile(this, x, y);
            }
        }
        Debug.Log("World created. Size: " + width*length + " tiles.");
    }

    public Tile GetTile(int x, int y)
    {
        if (x > width || x < 0) {
            Debug.LogError("Tile ("+x+", "+y+") out of range");
            return null;
        } 
        if (y > length || y < 0) {
            Debug.LogError("Tile ("+x+", "+y+") out of range");
            return null;
        }
        return tiles[x, y];
    }

    public int MeanFiltered(int x, int y, int filterSize=3) {
        int diff = (int) Mathf.Floor(filterSize / 2.0f);
        int tiles = 0;
        int tileSum = 0;
        for (int xx = x - diff; xx < x + diff; xx++) {
            for (int yy = y - diff; yy < y + diff; yy++) {
                if (xx >= 0 && xx < width && yy >= 0 && yy < length) {
                    tiles += 1;
                    tileSum += GetTile(xx, yy).H;
                }
            }
        }
        return (int) Mathf.Round(tileSum / tiles);
    }

    public int MedianFiltered(int x, int y, int filterSize=3) {
        int diff = (int) Mathf.Floor(filterSize / 2.0f);
        int tiles = 0;
        List<int> tileValues = new List<int>();
        for (int xx = x - diff; xx < x + diff; xx++) {
            for (int yy = y - diff; yy < y + diff; yy++) {
                if (xx >= 0 && xx < width && yy >= 0 && yy < length) {
                    tileValues.Add(GetTile(xx, yy).H);
                    tiles += 1;
                }
            }
        }
        return tileValues[(int) Mathf.Round(tiles / 2)];
    }

    public void SmoothHeights(int k=3, int n=2) {
        for (int nn = 0; nn < n; nn++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    GetTile(x, y).H = MeanFiltered(x, y, k);
                    // GetTile(x, y).H = MedianFiltered(x, y, k);
                }
            }
        }
        Debug.Log("Mean filtering complete");
    }

    public void RandomizeTilesWithHeight(int minHeight=-80, int maxHeight=150) {
        // TODO: not kovakoodattu
        int newMin;
        int newMax;
        int totalHeight = minHeight + maxHeight;
        for (int x = 0; x < width; x++){
            for (int y = 0; y < length; y++){ 
                newMin = minHeight;
                newMax = maxHeight;
                if (x == 1 || y == 1) { // Left and bottom edges sea
                    newMin -= 900;
                }
                else if (x == 99 || y == 99) { // Top and right edges mountain
                    newMax += 1000;
                }
                else if (x > 47 && x < 53 && y > 47 && y < 53) {
                    newMin = -15;
                    newMax = 45;
                }
                else if (x * y < 100 || (x < 10 || y < 10)) { // Southwest corner sea
                    newMin -= 200;
                }
                else if (x * y > 6000 || (x > 93 || y > 93)) { // Northeast corner mountain
                    newMax += 300;
                }
                int newHeight = Random.Range(newMin, newMax);
                GetTile(x, y).H = newHeight;
            }
        }
        Debug.Log("Tiles randomized");
    }

    public void RandomizeTiles() {
        for (int x = 0; x < width; x++){
            for (int y = 0; y < length; y++){
                if (Random.Range(0,4) == 0) {
                    GetTile(x, y).Type = Tile.TileType.Rock;
                }
                else {
                    GetTile(x, y).Type = Tile.TileType.Ground;
                }
            }
        }
        Debug.Log("Tiles randomized");
    }

    public void generateRocks() {
        for (int x = 0; x < width; x++){
            for (int y = 0; y < length; y++){
                if (Random.Range(0,15) == 0) {
                    GetTile(x, y).Type = Tile.TileType.Rock;
                }
                else {
                    if (x > 1 && y > 1) {
                        List<Tile> neighbors = new List<Tile>(4);
                        neighbors.Add(tiles[x-1,y]);
                        neighbors.Add(tiles[x+1,y]);
                        neighbors.Add(tiles[x,y-1]);
                        neighbors.Add(tiles[x,y+1]);
                        int neighbor_rock_count = 0;
                        for (int i = 0; i<4; i++) {
                            if (neighbors[i].Type == Tile.TileType.Rock) {
                                neighbor_rock_count += 1;
                            }
                        }
                        if (Random.Range(2*neighbor_rock_count, 20) > 15 ) {
                            tiles[x,y].Type = Tile.TileType.Rock;
                        }
                    }
                }
            }
        }
    }
}
