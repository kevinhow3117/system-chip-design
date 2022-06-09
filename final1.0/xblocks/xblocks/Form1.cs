using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace xblocks
{
    public partial class Form1 : Form
    {

        //down    :1
        //left    :2
        //right   :3
        //spin    :4
        //default :5
        int keycode = 5;


        Label[,] grids = new Label[20,10]; //main area, total 200 grids 
        Label[,] next = new Label[4, 3];   //next area, total 12 grids 
        bool[,] signs = new bool[24,10];
        Color[,] grids_color = new Color[24,10];
        uint block_row = 20;
        uint block_col = 4;
        uint block_type ;
        uint block_row_pre = 20;
        uint block_col_pre = 4;
        uint block_type_pre ;
        uint block_type_next;
        bool block_changed = false;
        Random rander = new Random(System.DateTime.Now.Millisecond);
        int timer_interval = 1010;
        int game_mode = 1;
        uint block_count = 0;
        uint score = 0;
        char[] data_to_char = new char[8];

        public Form1()
        {
            InitializeComponent();

            serialPort1.Open();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            block_type = (uint)rander.Next(0, 7) + 1;
            block_type_pre = block_type;
            block_type_next= block_type;
            // generate 20x10 labels for "main" area, dynamically.
            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 10; j++)
                {
                    grids[i, j] = new Label();
                    grids[i, j].Width = 30;
                    grids[i, j].Height = 30;
                    grids[i, j].BorderStyle = BorderStyle.FixedSingle;
                    grids[i, j].BackColor = Color.Black;                    
                    grids[i, j].Left = 150 + 30 * j;
                    grids[i, j].Top = 600 - i * 30;
                    grids[i, j].Visible = true;
                    this.Controls.Add(grids[i, j]);
                }
            // generate 4x3 labels for "next" area, dynamically.
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                {
                    next[i, j] = new Label();
                    next[i, j].Width = 20;
                    next[i, j].Height = 20;
                    next[i, j].BorderStyle = BorderStyle.FixedSingle;
                    next[i, j].BackColor = Color.White;                    
                    next[i, j].Left = 515 + 20 * j; 
                    next[i, j].Top = 150 - i * 20;
                    next[i, j].Visible = true;
                    this.Controls.Add(next[i, j]);
                }
            // init variables of the game
            init_game();

            //System.Media.SoundPlayer player =new System.Media.SoundPlayer();
            //player.SoundLocation = "xblocks.wav";
            //player.Load();
            //player.PlayLooping();

            
        }

        void init_game()
        {
            block_type = (uint)rander.Next(0, 7) + 1;
            block_type_pre = block_type;
            block_row = 20;
            block_col = 4;
            block_row_pre = 20;
            block_col_pre = 4;
            block_type_pre = block_type;
            block_type_next = block_type;
            block_changed = false;
            timer_interval = 1010;
            timer1.Interval =  timer_interval;
            block_count = 0;
            score = 0;
            game_mode = 1;

            for (uint i = 0; i < 24; i++)
                for (uint j = 0; j < 10; j++)
                    signs[i, j] = false;
        }

        void update_block(uint i, uint j, uint type)
        {
            switch (type)
            {
                case 1: 
                    signs[i, j] = signs[i+1, j] = signs[i+2, j] = signs[i+3, j] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i + 2, j] = grids_color[i + 3, j] = Color.Blue;
                    break;
                case 11: 
                    signs[i, j] = signs[i, j+1] = signs[i, j+2] = signs[i, j+3] = true;
                    grids_color[i, j] = grids_color[i, j + 1] = grids_color[i, j + 2] = grids_color[i, j + 3] = Color.Blue;
                    break;
               case 2:
                    signs[i, j] = signs[i + 1, j] = signs[i , j+1] = signs[i + 1, j+1] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i, j + 1] = grids_color[i + 1, j + 1] = Color.Yellow;
                    break;
               case 3:
                    signs[i, j] = signs[i + 1, j] = signs[i+1, j-1] = signs[i, j + 1] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i + 1, j - 1] = grids_color[i, j + 1] = Color.Red;
                    break;
               case 13:
                    signs[i, j] = signs[i - 1, j] = signs[i , j + 1] = signs[i+1, j + 1] = true;
                    grids_color[i, j] = grids_color[i - 1, j] = grids_color[i, j + 1] = grids_color[i + 1, j + 1] = Color.Red;
                    break;
               case 4:
                    signs[i, j] = signs[i , j-1] = signs[i + 1, j] = signs[i+1, j + 1] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i + 1, j] = grids_color[i + 1, j + 1] = Color.Green;
                    break;
               case 14:
                    signs[i, j] = signs[i+1, j] = signs[i, j+1] = signs[i-1, j + 1] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i, j + 1] = grids_color[i - 1, j + 1] = Color.Green;
                    break;
               case 5:
                    signs[i, j] = signs[i+1, j] = signs[i + 1, j+1] = signs[i + 1, j + 2] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i + 1, j + 1] = grids_color[i + 1, j + 2] = Color.Orange;
                    break;
               case 15:
                    signs[i, j] = signs[i, j-1] = signs[i + 1, j - 1] = signs[i + 2, j -1] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i + 1, j - 1] = grids_color[i + 2, j - 1] = Color.Orange;
                    break;
               case 25:
                    signs[i, j] = signs[i-1, j] = signs[i - 1, j - 1] = signs[i -1, j - 2] = true;
                    grids_color[i, j] = grids_color[i - 1, j] = grids_color[i - 1, j - 1] = grids_color[i - 1, j - 2] = Color.Orange;
                    break;
               case 35:
                    signs[i, j] = signs[i, j+1] = signs[i - 1, j + 1] = signs[i - 2, j +1] = true;
                    grids_color[i, j] = grids_color[i, j + 1] = grids_color[i - 1, j + 1] = grids_color[i - 2, j + 1] = Color.Orange;
                    break;
               case 6:
                    signs[i, j] = signs[i + 1, j] = signs[i + 1, j - 1] = signs[i + 1, j - 2] = true;
                    grids_color[i, j] = grids_color[i + 1, j] = grids_color[i + 1, j - 1] = grids_color[i + 1, j - 2] = Color.LightBlue;
                    break;
               case 16:
                    signs[i, j] = signs[i, j+1] = signs[i + 1, j + 1] = signs[i + 2, j + 1] = true;
                    grids_color[i, j] = grids_color[i, j + 1] = grids_color[i + 1, j + 1] = grids_color[i + 2, j + 1] = Color.LightBlue;
                    break;
               case 26:
                    signs[i, j] = signs[i-1, j] = signs[i-1, j + 1] = signs[i -1, j + 2] = true;
                    grids_color[i, j] = grids_color[i - 1, j] = grids_color[i - 1, j + 1] = grids_color[i - 1, j + 2] = Color.LightBlue;
                    break;
               case 36:
                    signs[i, j] = signs[i, j-1] = signs[i - 1, j - 1] = signs[i - 2, j -1] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i - 1, j - 1] = grids_color[i - 2, j - 1] = Color.LightBlue;
                    break;

               case 7:
                    signs[i, j] = signs[i, j-1] = signs[i, j+1] = signs[i+1, j] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i, j + 1] = grids_color[i + 1, j] = Color.Purple;
                    break;
               case 17:
                    signs[i, j] = signs[i, j + 1] = signs[i-1, j] = signs[i + 1, j] = true;
                    grids_color[i, j] = grids_color[i, j + 1] = grids_color[i - 1, j] = grids_color[i + 1, j] = Color.Purple;
                    break;
               case 27:
                    signs[i, j] = signs[i, j - 1] = signs[i, j+1] = signs[i - 1, j] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i, j + 1] = grids_color[i - 1, j] = Color.Purple;
                    break;
               case 37:
                    signs[i, j] = signs[i, j - 1] = signs[i+1, j] = signs[i - 1, j] = true;
                    grids_color[i, j] = grids_color[i, j - 1] = grids_color[i + 1, j] = grids_color[i - 1, j] = Color.Purple;
                    break;
            }

            
        }

        void erase_block(uint i, uint j, uint type)
        {
            switch (type)
            {
                case 1:
                    signs[i, j] = signs[i + 1, j] = signs[i + 2, j] = signs[i + 3, j] = false;                    
                    break;
                case 11:
                    signs[i, j] = signs[i, j+1] = signs[i, j+2] = signs[i, j+3] = false;
                    break;
                case 2:
                    signs[i, j] = signs[i + 1, j] = signs[i , j+1] = signs[i + 1, j+1] = false;
                    break;
                case 3:
                    signs[i, j] = signs[i + 1, j] = signs[i + 1, j - 1] = signs[i, j + 1] = false;
                    break;
                case 13:
                    signs[i, j] = signs[i - 1, j] = signs[i, j + 1] = signs[i + 1, j + 1] = false;
                    break;
                case 4:
                    signs[i, j] = signs[i, j - 1] = signs[i + 1, j] = signs[i + 1, j + 1] = false;
                    break;
                case 14:
                    signs[i, j] = signs[i + 1, j] = signs[i, j + 1] = signs[i - 1, j + 1] = false;
                    break;
                case 5:
                    signs[i, j] = signs[i + 1, j] = signs[i + 1, j + 1] = signs[i + 1, j + 2] = false;
                    break;
                case 15:
                    signs[i, j] = signs[i, j - 1] = signs[i + 1, j - 1] = signs[i + 2, j - 1] = false;
                    break;
                case 25:
                    signs[i, j] = signs[i - 1, j] = signs[i - 1, j - 1] = signs[i - 1, j - 2] = false;
                    break;
                case 35:
                    signs[i, j] = signs[i, j + 1] = signs[i - 1, j + 1] = signs[i - 2, j + 1] = false;
                    break;
                case 6:
                     signs[i, j] = signs[i + 1, j] = signs[i + 1, j - 1] = signs[i + 1, j - 2] = false;
                    break;
                case 16:
                    signs[i, j] = signs[i, j + 1] = signs[i + 1, j + 1] = signs[i + 2, j + 1] = false;
                    break;
                case 26:
                    signs[i, j] = signs[i - 1, j] = signs[i - 1, j + 1] = signs[i - 1, j + 2] = false;              
                    break;
                case 36:
                    signs[i, j] = signs[i, j - 1] = signs[i - 1, j - 1] = signs[i - 2, j - 1] = false;
                    break;
                case 7:
                    signs[i, j] = signs[i, j - 1] = signs[i, j + 1] = signs[i + 1, j] = false;
                    break;
                case 17:
                    signs[i, j] = signs[i, j + 1] = signs[i - 1, j] = signs[i + 1, j] = false;
                    break;
                case 27:
                    signs[i, j] = signs[i, j - 1] = signs[i, j + 1] = signs[i - 1, j] = false;
                    break;
                case 37:
                    signs[i, j] = signs[i, j - 1] = signs[i + 1, j] = signs[i - 1, j] = false;
                    break;
            }
        }

        bool y_direction(uint type, uint i, uint j)
        {
            switch (type)
            {
                case 1:
                    if (i != 0 && !signs[i-1, j]) return true;
                    else return false;

                case 11:
                    if (i != 0 && !signs[i - 1, j] && !signs[i - 1, j + 1] && !signs[i - 1, j + 2] && !signs[i - 1, j + 3]) return true;
                    else return false;

                case 2:
                    if (i != 0 && !signs[i-1, j] && !signs[i-1, j+1]) return true;
                    else return false;

                case 3:
                    if (i != 0 && !signs[i, j-1] && !signs[i-1, j] && !signs[i-1, j+1]) return true;
                    else return false;

                case 13:
                    if (i != 1 && !signs[i-2, j] && !signs[i-1, j+1])  return true;
                    else return false;

                case 4:
                    if (i != 0 && !signs[i, j+1] && !signs[i-1, j] && !signs[i-1, j-1]) return true;
                    else return false;

                case 14:
                    if (i != 1 && !signs[i-1, j] && !signs[i-2, j+1]) return true;
                    else return false;

                case 5:
                    if (i != 0 && !signs[i-1, j] && !signs[i, j+1] && !signs[i, j+2]) return true;
                    else return false;

                case 15:
                    if (i != 0 && !signs[i - 1, j] && !signs[i-1, j-1]) return true;
                    else return false;

                case 25:
                    if (i != 1 && !signs[i - 2, j] && !signs[i - 2, j - 1] && !signs[i - 2, j - 2]) return true;
                    else return false;

                case 35:
                    if (i != 2 && !signs[i - 1, j] && !signs[i - 3, j + 1]) return true;
                    else return false;

                case 6:
                    if (i != 0 && !signs[i, j-1] && !signs[i, j-2] && !signs[i-1, j]) return true;
                    else return false;

                case 16:
                    if (i != 0 && !signs[i-1, j] && !signs[i-1, j+1]) return true;
                    else return false;
                    
                case 26:
                    if (i != 1 && !signs[i-2, j] && !signs[i-2, j + 1] && !signs[i-2, j+2]) return true;
                    else return false;
                    
                case 36:
                    if (i != 2 && !signs[i-1, j] && !signs[i-3, j-1]) return true;
                    else return false;
                    
                case 7:
                    if (i != 0 && !signs[i-1, j-1] && !signs[i-1, j] && !signs[i-1, j+1]) return true;
                    else return false;
                    
                case 17:
                    if (i != 1 && !signs[i-2, j] && !signs[i - 1, j+1]) return true;
                    else return false;
                    
                case 27:
                    if (i != 1 && !signs[i - 1, j - 1] && !signs[i - 1, j + 1] && !signs[i - 2, j]) return true;
                    else return false;
                    
                case 37:
                    if (i != 1 && !signs[i-2, j] && !signs[i-1, j-1]) return true;
                    else return false;

                default:
                    return false;                  
            }
        }

        bool x_direction(uint type, uint i, uint j, int d)
        { 
           switch(type)
           {
               case 1:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j - 1] && !signs[i + 2, j - 1] && !signs[i + 3, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i + 1, j + 1] && !signs[i + 2, j + 1] && !signs[i + 3, j + 1]) return true;
                       else return false;
                   }

               case 11:
                   if (d == -1) 
                   {
                       if (j != 0 && !signs[i, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 6 && !signs[i, j + 4]) return true;
                       else return false;
                   }
               
               case 2:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 2]) return true;
                       else return false;
                   }

               case 3:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 1] && !signs[i + 1, j - 2]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 1]) return true;
                       else return false;
                   }

               case 13:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j] && !signs[i + 1, j-1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 2] && !signs[i - 1, j + 1]) return true;
                       else return false;
                   }

               case 4:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i + 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 1] && !signs[i + 1, j + 2]) return true;
                       else return false;
                   }
                  
               case 14:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j-1] && !signs[i+1, j-1] && !signs[i-1, j]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 1] && !signs[i-1, j + 2]) return true;
                       else return false;
                   }

               case 5:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 7 && !signs[i, j + 1] && !signs[i + 1, j + 3]) return true;
                       else return false;
                   }

               case 15:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i + 1, j - 2] && !signs[i + 2, j - 2]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i + 1, j] && !signs[i + 2, j]) return true;
                       else return false;
                   }
              
               case 25:
                   if (d == -1)
                   {
                       if (j != 2 && !signs[i, j-1] && !signs[i-1, j-3]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i-1, j+1]) return true;
                       else return false;
                   }
                
               case 35:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i-1, j] && !signs[i-2, j]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i-1, j+2] && !signs[i-2, j+2]) return true;
                       else return false;
                   }

               case 6:
                   if (d == -1)
                   {
                       if (j != 2 && !signs[i, j - 1] && !signs[i + 1, j - 3]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i + 1, j + 1]) return true;
                       else return false;
                   }

               case 16:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j] && !signs[i + 2, j]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 2] && !signs[i + 2, j + 2]) return true;
                       else return false;
                   }

               case 26:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i - 1, j-1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 7 && !signs[i, j + 1] && !signs[i - 1, j + 3]) return true;
                       else return false;
                   }

               case 36:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i - 1, j - 2] && !signs[i - 2, j - 2]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i - 1, j] && !signs[i - 2, j]) return true;
                       else return false;
                   }

               case 7:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i + 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 1]) return true;
                       else return false;
                   }

               case 17:
                   if (d == -1)
                   {
                       if (j != 0 && !signs[i, j - 1] && !signs[i + 1, j - 1] && !signs[i - 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i + 1, j + 1] && !signs[i - 1, j + 1]) return true;
                       else return false;
                   }

               case 27:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i - 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 8 && !signs[i, j + 2] && !signs[i - 1, j + 1]) return true;
                       else return false;
                   }
                   
               case 37:
                   if (d == -1)
                   {
                       if (j != 1 && !signs[i, j - 2] && !signs[i + 1, j - 1] && !signs[i - 1, j - 1]) return true;
                       else return false;
                   }
                   else
                   {
                       if (j != 9 && !signs[i, j + 1] && !signs[i + 1, j + 1] && !signs[i - 1, j + 1]) return true;
                       else return false;
                   }

               default:
                   return false;
           }            
        }
        
        void show_grids()
        {
            int i, j;
            for (i = 0; i < 20; i++)
                for (j = 0; j < 10; j++)
                    if (signs[i, j])
                        grids[i, j].BackColor = grids_color[i, j];
                    else
                        grids[i, j].BackColor = Color.Black;
        }

        void display_next_block(uint type)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                    next[i, j].BackColor = Color.White;

                switch (type)
                {
                    case 1:
                        next[0, 1].BackColor = next[1, 1].BackColor = next[2, 1].BackColor = next[3, 1].BackColor = Color.Blue;                        
                        break;
                    case 2:
                        next[1, 0].BackColor = next[1, 1].BackColor = next[2, 0].BackColor = next[2, 1].BackColor = Color.Yellow;                     
                        break;
                    case 3:
                        next[2, 0].BackColor = next[2, 1].BackColor = next[1, 1].BackColor = next[1, 2].BackColor = Color.Red; 
                        break;
                    case 4:
                        next[1, 0].BackColor = next[1, 1].BackColor = next[2, 1].BackColor = next[2, 2].BackColor = Color.Green; 
                        break;
                    case 5:
                        next[1, 0].BackColor = next[2, 0].BackColor = next[2, 1].BackColor = next[2, 2].BackColor = Color.Orange;
                        break;
                    case 6:
                        next[2, 0].BackColor = next[2, 1].BackColor = next[2, 2].BackColor = next[1, 2].BackColor = Color.LightBlue;
                        break;
                    case 7:
                       next[1, 0].BackColor = next[1, 1].BackColor = next[1, 2].BackColor = next[2, 1].BackColor = Color.Purple;
                       break;
                }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"timer1");

            

            if (y_direction(block_type, block_row, block_col))
            {

                block_row_pre = block_row; block_row_pre = block_row; block_type_pre = block_type;
                block_row--;

                if (block_row == 19)
                {
                    block_type_next = (uint)rander.Next(0, 7) + 1;
                    display_next_block(block_type_next);
                    block_count++;
                    score += 5;
                    label_block_count.Text = "Blocks:" + block_count.ToString();
                    label_score.Text = "Score:" + score.ToString();
                    if (game_mode == 1)
                    {
                        timer_interval = 1010 - (int)(score / 150) * 50;
                        if (timer_interval <= 0)
                            timer_interval = 10;

                        timer1.Interval = timer_interval;
                        label_level.Text = "Level:" + (1010 - timer_interval) / 50;
                    }                    
                  }
                erase_block(block_row_pre, block_col_pre, block_type_pre);
                update_block(block_row, block_col, block_type);
                show_grids();
                block_row_pre = block_row;
                block_changed = false;
            }
            else 
            {

                show_grids();
                full_line_check();
                if (block_row == 20)
                {                    
                  label_info.Text = "Game Over!";
                  button1.Visible = true;
                  button1.Enabled = true;
                  timer1.Enabled = false;
                  return;

                };
                block_type = block_type_next; 
                block_row = 20;
                block_col = 4;
                block_row_pre = 20;
                block_col_pre = 4;
                block_type_pre = block_type;
                block_changed = false;

            }
            
            

        }

        void full_line_check()
        {
            uint row_sum;
            uint i, j;

            i = 0;
            while(i < 20)
            {
                row_sum = 0;
                for (j = 0; j < 10; j++)
                    if (signs[i, j]) row_sum++;

                if (row_sum == 10)
                {
                    score += 20;
                    label_score.Text = "Score:" + score.ToString();
                    for (j = 0; j < 10; j++)
                        signs[i, j] = false;

                    show_grids(); // show a black line 

                    for (uint y = i; y < 21; y++)
                        for (j = 0; j < 10; j++)
                        {
                            signs[y, j] = signs[y + 1, j];
                            grids_color[y, j] = grids_color[y + 1, j];
                        }
                    show_grids();
                }
                else i++;
            }
        }

        uint next_block_type(uint type, uint i, uint j)
        {
            switch(type)
            {
                case 1:
                    if (j <= 7 && j>=1 && !signs[i+2, j-1] && !signs[i+2, j + 1] && !signs[i+2, j + 2])
                    {
                        block_row = i + 2; block_col = j - 1;
                        return 11;
                    }
                    else return 1;
                
                case 11:
                    if (i >= 2 && !signs[i - 1, j+1] && !signs[i - 2, j+1] && !signs[i +1, j+1])
                    {
                        block_row = i -2; block_col = j + 1;
                        return 1;
                    }
                    else return 11; 
                
                case 2: return 2; 
                
                case 3:
                    if (i >= 1 && !signs[i + 1, j+1] && !signs[i-1, j])
                        return 13;
                    else return 3;  

                case 13:
                    if (j >= 1 && !signs[i + 1, j] && !signs[i+1, j-1])
                        return 3;
                    else return 13;  
                
                case 4:
                    if (i >= 1 && !signs[i, j+1] && !signs[i-1, j+1])
                        return 14;
                    else return 4;  

                case 14:
                    if (j >= 1 && !signs[i, j-1] && !signs[i + 1, j + 1])
                        return 4;
                    else return 14;   

                case 5:
                    if (!signs[i + 2, j] && !signs[i, j + 1])
                    {
                        block_col = j + 1;
                        return 15;
                    }
                    else return 5;   

                case 15:
                    if (j >= 2 && !signs[i, j-2] && !signs[i + 1, j] )
                    {
                        block_row = i + 1;
                        return 25;
                    }
                    else return 15;                       

                case 25:
                    if (i >= 2 && !signs[i, j - 1] && !signs[i - 2, j])
                    {
                        block_col = j - 1;
                        return 35;
                    }
                    else return 25;  

                case 35:
                    if (j <= 7 && !signs[i - 1, j] && !signs[i, j + 2])
                    {
                        block_row = i - 1;
                        return 5;
                    }
                    else return 35; 

                case 6:
                    if (!signs[i, j - 1] && !signs[i + 2, j] )
                    {
                        block_col = j - 1;
                        return 16;
                    }
                    else return 6; 

                case 16:
                    if (j <= 7 && !signs[i - 1, j] && !signs[i, j + 2])
                    {
                        block_row = i + 1;
                        return 26;
                    }
                    else return 16; 

                case 26:
                    if (i >= 2 && !signs[i, j +1] && !signs[i - 2, j])
                    {
                        block_col = j + 1;
                        return 36;
                    }
                    else return 26; 

                case 36:
                    if (j >= 2 && !signs[i, j-2] && !signs[i - 1, j])
                    {
                        block_row = i - 1;
                        return 6;
                    }
                    else return 36; 

                case 7:
                    if (i>=1 && !signs[i-1, j])
                        return 17;
                    else return 7;  

                case 17:
                    if (j >= 1 && !signs[i, j-1])
                        return 27;
                    else return 17;  

                case 27:
                    if (!signs[i+1, j])
                        return 37;
                    else return 27;  

                case 37:
                    if (j<=8 && !signs[i, j+1])
                        return 7;
                    else return 37;  

                default: return 0;  
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                if (game_mode == 0) { game_mode = 1; timer1.Enabled = true; }
                else { game_mode = 0; timer1.Enabled = false; }
            }

            if (e.KeyCode == Keys.Left || keycode==2)
            {
                if (x_direction(block_type, block_row, block_col, -1))
                {
                    block_col_pre = block_col; block_col--;
                    block_changed = true;
                }
            }

            if (e.KeyCode == Keys.Right ||keycode==3)
            {
                if (x_direction(block_type, block_row, block_col, 1))
                {
                    block_col_pre = block_col; block_col++;
                    block_changed = true;
                }
            }

            if (e.KeyCode == Keys.Space ||keycode==4)
            {
                block_type_pre = block_type;
                block_col_pre = block_col; block_row_pre = block_row;
                block_type = next_block_type(block_type, block_row, block_col);
                if(block_type != block_type_pre)
                    block_changed = true;
            }

            if (e.KeyCode == Keys.S)
            {
                game_mode = 2;               
                timer_interval -= 50;
               
                if(timer_interval <= 0)
                    timer_interval = 1;
                
                timer1.Interval = timer_interval;
                label_level.Text = "Level:" + (1010 - timer_interval) / 50;
            }

            if (e.KeyCode == Keys.A)
            {
                game_mode = 2;
                timer_interval += 50;

                if (timer_interval >= 1010)
                    timer_interval = 1010;

                timer1.Interval = timer_interval;
                label_level.Text = "Level:" + (1010 - timer_interval) / 50;
            }

            if (e.KeyCode == Keys.Down || keycode==1)
                timer1.Interval = 1;
      
            if (block_changed)
            {
                erase_block(block_row_pre, block_col_pre, block_type_pre);
                update_block(block_row, block_col, block_type);
                show_grids();
                block_row_pre = block_row; block_col_pre = block_col; block_type_pre = block_type;
                block_changed = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init_game();            
            label_info.Text = "";           
            button1.Visible = false;
            button1.Enabled = false;
            timer1.Enabled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                timer1.Interval = timer_interval;
            }
        }
        private void serialPort1_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            //string data = serialPort1.ReadExisting();
            Console.WriteLine($"{data}");
            string data_to_string = data.Replace("\r\n", "");
            Console.WriteLine($"{data_to_string}");
            switch (data_to_string)
            {
                case "down":
                    keycode = 1;
                 break;
                case "left":
                    keycode = 2;
                    break;
                case "righ"://case "right":
                    keycode = 3;
                    break;
                case "spin":
                    keycode = 4;
                    break;

                default:
                    //keycode = 5;
                break;
            }
            //for (int i = 0; i < 10; i++) ;
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"{keycode}");
            if (keycode == 2)
            {
                if (x_direction(block_type, block_row, block_col, -1))
                {
                    block_col_pre = block_col; block_col--;
                    block_changed = true;
                }
            }
            if (keycode == 3)
            {
                if (x_direction(block_type, block_row, block_col, 1))
                {
                    block_col_pre = block_col; block_col++;
                    block_changed = true;
                }
            }

            if (keycode == 4)
            {
                block_type_pre = block_type;
                block_col_pre = block_col; block_row_pre = block_row;
                block_type = next_block_type(block_type, block_row, block_col);
                if (block_type != block_type_pre)
                    block_changed = true;
            }

            if (keycode == 1)
            {
                timer1.Interval = 1;                
            }

            if (block_changed)
            {
                erase_block(block_row_pre, block_col_pre, block_type_pre);
                update_block(block_row, block_col, block_type);
                show_grids();
                block_row_pre = block_row; block_col_pre = block_col; block_type_pre = block_type;
                block_changed = false;
            }
            if (keycode == 5)
            {
                timer1.Interval = timer_interval;
            }
            keycode = 5;
        }
    }
}
