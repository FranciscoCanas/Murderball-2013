using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


namespace MurderBall
{
    public class Ball
    {
        MurderBallGame parent;
        AnimatedSprite asBall;
        Texture2D shadow;
        int iX; // Ball's location
        int iY;
        int iZ; // Ball's height.
        int shadowOffset = (int)((yScale)*(ySize / 3));
        public float fXSpeed; // Linear velocities
        public float fYSpeed;
        public float fZSpeed; 

        
        const int xSize = 32;
        const int ySize = 32;
        Vector2 vCentre;
        bool bActive;
        int iFacing = 0;
        float fElapsed = 0f;
        float fUpdateInterval = 0.015f;

        // Ball physics properties
        public float fSpeed = 15.0f;
        public float fSpeedMultiple = 1.0f;
        public float fSpeedDecayRate = 0.9999f;
        public float fSpeedBounceMult = 0.99f;


        bool isFired;
        int iBounceCount = 0;
        float fLastBounce = 0;
        float fLastBounceInterval = 0.15f;
        Player curPlayer = null;
        Player target = null;
        Player thrower = null;
        
        Texture2D particlebase;
        List<Color> lColors;
        Color curColor = Color.White;
        int colorCount = 0;
        const float xScale = 0.66f;
        const float yScale = 0.66f;
        const int xOrigin = (int)(0.5 * xSize);
        const int yOrigin = (int)(0.5 * ySize);

        SoundBank ballSounds;
        WaveBank ballWaves;
        
        Cue cuePlayerHit;
        
        

        public Ball(Texture2D texture, MurderBallGame parent)
        {
            asBall = new AnimatedSprite(texture, 
                0, 0, 
                xSize, 
                ySize, 
                1, 
                new Vector2(xScale, yScale),
                new Vector2(xOrigin, yOrigin));

            this.parent = parent;

            asBall.IsAnimating = true;
            
            bActive = true;
            isFired = false;
            vCentre = new Vector2(iX + (xSize /2), iY + (ySize / 2));
            particlebase = MurderBallGame.particlebase;

           
            lColors = new List<Color>(6);
            lColors.Add(Color.Red);
            lColors.Add(Color.Orange);
            lColors.Add(Color.Yellow);
            lColors.Add(Color.Aquamarine);
            lColors.Add(Color.Cyan);
            lColors.Add(Color.Gold);

            ballWaves = new WaveBank(parent.Audio, "Content\\Ball Sounds.xwb");
            ballSounds = new SoundBank(parent.Audio, "Content\\Ball Cues.xsb");
            shadow = parent.Content.Load<Texture2D>(@"ballshadow");

            

        }
 
        public Ball()
        {
            iFacing = 0;
            iX = 0;
            iY = 0;
            bActive = false;
        }

        public void Fire(Player inThrower, Player inTarget)
        {
            float deltaX;
            float deltaY;
            float theta;
            target = inTarget;
            thrower = inThrower;
            
            iBounceCount=0;
            iX = X;
            iY = Y;
            int oY = target.Y;
            int oX = target.X;

            
            // Some trig shit do get our aim right:
            deltaX = oX - X;
            deltaY = oY - Y;
            theta = (float)Math.Atan2((double)deltaY , (double)deltaX);
            fYSpeed = (float)((float)thrower.Power * Math.Sin(theta));
            fXSpeed = (float)((float)thrower.Power * Math.Cos(theta));
            fZSpeed = 0; // 0 at first, then gravity kicks in.
            iZ = 24;
           
            // Fix the facing on the angles:
            /*
            if ((oX - X) < 0)
                fXSpeed *= -1.0f;
            if ((oY - Y) < 0)
               fYSpeed *= -1.0f;
            */
            //iFacing = Facing;
            curPlayer = null;
            isFired = true;
        }

        public void Update(GameTime gameTime, Player player1, Player player2)
        {
            fElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            fLastBounce += fElapsed;
            
            // Update particles:
            if (fLastBounce > fLastBounceInterval)
            {
               // Bouncy here
            }

            if (curPlayer != null)
            {
                iX = curPlayer.X + 25;
                iY = curPlayer.Y + 25;
            } else if (isFired)
            {
                
               
                

                if (fElapsed > fUpdateInterval)
                {
                    colorCount++;
                    // Change colors:
                    curColor = lColors[colorCount % lColors.Count];
                    fElapsed = 0f;

                    iX += (int)fXSpeed;
                    iY += (int)fYSpeed;
                    iZ -= (int)fZSpeed;
                    if (iZ <= 0)
                        iZ = 0;

                    // Check for player hits:
                    if (iBounceCount > 0)
                        CheckHit(thrower);

                    CheckHit(target);

                    // If ball hits edge of play area, bounce that shit.
                    
                    if (BoundingBox.Bottom > MurderBallGame.rCourt.Bottom)
                    {
                        fYSpeed *= -1 * fSpeedBounceMult;
                        
                        
                        HitWall();
                    }
                    
                    if (BoundingBox.Top < MurderBallGame.rCourt.Top)
                    {
                        fYSpeed *= -1 * fSpeedBounceMult;
                        
                        
                        HitWall();
                    }
                    
                    if (BoundingBox.Left < MurderBallGame.rCourt.Left)
                    {
                        fXSpeed *= -1 * fSpeedBounceMult;
                        
                        
                        HitWall();
                    }
                    
                    if (BoundingBox.Right > MurderBallGame.rCourt.Right)
                    {
                        fXSpeed *= -1 * fSpeedBounceMult;
                        
                        
                        HitWall();
                    }

                    // If we hit the ground:
                    if (iZ <= 0)
                    {
                        fZSpeed *= -1;
                    }

                    // Slow the balls down.
                    fSpeedMultiple *= fSpeedDecayRate;
                    fXSpeed *= fSpeedMultiple;
                    fYSpeed *= fSpeedMultiple;
                    fZSpeed += MurderBallGame.grav;

                    // Rotate:
                    
                    //asBall.Rotation += 0.1f * Power;
                    asBall.Rotation += 0.5f * Power;
                    float circle = MathHelper.Pi * 2;
                    asBall.Rotation = asBall.Rotation % circle;

   

                    

                    
                    // If ball stops, set it so it can be picked up.
                    if ((Math.Abs(fXSpeed) < 1) && (Math.Abs(fYSpeed) < 1))
                        BallStops();

                    vCentre = new Vector2(iX + (xSize / 2), iY + (ySize / 2));
                }
            }
        }

        /// <summary>
        ///  Reset values to defaults;
        /// </summary>
        public void BallStops()
        {
            isFired = false;
            iBounceCount = 0;
            target = null;
            curPlayer = null;
            thrower = null;
            fSpeedMultiple = 1.0f;
            fZSpeed = 0.0f;
            iZ = 0;
            
            curColor = Color.White;
        }

        public void CheckHit(Player player)
        {
            if (BoundingBox.Intersects(player.BoundingBox))
            {
                HitPlayer(player);
                fSpeedMultiple *= 0.85f;
                
            }

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(shadow, new Vector2(iX,iY), null, Color.White,
               0.0f, new Vector2(xOrigin,yOrigin), 
               new Vector2(xScale,yScale), SpriteEffects.None, 1);
           
            asBall.Draw(sb, iX, iY - iZ - shadowOffset, false, curColor);
                  
        }

        public void HitWall()
        {
            Cue cueWallHit = ballSounds.GetCue("hitWall");
            cueWallHit.Play();
            iBounceCount += 1;
            fLastBounce = 0;
        }

        public void HitPlayer(Player player)
        {
            // Do this when ball hits player
            //isFired = false;
            //fSpeedDecay *= (fSpeedDecayRate * 0.6f);
            
            
            Cue cuePlayerHit = ballSounds.GetCue("hitPlayer");
            cuePlayerHit.Play();
            player.HitByBall(this);
            this.fXSpeed *= -1;
            this.fYSpeed *= -1;
            
            fLastBounce = 0;
            
             
            
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

        public bool IsActive
        {
            get { return bActive; }
            set { bActive = value; }
        }

        public int Facing
        {
            get { return iFacing; }
            set { iFacing = value; }
        }

        public float Speed
        {
            get { return fSpeed; }
            set { fSpeed = value; }
        }

        public Player player
        {
            get { return curPlayer; }
            set { curPlayer = value; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(iX, iY, xSize, ySize); }
        }
        public Boolean Fired
        {
            get { return isFired; }
        }

        public Vector2 Centre
        {
            get { return vCentre; }
        }

        /// <summary>
        /// Returns amount of power for ball hitting player.
        /// </summary>
        public float Power
        {
            get { return Math.Max(Math.Abs((0.5f)*fXSpeed * fYSpeed), 1); }
        }

    }
}
