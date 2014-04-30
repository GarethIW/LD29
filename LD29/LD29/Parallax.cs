using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimersAndTweens;

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
        private float[] waveoffsets;

        private bool wavyEffect = false;
        private bool cutoff = false;
        private bool reverse = false;

        public Parallax(Texture2D tex, int layerheight, float heightscale, float ypos, int mapwidth, Viewport viewport, bool wavy, bool cut)
        {
            texBG = tex;
            layerHeight = layerheight;
            Position = new Vector2(0,ypos);
            vp = viewport;
            mapWidth = mapwidth;
            HeightScale = heightscale;

            numLayers = texBG.Height/layerheight;
            offsets = new float[numLayers];
            waveoffsets =new float[numLayers];

            wavyEffect = wavy;
            cutoff = cut;

            if (wavyEffect) TweenController.Instance.Create("", TweenFuncs.Linear, WaveCallback, 5000, true, true);
        }

        public Parallax(Texture2D tex, int layerheight, float heightscale, float ypos, int mapwidth, Viewport viewport,
            bool wavy, bool cut, bool rev)
            :this(tex, layerheight, heightscale, ypos, mapwidth, viewport, wavy, cut)
        {
            reverse = rev;
        }

        private void WaveCallback(Tween tween)
        {
            for (int i = 0; i < numLayers; i++)
            {
                waveoffsets[i]=(float)Math.Sin((-MathHelper.PiOver2 + (MathHelper.Pi* tween.Value)) + (float)(i*0.2f)) * 15f;
            }
        }

        public void Update(GameTime gameTime, float xSpeed, int camPos)
        {
            if (!wavyEffect)
            {
                float offsetStep = 2f/(float) numLayers;
                if (!reverse)
                {
                    for (int i = 0; i < numLayers; i++)
                    {
                        offsets[i] -= (xSpeed*(((float) i + 1f)*offsetStep));
                        if (offsets[i] >= texBG.Width) offsets[i] = offsets[i] - texBG.Width;
                        if (offsets[i] < 0f) offsets[i] = texBG.Width + offsets[i];
                    }
                }
                else
                {
                    for (int i = 0; i < numLayers; i++)
                    {
                        offsets[i] -= (xSpeed * (((float)(numLayers-i) + 1f) * offsetStep));
                        if (offsets[i] >= texBG.Width) offsets[i] = offsets[i] - texBG.Width;
                        if (offsets[i] < 0f) offsets[i] = texBG.Width + offsets[i];
                    }
                }
            }
            else
            {
          
                    for (int i = 0; i < numLayers; i++)
                    {
                        offsets[i] -= xSpeed;
                        if (offsets[i] >= texBG.Width) offsets[i] = offsets[i] - texBG.Width;
                        if (offsets[i] < 0f) offsets[i] = texBG.Width + offsets[i];
                    }
               
                
            }



            Position.X = camPos;

            
        }

        public void Draw(SpriteBatch sb, bool fg, float camY)
        {
            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            int mid = numLayers>1?numLayers/2:1;
            int start = fg ? mid : 0;

            if (!reverse)
            {
                for (int layer = start; layer < start + mid; layer++)
                {
                    int xoff = (int) offsets[layer]; // + (int)waveoffsets[layer];
                    for (int x = xoff - mapWidth; x < xoff + mapWidth; x += texBG.Width)
                    {
                        //if(x>=Position.X-(vp.Width*2) && x<=Position.X+(vp.Width*2))
                        sb.Draw(texBG,
                            new Vector2(x,
                                ((Position.Y - ((numLayers/2)*(layerHeight*HeightScale))) +
                                 (layer*(layerHeight*HeightScale))) - (camY - (vp.Height/2))),
                            new Rectangle(0, layerHeight*layer, texBG.Width,
                                (cutoff && HeightScale < 0.5f)
                                    ? (int) (((float) layerHeight/5f)*(HeightScale*10f))
                                    : layerHeight),
                            Color.White,
                            0f, new Vector2(texBG.Width, layerHeight)/2, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            else
            {
                for (int layer = start+mid-1; layer >=start ; layer--)
                {
                    int xoff = (int)offsets[layer]; // + (int)waveoffsets[layer];
                    for (int x = xoff - mapWidth; x < xoff + mapWidth; x += texBG.Width)
                    {
                        //if(x>=Position.X-(vp.Width*2) && x<=Position.X+(vp.Width*2))
                        sb.Draw(texBG,
                            new Vector2(x,
                                ((Position.Y - ((numLayers / 2) * (layerHeight * HeightScale))) +
                                 (layer * (layerHeight * HeightScale))) - (camY - (vp.Height / 2))),
                            new Rectangle(0, layerHeight * layer, texBG.Width,
                                (cutoff && HeightScale < 0.5f)
                                    ? (int)(((float)layerHeight / 5f) * (HeightScale * 10f))
                                    : layerHeight),
                            Color.White,
                            0f, new Vector2(texBG.Width, layerHeight) / 2, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            sb.End();
        }

        public void Draw(SpriteBatch sb, float camY)
        {
            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            int start = 0;

            if (!reverse)
            {
                for (int layer = start; layer < numLayers; layer++)
                {
                    int xoff = (int) offsets[layer] + (int) waveoffsets[layer];
                    for (int x = xoff - mapWidth; x < xoff + mapWidth; x += texBG.Width)
                    {
                        //if(x>=Position.X-(vp.Width*2) && x<=Position.X+(vp.Width*2))
                        sb.Draw(texBG,
                            new Vector2(x,
                                ((Position.Y - ((numLayers/2)*(layerHeight*HeightScale))) +
                                 (layer*(layerHeight*HeightScale))) - (camY - (vp.Height/2))),
                            new Rectangle(0, layerHeight*layer, texBG.Width,
                                HeightScale < 0.5f ? (int) (((float) layerHeight/5f)*(HeightScale*10f)) : layerHeight),
                            Color.White,
                            0f, new Vector2(texBG.Width, layerHeight)/2, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            else
            {
                for (int layer = numLayers-1; layer >=start; layer--)
                {
                    int xoff = (int)offsets[layer] + (int)waveoffsets[layer];
                    for (int x = xoff - mapWidth; x < xoff + mapWidth; x += texBG.Width)
                    {
                        //if(x>=Position.X-(vp.Width*2) && x<=Position.X+(vp.Width*2))
                        sb.Draw(texBG,
                            new Vector2(x,
                                ((Position.Y - ((numLayers / 2) * (layerHeight * HeightScale))) +
                                 (layer * (layerHeight * HeightScale))) - (camY - (vp.Height / 2))),
                            new Rectangle(0, layerHeight * layer, texBG.Width,
                                HeightScale < 0.5f ? (int)(((float)layerHeight / 5f) * (HeightScale * 10f)) : layerHeight),
                            Color.White,
                            0f, new Vector2(texBG.Width, layerHeight) / 2, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            sb.End();
        }
    }
}
