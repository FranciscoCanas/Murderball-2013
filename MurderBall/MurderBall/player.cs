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
        AnimatedSprite spriteStanding;
        AnimatedSprite spriteRolling;
        ParticleEngine bloodSpray;
        ParticleEngine smoke;
        ParticleEngine burning;
        ParticleEngine electric;

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
        private const int iMaxHitpoints = 100;
        float curHitPoints;
        int iMoveRate = 5; // Player's speed

        // Player states used to keep track of what to animate:
        public Boolean isMoving = false; 
        public Boolean bHasBall = false;
        public Boolean isThrowing = false;
        public Boolean isRolling = false;
        public Boolean isHit = false;
        public Boolean isDead = false;
        public Boolean zapped = false;
        public Boolean onFire = false;

        // Various times to keep track of player state:
        private float timeHit = 0; // Used when player gets hit.
        private float timerFire = 0f; // Used when player gets set on fire. Yes this can happen. This ain't your grandmammy's dodgeball.
        private const float timerRecover = 2.0f; // Time it takes for player to get up.
        float elapsedtime = 0.0f;
        private const float timerRoll = 0.5f; // Time it takes for player to recover after rolling.
        private float timeRolled;
        private float timerZapped;

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
        public Player(Texture2D texture, int player, MurderBallGame parent)
        {
            if (player == 1)
            {
                spriteStanding = new AnimatedSprite(texture, 
                    0, 
                    0, 
                    iframeWidth,
                    iframeHeight, 
                    3, 
                    new Vector2(xScale,yScale),
                    new Vector2(xOrigin,yOrigin));
                /*
              
                */
                iX = 200;
                iY = 260;
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
                    0,
                    iframeWidth,
                    iframeHeight, 
                    3,
                    new Vector2(xScale,yScale),
                    new Vector2(xOrigin,yOrigin));

                iX = 600;
                iY = 260;
                
            }

            spriteStanding.IsAnimating = false;
            spriteStanding.FrameLength = (float)1 / 8.0f; // FPS for standing frame.
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
            curHitPoints -= (int)ball.Power;
            timeHit = 0;
            isHit = true;
            isMoving = false;
            isRolling = false;

            fZSpeed = -10.0f;
            fXSpeed = ball.fXSpeed;
            fYSpeed = ball.fYSpeed;

            if (fXSpeed > 0)
            {
                if (fYSpeed > 0)
                {
                    bloodSpray.velocityMax = new Vector2(fXSpeed,fYSpeed);
                    bloodSpray.velocityMin = new Vector2(0, 0);

                }
                else
                {
                    bloodSpray.velocityMax = new Vector2(fXSpeed, 0);
                    bloodSpray.velocityMin = new Vector2(0, fYSpeed);
                }
            } else 
                {
                    if (fYSpeed > 0)
                    {
                        bloodSpray.velocityMax = new Vector2(0, fYSpeed);
                        bloodSpray.velocityMin = new Vector2(fXSpeed, 0);
                    }
                    else
                    {
                        bloodSpray.velocityMax = new Vector2(0, 0);
                        bloodSpray.velocityMin = new Vector2(fXSpeed, fYSpeed);
                    }

                }


            bloodSpray.emitterLocation = new Vector2(X + iframeWidth/ 2 ,Y + iframeHeight / 2);
            //bloodSpray.sourceRect = new Rectangle(X, Y, iframeWidth / 2, iframeHeight / 2);
            bloodSpray.start();
            if (curHitPoints <= 0)
                Dies();
        }

        public void SetOnFire()
        {
            onFire = true;
            timerFire = 0f;
            if (!smoke.isActive)
                smoke.start();
            if (!burning.isActive)
                burning.start();
        }

        public void Zap()
        {
            zapped = true;
            timerZapped = 0f;
            if (!electric.isActive)
                electric.start();
            

        }

        /// <summary>
        /// Used upon player death.
        /// Shoot lot's of blood or something.
        /// </summary>
        public void Dies()
        {
            isDead = true;
            parent.courtBot.declareWinner(this.opponent);
            // Awesomeness here
        }

        /// <summary>
        /// Player's update method gets run every frame:
        /// Checks the player's state, and calls the appropriate
        /// method to handle changes.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            elapsedtime += (float)gametime.ElapsedGameTime.TotalSeconds;

            Rectangle plocation = BoundingBox;
            if (isDead)
            {
                spriteStanding.OffsetY = 0;
                spriteStanding.OffsetX = 0;
                // Stuff here when guy dead.
            }
            else if (zapped)
            {
                timerZapped += (float)gametime.ElapsedGameTime.TotalSeconds;
                curHitPoints -= 0.1f;
                electric.sourceRect = new Rectangle(plocation.X + 16, plocation.Y + 16, 16,16);
                // No movements allowed.
                if (timerZapped > 2.0f)
                {
                    zapped = false;
                    electric.isActive = false;
                }

            }
            else if (onFire)
            {

                timerFire += (float)gametime.ElapsedGameTime.TotalSeconds;
                curHitPoints -= (float)gametime.ElapsedGameTime.TotalSeconds * 2;
                
                smoke.sourceRect = plocation;
                burning.sourceRect = plocation;


                if (timerFire > 3.0)
                {
                    onFire = false;
                    smoke.isActive = false;
                    burning.isActive = false;
                }

            }
            else if (isRolling) // Player is rolling around.
            {
                spriteStanding.OffsetY = 0;
                spriteStanding.OffsetX = 0;
                onFire = false;
                timeRolled += elapsedtime;
                if (timeRolled > timerRoll)
                {
                    isRolling = false;

                }
            }
            else if (isThrowing) // Player is throwing a ball.
            {
                spriteStanding.OffsetY = 0;
                spriteStanding.OffsetX = 0;
            }
            else if (isHit) // Player just got hit.
            {
                spriteStanding.OffsetY = 0;
                spriteStanding.OffsetX = 0;

                iX += (int)fXSpeed;
                iY += (int)fYSpeed;
                iZ += (int)fZSpeed;
                fZSpeed = Math.Min(fZSpeed + MurderBallGame.grav, 0);

                timeHit += elapsedtime;
                if (timeHit > timerRecover)
                {
                    isHit = false;
                    iZ = 0;
                }
            }
            else if (isMoving) // Player is moving around.
            {

                spriteStanding.OffsetY = 0;
                spriteStanding.OffsetX = iframeWidth;
                spriteStanding.IsAnimating = true;
                spriteStanding.Update(gametime);
            }
            else // Just standing there.
            {
                spriteStanding.OffsetY = 0;
                spriteStanding.Frame = 0;
                spriteStanding.OffsetX = 0;
            }

            if (bloodSpray.isActive)
            {
                bloodSpray.sourceRect = new Rectangle(X, Y, iframeWidth / 2, iframeHeight / 2);
                bloodSpray.Update(gametime);
            }

            smoke.Update(gametime);
            burning.Update(gametime);
            electric.Update(gametime);
        }

        public void Draw(SpriteBatch sb)
        {
            spriteStanding.Draw(sb, iX, iY - iZ, false, Color.White, GetDepth);
           
            //if (smoke.isActive)
           // {
           // sb.End();
            
           // sb.Begin();
            //}
        }

        public void DrawParticles(SpriteBatch sb)
        {
            if (bloodSpray.isActive)
            {
                
                bloodSpray.Draw(sb);
                
            }
            smoke.Draw(sb);
            burning.Draw(sb);
            electric.Draw(sb);

        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(iX, iY, iframeWidth, iframeHeight); }
        }

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

        public int HitPoints
        {
            get { return (int)curHitPoints; }
            set { curHitPoints = value; }
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

            if (isDead || isHit || isRolling || zapped )
            {
                return;
            }

            
            KeyFireHandler(keyState, pad);

            if (isThrowing)
            {
                prevKeyState = keyState;
                return;
            }

            if (keyState.IsKeyDown(keyRoll))
            {
                // Roll call here bitch.
                isRolling = true;
                timeRolled = 0.0f;
                
            }

            if (keyState.IsKeyUp(keyDown) &&
                keyState.IsKeyUp(keyUp) &&
                keyState.IsKeyUp(keyLeft) &&
                keyState.IsKeyUp(keyRight))
            {
                Moving = false;
                prevKeyState = keyState;
                return;
            }

            if ((keyState.IsKeyDown(keyUp) ||
                pad.ThumbSticks.Left.Y > 0))
            {
                if (BoundingBox.Bottom > MurderBallGame.rCourt.Top)
                {
                    Y -= MoveRate;
                    Moving = true;
                   // bResetTimer = true;
                }
            }

            if ((keyState.IsKeyDown(keyDown) ||
                pad.ThumbSticks.Left.Y < 0))
            {
                if (BoundingBox.Bottom < MurderBallGame.rCourt.Bottom)
                {
                    Y += MoveRate;
                    Moving = true;
                    //bResetTimer = true;
                }
            }

            LeftBound = MurderBallGame.rCourt.Left;
            RightBound = MurderBallGame.rCourt.Right;

            if (PlayerNum == 1)
            {

                RightBound = MurderBallGame.iPlayAreaHalf;
            }
            else if (PlayerNum == 2)
            {
                LeftBound = MurderBallGame.iPlayAreaHalf;

            }

            if ((keyState.IsKeyDown(keyLeft) ||
                pad.ThumbSticks.Left.X < 0))
            {
                if (BoundingBox.Left > LeftBound)
                {
                    X -= MoveRate;
                    Moving = true;
                    //bResetTimer = true;
                }
            }
            if ((keyState.IsKeyDown(keyRight) ||
                pad.ThumbSticks.Left.X > 0))
            {
                if (BoundingBox.Right < RightBound)
                {
                    X += MoveRate;
                    Moving = true;
                   // bResetTimer = true;
                }
            }
            prevKeyState = keyState;
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
                            
                    }

                }
                else if (prevKeyState.IsKeyDown(keyFire))
                {
                    // Player just released fire key
                    // throw ball!
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
