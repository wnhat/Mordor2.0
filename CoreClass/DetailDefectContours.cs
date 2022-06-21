﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using CoreClass.Model;

namespace CoreClass
{
    public class DetailDefectContours
    {
        static readonly int cellX = 1500;
        static readonly int cellY = 600;
        readonly Bitmap defectMap = new(cellX, cellY);
        readonly Graphics graphics;
        public DetailDefectContours(params string[] data)
        {
            //绑定画布
            graphics = Graphics.FromImage(defectMap);
            //添加背景颜色
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(69, 99, 73));
            graphics.FillRectangle(solidBrush, new Rectangle(new Point(0, 0), new Size(cellX, cellY)));
            
            // data 中的每一项都是contours文件中所有的数据；
            foreach (var item in data)
            {
                if (item == null) continue;
                GetDefectContours(item.Split('\n'));
            }
        }
        private void GetDefectContours(IEnumerable<string> L)
        {
            double scaleX = 1;
            double scaleY = 1;
            List<Point> points = new List<Point>();
            for (int i = 0; i < L.Count(); i++)
            {
                string s = L.ElementAt(i);
                if (s.StartsWith("Cell_X"))//记录cell尺寸
                {
                    string[] cellSizeInfo = s.Replace(" ","").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    scaleX = Convert.ToDouble(cellSizeInfo[0].Substring(7)) / (double)cellX;
                    scaleY = Convert.ToDouble(cellSizeInfo[1].Substring(7)) / (double)cellY;
                }
                else if (s.StartsWith("No="))
                {
                    string[] defectInfo = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int defectNum = Convert.ToInt32(defectInfo[0].Split(new char[] { '=' })[1]);
                    int pattern = Convert.ToInt32(defectInfo[1].Split(new char[] { '=' })[1]);
                    int camera = Convert.ToInt32(defectInfo[2].Split(new char[] { '=' })[1]);
                    int defect = Convert.ToInt32(defectInfo[3].Split(new char[] { '=' })[1]);
                    if (defectNum != 1)
                    {
                        DrawDefectMap(points.ToArray());
                    }
                    points.Clear();
                }
                else if( s != "" )
                {
                    string[] pointLocation = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int pointX = Convert.ToInt32(pointLocation[0]);
                    if (pointX > 10)
                    {
                        pointX = (int)((double)pointX / scaleX);
                    }
                    else
                    {
                        pointX += 5;
                    }
                    int pointY = Convert.ToInt32(pointLocation[1]);
                    if (pointY > 10)
                    {
                        pointY = (int)((double)pointY / scaleY);
                    }
                    else
                    {
                        pointY += 5;
                    }
                    points.Add(new Point(pointX, pointY));
                    if (i == L.Count() - 1)
                    {
                        DrawDefectMap(points.ToArray());
                    }
                }
            }
        }
        /// <summary>
        /// 绘制缺陷图
        /// </summary>
        /// <param name="pointList"></param>
        private void DrawDefectMap(Point[] pointList)
        {
            Pen defectDrawPen = new(Color.White);
            defectDrawPen.Width = (float)1.6;
            if (pointList.Length > 2)
            {
                graphics.DrawPolygon(defectDrawPen, pointList);
            }
        }
        public byte[] GetByte()
        {
            MemoryStream buffer = new();
            defectMap.Save(buffer, System.Drawing.Imaging.ImageFormat.Jpeg);
            return buffer.ToArray();
        }
        public Bitmap GetBitmap()
        {
            return defectMap;
        }
        public ImageContainer GetImageContainer()
        {
            ImageContainer imageContainer = new("Contours", GetByte());
            return imageContainer;
        }
    }
}