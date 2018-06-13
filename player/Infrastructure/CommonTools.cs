using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.Infrastructure
{
    public static class CommonTools
    {

        //get relative angle to target x,y
        public static double GetRelativeAngle(float? bodyAngle, PointF? pos, float trg__pos_x, float trg__pos_y)
        {

            double direction = Math.Atan2(trg__pos_y - pos.Value.Y, trg__pos_x - pos.Value.X) / Math.PI * 360;
            double angleToTurn = direction - bodyAngle.Value;

            if (angleToTurn > 180)
            {
                return angleToTurn - 360;
            }
            else if (angleToTurn < -180)
            {
                return angleToTurn + 360;

            }
            return angleToTurn;
        }

        public static double GetDistance(PointF? obj1, PointF? obj2)
        {

            double result1 =  Math.Pow(obj1.Value.Y - obj2.Value.Y, 2);
            double result2 = Math.Pow(obj1.Value.X - obj2.Value.X, 2);
            return Math.Sqrt(result1 + result2);
        }
    }
}
