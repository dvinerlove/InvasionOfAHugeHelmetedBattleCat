using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace InvasionOfAHugeHelmetedWarCat.Models
{
    internal class MapGenerator
    {

        public int MapWidth { get; set; }
        public Texture2D TileSet { get; }

        private readonly List<Platform> FloorPlatforms = new List<Platform>();
        private readonly List<Platform> ShelfPlatforms = new List<Platform>();
        private readonly Random Random = new Random();

        public MapGenerator(Texture2D texture2D, int MapWidth)
        {
            TileSet = texture2D;

            this.MapWidth = MapWidth;
            CreateFloor();
            CreateShelf();
        }

        private void CreateFloor()
        {
            int y = 128;
            for (int i = 0; i < MapWidth; i++)
            {
                if (Random.Next(100) > 90)
                {
                    if (Random.Next(0, 3) == 2)
                    {
                        y += 8;
                    }
                    else
                    {
                        y -= 8;
                    }
                }
                FloorPlatforms.Add(new Platform(new MonoGame.Extended.RectangleF(i * 8, y, 8, 8), TileSet, PlatformType.Floor));
            }
        }

        private void CreateShelf()
        {

            int y = 128 - 16;
            for (int i = 0; i < MapWidth; i++)
            {
                y = (int)(FloorPlatforms[i].Bounds.Position.Y - 32);
                if (Random.Next(100) > 80)
                {
                    if (Random.Next(0, 3) == 2)
                    {
                        y += 8;
                    }
                    else
                    {
                        y -= 8;
                    }

                    if (Random.Next(0, 3) == 2)
                    {
                        ShelfPlatforms.Add(new Platform(new MonoGame.Extended.RectangleF(i * 8, y, 8, 8), TileSet, PlatformType.Shelf));
                    }
                }
            }
            int max = ShelfPlatforms.Count - 1;
            for (int i = 0; i < max; i++)
            {
                if (Random.Next(100) > 10)
                {
                    for (int j = 1; j < Random.Next(2, 6); j++)
                    {
                        ShelfPlatforms.Add(new Platform(new MonoGame.Extended.RectangleF(ShelfPlatforms[i].Bounds.Position.X + j * 8, ShelfPlatforms[i].Bounds.Position.Y, 8, 8), TileSet, PlatformType.Shelf));
                    }
                }
            }
        }

        public List<IEntity> GetShelf()
        {
            List<IEntity> entities = new List<IEntity>();

            foreach (Platform item in ShelfPlatforms)
            {
                entities.Add(item);
            }

            return entities;
        }

        public List<IEntity> GetFloor()
        {

            List<IEntity> entities = new List<IEntity>();
            foreach (Platform item in FloorPlatforms)
            {
                entities.Add(item);
            }


            return entities;
        }

    }
}
