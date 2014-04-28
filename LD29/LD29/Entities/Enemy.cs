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

        public Enemy(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            Active = true;
        }

        public void Spawn(Vector2 spawnPos)
        {
            Position = spawnPos;

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

            if (!(this is Gorger) && !(this is Lunger) && !(this is Flyer))
            {
                if (Speed.X <= 0f) _faceDir = -1;
                else _faceDir = 1;
            }

            CheckMapCollisions(gameMap);

            if (Life <= 0f) Die();

            base.Update(gameTime, gameMap);
        }

        public void Die()
        {
            Active = false;

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
