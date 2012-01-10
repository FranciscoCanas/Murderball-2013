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
            ParticleEngine engine = new ParticleEngine(particleList, 
                new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2),
                parent);
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
            ParticleEngine engine = new ParticleEngine(particleList, 
                new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2),
                parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 0;
            engine.angleMin = -3;
            engine.angleMax = 3;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 260;
            engine.sizeMin = 150;
            engine.TTLMax = 400;
            engine.TTLMin = 300;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-5, -5);
            engine.sourceRect = new Rectangle(150, 200, 500, 150);
            engine.lifetime = 1f;
            //engine.gravity = -0.01f;
            engine.maxParticles = 250;
            engine.generateRate = 5;
            engine.ColAlphaMax = 25;
            engine.ColAlphaMin = 0;
            engine.startDelay = 0.25f;
            engine.produceDelay = 0.05f;
            engine.colorVelocity = new Vector4(-5/255f,-5/255f,-5/255f,-10/255f);
            //engine.spMode = SpriteSortMode.Immediate;
            //engine.blendState = BlendState.NonPremultiplied;
            


            return engine;
        }

        public ParticleEngine titleStars()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"particlecircle"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, 
                new Vector2(MurderBallGame.ScreenWidth / 2, MurderBallGame.ScreenHeight / 2),
                parent);
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
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(ball.X, ball.Y), parent);
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
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(ball.X, ball.Y), parent);
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


        public ParticleEngine bloodSprays(Player player)
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"particle_base"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(player.X, player.Y), parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 99;
            engine.angleMin = 0;
            engine.angleMax = 99;
            engine.ColMin = new Vector3(255, 0, 0);
            engine.ColMax = new Vector3(255, 0, 0);
            engine.sizeMax = 2;
            engine.sizeMin = 1;
            engine.TTLMax = 25;
            engine.TTLMin = 3;
            engine.velocityMax = new Vector2(5, 5);
            engine.velocityMin = new Vector2(-5, -5);
            engine.lifetime = 0.5f;
            engine.isActive = false;
            engine.generateRate = 100;
            engine.maxParticles = 100;
            engine.generateRate = 100;
            engine.gravity = MurderBallGame.grav;
            engine.bState = BlendState.Opaque;
            
            return engine;
        }

        public ParticleEngine smokeGust()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"smoke"));

            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            engine.velocityMax = new Vector2(0, 0);
            engine.maxParticles = 100;
            engine.generateRate = 25;
            engine.lifetime = 2.0f;
            //engine.bState = BlendState.Additive;
            engine.sizeMax = 200;
            engine.sizeMin = 100;
            engine.gravity = MurderBallGame.grav;
            return engine;
        }

        public ParticleEngine flameGust()
        {
            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"flame"));
            //particleList.Add(parent.Content.Load<Texture2D>(@"smoke"));
            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            engine.maxParticles = 1000;
            engine.generateRate = 25;
            engine.lifetime = 2.0f;
            engine.bState = BlendState.NonPremultiplied;
            //engine.bState.;
            engine.hasHitBox = true;
            engine.isActive = false;
            engine.TTLMax = 35;
            engine.TTLMin = 20;
            engine.angleMax = 55;
            engine.angleMin = 45;
            engine.sizeMax = 50;
            engine.sizeMin = 35;
            engine.sdMin = 101;
            engine.sdMax = 103;
            engine.produceDelay = 0.00001f;


            return engine;
        }
    }
}
