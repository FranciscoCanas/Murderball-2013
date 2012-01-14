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
            engine.angleMax = 360;
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
            engine.angleMin = -9;
            engine.angleMax = 9;
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
            engine.angleMax = 360;
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
            engine.angleMax = 360;
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

        public ParticleEngine fireSparks()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"explosion"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 10;
            engine.angleMin = 0;
            engine.angleMax = 360;
            engine.ColMin = new Vector3(0, 0, 0);
            engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 10;
            engine.sizeMin = 5;
            engine.TTLMax = 50;
            engine.TTLMin = 15;
            engine.velocityMax = new Vector2(1, 0);
            engine.velocityMin = new Vector2(-1, -5);
            engine.lifetime = 3.0f;
            engine.isActive = false;
            engine.generateRate = 50;
            engine.maxParticles = 50;
            //engine.gravity = MurderBallGame.grav;
            


            return engine;
        }

        public ParticleEngine electric()
        {
            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"electric"));

            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 50;
            engine.angleMin = 0;
            engine.angleMax = 360;
            //engine.ColMin = new Vector3(0, 0, 0);
            //engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 250;
            engine.sizeMin = 50;
            engine.TTLMax = 15;
            engine.TTLMin = 5;
            engine.velocityMax = new Vector2(0, 0);
            engine.velocityMin = new Vector2(0, 0);
            engine.lifetime = 3.0f;
            engine.isActive = false;
            engine.generateRate = 5;
            engine.maxParticles = 10;
            //engine.bState = BlendState.AlphaBlend;
            
            
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
            engine.angleMax = 360;
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
            engine.maxParticles = 300;
            engine.generateRate = 300;
            engine.gravity = MurderBallGame.grav;
            engine.bState = BlendState.Opaque;
            
            return engine;
        }

        public ParticleEngine smokeGust()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"smoke2"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0),
                parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 0;
            engine.angleMin = -9;
            engine.angleMax = 9;
           
            engine.sizeMax = 40;
            engine.sizeMin = 10;
            engine.TTLMax = 50;
            engine.TTLMin = 20;
           
            engine.lifetime = 2f;
           
            engine.gravity = (-1) * MurderBallGame.grav;
            engine.maxParticles = 1000;
            engine.generateRate = 5;
            engine.ColAlphaMax = 25;
            engine.ColAlphaMin = 0;
            
            
            engine.colorVelocity = new Vector4(-5 / 255f, -5 / 255f, -5 / 255f, -10 / 255f);
            engine.isActive = false;
            engine.produceDelay = 0.0001f;
            
            engine.bState = BlendState.AlphaBlend;


            return engine;
        }

        public ParticleEngine flameGust()
        {
            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"fire"));
            particleList.Add(parent.Content.Load<Texture2D>(@"explosion"));
            particleList.Add(parent.Content.Load<Texture2D>(@"flame"));

            
            
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            engine.maxParticles = 2000;
            engine.generateRate = 20;
            engine.lifetime = 2.0f;
            engine.bState = BlendState.Additive;
            //engine.bState.;
            engine.hasHitBox = true;
            engine.isActive = false;
            engine.TTLMax = 30;
            engine.TTLMin = 20;
            engine.angleMax = 0;
            engine.angleMin = 0;
            engine.sizeMax = 20;
            engine.sizeMin = 5;
            engine.sdMin = 101;
            engine.sdMax = 103;
            engine.produceDelay = 0.000005f;
            engine.HitType = 0; // flames


            return engine;
        }


        public ParticleEngine smoke()
        {

            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"smoke2"));


            //Cue titleMusicCue = titleSoundBank.GetCue("titleTheme");
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0,0),
                parent);
            // Set up all particle parameters. TODO: Move this to helper method.
            engine.angularVelocityMin = 0;
            engine.angularVelocityMax = 0;
            engine.angleMin = -9;
            engine.angleMax = 9;
            //engine.ColMin = new Vector3(0, 0, 0);
            //engine.ColMax = new Vector3(255, 255, 255);
            engine.sizeMax = 50;
            engine.sizeMin = 15;
            engine.TTLMax = 200;
            engine.TTLMin = 100;
            engine.velocityMax = new Vector2(1, -1);
            engine.velocityMin = new Vector2(-1, -10);
            //engine.sourceRect = new Rectangle(150, 200, 500, 150);
            engine.lifetime = 4.0f;
            //engine.gravity = -0.01f;
            engine.maxParticles = 200;
            engine.generateRate = 10;
            engine.ColAlphaMax = 25;
            engine.ColAlphaMin = 0;
            engine.startDelay = 0.25f;
            engine.produceDelay = 0.1f;
            engine.colorVelocity = new Vector4(-5 / 255f, -5 / 255f, -5 / 255f, -10 / 255f);
            engine.isActive = false;
            //engine.spMode = SpriteSortMode.Immediate;
            engine.bState = BlendState.AlphaBlend;


            return engine;
        }

        public ParticleEngine laser() {
            List<Texture2D> particleList = new List<Texture2D>();
            particleList.Add(parent.Content.Load<Texture2D>(@"laser"));
           
            ParticleEngine engine = new ParticleEngine(particleList, new Vector2(0, 0), parent);
            engine.angleMax = 0;
            engine.angleMin = 0;
            engine.angularVelocityMax = 0;
            engine.angularVelocityMin = 0;
            engine.generateRate = 15;
            engine.maxParticles = 1000;
            engine.lifetime = 1.0f;
            engine.isActive = false;
            engine.hasHitBox = true;
            engine.TTLMax = 80;
            engine.TTLMin = 50;
            engine.bState = BlendState.AlphaBlend;
            engine.HitType = 1;
            
            engine.sizeMax = 50;
            engine.sizeMin = 50;
            //engine.sdMin = 101;
            //engine.sdMax = 103;
            //engine.produceDelay = 0.000005f;

            return engine;
        }

    }
}
