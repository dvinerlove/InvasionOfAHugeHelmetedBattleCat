using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public class Cat : SpriteClass
    {
        private float delta;
        private readonly Random Random = new Random();
        public Cat(RectangleF rectangleF) : base(rectangleF)
        {
            NewDeriction();
            Speed = Random.Next(5, 20);
            Velocity.X = -1;
        }

        public bool Dead { get; private set; }
        public Point2 BodyPosition { get; private set; }
        public float Speed { get; private set; }

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {

            if (HP.X < 0)
            {
                Dead = true;
                if (BodyPosition == Point2.Zero)
                {

                    BodyPosition = Bounds.Position;
                }
            }
            else
            {
                MovingLogic(gameTime, entities);
                newDirCoolDown(gameTime, entities);

                delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Bounds.Position = new Vector2(Bounds.Position.X + Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));
                rectParams = (RectangleF)Bounds;

            }


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Dead)
            {
                BodyPosition = new Point2(BodyPosition.X, BodyPosition.Y + 0.01f);
                if (facing == Facing.Right)
                {
                    spriteBatch.Draw(Texture2D, new Vector2(BodyPosition.X, BodyPosition.Y + 3), null, Color.Gray, ConvertDegreesToRadians(90), new Vector2(0, 8), 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Texture2D, new Vector2(BodyPosition.X, BodyPosition.Y + 3), null, Color.Gray, ConvertDegreesToRadians(90), new Vector2(0, 8), 1, SpriteEffects.FlipVertically, 0);
                }
                Bounds.Position = new Point2(Bounds.Position.X, 128 * 2);
                rectParams.Position = new Point2(Bounds.Position.X, 128 * 2);
            }
            else
            {

                if (Velocity.X >= 0)
                {
                    spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }

        private void MovingLogic(GameTime gameTime, List<IEntity> entities)
        {
            //  Velocity.X = 0;
            if (Velocity.Y >= 0)
            {

                Velocity.Y -= WorldParams.Gravity / 5;
            }
            else
                if (_bottom == 0)
            {
                Velocity.Y = 0;
            }

            foreach (SpriteClass item in entities)
            {


                if (item.SpriteType == SpriteType.Platform)
                {
                    if (IsTouchingTop(item))
                    {
                        _bottom = 0;
                    }
                    else
                    {
                        _bottom = 1;

                    }
                    if (_bottom == 0)
                    {

                        break;
                    }
                }
            }
            if (Velocity.Y < 0)
            {
                Velocity.Y = 0;
            }



        }


        private float _bottom = 1;
        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (!Dead)
            {
                float a = collisionInfo.Other.Bounds.Position.Y - collisionInfo.Other.Bounds.Position.Y % 8;
                if (Bounds.Position.Y - Bounds.Position.Y % 8 - (a + 8) < 0 && _bottom == 0)
                {
                    Velocity.Y = WorldParams.Gravity * 2.5f;
                }
                Bounds.Position -= collisionInfo.PenetrationVector;
                //   NewDeriction();
            }


        }
    }
}
