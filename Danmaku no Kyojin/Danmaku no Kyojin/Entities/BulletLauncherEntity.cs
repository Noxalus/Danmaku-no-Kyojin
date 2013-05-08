using System.Collections.Generic;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class BulletLauncherEntity : Entity
    {
        private List<BaseBullet> _bullets;

        protected BulletLauncherEntity(DnK game, ref List<BaseBullet> bullets)
            : base(game)
        {
            _bullets = bullets;
        }

        protected void AddBullet(BaseBullet bullet)
        {
            _bullets.Add(bullet);
        }
    }
}
