using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LD29.Entities;
using LD29.EntityPools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29
{
    public class HUD
    {
        private Texture2D _sheet;

        private int prevHealth;

        private List<HBSegment> hbSegments = new List<HBSegment>();

        private int hbPos = 10;

        public HUD(Texture2D hudTex)
        {
            _sheet = hudTex;
        }

        public void Update(GameTime gameTime, Viewport vp)
        {
            if ((int)Ship.Instance.Life < prevHealth)
            {
                hbSegments.Add(new HBSegment()
                {
                    Position = new Vector2((vp.Bounds.Center.X - 100) + ((200 / 100) * (int)Ship.Instance.Life), hbPos),
                    Speed = new Vector2(0f,-1f),
                    Alpha = 1f,
                    Amount = (int)(prevHealth - Ship.Instance.Life) * 2
                });
            }

            foreach (var hbs in hbSegments)
            {
                if (hbs.Amount == 0) hbs.Amount = 2;
                hbs.Speed.Y += 0.05f;
                hbs.Alpha -= 0.02f;
                hbs.Position += hbs.Speed;
            }
            hbSegments.RemoveAll(hbs => hbs.Alpha <= 0f);

            prevHealth = (int)Ship.Instance.Life;
        }

        public void Draw(SpriteBatch sb, Viewport vp, Camera gameCamera, bool radar)
        {
            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(_sheet, new Vector2(vp.Bounds.Center.X - 100, hbPos), new Rectangle(0, 0, 201, 4), Color.White);
            sb.Draw(_sheet, new Vector2(vp.Bounds.Center.X - 100, hbPos), new Rectangle(0, 5, (200/100) * (int)Ship.Instance.Life, 4), Color.White);

            Vector2 pos = new Vector2(vp.Bounds.Center.X - 99, hbPos + 6);
            for (int i = 0; i < 20; i++)
            {
                sb.Draw(_sheet, pos, new Rectangle(Ship.Instance.PowerUpMeter>i?8:0, 10, 8, 6), Color.White);
                pos.X += 10;
            }

            foreach (var hbs in hbSegments)
                sb.Draw(_sheet, hbs.Position, new Rectangle(0, 6, hbs.Amount, 2), Color.White * hbs.Alpha);

            if (radar)
            {
                foreach (Enemy e in EnemyController.Instance.Enemies)
                {
                    Vector2 epos = e.Position - (gameCamera.Position - new Vector2(vp.Width/2, vp.Height/2));
                    epos = Vector2.Clamp(epos, Vector2.One, new Vector2(vp.Bounds.Right - 2, vp.Bounds.Bottom - 2));
                    if (epos.X > 1 && epos.X < vp.Bounds.Right - 2 && epos.Y > 1 && epos.Y < vp.Bounds.Bottom - 2)
                        continue;

                    sb.Draw(_sheet, epos, new Rectangle(17, 11, 3, 3), Color.White, 0f, new Vector2(1, 1), 1f,
                        SpriteEffects.None, 0);
                }
            }

            sb.End();
        }
    }

    public class HBSegment
    {
        public Vector2 Position;
        public Vector2 Speed;
        public float Alpha;
        public int Amount;
    }
}
