using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class ManOWar : Enemy
    {
        public ManOWar(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 1, 4, 16, 16, 500, new Vector2(8, 8), true, false, Helper.Random.Next(4));
            _idleAnim.Play();
            _hitAnim = new SpriteAnim(spritesheet, 7, 4, 16, 16, 500, new Vector2(8, 8), true, false, _idleAnim.CurrentFrame);
            _hitAnim.Play();

            Speed.X = Helper.RandomFloat(-0.1f, 0.1f);
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            if(Helper.Random.Next(10)==0)
                ParticleController.Instance.Add(Position + new Vector2(0,5),
                                   new Vector2(Helper.RandomFloat(-0.1f,0.1f), 0f),
                                   100, 3000, 1000,
                                   true, true,
                                   new Rectangle(0, 0, 2, 2),
                                   new Color(new Vector3(1f, 0f, 0f) * (0.25f + Helper.RandomFloat(0.5f))),
                                    part => { ParticleFunctions.FadeInOut(part);
                                                if (part.Position.Y > 260) part.State = ParticleState.Done;
                                    }  ,
                                   1f, 0f, 0f,
                                   1, ParticleBlend.Alpha);

            base.Update(gameTime, gameMap);
        }
    }
}
