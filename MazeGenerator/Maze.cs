using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MazeGenerator
{
    public class Maze
    {
        internal struct Cell
        {
            public int right, top, visited, rvisited, tvisited, solution, rsolve, tsolve, rdrawn, tdrawn, identity, tru;

            public Color color;
        }

        internal class MazeCell
        {
            public int column, row, direction;

            public MazeCell(int c, int r, int d = 0)
            {
                column = c;
                row = r;
                direction = d;
            }
        }

        Graphics mazeCanvas;
        Color colorBackground, colorForeground, colorCurrentCell, colorGenerate, colorSolve, colorBacktrack, colorStart, colorStop;
        Pen penBackground, penForeground;

        Cell[,] grid;
        Stack<MazeCell> visitedCellsStack;
        Queue<MazeCell> visitedCellsQueue;

        int height, width;

        int visited, hunt, kill;
        int showgen, genspeed, genspeed_exp;
        int backtrack;
        int showsolve, solvespeed, solvespeed_exp;
        int sleeptime;
        PointF startGen, stopGen;
        Point startSolve, stopSolve;
        string algorithm;
        int algorithmType;
        int inverse;

        float start_x, start_y, spacer, padding_x, padding_y, side_length, cell_padding;

        bool displayGridFlag;

        Random rng;

        Stopwatch stopwatch;

        public Maze(Graphics g, Color c)
        {
            mazeCanvas = g;

            colorBackground = c;
            colorForeground = Color.Black;
            colorCurrentCell = Color.Blue;
            colorGenerate = Color.LightBlue;
            colorSolve = Color.Blue;
            colorBacktrack = colorBackground;
            colorStart = Color.Green;
            colorStop = Color.Red;

            penBackground = new Pen(colorBackground, 2);
            penForeground = new Pen(colorForeground, 2);

            height = 40;
            width = 40;

            algorithm = "Recursive Backtracker";
            algorithmType = 0;
            inverse = 0;
            showgen = 1;
            genspeed = 16;
            genspeed_exp = 4;
            backtrack = 0;
            showsolve = 1;
            solvespeed = 16;
            solvespeed_exp = 4;
            sleeptime = 50;

            padding_x = 50;
            padding_y = 50;
            side_length = 821;

            stopwatch = new Stopwatch();

            initializeVars();
        }

        void initializeVars()
        {
            grid = new Cell[height, width];

            calculateCoords();

            visitedCellsStack = new Stack<MazeCell>();
            visitedCellsQueue = new Queue<MazeCell>();

            displayGridFlag = true;

            rng = new Random(Guid.NewGuid().GetHashCode());
        }

        void initializeGrid()
        {
            initializeVars();

            findStart();

            visited = 0;

            hunt = ((height * width) + 1);
            kill = 0;

            int count = 0;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    grid[r, c].right = 0;
                    grid[r, c].top = 0;
                    grid[r, c].visited = 0;
                    grid[r, c].solution = 0;
                    grid[r, c].visited = 0;
                    grid[r, c].rvisited = 0;
                    grid[r, c].tvisited = 0;
                    grid[r, c].rdrawn = 0;
                    grid[r, c].tdrawn = 0;
                    grid[r, c].rsolve = 0;
                    grid[r, c].tsolve = 0;
                    grid[r, c].identity = count;
                    grid[r, c].color = Color.FromArgb(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
                    count++;
                }
            }

            mazeCanvas.Clear(colorBackground);
        }

        public void setGraphics(Graphics g)
        {
            mazeCanvas = g;
        }

        public void setLocation(float pad_x, float pad_y, float side_len)
        {
            padding_x = pad_x;
            padding_y = pad_y;
            side_length = side_len;
        }

        public int getHeight()
        {
            return height;
        }

        public void setHeight(int h)
        {
            height = h;
        }

        public int getWidth()
        {
            return width;
        }

        public void setWidth(int w)
        {
            width = w;
        }

        public int getShowgen()
        {
            return showgen;
        }

        public void setShowgen(int s)
        {
            showgen = s;
        }

        public int getGenspeed()
        {
            return (genspeed_exp * 10);
        }

        public void setGenspeed(int g)
        {
            genspeed_exp = (int)(g / 10);
            genspeed = (int)Math.Pow(2D, (double)genspeed_exp);
        }

        public int getBacktrack()
        {
            return backtrack;
        }

        public void setBacktrack(int b)
        {
            backtrack = b;

            if (backtrack == 1)
            {
                colorBacktrack = Color.Gray;
            }
            else
            {
                colorBacktrack = colorBackground;
            }
        }

        public int getShowsolve()
        {
            return showsolve;
        }

        public void setShowsolve(int s)
        {
            showsolve = s;
        }

        public int getSolvespeed()
        {
            return (solvespeed_exp * 10);
        }

        public void setSolvespeed(int s)
        {
            solvespeed_exp = (int)(s / 10);
            solvespeed = (int)Math.Pow(2D, (double)solvespeed_exp);
        }

        public string getAlgorithm()
        {
            return algorithm;
        }

        public void setAlgorithm(string a)
        {
            algorithm = a;
        }

        public int getAlgorithmType()
        {
            return algorithmType;
        }

        public void setAlgorithmType(int a)
        {
            algorithmType = a;

            GenerateGrid();
        }

        public void inverseColors()
        {
            if (inverse == 0)
            {
                colorForeground = colorBackground;
                colorBackground = Color.Black;
                colorCurrentCell = Color.Red;
                colorGenerate = Color.Pink;
                colorSolve = Color.Red;
                colorStart = Color.Green;
                colorStop = Color.Blue;

                inverse = 1;
            }
            else
            {
                colorBackground = colorForeground;
                colorForeground = Color.Black;
                colorCurrentCell = Color.Blue;
                colorGenerate = Color.LightBlue;
                colorSolve = Color.Blue;
                colorStart = Color.Green;
                colorStop = Color.Red;

                inverse = 0;
            }

            setBacktrack(backtrack);

            penBackground = new Pen(colorBackground, 2);
            penForeground = new Pen(colorForeground, 2);

            GenerateGrid();
        }

        private void calculateCoords()
        {
            if (height >= width)
            {
                spacer = side_length / (float)height;
                start_x = padding_x + (((float)(height - width) * spacer) / 2F);
                start_y = padding_y;
            }
            else
            {
                spacer = side_length / (float)width;
                start_x = padding_x;
                start_y = padding_y + (((float)(width - height) * spacer) / 2F);
            }

            cell_padding = 8;

            if (spacer <= 6)
                cell_padding = 0;
            else if (spacer <= 10)
                cell_padding = 1;
            else if (spacer <= 15)
                cell_padding = 2;
            else if (spacer <= 21)
                cell_padding = 3;
            else if (spacer <= 28)
                cell_padding = 4;
            else if (spacer <= 36)
                cell_padding = 5;
            else if (spacer <= 46)
                cell_padding = 6;
            else if (spacer <= 58)
                cell_padding = 7;
        }

        public void displayMaze()
        {
            mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y), new PointF(start_x, start_y + ((float)height * spacer)));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y), new PointF(start_x, start_y));

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    //if (r != 0)
                    //{
                    //    if (grid[r, c].top == 1)
                    //        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c* spacer) + 2, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 2, start_y + ((float)r * spacer)));
                        
                    //    //if (grid[r, c].top != 1)
                    //    //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));
                    //}

                    //if (c != (width - 1))
                    //{
                    //    if (grid[r, c].right == 1)
                    //        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 2), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 2));
                        
                    //    //if (grid[r, c].right != 1)
                    //    //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));
                    //}

                    if (grid[r, c].rsolve == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                    }
                    
                    if (grid[r, c].tsolve == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                    }
                }
            }
        }

        public void displayOutline()
        {
            //mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y), new PointF(start_x, start_y + ((float)height * spacer)));
            //mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)));
            //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y));
            //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y), new PointF(start_x, start_y));

            mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x, start_y), new SizeF((float)width * spacer, 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x, start_y + 2), new SizeF(2, (float)height * spacer)));
            mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + 2, start_y + ((float)height * spacer)), new SizeF((float)width * spacer, 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)width * spacer), start_y), new SizeF(2, (float)height * spacer)));
        }

        public void displayGrid()
        {
            displayOutline();

            //for (int i = 1; i < height; i++)
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y + ((float)i * spacer)), new PointF(start_x + ((float)width * spacer), start_y + ((float)i * spacer)));

            //for (int i = 1; i < width; i++)
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)i * spacer), start_y), new PointF(start_x + ((float)i * spacer), start_y + ((float)height * spacer)));

            for (int r = 1; r < height; r++)
                mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + 2, start_y + ((float)r * spacer)), new SizeF(((float)width * spacer) - 2, 2)));

            for (int c = 1; c < width; c++)
                mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + 2), new SizeF(2, ((float)height * spacer) - 2)));
        }

        public void displayBlock()
        {
            mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x, start_y), new SizeF((spacer * width) + 2, (spacer * height) + 2)));
        }

        public void displayFinishedMaze_FromGrid()
        {
            mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y), new PointF(start_x, start_y + ((float)height * spacer)));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x, start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y + ((float)height * spacer)), new PointF(start_x + ((float)width * spacer), start_y));
            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)width * spacer), start_y), new PointF(start_x, start_y));

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i != 0)
                    {
                        if (grid[i, j].top == 1)
                            mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)j * spacer) + 1, start_y + ((float)i * spacer)), new PointF(start_x + ((float)j * spacer) + spacer - 1, start_y + ((float)i * spacer)));
                    }

                    if (j != (width - 1))
                    {
                        if (grid[i, j].right == 1)
                            mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)j * spacer) + spacer, start_y + ((float)i * spacer) + 1), new PointF(start_x + ((float)j * spacer) + spacer, start_y + ((float)i * spacer) + spacer - 1));
                    }
                }
            }
        }

        void displayRemoveWalls()
        {
            int r, c;

            foreach (MazeCell coord in visitedCellsQueue)
            {
                r = coord.row;
                c = coord.column;

                if (r < (height - 1) && grid[r + 1, c].top == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                if (grid[r, c].right == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                if (c > 0 && grid[r, c - 1].right == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                if (grid[r, c].top == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }
        }

        void displayBuildWalls()
        {
            int r, c;

            int visited = 0;

            //mazeCanvas.DrawString(visitedCellsQueue.Count.ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(50, 10));

            foreach (MazeCell coord in visitedCellsQueue)
            {
                r = coord.row;
                c = coord.column;

                if (r > 0 && grid[r, c].top == 0 && grid[r, c].tdrawn == 0)
                {
                    //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new SizeF(spacer + 2, 2)));

                    grid[r, c].tdrawn = 1;
                }

                if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].rdrawn == 0)
                {
                    //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

                    mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer)), new SizeF(2, spacer + 2)));

                    grid[r, c].rdrawn = 1;
                }

                if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r + 1, c].tdrawn == 0)
                {
                    //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)(r + 1) * spacer)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)(r + 1) * spacer)), new SizeF(spacer + 2, 2)));

                    grid[r + 1, c].tdrawn = 1;
                }

                if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c - 1].rdrawn == 0)
                {
                    //mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

                    mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new SizeF(2, spacer + 2)));

                    grid[r, c - 1].rdrawn = 1;
                }

                visited++;

                if (showgen == 1 && (visited % genspeed) == 0)
                {
                    Thread.Sleep(sleeptime);
                }
            }
        }

        void displayFillCells()
        {
            int r, c;

            int visited = 0;
            
            foreach (MazeCell coord in visitedCellsQueue)
            {
                r = coord.row;
                c = coord.column;

                if (r < (height - 1) && grid[r + 1, c].top == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                if (grid[r, c].right == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                if (c > 0 && grid[r, c - 1].right == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                if (grid[r, c].top == 1)
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                visited++;

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }
        }

        void displaySolve_Stack()
        {
            int r, c, direction;

            int visited = 0;

            foreach (MazeCell coord in visitedCellsQueue)
            {
                r = coord.row;
                c = coord.column;

                direction = coord.direction;

                if (direction == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (2 * cell_padding), (spacer * 2) - (2 * cell_padding))));
                }
                else if (direction == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF((spacer * 2) - (2 * cell_padding), spacer - (2 * cell_padding))));
                }
                else if (direction == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF((spacer * 2) - (2 * cell_padding), spacer - (2 * cell_padding))));
                }
                else if (direction == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)(r - 1) * spacer) + cell_padding), new SizeF(spacer - (2 * cell_padding), (spacer * 2) - (2 * cell_padding))));
                }
                else if (direction == 5)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (2 * cell_padding), (spacer * 2) - (2 * cell_padding))));
                }
                else if (direction == 6)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF((spacer * 2) - (2 * cell_padding), spacer - (2 * cell_padding))));
                }
                else if (direction == 7)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF((spacer * 2) - (2 * cell_padding), spacer - (2 * cell_padding))));
                }
                else if (direction == 8)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)(r - 1) * spacer) + cell_padding), new SizeF(spacer - (2 * cell_padding), (spacer * 2) - (2 * cell_padding))));
                }

                if ((r == startSolve.Y && c == startSolve.X) ||
                    ((r + 1) == startSolve.Y && c == startSolve.X) ||
                    ((r - 1) == startSolve.Y && c == startSolve.X) ||
                    (r == startSolve.Y && (c + 1) == startSolve.X) ||
                    (r == startSolve.Y && (c - 1) == startSolve.X))
                {
                    displayStart();
                }

                visited++;

                if (showsolve == 1 && (visited % solvespeed) == 0)
                {
                    Thread.Sleep(sleeptime);
                }
            }

            displayStart();
        }

        void displayStart()
        {
            mazeCanvas.FillRectangle(new SolidBrush(colorStart), new RectangleF(startGen, new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorStop), new RectangleF(stopGen, new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void findStart()
        {
            int corner = rng.Next(1, 5);

            if (corner == 1)
            {
                startGen = new PointF(start_x + cell_padding + 2, start_y + cell_padding + 2);
                stopGen = new PointF(start_x + ((float)(width - 1) * spacer) + cell_padding + 2, start_y + ((float)(height - 1) * spacer) + cell_padding + 2);

                startSolve = new Point(0, 0);
                stopSolve = new Point(width - 1, height - 1);
            }
            else if (corner == 2)
            {
                startGen = new PointF(start_x + cell_padding + 2, start_y + ((float)(height - 1) * spacer) + cell_padding + 2);
                stopGen = new PointF(start_x + ((float)(width - 1) * spacer) + cell_padding + 2, start_y + cell_padding + 2);

                startSolve = new Point(0, height - 1);
                stopSolve = new Point(width - 1, 0);
            }
            else if (corner == 3)
            {
                startGen = new PointF(start_x + ((float)(width - 1) * spacer) + cell_padding + 2, start_y + cell_padding + 2);
                stopGen = new PointF(start_x + cell_padding + 2, start_y + ((float)(height - 1) * spacer) + cell_padding + 2);

                startSolve = new Point(width - 1, 0);
                stopSolve = new Point(0, height - 1);
            }
            else
            {
                startGen = new PointF(start_x + ((float)(width - 1) * spacer) + cell_padding + 2, start_y + ((float)(height - 1) * spacer) + cell_padding + 2);
                stopGen = new PointF(start_x + cell_padding + 2, start_y + cell_padding + 2);

                startSolve = new Point(width - 1, height - 1);
                stopSolve = new Point(0, 0);
            }
        }

        bool deadEnd(int r, int c)
        {
            if ((r > 0 && grid[r - 1, c].visited == 0) ||
                (c < (width - 1) && grid[r, c + 1].visited == 0) ||
                (r < (height - 1) && grid[r + 1, c].visited == 0) ||
                (c > 0 && grid[r, c - 1].visited == 0))
                return false;

            return true;
        }

        bool emptyNeighbors(int r, int c)
        {
            if ((r > 0 && grid[r - 1, c].visited == 1) ||
                (c < (width - 1) && grid[r, c + 1].visited == 1) ||
                (r < (height - 1) && grid[r + 1, c].visited == 1) ||
                (c > 0 && grid[r, c - 1].visited == 1))
                return false;

            return true;
        }

        bool emptyNeighborsPrim_BuildWalls(int r, int c)
        {
            if (r > 0 && grid[r - 1, c].visited == 1)
                return false;

            if (c > 0 && grid[r, c - 1].visited == 1)
                return false;

            if (c < (width - 1) && grid[r, c + 1].visited == 1)
                return false;

            if (r < (height - 1) && grid[r + 1, c].visited == 1)
                return false;

            return true;
        }

        bool emptyNeighborsPrim_BuildWallsSingleWall(int r, int c)
        {
            if (grid[r, c].rvisited == 1 || grid[r, c].tvisited == 1)
                return false;

            if (r > 0 && (grid[r - 1, c].rvisited == 1 || grid[r - 1, c].tvisited == 1))
                return false;

            if (c > 0 && (grid[r, c - 1].rvisited == 1 || grid[r, c - 1].tvisited == 1))
                return false;

            if (c < (width - 1) && (grid[r, c + 1].rvisited == 1 || grid[r, c + 1].tvisited == 1))
                return false;

            if (r < (height - 1) && (grid[r + 1, c].rvisited == 1 || grid[r + 1, c].tvisited == 1))
                return false;

            return true;
        }

        bool emptyNeighborsPrim_BuildWallsSingleWall2(int r, int c)
        {
            if (grid[r, c].rvisited == 1 || grid[r, c].tvisited == 1)
                return false;

            if (r > 0 && (grid[r - 1, c].rvisited == 1 || grid[r - 1, c].tvisited == 1))
                return false;

            if (c > 0 && (grid[r, c - 1].rvisited == 1 || grid[r, c - 1].tvisited == 1))
                return false;

            if (c < (width - 1) && (grid[r, c + 1].rvisited == 1 || grid[r, c + 1].tvisited == 1))
                return false;

            if (r < (height - 1) && (grid[r + 1, c].rvisited == 1 || grid[r + 1, c].tvisited == 1))
                return false;

            return true;
        }

        bool hasUnvisitedNeighbor(int r, int c)
        {
            if ((r > 0 && grid[r - 1, c].visited == 0) ||
                (c < (width - 1) && grid[r, c + 1].visited == 0) ||
                (r < (height - 1) && grid[r + 1, c].visited == 0) ||
                (c > 0 && grid[r, c - 1].visited == 0))
                return true;

            return false;
        }

        bool hasVisitedNeighbor(int r, int c)
        {
            if ((r > 0 && grid[r - 1, c].visited == 1) ||
                (c < (width - 1) && grid[r, c + 1].visited == 1) ||
                (r < (height - 1) && grid[r + 1, c].visited == 1) ||
                (c > 0 && grid[r, c - 1].visited == 1))
                return true;

            return false;
        }

        int findCell(int r, int c)
        {
            while (true)
            {
                int d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    return 1;
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    return 2;
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    return 3;
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    return 4;
            }
        }

        void pickCell_RemoveWalls(int r, int c)
        {
            while (true)
            {
                int d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 1)
                {
                    grid[r, c].top = 1;

                    //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                    
                    return;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 1)
                {
                    grid[r, c].right = 1;

                    //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));

                    return;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 1)
                {
                    grid[r + 1, c].top = 1;

                    //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));

                    return;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 1)
                {
                    grid[r, c - 1].right = 1;

                    //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));

                    return;
                }
            }
        }

        void pickCell_BuildWalls(int r, int c)
        {
            bool go;

            do
            {
                go = false;

                int d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 1)
                {
                    grid[r, c].top = 1;
                }
                else if (d == 2 && c > 0 && grid[r, c - 1].visited == 1)
                {
                    grid[r, c - 1].right = 1;
                }
                else if (d == 3 && c < (width - 1) && grid[r, c + 1].visited == 1)
                {
                    grid[r, c].right = 1;
                }
                else if (d == 4 && r < (height - 1) && grid[r + 1, c].visited == 1)
                {
                    grid[r + 1, c].top = 1;
                }
                else
                {
                    go = true;
                }

            } while (go);
        }

        void pickCell_BuildWalls2(int r, int c)
        {
            if (r > 0 && grid[r, c].top == 0)
            {
                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));
            }

            if (c > 0 && grid[r, c - 1].right == 0)
            {
                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer));
            }

            if (c < (width - 1) && grid[r, c].right == 0)
            {
                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));
            }

            if (r < (height - 1) && grid[r + 1, c].top == 0)
            {
                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)(r + 1) * spacer)));
            }
        }

        void pickCell_FillCells(int r, int c)
        {
            while (true)
            {
                int d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 1)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, spacer)));

                    return;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 1)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    return;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 1)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer)));
                    
                    return;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 1)
                {
                    grid[r, c - 1].right = 1; ;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    return;
                }
            }
        }

        void generateRBStraight_RemoveWalls()
        {
            //visitRBStraight_RemoveWalls(rng.Next(0, height), rng.Next(0, width), 0);

            visitRBStraight_RemoveWalls_Stack_DFS();

            //visitRBStraight_RemoveWalls_Queue_BFS();

            //displayRemoveWalls();
        }

        void generateRBStraight_BuildWalls()
        {
            visitRBStraight_BuildWalls(rng.Next(0, height), rng.Next(0, width), 0);

            //visitRBStraight_BuildWalls_Stack();

            displayBuildWalls();
        }

        void generateRBStraight_FillCells()
        {
            //visitRBStraight_FillCells(rng.Next(0, height), rng.Next(0, width), 0);

            visitRBStraight_FillCells_Stack();
        }

        void generateRBJagged_RemoveWalls()
        {
            //visitRBJagged_RemoveWalls(rng.Next(0, height), rng.Next(0, width), 0);

            visitRBJagged_RemoveWalls_Stack();
        }

        void generateRBJagged_BuildWalls()
        {
            visitRBJagged_BuildWalls(rng.Next(0, height), rng.Next(0, width), 0);

            displayBuildWalls();
        }

        void generateRBJagged_FillCells()
        {
            //visitRBJagged_FillCells(rng.Next(0, height), rng.Next(0, width), 0);

            visitRBJagged_FillCells_Stack();
        }

        void generateHKStraight_RemoveWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKStraight_RemoveWalls(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r,c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_RemoveWalls(r, c);

                    if (hunt == 1 && showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    kill = 0;

                    visitHKStraight_RemoveWalls(r, c, 0);

                } while (visited < height * width);
            }
        }

        void generateHKStraight_RemoveWalls_Stack()
        {
            visited = 0;

            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKStraight_RemoveWalls_Stack(r, c);

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    pickCell_RemoveWalls(r, c);

                    visitHKStraight_RemoveWalls_Stack(r, c);
                }
            }
        }

        void generateHKStraight_BuildWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKStraight_BuildWalls(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_BuildWalls(r, c);

                    kill = 0;

                    visitHKStraight_BuildWalls(r, c, 0);

                } while (visited < height * width);
            }

            displayBuildWalls();
        }

        void generateHKStraight_FillCells()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKStraight_FillCells(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_FillCells(r, c);

                    if (hunt == 1 && showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    kill = 0;

                    visitHKStraight_FillCells(r, c, 0);

                } while (visited < height * width);
            }
        }

        void generateHKStraight_FillCells_Stack()
        {
            visited = 0;

            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));

            visitHKStraight_FillCells_Stack(r, c);

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    pickCell_FillCells(r, c);

                    visitHKStraight_FillCells_Stack(r, c);
                }
            }
        }

        void generateHKJagged_RemoveWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKJagged_RemoveWalls(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r,c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_RemoveWalls(r, c);

                    if (hunt == 1 && showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    kill = 0;

                    visitHKJagged_RemoveWalls(r, c, 0);

                } while (visited < height * width);
            }
        }

        void generateHKJagged_RemoveWalls_Stack()
        {
            visited = 0;

            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKJagged_RemoveWalls_Stack(r, c);

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    pickCell_RemoveWalls(r, c);

                    visitHKJagged_RemoveWalls_Stack(r, c);
                }
            }
        }

        void generateHKJagged_BuildWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKJagged_BuildWalls(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_BuildWalls(r, c);

                    kill = 0;

                    visitHKJagged_BuildWalls(r, c, 0);

                } while (visited < height * width);
            }

            displayBuildWalls();
        }

        void generateHKJagged_FillCells()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            visitHKJagged_FillCells(r, c, 0);

            if (visited < height * width)
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_FillCells(r, c);

                    if (hunt == 1 && showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    kill = 0;

                    visitHKJagged_FillCells(r, c, 0);

                } while (visited < height * width);
            }
        }

        void generateHKJagged_FillCells_Stack()
        {
            visited = 0;

            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));

            visitHKJagged_FillCells_Stack(r, c);

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    pickCell_FillCells(r, c);

                    visitHKJagged_FillCells_Stack(r, c);
                }
            }
        }

        void generatePrim_RemoveWalls()
        {
            visitPrim_RemoveWalls();
        }

        void generatePrim_BuildWalls()
        {
            visitPrim_BuildWalls();
        }

        void generatePrim_FillCells()
        {
            visitPrim_FillCells();}

        void generateKruskal_RemoveWalls()
        {
            //visitKruskal_RemoveWalls();

            visitKruskal_RemoveWalls_Stack();
        }

        void generateKruskal_BuildWalls()
        {
            visitKruskal_BuildWalls();
        }

        void generateKruskal_FillCells()
        {
            //visitKruskal_FillCells();

            visitKruskal_FillCells_Stack();
        }

        void visitRBStraight_RemoveWalls(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r,c].visited != 0)
                return;

            grid[r,c].visited = 1;

            //visitedCellsQueue.Enqueue(new MazeCell(c, r));

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else
                {
                    grid[r, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBStraight_RemoveWalls(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBStraight_RemoveWalls(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBStraight_RemoveWalls(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r + 1, c, 4);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBStraight_RemoveWalls(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c + 1, 3);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_RemoveWalls(r, c - 1, 2);
                        visitRBStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBStraight_RemoveWalls_Stack_DFS()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r--;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c++;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r++;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c--;

                        sleep = true;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                    if (d == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 4)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    if (visitedCellsStack.Count > 0)
                    {
                        r = visitedCellsStack.Peek().row;
                        c = visitedCellsStack.Peek().column;

                        d = visitedCellsStack.Peek().direction;

                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        genvisited++;

                        if (showgen == 1 && (genvisited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }

            } while (visitedCellsStack.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitRBStraight_RemoveWalls_Queue_BFS()
        {
            //sleeptime = 50;
            //genspeed = 1;
            //showgen = 1;
            //colorGenerate = colorBackground;
            //colorCurrentCell = colorBackground;

            Queue<MazeCell> visitedCellsQueueCopy = new Queue<MazeCell>();

            int backtrackCopy = backtrack;

            setBacktrack(1);

            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;

            int path = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                    //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(cell_padding, cell_padding)));

                    visitedCellsQueue.Enqueue(new MazeCell(c, r, d));

                    path = 1;

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r--;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c++;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r++;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c--;

                        sleep = true;
                    }
                }
                else if (path == 1)
                {
                    path = 3;

                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    r = visitedCellsQueue.Peek().row;
                    c = visitedCellsQueue.Peek().column;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }
                else if (path == 2)
                {
                    path = 3;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    r = visitedCellsQueue.Peek().row;
                    c = visitedCellsQueue.Peek().column;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }
                else if (path == 3)
                {
                    path = 2;

                    d = visitedCellsQueue.Peek().direction;

                    //d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                    if (d == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 4)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsQueueCopy.Enqueue(visitedCellsQueue.Dequeue());
                }

            } while (visitedCellsQueue.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            setBacktrack(backtrackCopy);

            //flood_fill_BFS(r, c);

            while (visitedCellsQueueCopy.Count > 0)
            {
                r = visitedCellsQueueCopy.Peek().row;
                c = visitedCellsQueueCopy.Peek().column;

                d = visitedCellsQueueCopy.Peek().direction;

                d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                if (d == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d == 0)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                }

                mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                visitedCellsQueueCopy.Dequeue();

                genvisited++;

                if (showgen == 1 && (genvisited % genspeed) == 0)
                    Thread.Sleep(sleeptime / 10);

                mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }
        }

        void visitRBStraight_RemoveWalls_Stack2()
        {
            int r, c, d;

            r = rng.Next(0, height);
            c = rng.Next(0, width);

            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited = 1;

            int genvisited = 0;

            visitedCellsStack.Push(new MazeCell(c, r));

            bool sleep = false;

            while (visited < (height * width))
            {
                if (sleep)
                {
                    sleep = false;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));
                        //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)(r - 1) * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (cell_padding * 2), spacer - (cell_padding * 2))));
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, spacer - cell_padding)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r - 1, c].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c, r - 1));

                        visited++;

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));
                        //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (cell_padding * 2), spacer - (cell_padding * 2))));
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - cell_padding, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r, c + 1].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c + 1, r));

                        visited++;

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));
                        //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)(r + 1) * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (cell_padding * 2), spacer - (cell_padding * 2))));
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - 2, spacer - cell_padding)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r + 1, c].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c, r + 1));

                        visited++;

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        //mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));
                        //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, spacer - 8)));

                        //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding, start_y + ((float)r * spacer) + cell_padding), new SizeF(spacer - (cell_padding * 2), spacer - (cell_padding * 2))));
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer - cell_padding, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r, c - 1].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c - 1, r));

                        visited++;

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    r = visitedCellsStack.Peek().row;
                    c = visitedCellsStack.Peek().column;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    sleep = true;
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitRBStraight_BuildWalls(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visitedCellsQueue.Enqueue(new MazeCell(c, r));

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                }
                else
                {
                    grid[r, c].top = 1;
                }
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBStraight_BuildWalls(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBStraight_BuildWalls(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBStraight_BuildWalls(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r + 1, c, 4);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBStraight_BuildWalls(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c + 1, 3);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_BuildWalls(r, c - 1, 2);
                        visitRBStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBStraight_BuildWalls_Stack()
        {

        }

        void visitRBStraight_FillCells(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBStraight_FillCells(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBStraight_FillCells(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBStraight_FillCells(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r + 1, c, 4);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBStraight_FillCells(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitRBStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c + 1, 3);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitRBStraight_FillCells(r - 1, c, 1);
                        visitRBStraight_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBStraight_FillCells(r, c - 1, 2);
                        visitRBStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBStraight_FillCells_Stack()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                    if (d == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 4)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    if (visitedCellsStack.Count > 0)
                    {
                        r = visitedCellsStack.Peek().row;
                        c = visitedCellsStack.Peek().column;

                        d = visitedCellsStack.Peek().direction;

                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        genvisited++;

                        if (showgen == 1 && (genvisited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }

            } while (visitedCellsStack.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitRBStraight_FillCells_Stack2()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;

            //mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    if (d == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer)));
                    }
                    else if (d == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    }
                    else if (d == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, spacer)));
                    }
                    else
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    }

                    if (visitedCellsStack.Count > 0)
                    {
                        r = visitedCellsStack.Peek().row;
                        c = visitedCellsStack.Peek().column;

                        d = visitedCellsStack.Peek().direction;

                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        genvisited++;

                        if (showgen == 1 && (genvisited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }

            } while (visitedCellsStack.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitRBStraight_FillCells_Stack_Original()
        {
            int r, c, d;

            r = rng.Next(0, height);
            c = rng.Next(0, width);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited = 1;

            int genvisited = 1;

            visitedCellsStack.Push(new MazeCell(c, r));

            if (showgen == 1 && (genvisited % genspeed) == 0)
                Thread.Sleep(sleeptime);

            bool sleep = false;

            while (visited < (height * width))
            {
                if (hasUnvisitedNeighbor(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r - 1, c].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c, r - 1));

                        visited++;

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r, c + 1].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c + 1, r));

                        visited++;

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        grid[r + 1, c].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c, r + 1));

                        visited++;

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                        grid[r, c - 1].visited = 1;

                        visitedCellsStack.Push(new MazeCell(c - 1, r));

                        visited++;

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    r = visitedCellsStack.Peek().row;
                    c = visitedCellsStack.Peek().column;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    sleep = true;
                }

                if (sleep)
                {
                    sleep = false;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitRBJagged_RemoveWalls(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r,c].visited != 0)
                return;

            grid[r,c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else
                {
                    grid[r, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBJagged_RemoveWalls(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitRBJagged_RemoveWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBJagged_RemoveWalls(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_RemoveWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBJagged_RemoveWalls(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_RemoveWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r + 1, c, 4);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBJagged_RemoveWalls(r + 1, c, 4);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_RemoveWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_RemoveWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c + 1, 3);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_RemoveWalls(r, c - 1, 2);
                        visitRBJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBJagged_RemoveWalls_Stack()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;
            int d_prev = 0;
            int d_tries = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    d_prev = d;

                    if (d == 0)
                        d = rng.Next(1, 5);
                    else
                        d = (rng.Next(0, 2) * 2) + (d_prev % 2) + 1;

                    d_tries = 0;

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                    else
                    {
                        d = ++d_tries == 1 ? d > 2 ? d - 2 : d + 2 : d_prev;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    d_prev = d_prev != 0 ? d_prev > 2 ? d_prev - 2 : d_prev + 2 : 0;

                    if (d_prev == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d_prev == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d_prev == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d_prev == 4)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    if (visitedCellsStack.Count > 0)
                    {
                        r = visitedCellsStack.Peek().row;
                        c = visitedCellsStack.Peek().column;

                        d = d_prev;
                        d_prev = visitedCellsStack.Peek().direction;
                        d_tries = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        genvisited++;

                        if (showgen == 1 && (genvisited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }

            } while (visitedCellsStack.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitRBJagged_BuildWalls(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visitedCellsQueue.Enqueue(new MazeCell(c, r));

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                }
                else
                {
                    grid[r, c].top = 1;
                }
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBJagged_BuildWalls(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitRBJagged_BuildWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBJagged_BuildWalls(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_BuildWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBJagged_BuildWalls(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_BuildWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r + 1, c, 4);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBJagged_BuildWalls(r + 1, c, 4);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_BuildWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_BuildWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c + 1, 3);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_BuildWalls(r, c - 1, 2);
                        visitRBJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBJagged_FillCells(int r, int c, int df)
        {
            if (visited == height * width)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitRBJagged_FillCells(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitRBJagged_FillCells(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                }
                else
                {
                    visitRBJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitRBJagged_FillCells(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_FillCells(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitRBJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitRBJagged_FillCells(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_FillCells(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r + 1, c, 4);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitRBJagged_FillCells(r + 1, c, 4);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitRBJagged_FillCells(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitRBJagged_FillCells(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c + 1, 3);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitRBJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitRBJagged_FillCells(r - 1, c, 1);
                        visitRBJagged_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitRBJagged_FillCells(r, c - 1, 2);
                        visitRBJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
        }

        void visitRBJagged_FillCells_Stack()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int d = 0;
            int d_prev = 0;
            int d_tries = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visited = 0;

            int genvisited = 0;

            bool sleep = true;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    d_prev = d;

                    if (d == 0)
                        d = rng.Next(1, 5);
                    else
                        d = (rng.Next(0, 2) * 2) + (d_prev % 2) + 1;

                    d_tries = 0;

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                        mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                    else
                    {
                        d = ++d_tries == 1 ? d > 2 ? d - 2 : d + 2 : d_prev;
                    }
                }
                else
                {
                    visitedCellsStack.Pop();

                    d_prev = d_prev != 0 ? d_prev > 2 ? d_prev - 2 : d_prev + 2 : 0;

                    if (d_prev == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d_prev == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d_prev == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d_prev == 4)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    if (visitedCellsStack.Count > 0)
                    {
                        r = visitedCellsStack.Peek().row;
                        c = visitedCellsStack.Peek().column;

                        d = d_prev;
                        d_prev = visitedCellsStack.Peek().direction;
                        d_tries = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                        genvisited++;

                        if (showgen == 1 && (genvisited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }

            } while (visitedCellsStack.Count > 0);

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitHKStraight_RemoveWalls(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r,c].visited != 0)
                return;

            grid[r,c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else
                {
                    grid[r, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKStraight_RemoveWalls(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKStraight_RemoveWalls(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKStraight_RemoveWalls(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r + 1, c, 4);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_RemoveWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKStraight_RemoveWalls(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c + 1, 3);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_RemoveWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_RemoveWalls(r, c - 1, 2);
                        visitHKStraight_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKStraight_RemoveWalls_Stack(int r, int c)
        {
            int d = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            int genvisited = 1;

            if (showgen == 1 && (genvisited % genspeed) == 0)
                Thread.Sleep(sleeptime);

            if (!hasUnvisitedNeighbor(r, c))
            {
                mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                return;
            }

            visitedCellsStack.Push(new MazeCell(c, r, d));

            d = findCell(r, c);

            bool sleep = false;

            do
            {
                d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r - 1;

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c + 1;

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r + 1;

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c - 1;

                    sleep = true;
                }

                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

            } while (hasUnvisitedNeighbor(r, c));

            while (visitedCellsStack.Count > 0)
            {
                MazeCell cell = visitedCellsStack.Pop();

                d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                if (d == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }

                r = cell.row;
                c = cell.column;

                d = cell.direction;

                mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                genvisited++;

                if (showgen == 1 && (genvisited % genspeed) == 0)
                    Thread.Sleep(sleeptime / 10);
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitHKStraight_RemoveWalls_Stack2(int r, int c)
        {
            bool sleep = true;

            bool kill = false;

            while (!kill)
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    grid[r, c].visited = 1;

                    visited++;

                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    int d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, spacer - cell_padding)));
                        
                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - cell_padding, spacer - 2)));
                        
                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - 2, spacer - cell_padding)));
                        
                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer - cell_padding, spacer - 2)));
                        
                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    kill = true;
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitHKStraight_BuildWalls(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visitedCellsQueue.Enqueue(new MazeCell(c, r));

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                }
                else
                {
                    grid[r, c].top = 1;
                }
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKStraight_BuildWalls(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKStraight_BuildWalls(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKStraight_BuildWalls(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r + 1, c, 4);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_BuildWalls(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKStraight_BuildWalls(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c + 1, 3);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_BuildWalls(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_BuildWalls(r, c - 1, 2);
                        visitHKStraight_BuildWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKStraight_FillCells(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKStraight_FillCells(r - 1, c, 1);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKStraight_FillCells(r, c - 1, 2);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKStraight_FillCells(r, c + 1, 3);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r + 1, c, 4);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_FillCells(r + 1, c, 4);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKStraight_FillCells(r + 1, c, 4);
                d = rng.Next(1, 4);
                if (d == 1)
                {
                    visitHKStraight_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKStraight_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c + 1, 3);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKStraight_FillCells(r, c + 1, 3);
                    d = rng.Next(1, 3);
                    if (d == 1)
                    {
                        visitHKStraight_FillCells(r - 1, c, 1);
                        visitHKStraight_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKStraight_FillCells(r, c - 1, 2);
                        visitHKStraight_FillCells(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKStraight_FillCells_Stack(int r, int c)
        {
            int d = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            int genvisited = 1;

            if (showgen == 1 && (genvisited % genspeed) == 0)
                Thread.Sleep(sleeptime);

            if (!hasUnvisitedNeighbor(r, c))
            {
                mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                return;
            }

            visitedCellsStack.Push(new MazeCell(c, r, d));

            d = findCell(r, c);

            bool sleep = false;

            do
            {
                d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r - 1;

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c + 1;

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r + 1;

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c - 1;

                    sleep = true;
                }

                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

            } while (hasUnvisitedNeighbor(r, c));

            while (visitedCellsStack.Count > 0)
            {
                MazeCell cell = visitedCellsStack.Pop();

                d = d != 0 ? d > 2 ? d - 2 : d + 2 : 0;

                if (d == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }

                r = cell.row;
                c = cell.column;

                d = cell.direction;

                mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                genvisited++;

                if (showgen == 1 && (genvisited % genspeed) == 0)
                    Thread.Sleep(sleeptime / 10);
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitHKStraight_FillCells_Stack2(int r, int c)
        {
            bool sleep = true;

            bool kill = false;

            while (!kill)
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    grid[r, c].visited = 1;

                    visited++;

                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    int d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    kill = true;
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitHKJagged_RemoveWalls(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r,c].visited != 0)
                return;

            grid[r,c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));
                }
                else
                {
                    grid[r, c].top = 1;
                    mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKJagged_RemoveWalls(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitHKJagged_RemoveWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKJagged_RemoveWalls(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_RemoveWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKJagged_RemoveWalls(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_RemoveWalls(r - 1, c, 1);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_RemoveWalls(r, c - 1, 2);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r + 1, c, 4);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_RemoveWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKJagged_RemoveWalls(r + 1, c, 4);
                d = rng.Next(1, 3);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_RemoveWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_RemoveWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c + 1, 3);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_RemoveWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_RemoveWalls(r, c - 1, 2);
                        visitHKJagged_RemoveWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKJagged_RemoveWalls_Stack(int r, int c)
        {
            int d = 0;
            int d_prev = 0;
            int d_tries = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            int genvisited = 1;

            if (showgen == 1 && (genvisited % genspeed) == 0)
                Thread.Sleep(sleeptime);

            if (!hasUnvisitedNeighbor(r, c))
            {
                mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                return;
            }

            visitedCellsStack.Push(new MazeCell(c, r, d));

            d = findCell(r, c);

            bool sleep = false;

            do
            {
                if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r - 1;

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c + 1;

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r + 1;

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c - 1;

                    sleep = true;
                }
                else
                {
                    d = ++d_tries == 1 ? d > 2 ? d - 2 : d + 2 : d_prev;
                }

                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    d_prev = d;

                    d = (rng.Next(0, 2) * 2) + (d_prev % 2) + 1;

                    d_tries = 0;

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

            } while (hasUnvisitedNeighbor(r, c));

            while (visitedCellsStack.Count > 0)
            {
                MazeCell cell = visitedCellsStack.Pop();

                d_prev = d_prev != 0 ? d_prev > 2 ? d_prev - 2 : d_prev + 2 : 0;

                if (d_prev == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d_prev == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d_prev == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d_prev == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }

                r = cell.row;
                c = cell.column;

                d_prev = cell.direction;

                mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                genvisited++;

                if (showgen == 1 && (genvisited % genspeed) == 0)
                    Thread.Sleep(sleeptime / 10);
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitHKJagged_BuildWalls(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visitedCellsQueue.Enqueue(new MazeCell(c, r));

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;
                }
                else
                {
                    grid[r, c].top = 1;
                }
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKJagged_BuildWalls(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitHKJagged_BuildWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKJagged_BuildWalls(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_BuildWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKJagged_BuildWalls(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_BuildWalls(r - 1, c, 1);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_BuildWalls(r, c - 1, 2);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r + 1, c, 4);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_BuildWalls(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKJagged_BuildWalls(r + 1, c, 4);
                d = rng.Next(1, 3);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_BuildWalls(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_BuildWalls(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c + 1, 3);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_BuildWalls(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_BuildWalls(r, c - 1, 2);
                        visitHKJagged_BuildWalls(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKJagged_FillCells(int r, int c, int df)
        {
            if (kill == 1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (grid[r, c].visited != 0)
                return;

            grid[r, c].visited = 1;

            visited++;

            if (df != 0)
            {
                if (df == 1)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }
                else if (df == 2)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                }
                else
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                }

                if (showgen == 1 && (visited % genspeed) == 0)
                    Thread.Sleep(sleeptime);
            }

            if (deadEnd(r, c) || (visited % hunt) == 0)
            {
                kill = 1;

                return;
            }

            int d = rng.Next(1, 5);

            while (d == df)
                d = rng.Next(1, 5);

            if (d == 1)
            {
                visitHKJagged_FillCells(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == df);
                if (d == 2)
                {
                    visitHKJagged_FillCells(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                }
                else
                {
                    visitHKJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                visitHKJagged_FillCells(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_FillCells(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == df);
                    if (d == 3)
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    visitHKJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                visitHKJagged_FillCells(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_FillCells(r - 1, c, 1);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_FillCells(r, c - 1, 2);
                    d = rng.Next(1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r + 1, c, 4);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r + 1, c, 4);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_FillCells(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
            else
            {
                visitHKJagged_FillCells(r + 1, c, 4);
                d = rng.Next(1, 3);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == df);
                if (d == 1)
                {
                    visitHKJagged_FillCells(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == df);
                    if (d == 2)
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    visitHKJagged_FillCells(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r, c + 1, 3);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c + 1, 3);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
                else
                {
                    visitHKJagged_FillCells(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == df);
                    if (d == 1)
                    {
                        visitHKJagged_FillCells(r - 1, c, 1);
                        visitHKJagged_FillCells(r, c - 1, 2);
                    }
                    else
                    {
                        visitHKJagged_FillCells(r, c - 1, 2);
                        visitHKJagged_FillCells(r - 1, c, 1);
                    }
                }
            }
        }

        void visitHKJagged_FillCells_Stack(int r, int c)
        {
            int d = 0;
            int d_prev = 0;
            int d_tries = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            int genvisited = 1;

            if (showgen == 1 && (genvisited % genspeed) == 0)
                Thread.Sleep(sleeptime);

            if (!hasUnvisitedNeighbor(r, c))
            {
                mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                return;
            }

            visitedCellsStack.Push(new MazeCell(c, r, d));

            d = findCell(r, c);

            bool sleep = false;

            do
            {
                if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r - 1;

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c + 1;

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                    r = r + 1;

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));
                    mazeCanvas.FillRectangle(new SolidBrush(colorGenerate), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                    c = c - 1;

                    sleep = true;
                }
                else
                {
                    d = ++d_tries == 1 ? d > 2 ? d - 2 : d + 2 : d_prev;
                }

                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    visitedCellsStack.Push(new MazeCell(c, r, d));

                    d_prev = d;

                    d = (rng.Next(0, 2) * 2) + (d_prev % 2) + 1;

                    d_tries = 0;

                    grid[r, c].visited = 1;

                    visited++;

                    genvisited++;

                    if (showgen == 1 && (genvisited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

            } while (hasUnvisitedNeighbor(r, c));

            while (visitedCellsStack.Count > 0)
            {
                MazeCell cell = visitedCellsStack.Pop();

                d_prev = d_prev != 0 ? d_prev > 2 ? d_prev - 2 : d_prev + 2 : 0;

                if (d_prev == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d_prev == 2)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }
                else if (d_prev == 3)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                }
                else if (d_prev == 4)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                }

                r = cell.row;
                c = cell.column;

                d_prev = cell.direction;

                mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                genvisited++;

                if (showgen == 1 && (genvisited % genspeed) == 0)
                    Thread.Sleep(sleeptime / 10);
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitHKJagged_FillCells_Stack2(int r, int c)
        {
            bool sleep = true;

            bool kill = false;

            while (!kill)
            {
                if (sleep)
                {
                    sleep = false;

                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    grid[r, c].visited = 1;

                    visited++;

                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                if (hasUnvisitedNeighbor(r, c))
                {
                    int d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].visited == 0)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].visited == 0)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].visited == 0)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].visited == 0)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    kill = true;
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void markNeighbors_RemoveWalls(int r, int c)
        {
            if (r > 0)
            {
                if (grid[r - 1, c].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (c < (width - 1))
            {
                if (grid[r, c + 1].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (r < (height - 1))
            {
                if (grid[r + 1, c].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (c > 0)
            {
                if (grid[r, c - 1].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }
        }

        void visitPrim_RemoveWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int r_prev = r;
            int c_prev = c;

            //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    //r_prev = r;
                    //c_prev = c;

                    pickCell_RemoveWalls(r, c);

                    //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    markNeighbors_RemoveWalls(r, c);

                    grid[r, c].visited = 1;

                    visited++;
                }
            }

            //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitPrim_BuildWalls()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            if (grid[r, c].visited == 0)
            {
                grid[r, c].visited = 1;

                visited++;
            }

            if (visited < (height * width))
            {
                do
                {
                    do
                    {
                        r = rng.Next(0, height);
                        c = rng.Next(0, width);

                    } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

                    pickCell_BuildWalls(r, c);

                    if (grid[r, c].visited == 0)
                    {
                        grid[r, c].visited = 1;

                        visitedCellsQueue.Enqueue(new MazeCell(c, r));

                        visited++;
                    }

                } while (visited < (height * width));
            }

            displayBuildWalls();

            //visited = 0;

            //r = rng.Next(0, height);
            //c = rng.Next(0, width);

            //for (int i = 0; i < height; i++)
            //    for (int j = 0; j < width; j++)
            //        grid[i, j].visited = 0;

            //if (grid[r, c].visited == 0)
            //{
            //    grid[r, c].visited = 1;

            //    visited++;
            //}

            //if (visited < (height * width))
            //{
            //    do
            //    {
            //        do
            //        {
            //            r = rng.Next(0, height);
            //            c = rng.Next(0, width);

            //        } while (grid[r, c].visited == 1 || emptyNeighbors(r, c));

            //        pickCell_BuildWalls2(r, c);

            //        if (grid[r, c].visited == 0)
            //        {
            //            grid[r, c].visited = 1;

            //            visited++;
            //        }

            //    } while (visited < (height * width));
            //}

            //int d;

            //bool continueLoop = true;

            //if ((height * width) > 1)
            //{
            //    while (continueLoop)
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        d = rng.Next(1, 3);

            //        if (d == 1)
            //        {
            //            if (r != 0 && grid[r, c].top == 0)
            //            {
            //                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                grid[r, c].tdrawn = 1;

            //                continueLoop = false;

            //                //if (showgen == 1 && (visited % genspeed) == 0)
            //                //    Thread.Sleep(sleeptime);
            //            }

            //            grid[r, c].tvisited = 1;
            //        }
            //        else
            //        {
            //            if (c != (width - 1) && grid[r, c].right == 0)
            //            {
            //                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                grid[r, c].rdrawn = 1;

            //                continueLoop = false;

            //                //if (showgen == 1 && (visited % genspeed) == 0)
            //                //    Thread.Sleep(sleeptime);
            //            }

            //            grid[r, c].rvisited = 1;
            //        }
            //    }

            //    do
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWallsSingleWall2(r, c))
            //        {
            //            d = rng.Next(1, 3);

            //            if (d == 1)
            //            {
            //                if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        grid[r, c].tdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //                else if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        grid[r, c].rdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //            }
            //            else if (d == 2)
            //            {
            //                if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //                else if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //            }

            //            if (grid[r, c].tvisited == 1 && grid[r, c].rvisited == 1)
            //            {
            //                if (showgen == 1 && (visited % genspeed) == 0)
            //                    Thread.Sleep(sleeptime);

            //                grid[r, c].visited = 1;

            //                visited++;
            //            }
            //        }

            //    } while (visited < (height * width));
            //}

            //------------------------------

            //int d;

            //bool continueLoop = true;

            //if ((height * width) > 1)
            //{
            //    while (continueLoop)
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        d = rng.Next(1, 3);

            //        if (d == 1)
            //        {
            //            if (r != 0 && grid[r, c].top == 0)
            //            {
            //                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                //if (showgen == 1 && (visited % genspeed) == 0)
            //                //    Thread.Sleep(sleeptime);

            //                grid[r, c].tdrawn = 1;

            //                grid[r, c].tvisited = 1;

            //                continueLoop = false;
            //            }
            //        }
            //        else
            //        {
            //            if (c != (width - 1) && grid[r, c].right == 0)
            //            {
            //                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                //if (showgen == 1 && (visited % genspeed) == 0)
            //                //    Thread.Sleep(sleeptime);

            //                grid[r, c].rdrawn = 1;

            //                grid[r, c].rvisited = 1;

            //                continueLoop = false;
            //            }
            //        }
            //    }

            //    do
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWallsSingleWall2(r, c))
            //        {
            //            d = rng.Next(1, 3);

            //            if (d == 1)
            //            {
            //                if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        grid[r, c].tdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //                else if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        grid[r, c].rdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //            }
            //            else if (d == 2)
            //            {
            //                if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        grid[r, c].rdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //                else if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        grid[r, c].tdrawn = 1;

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //            }

            //            if (grid[r, c].tvisited == 1 && grid[r, c].rvisited == 1)
            //            {
            //                if (showgen == 1 && (visited % genspeed) == 0)
            //                    Thread.Sleep(sleeptime);

            //                grid[r, c].visited = 1;

            //                visited++;
            //            }
            //        }

            //    } while (visited < (height * width));
            //}

            //------------------------------

            //r = rng.Next(0, height);
            //c = rng.Next(0, width);

            //if (r > 0 && grid[r, c].top == 0)
            //{
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //    grid[r, c].tdrawn = 1;
            //}

            //if (c < (width - 1) && grid[r, c].right == 0)
            //{
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //    grid[r, c].rdrawn = 1;
            //}

            //if (c > 0 && grid[r, c - 1].right == 0)
            //{
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //    grid[r, c - 1].rdrawn = 1;
            //}

            //if (r < (height - 1) && grid[r + 1, c].top == 0)
            //{
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)(r + 1) * spacer)));

            //    grid[r + 1, c].tdrawn = 1;
            //}

            //if (showgen == 1 && (visited % genspeed) == 0)
            //    Thread.Sleep(sleeptime);

            //grid[r, c].visited = 1;

            //visited++;

            //while (visited < (height * width))
            //{
            //    r = rng.Next(0, height);
            //    c = rng.Next(0, width);

            //    if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWalls(r, c))
            //    {
            //        if (r > 0 && grid[r, c].top == 0 && grid[r, c].tdrawn == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //            grid[r, c].tdrawn = 1;
            //        }

            //        if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].rdrawn == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //            grid[r, c].rdrawn = 1;
            //        }

            //        if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c - 1].rdrawn == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //            grid[r, c - 1].rdrawn = 1;
            //        }

            //        if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r + 1, c].tdrawn == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)(r + 1) * spacer)));

            //            grid[r + 1, c].tdrawn = 1;
            //        }

            //        if (showgen == 1 && (visited % genspeed) == 0)
            //            Thread.Sleep(sleeptime);

            //        grid[r, c].visited = 1;

            //        visited++;
            //    }
            //}

            //------------------------------

            //int d;

            //if ((height * width) > 1)
            //{
            //    r = rng.Next(0, height);
            //    c = rng.Next(0, width);

            //    d = rng.Next(1, 3);

            //    if (r != 0 && grid[r, c].top == 0)
            //    {
            //        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //        //if (showgen == 1 && (visited % genspeed) == 0)
            //        //    Thread.Sleep(sleeptime);
            //    }

            //    grid[r, c].tvisited = 1;

            //    if (c != (width - 1) && grid[r, c].right == 0)
            //    {
            //        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //        //if (showgen == 1 && (visited % genspeed) == 0)
            //        //    Thread.Sleep(sleeptime);
            //    }

            //    grid[r, c].rvisited = 1;

            //    if (showgen == 1 && (visited % genspeed) == 0)
            //        Thread.Sleep(sleeptime);

            //    grid[r, c].visited = 1;

            //    visited++;

            //    do
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWalls(r, c))
            //        {
            //            d = rng.Next(1, 3);

            //            if (d == 1)
            //            {
            //                if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //                else if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //            }
            //            else if (d == 2)
            //            {
            //                if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //                else if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //            }

            //            if (grid[r, c].tvisited == 1 && grid[r, c].rvisited == 1)
            //            {
            //                if (showgen == 1 && (visited % genspeed) == 0)
            //                    Thread.Sleep(sleeptime);

            //                grid[r, c].visited = 1;

            //                visited++;
            //            }
            //        }

            //    } while (visited < (height * width));
            //}

            //------------------------------

            //int d;

            //if ((height * width) > 1)
            //{
            //    r = rng.Next(0, height);
            //    c = rng.Next(0, width);

            //    d = rng.Next(1, 3);

            //    if (d == 1)
            //    {
            //        if (r != 0 && grid[r, c].top == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //            //if (showgen == 1 && (visited % genspeed) == 0)
            //            //    Thread.Sleep(sleeptime);
            //        }

            //        grid[r, c].tvisited = 1;
            //    }
            //    else
            //    {
            //        if (c != (width - 1) && grid[r, c].right == 0)
            //        {
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //            //if (showgen == 1 && (visited % genspeed) == 0)
            //            //    Thread.Sleep(sleeptime);
            //        }

            //        grid[r, c].rvisited = 1;
            //    }

            //    do
            //    {
            //        r = rng.Next(0, height);
            //        c = rng.Next(0, width);

            //        if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWallsSingleWall(r, c))
            //        {
            //            d = rng.Next(1, 3);

            //            if (d == 1)
            //            {
            //                if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //                else if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //            }
            //            else if (d == 2)
            //            {
            //                if (grid[r, c].rvisited == 0)
            //                {
            //                    if (c != (width - 1) && grid[r, c].right == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].rvisited = 1;
            //                }
            //                else if (grid[r, c].tvisited == 0)
            //                {
            //                    if (r != 0 && grid[r, c].top == 0)
            //                    {
            //                        mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //                        //if (showgen == 1 && (visited % genspeed) == 0)
            //                        //    Thread.Sleep(sleeptime);
            //                    }

            //                    grid[r, c].tvisited = 1;
            //                }
            //            }

            //            if (grid[r, c].tvisited == 1 && grid[r, c].rvisited == 1)
            //            {
            //                if (showgen == 1 && (visited % genspeed) == 0)
            //                    Thread.Sleep(sleeptime);

            //                grid[r, c].visited = 1;

            //                visited++;
            //            }
            //        }

            //    } while (visited < (height * width));
            //}

            //------------------------------

            //r = rng.Next(0, height);
            //c = rng.Next(0, width);

            //if (r != 0 && grid[r, c].top == 0)
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //if (c != (width - 1) && grid[r, c].right == 0)
            //    mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //if (showgen == 1 && (visited % genspeed) == 0)
            //    Thread.Sleep(sleeptime);

            //grid[r, c].visited = 1;

            //visited++;

            //while (visited < (height * width))
            //{
            //    r = rng.Next(0, height);
            //    c = rng.Next(0, width);

            //    if (grid[r, c].visited == 0 && !emptyNeighborsPrim_BuildWalls(r, c))
            //    {
            //        if (r != 0 && grid[r, c].top == 0)
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //        if (c != (width - 1) && grid[r, c].right == 0)
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //        if (showgen == 1 && (visited % genspeed) == 0)
            //            Thread.Sleep(sleeptime);

            //        grid[r, c].visited = 1;

            //        visited++;
            //    }
            //}
        }

        void markNeighbors_FillCells(int r, int c)
        {
            if (r > 0)
            {
                if (grid[r - 1, c].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (c < (width - 1))
            {
                if (grid[r, c + 1].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (r < (height - 1))
            {
                if (grid[r + 1, c].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }

            if (c > 0)
            {
                if (grid[r, c - 1].visited == 0)
                    mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
            }
        }

        void visitPrim_FillCells()
        {
            int r = rng.Next(0, height);
            int c = rng.Next(0, width);

            int r_prev = r;
            int c_prev = c;

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));
            //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].visited = 1;

            visited++;

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0 && hasVisitedNeighbor(r, c))
                {
                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);

                    //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    //r_prev = r;
                    //c_prev = c;

                    pickCell_FillCells(r, c);

                    //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    markNeighbors_FillCells(r, c);

                    grid[r, c].visited = 1;

                    visited++;
                }
            }

            //mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void flood_fill(int r, int c, int oi, int ni)
        {
            if (r < 0 || r == height || c < 0 || c == width)
                return;

            if (grid[r, c].identity != oi)
                return;

            grid[r, c].identity = ni;

            flood_fill(r, c - 1, oi, ni);
            flood_fill(r - 1, c, oi, ni);
            flood_fill(r, c + 1, oi, ni);
            flood_fill(r + 1, c, oi, ni);
        }

        void flood_fill_color(int r, int c, int oi, int ni, Color newColor)
        {
            if (r < 0 || r == height || c < 0 || c == width)
                return;

            if (grid[r, c].identity != oi)
                return;

            if (grid[r, c].visited == 0)
            {
                visitedCellsQueue.Enqueue(new MazeCell(c, r));
                grid[r, c].visited = 1;
            }

            grid[r, c].identity = ni;
            grid[r, c].color = newColor;

            mazeCanvas.FillRectangle(new SolidBrush(grid[r, c].color), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, spacer - 2)));
            //mazeCanvas.DrawString(ni.ToString(), new Font("Arial", spacer / 4), new SolidBrush(colorForeground), new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2));

            flood_fill_color(r, c - 1, oi, ni, newColor);
            flood_fill_color(r - 1, c, oi, ni, newColor);
            flood_fill_color(r, c + 1, oi, ni, newColor);
            flood_fill_color(r + 1, c, oi, ni, newColor);
        }

        void flood_fill_Stack(int r, int c, int oi, int ni)
        {
            mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            visitedCellsStack.Push(new MazeCell(c, r));

            bool sleep = false;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    if (showgen == 1 && ((visitedCellsStack.Count - 1) % genspeed) == 0)
                        ;//Thread.Sleep(sleeptime);
                }

                r = visitedCellsStack.Peek().row;
                c = visitedCellsStack.Peek().column;

                grid[r, c].identity = ni;

                if (r > 0 && grid[r - 1, c].identity == oi && grid[r, c].top == 1)
                {
                    visitedCellsStack.Push(new MazeCell(c, r - 1));

                    sleep = true;
                }

                if (c < (width - 1) && grid[r, c + 1].identity == oi && grid[r, c].right == 1)
                {
                    visitedCellsStack.Push(new MazeCell(c + 1, r));

                    sleep = true;
                }

                if (r < (height - 1) && grid[r + 1, c].identity == oi && grid[r + 1, c].top == 1)
                {
                    visitedCellsStack.Push(new MazeCell(c, r + 1));

                    sleep = true;
                }

                if (c > 0 && grid[r, c - 1].identity == oi && grid[r, c - 1].right == 1)
                {
                    visitedCellsStack.Push(new MazeCell(c - 1, r));

                    sleep = true;
                }

                if (!sleep)
                    visitedCellsStack.Pop();

            } while (visitedCellsStack.Count > 0);
        }

        void flood_fill_Stack_Color(int r, int c, int oi, int ni, Color newColor)
        {
            visitedCellsStack.Push(new MazeCell(c, r));

            bool sleep = false;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    if (showgen == 1 && ((visitedCellsStack.Count - 1) % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                r = visitedCellsStack.Peek().row;
                c = visitedCellsStack.Peek().column;

                grid[r, c].identity = ni;
                grid[r, c].color = newColor;

                if (r > 0 && grid[r - 1, c].identity == oi && grid[r, c].top == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(newColor), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));

                    visitedCellsStack.Push(new MazeCell(c, r - 1));

                    sleep = true;
                }

                if (c < (width - 1) && grid[r, c + 1].identity == oi && grid[r, c].right == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(newColor), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    visitedCellsStack.Push(new MazeCell(c + 1, r));

                    sleep = true;
                }

                if (r < (height - 1) && grid[r + 1, c].identity == oi && grid[r + 1, c].top == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(newColor), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));

                    visitedCellsStack.Push(new MazeCell(c, r + 1));

                    sleep = true;
                }

                if (c > 0 && grid[r, c - 1].identity == oi && grid[r, c - 1].right == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(newColor), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    visitedCellsStack.Push(new MazeCell(c - 1, r));

                    sleep = true;
                }

                if (!sleep)
                    visitedCellsStack.Pop();

            } while (visitedCellsStack.Count > 0);
        }

        void flood_fill_BFS(int r, int c)
        {
            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, spacer - 2)));

            visitedCellsStack.Push(new MazeCell(c, r));

            bool sleep = false;

            do
            {
                if (sleep)
                {
                    sleep = false;

                    if (showgen == 1 && ((visitedCellsStack.Count - 1) % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                r = visitedCellsStack.Peek().row;
                c = visitedCellsStack.Peek().column;

                grid[r, c].visited = 0;

                if (r > 0 && grid[r - 1, c].visited == 1 && grid[r, c].top == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, spacer)));

                    visitedCellsStack.Push(new MazeCell(c, r - 1));

                    sleep = true;
                }

                if (c < (width - 1) && grid[r, c + 1].visited == 1 && grid[r, c].right == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    visitedCellsStack.Push(new MazeCell(c + 1, r));

                    sleep = true;
                }

                if (r < (height - 1) && grid[r + 1, c].visited == 1 && grid[r + 1, c].top == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, spacer)));

                    visitedCellsStack.Push(new MazeCell(c, r + 1));

                    sleep = true;
                }

                if (c > 0 && grid[r, c - 1].visited == 1 && grid[r, c - 1].right == 1)
                {
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer, spacer - 2)));

                    visitedCellsStack.Push(new MazeCell(c - 1, r));

                    sleep = true;
                }

                if (!sleep)
                    visitedCellsStack.Pop();

            } while (visitedCellsStack.Count > 0);
        }

        void visitKruskal_RemoveWalls()
        {
            int r, c, d;

            while (visited < ((height * width) - 1))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                d = rng.Next(1, 5);

                if (d == 1)
                {
                    if (r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                        flood_fill(r, c, grid[r, c].identity, grid[r - 1, c].identity);

                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 2)
                {
                    if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        flood_fill(r, c, grid[r, c].identity, grid[r, c - 1].identity);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 3)
                {
                    if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        flood_fill(r, c, grid[r, c].identity, grid[r, c + 1].identity);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else
                {
                    if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                        flood_fill(r, c, grid[r, c].identity, grid[r + 1, c].identity);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
            }
        }

        void visitKruskal_RemoveWalls_Color()
        {
            int r, c, d;

            while (visited < ((height * width) - 1))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);
                
                d = rng.Next(1, 5);

                if (d == 1)
                {
                    if (r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)r * spacer)));

                        flood_fill_color(r, c, grid[r, c].identity, grid[r - 1, c].identity, grid[r - 1, c].color);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 2)
                {
                    if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)(c - 1) * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        flood_fill_color(r, c, grid[r, c].identity, grid[r, c - 1].identity, grid[r, c - 1].color);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 3)
                {
                    if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + 1), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer - 1));

                        flood_fill_color(r, c, grid[r, c].identity, grid[r, c + 1].identity, grid[r, c + 1].color);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else
                {
                    if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.DrawLine(penBackground, new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r + 1) * spacer)), new PointF(start_x + ((float)c * spacer) + spacer - 1, start_y + ((float)(r + 1) * spacer)));

                        flood_fill_color(r, c, grid[r, c].identity, grid[r + 1, c].identity, grid[r + 1, c].color);
                        
                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
            }

            //MazeCell[] visitedSquares = visitedCellsQueue.ToArray();

            mazeCanvas.DrawString(visitedCellsQueue.Count.ToString(), new Font("Arial", spacer / 4), new SolidBrush(colorForeground), new PointF(start_x, start_y / 2));
            //mazeCanvas.DrawString(visitedSquares.Length.ToString(), new Font("Arial", spacer / 4), new SolidBrush(colorForeground), new PointF(start_x, start_y / 2));

            //for (r = 0; r < height; r++)
            //{
            //    for (c = 0; c < width; c++)
            //    {
            //        if (grid[r, c].visited == 1)
            //            mazeCanvas.FillRectangle(new SolidBrush(Color.Aqua), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, spacer - 2)));
            //        else
            //            mazeCanvas.FillRectangle(new SolidBrush(Color.Coral), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, spacer - 2)));
            //    }
            //}

            //r = visitedSquares[0].row;
            //c = visitedSquares[0].column;

            //mazeCanvas.FillRectangle(new SolidBrush(colorCurrentCell), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, spacer - 2)));

            //r = visitedSquares[visitedSquares.Length-1].row;
            //c = visitedSquares[visitedSquares.Length-1].column;

            //mazeCanvas.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, spacer - 2)));
        }

        void visitKruskal_RemoveWalls_Stack()
        {
            int r = 0, c = 0, r_prev = 0, c_prev = 0, d;

            bool sleep = false;

            visited = 0;

            while (visited < ((height * width) - 1))
            {
                if (sleep)
                {
                    sleep = false;

                    r_prev = r;
                    c_prev = c;

                    visited++;

                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                r = rng.Next(0, height);
                c = rng.Next(0, width);

                d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                    
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer)), new SizeF(spacer - 2, 2)));

                    flood_fill_Stack(r, c, grid[r, c].identity, grid[r - 1, c].identity);

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));

                    flood_fill_Stack(r, c, grid[r, c].identity, grid[r, c + 1].identity);

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                    
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r + 1) * spacer)), new SizeF(spacer - 2, 2)));

                    flood_fill_Stack(r, c, grid[r, c].identity, grid[r + 1, c].identity);

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
                    
                    mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer) + 2), new SizeF(2, spacer - 2)));

                    flood_fill_Stack(r, c, grid[r, c].identity, grid[r, c - 1].identity);

                    sleep = true;
                }
            }

            mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c_prev * spacer) + cell_padding + 2, start_y + ((float)r_prev * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));
        }

        void visitKruskal_BuildWalls()
        {
            int r, c, d;

            while (visited < ((height * width) - 1))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                d = rng.Next(1, 5);

                if (d == 1)
                {
                    if (r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                    {
                        grid[r, c].top = 1;

                        flood_fill(r, c, grid[r, c].identity, grid[r - 1, c].identity);

                        visited++;
                    }
                }
                else if (d == 2)
                {
                    if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                    {
                        grid[r, c - 1].right = 1;

                        flood_fill(r, c, grid[r, c].identity, grid[r, c - 1].identity);

                        visited++;
                    }
                }
                else if (d == 3)
                {
                    if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                    {
                        grid[r, c].right = 1;

                        flood_fill(r, c, grid[r, c].identity, grid[r, c + 1].identity);

                        visited++;
                    }
                }
                else
                {
                    if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                    {
                        grid[r + 1, c].top = 1;

                        flood_fill(r, c, grid[r, c].identity, grid[r + 1, c].identity);

                        visited++;
                    }
                }
            }

            visited = 0;

            while (visited < (height * width))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                if (grid[r, c].visited == 0)
                {
                    d = rng.Next(1, 3);

                    if (d == 1)
                    {
                        if (grid[r, c].tvisited == 0)
                        {
                            if (r > 0 && grid[r, c].top == 0)
                            {
                                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

                                //if (showgen == 1 && (visited % genspeed) == 0)
                                //    Thread.Sleep(sleeptime);
                            }

                            grid[r, c].tvisited = 1;
                        }
                        else if (grid[r, c].rvisited == 0)
                        {
                            if (c < (width - 1) && grid[r, c].right == 0)
                            {
                                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

                                //if (showgen == 1 && (visited % genspeed) == 0)
                                //    Thread.Sleep(sleeptime);
                            }

                            grid[r, c].rvisited = 1;
                        }
                    }
                    else if (d == 2)
                    {
                        if (grid[r, c].rvisited == 0)
                        {
                            if (c < (width - 1) && grid[r, c].right == 0)
                            {
                                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

                                //if (showgen == 1 && (visited % genspeed) == 0)
                                //    Thread.Sleep(sleeptime);
                            }

                            grid[r, c].rvisited = 1;
                        }
                        else if (grid[r, c].tvisited == 0)
                        {
                            if (r > 0 && grid[r, c].top == 0)
                            {
                                mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

                                //if (showgen == 1 && (visited % genspeed) == 0)
                                //    Thread.Sleep(sleeptime);
                            }

                            grid[r, c].tvisited = 1;
                        }
                    }

                    if (grid[r, c].tvisited == 1 && grid[r, c].rvisited == 1)
                    {
                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);

                        grid[r, c].visited = 1;

                        visited++;
                    }
                }
            }

            //------------------------------

            //while (visited < (height * width))
            //{
            //    r = rng.Next(0, height);
            //    c = rng.Next(0, width);

            //    if (grid[r, c].visited == 0)
            //    {
            //        if (r != 0 && grid[r, c].top == 0)
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer), start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)));

            //        if (c != (width - 1) && grid[r, c].right == 0)
            //            mazeCanvas.DrawLine(penForeground, new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer)), new PointF(start_x + ((float)c * spacer) + spacer, start_y + ((float)r * spacer) + spacer));

            //        if (showgen == 1 && (visited % genspeed) == 0)
            //            Thread.Sleep(sleeptime);

            //        grid[r, c].visited = 1;

            //        visited++;
            //    }
            //}
        }

        void visitKruskal_FillCells()
        {
            int r, c, d;

            while (visited < ((height * width) - 1))
            {
                r = rng.Next(0, height);
                c = rng.Next(0, width);

                d = rng.Next(1, 5);

                if (d == 1)
                {
                    if (r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                    {
                        grid[r, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)(r - 1) * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));

                        flood_fill(r, c, grid[r, c].identity, grid[r - 1, c].identity);

                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 2)
                {
                    if (c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                    {
                        grid[r, c - 1].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                        
                        flood_fill(r, c, grid[r, c].identity, grid[r, c - 1].identity);

                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else if (d == 3)
                {
                    if (c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                    {
                        grid[r, c].right = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF((spacer * 2) - 2, spacer - 2)));
                        
                        flood_fill(r, c, grid[r, c].identity, grid[r, c + 1].identity);

                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
                else
                {
                    if (r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                    {
                        grid[r + 1, c].top = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorBackground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 1, start_y + ((float)r * spacer) + 1), new SizeF(spacer - 2, (spacer * 2) - 2)));
                        
                        flood_fill(r, c, grid[r, c].identity, grid[r + 1, c].identity);

                        visited++;

                        if (showgen == 1 && (visited % genspeed) == 0)
                            Thread.Sleep(sleeptime);
                    }
                }
            }
        }

        void visitKruskal_FillCells_Stack()
        {
            int r = 0;
            int c = 0;
            int d = 0;

            bool sleep = false;

            visited = 0;

            while (visited < ((height * width) - 1))
            {
                if (sleep)
                {
                    sleep = false;

                    visited++;

                    if (showgen == 1 && (visited % genspeed) == 0)
                        Thread.Sleep(sleeptime);
                }

                r = rng.Next(0, height);
                c = rng.Next(0, width);

                d = rng.Next(1, 5);

                if (d == 1 && r > 0 && grid[r, c].top == 0 && grid[r, c].identity != grid[r - 1, c].identity)
                {
                    grid[r, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(grid[r - 1, c].color), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)(r - 1) * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                    flood_fill_Stack_Color(r, c, grid[r, c].identity, grid[r - 1, c].identity, grid[r - 1, c].color);

                    sleep = true;
                }
                else if (d == 2 && c < (width - 1) && grid[r, c].right == 0 && grid[r, c].identity != grid[r, c + 1].identity)
                {
                    grid[r, c].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(grid[r, c + 1].color), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                    flood_fill_Stack_Color(r, c, grid[r, c].identity, grid[r, c + 1].identity, grid[r, c + 1].color);

                    sleep = true;
                }
                else if (d == 3 && r < (height - 1) && grid[r + 1, c].top == 0 && grid[r, c].identity != grid[r + 1, c].identity)
                {
                    grid[r + 1, c].top = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(grid[r + 1, c].color), new RectangleF(new PointF(start_x + ((float)c * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF(spacer - 2, (spacer * 2) - 2)));

                    flood_fill_Stack_Color(r, c, grid[r, c].identity, grid[r + 1, c].identity, grid[r + 1, c].color);

                    sleep = true;
                }
                else if (d == 4 && c > 0 && grid[r, c - 1].right == 0 && grid[r, c].identity != grid[r, c - 1].identity)
                {
                    grid[r, c - 1].right = 1;

                    mazeCanvas.FillRectangle(new SolidBrush(grid[r, c - 1].color), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 2, start_y + ((float)r * spacer) + 2), new SizeF((spacer * 2) - 2, spacer - 2)));

                    flood_fill_Stack_Color(r, c, grid[r, c].identity, grid[r, c - 1].identity, grid[r, c - 1].color);

                    sleep = true;
                }
            }

            colorBacktrack = grid[r, c].color;
        }

        void solveRB(int r, int c, int df)
        {
            if (kill == -1)
                return;

            if (r < 0 || r > (height - 1) || c < 0 || c > (width - 1))
                return;

            if (df != 0 || !(startSolve.Y == r && startSolve.X == c))
            {
                if (df == 1)
                {
                    if (grid[r + 1,c].top == 0)
                        return;
                }
                else if (df == 2)
                {
                    if (grid[r,c].right == 0)
                        return;
                }
                else if (df == 3)
                {
                    if (grid[r,c - 1].right == 0)
                        return;
                }
                else
                {
                    if (grid[r,c].top == 0)
                        return;
                }
            }

            if (grid[r, c].solution == 2)
                return;

            kill++;

            if (grid[r, c].solution == 1)
            {
                if (df == 1)
                {
                    grid[r + 1, c].solution = 2;
                    grid[r + 1, c].tsolve = 2;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, (spacer * 2) - 8)));
                }
                else if (df == 2)
                {
                    grid[r, c + 1].solution = 2;
                    grid[r, c].rsolve = 2;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF((spacer * 2) - 8, spacer - 8)));
                }
                else if (df == 3)
                {
                    grid[r, c - 1].solution = 2;
                    grid[r, c - 1].rsolve = 2;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF((spacer * 2) - 8, spacer - 8)));
                }
                else
                {
                    grid[r - 1, c].solution = 2;
                    grid[r, c].tsolve = 2;

                    mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)(r - 1) * spacer) + 4), new SizeF(spacer - 8, (spacer * 2) - 8)));
                }

                //if (showsolve == 1 && (kill % solvespeed) == 0)
                //    Thread.Sleep(sleeptime);

                //if (backtrack == 1)
                //    kill++;

                return;
            }
            else
            {
                grid[r, c].solution = 1;

                if (df != 0)
                {
                    if (df == 1)
                    {
                        grid[r + 1, c].tsolve = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF(spacer - 8, (spacer * 2) - 8)));
                    }
                    else if (df == 2)
                    {
                        grid[r, c].rsolve = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF((spacer * 2) - 8, spacer - 8)));
                    }
                    else if (df == 3)
                    {
                        grid[r, c - 1].rsolve = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + 4, start_y + ((float)r * spacer) + 4), new SizeF((spacer * 2) - 8, spacer - 8)));
                    }
                    else
                    {
                        grid[r, c].tsolve = 1;

                        mazeCanvas.FillRectangle(new SolidBrush(colorForeground), new RectangleF(new PointF(start_x + ((float)c * spacer) + 4, start_y + ((float)(r - 1) * spacer) + 4), new SizeF(spacer - 8, (spacer * 2) - 8)));
                    }

                    //if (kill == 2)
                    //    displayStart();

                    //if (showsolve == 1 && (kill % solvespeed) == 0)
                    //    Thread.Sleep(dleeptime);
                }
            }

            //grid[r,c].tru = df;

            if (stopSolve.Y == r && stopSolve.X == c)
            {
                kill = -1;

                return;
            }

            //kill++;

            //if (showsolve == 1 && (kill % solvespeed) == 0)
            //    Thread.Sleep(sleeptime);

            int d = rng.Next(1, 5);

            while (d == (5 - df))
                d = rng.Next(1, 5);

            if (d == 1)
            {
                solveRB(r - 1, c, 1);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 1)
                        d = 4;
                } while (d == (5 - df));
                if (d == 2)
                {
                    solveRB(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 3)
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    solveRB(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 2)
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r, c - 1, 2);
                    }
                }
                else
                {
                    solveRB(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == (5 - df));
                    if (d == 2)
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r, c + 1, 3);
                    }
                    else
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r, c - 1, 2);
                    }
                }
            }
            else if (d == 2)
            {
                solveRB(r, c - 1, 2);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 2)
                        d = 4;
                } while (d == (5 - df));
                if (d == 1)
                {
                    solveRB(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                        else
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 3)
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r, c + 1, 3);
                    }
                }
                else if (d == 3)
                {
                    solveRB(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r - 1, c, 1);
                    }
                }
                else
                {
                    solveRB(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r, c + 1, 3);
                    }
                    else
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r - 1, c, 1);
                    }
                }
            }
            else if (d == 3)
            {
                solveRB(r, c + 1, 3);
                do
                {
                    d = rng.Next(1, 4);
                    if (d == 3)
                        d = 4;
                } while (d == (5 - df));
                if (d == 1)
                {
                    solveRB(r - 1, c, 1);
                    d = rng.Next(1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 2)
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    solveRB(r, c - 1, 2);
                    d = rng.Next(1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 4;
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r + 1, c, 4);
                    }
                    else
                    {
                        solveRB(r + 1, c, 4);
                        solveRB(r - 1, c, 1);
                    }
                }
                else
                {
                    solveRB(r + 1, c, 4);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r, c - 1, 2);
                    }
                    else
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r - 1, c, 1);
                    }
                }
            }
            else
            {
                solveRB(r + 1, c, 4);
                d = rng.Next(1, 4);
                do
                {
                    d = rng.Next(1, 4);
                } while (d == (5 - df));
                if (d == 1)
                {
                    solveRB(r - 1, c, 1);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 1)
                            d = 3;
                    } while (d == (5 - df));
                    if (d == 2)
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r, c + 1, 3);
                    }
                    else
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r, c - 1, 2);
                    }
                }
                else if (d == 2)
                {
                    solveRB(r, c - 1, 2);
                    do
                    {
                        d = rng.Next(1, 3);
                        if (d == 2)
                            d = 3;
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r, c + 1, 3);
                    }
                    else
                    {
                        solveRB(r, c + 1, 3);
                        solveRB(r - 1, c, 1);
                    }
                }
                else
                {
                    solveRB(r, c + 1, 3);
                    do
                    {
                        d = rng.Next(1, 3);
                    } while (d == (5 - df));
                    if (d == 1)
                    {
                        solveRB(r - 1, c, 1);
                        solveRB(r, c - 1, 2);
                    }
                    else
                    {
                        solveRB(r, c - 1, 2);
                        solveRB(r - 1, c, 1);
                    }
                }
            }
        }

        bool hasUnvisitedNeighbor_Solve(int r, int c)
        {
            if (r > 0 && grid[r - 1, c].solution == 0 && grid[r, c].top == 1)
                return true;

            if (c < (width - 1) && grid[r, c + 1].solution == 0 && grid[r, c].right == 1)
                return true;

            if (r < (height - 1) && grid[r + 1, c].solution == 0 && grid[r + 1, c].top == 1)
                return true;

            if (c > 0 && grid[r, c - 1].solution == 0 && grid[r, c - 1].right == 1)
                return true;

            return false;
        }

        void solveRB_Stack()
        {
            int r = startSolve.Y;
            int c = startSolve.X;

            int d = 0;

            mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer - (cell_padding * 2) - 2)));

            grid[r, c].solution = 1;

            visitedCellsStack.Push(new MazeCell(c, r, 0));

            int solvevisited = 1;

            bool sleep = false;

            while (!(r == stopSolve.Y && c == stopSolve.X))
            {
                if (hasUnvisitedNeighbor_Solve(r, c))
                {
                    d = rng.Next(1, 5);

                    if (d == 1 && r > 0 && grid[r - 1, c].solution == 0 && grid[r, c].top == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r - 1) * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                        
                        grid[r - 1, c].solution = 1;

                        visitedCellsStack.Push(new MazeCell(c, r - 1, d));

                        r = r - 1;

                        sleep = true;
                    }
                    else if (d == 2 && c < (width - 1) && grid[r, c + 1].solution == 0 && grid[r, c].right == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)(c + 1) * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                        
                        grid[r, c + 1].solution = 1;

                        visitedCellsStack.Push(new MazeCell(c + 1, r, d));

                        c = c + 1;

                        sleep = true;
                    }
                    else if (d == 3 && r < (height - 1) && grid[r + 1, c].solution == 0 && grid[r + 1, c].top == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)(r + 1) * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));

                        grid[r + 1, c].solution = 1;

                        visitedCellsStack.Push(new MazeCell(c, r + 1, d));

                        r = r + 1;

                        sleep = true;
                    }
                    else if (d == 4 && c > 0 && grid[r, c - 1].solution == 0 && grid[r, c - 1].right == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorSolve), new RectangleF(new PointF(start_x + ((float)(c - 1) * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));

                        grid[r, c - 1].solution = 1;

                        visitedCellsStack.Push(new MazeCell(c - 1, r, d));

                        c = c - 1;

                        sleep = true;
                    }
                }
                else
                {
                    grid[r, c].solution = 2;

                    if (d == 1)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else if (d == 2)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) - cell_padding, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }
                    else if (d == 3)
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) - cell_padding), new SizeF(spacer - (cell_padding * 2) - 2, spacer)));
                    }
                    else
                    {
                        mazeCanvas.FillRectangle(new SolidBrush(colorBacktrack), new RectangleF(new PointF(start_x + ((float)c * spacer) + cell_padding + 2, start_y + ((float)r * spacer) + cell_padding + 2), new SizeF(spacer, spacer - (cell_padding * 2) - 2)));
                    }

                    visitedCellsStack.Pop();

                    r = visitedCellsStack.Peek().row;
                    c = visitedCellsStack.Peek().column;

                    d = visitedCellsStack.Peek().direction;

                    sleep = true;
                }

                if (sleep)
                {
                    sleep = false;

                    if ((r == startSolve.Y && c == startSolve.X) ||
                        ((r + 1) == startSolve.Y && c == startSolve.X) ||
                        ((r - 1) == startSolve.Y && c == startSolve.X) ||
                        (r == startSolve.Y && (c + 1) == startSolve.X) ||
                        (r == startSolve.Y && (c - 1) == startSolve.X))
                    {
                        displayStart();
                    }

                    solvevisited++;

                    if (showsolve == 1 && (solvevisited % solvespeed) == 0)
                        Thread.Sleep(sleeptime);
                }
            }

            displayStart();
        }

        public void GenerateGrid()
        {
            initializeGrid();

            if (algorithmType == 0)
            {
                displayGrid();

                if (inverse == 0)
                {
                    colorCurrentCell = Color.Blue;
                    colorGenerate = Color.LightBlue;
                }
                else
                {
                    colorCurrentCell = Color.Red;
                    colorGenerate = Color.Pink;
                }
            }
            else if (algorithmType == 1)
            {
                displayOutline();

                if (inverse == 0)
                {
                    colorCurrentCell = Color.Blue;
                    colorGenerate = Color.LightBlue;
                }
                else
                {
                    colorCurrentCell = Color.Red;
                    colorGenerate = Color.Pink;
                }
            }
            else
            {
                displayBlock();

                colorCurrentCell = Color.Red;
                colorGenerate = Color.Pink;
            }

            displayGridFlag = false;
        }

        public void GenerateMaze()
        {
            if (displayGridFlag)
            {
                GenerateGrid();
            }

            //cell_padding = 0;

            stopwatch.Reset();

            stopwatch.Start();

            if (algorithm == "Recursive Backtracker")
            {
                if (algorithmType == 0)
                {
                    generateRBStraight_RemoveWalls();
                }
                else if (algorithmType == 1)
                {
                    generateRBStraight_BuildWalls();
                }
                else
                {
                    generateRBStraight_FillCells();
                }
            }
            else if (algorithm == "Recursive Backtracker - Jagged")
            {
                if (algorithmType == 0)
                {
                    generateRBJagged_RemoveWalls();
                }
                else if (algorithmType == 1)
                {
                    generateRBJagged_BuildWalls();
                }
                else
                {
                    generateRBJagged_FillCells();
                }
            }
            else if (algorithm == "Hunt and Kill")
            {
                if (algorithmType == 0)
                {
                    generateHKStraight_RemoveWalls_Stack();
                }
                else if (algorithmType == 1)
                {
                    generateHKStraight_BuildWalls();
                }
                else
                {
                    generateHKStraight_FillCells_Stack();
                }
            }
            else if (algorithm == "Hunt and Kill - Jagged")
            {
                if (algorithmType == 0)
                {
                    generateHKJagged_RemoveWalls_Stack();
                }
                else if (algorithmType == 1)
                {
                    generateHKJagged_BuildWalls();
                }
                else
                {
                    generateHKJagged_FillCells_Stack();
                }
            }
            else if (algorithm == "Prim's Algorithm")
            {
                cell_padding = 0;

                if (algorithmType == 0)
                {
                    generatePrim_RemoveWalls();
                }
                else if (algorithmType == 1)
                {
                    generatePrim_BuildWalls();
                }
                else
                {
                    generatePrim_FillCells();
                }
            }
            else if (algorithm == "Kruskal's Algorithm")
            {
                if (algorithmType == 0)
                {
                    generateKruskal_RemoveWalls();
                }
                else if (algorithmType == 1)
                {
                    generateKruskal_BuildWalls();
                }
                else
                {
                    generateKruskal_FillCells();
                }
            }

            stopwatch.Stop();

            //mazeCanvas.DrawString(stopwatch.ElapsedMilliseconds.ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(50, 10));

            calculateCoords();

            findStart();

            displayStart();

            //mazeCanvas.DrawString(cell_padding.ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(150, 10));
            //mazeCanvas.DrawString((spacer - (cell_padding * 2) - 2).ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(450, 10));
            //mazeCanvas.DrawString(spacer.ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(200, 10));
            
            if (visitedCellsStack.Count > 0)
                visitedCellsStack.Clear();

            if (visitedCellsQueue.Count > 0)
                visitedCellsQueue.Clear();

            displayGridFlag = true;
        }

        public void SolveMaze()
        {
            stopwatch.Reset();

            stopwatch.Start();

            solveRB_Stack();

            stopwatch.Stop();

            //mazeCanvas.DrawString(stopwatch.ElapsedMilliseconds.ToString(), new Font("Arial", 12), new SolidBrush(colorForeground), new PointF(100, 10));

            if (visitedCellsStack.Count > 0)
                visitedCellsStack.Clear();

            if (visitedCellsQueue.Count > 0)
                visitedCellsQueue.Clear();
        }
    }
}
