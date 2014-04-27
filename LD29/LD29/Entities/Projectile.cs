using GameStateManagement;
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
            if (coll.HasValue && coll.Value) Active=false;

            switch (Type)
            {
                case ProjectileType.Bomb:
                    Speed.Y += 0.1f;
                    ParticleController.Instance.Add(Position,
                                   new Vector2(-0.05f + Helper.RandomFloat(0.1f), -0.1f),
                                   1000, Helper.Random.NextDouble() * 3000, Helper.Random.NextDouble() * 3000,
                                   false, false,
                                   new Rectangle(0, 0, 16, 16),
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.Smoke,
                                   0.1f, 0f, 0f,
                                   1, ParticleBlend.Additive);
                    break;
            }

            base.Update(gameTime, gameMap);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            sb.Draw(SpriteSheet, Position, SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width,SourceRect.Height)/2,1f,SpriteEffects.None, 0);
            if (Position.X >= 0 && Position.X < 200) sb.Draw(SpriteSheet, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width, SourceRect.Height) / 2, 1f, SpriteEffects.None, 0);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) sb.Draw(SpriteSheet, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), SourceRect, Color.White, Rotation, new Vector2(SourceRect.Width, SourceRect.Height) / 2, 1f, SpriteEffects.None, 0);
            base.Draw(sb, gameMap);
        }

       

    }
}
