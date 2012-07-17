using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        SpriteFont titleFont;

        public SoundBank titleSoundBank;
        WaveBank titleWaveBank;
        Cue titleThemeCue;
        Cue creditsThemeCue;

        ParticleEngine titleParticles;
        ParticleEngine titleExplosion;
        ParticleEngine titleSmoke;

        public CourtBot courtBot;

        public List<Ball> listBalls;
        public List<Texture2D> spriteList = new List<Texture2D>();
        public AudioEngine audio = null;
        

        Texture2D titleSprite;
        Texture2D courtSprite;
        Texture2D helpScreenSprite;
        AnimatedSprite titleSpriteAnimation;
        
        Boolean titleState = true;
        Boolean matchState = false;
        Boolean creditsState = false;
       
        Boolean helpScreenState = false;
        Boolean skipIntro = false;

        /* This keeps track of whether we are compiling arcade cabinet version of laptop version*/
        public Boolean usesArcade = false;

        public List<Texture2D> listSplats = new List<Texture2D>(3);
        
        public List<Vector2> listSplatCoords;

        public Player player1 { get; set; }
        public Player player2 { get; set; }

        public const float grav = 0.25f;
        
        public const int ScreenHeight = 600;
        public const int ScreenWidth = 800;
        //public const int ScreenHeight = 768;
        //public const int ScreenWidth = 1024;
        
        public const int iPlayAreaTop = 90;
        public const int iPlayAreaBottom = 590;
        public const int iPlayAreaLeft = 10;
        public const int iPlayAreaRight = 785;
        public const int iPlayAreaHalf = 400;
        public static Rectangle rCourt = new Rectangle(
            iPlayAreaLeft, 
            iPlayAreaTop, 
            iPlayAreaRight - iPlayAreaLeft, 
            iPlayAreaBottom - iPlayAreaTop);

        Vector2 introTextPos;
        
        Vector2 titlePos;
        Vector2 creditsTextPos;
        //float introTimer;
        private const float introLength = 47.0f;

        private const String introText = "In the not-so-distant future, global corporations hold great sway over all of humanity. The subset of people who commit crimes against these corporations are sent to massive megaprisons, and forced to take part in the brutal blood sports that fuel the entertainment industry. File sharers, music remixers, open source enthusiasts, and all other hated enemies of humanity's de-facto corporate rulers are thrown into the life or death gladiatorial competition known as...";
        /*private const String creditsText = "Blue guy was shived to death in his ultra-cell 3 days after the filming of this game. He did not recover.\n" +
            "Red guy escaped from robo-prison and made his way to the Bahamas. He is now a lawyer.\n" +
            "Green guy ate a bad sandwich and died.\n" +
            "Yellow guy starred in his own movie about the Murderball industry. He's a famous celebrity today.\n" +
            " The End."*/
        private const String creditsText = "Murderball 2013\n\n\n\n\n\n\n" +
            "Bit Manipulation and Reasoning:\n"+
            "Francisco\n\n\n\n\n"+
            "Neon Visual Direction:\n"+
            "Adrian\n\n\n\n\n"+
            "Apocalyptic Historic Revisioning:\n" +
            "Francisco\n\n\n\n\n" +
            "Pictographical Manufacturing:\n" +
            "Adrian\n\n\n\n\n" +
            "Pyrotechnical Fabrications\n" +
            "Francisco\n\n\n\n\n" +
            "2-Dimensional Image Kinetics:\n" +
            "Adrian\n\n\n\n\n" +
            "Synthetic Aural Constructions:\n"+
            "Francisco\n\n\n\n\n" +
            "Catering:\n" +
            "Adrian\n\n\n\n\n" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nThe End\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
            "...or is it?";
        private String introTextW;
        private String creditsTextW;
        private const float titleUpdateInterval = 1.0f / 28.0f;
        public static readonly Stopwatch watch = new Stopwatch();
        private float titleUpdateTimer = 0.0f;
        private Boolean titleSongCue = false;
        private Boolean titleDrawCue = false;
        private Boolean cueOrIsIt = false;
        private float titleDrawTimer = 0.0f;
        private float pressKeyTimer = 0.0f;
        private float creditsTimer = 0.0f;
        private float noWinnersTimer = 0.0f;

        KeyboardState prevState1, keyState1;
        public static Texture2D particlebase;

        public Keys key1Fire, key2Fire, key1Dodge, key2Dodge;

        public MurderBallGame()
        {
            graphics = new GraphicsDeviceManager(this);
            // Full screen here:
            
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            Content.RootDirectory = "Content";
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
            audio = new AudioEngine("Content\\MurderBallSound.xgs");
            titleWaveBank = new WaveBank(audio, "Content\\BG Music.xwb");
            titleSoundBank = new SoundBank(audio, "Content\\BG Music.xsb");
            creditsState = false;            
            matchState = false;
            titleState = true;
            skipIntro = false;
            InitTitle();

            if (usesArcade)
            {
                key1Fire = Keys.Z;
                key2Fire = Keys.A;
                key1Dodge = Keys.X;
                key2Dodge = Keys.S;
            }
            else
            {
                key1Fire = Keys.F;
                key2Fire = Keys.L;
                key1Dodge = Keys.G;
                key2Dodge = Keys.K;
            }


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            titleSprite = Content.Load<Texture2D>(@"title");
            if (usesArcade)
            {
                helpScreenSprite = Content.Load<Texture2D>(@"arcadehelpscreen");
            }
            else
            {
                helpScreenSprite = Content.Load<Texture2D>(@"helpscreen");
            }
            //titleSpriteAnimation = new AnimatedSprite(Content.Load<Texture2D>(@"transparentintro"), 0, 0, 270, 270, 72, new Vector2(1, 1), new Vector2(0, 0));

            listSplats.Add(Content.Load<Texture2D>(@"splat1"));
            listSplats.Add(Content.Load<Texture2D>(@"splat2"));
            listSplats.Add(Content.Load<Texture2D>(@"splat3"));
        }

        /// <summary>
        /// Initializes title screen.
        /// </summary>
        void InitTitle()
        {
            InitTitleParticles();
            matchState = false;
            helpScreenState = false;
            titleDrawCue = false;
            titleDrawTimer = 0.0f;
   
            titleThemeCue = titleSoundBank.GetCue("titleTheme");
            
            audio.Update();
            titleFont = Content.Load<SpriteFont>(@"TitleIntro");
            introTextPos = new Vector2(200, 650);
            titlePos = new Vector2(250, 140);
            
            titleUpdateTimer = 0.0f;
            introTextW = WrapText(titleFont,introText, 400);
            if (titleThemeCue.IsPlaying)
                titleThemeCue.Stop(AudioStopOptions.Immediate);
            titleThemeCue.Play();
            pressKeyTimer = 0.0f;
            creditsTimer = 0.0f;
            titleSongCue = false;
            watch.Restart();
        }

        void InitCredits()
        {
            creditsTextPos = new Vector2(450, 650);
            creditsTextW = WrapText(titleFont, creditsText, 800);
            creditsTimer = 0.0f;
            cueOrIsIt = false;
            courtBot.StopSound();
            creditsThemeCue = titleSoundBank.GetCue("creditsTheme");

            if (creditsThemeCue != null)
            {
                if (!creditsThemeCue.IsPlaying)
                    creditsThemeCue.Play();
            }
        }

        /// <summary>
        /// Initializes particle effects for title screen.
        /// </summary>
        void InitTitleParticles()
        {
            ParticleEngineBuilder builder = new ParticleEngineBuilder(this);
            titleParticles = builder.titleStars();
            titleExplosion = builder.titleExplosion();
            titleSmoke = builder.titleExplosionSmoke();
        }

        void InitMatch()
        {
            listBalls = new List<Ball>();
            courtBot = new CourtBot(this, listBalls);
            courtBot.InitMatch();
            spriteList.Add(Content.Load<Texture2D>(@"man1"));
            spriteList.Add(Content.Load<Texture2D>(@"man2"));
            spriteList.Add(Content.Load<Texture2D>(@"man3"));
            spriteList.Add(Content.Load<Texture2D>(@"man4"));
            noWinnersTimer = 0.0f;
            StopAllSound();
            // TODO: use this.Content to load your game content here
            //courtSprite = Content.Load<Texture2D>(@"court");
            player1 = new Player(1,this);
            player2 = new Player(2,this);
            player1.Foe = player2;
            player2.Foe = player1;

            
            LoadParticles();
            //PlaceRandomSplats();

           
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
            if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.I) ||
                gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                if (matchState || helpScreenState)
                {
                    matchState = false;
                    helpScreenState = false;
                    courtBot.StopSound();
                    InitTitle();
                    titleState = true;
                    prevState1 = keyState1;

                }else if (titleState)
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

            if (creditsState)
            {
                CreditsUpdate(gameTime);
            }

            else if (matchState)
            {
                MatchUpdate(gameTime);
            }
            else if (helpScreenState)
            {
                HelpScreenUpdate(gameTime);
            }
            else if (titleState)
            {
                TitleUpdate(gameTime);
            }
        }

        private void CreditsUpdate(GameTime gameTime)
        {
            titleUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            keyState1 = Keyboard.GetState();

            if (titleUpdateTimer > titleUpdateInterval)
            {


                titleUpdateTimer = 0.0f;
                titleParticles.Update(gameTime);
            }


            creditsTextPos.Y -= 0.4f;
            if (keyState1 != prevState1)
            {
                if (keyState1.IsKeyDown(key1Fire) || keyState1.IsKeyDown(key2Fire) || keyState1.IsKeyDown(Keys.I))
                {
                    if (creditsThemeCue.IsPlaying)
                        creditsThemeCue.Stop(AudioStopOptions.Immediate);
                    Initialize();
                }
            }
            creditsTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (creditsTimer > 10.0f)
            {
                cueOrIsIt = true;
            }

            prevState1 = keyState1;
        }

        void HelpScreenUpdate(GameTime gameTime)
        {
            titleUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            keyState1 = Keyboard.GetState();

            if (titleUpdateTimer > titleUpdateInterval)
            {
                

                titleUpdateTimer = 0.0f;
                titleParticles.Update(gameTime);
            }
            
            player1.Update(gameTime);
            player2.Update(gameTime);
            if (titleThemeCue.IsPlaying)
                titleThemeCue.Stop(AudioStopOptions.Immediate);
            if (keyState1 != prevState1)
            {
                checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));
                if (keyState1.IsKeyDown(key1Fire) ||
                    keyState1.IsKeyDown(key2Fire) || 
                    (keyState1.IsKeyDown(Keys.Space)))
                {
                    helpScreenState = false;
                    
                    matchState = true;
                    
                }
            }
            prevState1 = keyState1;


        }

        void TitleUpdate(GameTime gameTime)
        {
            pressKeyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            titleUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //introTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((float)watch.Elapsed.Seconds > introLength)
                skipIntro = true;
            if (skipIntro)
            {
                titleSongCue = true;
                titleExplosion.Update(gameTime);
                titleSmoke.Update(gameTime);
                
                if (titleDrawTimer > 1.0f)
                    titleDrawCue = true;
                titleDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            // Title screen stuff heres.
            if (titleUpdateTimer > titleUpdateInterval)
            {
                introTextPos.Y -= 1;
                
                titleUpdateTimer = 0.0f;
                titleParticles.Update(gameTime);
            }

            

            // TODO: Add your update logic here
            keyState1 = Keyboard.GetState();

            if (keyState1 != prevState1) {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));

                if (keyState1.IsKeyDown(key1Fire) || keyState1.IsKeyDown(key2Fire) || keyState1.IsKeyDown(Keys.Space))
                {
                    if (skipIntro)
                    {

                        this.InitMatch();

                        helpScreenState = true;
                        titleState = false;
                    }
                    else
                    {
                        skipIntro = true;
                    }
                
                } else if (keyState1.IsKeyDown(key1Dodge) || keyState1.IsKeyDown(key2Dodge))
                {
                    if (titleThemeCue.IsPlaying)
                        titleThemeCue.Stop(AudioStopOptions.Immediate);
                    Initialize();
                }
            }
            prevState1 = keyState1;

        }

        /// <summary>
        ///  Update Method for game during matches.
        /// </summary>
        /// <param name="gameTime"></param>
        void MatchUpdate(GameTime gameTime)
        {
            Player selector;
            courtBot.Update(gameTime);
    
            // TODO: Add your update logic here
            keyState1 = Keyboard.GetState();



            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Quit();

            }
            if (!courtBot.hasWinner)
            {
                checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));
            }
            else if (courtBot.hasWinner)
            {
                selector = player1;
                if (courtBot.winner == 2)
                    selector = player2;

                noWinnersTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // There are no winners code here:
                if ((noWinnersTimer > 4.0f) && (!selector.exploded))
                {
                    selector.Explode();
                    courtBot.noWinnerGo = true;
                }

                if (keyState1.IsKeyDown(selector.keyFire))
                {
                    // Restart Match:
                    this.InitMatch();
                    courtBot.curRound += 1;
                    courtBot.StopSound();
                    helpScreenState = true;
                    matchState = false;

                    //titleState = false;
                }
                else if (keyState1.IsKeyDown(selector.keyRoll))
                {
                    courtBot.StopSound();
                    InitCredits();
                    creditsState = true;
                    matchState = false;
                    prevState1 = keyState1;
                    return;
                }

            }

            // Checks if round has started
            if (courtBot.hasStarted || courtBot.hasWinner)
            {

                
                player1.KeyInputHandler(keyState1,
                    GamePad.GetState(PlayerIndex.One),
                    gameTime);

                player2.KeyInputHandler(keyState1,
                    GamePad.GetState(PlayerIndex.Two),
                    gameTime);
                
                prevState1 = keyState1;


                player1.Update(gameTime);
                player2.Update(gameTime);
            } 

            
           // UpdateBalls(gameTime);
            prevState1 = keyState1;
            base.Update(gameTime);
        }

        

        

        

        protected override void Draw(GameTime gameTime)
        {
            if (creditsState)
            {
                DrawCredits(gameTime);
            }
            else if (matchState)
            {
                DrawMatch(gameTime);
            }
            else if (helpScreenState)
            {
                // Help Screen here.
                DrawHelpScreen(gameTime);
            }
            else if (titleState)
            {
                DrawTitle(gameTime);
            }
            base.Draw(gameTime);
        }

        protected void DrawHelpScreen(GameTime gameTime)
        {
            // Draw title screen here:
            GraphicsDevice.Clear(Color.Black);
            titleParticles.Draw(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.Draw(helpScreenSprite, new Vector2(0, 0), Color.White);
            
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            
            spriteBatch.End();

        }

        protected void Quit()
        {
            Initialize();
        }

        void DrawCredits(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            titleParticles.Draw(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.DrawString(titleFont, 
                creditsTextW, 
                creditsTextPos, 
                Color.White);
            /*
            if (cueOrIsIt)
                spriteBatch.DrawString(titleFont,
                "...or is it?",
                new Vector2(400, 300),
                Color.White);
            */
            spriteBatch.End();
        }

        /// <summary>
        /// Draws title screen stuffs.
        /// </summary>
        /// <param name="gameTime"></param>
        void DrawTitle(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            titleParticles.Draw(spriteBatch);
            
            spriteBatch.Begin();

            if (skipIntro)
            {
                spriteBatch.DrawString(titleFont, "Push Button Red: Start Match\nPush Button Orange: Replay Intro", new Vector2(250, 550), Color.White);
            }


            if (!titleSongCue)
            {
                spriteBatch.DrawString(titleFont, introTextW, introTextPos, Color.White);
            }
            else
            {
                if (titleDrawCue)
                    spriteBatch.Draw(titleSprite, titlePos, Color.White);
                
                spriteBatch.End();
                titleSmoke.Draw(spriteBatch);
                titleExplosion.Draw(spriteBatch);
                
                spriteBatch.Begin();
                
            }

            
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
            
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.Draw(courtBot.getCourtTexture(), 
                new Vector2(0,0), 
                new Rectangle(0,0,800,600),
                Color.White,0.0f, 
                new Vector2(0,0),
                new Vector2(1,1),
                SpriteEffects.None,
                1.0f);
            spriteBatch.End();
            courtBot.DrawBloodSplats(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            courtBot.DrawBalls(spriteBatch);
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            
            courtBot.Draw(spriteBatch);
            

            
            spriteBatch.End();
            player1.DrawParticles(spriteBatch);
            player2.DrawParticles(spriteBatch);
            courtBot.DrawParticles(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            DrawHUD(spriteBatch);
            if (courtBot.hasWinner)
                spriteBatch.DrawString(titleFont, "Push Button Red: Next Match\nPush Button Orange: Show Credits", new Vector2(250, 550), Color.White);
            spriteBatch.End();
           
        }

        /// <summary>
        /// Used to draw player's hitpoints, score, etc.
        
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawHUD(SpriteBatch spriteBatch) {
            int p1h = Math.Max((int)player1.HitBar, 0);
            int p2h = Math.Max((int)player2.HitBar, 0);

            spriteBatch.DrawString(courtBot.spriteFont,
                p1h.ToString(),
                new Vector2(10, 5), 
                Color.GreenYellow,
                0.0f, 
                new Vector2(0,0),
                new Vector2(1,1),
                SpriteEffects.None, 0);

            spriteBatch.DrawString(courtBot.spriteFont,
                p2h.ToString(),
                new Vector2(690, 5), Color.GreenYellow, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);

        }

        public void StopAllSound()
        {
            if (titleThemeCue != null)
            {
                if (titleThemeCue.IsPlaying)
                    titleThemeCue.Stop(AudioStopOptions.Immediate);

            }
            if (creditsThemeCue != null)
            {
                if (creditsThemeCue.IsPlaying)
                    creditsThemeCue.Stop(AudioStopOptions.Immediate);

            }
        }

        public AudioEngine Audio
        {
            get { return audio; }
        }

        /// <summary>
        /// Randomly draws some disgustin gross random splats around
        /// the court.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void PlaceRandomSplats()
        {
            Random rand = new Random();
            Vector2 place = new Vector2(0, 0);
            int numSplats = rand.Next(6);

            listSplatCoords = new List<Vector2>(numSplats);

            for (int i = 0; i < numSplats; i++)
            {

                listSplatCoords.Add(new Vector2(rand.Next(100,700), rand.Next(50,550)));

            }

        }

        private void PlaceBloodSplats(int x, int y)
        {
            listSplatCoords.Add(new Vector2(x,y));
            listSplats.Add(Content.Load<Texture2D>(@"splat1"));
        }

        /// <summary>
        /// String wrapping class.
        /// </summary>
        /// <param name="spriteFont"></param>
        /// <param name="text"></param>
        /// <param name="maxLineWidth"></param>
        /// <returns></returns>
        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }


    }
}

