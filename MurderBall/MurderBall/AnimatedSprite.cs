using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MurderBall
{
    class AnimatedSprite
    {
        Texture2D t2dTexture;

        float fFrameRate = (float)1 / 12.0f;
        float fElapsed = 0.0f;

        public int iFrameOffsetX = 0;
        public int iFrameOffsetY = 0;
        int iFrameWidth = 32;
        int iFrameHeight = 32;

        int iFrameCount = 1;
        int iCurrentFrame = 0;
        int iScreenX = 0;
        int iScreenY = 0;

        public float Rotation = 0.0f;
        Vector2 fOrigin;
        Vector2 fScale;
        float Depth = 0.0f;

        bool bAnimating = true;

        public int X
        {
            get { return iScreenX; }
            set { iScreenX = value; }
        }

        public int Y
        {
            get { return iScreenY; }
            set { iScreenY = value; }
        }

        public int Frame
        {
            get { return iCurrentFrame; }
            set { iCurrentFrame = (int)MathHelper.Clamp(value, 0, iFrameCount); }
        }

        public float FrameLength
        {
            get { return fFrameRate; }
            set { fFrameRate = (float)Math.Max(value, 0f); }
        }

        public bool IsAnimating
        {
            get { return bAnimating; }
            set { bAnimating = value; }
        }

        public int OffsetX
        {
            set { iFrameOffsetX = value; }
        }
        public int OffsetY
        {
            set { iFrameOffsetY = value; }
        }

        public AnimatedSprite(
          Texture2D texture,
          int FrameOffsetX,
          int FrameOffsetY,
          int FrameWidth,
          int FrameHeight,
          int FrameCount,
            Vector2 Scale,
            Vector2 Origin)
        {
            t2dTexture = texture;
            iFrameOffsetX = FrameOffsetX;
            iFrameOffsetY = FrameOffsetY;
            iFrameWidth = FrameWidth;
            iFrameHeight = FrameHeight;
            iFrameCount = FrameCount;
            fScale = Scale;
            fOrigin = Origin;

        } // End of Animatedsprite

        public Rectangle GetSourceRect()
        {
            return new Rectangle(
            iFrameOffsetX + (iFrameWidth * iCurrentFrame),
            iFrameOffsetY,
            iFrameWidth,
            iFrameHeight);
        } // End of GetSourceRect()

        public void Update(GameTime gametime)
        {
            if (bAnimating)
            {
                // Accumulate elapsed time...
                fElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

                // Until it passes our frame length
                if (fElapsed > fFrameRate)
                {
                    // Increment the current frame, wrapping back to 0 at iFrameCount
                    iCurrentFrame = ((iCurrentFrame + 1) % iFrameCount);

                    // Reset the elapsed frame time.
                    fElapsed = 0.0f;
                }
            }
        }// End of Update

        public void Draw(
          SpriteBatch spriteBatch,
          int XOffset,
          int YOffset,
          bool NeedBeginEnd, 
          Color col, float depth)
        {
            if (NeedBeginEnd)
                spriteBatch.Begin();
            
            spriteBatch.Draw(t2dTexture, new Vector2(iScreenX + XOffset,
                  iScreenY + YOffset), GetSourceRect(), col,
               Rotation, fOrigin, fScale, SpriteEffects.None, depth);
            
            if (NeedBeginEnd)
                spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset, Color col, float depth)
        {
            Draw(spriteBatch, XOffset, YOffset, true, col, depth);
        }

    }// End of AnimatedSprite Class

} // End of NameSpace
