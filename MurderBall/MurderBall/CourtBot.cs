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
        private const String round1Text = "Round 1";
        private const String round2Text = "Round 2";
        private const String round3Text = "Round 3";
        private const String murderballedText = "Murderball'd!";
        private const String anyKeyText = "Press a key to continue...";
        

        // Members:
        SpriteFont spriteFont; 
        public List<Ball> balls { get; set; } // Reference to list of balls used in match.
        private MurderBallGame parent;
        Random rand = new Random();
        public List<Vector2> listBallCoords = new List<Vector2>(4);
        private List<String> roundText = new List<String>();
        private String winnerText = ""; // Used to show winner. Dynamically created.

        // Particles:
        private ParticleEngineBuilder builder;
        private ParticleEngine flameGust;
        private ParticleEngine smokeGust;

        // Flags
        public Boolean hasStarted { get; set; }
        public Boolean inIntro { get; set; }
        public Boolean inCountIn { get; set; }
        public Boolean hasWinner { get; set; }

        // Timers and periods:
        private float timerFire { get; set; }
        private float periodFire { get; set; }
        private float timerHealth { get; set; }
        private float periodHealth { get; set; }
        private float timerRound { get; set; }
        private float timerToStart { get; set; }

        // Match parameters:
        private int maxBalls { get; set; }
        private int minBalls { get; set; }
        private int numRounds { get; set; }
        private int curRound { get; set; }
        private float roundLength { get; set; }



        /// <summary>
        /// Constructor for courtBot.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="listBalls"></param>
        public CourtBot(MurderBallGame parent, List<Ball> listBalls)
        {
            spriteFont = parent.Content.Load<SpriteFont>(@"CourtBot"); // load font
            balls = listBalls;
            this.parent = parent;
            // Default parameters:

             // Timers and periods:
            periodFire = 3.0f;
            periodHealth = 60.0f;
            // Match parameters:
            maxBalls = 4;
            minBalls = 4;
            numRounds = 3;           
            roundLength = 180f;

            roundText.Add(round1Text);
            roundText.Add(round2Text);
            roundText.Add(round3Text);
        }

        /// <summary>
        /// Initializes all parameters to start a game of MurderBall.
        /// Run at start of match.
        /// </summary>
        public void InitMatch() 
        {
            curRound = 0;
            //balls = new List<Ball>(maxBalls);
            listBallCoords.Add(new Vector2(200, 200));
            listBallCoords.Add(new Vector2(600, 200));
            listBallCoords.Add(new Vector2(200, 400));
            listBallCoords.Add(new Vector2(600, 400));
            hasWinner = false;
            InitParticles();
            InitRound();
        }

        /// <summary>
        /// Run this to start every round.
        /// </summary>
        public void InitRound()
        {
            inIntro = true;
            inCountIn = false;
            timerFire = 0.0f;
            timerHealth = 0.0f;
            timerRound = 0.0f;
            timerToStart = 6.0f;

            for (int i = 0; i < 4; i++)
            {
                balls.Add(new Ball(parent.Content.Load<Texture2D>(@"ball"), parent));

                balls[i].X = (int)listBallCoords[i].X;
                balls[i].Y = (int)listBallCoords[i].Y;
                balls[i].IsActive = true;

            }
        }

        /// <summary>
        /// Does what it sounds like it does.
        /// </summary>
        private void InitParticles() {
            builder = new ParticleEngineBuilder(parent);
            flameGust = builder.flameGust();
            smokeGust = builder.smokeGust();
        }

        /// <summary>
        /// Updates obviously.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (hasWinner)
            {
                return;
            }

            if (!hasStarted)
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
                    //inIntro = false;
                    hasStarted = true;
                return;
            }

            // From here, we run only once round has started:
            
            timerRound += elapsed;
            timerFire += elapsed;
            timerHealth += elapsed;

            if (timerRound > this.roundLength)
                StopRound();
            if (timerFire > periodFire)
                ShootFire();
            if (timerHealth > periodHealth)
                DropHealth();

            //if (flameGust.isActive)
            //{
            flameGust.Update(gameTime);
            smokeGust.Update(gameTime);
            //}

            
            UpdateBalls(gameTime);
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

        /// <summary>
        /// TODO: When round ends.
        /// </summary>
        private void StopRound()
        {
            
        }

        /// <summary>
        /// TODO: Shoot fire randomly.
        /// </summary>
        private void ShootFire()
        {
            timerFire = 0.0f;
            int y; 
            int x = rand.Next(MurderBallGame.rCourt.Left, MurderBallGame.rCourt.Right);
            int ySign = rand.Next(0, 1);
            
            if (ySign > 0)
                y = MurderBallGame.rCourt.Bottom;
            else
                y = MurderBallGame.rCourt.Top;

            flameGust.emitterLocation = new Vector2(x,y);
            flameGust.velocityMin = new Vector2(0,
                20 * (float)Math.Pow(-1,ySign)
                );
            flameGust.velocityMax = new Vector2(0,
                20 * (float)Math.Pow(-1, ySign)
                );

            smokeGust.emitterLocation = flameGust.emitterLocation;
            smokeGust.velocityMax = flameGust.velocityMax;
            smokeGust.velocityMin = flameGust.velocityMin;
            flameGust.start();
            smokeGust.start();
        }

        /// <summary>
        /// TODO: Drops health randomly around court.
        /// </summary>
        private void DropHealth()
        {
            timerHealth = 0.0f;
        }

        /// <summary>
        /// Runs when a player is killed.
        /// </summary>
        /// <param name="winner">The winner</param>
        public void declareWinner(Player winner)
        {
            hasWinner = true;
            hasStarted = false;
            this.winnerText = "The winner is\n" + winner.ToString();
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
                        new Vector2(250, 200), Color.Yellow);
                } else
                if (inIntro)
                {
                    spriteBatch.DrawString(spriteFont, roundText[curRound],
                        new Vector2(300, 200), Color.Yellow);
                }
                else if (inCountIn)
                {
                    spriteBatch.DrawString(spriteFont, ((int)timerToStart).ToString(), 
                        new Vector2(375, 200), Color.Yellow);
                }
                
                return;
            } 

            // Do code during match here:

            foreach (Ball ball in balls)
            {
                if (ball.IsActive)
                {
                    ball.Draw(spriteBatch);


                }

            }

            // Particles:
            //if (flameGust.isActive)
           // {
                spriteBatch.End();
                flameGust.Draw(spriteBatch);
                smokeGust.Draw(spriteBatch);
                spriteBatch.Begin();

            //}

        }
    }
}
