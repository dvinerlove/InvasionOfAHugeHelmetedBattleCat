using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public enum PlatformType
    {
        Floor,
        Shelf
    }
    public class Platform : SpriteClass
    {
        public PlatformType PlatformType { get; set; }
        public Texture2D TileSet { get; set; }
        public Texture2D Texture { get; set; }

        private readonly List<Texture2D> textures = new List<Texture2D>();
        private readonly List<Texture2D> shelfTextures = new List<Texture2D>();
        private readonly List<Texture2D> grassTextures = new List<Texture2D>();
        private readonly Random random = new Random();
        public Platform(RectangleF rectangleF, Texture2D tileSet, PlatformType platformType) : base(rectangleF)
        {
            SpriteType = SpriteType.Platform;
            TileSet = tileSet;
            PlatformType = platformType;
            switch (PlatformType)
            {
                case PlatformType.Floor:
                    for (int i = 0; i < 12; i++)
                    {
                        textures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(random.Next(0, 4) * 8, i * 8, 8, 8)));
                    }
                    grassTextures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(64 - 16 - 8 - 8, 0, 8, 8)));
                    grassTextures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(64 - 16 - 8 - 8, 8, 8, 8)));
                    break;
                case PlatformType.Shelf:
                    for (int i = 0; i < 7; i++)
                    {
                        textures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(64 - 16 + random.Next(0, 4) * 8, i * 8, 8, 8)));
                    }
                    shelfTextures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(64 - 16 - 8, 8, 8, 8)));
                    shelfTextures.Add(Texture2DExtention.GetPart(tileSet, new Rectangle(64 - 16 + 8 + 3 * 8, 8, 8, 8)));
                    break;
            }







            rectParams = (RectangleF)Bounds;
        }
        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            rectParams = (RectangleF)Bounds;

        }
        public override void Update(GameTime gameTime, List<IEntity> entities)
        {
            rectParams = (RectangleF)Bounds;

        }


        public void DrawGrass(SpriteBatch spriteBatch)
        {
            switch (PlatformType)
            {
                case PlatformType.Floor:
                    spriteBatch.Draw(grassTextures[0], new Vector2(Bounds.Position.X - 8, Bounds.Position.Y), Color.White);
                    spriteBatch.Draw(grassTextures[1], new Vector2(Bounds.Position.X + 8, Bounds.Position.Y), Color.White);

                    break;
                case PlatformType.Shelf:
                    spriteBatch.Draw(shelfTextures[0], new Vector2(Bounds.Position.X - 8, Bounds.Position.Y), Color.White);
                    spriteBatch.Draw(shelfTextures[1], new Vector2(Bounds.Position.X + 8, Bounds.Position.Y), Color.White);

                    break;

            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (PlatformType == PlatformType.Floor)
            //{


            // spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 1);
            for (int i = textures.Count - 1; i >= 0; i--)
            {
                Texture2D item = textures[i];
                spriteBatch.Draw(item, new Vector2(Bounds.Position.X, -8 + Bounds.Position.Y + i * 8), Color.White);

            }
        }
    }
}
