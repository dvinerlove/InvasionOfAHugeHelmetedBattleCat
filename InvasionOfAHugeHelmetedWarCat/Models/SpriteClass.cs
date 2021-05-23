using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public enum SpriteType
    {
        Player,
        Platform,
        Enemy,
        RocketLouncher,
        Cat
    }
    public class SpriteClass : IEntity
    {
        public IShapeF Bounds { get; set; }
        public Facing facing;
        public Texture2D Texture2D { get; set; }
        public Vector2 Velocity;

        public RectangleF rectParams;
        public Vector2 Origin;
        public Vector2 Position;
        public Vector2 Direction;
        public SpriteClass Parent;
        public bool IsRemoved = false;
        public float _rotation;
        public SpriteType SpriteType { get; set; }
        public Vector2 HP;

        public MouseState _previousMouseKey;
        public MouseState _currentMouseKey;

        public bool IsSolid = false;
        public string sType;

        public float LifeSpan { get; set; }
        public Vector2 MousePosition { get; set; }
        public delegate void HitHandler(Vector2 position, float direction, IEntity sprite, float damage);
        public event HitHandler HitEvent;
        public SpriteClass(RectangleF rectangleF)
        {
            // Origin = new Vector2(rectangleF.X, rectangleF.Y);
            // Bounds = new RectangleF(Origin.X - rectangleF.X / 2, Origin.Y - rectangleF.Y / 2, rectangleF.Width, rectangleF.Height);
            Bounds = new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
            rectParams = rectangleF;
        }

        //internal void Hit(Point2 position, float x, IEntity item, object bulletDamage)
        //{
        //    throw new NotImplementedException();
        //}

        internal void Hit(Vector2 origin, float direction, IEntity item, float damage)
        {
            HitEvent?.Invoke(origin, direction, item, damage);
        }

        //public SpriteClass(SpriteSheet spriteSheet, string playAnimation = null) : base(spriteSheet, playAnimation)
        //{
        //}

        public virtual void Update(GameTime gameTime, List<IEntity> entities)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            return (180 / Math.PI) * radians;
        }
        public static float ConvertDegreesToRadians(float degrees)
        {
            return (float)((Math.PI / 180) * degrees);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Black, 1);
        }
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2((float)Math.Cos(radian), (float)Math.Sin(radian));
        }
        public Vector2 GetTopSidePosition()
        {
            return new Vector2(Origin.X - rectParams.Width / 2, Origin.Y + rectParams.Height / 2);
        }
        public Vector2 GetButtomSidePosition()
        {
            return new Vector2(Origin.X - rectParams.Width / 2, Origin.Y - rectParams.Height / 2);
        }
        public Vector2 GetLeftSidePosition()
        {
            return new Vector2(Origin.X - rectParams.Width / 2, Origin.Y + rectParams.Height / 2);
        }
        public int GetObjectsOnScreen(List<IEntity> sprites, string sType)
        {
            int Counter = 0;
            if (sprites != null)
            {
                foreach (IEntity item in sprites)
                {
                    if ((item as SpriteClass).sType == sType)
                    {
                        Counter++;
                    }
                }
            }

            return Counter;
        }
        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            //    Bounds.Position -= collisionInfo.PenetrationVector;



        }

        public bool IsTouchingLeft(RectangleF sprite)
        {
            return rectParams.Right + 16 > sprite.Left &&
              rectParams.Left < sprite.Left &&
              rectParams.Bottom > sprite.Top &&
              rectParams.Top < sprite.Bottom;
        }
        public bool IsTouchingRight(RectangleF sprite)
        {
            return rectParams.Left - 16 < sprite.Right &&
             rectParams.Right > sprite.Right &&
             rectParams.Bottom > sprite.Top &&
             rectParams.Top < sprite.Bottom;
        }
        public bool IsTouchingTop(SpriteClass sprite)
        {
            return rectParams.Bottom + /*this.Velocity.Y **/ 1.5f > sprite.rectParams.Top &&
              rectParams.Top < sprite.rectParams.Top &&
              rectParams.Right > sprite.rectParams.Left &&
              rectParams.Left < sprite.rectParams.Right;
        }
        public bool IsTouchingBottom(SpriteClass sprite)
        {
            return rectParams.Top + /*this.Velocity.Y **/ -2 < sprite.rectParams.Bottom &&
               rectParams.Bottom > sprite.rectParams.Bottom &&
               rectParams.Right > sprite.rectParams.Left &&
               rectParams.Left < sprite.rectParams.Right;
        }
        public CollisionComponent collisionComponent;
        internal void SetCollision(CollisionComponent collisionComponent)
        {
            this.collisionComponent = collisionComponent;
        }

        private float _attackTimer = 0;
        private bool ableToShoot = false;
        private int ableToShootInt = -1;
        public TimeSpan dirCoolDown = TimeSpan.FromSeconds(3);
        private readonly Random Random = new Random();
        public void newDirCoolDown(GameTime gameTime, List<IEntity> entities)
        {
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (
                    Convert.ToInt32(_attackTimer) % dirCoolDown.TotalSeconds == 0)
                {
                    ableToShootInt = Convert.ToInt32(_attackTimer);
                    ableToShoot = true;
                    NewDeriction();

                }
            }


        }

        public void NewDeriction()
        {
            int tmp = Random.Next(10);
            //if (tmp == 0)
            //    Velocity.X = 0;
            if (tmp < 5)
            {
                Velocity.X = 1;
                facing = Facing.Right;
            }
            if (tmp > 5)
            {
                Velocity.X = -1;
                facing = Facing.Left;

            }

        }


    }
}