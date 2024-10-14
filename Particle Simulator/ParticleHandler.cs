using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using particles;

namespace handler;

class Handler
{
    private int size; //size of each particle
    private int gridWidth;
    private int gridHeight;
    private int[,] grid;
    private Particle[] particles;
    private List<int> liquidTokens = new List<int>(); //list of all distinct liquid particle tokens (used for checking when sands collide with liquids)
    public Handler(int s, int screenWidth, int screenHeight, Particle[] p)
    {
        //defining particle size
        size = s;

        //defining a new 2D array of ints based on the size of the screen relative to the particle size
        gridWidth = screenWidth / size;
        gridHeight = screenHeight / size;
        grid = new int[gridWidth, gridHeight];

        particles = p;

        //adding all the tokens of any differnt types of liquid particles to the liquidToken list
        foreach (Particle particle in particles)
        {
            if (particle.type == "liquid")
            {
                liquidTokens.Add(particle.token);
            }
        }
    }

    public void SetParticle(int x, int y, int token)
    {
        x /= size;
        y /= size;

        //only setting a particle if cell is 0 and the token is not 0, or the cell is not 0 and the token is
        if ((grid[x, y] == 0 && token != 0) || (grid[x, y] != 0 && token == 0))
        {
            grid[x, y] = token;
        }
    }

    public void ClearGrid()
    {
        //resetting the grid
        grid = new int[gridWidth, gridHeight];
    }

    public void MoveParticles()
    {
        //iterating through each horizontal row in grid from bottom to top
        for (int y = gridHeight - 2; y > -1; y--)
        {
            //iterating through each cell in each row
            for (int x = 0; x < gridWidth; x++)
            {
                //checking if current cell is a particle
                foreach (Particle particle in particles)
                {
                    if (grid[x, y] == particle.token)
                    {
                        //calling the particles 'MoveParticle' method
                        grid = particle.MoveParticle(grid, gridWidth, gridHeight, x, y, liquidTokens);
                    }
                }
            }
        }
    }

    public void DrawGrid(SpriteBatch _spritebatch, Texture2D blank)
    {
        //drawing cells
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                foreach (Particle particle in particles)
                {
                    if (grid[x, y] == particle.token)
                    {
                        _spritebatch.Draw(blank, new Vector2(x * size, y * size), new Rectangle(x * size, y * size, size, size), particle.colour);
                    }
                }
            }
        }

        //drawing buttons
        foreach (Particle particle in particles)
        {
            _spritebatch.Draw(blank, new Vector2(particle.button.X, particle.button.Y), particle.button, particle.colour);
        }
    }
}