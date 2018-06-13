using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RoboCup.Entities
{
    public class Consts
    {
        public const float KICKABLE_AREA = 0.8f + 0.085f + 1f;
        public const int WAIT_FOR_MSG_TIME = 10;

        public readonly PointF plt = new PointF(-36, 20);
        public readonly PointF plb = new PointF(-36, -20);
        public readonly PointF prt = new PointF(36, 20);
        public readonly PointF prb = new PointF(36, -20);

        public readonly PointF center = new PointF(0, 0);

    }
}
