using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class Eyes : Enemy
    {
        public Eyes(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 2, 4, 16, 16, 100, new Vector2(8, 8), true, true, 0);
            _idleAnim.Play();
            _hitAnim = new SpriteAnim(spritesheet, 8, 4, 16, 16, 100, new Vector2(8, 8), true, true, _idleAnim.CurrentFrame);
            _hitAnim.Play();

            Speed.X = Helper.RandomFloat(-0.3f, 0.3f);
            Speed.Y = Helper.RandomFloat(-0.2f, 0.2f);
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            if (Speed.Y < 0f && Position.Y < 250) Speed.Y = -Speed.Y;

            if(Helper.Random.Next(20)==0)
                ParticleController.Instance.Add(Position,
                                   new Vector2(Helper.RandomFloat(-0.1f, 0.1f), Helper.RandomFloat(-0.1f, 0.1f)),
                                   0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                   false, false,
                                   new Rectangle(128, 0, 16, 16),
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.FadeInOut,
                                   0.5f, 0f, 0f,
                                   1, ParticleBlend.Alpha);

            if (Helper.Random.Next(200) == 0)
            {
                if (Ship.Instance.Position.Y > 260)
                    AudioController.PlaySFX("eyespew", 1f, -0.1f, 0.1f, Camera.Instance, Position);

                float start = Helper.RandomFloat(0f, 0.25f);
                for (float a = start; a < start + MathHelper.TwoPi; a += 0.25f)
                    ProjectileController.Instance.Spawn(entity =>
                    {
                        ((Projectile) entity).Type = ProjectileType.EyesPew;
                        ((Projectile) entity).SourceRect = new Rectangle(44, 3, 4, 10);
                        entity.HitBox = new Rectangle(0, 0, 6, 6);
                        ((Projectile) entity).Life = 5000;
                        ((Projectile) entity).EnemyOwner = true;
                        ((Projectile) entity).Damage = 3f;
                        entity.Speed = Helper.AngleToVector(a, 1f);
                        entity.Position = Position + Helper.AngleToVector(a, 5f);
                    });
            }



            base.Update(gameTime, gameMap);
        }
    }
}
