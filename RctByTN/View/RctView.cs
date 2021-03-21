﻿using RctByTN.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RctByTN.View
{
    public partial class RctView : Form
    {
        private const Int32 ParkHeight = 13;
        private const Int32 ParkWidth = 23;

        private RctModel _model;
        private Button[,] _buttonGrid;
        private Int32 _selectedTab;
        public RctView()
        {
            InitializeComponent();
            _selectedTab = -1;
        }

        private void RctView_Load(object sender, EventArgs e)
        {
            _model = new RctModel();
            _model.ElementChanged += new EventHandler<ParkElementEventArgs>(Game_ElementChanged);
            _model.CashChanged += new EventHandler(Game_CashChanged);
            parkElementPanel1.Visible = true;
            parkElementPanel2.Visible = false;
            GenerateTable();
        }

        private void Game_CashChanged(object sender, EventArgs e)
        {
            cashLabel.Text = "Cash: "+_model.Cash.ToString();
            incomeLabel.Text = "Income: "+_model.Income.ToString();
            outcomeLabel.Text = "Outcome: "+_model.Outcome.ToString();
        }

        private void Game_ElementChanged(Object sender, ParkElementEventArgs e)
        {
            var element = e.Element;
            switch (element.Status)
            {
                case ElementStatus.Operate:
                    BuildParkElement(element,(button) => button.BackColor = Color.Green);
                    //_buttonGrid[element.X, element.Y].BackColor = Color.Green;
                    break;
                case ElementStatus.InWaiting:
                    if (element.GetType() == typeof(Road))
                    {
                        _buttonGrid[element.X, element.Y].BackgroundImage = Properties.Resources.road;
                    }
                    else if(element.GetType() == typeof(Grass))
                    {
                        _buttonGrid[element.X, element.Y].BackgroundImage = Properties.Resources.grass;
                    }
                    else if(element.GetType() == typeof(Bush))
                    {
                        _buttonGrid[element.X, element.Y].BackgroundImage = Properties.Resources.bush;
                    }
                    else
                    {
                        _buttonGrid[element.X - 1, element.Y - 1].BackgroundImage = Properties.Resources.giantwheel1;
                        _buttonGrid[element.X, element.Y - 1].BackgroundImage = Properties.Resources.giantwheel3;
                        _buttonGrid[element.X - 1, element.Y].BackgroundImage = Properties.Resources.giantwheel2;
                        _buttonGrid[element.X, element.Y].BackgroundImage = Properties.Resources.giantwheel4;
                        BuildParkElement(element, (button) => button.BackColor = Color.Gray);
                        BuildParkElement(element, (button) => button.Image = null);
                    }
                    _buttonGrid[element.X, element.Y].Image = null;
                    //_buttonGrid[element.X, element.Y].BackColor = Color.Orange;
                    break;
                case ElementStatus.InBuild:
                    BuildParkElement(element, (button) => button.Image = Properties.Resources.buildsmall);
                    //_buttonGrid[element.X, element.Y].Image = Properties.Resources.buildsmall;
                    break;
            }
        }

        private void BuildParkElement(ParkElement element,Action<Button> action)
        {
            int x = element.X;
            int y = element.Y;
            if (element.AreaSize == 1)
            {
                action(_buttonGrid[x,y]);
            }
            else if(element.AreaSize == 4)
            {
                action(_buttonGrid[x, y]);
                action(_buttonGrid[x-1, y]);
                action(_buttonGrid[x, y-1]);
                action(_buttonGrid[x-1, y-1]);
            }
        }

        public void GenerateTable()
        {
            _buttonGrid = new Button[ParkHeight, ParkWidth];
            this.buttonGridPanel.ColumnCount = ParkWidth;
            this.buttonGridPanel.RowCount = ParkHeight;
            this.buttonGridPanel.ColumnStyles.Clear();
            this.buttonGridPanel.RowStyles.Clear();

            for (int i = 0; i < ParkWidth; i++)
            {
                this.buttonGridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,
                                                                        1 / Convert.ToSingle(ParkWidth)));
            }
            for (int i = 0; i < ParkHeight; i++)
            {
                this.buttonGridPanel.RowStyles.Add(new RowStyle(SizeType.Percent,
                                                                        1 / Convert.ToSingle(ParkHeight)));
            }

            for (Int32 i = 0; i < ParkHeight; i++)
            {
                for (Int32 j = 0; j < ParkWidth; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Size = new Size(50, 50);
                    _buttonGrid[i,j].BackgroundImageLayout = ImageLayout.Stretch;
                    //_buttonGrid[i, j].BackColor = Color.FromArgb(37, 211, 102);
                    //_buttonGrid[i, j].BackColor = Color.FromArgb(120, 146, 74);
                    _buttonGrid[i, j].BackColor = Color.FromArgb(117,185,67);
                    //_buttonGrid[i, j].FlatAppearance.BorderColor = Color.FromArgb(82, 255, 164);
                    //_buttonGrid[i, j].FlatAppearance.BorderColor = Color.FromArgb(157, 182, 113);
                    _buttonGrid[i, j].FlatAppearance.BorderColor = Color.FromArgb(140,189,105);
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat;
                    _buttonGrid[i, j].Margin = new Padding(0);
                    _buttonGrid[i, j].TabIndex = i * ParkWidth + j;
                    _buttonGrid[i, j].Click += buttonGrid_Click;
                    buttonGridPanel.Controls.Add(_buttonGrid[i, j]);
                }
            }
        }

        private void RefreshTable()
        {
            //for the guest 
        }

        private void buttonGrid_Click(object sender, EventArgs e)
        {
            if (_model.IsParkOpen)
                return;

            if (_selectedTab == -1)
            {
                MessageBox.Show("Az építés megkezdése előtt válassza ki az építésre szánt park elemet!"
                    , "Az építés megkezdése sikertelen!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Int32 x = (sender as Button).TabIndex / ParkWidth;
            Int32 y = (sender as Button).TabIndex % ParkWidth;

            if(_model.IsFreeArea(x,y,_selectedTab))
            {
                MessageBox.Show("A kiválasztott terület foglalt, az építéshez válaszon ki a választott vidámpark elemnek megfelelő szabad területet"
                    , "Az építés megkezdése sikertelen!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_selectedTab >= 0 && _selectedTab <= 2) 
            {
                var cgv = new CreateGameView();
                if (cgv.ShowDialog() == DialogResult.OK)
                {
                    _model.Build(x, y, _selectedTab, cgv.TicketCost, cgv.MinCapacity);
                }
            }
            else if(_selectedTab>=3 && _selectedTab<=5)
            {
                var crv = new CreateRestaurantView();
                if (crv.ShowDialog() == DialogResult.OK)
                {
                    _model.Build(x, y, _selectedTab, crv.FoodCost, 0);
                }
            }
            else
            {
                _model.Build(x, y, _selectedTab, 0, 0);
            }
        }

        private void parkElementPanel_Click(object sender, EventArgs e)
        {
            _selectedTab = ((Button)sender).TabIndex;
            foreach (Button button in parkElementPanel1.Controls)
            {
                
                if (_selectedTab == button.TabIndex)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.FromArgb(121, 96, 76);
                }
                else
                {
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.BorderColor = Color.Empty;
                    button.FlatStyle = FlatStyle.Standard;
                }
            }
            foreach (Button button in parkElementPanel2.Controls)
            {
                if (_selectedTab == button.TabIndex)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.FromArgb(121, 96, 76);
                }
                else
                {
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.BorderColor = Color.Empty;
                    button.FlatStyle = FlatStyle.Standard;
                }
            }
        }

        private void nextPictureBox_Click(object sender, EventArgs e)
        {
            parkElementPanel1.Visible = !parkElementPanel1.Visible;
            parkElementPanel2.Visible = !parkElementPanel2.Visible;
        }

        private void openEditButton_Click(object sender, EventArgs e)
        {
            //_model.IsParkOpen = !_model.IsParkOpen;
            if (_model.IsParkOpen) {
                _model.IsParkOpen = false;
                foreach(Button button in _buttonGrid) button.FlatAppearance.BorderSize = 1; 
            }
            else
            {
                _model.IsParkOpen = true;
                foreach (Button button in _buttonGrid) button.FlatAppearance.BorderSize = 0;
            }
            openEditButton.Text = _model.IsParkOpen ?
                                    "Park szerkesztése"
                                    : "Park megnyitása";
            campaignButton.Enabled = _model.IsParkOpen;
        }

        private void cancelPictureBox_Click(object sender, EventArgs e)
        {
            _selectedTab = -1;
            foreach (Button button in parkElementPanel1.Controls)
            {
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.BorderColor = Color.Empty;
                button.FlatStyle = FlatStyle.Standard;
            }
            foreach (Button button in parkElementPanel2.Controls)
            {
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.BorderColor = Color.Empty;
                button.FlatStyle = FlatStyle.Standard;
            }
        }
    }
}
