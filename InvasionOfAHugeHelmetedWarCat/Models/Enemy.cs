using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public enum EnemyType
    {
        Civil,
        Army,
        Engineer,
        Mad
    }
    public class Enemy : SpriteClass
    {
        private float delta;
        private float _bottom = 1;
        private readonly float FieldOfView = 128;
        private RectangleF fieldOfViewRectandle;

        public bool Calaborative = false;
        private Bullet Bullet;
        private float _attackTimer = 0;
        private bool ableToShoot = false;
        private int ableToShootInt = -1;
        public TimeSpan AttackCoolDown = TimeSpan.FromSeconds(.5f);
        private float _buildTimer = 0;
        private bool ableToBuild = false;
        private int ableToBuildInt = -1;
        public TimeSpan BuildCoolDown = TimeSpan.FromSeconds(10);

        public EnemyType EnemyType { get; set; }

        private readonly Random Random = new Random();
        public float Speed { get; set; }
        public Texture2D WeaponTexture { get; set; }
        public bool Dead { get; private set; }
        public Point2 BodyPosition { get; private set; }
        public bool Stacked { get; private set; }
        public Texture2D LouncherTexture { get; internal set; }
        public float Damage { get; set; }
        public Texture2D ParachuteTexture { get; internal set; }

        public Enemy(RectangleF rectangleF, Texture2D texture, EnemyType enemyType/* GraphicsDevice graphicsDevice*/) : base(rectangleF)
        {
            Texture2D = texture;
            FieldOfView = 255;
            fieldOfViewRectandle = new RectangleF(rectangleF.Position.X - FieldOfView / 2, rectangleF.Position.Y - FieldOfView / 2, FieldOfView, FieldOfView);
            SpriteType = SpriteType.Enemy;
            EnemyType = enemyType;

            Speed = Random.Next(15, 25);
            if (enemyType == EnemyType.Army)
            {
                Speed += 2;
            }
            if (enemyType == EnemyType.Engineer)
            {
                Speed += 5;
            }
            _rotation = ConvertDegreesToRadians(90);
            BodyPosition = Point2.Zero;
        }
        public override void Update(GameTime gameTime, List<IEntity> entities)
        {

            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Origin = new Vector2(Bounds.Position.X - rectParams.Width / 2, Bounds.Position.Y - rectParams.Height / 2);
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
                MovingLogic(entities);
                switch (EnemyType)
                {
                    case EnemyType.Civil:
                        CivilBehavior(gameTime, entities);
                        break;
                    case EnemyType.Army:
                        ArmyBehavior(gameTime, entities);
                        break;
                    case EnemyType.Engineer:
                        EngineerBehavior(gameTime, entities);
                        break;
                    case EnemyType.Mad:
                        MadBehavior(gameTime, entities);
                        break;
                }
                rec = new RectangleF(new Point2(Bounds.Position.X - 4, Bounds.Position.Y), new Size2(16, 4));
                fieldOfViewRectandle.Position = new Vector2(Bounds.Position.X - FieldOfView / 2, Bounds.Position.Y - FieldOfView / 2);

            }


        }

        private void MadBehavior(GameTime gameTime, List<IEntity> entities)
        {
            IEntity player = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Player).FirstOrDefault();
            if (Vector2.Distance(Bounds.Position, player.Bounds.Position) > 6)
            {
                Bounds.Position = new Vector2(Bounds.Position.X - Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));

            }
            else
            {
                Bounds.Position = new Vector2(Bounds.Position.X, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));

            }

            rectParams = (RectangleF)Bounds;
            rectParams.Position = Bounds.Position;
            MeleeAttackCoolDown(gameTime, entities);
            MeleeAttack(gameTime, entities);
        }

        private void MeleeAttack(GameTime gameTime, List<IEntity> entities)
        {
            if (ableToShoot)
            {
                foreach (SpriteClass item in entities)
                {
                    if (item.SpriteType == SpriteType.Player && Vector2.Distance(item.Bounds.Position, Bounds.Position) < 10)
                    {
                        if (facing == Facing.Left)
                        {
                            Hit(new Vector2(Bounds.Position.X + 8, Bounds.Position.Y + 4), Velocity.X, item, Damage);
                        }
                        else
                        {
                            Hit(new Vector2(Bounds.Position.X, Bounds.Position.Y + 4), Velocity.X, item, Damage);
                        }
                    }
                }

                ableToShoot = false;

            }
        }

        private void MeleeAttackCoolDown(GameTime gameTime, List<IEntity> entities)
        {
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (Convert.ToInt32(_attackTimer) % AttackCoolDown.TotalSeconds == 0)
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
                    //  AddBullet(entities);


                }
            }
        }

        private readonly SpriteClass Friend = null;

        private void EngineerBehavior(GameTime gameTime, List<IEntity> entities)
        {

            {
                IEntity player = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Player).FirstOrDefault();

                Vector2 moveDir = player.Bounds.Position - Bounds.Position;
                moveDir.Normalize();
                if (Math.Abs(moveDir.X) < 5)
                {
                    if (moveDir.X > 0)
                    {
                        facing = Facing.Left;
                        Velocity.X = -1;
                    }
                    if (moveDir.X < 0)
                    {
                        Velocity.X = 1;
                        facing = Facing.Right;
                    }
                }
                else
                {
                    Velocity.X = 0;
                }
                if (Vector2.Distance(player.Bounds.Position, Bounds.Position) < FieldOfView)
                {
                    Bounds.Position = new Vector2(Bounds.Position.X + Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));
                }
                else
                {

                }

                Build(gameTime, entities);

                if (Math.Abs(Math.Abs(Bounds.Position.X) - Math.Abs(player.Bounds.Position.X)) < FieldOfView)
                {
                    if (player.Bounds.Position.Y - player.Bounds.Position.Y % 8 == Bounds.Position.Y - Bounds.Position.Y % 8 ||
                    player.Bounds.Position.Y - player.Bounds.Position.Y % 8 == Bounds.Position.Y - Bounds.Position.Y % 8 + 8)
                    {
                        Shoot(gameTime, entities);
                    }
                }
            }
            rectParams = (RectangleF)Bounds;
            rectParams.Position = Bounds.Position;

        }

        private void Build(GameTime gameTime, List<IEntity> entities)
        {



            _buildTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToBuildInt != Convert.ToInt32(_buildTimer))
            {
                if (/*gameTime.TotalGameTime.TotalSeconds - AttackCoolDown.TotalSeconds > 0 &&*/
                    Convert.ToInt32(_buildTimer) % BuildCoolDown.TotalSeconds == 0)
                {
                    ableToBuildInt = Convert.ToInt32(_buildTimer);
                    ableToBuild = true;
                }
                else
                {
                    ableToBuild = false;
                }
                if (ableToBuild && _bottom == 0)
                {
                    if (Velocity.X > 0)
                    {

                        RocketLouncher rocketLouncher = new RocketLouncher(new RectangleF(new Vector2(Bounds.Position.X - 8, Bounds.Position.Y - 1), new Size2(8, 8)), LouncherTexture);
                        entities.Add(rocketLouncher);
                    }
                    else
                    {

                        RocketLouncher rocketLouncher = new RocketLouncher(new RectangleF(new Vector2(Bounds.Position.X + 8, Bounds.Position.Y - 1), new Size2(8, 8)), LouncherTexture);
                        entities.Add(rocketLouncher);
                    }

                    ableToBuild = false;

                }
            }

        }

        private void ArmyBehavior(GameTime gameTime, List<IEntity> entities)
        {
            {
                IEntity player = entities.Where(x => (x as SpriteClass).SpriteType == SpriteType.Player).FirstOrDefault();
                if (Math.Abs(Math.Abs(Bounds.Position.X) - Math.Abs(player.Bounds.Position.X)) > 16)
                {
                    Bounds.Position = new Vector2(Bounds.Position.X - Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));

                }
                else
                {
                    Bounds.Position = new Vector2(Bounds.Position.X, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));
                }
                rectParams = (RectangleF)Bounds;
                rectParams.Position = Bounds.Position;
                if (Math.Abs(Math.Abs(Bounds.Position.X) - Math.Abs(player.Bounds.Position.X)) < FieldOfView)
                {
                    if (player.Bounds.Position.Y - player.Bounds.Position.Y % 8 == Bounds.Position.Y - Bounds.Position.Y % 8 ||
                    player.Bounds.Position.Y - player.Bounds.Position.Y % 8 == Bounds.Position.Y - Bounds.Position.Y % 8 + 8)
                    {
                        Shoot(gameTime, entities);
                    }
                }
            }




        }


        private void Shoot(GameTime gameTime, List<IEntity> entities)
        {
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (Convert.ToInt32(_attackTimer) % AttackCoolDown.TotalSeconds == 0)
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

        private void AddBullet(List<IEntity> entities)
        {
            Bullet = new Bullet(new RectangleF(Bounds.Position, new Size2(2, 1)))
            {
                Parent = this,
                BulletSpeed = 150,
                LifeSpan = 5f,
                BulletDamage = Damage
            };
            if (facing == Facing.Right)
            {
                Bullet.Velocity.X = -1;
                Bullet.Bounds.Position = new Point2(Bounds.Position.X + 7, Bounds.Position.Y + 2);
            }
            else
            {
                Bullet.Bounds.Position = new Point2(Bounds.Position.X - 1, Bounds.Position.Y + 2);
                Bullet.Velocity.X = 1;

            }
            entities.Add(Bullet);
        }

        private void CivilBehavior(GameTime gameTime, List<IEntity> entities)
        {

            Bounds.Position = new Vector2(Bounds.Position.X + Velocity.X * delta * Speed, Bounds.Position.Y + (WorldParams.Gravity * delta * 100 * _bottom) - (Velocity.Y * delta * 100));
            rectParams = (RectangleF)Bounds;
            rectParams.Position = Bounds.Position;

        }

        private void MovingLogic(List<IEntity> entities)
        {
            Velocity.X = 0;
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

                if (item.SpriteType == SpriteType.Player && fieldOfViewRectandle.Intersects(item.Bounds) && Friend == null
                    )
                {
                    Vector2 moveDir = item.Bounds.Position - Bounds.Position;
                    moveDir.Normalize();
                    if (Math.Abs(moveDir.X) < 5)
                    {
                        if (moveDir.X > 0)
                        {
                            facing = Facing.Left;
                            Velocity.X = -1;
                        }
                        if (moveDir.X < 0)
                        {
                            Velocity.X = 1;
                            facing = Facing.Right;
                        }
                    }
                    else
                    {
                        Velocity.X = 0;
                    }
                }

                if (item.SpriteType == SpriteType.Platform)
                {
                    if (IsTouchingTop(item))
                    {
                        _bottom = 0; ParachuteTexture = null;
                    }
                    else
                    {
                        _bottom = 1;

                    }
                    if (_bottom == 0)
                    {
                        if (rec.Intersects(item.rectParams))
                        {
                        }
                        break;
                    }
                }
            }


            if (EnemyType == EnemyType.Engineer && Friend != null && !Stacked)
            {
                Vector2 moveDir = Friend.Bounds.Position - Bounds.Position;
                moveDir.Normalize();

                if (Vector2.Distance(Friend.Bounds.Position, Bounds.Position) > 10)
                {
                    if (Math.Abs(moveDir.X) < 5)
                    {
                        if (moveDir.X > 0)
                        {
                            facing = Facing.Left;

                            Velocity.X = -1;
                        }
                        if (moveDir.X < 0)
                        {
                            Velocity.X = 1;
                            facing = Facing.Right;
                        }
                    }
                }
                else
                {
                    Stacked = true;
                }


            }

            if (Velocity.Y < 0)
            {
                Velocity.Y = 0;
            }


        }

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
            }


        }

        private RectangleF rec;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ParachuteTexture != null)
            {
                spriteBatch.Draw(ParachuteTexture, new Vector2(Bounds.Position.X, Bounds.Position.Y - 6), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            }
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

                switch (EnemyType)
                {
                    case EnemyType.Civil:
                        break;
                    case EnemyType.Army:
                        if (Velocity.X >= 0)
                        {
                            spriteBatch.Draw(WeaponTexture, new Vector2(Bounds.Position.X - 4, Bounds.Position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(WeaponTexture, new Vector2(Bounds.Position.X + 4, Bounds.Position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                        }

                        break;
                }
            }


        }
    }
}
