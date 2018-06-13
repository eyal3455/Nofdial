using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RoboCup.Entities
{
    public static class Consts
    {
        public const float KICKABLE_AREA = 0.8f + 0.085f + 1f;
        public const int WAIT_FOR_MSG_TIME = 10;

        public static PointF plt = new PointF(FieldLocations.LeftPeneltyArea, FieldLocations.TopPeneltyArea);
        public static PointF plb = new PointF(FieldLocations.LeftPeneltyArea, FieldLocations.ButtomPeneltyArea);
        public static PointF prt = new PointF(FieldLocations.RightPeneltyArea, FieldLocations.TopPeneltyArea);
        public static PointF prb = new PointF(FieldLocations.RightPeneltyArea, FieldLocations.ButtomPeneltyArea);

        public static PointF goal_l = new PointF(FieldLocations.LeftLine, 0);
        public static PointF goal_r = new PointF(FieldLocations.RightLine, 0);

        public static PointF center = new PointF(0, 0);

    }
}
