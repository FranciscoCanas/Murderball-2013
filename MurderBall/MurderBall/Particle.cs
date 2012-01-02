using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MurderBall
{
    public class Particle
    {
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public float angle { get; set; }
        public float angularVelocity { get; set; }
        public Color color { get; set; }
        public float size { get; set; }
        public int TTL { get; set; } // time to live
        public float sizeDelta { get; set; }
        public float gravity { get; set; }

        /// <summary>
        /// Constructor.
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        /// <param name="angle"></param>
        /// <param name="angVel"></param>
        /// <param name="col"></param>
        /// <param name="size"></param>
        /// <param name="ttl"></param>
        public Particle(Texture2D text, Vector2 pos, Vector2 vel,
            float angle, float angVel, Color col, float size, int ttl, float sizeDelta, float gravity)
        {
            this.texture = text;
            this.position = pos;
            this.velocity = vel;
            this.angle = angle;
            this.angularVelocity = angVel;
            this.color = col;
            this.size = size;
            this.TTL = ttl;
            this.sizeDelta = sizeDelta;
            this.gravity = gravity;

        }

        /// <summary>
        /// Update runs every frame.
        /// </summary>
        public void Update() 
        {
            TTL--;
            position += velocity;
            angle += angularVelocity;
            size *= sizeDelta;
            velocity = new Vector2(velocity.X, velocity.Y + gravity);
        }

        /// <summary>
        /// Draw runs every time we draw to screen.
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, position, sourceRect, color, angle,
                origin, size, SpriteEffects.None, 0f);
        }

    }
}
