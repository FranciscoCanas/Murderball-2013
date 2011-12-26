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


        int iX = 604;
        int iY = 260;
        int iFacing = 0;
        int iMoveRate = 5; // Player's speed
        int iScrollRate = 0;
        //int iCurrentFrame = 0;
        Boolean isMoving = false;
        Boolean bHasBall = false;
        Ball curBall = null;
        Player opponent;

        int hitPoints = 5;
        
        float fSpeedChangeCount = 0.0f;
        float fSpeedChangeDelay = 0.1f;
        float fVerticalChangeCount = 0.0f;
        float fVerticalChangeDelay = 0.01f;
        int iPlayer; // Player 1 or Player 2

        // Keys:
        Keys kUp = Keys.Up;
        Keys kDown = Keys.Down;
        Keys kLeft = Keys.Left;
        Keys kRight = Keys.Right;
        Keys kFire = Keys.L;
        Keys kRoll = Keys.K;
        
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
                spriteRolling = new AnimatedSprite(
                  parent.Content.Load<Texture2D>(@"roll1"),
                   0,
                   0,
                   iframeWidth,
                   iframeHeight,
                   2,
                   new Vector2(xScale, yScale),
                   new Vector2(xOrigin, yOrigin));
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
                /*
                spriteRolling = new AnimatedSprite(
                    parent.Content.Load<Texture2D>(@"roll2"),
                    0,
                    0,
                    iframeWidth,
                    iframeHeight,
                    2,
                    new Vector2(xScale, yScale),
                    new Vector2(xOrigin, yOrigin));
                */
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
            spriteStanding.FrameLength = (float)1 / 8.0f;
            iPlayer = player;
            this.parent = parent;
            
            
        }

        public void FireBall()
        {
            if (curBall != null)
            {
                curBall.Fire(this, opponent);
                curBall = null;
                
                bHasBall = false;
            }
        }

        public void GrabBall(Ball ball)
        {
            curBall = ball;
            bHasBall = true;
            ball.player = this;
        }

        public void HitByBall(Ball ball)
        {
            // Do this when player gets hit by ball
            hitPoints--;

            if (hitPoints < 1)
                Dies();
        }

        public void Dies()
        {
            // Awesomeness here
        }

        public void Update(GameTime gametime)
        {

            if (isMoving)
            {
                spriteStanding.OffsetX = iframeWidth;
                spriteStanding.IsAnimating = true;
                spriteStanding.Update(gametime);
            }
            else
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

        public int Facing
        {
            get { return iFacing; }
            set { iFacing = value; }
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

     
        public float SpeedChangeCount
        {
            get { return fSpeedChangeCount; }
            set { fSpeedChangeCount = value; }
        }

        public float SpeedChangeDelay
        {
            get { return fSpeedChangeDelay; }
            set { fSpeedChangeDelay = value; }
        }

        public float VerticalChangeCount
        {
            get { return fVerticalChangeCount; }
            set { fVerticalChangeCount = value; }
        }

        public float VerticalChangeDelay
        {
            get { return fVerticalChangeDelay; }
            set { fVerticalChangeDelay = value; }
        }

        public Boolean HasBall
        {
            get { return bHasBall; }
        }

        public int HitPoints
        {
            get { return hitPoints; }
            set { hitPoints = value; }
        }

        public Boolean Moving
        {
            set { isMoving = value; }
        }

        public int PlayerNum
        {
            get { return iPlayer; }
        }

    }
}
