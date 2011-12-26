using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MurderBall
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MurderBallGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public AudioEngine audio = null;
        
        Boolean titleState = true;
        Boolean matchState = false;
        Boolean endState = false;

        List<Ball> listBalls; 

        Player player1, player2;
        
        public const int ScreenHeight = 600;
        public const int ScreenWidth = 800;
        public static Rectangle rCourt = new Rectangle(0,0,ScreenWidth,ScreenHeight);
        public const int iPlayAreaTop = 30;
        public const int iPlayAreaBottom = 420;
        public const int iPlayAreaLeft = 30;
        public const int iPlayAreaRight = 680;
        public const int iPlayAreaHalf = 400;


        KeyboardState prevState1, keyState1;
        public static Texture2D particlebase;

        public MurderBallGame()
        {
            graphics = new GraphicsDeviceManager(this);

            // Full screen here:
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            Content.RootDirectory = "Content";



            audio = new AudioEngine("Content\\MurderBallSound.xgs");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            

        }

        void InitMatch()
        {

            // TODO: use this.Content to load your game content here
            player1 = new Player(Content.Load<Texture2D>(@"man1"), 1,this);
            player2 = new Player(Content.Load<Texture2D>(@"man2"), 2,this);
            player1.Foe = player2;
            player2.Foe = player1;

            listBalls = new List<Ball>();
            LoadParticles();

            for (int i = 0; i < 4; i++)
            {
                listBalls.Add(new Ball(Content.Load<Texture2D>(@"ball"), this));

                listBalls[i].X = 200 + (i * 100);
                listBalls[i].Y = 320;
                listBalls[i].IsActive = true;

            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        ///
        /// <summary> Particle stuffs </summary>
        protected void LoadParticles()
        {
            particlebase = Content.Load<Texture2D>(@"particle_base");
             
        }

        void checkExitKey(KeyboardState keyboardState, GamePadState gamePadState)
        {
            if (prevState1 == keyState1)
                return;
            // Check to see whether ESC was pressed on the keyboard 
            // or BACK was pressed on the controller.
            if (keyboardState.IsKeyDown(Keys.Escape) ||
                gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                if (matchState)
                {
                    matchState = false;
                    titleState = true;
                }
                else if (titleState)
                {
                    Exit();
                }
                
            }
            
        }

         /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {


            if (matchState) {
                MatchUpdate(gameTime);
            } else if (titleState) {
                TitleUpdate(gameTime);
            }
        }

        void TitleUpdate(GameTime gameTime)
        {
            // Title screen stuff heres.

            // TODO: Add your update logic here
            keyState1 = Keyboard.GetState();


            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));

            if (keyState1.IsKeyDown(Keys.G) || keyState1.IsKeyDown(Keys.L))
            {
                this.InitMatch();
                matchState = true;
                titleState = false;
                
            }
            prevState1 = keyState1;

        }

        /// <summary>
        ///  Update Method for game during matches.
        /// </summary>
        /// <param name="gameTime"></param>
        void MatchUpdate(GameTime gameTime)
        {
           


            
            // TODO: Add your update logic here
            keyState1 = Keyboard.GetState();


            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));

           
            KeyInputHandler(Keyboard.GetState(),
                GamePad.GetState(PlayerIndex.One),
                player1);

            KeyInputHandler(Keyboard.GetState(),
                GamePad.GetState(PlayerIndex.One),
                player2);
            
            prevState1 = keyState1;
            

            player1.Update(gameTime);
            player2.Update(gameTime);
            UpdateBalls(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates all of the balls.
        /// </summary>
        protected void UpdateBalls(GameTime gameTime)
        {
            foreach (Ball ball in listBalls)
            {
                ball.Update(gameTime, player1, player2);
                
            }
        }

        protected void KeyFireHandler(KeyboardState keys, GamePadState pad, Player player)
        {
            if (prevState1 == keyState1)
                return;

            if (keys.IsKeyDown(player.keyFire))
            {

                if (player.HasBall)
                {
                    player.FireBall();

                    return;
                }
                else
                {
                    foreach (Ball ball in listBalls)
                    {
                        if (ball.Fired)
                            continue;
                        if (player.BoundingBox.Intersects(ball.BoundingBox))
                        {
                            player.GrabBall(ball);

                            break;
                        }
                    }

                }

            }
        }

        /// <summary>
        /// Checks player input. Specifically, keyboard input.
        /// </summary>
        protected void KeyInputHandler(KeyboardState keys, 
            GamePadState pad, 
            Player player)
        {
            int LeftBound, RightBound;
            bool bResetTimer = false;

            KeyFireHandler(keys, pad, player);

            if (keys.IsKeyDown(player.keyRoll))
            {
                // Roll call here bitch.
            }

            if (keys.IsKeyUp(player.keyDown) &&
                keys.IsKeyUp(player.keyUp) &&
                keys.IsKeyUp(player.keyLeft) &&
                keys.IsKeyUp(player.keyRight))
            {
                player.Moving = false;
                return;
            }
            
            if ((keys.IsKeyDown(player.keyUp) || 
                pad.ThumbSticks.Left.Y > 0)) {
                    if (player.BoundingBox.Top > rCourt.Top)
                    {
                        player.Y -= player.MoveRate;
                        player.Moving = true;
                        bResetTimer = true;
                    }
            }
            
            if ((keys.IsKeyDown(player.keyDown) ||
                pad.ThumbSticks.Left.Y < 0))
            {
                if (player.BoundingBox.Bottom < rCourt.Bottom)
                {
                    player.Y += player.MoveRate;
                    player.Moving = true;
                    bResetTimer = true;
                }
            }

            LeftBound = rCourt.Left;
            RightBound = rCourt.Right;
            
            if (player.PlayerNum == 1)
            {
                
                RightBound = iPlayAreaHalf;
            }
            else if (player.PlayerNum ==2)
            {
                LeftBound = iPlayAreaHalf;
               
            }

            if ((keys.IsKeyDown(player.keyLeft) ||
                pad.ThumbSticks.Left.X < 0))
            {
                if (player.BoundingBox.Left > LeftBound)
                {
                    player.X -= player.MoveRate;
                    player.Moving = true;
                    bResetTimer = true;
                }
            } 
            if ((keys.IsKeyDown(player.keyRight) ||
                pad.ThumbSticks.Left.X > 0))
            {
                if (player.BoundingBox.Right < RightBound)
                {
                    player.X += player.MoveRate;
                    player.Moving = true;
                    bResetTimer = true;
                }
            }
            
            
            
            
            if (bResetTimer)
                player.VerticalChangeCount = 0f;

        }

        protected override void Draw(GameTime gameTime)
        {
            if (matchState)
            {
                DrawMatch(gameTime);
            }
            else if (titleState)
            {
                DrawTitle(gameTime);
            }
            base.Draw(gameTime);
        }

        void DrawTitle(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.End();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void DrawMatch(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);

            foreach (Ball ball in listBalls)
            {
                if (ball.IsActive)
                {
                    ball.Draw(spriteBatch);
                    ball.particleSys.Draw(spriteBatch, 1, Vector2.Zero);
                    
                }
         
            }
            spriteBatch.End();
           
        }

        public AudioEngine Audio
        {
            get { return audio; }
        }


    }
}

