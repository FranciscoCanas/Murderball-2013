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
        float curPower = fMaxPower / 5.0f;
        private const int iMaxHitpoints = 100;
        int curHitPoints;
        int iMoveRate = 5; // Player's speed

        // Player states used to keep track of what to animate:
        Boolean isMoving = false; 
        Boolean bHasBall = false;
        Boolean isThrowing = false;
        Boolean isRolling = false;
        Boolean isHit = false;
        Boolean isDead = false;

        // Various times to keep track of player state:
        int timerHit = 0; // Used when player gets hit.
        private const int timerRecover = 5; // Time it takes for player to get up.

        // Coordinates to keep track of player on screen.
        int iX = 604;
        int iY = 260;
        
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
                iX = 240;
                iY = 260;
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

                iX = 604;
                iY = 260;
                kUp = Keys.W;
                kDown = Keys.S;
                kLeft = Keys.A;
                kRight = Keys.D;
                kFire = Keys.G;
                kRoll = Keys.F;
            }

            spriteStanding.IsAnimating = false;
            spriteStanding.FrameLength = (float)1 / 8.0f; // FPS for standing frame.
            iPlayer = player;
            this.parent = parent;
            keyState = new KeyboardState();
            prevKeyState = new KeyboardState();

            // Set up stats:
            curHitPoints = iMaxHitpoints;
            
            
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
            timerHit = 0;
            isHit = true;
            if (curHitPoints <= 0)
                Dies();
        }

        /// <summary>
        /// Used upon player death.
        /// Shoot lot's of blood or something.
        /// </summary>
        public void Dies()
        {
            isDead = true;

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
            if (isDead)
            {
                // Stuff here when guy dead.
            }
            else if (isMoving) // Player is moving around.
            {
                spriteStanding.OffsetX = iframeWidth;
                spriteStanding.IsAnimating = true;
                spriteStanding.Update(gametime);
            }
            else if (isRolling) // Player is rolling around.
            {

            }
            else if (isThrowing) // Player is throwing a ball.
            {
            }
            else if (isHit) // Player just got hit.
            {
                timerHit += 1;
                if (timerHit > timerRecover)
                {
                    isHit = false;
                }
            }
            else // Just standing there.
            {
                spriteStanding.Frame = 0;
                spriteStanding.OffsetX = 0;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            spriteStanding.Draw(sb, iX, iY, false, Color.White);
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
            get { return curHitPoints; }
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

        /// <summary>
        /// Checks player input. Specifically, keyboard input.
        /// GamePad input to be done up at a future time.
        /// </summary>
        public void KeyInputHandler(KeyboardState keys, GamePadState pad)
        {
            int LeftBound, RightBound;
            keyState = keys;

            if (isDead)
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
                if (BoundingBox.Top > MurderBallGame.rCourt.Top)
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
                        curPower += 0.5f;
                        
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
                    foreach (Ball ball in parent.listBalls)
                    {
                        if (ball.Fired)
                            continue;
                        if (BoundingBox.Intersects(ball.BoundingBox))
                        {
                            GrabBall(ball);
                            break;
                        }
                    } // end for

                }

            }

        } // End of fireHandler


    } // End of class Player
}
