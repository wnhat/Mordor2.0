using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace CoreClass
{
    class DetailDefectContours
    {
        int cellX = 1500;
        int cellY = 600;
        List<string> lineData1 = new List<string>();
        List<string> lineData2 = new List<string>();
        List<DefectContours> defectContoursList = new List<DefectContours>();
        public Bitmap defectMap = null;
        public DetailDefectContours(string filePath1, string filePath2)
        {
            if (!System.IO.File.Exists(filePath1))
            {
                if (!System.IO.File.Exists(filePath2))
                {
                }
                else
                {
                }
            }
            lineData1 = ReadFile(filePath1);
            lineData2 = ReadFile(filePath2);
            GetDefectContours(lineData1);
            GetDefectContours(lineData2);
            defectMap = DrawDefectMap(defectContoursList, cellX, cellY);
        }
        public DetailDefectContours(string filePath)
        {
            lineData1 = ReadFile(filePath);
            GetDefectContours(lineData1);
            defectMap = DrawDefectMap(defectContoursList, cellX, cellY);
        }
        private List<string> ReadFile(string filePath)
        {
            List<string> lineData = new List<string>();
            if (!System.IO.File.Exists(filePath))
            {
                return lineData;
            }
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fsRead, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        string lineInfo = sr.ReadLine().Trim();
                        lineInfo = lineInfo.Replace(" ", "");
                        lineData.Add(lineInfo);//添加每行数据到列表
                    }
                }
            }
            return lineData;
        }
        private void GetDefectContours(List<string> L)
        {
            double scaleX = 1;
            double scaleY = 1;
            List<Point> points = new List<Point>();
            DefectContours defectContours;
            for (int i = 0; i < L.Count; i++)
            {
                string s = L[i];
                if (s.StartsWith("Cell_X"))//记录cell尺寸
                {
                    string[] cellSizeInfo = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    scaleX = Convert.ToDouble(cellSizeInfo[0].Substring(7)) / (double)cellX;
                    scaleY = Convert.ToDouble(cellSizeInfo[1].Substring(7)) / (double)cellY;
                }
                else if (s.StartsWith("No="))//
                {
                    string[] defectInfo = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int defectNum = Convert.ToInt32(defectInfo[0].Split(new char[] { '=' })[1]);
                    int pattern = Convert.ToInt32(defectInfo[1].Split(new char[] { '=' })[1]);
                    int camera = Convert.ToInt32(defectInfo[2].Split(new char[] { '=' })[1]);
                    int defect = Convert.ToInt32(defectInfo[3].Split(new char[] { '=' })[1]);
                    defectContours = new DefectContours(points.ToArray());
                    defectContoursList.Add(defectContours);
                    points.Clear();
                }
                else
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
                    if (i == L.Count - 1)
                    {
                        defectContours = new DefectContours(points.ToArray());
                        defectContoursList.Add(defectContours);
                    }
                }
            }
        }
        private Bitmap DrawDefectMap(List<Point[]> pointList, int length, int width)
        {
            int cellX = length;//Cell X 尺寸
            int cellY = width;//Cell Y 尺寸
            //创建画图对象
            Graphics g;
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(69, 99, 73));
            defectMap = new Bitmap(cellX, cellY);//指明画布大小
            g = Graphics.FromImage(defectMap);//指明画图对象
            Rectangle r = new Rectangle(new Point(0, 0), new Size(cellX, cellY));
            g.FillRectangle(solidBrush, r);//添加背景颜色
            Pen defectDrawPen = new Pen(Color.White);
            defectDrawPen.Width = (float)1.6;
            foreach (Point[] p in pointList)
            {
                if (p.Length > 2)
                {
                    g.DrawPolygon(defectDrawPen, p);
                }
            }
            return defectMap;
        }
        private Bitmap DrawDefectMap(List<DefectContours> defectContoursList, int length, int width)
        {
            List<Point[]> pointList = new List<Point[]>();
            foreach (DefectContours defectContours in defectContoursList)
            {
                pointList.Add(defectContours.points);
            }
            return DrawDefectMap(pointList, length, width); ;
        }
    }
    class DefectContours
    {
        int defectNum = 0;
        int pattern = 0;
        int camera = 0;
        int defect = 0;
        public Point[] points;
        public DefectContours(Point[] pointArray)
        {
            points = pointArray;
        }
    }
}
