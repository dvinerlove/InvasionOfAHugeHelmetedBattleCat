using InvasionOfAHugeHelmetedWarCat.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat
{
    public enum ScreenType
    {
        Title,
        Info,
        GameOver,
        Restart,
        None
    }
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Player Player;
        private List<Enemy> enemies = new List<Enemy>();
        private List<IEntity> _entities = new List<IEntity>();
        private CollisionComponent _collisionComponent;
        private DisplayMode Display;
        private OrthographicCamera _camera;
        private Matrix transformMatrix;
        private ParticleEffect bulletSparks;
        private readonly GuiSystem _guiSystem;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        private Effect allWhite;
        private Texture2D? CurrentScreen;
        private Texture2D TitleScreen;
        private Texture2D TitleScreenInfo;
        private Texture2D GameOverScreen;
        private Texture2D LoadingScreen;
        private ScreenType ScreenType = ScreenType.Title;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // LoadGui();
            _nativeRenderTarget =  new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight); 
            _entities = new List<IEntity>();
            Display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;//MonoGame.Extended.Screens.Screen.AllScreens[0]; // Change 0 for other screens

            allWhite = Content.Load<Effect>("effect");
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Display.Width, Display.Height);
            _camera = new OrthographicCamera(viewportAdapter)
            {
                Position = new Vector2(-Display.Width / 2, -Display.Height / 2)
            };
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.PreferredBackBufferWidth = Display.Width;
            _graphics.PreferredBackBufferHeight = Display.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            // WorldParams worldParams = new WorldParams();
            WorldParams.CameraZoom = 8;
            WorldParams.Gravity = 2.0f;
            WorldParams.MapLength = 250;
            _camera.Zoom = WorldParams.CameraZoom;
            _collisionComponent = new CollisionComponent(new RectangleF(0, 0, Display.Width, Display.Height));

            _font = Content.Load<SpriteFont>("font");
            Texture2D texture = Content.Load<Texture2D>("Player");
            Texture2D buttontexture = Content.Load<Texture2D>("Button_E");
            Texture2D cattexture = Content.Load<Texture2D>("LilCat");

            Player = new Player(new RectangleF(new Vector2(0, -64), new Size2(8, 8)), texture, GraphicsDevice) { CatTexture = cattexture, ButtonTexture = buttontexture };

            //screens
            TitleScreen = Content.Load<Texture2D>("TitleScreen");
            TitleScreenInfo = Content.Load<Texture2D>("TitleScreenInfo");
            GameOverScreen = Content.Load<Texture2D>("GameOverScreen");
            LoadingScreen = Content.Load<Texture2D>("LoadingScreen");
            CurrentScreen = TitleScreen;
            ScreenType = ScreenType.Title;
            //random spawning

            Texture2D enemytexture1 = Content.Load<Texture2D>("EnemyCivil");
            Texture2D enemytexture2 = Content.Load<Texture2D>("EnemyArmy");
            Texture2D enemytexture3 = Content.Load<Texture2D>("EnemyEngineer");
            Texture2D enemytexture4 = Content.Load<Texture2D>("EnemyMad");
            Texture2D texture2 = Content.Load<Texture2D>("EnemyWeapon");
            Texture2D texture3 = Content.Load<Texture2D>("Parachute");

            texture = Content.Load<Texture2D>("RocketLouncher");
            enemies = new List<Enemy>();
            for (int i = 0; i < WorldParams.MapLength; i++)
            {
                if (random.Next(0, 40) >= 35)
                {
                    enemies.Add(new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength) * 8, 32), new Size2(8, 8)), enemytexture1, EnemyType.Civil) { ParachuteTexture = texture3, EnemyType = EnemyType.Civil, HP = new Vector2(1, 2) });
                }
                if (random.Next(0, 30) == 2)
                {
                    enemies.Add(new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength / 2, WorldParams.MapLength) * 8, -128), new Size2(8, 8)), enemytexture2, EnemyType.Army) { ParachuteTexture = texture3, EnemyType = EnemyType.Army, WeaponTexture = texture2, Damage = 1, HP = new Vector2(5, 5) });
                }
                if (random.Next(0, 50) == 2)
                {
                    enemies.Add(new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength / 5, WorldParams.MapLength) * 8, -128), new Size2(8, 8)), enemytexture4, EnemyType.Mad) { ParachuteTexture = texture3, Speed = 80, Damage = 1f, HP = new Vector2(2, 2) });

                }
            }








            MapGenerator map = new MapGenerator(Content.Load<Texture2D>("tileset"), WorldParams.MapLength);
            //map.MapWidth = ;

            //AnimatedSprite animated
            bulletSparks = LoadParticles(Color.White);

            Player.HitEvent += HitEvent;
            foreach (Enemy item in enemies)
            {
                //if (item.EnemyType == EnemyType.Army)
                {
                    item.HitEvent += HitEvent;
                }
            }
            _entities.Add(Player);
            _entities.AddRange(enemies);

            _entities.AddRange(map.GetShelf());

            _entities.AddRange(map.GetFloor());


            foreach (IEntity entity in _entities)
            {

                _collisionComponent.Insert(entity);
            }

            base.Initialize();
        }

        private void HitEvent(Vector2 position, float direction, IEntity hittedSprite, float damage)
        {
            bulletSparks.Emitters[0].Profile = bulletSparks.Emitters[0].Profile = Profile.Spray(new Vector2(-direction, 0), 2f);
            bulletSparks.Trigger(position);
            bulletSparks.Trigger(position);
            bulletSparks.Trigger(position);

            shakeStart = TimeSpan.FromSeconds(.1);
            shakeSeconds = 0.1f;
            shakeViewport = true;
            shakeRadius = 0.1f;
            shakeStartAngle = (float)((Math.PI / 180) * 90) * direction;


            switch (((SpriteClass)hittedSprite).SpriteType)
            {
                case SpriteType.Player:
                    (hittedSprite as SpriteClass).HP.X -= damage;
                    HittedSprite = hittedSprite;

                    break;
                case SpriteType.Platform:
                    break;
                case SpriteType.Enemy:
                    (hittedSprite as SpriteClass).HP.X -= damage;
                    HittedSprite = hittedSprite;

                    break;
                case SpriteType.RocketLouncher:
                    (hittedSprite as SpriteClass).HP.X -= damage;
                    break;
            }
            if (gt != null)
            {
                gt.TotalGameTime = TimeSpan.Zero;

            }
        }

        private ParticleEffect LoadParticles(Color color)
        {


            Texture2D sparksTexture = new Texture2D(GraphicsDevice, 1, 1);
            sparksTexture.SetData(new[] { color });

            TextureRegion2D textureRegion = new TextureRegion2D(sparksTexture);
            ParticleEffect Sparks = new ParticleEffect(autoTrigger: true)
            {
                Position = new Vector2(-1000, -1000),
                Emitters = new List<ParticleEmitter>
            {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(0.2),
                       Profile.Spray(Vector2.Zero, 1.5f ))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 2,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(0.5f, 1.0f),
                        },
                        Modifiers =
                        {
                            new RotationModifier {RotationRate = -2.1f},
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                       new OpacityInterpolator { StartValue = 2f, EndValue = 0f }
                                }
                            }
                        },
                    }
            }
            };
            return Sparks;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {

            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            // Debug.WriteLine(Player.Bounds.Position);
            if (ScreenType != ScreenType.Restart)
            {
                if (Player.HP.X <= 0 || Player.IsRemoved)
                {
                    CurrentScreen = GameOverScreen;

                    ScreenType = ScreenType.GameOver;
                }
            }

            if (ScreenType != ScreenType.None)
            {
                if (ScreenType == ScreenType.Restart)
                {

                    Initialize();
                    CurrentScreen = null;
                    ScreenType = ScreenType.None;
                }

                if (_currentKey.IsKeyDown(Keys.R) && _previousKey.IsKeyUp(Keys.R) && ScreenType == ScreenType.GameOver)
                {

                    CurrentScreen = LoadingScreen;
                    ScreenType = ScreenType.Restart;
                }

                if (_currentKey.IsKeyDown(Keys.Space) && _previousKey.IsKeyUp(Keys.Space) && ScreenType == ScreenType.Title)
                {
                    CurrentScreen = null;

                    ScreenType = ScreenType.None;
                }
                if (_currentKey.IsKeyDown(Keys.I) && _previousKey.IsKeyUp(Keys.I))
                {
                    switch (ScreenType)
                    {
                        case ScreenType.Title:
                            CurrentScreen = TitleScreenInfo;
                            ScreenType = ScreenType.Info;
                            break;
                        case ScreenType.Info:

                            CurrentScreen = TitleScreen;
                            ScreenType = ScreenType.Title;

                            break;
                        case ScreenType.GameOver:
                            break;
                        case ScreenType.None:
                            break;
                        default:
                            break;
                    }

                }
            }
            else
            {
                if (random.Next(0, 600) >= 550)
                {
                    Texture2D enemytexture2 = Content.Load<Texture2D>("EnemyArmy");
                    Texture2D enemytexture3 = Content.Load<Texture2D>("EnemyEngineer");
                    Texture2D enemytexture4 = Content.Load<Texture2D>("EnemyMad");
                    Texture2D texture3 = Content.Load<Texture2D>("Parachute");
                    Texture2D texture2 = Content.Load<Texture2D>("EnemyWeapon");
                    Texture2D texture = Content.Load<Texture2D>("RocketLouncher");


                    if (random.Next(0, 30) == 2)
                    {
                        Enemy a1 = (new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength) * 8, -128), new Size2(8, 8)), enemytexture2, EnemyType.Army)
                        { ParachuteTexture = texture3, Damage = 1f, EnemyType = EnemyType.Army, WeaponTexture = texture2, HP = new Vector2(5, 5) });
                        a1.HitEvent += HitEvent;
                        _entities.Insert(0, a1);
                        _collisionComponent.Insert(a1);
                    }
                    if (random.Next(0, 4) == 2)
                    {
                        Enemy a1 = (new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength) * 8, -128), new Size2(8, 8)), enemytexture3, EnemyType.Engineer)
                        { ParachuteTexture = texture3, Damage = 0.5f, LouncherTexture = texture, EnemyType = EnemyType.Engineer, HP = new Vector2(1, 1) });
                        _entities.Insert(0, a1);
                        a1.HitEvent += HitEvent;
                        _collisionComponent.Insert(a1);
                    }
                    if (random.Next(0, 45) == 2)
                    {
                        Enemy a1 = (new Enemy(new RectangleF(new Vector2(random.Next(WorldParams.MapLength) * 8, -128), new Size2(8, 8)), enemytexture4, EnemyType.Mad)
                        { ParachuteTexture = texture3, Speed = 80, Damage = 1f, HP = new Vector2(2, 2) });
                        _entities.Insert(0, a1);
                        a1.HitEvent += HitEvent;
                        _collisionComponent.Insert(a1);
                    }
                }



                foreach (IEntity entity in _entities.ToArray())
                {

                    if ((entity as SpriteClass).SpriteType == SpriteType.Player)
                    {
                        (entity as SpriteClass).SetCollision(_collisionComponent);
                    }
                    if (entity.Bounds.Position.Y > 500)
                    {
                        (entity as SpriteClass).IsRemoved = true;
                        (entity as SpriteClass).HP = Vector2.Zero;
                    }
                    entity.Update(gameTime, _entities);
                }
                _camera.Position = new Vector2(Player.rectParams.X - Player.Velocity.X - Display.Width / 2, Player.rectParams.Y - Display.Height / 2);

                _collisionComponent.Update(gameTime);
                // TODO: Add your update logic here
                bulletSparks.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                PostUpdate();
                base.Update(gameTime);

            }
        }
        private void PostUpdate()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                if ((_entities[i] as Models.SpriteClass).IsRemoved)
                {
                    _collisionComponent.Remove(_entities[i]);
                    _entities.RemoveAt(i);
                    i--;
                }
            }
        }

        private double shakeStartAngle = 0;
        private double shakeRadius = 0;
        private TimeSpan shakeStart;
        private bool shakeViewport;
        private float shakeSeconds;
        private readonly Random random = new Random();
        private GameTime gt;
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
        private RenderTarget2D _nativeRenderTarget;

        public IEntity HittedSprite { get; private set; }

        protected override void Draw(GameTime gameTime)
        {

            gt = gameTime;
            // GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            GraphicsDevice.Clear(Color.FromNonPremultiplied(158, 158, 158, 255));
            GraphicsDevice.Viewport = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);


            //var rt 
            //GraphicsDevice.SetRenderTarget(rt);
            transformMatrix = _camera.GetViewMatrix();


            Vector2 offset = new Vector2(0, 0);
            if (shakeViewport)
            {
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= shakeSeconds * 5;
                shakeStartAngle += (150 + random.Next(60));
                if (gameTime.TotalGameTime.TotalSeconds - shakeStart.TotalSeconds > 0)
                {
                    shakeViewport = false;
                    HittedSprite = null;
                }
            }
            transformMatrix *= Matrix.CreateTranslation(offset.X, offset.Y, 0);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
            foreach (IEntity entity in _entities.ToArray())
            {
                if ((entity as Models.SpriteClass).SpriteType == SpriteType.Platform)
                {
                    (entity as Platform).DrawGrass(_spriteBatch);
                }
            }
            foreach (IEntity entity in _entities.ToArray())
            {

                entity.Draw(_spriteBatch);

            }
            _spriteBatch.Draw(bulletSparks);
            _spriteBatch.DrawString(_font, "eaten " + Player.EnemyCounter + "\n\nHP " + Player.HP.X, new Vector2(0, 0), Color.White);
            Transform2 transform2 = new Transform2(Vector2.Zero);
            _spriteBatch.End();

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix, effect: allWhite);
            if (HittedSprite != null)
            {
                HittedSprite.Draw(_spriteBatch);

            }
            _spriteBatch.End();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
           
            if (CurrentScreen != null)
            {
                _spriteBatch.Draw(CurrentScreen, new Rectangle(0,0, (int)_graphics.PreferredBackBufferWidth,  (int)_graphics.PreferredBackBufferHeight),  Color.White);
            }

            _spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets an array of colors for a Top Down Gradient.
        /// </summary>
        /// <param name="width">The width of the color array.</param>
        /// <param name="height">The height of the color array.</param>
        /// <returns>Color Array.</returns>
        private static Color[] GetGradientColors(uint width, uint height)
        {
            //Declare variables
            Color[] result;
            float
                increment;
            int color;

            //Determine that both height and width are greater than 0
            if (!(width > 0 && height > 0))
            //exit the function with a null color array
            { return null; }

            //Setup the result array
            result = new Color[width * height];

            //Calculate the increment values
            increment = (float)255 / (result.Length);

            //Loop through each color
            for (int i = 0; i < result.Length; i++)
            {
                color = (int)(increment * i);

                result[i] = new Color(
                    color,
                    color,
                    color);
            }

            //return the color
            return result;
        }



        private void MoveCamera(GameTime gameTime)
        {
            _camera.Zoom = 2.5f;
        }
    }
}
