using System;
using System.Collections.Generic;
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

        private SpriteAnim _headIdleAnim;
        private SpriteAnim _headIdleAnimHit;
        private SpriteAnim _headGobAnim;
        private SpriteAnim _headGobAnimHit;
        private SpriteAnim _headEyeAnim;
        private SpriteAnim _headEyeAnimHit;
        private SpriteAnim _bodyAnim;
        private SpriteAnim _bodyAnimHit;
        private SpriteAnim _tailAnim;
        private SpriteAnim _tailAnimHit;


        public Boss(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _headIdleAnim = new SpriteAnim(spritesheet, 0, 1, 32, 32, 0, new Vector2(16,16));
            _headIdleAnimHit = new SpriteAnim(spritesheet, 5, 1, 32, 32, 0, new Vector2(16, 16));

        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            _headIdleAnim.Draw(sb,Position);
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
                seg.Head = true;
                seg.Ordinal = 1+i;
                seg.Life = 100;
                seg.Spawn(pos);
                EnemyController.Instance.Enemies.Add(seg);
            }

            pos.X -= face * 10;
            Boss tail = new Boss(sheet, new Rectangle(0, 0, 25, 25), null, Vector2.Zero);
            tail.Head = true;
            tail.Ordinal = 8;
            tail.Life = 100;
            tail.Spawn(pos);
            EnemyController.Instance.Enemies.Add(tail);
        }
    }

}
