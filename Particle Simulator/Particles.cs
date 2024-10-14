using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Threading.Tasks.Dataflow;
using Microsoft.Xna.Framework;

namespace particles;

class Particle
{
    public string type { get; set; } //string type used to find which particles are liquids so that a list of liquid tokens can be generated to be used in sand movement
    public int token { get; set; } //numerical token that represents this particle on the 2D array 'grid'
    public Color colour { get; set; } //colour of the particle and its toggle button
    public Rectangle button { get; set; } //the toggle button for this particle
    public Particle(int t, Color c, int buttonX)
    {
        type = "solid";
        token = t;
        colour = c;
        button = new Rectangle(buttonX, 0, 20, 20);
    }

    public virtual int[,] MoveParticle(int[,] grid, int gridWidth, int gridHeight, int x, int y, List<int> liquidTokens)
    {
        return grid;
    }
}

class Sand : Particle
{
    public Sand(int t, Color c, int buttonX) : base(t, c, buttonX)
    {
        type = "solid";
    }

    public override int[,] MoveParticle(int[,] grid, int gridWidth, int gridHeight, int x, int y, List<int> liquidTokens)
    {
        //if cell has nothing below it (except liquids): move down
        if (grid[x, y + 1] == 0 || IsInTokenList(liquidTokens, grid[x, y + 1]) )
        {
            grid[x, y] = 0;
            grid[x, y + 1] = token;
        }
        //else if cell is not touching the left side of the screen, has a cell beneath it and no cell to its left or bottom left (except liquids): move down and left
        else if (x > 0 && (grid[x - 1, y + 1] == 0 || IsInTokenList(liquidTokens, grid[x - 1, y + 1])) && (grid[x - 1, y] == 0 || IsInTokenList(liquidTokens, grid[x - 1, y])))
        {
            grid[x, y] = 0;
            grid[x - 1, y + 1] = token;
        }
        //else if cell is not touching the right side of the screen, has a cell beneath it and no cell to its right or bottom right (except liquids): move down and right
        else if (x < gridWidth - 1 && (grid[x + 1, y + 1] == 0 || IsInTokenList(liquidTokens, grid[x + 1, y + 1])) && (grid[x + 1, y] == 0 || IsInTokenList(liquidTokens, grid[x + 1, y])))
        {
            grid[x, y] = 0;
            grid[x + 1, y + 1] = token;
        }

        return grid;
    }

    //function to check if a specific token belongs to a specific type of particle (used to detect if sand is ontop of a liquid)
    private bool IsInTokenList(List<int> liquidTokens, int token)
    {
        foreach (int liquidToken in liquidTokens)
        {
            if (liquidToken == token)
            {
                return true;
            }
        }
        return false;
    }
}

class Liquid : Particle
{
    public Liquid(int t, Color c, int buttonX) : base(t, c, buttonX)
    {
        type = "liquid";
    }

    public override int[,] MoveParticle(int[,] grid, int gridWidth, int gridHeight, int x, int y, List<int> liquidTokens)
    {
        //if cell has nothing below it: move down
        if (grid[x, y + 1] == 0)
        {
            grid[x, y] = 0;
            grid[x, y + 1] = token;
        }
        //else move a random direction (left or right) unless there is a screen border
        else
        {
            Random r = new Random();
            var direction = r.NextInt64(2);
            if (direction == 0 && x > 0 && grid[x - 1, y] == 0)
            {
                grid[x, y] = 0;
                grid[x - 1, y] = token;
            }
            else if (direction == 1 && x < gridWidth - 1 && grid[x + 1, y] == 0)
            {
                grid[x, y] = 0;
                grid[x + 1, y] = token;
            }
        }
        return grid;
    }
}