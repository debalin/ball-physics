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
using System.Diagnostics;

namespace SpriteTest
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont fontandy;
        private Texture2D background;

        private SpriteClass spriteClass1, spriteClass2;
        
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue backgroundmusic, rewindmusic;
        
        private Vector2[] storedpos1, storedpos2, storedvel1, storedvel2; 
        private double[] storedrot1, storedrot2, storedomg1, storedomg2;
        private String info, intro, outro, inputtext1, inputtext2, inputtext3, inputtext4, inputtext5, inputtext6, inputpostext1, inputpostext2;
        private int arraypos = 0;
        private float initvelx1, initvelx2, initvely1, initvely2, initposx1, initposx2, initposy1, initposy2, mass1, mass2; 
        private double step, laststep = -1f, lag, tempvely1 = 0d, tempvely2 = 0d;
        private bool inputx1 = false, inputx2 = false, inputy1 = false, inputy2 = false, inputmass1 = false, inputmass2 = false, credits = false, demo = false, desc = false, inputpos1 = false, inputpos2 = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            this.Window.Title = "Basic Bouncing Ball Physics";
            this.IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            storedpos1 = new Vector2[600];
            storedpos2 = new Vector2[600];
            storedvel1 = new Vector2[600];
            storedvel2 = new Vector2[600];
            storedrot1 = new double[600];
            storedrot2 = new double[600];
            storedomg1 = new double[600];
            storedomg2 = new double[600];
            for (arraypos = 0; arraypos < 600; arraypos++)
            {
                storedpos1[arraypos] = new Vector2(-1, -1);
                storedpos2[arraypos] = new Vector2(-1, -1);
            }
            arraypos = 0;
            fontandy = Content.Load<SpriteFont>("andy");
            audioEngine = new AudioEngine(@"Content/Music.xgs");
            waveBank = new WaveBank(audioEngine, @"Content/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content/Sound Bank.xsb");
            backgroundmusic = soundBank.GetCue("Machine Language");
            rewindmusic = soundBank.GetCue("rewind");
            rewindmusic.Play();
            rewindmusic.Pause();
            backgroundmusic.Play();
            backgroundmusic.Pause();
            initvelx1 = 5f;
            initvely1 = 2f;
            initvelx2 = -5f;
            initvely2 = 2f;
            initposx1 = 20f;
            initposy1 = 20f;
            initposx2 = graphics.PreferredBackBufferWidth - 64f - 20f;
            initposy2 = 20f;
            mass1 = 10f;
            mass2 = 15f;
            step = 12f;
            outro = "Code and Music by\n       Debalin";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteClass1 = new SpriteClass(Content.Load<Texture2D>("ball1"), new Vector2(initposx1, initposy1), new Vector2(64f, 64f), new Vector2(initvelx1, initvely1), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), mass1);
            spriteClass2 = new SpriteClass(Content.Load<Texture2D>("ball2"), new Vector2(initposx2, initposy2), new Vector2(64f, 64f), new Vector2(initvelx2, initvely2), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), mass2);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("room2");
        }

        protected override void UnloadContent()
        {
            spriteClass1.UnloadContent();
            spriteClass2.UnloadContent();
            background.Dispose();
            backgroundmusic.Dispose();
            waveBank.Dispose();
            soundBank.Dispose();
            audioEngine.Dispose();
        }

        private void Store() //store for later rewinding
        {
            storedpos1[arraypos] = spriteClass1.position;
            storedvel1[arraypos] = spriteClass1.velocity;
            storedpos2[arraypos] = spriteClass2.position;
            storedvel2[arraypos] = spriteClass2.velocity;
            storedrot1[arraypos] = spriteClass1.rotation;
            storedrot2[arraypos] = spriteClass2.rotation;
            storedomg1[arraypos] = spriteClass1.omega;
            storedomg2[arraypos] = spriteClass2.omega;
            if (++arraypos == 600)
                arraypos = 0;
        }

        private void TakeInput(GameTime gameTime) //take necessary inputs
        {
            KeyboardState keyboardState = Keyboard.GetState();;
            MouseState mouseState = Mouse.GetState();
            if (!inputmass1)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    mass1 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down) && (mass1 > 10f))
                    mass1 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    inputmass1 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
            }
            else if (!inputmass2)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    mass2 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down) && (mass2 > 10f))
                    mass2 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputmass2 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
            }
            else if (!inputpos1)
            {
                float temppos;
                if (keyboardState.IsKeyDown(Keys.Right) && (initposx1 < graphics.PreferredBackBufferWidth - 69f))
                {
                    temppos = initposx1 + 3f;
                    if ((Vector2.Distance(new Vector2(temppos, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposx1 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Left) && (initposx1 > 5f))
                {
                    temppos = initposx1 - 3f;
                    if ((Vector2.Distance(new Vector2(temppos, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposx1 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Up) && (initposy1 > 5f))
                {
                    temppos = initposy1 - 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, temppos) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposy1 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Down) && (initposy1 < graphics.PreferredBackBufferHeight - 69f))
                {
                    temppos = initposy1 + 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, temppos) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposy1 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputpos1 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
                spriteClass1.position = new Vector2(initposx1, initposy1);
            }
            else if (!inputpos2)
            {
                float temppos;
                if (keyboardState.IsKeyDown(Keys.Right) && (initposx2 < graphics.PreferredBackBufferWidth - 69f))
                {
                    temppos = initposx2 + 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(temppos, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposx2 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Left) && (initposx2 > 5f))
                {
                    temppos = initposx2 - 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(temppos, initposy2) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposx2 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Up) && (initposy2 > 5f))
                {
                    temppos = initposy2 - 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, temppos) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposy2 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Down) && (initposy2 < graphics.PreferredBackBufferHeight - 69f))
                {
                    temppos = initposy2 + 3f;
                    if ((Vector2.Distance(new Vector2(initposx1, initposy1) + new Vector2(64f / 2, 64f / 2), new Vector2(initposx2, temppos) + new Vector2(64f / 2, 64f / 2)) > 64f))
                        initposy2 = temppos;
                }
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputpos2 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
                spriteClass2.position = new Vector2(initposx2, initposy2);
            }
            else if (!inputx1)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    initvelx1 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    initvelx1 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputx1 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
            }
            else if (!inputy1)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    initvely1 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    initvely1 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputy1 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }

            }
            else if (!inputx2)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    initvelx2 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    initvelx2 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputx2 = true;
                    lag = gameTime.TotalGameTime.TotalSeconds + 1;
                }
            }
            else if (!inputy2)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    initvely2 += 0.05f;
                else if (keyboardState.IsKeyDown(Keys.Down))
                    initvely2 -= 0.05f;
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime.TotalSeconds > lag))
                {
                    inputy2 = true;
                    step = gameTime.TotalGameTime.TotalSeconds + 12.5f;
                    desc = true;
                    intro = "This is a close-to real life simulation of two bouncing balls showing \nthe effect of gravity, friction, collision, loss of kinetic energy and\nrotational motion." + "\n\nInitial velocity of the blue ball : " + String.Format("{0:0.00}", initvelx1) + " (X)   " + String.Format("{0:0.00}", initvely1) + " (Y)" + "\nInitial velocity of the brown ball : " + String.Format("{0:0.00}", initvelx2) + " (X)   " + String.Format("{0:0.00}", initvely2) + " (Y)" + "\n\nAfter one demostration ends, the balls will start again from their original" + "\nposition with their original velocities incremented by 5 in both" + "\ndirections. Their masses will also be incremented by 3.";
                    spriteClass1.velocity = new Vector2(initvelx1, initvely1);
                    spriteClass2.velocity = new Vector2(initvelx2, initvely2);
                    spriteClass1.mass = (float)Math.Round(mass1);
                    spriteClass2.mass = (float)Math.Round(mass2);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            double tempvelx1, tempvelx2, tempvelx3, tempvelx4, collisionangle, magnitude1, magnitude2, direction1, direction2;
            KeyboardState keyboardState = Keyboard.GetState();
            bool pause = keyboardState.IsKeyDown(Keys.Space);
            bool rewind = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            inputtext1 = "Enter intitial velocity of the blue ball in the X direction : " + String.Format("{0:0.00}", initvelx1) + "\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done. \n\nRight and down are positive X and Y directions respectively.";
            inputtext2 = "Enter intitial velocity of the blue ball in the Y direction : " + String.Format("{0:0.00}", initvely1) + "\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done. \n\nRight and down are positive X and Y directions respectively.";
            inputtext3 = "Enter intitial velocity of the brown ball in the X direction : " + String.Format("{0:0.00}", initvelx2) + "\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done. \n\nRight and down are positive X and Y directions respectively.";
            inputtext4 = "Enter intitial velocity of the brown ball in the Y direction : " + String.Format("{0:0.00}", initvely2) + "\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done. \n\nRight and down are positive X and Y directions respectively.";
            inputtext5 = "Enter mass of the blue ball : " + String.Format("{0:0}", Math.Round(mass1)) +  "\n\nMinimum allowed is 10.\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done.";
            inputtext6 = "Enter mass of the brown ball : " + String.Format("{0:0}", Math.Round(mass2)) + "\n\nMinimum allowed is 10.\n\nPress UP arrow to increase and DOWN arrow to" + "\ndecrease. Press ENTER when done.";
            inputpostext1 = "Position the blue ball.\n\nPress the arrow keys to move it around.\nPress ENTER when done.";
            inputpostext2 = "Position the brown ball.\n\nPress the arrow keys to move it around.\nPress ENTER when done.";
            if (!(inputmass1 && inputmass2 && inputpos1 && inputpos2 && inputx1 && inputx2 && inputy1 && inputy2))
                TakeInput(gameTime);
            else
            {
                if (!pause) 
                {
                    if (!rewind)
                    {
                        if (backgroundmusic.IsPaused)
                            backgroundmusic.Resume();
                        if (rewindmusic.IsPlaying)
                            rewindmusic.Pause();
                        if (gameTime.TotalGameTime.TotalSeconds > step)
                        {
                            desc = false;
                            demo = true;
                            spriteClass1.Move(gameTime.ElapsedGameTime.TotalSeconds, spriteClass2, soundBank);
                            if (spriteClass1.CirclesCollide(spriteClass2)) //collision response
                            {
                                soundBank.PlayCue("ballandball");
                                collisionangle = Math.Atan2(spriteClass1.position.Y - spriteClass2.position.Y, -spriteClass1.position.X + spriteClass2.position.X);
                                direction1 = Math.Atan2(-spriteClass1.velocity.Y, spriteClass1.velocity.X);
                                direction2 = Math.Atan2(-spriteClass2.velocity.Y, spriteClass2.velocity.X);
                                magnitude1 = Math.Sqrt(spriteClass1.velocity.X * spriteClass1.velocity.X + spriteClass1.velocity.Y*spriteClass1.velocity.Y);
                                magnitude2 = Math.Sqrt(spriteClass2.velocity.X * spriteClass2.velocity.X + spriteClass2.velocity.Y*spriteClass2.velocity.Y);
                                tempvelx1 = magnitude1 * Math.Cos(direction1 - collisionangle);
                                tempvely1 = magnitude1 * Math.Sin(direction1 - collisionangle);
                                tempvelx2 = magnitude2 * Math.Cos(direction2 - collisionangle);
                                tempvely2 = magnitude2 * Math.Sin(direction2 - collisionangle);
                                tempvelx3 = ((spriteClass1.mass - spriteClass2.mass) * tempvelx1 / (spriteClass1.mass + spriteClass2.mass)) + (2 * spriteClass2.mass * tempvelx2 / (spriteClass1.mass + spriteClass2.mass));
                                tempvelx4 = -((spriteClass1.mass - spriteClass2.mass) * tempvelx2 / (spriteClass1.mass + spriteClass2.mass)) + (2 * spriteClass1.mass * tempvelx1 / (spriteClass1.mass + spriteClass2.mass));
                                magnitude1 = Math.Sqrt(tempvelx3 * tempvelx3 + tempvely1 * tempvely1);
                                magnitude2 = Math.Sqrt(tempvelx4 * tempvelx4 + tempvely2 * tempvely2);
                                direction1 = Math.Atan2(tempvely1, tempvelx3) + collisionangle;
                                direction2 = Math.Atan2(tempvely2, tempvelx4) + collisionangle;
                                spriteClass1.omega = (tempvely2 - tempvely1) * 2 * 10 / (spriteClass1.size.X * spriteClass1.mass);
                                spriteClass2.omega = -(tempvely1 - tempvely2) * 2 * 10 / (spriteClass2.size.X * spriteClass2.mass);
                                spriteClass1.velocity = new Vector2((float)(magnitude1 * Math.Cos(direction1)), -(float)(magnitude1 * Math.Sin(direction1)));
                                spriteClass2.velocity = new Vector2((float)(magnitude2 * Math.Cos(direction2)), -(float)(magnitude2 * Math.Sin(direction2)));
                            }
                            spriteClass2.Move(gameTime.ElapsedGameTime.TotalSeconds, spriteClass1, soundBank);
                            Store();
                        }
                        if ((spriteClass1.velocity.X == 0f) && (spriteClass1.velocity.Y == 0f) && (spriteClass2.velocity.X == 0f) && (spriteClass2.velocity.Y == 0f) && backgroundmusic.IsPlaying) //change velocities and start again
                        {
                            spriteClass1.position = new Vector2(initposx1, initposy1);
                            spriteClass2.position = new Vector2(initposx2, initposy2);
                            initvelx1 += 5;
                            initvely1 += 5;
                            initvelx2 -= 5;
                            initvely2 += 5;
                            spriteClass1.velocity = new Vector2(initvelx1, initvely1);
                            spriteClass2.velocity = new Vector2(initvelx2, initvely2);
                            mass1 += 3;
                            mass2 += 3;
                            spriteClass1.mass = mass1;
                            spriteClass2.mass = mass2;
                            spriteClass1.rotation = 0d;
                            spriteClass2.rotation = 0d;
                            spriteClass1.omega = 0.05d;
                            spriteClass2.omega = 0.05d;
                            spriteClass1.kinetic = 0f;
                            spriteClass2.kinetic = 0f;
                            step = gameTime.TotalGameTime.TotalSeconds + 3;
                            for (arraypos = 0; arraypos < 600; arraypos++)
                            {
                                storedpos1[arraypos] = new Vector2(-1, -1);
                                storedpos2[arraypos] = new Vector2(-1, -1);
                            }
                            arraypos = 0;
                        }
                    }
                    else //rewind game
                    {
                        if (backgroundmusic.IsPlaying)
                            backgroundmusic.Pause();
                        if (rewindmusic.IsPaused)
                            rewindmusic.Resume();
                        if (rewindmusic.IsStopped)
                        {
                            rewindmusic = soundBank.GetCue("rewind");
                            rewindmusic.Play();
                        }
                        if (arraypos > 0)
                            --arraypos;
                        else
                            arraypos = 599;
                        if (storedpos1[arraypos].X != -1)
                        {
                            spriteClass1.position = storedpos1[arraypos];
                            spriteClass1.velocity = storedvel1[arraypos];
                            spriteClass2.position = storedpos2[arraypos];
                            spriteClass2.velocity = storedvel2[arraypos];
                            spriteClass1.rotation = storedrot1[arraypos];
                            spriteClass2.rotation = storedrot2[arraypos];
                            spriteClass1.omega = storedomg1[arraypos];
                            spriteClass2.omega = storedomg2[arraypos];
                            storedpos1[arraypos] = new Vector2(-1, -1);
                            storedpos2[arraypos] = new Vector2(-1, -1);
                        }
                    }
                }
                else if (backgroundmusic.IsPlaying) //pause game
                    backgroundmusic.Pause();
            }
            info = "---> Positive X\n|\n| Positive Y\nv\n        Blue        Brown\nMass = " + Math.Round(spriteClass1.mass) + " ----- " + Math.Round(spriteClass2.mass) + "\nAccelerationY = " + spriteClass1.accelerationY + " ----- " + spriteClass2.accelerationY + "\nAccelerationX = " + spriteClass1.accelerationX + " ----- " + spriteClass2.accelerationX + "\nVelocityX = " + String.Format("{0:0.00}", spriteClass1.velocity.X) + " ----- " + String.Format("{0:0.00}", spriteClass2.velocity.X) + "\nVelocityY = " + String.Format("{0:0.00}", spriteClass1.velocity.Y) + " ----- " + String.Format("{0:0.00}", spriteClass2.velocity.Y) + "\nKinetic Energy = " + String.Format("{0:0.00}", spriteClass1.kinetic) + " ----- " + String.Format("{0:0.00}", spriteClass2.kinetic) + "\n\nLoss of Kinetic Energy per impact on wall = 20% \nCo-Efficient of Kinetic Friction = 0.15 \nPure rolling is considered when the ball is on ground.\n\nPress and hold SPACE to pause.\nPress and hold SHIFT to rewind.";
            if ((spriteClass1.velocity.X == 0f) && (spriteClass1.velocity.Y == 0f) && (spriteClass2.velocity.X == 0f) && (spriteClass2.velocity.Y == 0f) && backgroundmusic.IsStopped && (laststep < 0f)) //credits
            {
                laststep = gameTime.TotalGameTime.TotalSeconds + 3;
                credits = true;
            }
            if ((laststep > 0f) && (gameTime.TotalGameTime.TotalSeconds > laststep))
                this.Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White, 0f, new Vector2(0,0), SpriteEffects.None, .75f);
            spriteClass1.Draw(spriteBatch);
            spriteClass2.Draw(spriteBatch);
            if (!inputmass1)
                spriteBatch.DrawString(fontandy, inputtext5, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2), Color.White, 0f, fontandy.MeasureString(inputtext5)/2, 1f, SpriteEffects.None, 0f);
            else if (!inputmass2)
                spriteBatch.DrawString(fontandy, inputtext6, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2), Color.White, 0f, fontandy.MeasureString(inputtext6)/2, 1f, SpriteEffects.None, 0f);
            else if (!inputpos1)
                spriteBatch.DrawString(fontandy, inputpostext1, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2), Color.White, 0f, fontandy.MeasureString(inputpostext1)/2, 1f, SpriteEffects.None, 0f);
            else if (!inputpos2)
                spriteBatch.DrawString(fontandy, inputpostext2, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2), Color.White, 0f, fontandy.MeasureString(inputpostext2)/2, 1f, SpriteEffects.None, 0f);
            else if (!inputx1)
                spriteBatch.DrawString(fontandy, inputtext1, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(inputtext1) / 2, 1f, SpriteEffects.None, 0f);
            else if (!inputy1)
                spriteBatch.DrawString(fontandy, inputtext2, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(inputtext2) / 2, 1f, SpriteEffects.None, 0f);
            else if (!inputx2)
                spriteBatch.DrawString(fontandy, inputtext3, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(inputtext3) / 2, 1f, SpriteEffects.None, 0f);
            else if (!inputy2)
                spriteBatch.DrawString(fontandy, inputtext4, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(inputtext2) / 2, 1f, SpriteEffects.None, 0f);
            if (desc)
                spriteBatch.DrawString(fontandy, intro, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(intro) / 2, 1f, SpriteEffects.None, 0f);
            if (credits)
                spriteBatch.DrawString(fontandy, outro, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White, 0f, fontandy.MeasureString(outro) / 2, 1f, SpriteEffects.None, 0f);
            else if (demo)
                spriteBatch.DrawString(fontandy, info, new Vector2(50,50), Color.White, 0f, new Vector2(0f,0f), 1f, SpriteEffects.None, .60f);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
