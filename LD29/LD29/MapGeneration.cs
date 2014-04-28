using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace LD29
{
    internal static class MapGeneration
    {
        public const int CENTER_BLOCK = 10;
        public const int EDGE_UP = 25;
        public const int EDGE_UP_ALT = 26;
        public const int EDGE_DOWN = 18;
        public const int EDGE_LEFT = 9;
        public const int EDGE_RIGHT = 11;

        public const int EDGE_INSIDE_UP = 5;
        public const int EDGE_INSIDE_DOWN = 17;
        public const int EDGE_INSIDE_LEFT = 10;
        public const int EDGE_INSIDE_RIGHT = 12;

        public const int CORNER_INSIDE_TL = 13;
        public const int CORNER_INSIDE_TR = 15;
        public const int CORNER_INSIDE_BL = 29;
        public const int CORNER_INSIDE_BR = 31;

        public const int CORNER_OUTSIDE_TL = 79;
        public const int CORNER_OUTSIDE_TR = 80;
        public const int CORNER_OUTSIDE_BL = 17;
        public const int CORNER_OUTSIDE_BR = 19;

        public static int[] CASTLE_ONE = new int[] { 97, 98, 99, 105, 106, 107, 113, 114, 115 };
        public static int[] CASTLE_TWO = new int[] { 101, 102, 103, 109, 110, 111, 117, 118, 119 };

        public static void Generate(Map gameMap)
        {
            TileLayer layer = (TileLayer)gameMap.GetLayer("fg");

            for(int x=0;x<gameMap.Width;x++)
                for (int y = 0; y < gameMap.Height; y++)
                {
                    layer.Tiles[x, y] = null;
                    if (y == gameMap.Height - 1) layer.Tiles[x, y] = gameMap.Tiles[EDGE_UP];

                }



            int numIslands = Helper.Random.Next(2) == 0 ? Helper.Random.Next(2) == 0 ? 2 : 3 : 5;

            for (int island = 0; island < numIslands; island++)
            {
                int spacing = gameMap.Width/(numIslands + 1);
                int islandwidth = (gameMap.Width/2)/numIslands;

                Point islandCenter = new Point(spacing * (island+1), 16);

                if (island == 0) islandCenter.X -= 5;
                if (island == numIslands-1) islandCenter.X += 5;

                List<Point> cps = new List<Point>();

                cps.Add(new Point(islandCenter.X - (islandwidth/2), islandCenter.Y));

                int numcps = 2 + Helper.Random.Next(2);
                for (int i = 0; i < numcps; i++)
                    cps.Add(new Point(((islandCenter.X - (islandwidth/2)) + ((islandwidth/(numcps + 1))*(i+1)) + (-2 + Helper.Random.Next(4))),
                                  islandCenter.Y - 2 - Helper.Random.Next(10)));

                cps.Add(new Point(islandCenter.X + (islandwidth/2), islandCenter.Y));

                for (int p = 0; p < cps.Count-1; p++)
                {
                    Vector2 pos = Helper.PtoV(cps[p]);
                    for (float d = 0f; d <= 1f; d += 0.02f)
                    {
                        pos = Vector2.Lerp(Helper.PtoV(cps[p]), Helper.PtoV(cps[p + 1]), d);
                        Point thistile = Helper.VtoP(pos);
                        for(int y=thistile.Y;y<=islandCenter.Y;y++)
                            layer.Tiles[thistile.X, y] = gameMap.Tiles[CENTER_BLOCK];
                    }
                }

                cps = new List<Point>();

                cps.Add(new Point(islandCenter.X - (islandwidth / 2), islandCenter.Y));

                numcps = 2 + Helper.Random.Next(2);
                for (int i = 0; i < numcps; i++)
                    cps.Add(new Point(((islandCenter.X - (islandwidth / 2)) + ((islandwidth / (numcps + 1)) * (i + 1)) + (-2 + Helper.Random.Next(4))),
                                  islandCenter.Y + 2 + Helper.Random.Next(8)));

                cps.Add(new Point(islandCenter.X + (islandwidth / 2), islandCenter.Y));

                for (int p = 0; p < cps.Count - 1; p++)
                {
                    Vector2 pos = Helper.PtoV(cps[p]);
                    for (float d = 0f; d <= 1f; d += 0.02f)
                    {
                        pos = Vector2.Lerp(Helper.PtoV(cps[p]), Helper.PtoV(cps[p + 1]), d);
                        Point thistile = Helper.VtoP(pos);
                        for (int y = thistile.Y; y >= islandCenter.Y; y--)
                            layer.Tiles[thistile.X, y] = gameMap.Tiles[CENTER_BLOCK];
                    }
                }

            }

            // Remove stray tiles
            for (int y = 0; y < gameMap.Height; y++)
            {
                for (int x = 0; x < gameMap.Width; x++)
                {
                    if (layer.Tiles[x, y] != null)
                        if (CountSurroundingTiles(gameMap, layer, x, y) <= 3)
                        {
                            layer.Tiles[x, y] = null;
                        }
                }
            }

            for (int y = 0; y < gameMap.Height; y++)
            {
                for (int x = 0; x < gameMap.Width; x++)
                {

                    if (GetTileIndex(gameMap, layer, x, y) == CENTER_BLOCK)
                    {
                        // Edges
                        if (layer.Tiles[x - 1, y] == null)
                            if (layer.Tiles[x, y - 1] != null)
                                if (layer.Tiles[x, y + 1] != null) layer.Tiles[x, y] = gameMap.Tiles[EDGE_LEFT];

                        if (layer.Tiles[x + 1, y] == null)
                            if (layer.Tiles[x, y - 1] != null)
                                if (layer.Tiles[x, y + 1] != null) layer.Tiles[x, y] = gameMap.Tiles[EDGE_RIGHT];

                        if (layer.Tiles[x, y + 1] == null)
                            if (layer.Tiles[x - 1, y] != null)
                                if (layer.Tiles[x + 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[EDGE_DOWN];

                        if (layer.Tiles[x, y - 1] == null)
                            if (layer.Tiles[x - 1, y] != null)
                                if (layer.Tiles[x + 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[Helper.Random.Next(2)==0?EDGE_UP:EDGE_UP_ALT];

                        // Corners - inside
                        if (layer.Tiles[x - 1, y - 1] == null)
                            if (layer.Tiles[x, y - 1] != null)
                                if (layer.Tiles[x - 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_INSIDE_BR];

                        if (layer.Tiles[x - 1, y + 1] == null)
                            if (layer.Tiles[x, y + 1] != null)
                                if (layer.Tiles[x - 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_INSIDE_TR];

                        if (layer.Tiles[x + 1, y - 1] == null)
                            if (layer.Tiles[x, y - 1] != null)
                                if (layer.Tiles[x + 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_INSIDE_BL];

                        if (layer.Tiles[x + 1, y + 1] == null)
                            if (layer.Tiles[x, y + 1] != null)
                                if (layer.Tiles[x + 1, y] != null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_INSIDE_TL];

                        // Corners - outside
                        if (layer.Tiles[x - 1, y - 1] == null)
                            if (layer.Tiles[x, y - 1] == null)
                                if (layer.Tiles[x - 1, y] == null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_OUTSIDE_TL];

                        if (layer.Tiles[x - 1, y + 1] == null)
                            if (layer.Tiles[x, y + 1] == null)
                                if (layer.Tiles[x - 1, y] == null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_OUTSIDE_BL];

                        if (layer.Tiles[x + 1, y - 1] == null)
                            if (layer.Tiles[x, y - 1] == null)
                                if (layer.Tiles[x + 1, y] == null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_OUTSIDE_TR];

                        if (layer.Tiles[x + 1, y + 1] == null)
                            if (layer.Tiles[x, y + 1] == null)
                                if (layer.Tiles[x + 1, y] == null) layer.Tiles[x, y] = gameMap.Tiles[CORNER_OUTSIDE_BR];

                    }

                }
            }
            // Remove stray tiles
            for (int y = 0; y < gameMap.Height; y++)
            {
                for (int x = 0; x < gameMap.Width; x++)
                {
                    if (layer.Tiles[x, y] != null)
                        if (CountSurroundingTiles(gameMap, layer, x, y) <= 3)
                        {
                            layer.Tiles[x, y] = null;
                        }
                }
            }

            // add castles
            for (int x = 0; x < gameMap.Width; x++)
                for (int y = 0; y < gameMap.Height; y++)
                    if ((layer.Tiles[x, y] == gameMap.Tiles[EDGE_UP] ||layer.Tiles[x, y] == gameMap.Tiles[EDGE_UP_ALT]) &&
                        (layer.Tiles[x - 1, y] == gameMap.Tiles[EDGE_UP] || layer.Tiles[x-1, y] == gameMap.Tiles[EDGE_UP_ALT]) &&
                        (layer.Tiles[x + 1, y] == gameMap.Tiles[EDGE_UP] || layer.Tiles[x+1, y] == gameMap.Tiles[EDGE_UP_ALT]) &&
                        Helper.Random.Next(5) == 0 && y>5)
                    {
                        int tile = 0;
                        int type = Helper.Random.Next(2);
                        for(int yy=0;yy<3;yy++)
                            for (int xx = 0; xx < 3; xx++)
                            {
                                layer.Tiles[(x - 1) + xx, (y - 2) + yy] = type == 0 ? gameMap.Tiles[CASTLE_ONE[tile]] : gameMap.Tiles[CASTLE_TWO[tile]];
                                tile++;
                            }
                    }



            for(int x=5;x<gameMap.Width-5;x++)
                layer.Tiles[x, gameMap.Height-1] = gameMap.Tiles[EDGE_UP];
        }

        static int GetTileIndex(Map map, TileLayer layer, int x, int y)
        {
            if (x > -1 && x < map.Width && y > -1 && y < map.Height && layer.Tiles[x, y] != null)
            {
                return map.Tiles.IndexOf(layer.Tiles[x, y]);
            }

            return -1;
        }

        static int CountSurroundingTiles(Map map, TileLayer layer, int x, int y)
        {
            int count = 0;

            for (int yy = y - 1; yy <= y + 1; yy++)
                for (int xx = x - 1; xx <= x + 1; xx++)
                    if(xx>=0 && xx<map.Width&&yy>=0 && yy<map.Height)
                        if (layer.Tiles[xx, yy] != null && !(x == xx && y == yy)) count++;

            return count;
        }
    }
}
