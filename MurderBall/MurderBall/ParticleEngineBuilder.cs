using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MurderBall
{
    class ParticleEngineBuilder
    {
        MurderBallGame parent;
        public ParticleEngineBuilder(MurderBallGame parent)
        {
            this.parent = parent;
        }

        public ParticleEngine titleExplosion()
        {
            
            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"explosion"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2));
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 5;
            engine.angularVelocityMax = 25;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(254, 254, 254);
            engine.sizeMax = 100;
            engine.sizeMin = 15;
            engine.TTLMax = 50;
            engine.TTLMin = 5;
            engine.velocityMax = new Vector2(10, 10);
            engine.velocityMin = new Vector2(-10, -10);
            engine.sourceRect = new Rectangle(150, 200, 500, 150);
            engine.lifetime = 0.5f;


            return engine;
        }

        public ParticleEngine titleExplosionSmoke()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"smoke"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2));
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 5;
            engine.angularVelocityMax = 25;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(254, 254, 254);
            engine.sizeMax = 100;
            engine.sizeMin = 50;
            engine.TTLMax = 100;
            engine.TTLMin = 50;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-1, -1);
            engine.sourceRect = new Rectangle(150, 200, 500, 150);
            engine.lifetime = 2.5f;
            engine.gravity = -0.5f;
            engine.maxParticles = 300;
            engine.generateRate = 50;


            return engine;
        }

        public ParticleEngine titleStars()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"particlecircle"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2));
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 99;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(128, 0, 0);
            engine.ColMax = new Vector3(254, 88, 88);
            engine.sizeMax = 25;
            engine.sizeMin = 1;
            engine.TTLMax = 200;
            engine.TTLMin = 25;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-5, -5);
            engine.maxParticles = 50;
            engine.sdMin = 101;
            engine.sdMax = 101;
            

            return engine;
        }


        public ParticleEngine sparks(Ball ball)
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"particle_base"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(ball.X, ball.Y));
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 99;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 3;
            engine.sizeMin = 1;
            engine.TTLMax = 25;
            engine.TTLMin = 3;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-5, -5);
            engine.lifetime = 0.5f;
            engine.isActive = false;
            engine.generateRate = 50;
            engine.maxParticles = 50;
            engine.gravity = MurderBallGame.grav;
            

            return engine;
        }

        public ParticleEngine fireSparks(Ball ball)
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"explosion"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(ball.X, ball.Y));
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 10;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 15;
            engine.sizeMin = 10;
            engine.TTLMax = 10;
            engine.TTLMin = 3;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-5, -5);
            engine.lifetime = 0.5f;
            engine.isActive = false;
            engine.generateRate = 50;
            engine.maxParticles = 50;
            //engine.gravity = MurderBallGame.grav;
            


            return engine;
        }
    }
}
