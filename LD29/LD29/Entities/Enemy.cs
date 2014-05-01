using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LD29.Entities.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace LD29.Entities
{
    public class Enemy : Entity
    {
        protected SpriteAnim _idleAnim;
        protected SpriteAnim _hitAnim;
        protected Vector2 _scale;

        protected Color _tint = Color.White;
        protected float _hitAlpha = 0f;

        protected int _faceDir = -1;

        public float Life;

        protected bool _underWater;

        public Enemy(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            Active = true;
        }

        public void Spawn(Vector2 spawnPos)
        {
            Position = spawnPos;
            if (spawnPos.Y > 260) _underWater = true;

            TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tween =>
            {
                _scale.X = 8f - (7f * tween.Value);
                _scale.Y = tween.Value;
                if (tween.State == TweenState.Finished) _scale = Vector2.One;
            }, 1000, false, false);
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            if (_hitAlpha > 0f) _hitAlpha -= 0.1f;

            _idleAnim.Update(gameTime);
            _hitAnim.Update(gameTime);

            if (!(this is Gorger) && !(this is Lunger) && !(this is Flyer) && !(this is Boss))
            {
                if (Speed.X <= 0f) _faceDir = -1;
                else _faceDir = 1;
            }

            if(!(this is Boss)) CheckMapCollisions(gameMap);

            if (Life <= 0f) Die();

            if (Position.Y > (gameMap.Height*gameMap.TileHeight)) Die();

            if (_underWater)
            {
                if (Position.Y < 260)
                {
                    _underWater = false;
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 pos = new Vector2(Helper.RandomFloat(-5f, 5f), 0f);
                        Color col = Color.Lerp(new Color(0, 81, 147), new Color(211, 234, 254),
                            Helper.RandomFloat(0f, 1f));
                        ParticleController.Instance.Add(Position + pos,
                            (pos*0.1f) + new Vector2(Speed.X, Speed.Y*Helper.RandomFloat(0.25f, 2f)),
                            0, 2000, 500, true, true, new Rectangle(0, 0, 3, 3),
                            col, particle =>
                            {
                                ParticleFunctions.FadeInOut(particle);
                                if (particle.Position.Y > 260)
                                    particle.State = ParticleState.Done;
                            }, 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 1, ParticleBlend.Alpha);
                    }
                }
            }
            else if(Position.Y>260) _underWater = true;

            base.Update(gameTime, gameMap);
        }

        public virtual void Die()
        {
            Active = false;

            AudioController.PlaySFX("explosion", 1f, -0.1f, 0.1f, Camera.Instance, Position);

            for (float a = 0f; a <= MathHelper.TwoPi; a += 0.1f)
            {
                Vector2 loc = Helper.PointOnCircle(ref Position, 7, a);
                Vector2 speed = loc - Position;
                speed.Normalize();
                
                ParticleController.Instance.Add(loc,
                    speed*Helper.RandomFloat(1f,3f),
                    100, 3000, 1000,
                    true, true,
                    new Rectangle(0, 0, 2, 2),
                    new Color(new Vector3(1f, 0f, 0f)*(0.25f + Helper.RandomFloat(0.5f))),
                    part =>
                    {
                        ParticleFunctions.FadeInOut(part);
                    },
                    1f, 0f, 0f,
                    1, ParticleBlend.Alpha);

                loc = Helper.PointOnCircle(ref Position, 3, a);
                speed = loc - Position;
                speed.Normalize();

                ParticleController.Instance.Add(loc,
                    speed * Helper.RandomFloat(1f, 3f),
                    100, 3000, 1000,
                    true, true,
                    new Rectangle(0, 0, 2, 2),
                    new Color(new Vector3(1f, 0f, 0f) * (0.25f + Helper.RandomFloat(0.5f))),
                    part =>
                    {
                        ParticleFunctions.FadeInOut(part);
                    },
                    1f, 0f, 0f,
                    1, ParticleBlend.Alpha);
            }

            if (Ship.Instance.PowerUpLevel < 4)
            {
                int amount = 3 - Ship.Instance.PowerUpLevel;
                if (amount <= 0) amount = 1;
                for (int i = 0; i < amount; i++)
                {
                    PowerupController.Instance.Spawn(pup =>
                    {
                        pup.Position = Position;
                        pup.Speed = new Vector2(Helper.RandomFloat(-0.5f, 0.5f), Helper.RandomFloat(-0.5f, 0.5f));
                    });
                }
            }

            Ship.Instance.AddScore();
        }

        public override void OnBoxCollision(Entity collided, Rectangle intersect)
        {
            if (collided is Projectile)
            {
                if (!((Projectile) collided).EnemyOwner)
                {
                    Life -= ((Projectile) collided).Damage;

                    _hitAlpha = 1f;
                }
            }

            base.OnBoxCollision(collided, intersect);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            _idleAnim.Draw(sb,Position, _faceDir==-1?SpriteEffects.None:SpriteEffects.FlipHorizontally,_scale,Rotation,_tint);
            if (Position.X >= 0 && Position.X < 200) _idleAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _idleAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
            if (_hitAlpha > 0f)
            {
                _hitAnim.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                if (Position.X >= 0 && Position.X < 200) _hitAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _hitAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha); 
            }
            base.Draw(sb, gameMap);
        }

        public virtual void CheckMapCollisions(Map gameMap)
        {
            // Check upward collision
            if (Speed.Y < 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Top + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = -Speed.Y;
                }

            // Check downward collision
            if (Speed.Y > 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = -Speed.Y;
                }

            // Check left collision
            if (Speed.X < 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X;
                    }
                }
        }
    }
}
