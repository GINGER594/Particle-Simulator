using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using particles;
using handler;

namespace Particle_Simulator;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    //defining constants for screen dimensions and fps
    private const int screenWidth = 400;
    private const int screenHeight = 400;
    private const int framerate = 60;

    //defining textures
    private Texture2D blank;

    //defining particles
    private Particle[] particles;

    //defining particle handler
    private Handler handler;

    //current block type selected
    private int currentBlock = 0;

    //erase and clear buttons
    private Rectangle eraseButton = new Rectangle(screenWidth - 40, 0, 20, 20);
    private Rectangle clearButton = new Rectangle(screenWidth - 20, 0, 20, 20);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //setting screen dimensions
        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        _graphics.ApplyChanges();

        //setting framerate
        TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / framerate);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        //each type of particle is given a token which represents it on the 2D array 'grid'
        Particle block = new Particle(1, Color.Gray, 0); //token is 1, block button x is 0
        Particle sand = new Sand(2, Color.Gold, 20); //token is 2, block button x is 20
        Particle water = new Liquid(3, Color.Blue, 40); //token is 3, block button x is 40
        particles = [block, sand, water];
        handler = new Handler(5, screenWidth, screenHeight, particles);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        //loading blank texture
        blank = new Texture2D(GraphicsDevice, 1, 1);
        blank.SetData([Color.White]);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        //getting mouse state
        var mouseState = Mouse.GetState();

        //checking for left clicks that are within the screen
        if (mouseState.LeftButton == ButtonState.Pressed && mouseState.X >= 0 && mouseState.X < screenWidth && mouseState.Y >= 0 && mouseState.Y <= screenHeight - 5)
        {
            Rectangle mouse = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            //checking through each button toggle
            var buttonClicked = false;
            foreach (Particle particle in particles)
            {
                if (mouse.Intersects(particle.button))
                {
                    currentBlock = particle.token;
                    buttonClicked = true;
                }
            }
            //checking for click on erase button
            if (mouse.Intersects(eraseButton))
            {
                currentBlock = 0;
                buttonClicked = true;
            }
            //checking for click on clear button
            else if (mouse.Intersects(clearButton))
            {
                currentBlock = 0;
                handler.ClearGrid();
                buttonClicked = true;
            }

            //if a button hasnt been pressed, place particle at mouse position (if last clicked erase button, particles placed will be empty spaces)
            if (buttonClicked == false)
            {
                handler.SetParticle(mouseState.X, mouseState.Y, currentBlock);
            }
        }

        //update all particles
        handler.MoveParticles();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        //drawing particles and their buttons
        handler.DrawGrid(_spriteBatch, blank);
        //drawing erase and clear buttons
        _spriteBatch.Draw(blank, new Vector2(eraseButton.X, eraseButton.Y), eraseButton, Color.White);
        _spriteBatch.Draw(blank, new Vector2(clearButton.X, clearButton.Y), clearButton, Color.Red);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
