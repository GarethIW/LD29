﻿using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace LD29.Entities
{
    public enum ProjectileType
    {
        Forward1,
        Bomb
    }

    class Projectile : Entity
    {
        public ProjectileType Type;
        public Rectangle SourceRect;
        public float Scale = 1f;
        public double Life = 0;
        public bool EnemyOwner = false;
        public float Damage = 1f;

        public Projectile(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            Rotation = Helper.V2ToAngle(Speed);

            Life -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Life <= 0) Active = false;

            bool? coll = gameMap.CheckCollision(Position);
            if (coll.HasValue && coll.Value)
            {
                Active=false;
                if (Type == ProjectileType.Bomb) Explode();
                else RaiseDust();
            }

            switch (Type)
            {
                case ProjectileType.Forward1:
                    if(Position.Y>260 && Helper.Random.Next(5)==0)
                        ParticleController.Instance.Add(Position,
                                   -Speed * Helper.RandomFloat(0f,0.3f),
                                   0, 200 + Helper.Random.NextDouble() * 500, 100,
                                   false, false,
                                   new Rectangle(0, 0, 2, 2),
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.Smoke,
                                   1f, 0f, 0f,
                                   1, ParticleBlend.Additive);
                    break;
                case ProjectileType.Bomb:
                    Speed.Y += (Position.Y > 260 ? 0.01f : 0.1f);
                    Rectangle particleRect = Position.Y > 260 ? new Rectangle(128, 0, 16, 16) : new Rectangle(0, 0, 16, 16);
                    ParticleController.Instance.Add(Position,
                                   new Vector2(-0.05f + Helper.RandomFloat(0.1f), -0.1f),
                                   0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                   false, false,
                                   particleRect,
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   //(Position.Y > 260) ? ParticleFunctions.FadeInOut : ParticleFunctions.Smoke,
                                   ParticleFunctions.Smoke,
                                   0.25f, 0f, 0f,
                                   1, Position.Y > 260 ? ParticleBlend.Alpha :ParticleBlend.Additive);
                    break;
            }

            base.Update(gameTime, gameMap);
        }

        private void Explode()
        {
            for (int i = 0; i < 10; i++)
            {
                Color c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                ParticleController.Instance.Add(Position + new Vector2(0f,-3f), 
                                                new Vector2(-1f + ((float)Helper.Random.NextDouble() * 2f), Helper.RandomFloat(-0.5f,-2f)),
                                                0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                               true, true,
                                               new Rectangle(0, 0, 3, 3),
                                               c,
                                               ParticleFunctions.FadeInOut,
                                               1f, 0f, 0f,
                                               1, ParticleBlend.Additive);
            }
            for (int i = 0; i < 10; i++)
            {
                Color c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                ParticleController.Instance.Add(Position + new Vector2(0f, -3f),
                                                new Vector2(-0.5f + ((float)Helper.Random.NextDouble() * 1f), Helper.RandomFloat(-0.5f, -3f)),
                                                0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                               true, true,
                                               new Rectangle(0, 0, 3, 3),
                                               c,
                                               ParticleFunctions.FadeInOut,
                                               1f, 0f, 0f,
                                               1, ParticleBlend.Additive);
            }
        }

        private void RaiseDust()
        {
            for(int i=0;i<5;i++)
                ParticleController.Instance.Add(Position,
                                   new Vector2(-0.05f + Helper.RandomFloat(0.1f), -0.1f),
                                   0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                   false, false,
                                   new Rectangle(0, 0, 2, 2),
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.Smoke,
                                   1f, 0f, 0f,
                                   1, ParticleBlend.Alpha);
        }

        public override void OnBoxCollision(Entity collided, Rectangle intersect)
        {
            if (collided is Ship && !EnemyOwner) return;

            collided.OnBoxCollision(this, intersect);

            if (Type == ProjectileType.Forward1)
            {
                Active = false;
            }

            if (Type == ProjectileType.Bomb)
            {
                Active = false;
                Explode();
            }
            base.OnBoxCollision(collided, intersect);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            sb.Draw(SpriteSheet, Position, SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width,SourceRect.Height)/2,Scale,SpriteEffects.None, 0);
            if (Position.X >= 0 && Position.X < 200) sb.Draw(SpriteSheet, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width, SourceRect.Height) / 2, Scale, SpriteEffects.None, 0);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) sb.Draw(SpriteSheet, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width, SourceRect.Height) / 2, Scale, SpriteEffects.None, 0);
            base.Draw(sb, gameMap);
        }

       

    }
}
