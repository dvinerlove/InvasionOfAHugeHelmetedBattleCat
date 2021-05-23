using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public class RocketLouncher : SpriteClass
    {
        private float delta;

        public RocketLouncher(RectangleF rectangleF, Texture2D texture) : base(rectangleF)
        {
            Texture2D = texture;
            Speed = 100;
            Velocity = Vector2.Zero;
            SpriteType = SpriteType.RocketLouncher;

        }

        public int Speed { get; }
        public Bullet Bullet { get; private set; }

        public int Peekable = -1;

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (HP.X < -2)
            {
                IsRemoved = true;
            }

            if (HP.X < 0)
            {
                if (Peekable < 0)
                {
                    if (Random.Next(50) > 30)
                    {
                        Peekable = 1;
                    }
                    else
                    {
                        IsRemoved = true;
                    }
                }
            }
            else
            {
                Bounds.Position = new Vector2(Bounds.Position.X + Velocity.X * delta * Speed, Bounds.Position.Y /*+ (WorldParams.Gravity * delta * 100)*/);
                Shoot(gameTime, entities);
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (HP.X < 0)
            {
                spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.Gray, 0, Origin, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.White, 0, Origin, 1, SpriteEffects.None, 0);
            }
        }
        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            Bounds.Position = new Point2(Bounds.Position.X, Bounds.Position.Y - collisionInfo.PenetrationVector.Y);
        }

        private float _attackTimer = 0;
        private bool ableToShoot = false;
        private int ableToShootInt = -1;
        public TimeSpan AttackCoolDown = TimeSpan.FromSeconds(3);
        private void Shoot(GameTime gameTime, List<IEntity> entities)
        {


            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (/*gameTime.TotalGameTime.TotalSeconds - AttackCoolDown.TotalSeconds > 0 &&*/
                    Convert.ToInt32(_attackTimer) % AttackCoolDown.TotalSeconds == 0)
                {
                    ableToShootInt = Convert.ToInt32(_attackTimer);
                    ableToShoot = true;
                }
                else
                {
                    ableToShoot = false;
                }
                if (ableToShoot)
                {
                    AddBullet(entities);

                    ableToShoot = false;

                }
            }


        }

        private readonly Random Random = new Random();
        private void AddBullet(List<IEntity> entities)
        {
            Bullet = new Bullet(new RectangleF(Bounds.Position, new Size2(1, 1)))
            {
                Parent = this,
                BulletSpeed = 150,
                LifeSpan = 5f
            };
            Bullet.Bounds.Position = new Point2(Bounds.Position.X, Bounds.Position.Y - 4);
            Bullet.randomNumber = Random.Next(0, 16) - 4;
            entities.Add(Bullet);
        }
    }
}
