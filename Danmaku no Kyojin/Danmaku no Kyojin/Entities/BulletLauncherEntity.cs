using System;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class BulletLauncherEntity : SpriteEntity
    {
        protected readonly List<BaseBullet> Bullets;
        protected TimeSpan BulletFrequence;

        public List<BaseBullet> GetBullets()
        {
            return Bullets;
        }

        protected BulletLauncherEntity(DnK game)
            : base(game)
        {
            Bullets = new List<BaseBullet>();
        }

        protected void AddBullet(BaseBullet bullet)
        {
            Bullets.Add(bullet);
        }
    }
}
