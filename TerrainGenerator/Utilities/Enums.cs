﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topographer3D.Utilities
{
    internal enum Layer
    {
        Height,
        Slope,
        Island,
        OpenSimplex,
        Voronoi,
        Hydraulic,
        DetailColorization
    }

    public enum ApplicationMode
    {
        Normal,
        Add,
        Subtract,
        Multiply,
    }

    internal enum Direction
    {
        X_Axis,
        Z_Axis
    }

    public enum InterpolationMode
    {
        Linear,
        Smooth
    }
}