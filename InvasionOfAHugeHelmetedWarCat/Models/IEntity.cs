using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System.Collections.Generic;


namespace InvasionOfAHugeHelmetedWarCat.Models
{
    public interface IEntity : ICollisionActor
    {
        void Update(GameTime gameTime, List<IEntity> entities);
        void Draw(SpriteBatch spriteBatch);
    }
}
