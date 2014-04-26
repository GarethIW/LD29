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

        private Texture2D texBG;
        private int layerHeight;
        private int numLayers;
        private Viewport vp;
        private int mapWidth;
        private float heightScale = 1f;

        public Parallax(Texture2D tex, int layerheight, float heightscale, int ypos, int mapwidth, Viewport viewport)
        {
            texBG = tex;
            layerHeight = layerheight;
            Position = new Vector2(0,ypos);
            vp = viewport;
            mapWidth = mapwidth;
            heightScale = heightscale;

            numLayers = texBG.Height/layerheight;
        }

        public void Update(GameTime gameTime, int xSpeed)
        {

            Position.X = xPos%texBG.Width;

        }

        public void Draw(SpriteBatch sb, bool fg)
        {
            sb.Begin();
            int mid = numLayers/2;
            int start = fg ? mid : 0;
            float offsetStep = 1.6f/(float) numLayers;
            for (int layer = start; layer < start + mid; layer++)
            {
                int xoff = (int)(Position.X * ((layer + 1f)*offsetStep));
                for (int x = -(vp.Width+xoff); x < mapWidth + vp.Width+xoff; x += texBG.Width)
                {
                    if(x>=Position.X-vp.Width-xoff && x<=Position.X+vp.Width+xoff)
                        sb.Draw(texBG, 
                                new Vector2(x, (Position.Y-((numLayers/2)*layerHeight)) + (layer * layerHeight)),
                                new Rectangle(0,layerHeight*layer,texBG.Width,layerHeight),
                                Color.White,
                                0f, new Vector2(texBG.Width, layerHeight)/2, 1f, SpriteEffects.None, 0);
                }
            }
            sb.End();
        }
    }
}
