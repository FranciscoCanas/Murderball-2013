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

        ParticleEngine titleParticles;
        ParticleEngine titleExplosion;
        ParticleEngine titleSmoke;

        public CourtBot courtBot;

        

        public List<Ball> listBalls;
        public AudioEngine audio = null;
        

        Texture2D titleSprite;
        Texture2D courtSprite;
        Texture2D helpScreenSprite;
        
        Boolean titleState = true;
        Boolean matchState = false;
        Boolean endState = false;
        Boolean helpScreenState = false;

       

        public List<Texture2D> listSplats = new List<Texture2D>(3);
        public List<Vector2> listSplatCoords;

        public Player player1 { get; set; }
        public Player player2 { get; set; }

        public const float grav = 0.25f;
        
        public const int ScreenHeight = 600;
        public const int ScreenWidth = 800;
        
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
        //float introTimer;
        private const float introLength = 47.0f;

        private const String introText = "In the not-so-distant future, global corporations hold great sway over all of humanity. The subset of people who have committed crimes against these corporations are forced into massive mega-prisons, and forced into taking part in the brutal blood sports that fuel the entertainment industry. File sharers, music remixers, open source enthusiasts, and all other hated enemies of humanity's de-facto corporate rulers are thrown into the life or death gladiatorial competition known as...";
        private String introTextW;
        private const float titleUpdateInterval = 1.0f / 28.0f;
        public static readonly Stopwatch watch = new Stopwatch();
        private float titleUpdateTimer = 0.0f;
        private Boolean titleSongCue = false;
        private Boolean titleDrawCue = false;
        private float titleDrawTimer = 0.0f;


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
            

            InitTitle();
         
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
            helpScreenSprite = Content.Load<Texture2D>(@"helpscreen");

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

            titleDrawCue = false;
            titleDrawTimer = 0.0f;

            titleThemeCue = titleSoundBank.GetCue("titleTheme");

            audio.Update();
            titleFont = Content.Load<SpriteFont>(@"TitleIntro");
            introTextPos = new Vector2(200, 650);
            titlePos = new Vector2(100, 125);
            titleUpdateTimer = 0.0f;
            introTextW = WrapText(titleFont,introText, 400);
            titleThemeCue.Play();
            //introTimer = 0.0f;
            titleSongCue = false;
            watch.Restart();
            


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

            if (titleThemeCue.IsPlaying)
                titleThemeCue.Stop(AudioStopOptions.AsAuthored);
            // TODO: use this.Content to load your game content here
            courtSprite = Content.Load<Texture2D>(@"court");
            player1 = new Player(Content.Load<Texture2D>(@"man1"), 1,this);
            player2 = new Player(Content.Load<Texture2D>(@"man2"), 2,this);
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
            if (keyboardState.IsKeyDown(Keys.Escape) ||
                gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                if (matchState)
                {
                    matchState = false;
                    courtBot.StopSound();
                    InitTitle();
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

        void HelpScreenUpdate(GameTime gameTime)
        {
            keyState1 = Keyboard.GetState();
            if (keyState1 != prevState1)
            {
                if (keyState1.IsKeyDown(player1.keyFire) ||
                    keyState1.IsKeyDown(player2.keyFire))
                {
                    helpScreenState = false;
                    
                    matchState = true;
                    
                }
            }
            prevState1 = keyState1;


        }

        void TitleUpdate(GameTime gameTime)
        {
            titleUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //introTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((float)watch.Elapsed.Seconds > introLength)
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

                if (keyState1.IsKeyDown(Keys.G) || keyState1.IsKeyDown(Keys.L))
                {
                    this.InitMatch();
                    helpScreenState = true;
                    titleState = false;
                
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
            courtBot.Update(gameTime);
    
            // TODO: Add your update logic here
            keyState1 = Keyboard.GetState();



            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();

            }
            checkExitKey(keyState1, GamePad.GetState(PlayerIndex.One));

            // Checks if round has started
            if (courtBot.hasStarted)
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

            base.Update(gameTime);
        }

        

        

        

        protected override void Draw(GameTime gameTime)
        {
            if (matchState)
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
            spriteBatch.Begin();
            spriteBatch.Draw(helpScreenSprite, new Vector2(0, 0), Color.White);
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

            spriteBatch.Draw(courtSprite, 
                new Vector2(0,0), 
                new Rectangle(0,0,800,600),
                Color.White,0.0f, 
                new Vector2(0,0),
                new Vector2(1,1),
                SpriteEffects.None,
                1.0f);


            courtBot.DrawBalls(spriteBatch);
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            
            courtBot.Draw(spriteBatch);
            

            DrawHUD(spriteBatch);
            spriteBatch.End();
            player1.DrawParticles(spriteBatch);
            player2.DrawParticles(spriteBatch);
            courtBot.DrawParticles(spriteBatch);
           
        }

        /// <summary>
        /// Used to draw player's hitpoints, score, etc.
        
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawHUD(SpriteBatch spriteBatch) {
            int p1h = Math.Max(player1.HitPoints, 0);
            int p2h = Math.Max(player2.HitPoints, 0);

            spriteBatch.DrawString(titleFont,
                p1h.ToString(),
                new Vector2(10, 10), Color.Yellow ,0.0f, new Vector2(0,0),new Vector2(1,1),SpriteEffects.None, 0);

            spriteBatch.DrawString(titleFont,
                p2h.ToString(),
                new Vector2(760, 10), Color.Yellow, 0.0f, new Vector2(0,0),new Vector2(1,1),SpriteEffects.None, 0);

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

