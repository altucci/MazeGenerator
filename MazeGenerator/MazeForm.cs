using System.Reflection;
using System.Runtime.InteropServices;

namespace MazeGenerator
{
    public partial class MazeForm : Form
    {
        Maze maze;

        bool drawGrid = false;

        public MazeForm()
        {
            InitializeComponent();
        }

        private void MazeForm_Load(object sender, EventArgs e)
        {
            maze = new Maze(this.panel1.CreateGraphics(), this.BackColor);

            int form_x_padding = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            int form_y_padding = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;

            this.Location = new Point(form_x_padding - 10, form_y_padding - 24);

            this.panel1.Location = new Point(0, 0);
            this.panel2.Width = 223;

            txtHeight.Text = maze.getHeight().ToString();
            txtWidth.Text = maze.getWidth().ToString();

            cboxAlgorithm.SelectedIndex = 0;

            tbarGenerateSpeed.Minimum = 0;
            tbarGenerateSpeed.Maximum = 90;
            tbarGenerateSpeed.TickFrequency = 10;

            tbarSolveSpeed.Minimum = 0;
            tbarSolveSpeed.Maximum = 90;
            tbarSolveSpeed.TickFrequency = 10;

            tbarGenerateSpeed.Value = maze.getGenspeed();
            tbarSolveSpeed.Value = maze.getSolvespeed();

            initializeMaze();

            drawGrid = true;
        }

        private void MazeForm_Resize(object sender, EventArgs e)
        {
            initializeMaze();

            maze.GenerateGrid();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (drawGrid)
            {
                drawGrid = false;

                maze.GenerateGrid();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (drawGrid)
            {
                drawGrid = false;

                maze.GenerateGrid();
            }
        }

        private void initializeMaze()
        {
            float padding_x_panel2 = 0;
            float padding_y_panel2 = 0;
            float padding_x_maze = 50F;
            float padding_y_maze = 50F;
            float maze_side = 0;

            this.panel1.Height = this.Height;
            this.panel1.Width = this.Width;

            if (panel1.Height >= (panel1.Width - panel2.Width - 50))
            {
                maze_side = (float)(panel1.Width - panel2.Width - 150);

                padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50 - maze_side) / 2F;

                if (maze_side < 0)
                    padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50) / 2F;

                if (padding_x_panel2 < 18F)
                    padding_x_panel2 = 18F;

                if (panel1.Width < (panel2.Width + 18))
                    padding_x_panel2 = panel1.Width - panel2.Width;

                padding_x_maze = padding_x_panel2;

                padding_y_panel2 = (float)(panel1.Height - panel2.Height) / 2F;

                if (padding_y_panel2 < 0F)
                    padding_y_panel2 = 0F;

                padding_y_maze = ((float)panel1.Height - maze_side) / 2F;

                if (padding_y_maze < 50F)
                    padding_y_maze = 50F;
            }
            else
            {
                maze_side = (float)panel1.Height - 100F;

                padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50 - maze_side) / 2F;

                if (maze_side < 0)
                    padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50) / 2F;

                if (padding_x_panel2 < 18F)
                    padding_x_panel2 = 18F;

                if (panel1.Width < (panel2.Width + 18))
                    padding_x_panel2 = panel1.Width - panel2.Width;

                padding_x_maze = padding_x_panel2;

                //if (maze_side < 0)
                //    maze_side = panel1.Height;

                padding_y_panel2 = (float)(panel1.Height - panel2.Height) / 2F;

                if (padding_y_panel2 < 0F)
                    padding_y_panel2 = 0F;

                padding_y_maze = ((float)panel1.Height - maze_side) / 2F;

                if (padding_y_maze < 50F)
                    padding_y_maze = 50F;
            }

            panel2.Location = new Point(panel1.Width - panel2.Width - (int)padding_x_panel2, (int)padding_y_panel2);

            maze.setGraphics(this.panel1.CreateGraphics());
            maze.setLocation(padding_x_maze, padding_y_maze, maze_side);
        }

        private void initializeMaze2()
        {
            float padding_x_panel2 = 0;
            float padding_y_panel2 = 0;
            float padding_x_maze = 50F;
            float padding_y_maze = 50F;
            float maze_side = 0;

            this.panel1.Height = this.Height;
            this.panel1.Width = this.Width;

            if (panel1.Height >= (panel1.Width - panel2.Width - 50))
            {
                maze_side = (float)(panel1.Width - panel2.Width - 198);

                if (maze_side < 0)
                    maze_side = panel1.Width - panel2.Width;

                padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50 - maze_side) / 2F;
                padding_x_maze = padding_x_panel2;

                if (panel1.Width < panel2.Width)
                {
                    padding_x_panel2 = -67F;
                }

                padding_y_panel2 = (float)(panel1.Height - panel2.Height) / 2F;

                if (padding_y_panel2 < 24F)
                    padding_y_panel2 = 24F;

                padding_y_maze = ((float)panel1.Height - maze_side) / 2F;

                if (padding_y_maze < 74F)
                    padding_y_maze = 74F;
            }
            else
            {
                padding_y_panel2 = (float)(panel1.Height - panel2.Height) / 2F;

                if (padding_y_panel2 < 24F)
                    padding_y_panel2 = 24F;

                maze_side = (float)panel1.Height - 148F;

                if (maze_side < 0)
                    maze_side = panel1.Height;

                padding_y_maze = ((float)panel1.Height - maze_side) / 2F;

                padding_x_panel2 = (float)(panel1.Width - panel2.Width - 50 - maze_side) / 2F;
                padding_x_maze = padding_x_panel2;

                if (panel1.Width < panel2.Width)
                {
                    padding_x_panel2 = -67F;
                }
            }

            panel2.Location = new Point(panel1.Width - panel2.Width - (int)padding_x_panel2 - 10, (int)padding_y_panel2 - 24);

            maze.setGraphics(this.panel1.CreateGraphics());
            maze.setLocation(padding_x_maze - 10, padding_y_maze - 24, maze_side);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            maze.GenerateMaze();
        }

        private void txtHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                try
                {
                    maze.setHeight(Int32.Parse(txtHeight.Text));

                    maze.GenerateGrid();

                }
                catch (Exception ex)
                {
                    txtHeight.Text = maze.getHeight().ToString();

                    txtHeight.Select(txtHeight.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        private void btnDecreaseHeight_MouseDown(object sender, MouseEventArgs e)
        {
            maze.setHeight(maze.getHeight() - 1);
            txtHeight.Text = maze.getHeight().ToString();

            drawGrid = true;
        }

        private void btnIncreaseHeight_MouseDown(object sender, MouseEventArgs e)
        {
            maze.setHeight(maze.getHeight() + 1);
            txtHeight.Text = maze.getHeight().ToString();

            drawGrid = true;
        }

        private void txtWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                try
                {
                    maze.setWidth(Int32.Parse(txtWidth.Text));

                    maze.GenerateGrid();

                }
                catch (Exception ex)
                {
                    txtWidth.Text = maze.getWidth().ToString();

                    txtWidth.Select(txtWidth.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        private void btnDecreaseWidth_MouseDown(object sender, MouseEventArgs e)
        {
            maze.setWidth(maze.getWidth() - 1);
            txtWidth.Text = maze.getWidth().ToString();

            drawGrid = true;
        }

        private void btnIncreaseWidth_MouseDown(object sender, MouseEventArgs e)
        {
            maze.setWidth(maze.getWidth() + 1);
            txtWidth.Text = maze.getWidth().ToString();

            drawGrid = true;
        }

        private void cboxAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            maze.setAlgorithm(cboxAlgorithm.Items[cboxAlgorithm.SelectedIndex].ToString());
        }

        private void rbgrpAlgorithmType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;

            if (rb != null)
            {
                if (rb.Equals(rbtnRemoveWalls))
                {
                    if (rbtnRemoveWalls.Checked)
                    {
                        maze.setAlgorithmType(0);
                    }
                }
                else if (rb.Equals(rbtnBuildWalls))
                {
                    if (rbtnBuildWalls.Checked)
                    {
                        maze.setAlgorithmType(1);
                    }
                }
                else if (rb.Equals(rbtnFillCells))
                {
                    if (rbtnFillCells.Checked)
                    {
                        maze.setAlgorithmType(2);
                    }
                }
            }
        }

        private void chkInverseColors_CheckedChanged(object sender, EventArgs e)
        {
            maze.inverseColors();
        }

        private void chkShowGen_CheckedChanged(object sender, EventArgs e)
        {
            maze.setShowgen(chkShowGen.Checked ? 1 : 0);
        }

        private void tbarGenSpeed_Scroll(object sender, EventArgs e)
        {
            maze.setGenspeed(tbarGenerateSpeed.Value);
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            maze.SolveMaze();
        }

        private void chkShowBacktracks_CheckedChanged(object sender, EventArgs e)
        {
            maze.setBacktrack(chkShowBacktracks.Checked ? 1 : 0);
        }

        private void chkShowSolve_CheckedChanged(object sender, EventArgs e)
        {
            maze.setShowsolve(chkShowSolve.Checked ? 1 : 0);
        }

        private void tbarSolveSpeed_Scroll(object sender, EventArgs e)
        {
            maze.setSolvespeed(tbarSolveSpeed.Value);
        }
    }
}