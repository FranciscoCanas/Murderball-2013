using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MurderBall
{
    public class Player
    {
        MurderBallGame parent;
        Texture2D texture;
        
        AnimatedSprite spriteStanding;
        AnimatedSprite spriteRolling;
        ParticleEngine bloodSpray;
        ParticleEngine smoke;
        ParticleEngine burning;
        ParticleEngine electric;
        ParticleEngine explodes;
        ParticleEngine explodeInnards;

        private const float Rotation = 0;
        private const int xScale = 1;
        private const int yScale = 1;
        private const float Depth = 0.5f;
        private const int xOrigin = 0;
        private const int yOrigin = 0;
        private const int iframeWidth = 52;
        private const int iframeHeight = 52;

        // Character stats:
        private const float fMaxPower = 25.0f;
        private float curPower = fMaxPower / 5.0f;
        float fPowerUpRate = 0.25f;
        public const int iMaxHitpoints = 150;
        float curHitPoints;
        int iMoveRate = 3; // Player's speed
        float fRollSpeed = 8.0f;
        Random rand = new Random();

        // Player states used to keep track of what to animate:
        public Boolean isMoving = false; 
        public Boolean bHasBall = false;
        public Boolean isThrowing = false;
        public Boolean isRolling = false;
        public Boolean isHit = false;
        public Boolean isDoneFor = false;
        public Boolean zapped = false;
        public Boolean onFire = false;
        public Boolean isDead = false;
        public Boolean exploded = false;
        public Boolean isDucking = false;
        public Boolean standing = true;

       

        // Various times to keep track of player state:
        private float timeHit = 0; // Used when player gets hit.
        private float timerFire = 0f; // Used when player gets set on fire. Yes this can happen. This ain't your grandmammy's dodgeball.
        private const float timerRecover = 2.0f; // Time it takes for player to get up.
        float elapsedtime = 0.0f;
        private const float timerRoll = 1.00f; // Time it takes for player to recover after rolling.
        private float timeRolled;
        private float timerZapped;
        private float isDoneForTimer;

        // Coordinates to keep track of player on screen.
        int iX = 604;
        int iY = 260;
        int iZ = 0; // used for height when gettin' thrown around.
        float fXSpeed = 0;
        float fYSpeed = 0;
        float fZSpeed = 0; // Speed at which player gets thrown.
        
        int iScrollRate = 0;
        //int iCurrentFrame = 0;
        
        Ball curBall = null;
        Player opponent;


        int iPlayer; // Player 1 or Player 2

        // Keys:
        Keys kUp = Keys.Up;
        Keys kDown = Keys.Down;
        Keys kLeft = Keys.Left;
        Keys kRight = Keys.Right;
        Keys kFire = Keys.L;
        Keys kRoll = Keys.K;

        // Input states:
        KeyboardState keyState;
        KeyboardState prevKeyState;
        
        // Constructor:
        public Player(int player, MurderBallGame parent)
        {

            int sprite = rand.Next(0, parent.spriteList.Count);
          
            texture = parent.spriteList[sprite];
            parent.spriteList.RemoveAt(sprite);
            
            

            if (player == 1)
            {
                
                spriteStanding = new AnimatedSprite(texture, 
                    0, 
                    52,
                    iframeWidth,
                    iframeHeight, 
                    3, 
                    new Vector2(xScale,yScale),
                    new Vector2(xOrigin,yOrigin));

                spriteStanding.effects = SpriteEffects.None;
                /*
              
                */
                iX = 133;
                iY = 200;
                kUp = Keys.W;
                kDown = Keys.S;
                kLeft = Keys.A;
                kRight = Keys.D;
                kFire = Keys.G;
                kRoll = Keys.F;
            }
            if (player == 2)
            {
                spriteStanding = new AnimatedSprite(texture, 
                    0, 
                    52,
                    iframeWidth,
                    iframeHeight, 
                    3,
                    new Vector2(xScale,yScale),
                    new Vector2(xOrigin,yOrigin));

                spriteStanding.effects = SpriteEffects.FlipHorizontally;

                iX = 610;
                iY = 200;
                
            }

            spriteStanding.IsAnimating = true;
            spriteStanding.FrameLength = (float)1 / 2.0f; // FPS for standing frame.
            iPlayer = player;
            this.parent = parent;
            keyState = new KeyboardState();
            prevKeyState = new KeyboardState();

            // Set up stats:
            curHitPoints = iMaxHitpoints;
            InitParticles();
            
            
        }

        public void InitParticles()
        {
            ParticleEngineBuilder builder = new ParticleEngineBuilder(parent);
            bloodSpray = builder.bloodSprays(this);
            smoke = builder.smoke();
            burning = builder.fireSparks();
            electric = builder.electric();
            explodes = builder.ManExplosion();
            explodeInnards = builder.ManExplosionInnards();

        }

        /// <summary>
        /// Handles the throwing of the ball.
        /// </summary>
        public void FireBall()
        {
            if (curBall != null)
            {
                curBall.Fire(this, opponent);
                curBall = null;
                
                bHasBall = false;
                isThrowing = false;
            }
        }

        /// <summary>
        /// Handles grabbing of ball, resets curPower and 
        /// sets all proper states.
        /// </summary>
        /// <param name="ball"></param>
        public void GrabBall(Ball ball)
        {
            curBall = ball;
            curPower = fMaxPower / 2.0f;
            bHasBall = true;
            ball.player = this;
        }

        /// <summary>
        /// Getting hit by ball.  
        /// Lowers hp, checks for death, changes animation frames.
        /// </summary>
        /// <param name="ball"></param>
        public void HitByBall(Ball ball)
        {
            // Do this when player gets hit by ball
            Vector2 vMax;
            Vector2 vMin;
            Boolean reverse= false;
            curHitPoints -= (int)ball.Power;
            timeHit = 0;
            //isHit = true;
            //isMoving = false;
            //isRolling = false;

            if (isThrowing || HasBall)
                DropBall();

            if (ball.isPowered && !isDoneFor && !isDead)
            {

                parent.courtBot.cueGasps = parent.courtBot.getSound().GetCue("gasp");
                parent.courtBot.cueGasps.Play();
            
            }

            if (isDoneFor && ball.isPowered)
                {
                    Explode();
                    parent.courtBot.cueBigCheers = parent.courtBot.getSound().GetCue("bigCheers");
                    parent.courtBot.cueBigCheers.Play();

                }

            

            //fZSpeed = -10.0f;
            fXSpeed = ball.fXSpeed /3;
            fYSpeed = ball.fYSpeed /3;

            if (fXSpeed > 0)
            {
                if (PlayerNum == 1)
                    reverse = true;

                if (fYSpeed > 0)
                {
                    vMax = new Vector2(fXSpeed,fYSpeed);
                    vMin = new Vector2(0, 0);

                }
                else
                {
                    vMax = new Vector2(fXSpeed, 0);
                    vMin = new Vector2(0, fYSpeed);
                }
            } else 
                {

                    if (PlayerNum == 2)
                        reverse = true;
                    if (fYSpeed > 0)
                    {
                        vMax = new Vector2(0, fYSpeed);
                        vMin = new Vector2(fXSpeed, 0);
                    }
                    else
                    {
                        vMax = new Vector2(0, 0);
                        vMin = new Vector2(fXSpeed, fYSpeed);
                    }

                }
      
            parent.courtBot.AddBloodSplat(this, vMax, vMin);

            if (isDoneFor && !ball.isPowered)
            {
                Dies();


                parent.courtBot.cueBoos = parent.courtBot.getSound().GetCue("boos");
                parent.courtBot.cueBoos.Play();

            }
            else
            {
                FrameHit(reverse);

                if (curHitPoints <= 0)
                    DoneFor();
            }
        }

      

        /// <summary>
        /// This ain't your mammy's dodgeball either.
        /// </summary>
        public void SetOnFire()
        {
            onFire = true;
            timerFire = 0f;
            DropBall();
            if (!smoke.isActive)
                smoke.start();
            if (!burning.isActive)
                burning.start();

            parent.courtBot.playerOnFireSound(this);
            FrameOnFire();
        }

        /// <summary>
        /// 
        /// Electricity comes from other planets.
        /// </summary>
        public void Zap()
        {
            zapped = true;
            timerZapped = 0f;
            DropBall();
            if (!electric.isActive)
                electric.start();
            parent.courtBot.playerElectrifiedSound(this);
            FrameZapped();
        }

        /// <summary>
        /// Explosive fun here.
        /// </summary>
        public void Explode()
        {
            explodes.emitterLocation = new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y);
            explodeInnards.emitterLocation = new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y); 
            exploded = true;

            explodes.start();
            explodeInnards.start();
            Dies();
        }

        /// <summary>
        /// Used upon player death.
        /// Shoot lot's of blood or something.
        /// </summary>
        public void DoneFor()
        {
            StateClear();
            isHit = true;
            isDoneFor = true;
            isDoneForTimer = 0.0f;
            spriteStanding.Repeats = false;
            spriteStanding.iFrameOffsetY = 5*52;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 0;
            spriteStanding.FrameLength = (float)1 / 4.0f;

            spriteStanding.CurrentFrame = 0;
            parent.courtBot.callForFinish();
            // Awesomeness here
        }

        public void Dies()
        {
            StateClear();
            isDoneFor = false;
            isHit = true;
            isDead = true;
            spriteStanding.Repeats = false;
            spriteStanding.iFrameOffsetY = 4*52;
            spriteStanding.iFrameOffsetX = 52;
            spriteStanding.FrameCount = 0;

            spriteStanding.CurrentFrame = 0;
            //isDoneFor = false;
            parent.courtBot.declareWinner(this.opponent);
            
        }

        public void DropBall()
        {
            if (curBall != null)
            {
                
                curBall.BallStops();
                bHasBall = false;
                curBall = null;
                isThrowing = false;
                
            }
        }

        /// <summary>
        /// Player's update method gets run every frame:
        /// Checks the player's state, and calls the appropriate
        /// method to handle changes.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            float elapsedtime =0.0f;
            elapsedtime += (float)gametime.ElapsedGameTime.TotalSeconds;
            

            // Check if player dies:
            if ((HitBar <= 0) && (!isDoneFor) && (!isDead))
                            this.DoneFor();

            smoke.Update(gametime);
            burning.Update(gametime);
            electric.Update(gametime);
            explodes.Update(gametime);
            explodeInnards.Update(gametime);

            Rectangle plocation = BoundingBox;

            if (isDead)
            {

                //StateClear();
                
                
                // Stuff here when guy dead.
            }

            if (isDoneFor)
            {
                
                isDoneForTimer += elapsedtime;

                if (isDoneForTimer > 15.0f)
                {
                    Dies();
                    if ((parent.courtBot.cueBoos == null) || (!parent.courtBot.cueBoos.IsPlaying))
                    {
                        parent.courtBot.cueBoos = parent.courtBot.getSound().GetCue("boos");
                        parent.courtBot.cueBoos.Play();
                    }
                }
                // Stuff here when guy done for.
            }
            
            if (zapped)
            {
                timerZapped += (float)gametime.ElapsedGameTime.TotalSeconds;
                curHitPoints -= 0.05f;
                electric.sourceRect = new Rectangle(plocation.X + 16, plocation.Y + 16, 16,16);
                
                // No movements allowed.
                if (timerZapped > 3.0f)
                {
                    
                    zapped = false;
                    //isMoving = true;
                    electric.isActive = false;
                    parent.courtBot.stopPlayerElectrifiedSound(this);
                }

            }
            
            if (onFire)
            {             
                timerFire += (float)gametime.ElapsedGameTime.TotalSeconds;
                curHitPoints -= (float)gametime.ElapsedGameTime.TotalSeconds * 2;
                
                smoke.sourceRect = plocation;
                burning.sourceRect = plocation;


                if (timerFire > 4.0)
                {
                    onFire = false;
                    //isMoving = true;
                    smoke.isActive = false;
                    burning.isActive = false;
                    parent.courtBot.stopPlayerOnFireSound(this);
                    FrameStanding();
                }

            }
            
            if (isRolling) // Player is rolling around.
            {
                
                UpdatePosition();
                fXSpeed *= 0.955f;
                fYSpeed *= 0.955f;
                
                onFire = false;
                timeRolled += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (timeRolled > timerRoll)
                {
                    isRolling = false;
                    //isMoving = true;
                    fXSpeed = 0;
                    fYSpeed = 0;

                }
            }
            
            if (isThrowing) // Player is throwing a ball.
            {
                
            }
            
            if (isHit) // Player just got hit.
            {
                
                fXSpeed *= 0.96f;
                fYSpeed *= 0.96f;

                UpdatePosition();

                //iZ += (int)fZSpeed;
                //fZSpeed = Math.Min(fZSpeed + MurderBallGame.grav, 0);

                timeHit += (float)gametime.ElapsedGameTime.TotalSeconds; 
                if (timeHit > timerRecover)
                {
                    //isHit = false;
                    //isMoving = true;
                    iZ = 0;
                    fXSpeed = 0;
                    fYSpeed = 0;
                    if (!isDoneFor && !isDead)
                    {
                        FrameStanding();
                    }
                    else
                    {
                        if (!isDead)
                            isHit = false;
                    }
                }
            }
            
            if (isMoving) // Player is moving around.
            {
                
                
                
              //  spriteStanding.Update(gametime);
              
            }
            else // Just standing there.
            {
                
                //spriteStanding.Frame = 0;
                //spriteStanding.Update(gametime);
                //spriteStanding.IsAnimating = true;
            }

            if (bloodSpray.isActive)
            {
                bloodSpray.sourceRect = new Rectangle(X, Y, iframeWidth / 2, iframeHeight / 2);
                bloodSpray.Update(gametime);
            }

            //UpdateFrames();

            spriteStanding.Update(gametime);
        }

        public void UpdateFrames()
        {
            
            //spriteStanding.FrameCount = 4;

            if (isDucking)
            {

            }
            else if (isHit)
            {
                /*
                spriteStanding.iFrameOffsetY = 208;
                spriteStanding.iFrameOffsetX = 0;
                spriteStanding.FrameCount = 2;*/
            }
            else if (isRolling)
            {
              
            }
            else if (isMoving && (!onFire))
            {
                spriteStanding.iFrameOffsetY = 0;
                spriteStanding.iFrameOffsetX = 0;
            }
            else if (onFire)
            {
                
            }
            else if (isThrowing)
            {
                
            }
            else
            {
                spriteStanding.iFrameOffsetY = 52;
                spriteStanding.iFrameOffsetX = 0;
            }
        }

        public void ClearStatus()
        {
            isHit = false;
            isRolling = false;
            onFire = false;
            zapped = false;
            isMoving = true;

        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePosition()
        {
            if (BoundingBox.Bottom > MurderBallGame.rCourt.Top &&
                    BoundingBox.Bottom < MurderBallGame.rCourt.Bottom)
                iY += (int)fYSpeed;

            if (BoundingBox.Left > GetLeftBound() &&
                BoundingBox.Right < GetRightBound())
                iX += (int)fXSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (!exploded)
                spriteStanding.Draw(sb, iX, iY - iZ, false, Color.White, GetDepth);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        public void DrawParticles(SpriteBatch sb)
        {
            if (bloodSpray.isActive)
            {


                bloodSpray.Draw(sb);    
            }
            
            smoke.Draw(sb);
            burning.Draw(sb);
            electric.Draw(sb);
            explodes.Draw(sb);
            explodeInnards.Draw(sb);

        }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle BoundingBox
        {
            get { return new Rectangle(iX+3, iY+1, iframeWidth-6, iframeHeight-2); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Player Foe
        {
            get { return opponent; }
            set { opponent = value; }
        }

        public Keys keyUp
        {
            get { return kUp; }
        }

        public Keys keyDown
        {
            get { return kDown; }
        }

        public Keys keyLeft
        {
            get { return kLeft; }
        }

        public Keys keyRight
        {
            get { return kRight; }
        }
        public Keys keyFire
        {
            get { return kFire; }
        }
        public Keys keyRoll
        {
            get { return kRoll; }
        }

        public int X
        {
            get { return iX; }
            set { iX = value; }
        }

        public int Y
        {
            get { return iY; }
            set { iY = value; }
        }

        public int MoveRate
        {
            get { return iMoveRate; }
            set { iMoveRate = value; }
        }
        
        public int ScrollRate
        {
            get { return iScrollRate; }
            set { iScrollRate = value; }
        }


        public Boolean HasBall
        {
            get { return bHasBall; }
        }

        public float HitPoints
        {
            get { return curHitPoints; }
            set { curHitPoints = value; }
        }

        public int HitBar
        {
            get { return (int)((curHitPoints / iMaxHitpoints) * 100); }
        }

        public float Power
        {
            get { return curPower; }
        }

        public Boolean Moving
        {
            set { isMoving = value; }
        }

        public int PlayerNum
        {
            get { return iPlayer; }
        }

        public float GetDepth
        {
            get { return (800 - (float)(Y+(iframeHeight * 1.5)))/800; }
        }

        /// <summary>
        /// Checks player input. Specifically, keyboard input.
        /// GamePad input to be done up at a future time.
        /// </summary>
        public void KeyInputHandler(KeyboardState keys, GamePadState pad, GameTime gameTime)
        {
            int LeftBound, RightBound;
            keyState = keys;

            if (isDoneFor || isHit || isRolling || zapped )
            {
                return;
            }

            
            KeyFireHandler(keyState, pad);

            if (isThrowing)
            {
                prevKeyState = keyState;
                return;
            }

            if (keyState.IsKeyDown(keyRoll) && (prevKeyState != keyState))
            {
                
                if (curBall != null)
                    return;

                
                
                if (prevKeyState.IsKeyDown(kDown))
                {
                    fYSpeed = fRollSpeed;
                    
                    //fXSpeed = 0f;
                }
                if (prevKeyState.IsKeyDown(kLeft))
                {

                    fXSpeed = -1 * fRollSpeed;
                    
                    //fYSpeed = 0f;
                }
                if (prevKeyState.IsKeyDown(kRight))
                {
                    fXSpeed = fRollSpeed;
                    
                    //fYSpeed = 0f;
                }
                if (prevKeyState.IsKeyDown(kUp))
                {
                    fYSpeed = -1 * fRollSpeed;
                    
                   
                    //fXSpeed = 0f;
                }

                if (prevKeyState.IsKeyUp(keyDown) &&
               prevKeyState.IsKeyUp(keyUp) &&
               prevKeyState.IsKeyUp(keyLeft) &&
               prevKeyState.IsKeyUp(keyRight))
                {
                    //FrameDucking();
                    if (!onFire && !isRolling)
                        FrameDucking();
                    prevKeyState = keyState;
                    //spriteStanding.
                    
                    return;
                }
                // Roll call here bitch.
                if (!isRolling)
                    FrameRolling();
                
                prevKeyState = keyState;
                return;
                
            }

            if (keyState.IsKeyUp(keyDown) &&
                keyState.IsKeyUp(keyUp) &&
                keyState.IsKeyUp(keyLeft) &&
                keyState.IsKeyUp(keyRight))
            {
                if (!onFire && !standing)
                    FrameStanding();
                prevKeyState = keyState;
                return;
            }

            if ((keyState.IsKeyDown(keyUp) ||
                pad.ThumbSticks.Left.Y > 0))
            {
                if (BoundingBox.Bottom > MurderBallGame.rCourt.Top)
                {
                    Y -= MoveRate;
                    if (!onFire && !isMoving)
                        FrameMoves();
                   // bResetTimer = true;
                }
            }

            if ((keyState.IsKeyDown(keyDown) ||
                pad.ThumbSticks.Left.Y < 0))
            {
                if (BoundingBox.Bottom < MurderBallGame.rCourt.Bottom - 3)
                {
                    Y += MoveRate;
                    if (!onFire && !isMoving)
                        FrameMoves();
                    //bResetTimer = true;
                }
            }


            if ((keyState.IsKeyDown(keyLeft) ||
                pad.ThumbSticks.Left.X < 0))
            {
                if (BoundingBox.Left > GetLeftBound())
                {
                    X -= MoveRate;
                    if (!onFire && !isMoving)
                        FrameMoves();
                    
                    //bResetTimer = true;
                }
            }
            if ((keyState.IsKeyDown(keyRight) ||
                pad.ThumbSticks.Left.X > 0))
            {
                if (BoundingBox.Right < GetRightBound())
                {
                    X += MoveRate;
                    if (!onFire && !isMoving)
                        FrameMoves();
                   // bResetTimer = true;
                }
            }
            prevKeyState = keyState;
        }

        public void StateClear()
        {
            standing = false;
            isMoving = false;
            //bHasBall = false;
            isThrowing = false;
            isRolling = false;
            isHit = false;
            //isDoneFor = false;
            zapped = false;
            onFire = false;
            //isDead = false;
            //exploded = false;
            isDucking = false;
        }

        private void FrameOnFire()
        {
            StateClear();
            onFire = true;
            spriteStanding.iFrameOffsetY = 157;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 3;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.Repeats = true;
            spriteStanding.FrameLength = (float)1 / 12.0f;

        }

        private void FrameThrowing()
        {
            StateClear();
            isThrowing = true;
            spriteStanding.iFrameOffsetY = 104;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 0;
            spriteStanding.FrameLength = (float)1 / 12.0f;
            spriteStanding.Repeats = false;

        }

        private void FrameThrown()
        {
            StateClear();
            isThrowing = true;
            spriteStanding.iFrameOffsetY = 104;
            spriteStanding.iFrameOffsetX = 52;
            spriteStanding.FrameCount = 0;
            spriteStanding.FrameLength = (float)1 / 12.0f;
            spriteStanding.Repeats = false;

        }

        private void FrameZapped()
        {
            StateClear();
            zapped = true;
            spriteStanding.iFrameOffsetY = 157;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 3;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.Repeats = true;
            spriteStanding.FrameLength = (float)1 / 12.0f;

        }

        private void FrameDucking()
        {
            StateClear();
            timeRolled = 0.0f;
            isRolling = true;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.iFrameOffsetY = 312;
            spriteStanding.FrameCount = 3;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.Repeats = false;
            spriteStanding.FrameLength = (float)1 / 12.0f;
        }

        private void FrameMoves()
        {
            StateClear();
            Moving = true;
            spriteStanding.iFrameOffsetY = 0;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 3;
            spriteStanding.Repeats = true;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.FrameLength = (float)1 / 10.0f;

        }

        private void FrameStanding()
        {
            StateClear();
            standing = true;
            spriteStanding.Repeats = true;
            spriteStanding.iFrameOffsetY = 52;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 3;
            
            spriteStanding.CurrentFrame = 0;
            spriteStanding.FrameLength = (float)1 / 2.0f;
        }

        private void FrameHit(Boolean rev)
        {
            StateClear();
            isHit = true;
            spriteStanding.iFrameOffsetY = 208;
            if (rev)
                spriteStanding.iFrameOffsetX = 104;
            else
                spriteStanding.iFrameOffsetX = 0;
            spriteStanding.FrameCount = 1;
            spriteStanding.Repeats = false;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.FrameLength = (float)1 / 12.0f;
        }

        private void FrameRolling()
        {
            StateClear();
            isRolling = true;
            spriteStanding.Repeats = true;
            spriteStanding.iFrameOffsetX = 0;
            spriteStanding.iFrameOffsetY = 260;
            spriteStanding.FrameCount = 2;
            timeRolled = 0.0f;
            spriteStanding.CurrentFrame = 0;
            spriteStanding.FrameLength = (float)1 / 4.0f;
        }

        
        
        public int GetLeftBound()
        {
            int LeftBound = MurderBallGame.rCourt.Left;
            

            if (PlayerNum == 2)
            {
                LeftBound = MurderBallGame.iPlayAreaHalf;

            }

            return LeftBound;
        }

        public int GetRightBound()
        {
            int RightBound = MurderBallGame.rCourt.Right;
            if (PlayerNum == 1)
                RightBound = MurderBallGame.iPlayAreaHalf;

            return RightBound;
        }
        
        /// <summary>
        /// Handles pressing the fire key.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="pad"></param>
        protected void KeyFireHandler(KeyboardState keys, GamePadState pad)
        {
            // If player already has a ball:
            if (HasBall)
            {
                if (keyState.IsKeyDown(keyFire))
                {
                    isThrowing = true;
                    FrameThrowing();
                    // Power up!
                    if (curPower < fMaxPower) 
                    {
                        curPower += fPowerUpRate;
                        if (curPower >= fMaxPower)
                        {
                            curBall.isPowered = true;
                            curPower = fMaxPower;
                        }
                    }
                    
                        
                    if (!prevKeyState.IsKeyDown(keyFire))
                    {
                        // Player has just pressed fire, but not released.
                        // Power up
                        
                        
                        isThrowing = true;
                        isMoving = false;
                            
                    }

                }
                else if (prevKeyState.IsKeyDown(keyFire))
                {
                    // Player just released fire key
                    // throw ball!
                    FrameThrown();
                    FireBall();
                }

            }
            else // Doesn't have a ball yet:
            {
                if (keyState.IsKeyDown(keyFire))
                {
                    if (!prevKeyState.IsKeyDown(keyFire))
                    {
                        // Player has pressed fire key, but not released
                        

                    }
                }
                else if (prevKeyState.IsKeyDown(keyFire))
                {
                    // Player just pressed and released fire key
                    // Get ball, if available.
                    Rectangle GrabBox = new Rectangle(BoundingBox.X, 
                        BoundingBox.Y + (iframeHeight), 
                        BoundingBox.Width, 
                        BoundingBox.Height /2);

                    foreach (Ball ball in parent.listBalls)
                    {
                        if (ball.Fired)
                            continue;
                        if (GrabBox.Intersects(ball.BoundingBox))
                        {
                            GrabBall(ball);
                            break;
                        }
                    } // end for

                }

            }

        } // End of fireHandler


        /// <summary>
        /// Returns the name of the player. Ie: "Player 2".
        /// </summary>
        /// <returns></returns>
        public String ToString() {
            String ret = "Player " + PlayerNum.ToString();
            return ret;
        }

    } // End of class Player
}
