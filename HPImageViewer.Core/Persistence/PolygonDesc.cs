﻿using System.Windows;

namespace HPImageViewer.Core.Persistence
{
    public class PolygonDesc : ROIDesc
    {
        public List<Point> Vertices { get; set; } = new List<Point>();
        /// <summary>
        /// 是否封闭图形
        /// </summary>
        public bool IsClosed { get; set; }

    }
}