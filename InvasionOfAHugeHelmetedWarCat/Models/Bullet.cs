using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvasionOfAHugeHelmetedWarCat.Models
{

    public class Bullet : SpriteClass
    {
        public int BulletSpeed { get; set; }
        public float BulletDamage { get; set; }
        public string BulletType { get; internal set; }

        private float _timer;
        public float randomNumber;

        public Bullet(RectangleF rectangleF) : base(rectangleF)
        {

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Parent.SpriteType == SpriteType.Player)
            {
                spriteBatch.DrawRectangle((RectangleF)Bounds, Color.White, 1);

            }
            else
            {
                spriteBatch.DrawRectangle((RectangleF)Bounds, Color.LightGray, 1);

            }
        }

        private IEntity target;
        private readonly Random Random = new Random();
        public override void Update(GameTime gameTime, List<IEntity> entities)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= LifeSpan)
            {
                IsRemoved = true;
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Parent.SpriteType == SpriteType.RocketLouncher || BulletType == "Missle")
            {
                if (target == null)
                {
                    target = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Player).FirstOrDefault();
                }
                if (Parent.SpriteType == SpriteType.Player && target != null)
                {
                    IEnumerable<IEntity> a = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Enemy).Where(x => (x as Enemy).Dead == false);
                    a.Count();

                    target = a.FirstOrDefault();
                    foreach (SpriteClass item in entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Enemy).Where(x => (x as Enemy).Dead == false))
                    {
                        if (Vector2.Distance(Bounds.Position, item.Bounds.Position) < 64)
                        {
                            target = item;
                        }
                    }

                }
                else
                {
                    target = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Player).FirstOrDefault();

                }
                Vector2 moveDir;
                if (target == null)
                {
                    target = new SpriteClass(new RectangleF(Parent.rectParams.X + randomNumber * 10 + (Random.Next(10) - 5) * 32, Parent.rectParams.Y + 32, Parent.rectParams.Width, Parent.rectParams.Height));
                }
                else
                {

                }
                moveDir = new Point2(target.Bounds.Position.X + randomNumber, target.Bounds.Position.Y - 16) - Bounds.Position;

                moveDir.Normalize();

                if (_timer < LifeSpan / 2.5f)
                {
                    Bounds.Position = new Point2(Bounds.Position.X, Bounds.Position.Y - BulletSpeed / 1.8f * delta);

                    Bounds.Position = new Point2(Bounds.Position.X + moveDir.X / 1.8f * delta * BulletSpeed, Bounds.Position.Y + moveDir.Y / 1.8f * delta * BulletSpeed);

                }
                else
                {
                    moveDir = new Point2(target.Bounds.Position.X + randomNumber, target.Bounds.Position.Y + 16) - Bounds.Position;
                    moveDir.Normalize();
                    Bounds.Position = new Point2(Bounds.Position.X + moveDir.X / 2 * delta * BulletSpeed, Bounds.Position.Y + moveDir.Y / 1.5f * delta * BulletSpeed);

                }

            }
            else
            {

                Bounds.Position = new Point2(Bounds.Position.X + BulletSpeed * delta * Velocity.X, Bounds.Position.Y);

            }

            foreach (IEntity item in entities)
            {
                if (Parent.SpriteType == SpriteType.Enemy)
                {
                    if ((item as SpriteClass).SpriteType != SpriteType.RocketLouncher
                        && Bounds.Intersects(item.Bounds)
                    && (item as SpriteClass) != this
                    && (item as SpriteClass).SpriteType != Parent.SpriteType)
                    {
                        IsRemoved = true;
                        (Parent).Hit(Bounds.Position, Velocity.X, item, BulletDamage);
                    }
                }
                else
                if (Bounds.Intersects(item.Bounds)
                    && (item as SpriteClass) != this
                    && (item as SpriteClass).SpriteType != Parent.SpriteType)
                {
                    IsRemoved = true;
                    (Parent).Hit(Bounds.Position, Velocity.X, item, BulletDamage);
                }
            }

        }
    }
}
