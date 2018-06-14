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

        public static PointF plt = new PointF(FieldLocations.LeftPeneltyArea+10, FieldLocations.TopPeneltyArea+10);
        public static PointF plb = new PointF(FieldLocations.LeftPeneltyArea+10, FieldLocations.ButtomPeneltyArea-10);
        public static PointF prt = new PointF(FieldLocations.RightPeneltyArea-10, FieldLocations.TopPeneltyArea+10);
        public static PointF prb = new PointF(FieldLocations.RightPeneltyArea-10, FieldLocations.ButtomPeneltyArea-10);

        public static PointF plc = new PointF(FieldLocations.LeftPeneltyArea + 2, 0);
        public static PointF prc = new PointF(FieldLocations.RightPeneltyArea - 2, 0);

        public static PointF clt = new PointF(-10, 10);
        public static PointF clb = new PointF(-10, -10);
        public static PointF crt = new PointF(10, 10);
        public static PointF crb = new PointF(10, -10);

        public static PointF goal_l = new PointF(FieldLocations.LeftLine, 0);
        public static PointF goal_r = new PointF(FieldLocations.RightLine, 0);

        public static PointF center = new PointF(0, 0);

    }
}
