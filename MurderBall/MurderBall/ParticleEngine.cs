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


        // Particle settings:
        public BlendState blendState { get; set; }
        public Vector2 velocityMax { get; set; }
        public Vector2 velocityMin { get; set; }
        public int angleMax { get; set; }
        public int angleMin { get; set; }
        public int angularVelocityMax { get; set; }
        public int angularVelocityMin { get; set; }
        public float gravity { get; set; }

        public Vector3 ColMax { get; set; }
        public Vector3 ColMin { get; set; }
        public int sizeMax { get; set; }
        public int sizeMin { get; set; }
        public int TTLMax { get; set; } // time to live
        public int TTLMin { get; set; } // time to live
        public Rectangle sourceRect { get; set; }
        public int maxParticles { get; set; }
        public int sdMin { get; set; }// Scaling delta min
        public int sdMax { get; set; } // Scaling delta max
        public int generateRate { get; set; }


        public Boolean isActive { get; set; }
        public float lifetime { get; set; } // How long these particles go for. If lifetime =0, goes for ever.
        private double timeElapsed; // Keeps track of how long this dude has been spewing stuff for.


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

            // Default values:
            isActive = true;
            lifetime = 0;
            timeElapsed = 0;
            blendState = new BlendState();
            blendState.AlphaSourceBlend = Blend.One;
            blendState.AlphaDestinationBlend = Blend.One;
            blendState.ColorBlendFunction = BlendFunction.Add;
            maxParticles = 200;
            sdMax = 100;
            sdMin = 100;
            gravity = 0.0f;
            generateRate = 10;
        }

        public void start() {
            particles.Clear();
            timeElapsed = 0;
            isActive = true;
        }

        /// <summary>
        /// Creates a new random particle
        /// </summary>
        /// <returns></returns>
        private Particle GenerateNewParticle()
        {
            Vector2 pos = new Vector2();
            if (sourceRect.IsEmpty)
            {
                pos = emitterLocation;
            }
            else
            {
                pos.X = random.Next(sourceRect.Left, sourceRect.Right);
                pos.Y = random.Next(sourceRect.Top, sourceRect.Bottom);
            }

            Texture2D texture = textures[random.Next(textures.Count)];
            
            Vector2 velocity = new Vector2((float)random.Next((int)velocityMin.X, (int)velocityMax.X),
                (float)random.Next((int)velocityMin.Y, (int)velocityMax.Y)              );

            float angle = (float)random.Next(angleMin, angleMax)/100;

            float angularVelocity = (float)random.Next((int)angularVelocityMin, (int)angularVelocityMax)/100;
            
            Color color = new Color(
                (float)random.Next((int)ColMin.X, (int)ColMax.X),
                (float)random.Next((int)ColMin.Y, (int)ColMax.Y),
                (float)random.Next((int)ColMin.Z, (int)ColMax.Z));

            float size = (float)random.Next(sizeMin, sizeMax)/100;
            int ttl = random.Next(TTLMin, TTLMax);

            float sizeDelta = (float)random.Next(sdMin, sdMax) / 100;

            return new Particle(texture, 
                pos,
                velocity,
                angle,
                angularVelocity,
                color,
                size,
                ttl,
                sizeDelta,
                gravity);
        }

        

        /// <summary>
        /// Updates particle system.
        /// </summary>
        public void Update(GameTime gametime)
        {
            int total = generateRate;
            timeElapsed += gametime.ElapsedGameTime.TotalSeconds;

            if ((lifetime > 0) && (timeElapsed > lifetime))
                isActive = false;
            // If active, add particles.
            if (isActive)
            {
                // Add some new particles
                for (int i = 0; i < total; i++)
                {
                    if (particles.Count < maxParticles)
                        particles.Add(GenerateNewParticle());

                }
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (Particle p in particles) {
                p.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    
    }
}

