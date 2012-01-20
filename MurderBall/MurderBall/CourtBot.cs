using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MurderBall
{

    public class CourtBot
    {
        // Constants:
        private const String roundString = "Match ";
        private const String round1Text = "Round 1";
        private const String round2Text = "Round 2";
        private const String round3Text = "Round 3";
        private const String murderballedText = "Murderball'd!";
        private const String anyKeyText = "Press a key to continue...";
        private const String finishText = "Finish Him!";
        

        // Members:
        public SpriteFont spriteFont; 
        public List<Ball> balls { get; set; } // Reference to list of balls used in match.
        private MurderBallGame parent;
        Random rand = new Random();
        public List<Vector2> listBallCoords = new List<Vector2>(4);
        public List<Texture2D> courtTextures = new List<Texture2D>(4);
        private List<String> roundText = new List<String>();
        private String winnerText = ""; // Used to show winner. Dynamically created.

        // Particles:
        private ParticleEngineBuilder builder;
        private ParticleEngine flameGust;
        private ParticleEngine smokeGust;
        private ParticleEngine laser;
        private List<ParticleEngine> listBloodSplats; 

        // Flags
        public Boolean hasStarted { get; set; }
        public Boolean inIntro { get; set; }
        public Boolean inCountIn { get; set; }
        public Boolean hasWinner { get; set; }
        public Boolean playerDoneFor { get; set; }
        public Boolean startRound { get; set; }

        // Timers and periods:
        private float timerFire { get; set; }
        private float periodFire { get; set; }
        private float nextFire { get; set; }
        private float timerHealth { get; set; }
        private float periodHealth { get; set; }
        private float nextHealth { get; set; }
        private float durationHealth { get; set; }
        private float timerRound { get; set; }
        private float timerToStart { get; set; }
        private float periodLaser { get; set; }
        private float nextLaser { get; set; }
        private float timerLaser { get; set; }
        private float durationLaser { get; set; }

        // Match parameters:
        private int maxBalls { get; set; }
        private int minBalls { get; set; }
        private int numRounds { get; set; }
        public int curRound = 1;
        private float roundLength { get; set; }

        // Sound:
        SoundBank courtSounds;
        WaveBank courtWaves;
        public Cue cueCheers;
        public Cue bgMusic;
        public Cue cuePowerup;
        public Cue cueFlamer;
        public Cue cueLaser;
        public Cue cueFinishHim;
        public Cue cuePlayerOnFire1;
        public Cue cuePlayerOnFire2;
        public Cue cuePlayerElectrified1;
        public Cue cuePlayerElectrified2;
        public Cue cueGasps;
        public Cue cueBigCheers;
        public Cue cueBoos;
        
        // First Aid:
        Texture2D firstAidTexture;
        public Rectangle aidBox;
        public Boolean isAidActive;

        // Flamer:
        Texture2D flamerTexture;
        public Boolean isFlamerActive;
        public Rectangle flamerBox;
        public float timeFlamer;

        // LaserGun:
        Texture2D laserGunTexture;
        public Rectangle laserGunBox;
        public float timeLaserGun;
        private SpriteEffects laserEffects; 
        private Boolean isLaserActive;
        float laserSpeed;

        Texture2D court;

        /// <summary>
        /// Constructor for courtBot.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="listBalls"></param>
        public CourtBot(MurderBallGame parent, List<Ball> listBalls)
        {
            
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court1"));
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court2"));
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court3"));
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court4"));
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court5"));
            courtTextures.Add(parent.Content.Load<Texture2D>(@"court6"));
            spriteFont = parent.Content.Load<SpriteFont>(@"CourtBot"); // load font
            balls = listBalls;
            this.parent = parent;
            // Default parameters:

             // Timers and periods:
            periodFire = 13.0f;
            periodHealth = 20.0f;
            durationHealth = 3.0f;
            periodLaser = 16.0f;
            // Match parameters:
            maxBalls = 4;
            minBalls = 4;
            numRounds = 3;           
            roundLength = 180f;

            roundText.Add(round1Text);
            roundText.Add(round2Text);
            roundText.Add(round3Text);

            firstAidTexture = parent.Content.Load<Texture2D>(@"aid");
            aidBox = new Rectangle(0, 0, 0, 0);

            flamerTexture = parent.Content.Load<Texture2D>(@"flamer");
            flamerBox = new Rectangle(0, 0, 0, 0);

            laserGunTexture = parent.Content.Load<Texture2D>(@"laserGun");
            laserGunBox = new Rectangle(0, 0, 32, 32);


            
            
        }

        /// <summary>
        /// Initializes all parameters to start a game of MurderBall.
        /// Run at start of match.
        /// </summary>
        public void InitMatch() 
        {
            //curRound = 0;
            //balls = new List<Ball>(maxBalls);
            court = courtTextures[rand.Next(0,courtTextures.Count)];
            listBallCoords.Add(new Vector2(200, 200));
            listBallCoords.Add(new Vector2(600, 200));
            listBallCoords.Add(new Vector2(200, 400));
            listBallCoords.Add(new Vector2(600, 400));
            listBallCoords.Add(new Vector2(MurderBallGame.rCourt.Left + 65 , MurderBallGame.rCourt.Top + 30));
            listBallCoords.Add(new Vector2(MurderBallGame.rCourt.Left + 55, MurderBallGame.rCourt.Bottom - 45));
            listBallCoords.Add(new Vector2(MurderBallGame.rCourt.Right - 60, MurderBallGame.rCourt.Top + 30));
            listBallCoords.Add(new Vector2(MurderBallGame.rCourt.Right - 50, MurderBallGame.rCourt.Bottom - 45));
            hasWinner = false;
            playerDoneFor = false;

            courtWaves = new WaveBank(parent.Audio, "Content\\Court Sounds.xwb");
            courtSounds = new SoundBank(parent.Audio, "Content\\Court Cues.xsb");
            
            bgMusic = parent.titleSoundBank.GetCue("dodgeballTheme");
            

            InitParticles();
            InitRound();

        }

        /// <summary>
        /// Run this to start every round.
        /// </summary>
        public void InitRound()
        {
            inIntro = true;
            startRound = true;
            inCountIn = false;
            timerFire = 0.0f;
            nextFire = periodFire;
            nextLaser = periodLaser;
            timerLaser = 0.0f;
            timerHealth = 0.0f;
            nextHealth = periodHealth;
            timerRound = 0.0f;
            timerToStart = 6.0f;
            isAidActive = false;

            listBloodSplats.Clear();

            for (int i = 0; i < 8; i++)
            {
                balls.Add(new Ball(parent.Content.Load<Texture2D>(@"ball"), parent));

                balls[i].X = (int)listBallCoords[i].X;
                balls[i].Y = (int)listBallCoords[i].Y;
                balls[i].IsActive = true;

            }
            cueCheers = courtSounds.GetCue("cheers");
            if (!cueCheers.IsPlaying)
                cueCheers.Play();
        }

        public Texture2D getCourtTexture()
        {
            return court; 
        }

        /// <summary>
        /// Does what it sounds like it does.
        /// </summary>
        private void InitParticles() {
            builder = new ParticleEngineBuilder(parent);
            flameGust = builder.flameGust();
            smokeGust = builder.smokeGust();
            laser = builder.laser();
            listBloodSplats = new List<ParticleEngine>();
        }

        public SoundBank getSound()
        {
            return courtSounds;
        }

        /// <summary>
        /// Updates obviously.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            if (!hasStarted && startRound)
            {
                if (timerToStart < 4.0f)
                {
                    inIntro = false;
                    inCountIn = true;
                } else if (timerToStart < 10.0f)
                {
                    inIntro = true;
                }
                
                timerToStart -= elapsed;

                if (timerToStart <= 0f)
                {
                    startRound = false;
                    hasStarted = true;
                    timerToStart = 6.0f;
                }
                
                UpdateBalls(gameTime);
                UpdateParticles(gameTime);
                
                return;
            }

            // From here, we run only once round has started:

            // BG Music:
            if (hasStarted)
            {
                if (!bgMusic.IsPlaying)
                {
                    bgMusic.Play();
                }
            }

            timerRound += elapsed;
            timerFire += elapsed;
            timerHealth += elapsed;
            timerLaser += elapsed;

            if (hasStarted)
            {

                if (timerRound > this.roundLength)
                    StopRound();
                if (timerFire > nextFire)
                    ShootFire();
                if (timerHealth > nextHealth)
                    DropHealth();
                if (timerLaser > nextLaser)
                    ShootLaser();
            }

            if (isFlamerActive)
            {
                timeFlamer += elapsed;
                if ((timeFlamer > 1.0f) && (timeFlamer < 3.0f))
                {
                    if (!flameGust.isActive)
                        flameGust.start();
                    if (!smokeGust.isActive)
                        smokeGust.start();
                    if (!cueFlamer.IsPlaying)
                        cueFlamer.Play();
                }
                if (timeFlamer > 6.0f)
                {

                    isFlamerActive = false;
                    if (cueFlamer.IsPlaying)
                        cueFlamer.Stop(AudioStopOptions.Immediate);
                }
            }

            if (isLaserActive)
            {
                timerLaser += elapsed;
                laserGunBox.Y += (int)laserSpeed;
                laser.emitterLocation = new Vector2(laser.emitterLocation.X, laser.emitterLocation.Y + (int)laserSpeed);
                if ((timerLaser > 2.0f) && (timerLaser < 4.0f))
                {
                    if (!laser.isActive)
                        laser.start();
                    if (!cueLaser.IsPlaying)
                        cueLaser.Play();

                }


                if (timerLaser > 6.0f)
                {
                    
                    
                    if (cueLaser.IsPlaying)
                        cueLaser.Stop(AudioStopOptions.Immediate);
                    
                }
                if (timerLaser > 8.0f)
                    isLaserActive = false;

            }

            if (isAidActive)
            {
                if (timerHealth > durationHealth)
                {
                    isAidActive = false;
                    timerHealth = 0.0f;
                }
            }

            //if (flameGust.isActive)
            //{
            UpdateParticles(gameTime);          
            //}
            // Check for first aid player grabbba.
            if (isAidActive)
            {
                if (parent.player1.BoundingBox.Intersects(aidBox))
                {
                    HealthGrabbed(parent.player1);
                }

                if (parent.player2.BoundingBox.Intersects(aidBox))
                {
                    HealthGrabbed(parent.player2);
                    
                }

            }
            
            UpdateBalls(gameTime);
        }

        private void UpdateParticles(GameTime gameTime)
        {
            flameGust.Update(gameTime);
            smokeGust.Update(gameTime);
            laser.Update(gameTime);

            foreach (ParticleEngine splat in listBloodSplats)
            {
                
                splat.Update(gameTime);
            }


        }

        protected void HealthGrabbed(Player p)
        {
            p.HitPoints = Math.Min(Player.iMaxHitpoints, p.HitPoints + 20);
            isAidActive = false;
            cuePowerup.Play();
        }


        /// <summary>
        /// Updates all of the balls.
        /// </summary>
        protected void UpdateBalls(GameTime gameTime)
        {
            foreach (Ball ball in balls)
            {
                ball.Update(gameTime, parent.player1, parent.player2);

            }
        }


        public void AddBloodSplat(Player p, Vector2 vMax, Vector2 vMin)
        {
            ParticleEngine splat = builder.bloodSprays(p);
            splat.velocityMax = vMax;
            splat.velocityMin = vMin;
            splat.start();
            listBloodSplats.Add(splat);



        }

      
        /// <summary>
        /// TODO: When round ends.
        /// </summary>
        private void StopRound()
        {

            //StopSound();
        }

        public void StopSound()
        {
            if (bgMusic != null)
            {
                if (bgMusic.IsPlaying)
                    bgMusic.Stop(AudioStopOptions.Immediate);

            }

         
           
            if (cueCheers != null){

                if (cueCheers.IsPlaying)
                    cueCheers.Stop(AudioStopOptions.Immediate);
         
            }

            if (cueBigCheers != null)
            {
                if (cueBigCheers.IsPlaying)
                    cueBigCheers.Stop(AudioStopOptions.Immediate);
            }

            if (cueBoos != null)
            {
                if (cueBoos.IsPlaying)
                    cueBoos.Stop(AudioStopOptions.Immediate);
            }

            if (cueLaser != null)
            {
                if (cueLaser.IsPlaying)
                    cueLaser.Stop(AudioStopOptions.Immediate);
                
            }
            if (cueFlamer != null)
            {
                if (cueFlamer.IsPlaying)
                    cueFlamer.Stop(AudioStopOptions.Immediate);
                
            }
        }

        /// <summary>
        /// TODO: Shoot fire randomly.
        /// </summary>
        private void ShootFire()
        {
            timerFire = 0.0f;
            nextFire = nextItem(periodFire);
            int y=0+16; 
            int x = rand.Next(MurderBallGame.rCourt.Left + 10, MurderBallGame.rCourt.Right - 10);
            int ySign = rand.Next(0, 1);

            // Flamer box:
            flamerBox = new Rectangle(x-16, 0, 32, 32);
            timeFlamer = 0.0f;
            isFlamerActive = true;

            cueFlamer = this.courtSounds.GetCue("flamer");
            
         

            flameGust.emitterLocation = new Vector2(x,y);
            flameGust.velocityMin = new Vector2(-1,
                20 * (float)Math.Pow(-1,ySign)
                );
            flameGust.velocityMax = new Vector2(1,
                20 * (float)Math.Pow(-1, ySign)
                );

            smokeGust.emitterLocation = flameGust.emitterLocation;
            smokeGust.velocityMax = flameGust.velocityMax;
            smokeGust.velocityMin = flameGust.velocityMin;
            
        }

        private void ShootLaser()
        {
            int y = rand.Next(100, 500);
            int x = rand.Next(2);

            if (y > 300)
            {
                laserSpeed = -1;
                laser.velocityMax = new Vector2(0, laserSpeed);
                laser.velocityMin = new Vector2(0, laserSpeed);
            }
            else
            {
                laserSpeed = 1;
                laser.velocityMax = new Vector2(0, laserSpeed);
                laser.velocityMin = new Vector2(0, laserSpeed);
            }

            if (x > 0)
            {
                laser.emitterLocation = new Vector2(800, y+16);
                laser.velocityMax = new Vector2(-25, 0);
                laser.velocityMin = new Vector2(-50, 0);
                laserGunBox.X = 800 - 32;
                laserGunBox.Y = y;
                laserEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                laser.emitterLocation = new Vector2(0, y+16);
                laser.velocityMax = new Vector2(50, 0);
                laser.velocityMin = new Vector2(25, 0);
                laserGunBox.X = 0;
                laserGunBox.Y = y;
                laserEffects = SpriteEffects.None;
            }

            timerLaser = 0.0f;
            isLaserActive = true;
            cueLaser = this.courtSounds.GetCue("laser");
            nextLaser = nextItem(periodLaser);

        }

        /// <summary>
        /// TODO: Drops health randomly around court.
        /// </summary>
        private void DropHealth()
        {
            int x = rand.Next(MurderBallGame.rCourt.Left + 5, MurderBallGame.rCourt.Right - 5);
            int y = rand.Next(MurderBallGame.rCourt.Top + 5, MurderBallGame.rCourt.Bottom - 5);

            timerHealth = 0.0f;
            aidBox = new Rectangle(x,y,firstAidTexture.Width, firstAidTexture.Height);
            isAidActive = true;
            cuePowerup = courtSounds.GetCue("powerup");
            nextHealth = nextItem(periodHealth);

        }

        /// <summary>
        /// Runs when a player is killed.
        /// </summary>
        /// <param name="winner">The winner</param>
        public void declareWinner(Player winner)
        {
            if (bgMusic.IsPlaying)
                bgMusic.Stop(AudioStopOptions.Immediate);
            hasWinner = true;
            hasStarted = false;
            //StopSound();
            this.winnerText = "Winner!";
        }

        /// <summary>
        /// 
        /// </summary>
        public void callForFinish()
        {

            if ((cueFinishHim == null) || (cueFinishHim.IsPlaying))
            {
                cueFinishHim = courtSounds.GetCue("finishHim");
                playerDoneFor = true;
                cueFinishHim.Play();
            }
            

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void playerOnFireSound(Player p)
        {
            if (p.PlayerNum == 1)
            {
                //if (cuePlayerOnFire1 == null)
                    cuePlayerOnFire1 = courtSounds.GetCue("owBurning");

                if (!cuePlayerOnFire1.IsPlaying)
                    cuePlayerOnFire1.Play();
            }
            else
            {
                //if (cuePlayerOnFire2 == null)
                    cuePlayerOnFire2 = courtSounds.GetCue("owBurning");

                if (!cuePlayerOnFire2.IsPlaying)
                    cuePlayerOnFire2.Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void stopPlayerOnFireSound(Player p)
        {
            if (p.PlayerNum == 1)
            {
                if (cuePlayerOnFire1.IsPlaying)
                    cuePlayerOnFire1.Stop(AudioStopOptions.Immediate);
            }
            else
            {
                if (cuePlayerOnFire2.IsPlaying)
                    cuePlayerOnFire2.Stop(AudioStopOptions.Immediate);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void playerElectrifiedSound(Player p)
        {
            if (p.PlayerNum == 1)
            {
                //if (cuePlayerElectrified1==null)
                    cuePlayerElectrified1 = courtSounds.GetCue("electrified");
                if (!cuePlayerElectrified1.IsPlaying)
                    cuePlayerElectrified1.Play();
            }
            else
            {

                //if (cuePlayerElectrified2 == null)
                    cuePlayerElectrified2 = courtSounds.GetCue("electrified");
                if (!cuePlayerElectrified2.IsPlaying)
                    cuePlayerElectrified2.Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void stopPlayerElectrifiedSound(Player p)
        {
            if (p.PlayerNum == 1)
            {
                
                if (cuePlayerElectrified1.IsPlaying)
                    cuePlayerElectrified1.Stop(AudioStopOptions.Immediate);
            }
            else
            {
                if (cuePlayerElectrified2.IsPlaying)
                    cuePlayerElectrified2.Stop(AudioStopOptions.Immediate);
            }

        }
        /// <summary>
        /// Run every frame to draw stuffs.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!hasStarted)
            {
                // Do drawing code for pre-match here
                if (hasWinner)
                {
                    spriteBatch.DrawString(spriteFont, winnerText,
                        new Vector2(280, 5), Color.GreenYellow);
                } else if (inIntro)
                {
                    spriteBatch.DrawString(spriteFont, roundString + curRound.ToString(),
                        new Vector2(300, 5), Color.GreenYellow);
                }
                else if (inCountIn)
                {
                    spriteBatch.DrawString(spriteFont, ((int)timerToStart).ToString(),
                        new Vector2(378, 5), Color.GreenYellow);
                }
                
                return;
            }

            if (playerDoneFor)
            {
                spriteBatch.DrawString(spriteFont, finishText,
                    new Vector2(200, 5),
                    Color.GreenYellow);
            }

            // Do code during match here:
            if (isAidActive)
                spriteBatch.Draw(firstAidTexture, aidBox, Color.White);

            
            if (isFlamerActive)
                spriteBatch.Draw(flamerTexture, flamerBox, Color.White);

            if (isLaserActive)
                spriteBatch.Draw(laserGunTexture, 
                    laserGunBox, 
                    new Rectangle(0,0,32,32),
                    Color.White,
                    0.0f,
                    new Vector2(0,0),
                    laserEffects,
                    0.0f);


        }

        private float nextItem(float norm)
        {
            return norm + rand.Next(-5,5);
        }

        public void DrawBalls(SpriteBatch spriteBatch)
        {
            foreach (Ball ball in balls)
            {
                if (ball.IsActive)
                {
                    ball.Draw(spriteBatch);


                }

            }
        }

        public void DrawBloodSplats(SpriteBatch sb)
        {
            foreach (ParticleEngine splat in listBloodSplats)
            {
                splat.Draw(sb);
            }

        }
        
        public void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (Ball ball in balls)
            {
                if (ball.IsActive)
                {
                    ball.DrawParticles(spriteBatch);


                }

            }

            
            smokeGust.Draw(spriteBatch);
            flameGust.Draw(spriteBatch);
            laser.Draw(spriteBatch);
            
        }
    }
}
