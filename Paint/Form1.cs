﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

using Paint.Manager;
using Paint.MyItem;

namespace Paint
{
    public partial class Form1 : Form
    {
        #region khai báo
        private static Color MainColor = Color.Black;
        // Data save
        public SelectedTool tool = new SelectedTool();
        private Pen _p = new Pen(Color.Black, Size);
        public MyData dt = new MyData();
        private static int Size = 3;
        
        
        public Point Brush_A = new Point();
        public Point Brush_B = new Point();
        // vị trí chuột
        
        int x = -1;
        int y = -1;
        // cho phép vẽ
        bool allowDraw;


        // độ phóng to thu nhỏ
        public int hesonhan = 1;
        // danh sach ptb
        public List<MyPtb> list_ptb;
        
        #endregion
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            menuStrip1.Renderer = new MenuStripRenderer();
              
        }

        #region pick color
        public class MenuStripRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

                if (e.Item.Selected)
                {
                    Color color = Color.DarkGray;
                    using (SolidBrush brush = new SolidBrush(color))
                        e.Graphics.FillRectangle(brush, rect);
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(37, 37, 37)))
                        e.Graphics.FillRectangle(brush, rect);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.Item.ForeColor = Color.White;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                base.OnRenderSeparator(e);
            }

            public MenuStripRenderer() : base(new MenuStripColors()) { }
        }

        public class MenuStripColors : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get { return MainColor; }
            }

            public override Color MenuItemSelected
            {
                get { return MainColor; }
            }

            public override Color MenuItemBorder
            {
                get { return MainColor; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return MainColor; }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get { return MainColor; }
            }

            public override Color MenuBorder
            {
                get { return MainColor; }
            }
        }

        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.tool.isBrush = true;
            

           

        }

        // mo file

        public void OpenImage(PictureBox picturebox)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files(*.png;*.jpg; *.jpeg; *.gif; *.bmp)|*.png;*.jpg; *.jpeg; *.gif; *.bmp";
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picturebox.Image = new Bitmap(openFile.FileName);
                picturebox.Size = pt_draw.Size;

            }
        }

        private void openfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox _myptb = new PictureBox();
            OpenImage(_myptb);
            pt_draw.Image = _myptb.Image;
            
        }
        private void textBox_RGBvalueChange()
        {
            textBox_Rvalue.Text = pictureBox_Color_Front.BackColor.R.ToString();
            textBox_Gvalue.Text = pictureBox_Color_Front.BackColor.G.ToString();
            textBox_Bvalue.Text = pictureBox_Color_Front.BackColor.B.ToString();
        }

        private void btn_SelectColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                pictureBox_Color_Front.BackColor = colorPicker.Color;
                textBox_RGBvalueChange();
            }
        }

        private void color_Swap(object sender, EventArgs e)
        {
            Color tmp = pictureBox_Color_Front.BackColor;
            pictureBox_Color_Front.BackColor = pictureBox_Color_Back.BackColor;
            pictureBox_Color_Back.BackColor = tmp;
            _p.Color = pictureBox_Color_Front.BackColor;
            textBox_RGBvalueChange();
        }

        private void colorBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorWheelToolStripMenuItem.Checked = false;
        }

        private void colorWheelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorBarToolStripMenuItem.Checked = false;
        }

        private void colorPicker_ColorPicked(object sender, EventArgs e)
        {
            pictureBox_Color_Front.BackColor = colorPicker.SelectedColor;
            _p.Color = pictureBox_Color_Front.BackColor;
            textBox_RGBvalueChange();
        }

        private void colorPicker_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox_ColorPreview.BackColor = colorPicker._canvas.GetPixel(e.X, e.Y);
        }
        #region Undo, Redo
        private void btn_Undo_Click(object sender, EventArgs e)
        {
            if (dt.luu.n > 0)
                dt.luu.n--;
            dt.velai(this, pt_draw);
        }


        #endregion
        private void pictureBox_Color_Front_BackColorChanged(object sender, EventArgs e)
        {
            
        }

        private void btn_Text_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_Shape_Click(object sender, EventArgs e)
        {
            tLP_Shape.Visible = true;
        }
        #region panel_paint paint
        private void panel_paint_MouseDown(object sender, MouseEventArgs e)
        {
            allowDraw = true; 
            
            Brush_A = e.Location;
            if (tool.isBrush)
            {
                x = e.X;
                y = e.Y;
                
                dt._points.Add(e.Location);
                
            }
            
        }

        private void panel_paint_MouseUp(object sender, MouseEventArgs e)
        {
            allowDraw = false;

            Brush_B = e.Location;

            if (tool.isBrush)
            {
                x = -1;
                y = -1;
                dt._points.Add(e.Location);
                Graphics _g = pt_draw.CreateGraphics();
                
                _g.SmoothingMode = SmoothingMode.AntiAlias;
                
                dt.luu.list[dt.luu.n] = new H();
                dt.luu.list[dt.luu.n].path.AddCurve(dt._points.ToArray());   
                dt.luu.list[dt.luu.n].p.Color = _p.Color;
                dt.luu.list[dt.luu.n].p.Width = _p.Width;


                _g.DrawPath(dt.luu.list[dt.luu.n].p, dt.luu.list[dt.luu.n].path);
                dt.luu.n++;
                dt.n = dt.luu.n;
                
                dt._points.Clear();
            }
            
        }

        private void panel_paint_MouseMove(object sender, MouseEventArgs e)
        {
            /////
            ///

            ////
            ///

            if (allowDraw)
            {
                if (tool.isBrush && x != -1 && y != -1)
                {
                    dt._points.Add(e.Location);
                    using (Graphics _g = pt_draw.CreateGraphics())
                    {
                        _g.SmoothingMode = SmoothingMode.AntiAlias;
                        _g.DrawLine(_p, new Point(x, y), e.Location);
                    }
                    x = e.X;
                    y = e.Y;
                }
            }
        }
        
#endregion

        #region tool check
        private void btn_Brush_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = true;
            this.tool.isLine = false;
            this.tool.isRect = false;
            this.tool.isCurve = false;
            this.tool.isCircle = false;
            this.tool.isTriangle = false;
            this.tool.isHexagon = false;

        }

        private void btn_Line_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = true;
            this.tool.isRect = false;
            this.tool.isCurve = false;
            this.tool.isCircle = false;
            this.tool.isTriangle = false;
            this.tool.isHexagon = false;

        }

        private void btn_Curve_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = false;
            this.tool.isRect = false;
            this.tool.isCurve = true;
            this.tool.isCircle = false;
            this.tool.isTriangle = false;
            this.tool.isHexagon = false;

        }

        private void btn_Rectangle_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = false;
            this.tool.isRect = true;
            this.tool.isCurve = false;
            this.tool.isCircle = false;
            this.tool.isTriangle = false;
            this.tool.isHexagon = false;

        }

        private void btn_Circle_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = false;
            this.tool.isRect = false;
            this.tool.isCurve = false;
            this.tool.isCircle = true;
            this.tool.isTriangle = false;
            this.tool.isHexagon = false;

        }

        private void btn_Triangle_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = false;
            this.tool.isRect = false;
            this.tool.isCurve = false;
            this.tool.isCircle = false;
            this.tool.isTriangle = true;
            this.tool.isHexagon = false;

        }

        private void btn_Polygon_Click(object sender, EventArgs e)
        {
            this.tool.isBrush = false;
            this.tool.isLine = false;
            this.tool.isRect = false;
            this.tool.isCurve = false;
            this.tool.isCircle = false;
            this.tool.isTriangle = false;
            this.tool.isHexagon = true;

        }

        private void btn_Fill_Click(object sender, EventArgs e)
        {
            if (this.tool.isFill == false) tool.isFill = true;
            else tool.isFill = false;
        }

        #endregion

    }
}
