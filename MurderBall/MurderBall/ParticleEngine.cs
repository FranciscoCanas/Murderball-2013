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
        public Boolean hasHitBox;

        // Particle settings:
       // public BlendState blendState { get; set; }
        public Vector2 velocityMax;
        public Vector2 velocityMin;
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
        public float ColAlphaMin { get; set; }
        public float ColAlphaMax { get; set; }
        public Vector4 colorVelocity { get; set; }
        public float startDelay { get; set; }
        public float produceDelay { get; set; }
        public SpriteSortMode spMode { get; set; }
        public BlendState bState { get; set; }



        public Boolean isActive { get; set; }
        public float lifetime { get; set; } // How long these particles go for. If lifetime =0, goes for ever.
        private double timeElapsed; // Keeps track of how long this dude has been spewing stuff for.
        private double lastProduct; // How long since last spawning of particles.

        private MurderBallGame parent;
        public int HitType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="location"></param>
        public ParticleEngine(List<Texture2D> textures, Vector2 location, MurderBallGame parent)
        {
            emitterLocation = location;
            this.parent = parent;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();

            // Default values:
            isActive = true;
            lifetime = 0;
            timeElapsed = 0;
            bState = BlendState.Additive;
            
            spMode = SpriteSortMode.Deferred;
            maxParticles = 200;
            sdMax = 100;
            sdMin = 100;
            gravity = 0.0f;
            generateRate = 10;
            ColAlphaMin = 255.0f;
            ColAlphaMax = 255.0f;
            ColMax = new Vector3(255, 255, 255);
            ColMin = new Vector3(255, 255, 255);
            startDelay = 0.0f;
            produceDelay = 0.0f;
            lastProduct = 0.0f;
            colorVelocity = new Vector4(0,0,0,0);
            hasHitBox = false;
            
        }

        public void start() {
            particles.Clear();
            timeElapsed = 0;
            startDelay = 0.0f;
            isActive = true;
        }

        public void stop()
        {
            isActive = false;

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

            float angle = (float)((random.Next(angleMin, angleMax)/360)*(2*Math.PI));

            float angularVelocity = (float)random.Next((int)angularVelocityMin, (int)angularVelocityMax)/100;
            
            Color color = new Color(
                (float)random.Next((int)ColMin.X, (int)ColMax.X),
                (float)random.Next((int)ColMin.Y, (int)ColMax.Y),
                (float)random.Next((int)ColMin.Z, (int)ColMax.Z),
                (float)random.Next((int)ColAlphaMin, (int)ColAlphaMax));

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
                gravity, 
                colorVelocity);
        }

        

        /// <summary>
        /// Updates particle system.
        /// </summary>
        public void Update(GameTime gametime)
        {
            
            int total = generateRate;
            timeElapsed += gametime.ElapsedGameTime.TotalSeconds;
            lastProduct += gametime.ElapsedGameTime.TotalSeconds;
            if ((timeElapsed < startDelay))
                return;


            if ((lifetime > 0) && (timeElapsed > lifetime))
            {
                isActive = false;

            }
            // If active, add particles.
            if (isActive && (lastProduct > produceDelay))
            {
                lastProduct = 0.0f;
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
                // Check for collisions with players if needed:
                if (hasHitBox)
                {
                    /*Rectangle hitBox = new Rectangle((int)particles[curP].position.X, 
                        (int)particles[curP].position.Y, 
                        (int)(particles[curP].texture.Width * particles[curP].size), 
                        (int)(particles[curP].texture.Height * particles[curP].size));*/

                    if (particles[curP].getHitBox().Intersects(parent.player1.BoundingBox))
                    {
                        // Hit player1 here.
                        //parent.player1.HitPoints -= 1;
                        if (!parent.player1.isRolling)
                        {
                            if (HitType == 0)
                                parent.player1.SetOnFire();
                            else if (HitType == 1)
                                parent.player1.Zap();

                        }
                    }

                    if (particles[curP].getHitBox().Intersects(parent.player2.BoundingBox))
                    {
                        // Hit player2 here.
                        //parent.player2.HitPoints -= 1;
                        if (!parent.player2.isRolling)
                        {
                            if (HitType == 0)
                                parent.player2.SetOnFire();
                            else if (HitType == 1)
                                parent.player2.Zap();

                        }
                    }
                }

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
            
            spriteBatch.Begin(spMode, bState);

            foreach (Particle p in particles) {
                p.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    
    }
}

