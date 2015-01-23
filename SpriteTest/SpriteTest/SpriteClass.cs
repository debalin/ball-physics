using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace SpriteTest
{
    class SpriteClass
    {
        private Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 screensize { get; set; }
        public float kinetic, angkinetic, inertia, friction, mass, accelerationX = 0f, accelerationY = 9.81f; 
        public double rotation = 0d, omega = 0.05d;

        public SpriteClass(Texture2D newtexture, Vector2 newposition, Vector2 newsize, Vector2 newvelocity, Vector2 newscreensize, float newmass)
        {
            texture = newtexture;
            position = newposition;
            size = newsize;
            velocity = newvelocity;
            screensize = newscreensize;
            mass = newmass;
            friction = 0.15f * mass * 9.81f;
        }

        public void Move(double time, SpriteClass otherSprite, SoundBank soundBank)
        {
            float tempvelx, tempvely ;
            if (velocity.Y != 0f)
                accelerationY = 9.81f;
            inertia = (mass * (size.X * size.X) / 8);
            velocity = new Vector2(velocity.X, (float)(velocity.Y + (int)accelerationY*time));
            kinetic = (float)0.5 * mass * (velocity.X * velocity.X + velocity.Y * velocity.Y);
            angkinetic = 0.5f * inertia * (float)(omega * omega);
            if (position.X < 0) //if the ball has crossed the screen boundary, bring it back
                position = new Vector2(0, position.Y);
            else if ((position.X + size.X) > screensize.X)
                position = new Vector2(screensize.X - size.X, position.Y);
            if (position.Y < 0)
                position = new Vector2(position.X, 0);
            else if ((position.Y + size.Y) > screensize.Y)
                position = new Vector2(position.X, screensize.Y - size.Y);
            if (((position.X + size.X + velocity.X) > screensize.X)) //Loss of Kinetic Energy due to impact on side walls
            {
                angkinetic = angkinetic - (angkinetic / 3);
                kinetic = kinetic - (kinetic / 5);
                try
                {
                    tempvelx = -Math.Sign(velocity.X) * (float)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.Y / velocity.X) * (velocity.Y / velocity.X)))));
                }
                catch
                {
                    tempvelx = 0f;
                }
                try
                {
                    tempvely = Math.Sign(velocity.Y) * (int)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.X / velocity.Y) * (velocity.X / velocity.Y)))));
                }
                catch
                {
                    tempvely = 0f;
                }
                velocity = new Vector2(tempvelx, tempvely);
                try
                {
                    omega = Math.Sign(omega) * (float)Math.Sqrt(2 * angkinetic / inertia);
                }
                catch
                {
                    omega = 0d;
                }
                omega = omega + 0.01d * velocity.Y; //spin effect on wall collision
                soundBank.PlayCue("ballground");
            }
            else if  ((position.X + velocity.X) < 0f)
            {
                angkinetic = angkinetic - (angkinetic / 3);
                kinetic = kinetic - (kinetic / 5);
                try
                {
                    tempvelx = -Math.Sign(velocity.X) * (float)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.Y / velocity.X) * (velocity.Y / velocity.X)))));
                }
                catch
                {
                    tempvelx = 0f;
                }
                try
                {
                    tempvely = Math.Sign(velocity.Y) * (int)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.X / velocity.Y) * (velocity.X / velocity.Y)))));
                }
                catch
                {
                    tempvely = 0f;
                }
                velocity = new Vector2(tempvelx, tempvely);
                try
                {
                    omega = Math.Sign(omega) * (float)Math.Sqrt(2 * angkinetic / inertia);
                }
                catch
                {
                    omega = 0d;
                }
                omega = omega - 0.01d * velocity.Y; //spin effect on wall collision
                soundBank.PlayCue("ballground");
            }
            if (((position.Y + size.Y + velocity.Y) > screensize.Y)) //Loss of Kinetic Energy due to impact on ground and top wall
            {
                angkinetic = angkinetic - (angkinetic / 3);
                kinetic = kinetic - (kinetic / 5);
                try
                {
                    tempvelx = Math.Sign(velocity.X) * (float)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.Y / velocity.X) * (velocity.Y / velocity.X)))));
                }
                catch
                {
                    tempvelx = 0f;
                }
                try
                {
                    tempvely = -Math.Sign(velocity.Y) * (int)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.X / velocity.Y) * (velocity.X / velocity.Y)))));
                }
                catch
                {
                    tempvely = 0f;
                }
                velocity = new Vector2(tempvelx, tempvely);
                try
                {
                    omega = Math.Sign(omega) * (float)Math.Sqrt(2 * angkinetic / inertia);
                }
                catch
                {
                    omega = 0d;
                }
                omega = omega - 0.01d * velocity.X; //spin effect on wall collision
                soundBank.PlayCue("ballground");
            }
            else if ((position.Y + velocity.Y) < 0f)
            {
                angkinetic = angkinetic - (angkinetic / 3);
                kinetic = kinetic - (kinetic / 5);
                try
                {
                    tempvelx = Math.Sign(velocity.X) * (float)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.Y / velocity.X) * (velocity.Y / velocity.X)))));
                }
                catch
                {
                    tempvelx = 0f;
                }
                try
                {
                    tempvely = -Math.Sign(velocity.Y) * (int)Math.Sqrt((2 * kinetic / (mass * (1 + (velocity.X / velocity.Y) * (velocity.X / velocity.Y)))));
                }
                catch
                {
                    tempvely = 0f;
                }
                velocity = new Vector2(tempvelx, tempvely);
                try
                {
                    omega = Math.Sign(omega) * (float)Math.Sqrt(2 * angkinetic / inertia);
                }
                catch
                {
                    omega = 0d;
                }
                omega = omega + 0.01d * velocity.X; //spin effect on wall collision
                soundBank.PlayCue("ballground");
            }
            if ((Math.Round(position.Y + size.Y) == screensize.Y) && (velocity.X != 0f)) //Effect of Friction
            {
                if (velocity.X > 0)
                {
                    velocity = new Vector2((float)(velocity.X - (friction / mass) * (float)time), velocity.Y);
                    accelerationX = -(friction / mass);
                }
                else
                {
                    velocity = new Vector2((float)(velocity.X + (friction / mass) * (float)time), velocity.Y);
                    accelerationX = (friction / mass);
                }
            }
            else
                accelerationX = 0f;
            if ((Math.Round(position.Y + size.Y) == screensize.Y) && (velocity.X != 0f) && (velocity.Y == 0f)) //pure rolling
                omega = -velocity.X * 2 / size.X;
            if ((Math.Round(position.Y + size.Y) == screensize.Y) && (velocity.Y == 0)) //no acceleration when on ground
                accelerationY = 0;
            if ((velocity.X < 0.015f) && (velocity.X > -0.015f)) //stop Friction
            {
                accelerationX = 0;
                velocity = new Vector2(0f, velocity.Y);
            }
            if ((velocity.X == 0f) && (velocity.Y == 0f)) //stop rotational motion
                omega = 0d;
            if (float.IsNaN(velocity.X))
                velocity = new Vector2(0f, velocity.Y);
            if (float.IsNaN(velocity.Y))
                velocity = new Vector2(velocity.X, 0f);
            position += velocity; //update position
            rotation -= omega; //update rotation
            if (omega > 0d) //air resistance
                omega -= 0.0001d;
            else if (omega < 0d)
                omega += 0.0001d;
        }
        
        public bool CirclesCollide(SpriteClass s) //collision detection
        {
            float pendepth, temppos1, temppos2;
            double collisionangle;
            if (((position.X - s.position.X) * (position.X - s.position.X) + (position.Y - s.position.Y) * (position.Y - s.position.Y)) <= ((size.X / 2 + s.size.X / 2) * (size.X / 2 + s.size.X / 2)))
            {   //pre-collision response
                pendepth = (float)(-Math.Sqrt((position.X - s.position.X) * (position.X - s.position.X) + (position.Y - s.position.Y) * (position.Y - s.position.Y)) + (size.X / 2 + s.size.X / 2));
                collisionangle = Math.Abs(Math.Atan2(position.Y - s.position.Y, -position.X + s.position.X));
                int dir1 = 1, dir2 = 1;
                if (position.X < s.position.X)
                    dir1 = -1;
                if (position.Y < s.position.Y)
                    dir2 = -1;
                temppos1 = (position.X + dir1 * pendepth/2 * (float)Math.Abs(Math.Cos(collisionangle))); //reverse penetration from both the balls equally but in opposite directions 
                temppos2 = (position.Y + dir2 * pendepth/2 * (float)Math.Abs(Math.Sin(collisionangle)));
                position = new Vector2(temppos1, temppos2);
                temppos1 = (s.position.X - dir1 * pendepth/2 * (float)Math.Abs(Math.Cos(collisionangle)));
                temppos2 = (s.position.Y - dir2 * pendepth/2 * (float)Math.Abs(Math.Sin(collisionangle)));
                s.position = new Vector2(temppos1, temppos2); 
                return true;
            }
            else
                return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Vector2(position.X + 32, position.Y + 32), null, Color.White, (float)rotation, new Vector2(32f,32f), 1f, SpriteEffects.None, 0.5f);
        }

        public void UnloadContent()
        {
            texture.Dispose();
        }
    }
}
