using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace LD29
{
    static class MapGeneration
    {
        private const int CENTER_BLOCK = 8;

        public static void Generate(Map gameMap)
        {
            TileLayer layer = (TileLayer)gameMap.GetLayer("fg");

            for(int x=0;x<gameMap.Width;x++)
                for (int y = 0; y < gameMap.Height; y++) layer.Tiles[x, y] = null;

            int numIslands = Helper.Random.Next(2) == 0 ? Helper.Random.Next(2) == 0 ? 2 : 3 : 5;

            for (int island = 0; island < numIslands; island++)
            {
                int spacing = gameMap.Width/(numIslands + 1);
                int islandwidth = (gameMap.Width/2)/numIslands;

                Point islandCenter = new Point(spacing * (island+1), 16);

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
                                  islandCenter.Y + 2 + Helper.Random.Next(10)));

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
        }
    }
}
