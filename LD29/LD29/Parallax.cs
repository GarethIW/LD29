using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD29
{
    public class Parallax
    {
        public Vector2 Position;
        public float HeightScale = 1f;

        private Texture2D texBG;
        private int layerHeight;
        private int numLayers;
        private Viewport vp;
        private int mapWidth;

        private float[] offsets;

        public Parallax(Texture2D tex, int layerheight, float heightscale, float ypos, int mapwidth, Viewport viewport)
        {
            texBG = tex;
            layerHeight = layerheight;
            Position = new Vector2(0,ypos);
            vp = viewport;
            mapWidth = mapwidth;
            HeightScale = heightscale;

            numLayers = texBG.Height/layerheight;
            offsets = new float[numLayers];
        }

        public void Update(GameTime gameTime, float xSpeed, int camPos)
        {
            float offsetStep = 1.6f / (float)numLayers;
            for (int i = 0; i < numLayers; i++)
            {
                offsets[i] -= (xSpeed*(((float) i + 1f)*offsetStep));
                if (offsets[i] >= texBG.Width) offsets[i] = offsets[i] - texBG.Width;
                if (offsets[i] < 0f) offsets[i] = texBG.Width + offsets[i];
            }

            Position.X = camPos;
        }

        public void Draw(SpriteBatch sb, bool fg, float camY)
        {
            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            int mid = numLayers/2;
            int start = fg ? mid : 0;

            for (int layer = start; layer < start + mid; layer++)
            {
                int xoff = (int)offsets[layer];
                for (int x = xoff-mapWidth; x <xoff+mapWidth; x += texBG.Width)
                {
                     //if(x>=Position.X-(vp.Width*2) && x<=Position.X+(vp.Width*2))
                         sb.Draw(texBG,
                             new Vector2(x, ((Position.Y - ((numLayers / 2) * (layerHeight * HeightScale))) + (layer * (layerHeight * HeightScale))) - (camY-(vp.Height/2))),
                                new Rectangle(0,layerHeight*layer,texBG.Width,layerHeight),
                                Color.White,
                                0f, new Vector2(texBG.Width, layerHeight)/2, 1f, SpriteEffects.None, 0);
                }
            }
            sb.End();
        }
    }
}
