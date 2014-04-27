using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LD29.Entities;
using LD29.EntityPools;
using Microsoft.Xna.Framework.Graphics;

namespace LD29
{
    public class ProjectileController : EntityPool
    {
        public static ProjectileController Instance;

        public ProjectileController(int maxEntities, Func<Texture2D, Entity> createFunc, Texture2D spriteSheet)
            : base(maxEntities, createFunc, spriteSheet)
        {
            Instance = this;
        }
    }
}
