﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using LD29.EntityPools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class Boss : Enemy
    {
        public bool Head = false;
        public bool Body = false;
        public bool Tail = false;
        public int Ordinal = 0;

        private SpriteAnim _headGobAnim;
        private SpriteAnim _headGobAnimHit;
        private SpriteAnim _headEyeAnim;
        private SpriteAnim _headEyeAnimHit;
        private SpriteAnim _bodyAnim;
        private SpriteAnim _bodyAnimHit;
        private SpriteAnim _tailAnim;
        private SpriteAnim _tailAnimHit;

        private bool up = false;

        public Boss(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16,16));
            _hitAnim = new SpriteAnim(spritesheet, 5, 1, 32, 32, 0, new Vector2(16, 16));

            _headGobAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _headGobAnimHit = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _headEyeAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _headEyeAnimHit = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _bodyAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _bodyAnimHit = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _tailAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
            _tailAnimHit = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16, 16));
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {

            if (Head)
            {
                Speed.X = _faceDir * 2f;

                if (up) Speed.Y -= 0.1f;
                else Speed.Y += 0.1f;

                Speed.Y = MathHelper.Clamp(Speed.Y, -2.5f, 2.5f);

                if (Position.Y < 100) up = false;
                if (Position.Y > 420) up = true;

            }
            else
            {
                Vector2 target = Vector2.Zero;
                List<Enemy> segs = EnemyController.Instance.Enemies.Where(en => en is Boss).OrderBy(en => ((Boss) en).Ordinal).ToList();
                bool found = false;
                for (int index = 1; index < segs.Count; index++)
                {
                    if (((Boss)segs[index]).Ordinal == Ordinal) 
                    {
                        target = segs[index-1].Position;
                        Position = Vector2.Lerp(Position, target, 0.1f);
                    }

                    
                }


            }


            base.Update(gameTime, gameMap);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            if (Head)
            {
                _idleAnim.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= 0 && Position.X < 200) _idleAnim.Draw(sb, Position + new Vector2(gameMap.Width*gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= (gameMap.Width*gameMap.TileWidth) - 200 && Position.X < (gameMap.Width*gameMap.TileWidth)) _idleAnim.Draw(sb, Position - new Vector2(gameMap.Width*gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (_hitAlpha > 0f)
                {
                    _hitAnim.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint*_hitAlpha);
                    if (Position.X >= 0 && Position.X < 200) _hitAnim.Draw(sb, Position + new Vector2(gameMap.Width*gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint*_hitAlpha);
                    if (Position.X >= (gameMap.Width*gameMap.TileWidth) - 200 && Position.X < (gameMap.Width*gameMap.TileWidth)) _hitAnim.Draw(sb, Position - new Vector2(gameMap.Width*gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint*_hitAlpha);
                }
            }

            if (Body)
            {
                _bodyAnim.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= 0 && Position.X < 200) _bodyAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _bodyAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (_hitAlpha > 0f)
                {
                    _bodyAnimHit.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                    if (Position.X >= 0 && Position.X < 200) _bodyAnimHit.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                    if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _bodyAnimHit.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                }
            }

            if (Tail)
            {
                _tailAnim.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= 0 && Position.X < 200) _tailAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _tailAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint);
                if (_hitAlpha > 0f)
                {
                    _tailAnimHit.Draw(sb, Position, _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                    if (Position.X >= 0 && Position.X < 200) _tailAnimHit.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                    if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _tailAnimHit.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, Rotation, _tint * _hitAlpha);
                }
            }
        }

        public static void Generate(Texture2D sheet, Vector2 pos)
        {
            int face = Helper.Random.Next(2) == 0 ? -1 : 1;

            Boss head = new Boss(sheet, new Rectangle(0,0,25,25), null, Vector2.Zero);
            head.Head = true;
            head.Ordinal = 0;
            head.Life = 100;
            head.Spawn(pos);
            EnemyController.Instance.Enemies.Add(head);

            for (int i = 0; i < 7; i++)
            {
                pos.X -= face*10;
                Boss seg = new Boss(sheet, new Rectangle(0, 0, 25, 25), null, Vector2.Zero);
                seg.Body = true;
                seg.Ordinal = 1+i;
                seg.Life = 100;
                seg.Spawn(pos);
                EnemyController.Instance.Enemies.Add(seg);
            }

            pos.X -= face * 10;
            Boss tail = new Boss(sheet, new Rectangle(0, 0, 25, 25), null, Vector2.Zero);
            tail.Tail = true;
            tail.Ordinal = 8;
            tail.Life = 100;
            tail.Spawn(pos);
            EnemyController.Instance.Enemies.Add(tail);
        }
    }

}
