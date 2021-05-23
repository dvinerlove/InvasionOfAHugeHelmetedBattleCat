using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public class Player : SpriteClass
    {
        private float delta = 0;
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;

        public float Speed { get; private set; }

        private int _bottom = 1;
        private readonly GraphicsDevice _graphicsDevice;
        private Bullet Bullet;
        private readonly Random Random = new Random();
        private string Weapon = "";
        public int EnemyCounter = 0;
        public Texture2D WeaponTexture { get; private set; }
        public Texture2D ButtonTexture { get; set; }
        public bool DisplayButton { get; private set; }
        public Texture2D CatTexture { get; internal set; }
        public int CatCoast = 2;

        public int CatsCounter = 0;

        public Player(RectangleF rectangleF, Texture2D texture, GraphicsDevice graphicsDevice) : base(rectangleF)
        {
            _graphicsDevice = graphicsDevice;
            Texture2D = texture;
            Speed = 100;
            Velocity = Vector2.Zero;
            HP = new Vector2(9, 9);
        }

        private void InputHandler(List<IEntity> entities, CollisionComponent collisionComponent)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
            //reset
            Velocity.X = 0;
            if (Velocity.Y >= 0)
            {

                Velocity.Y -= WorldParams.Gravity / 7;
            }
            if (Velocity.Y < 0 && _bottom == 0)
            {
                Velocity.Y = 0;
            }



            //shoot
            if (_currentKey.IsKeyDown(Keys.RightControl) && _previousKey.IsKeyUp(Keys.RightControl) ||
                _currentKey.IsKeyDown(Keys.V) && _previousKey.IsKeyUp(Keys.V)
                )
            {
                Shoot(entities);

                if (Weapon != "" && Random.Next(100) > 80)
                {
                    ShootMissles(entities);
                }
            }
            //jump
            if (_currentKey.IsKeyDown(Keys.Space) && _previousKey.IsKeyUp(Keys.Space))
            {
                foreach (SpriteClass item in entities)
                {
                    if (item.SpriteType == SpriteType.Platform && IsTouchingTop(item))
                    {
                        Velocity.Y = WorldParams.Gravity * 2.5f;
                    }
                }

            }
            //Summon
            if (_currentKey.IsKeyDown(Keys.F) && _previousKey.IsKeyUp(Keys.F))
            {
                if (EnemyCounter >= CatCoast)
                {

                    Cat cat;
                    if (facing == Facing.Left)
                    {
                        cat = new Cat(new RectangleF(new Point2(rectParams.Position.X + 8, rectParams.Position.Y), new Size2(8, 8)));
                    }
                    else
                    {
                        cat = new Cat(new RectangleF(new Point2(rectParams.Position.X - 8, rectParams.Position.Y), new Size2(8, 8)));
                    }
                    cat.HP = Vector2.One;
                    cat.SpriteType = SpriteType.Cat;
                    cat.Texture2D = CatTexture;
                    entities.Add(cat);
                    collisionComponent.Insert(cat);
                    EnemyCounter -= 2;
                    CatsCounter++;
                }
            }
            //Peek
            if (_currentKey.IsKeyDown(Keys.E) && _previousKey.IsKeyUp(Keys.E))
            {
                if (PeekableObject != null)
                {
                    foreach (SpriteClass item in entities)
                    {
                        if (item == PeekableObject)
                        {
                            if (item.SpriteType == SpriteType.RocketLouncher)
                            {
                                Weapon = PeekableObject.SpriteType.ToString();
                                WeaponTexture = PeekableObject.Texture2D;

                                item.IsRemoved = true;
                                DisplayButton = false;
                            }
                            if (item.SpriteType == SpriteType.Enemy)
                            {
                                item.IsRemoved = true;
                                DisplayButton = false;
                                EnemyCounter++;
                            }

                        }
                    }
                }

            }
            //move right
            if (_currentKey.IsKeyDown(Keys.D))
            {
                Velocity.X = 1;
                facing = Facing.Right;
            }
            //move left
            if (_currentKey.IsKeyDown(Keys.A))
            {
                Velocity.X = -1;
                facing = Facing.Left;
            }

            MoveCheck(entities);
        }


        private void ShootMissles(List<IEntity> entities)
        {

            Bullet = new Bullet(new RectangleF(Bounds.Position, new Size2(1, 1)))
            {
                Parent = this,
                BulletSpeed = 150,
                BulletDamage = 1,
                LifeSpan = 5f,
                BulletType = "Missle"
            };
            entities.Add(Bullet);
            ableToShoot = false;

        }


        private void Shoot(List<IEntity> entities)
        {
            AddBullet();

            Bullet = new Bullet(new RectangleF(Bounds.Position, new Size2(2, 1)))
            {
                Parent = this,
                BulletSpeed = 150,
                BulletDamage = 1,
                LifeSpan = 5f
            };
            if (facing == Facing.Right)
            {
                Bullet.Velocity.X = 1;
                Bullet.Bounds.Position = new Point2(Bounds.Position.X + 7, Bounds.Position.Y + 2);
            }
            else
            {
                Bullet.Bounds.Position = new Point2(Bounds.Position.X - 1, Bounds.Position.Y + 2);

                Bullet.Velocity.X = -1;

            }
            entities.Add(Bullet);
        }

        private void AddBullet()
        {
        }

        private float _attackTimer = 0;
        private bool ableToShoot = false;
        private int ableToShootInt = -1;
        public TimeSpan AttackCoolDown = TimeSpan.FromSeconds(1f);

        private void ShootCoolDown(GameTime gameTime, List<IEntity> entities)
        {


            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (gameTime.TotalGameTime.TotalSeconds - AttackCoolDown.TotalSeconds > 0 &&
                    Convert.ToInt32(_attackTimer) % AttackCoolDown.TotalSeconds == 0)
                {
                    ableToShootInt = Convert.ToInt32(_attackTimer);
                    ableToShoot = true;
                }
                else
                {
                    //if (true)
                    //{

                    //}
                    //ableToShoot = false;
                }

            }


        }

        private SpriteClass PeekableObject;

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {

            InputHandler(entities, collisionComponent);
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ShootCoolDown(gameTime, entities);
            Bounds.Position = new Vector2(Bounds.Position.X + Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));
            rectParams = (RectangleF)Bounds;

            DisplayButton = false;
            PeekableObject = null;
            foreach (SpriteClass item in entities)
            {
                switch (item.SpriteType)
                {
                    case SpriteType.Player:
                        break;
                    case SpriteType.Platform:
                        break;
                    case SpriteType.Enemy:
                        if ((item as Enemy).IsRemoved == false && Vector2.Distance(new Vector2(Bounds.Position.X - 4, Bounds.Position.Y), new Vector2((item as Enemy).BodyPosition.X - 4, (item as Enemy).BodyPosition.Y)) < 8)
                        {
                            DisplayButton = true;
                            PeekableObject = item;
                        }
                        break;
                    case SpriteType.RocketLouncher:
                        if ((item as RocketLouncher).Peekable >= 0 && (item as RocketLouncher).IsRemoved == false && Vector2.Distance(new Vector2(Bounds.Position.X - 4, Bounds.Position.Y), new Vector2(item.Bounds.Position.X - 4, item.Bounds.Position.Y)) < 12)
                        {
                            DisplayButton = true;
                            PeekableObject = item;
                        }
                        break;
                }
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            ///Button Render



            if (Weapon != "")
            {
                spriteBatch.Draw(WeaponTexture, new Vector2(Bounds.Position.X, Bounds.Position.Y - 4), null, Color.Gray, 0, Origin, 1, SpriteEffects.None, 0);

            }
            switch (facing)
            {
                case Facing.Right:
                    spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.White, 0, Origin, 1, SpriteEffects.None, 0);

                    break;
                case Facing.Left:
                    spriteBatch.Draw(Texture2D, new Vector2(Bounds.Position.X, Bounds.Position.Y), null, Color.White, 0, Origin, 1, SpriteEffects.FlipHorizontally, 0);

                    break;
            }
            if (DisplayButton)
            {
                spriteBatch.Draw(ButtonTexture, new Vector2(Bounds.Position.X, Bounds.Position.Y - 8), null, Color.White, 0, Origin, 1, SpriteEffects.None, 0);

            }
            // spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Black, 1);
        }
        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.PenetrationVector.Y > 0)
            {
                Bounds.Position -= collisionInfo.PenetrationVector;

            }
            else
            {
                Bounds.Position = new Point2(Bounds.Position.X - collisionInfo.PenetrationVector.X, Bounds.Position.Y);

            }
            Debug.WriteLine(collisionInfo.PenetrationVector.Y + " " + Bounds.Position.Y);
        }

        private void MoveCheck(List<IEntity> sprites)
        {
            if (sprites != null)
            {
                foreach (SpriteClass sprite in sprites)
                {
                    if (sprite.SpriteType == SpriteType.Platform)
                    {
                        if (IsTouchingTop(sprite))
                        {
                            _bottom = 0;
                            break;
                        }
                        else
                        {
                            _bottom = 1;
                        }
                    }


                }
            }
        }



    }
}
