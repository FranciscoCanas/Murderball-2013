using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MurderBall
{
    public class ParticleEngine
    {

        private Random random;
        public Vector2 emitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="location"></param>
        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            emitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        /// <summary>
        /// Creates a new random particle
        /// </summary>
        /// <returns></returns>
        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 pos = emitterLocation;
            Vector2 velocity = new Vector2(
                1f * (float) (random.NextDouble() * 2 - 1),
                1f * (float) (random.NextDouble() * 2 - 1));

            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                (float)random.NextDouble(),
                (float)random.NextDouble(),
                (float)random.NextDouble());

            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(40);


            return new Particle(texture, 
                pos,
                velocity,
                angle,
                angularVelocity,
                color,
                size,
                ttl);
        }

        /// <summary>
        /// Updates all particles
        /// </summary>
        public void Update()
        {
            int total = 10;

            // Add some new particles
            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());

            }

            // Update and remove old particles
            for (int curP = 0; curP < particles.Count; curP++)
            {
                // Update
                particles[curP].Update();

                // Remove
                if (particles[curP].TTL <= 0)
                {
                    particles.RemoveAt(curP);
                    curP--;
                }
            }
        } // End update

        public void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Begin();
            foreach (Particle p in particles) {
                p.Draw(spriteBatch);
            }
        }
    
    }
}
