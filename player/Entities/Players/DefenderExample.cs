using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Drawing;
using System.Threading;

namespace RoboCup
{
    public class DefenderExample : Player
    {
        public enum DefenderSide { LEFT, RIGHT };
        private DefenderSide m_DefSide;
        public DefenderExample(Team team, ICoach coach, DefenderSide side)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 0);
            m_DefSide = side;
        }

        public override void play()
        {
            // first ,ove to start position
            //m_robot.Move(m_startPosition.X, m_startPosition.Y);

            while (!m_timeOver)
            {
                SeenObject ball = null;
                SeenObject goal = null;
                //SeenObject goalArea = null;
                SeenObject fieldCenter = null;

                //Get current player's info:
                //var bodyInfo = GetBodyInfo();
                //Console.WriteLine($"Kicks so far : {bodyInfo.Kick}");

                var ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                if (ballPosByCoach != null && ballPosByCoach.Pos != null)
                {
                    Console.WriteLine($"Ball Position {ballPosByCoach.Pos.Value.X}, {ballPosByCoach.Pos.Value.Y}");
                }
                bool IsBallOnOurSide = BallInSideOurHalf(ballPosByCoach.Pos);

                if (IsBallOnOurSide)
                {
                    var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                    if(my_data == null)
                    {
                        m_memory.waitForNewInfo();
                        my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                    }
                    var angleToTurn = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, ballPosByCoach.Pos.Value.X, ballPosByCoach.Pos.Value.Y);

                    if (Math.Abs(angleToTurn) > 2)
                    {
                        m_robot.Turn(angleToTurn);
                        m_memory.waitForNewInfo();
                    }

                    var distToBall = CommonTools.GetDistance(my_data.Pos, ballPosByCoach.Pos);
                    // if the ball far, dash to it
                    if (distToBall > Consts.KICKABLE_AREA)
                    {
                        m_robot.Dash(10 * distToBall);
                    }
                    else // if the ball is close - kick it
                    {
                        PointF opponentGoal = m_side == 'l' ? Consts.goal_r : Consts.goal_l;
                        var angleToGoal = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, opponentGoal.X, opponentGoal.Y);

                        //m_robot.Turn(angleToGoal);
                        //m_memory.waitForNewInfo();
                        m_robot.Kick(100, angleToGoal);
                    }
                }
                else // ball on opponent side - go to defenders area
                {
                    string flag = GetDefFlag();

                    PointF flag_pos = GetFlagPos(flag);

                    var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                    var angleToTurn = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, flag_pos.X, flag_pos.Y);

                    m_robot.Turn(angleToTurn);
                    m_memory.waitForNewInfo();
                    m_robot.Dash(100);
                }


                // sleep one step to ensure that we will not send
                // two commands in one cycle.
                try
                {
                    Thread.Sleep(SoccerParams.simulator_step);
                }
                catch (Exception e)
                {

                }
            }
        }

        private PointF GetFlagPos(string flag)
        {
            switch (flag)
            {
                case "flag p l t":
                    return Consts.plt;
                case "flag p l b":
                    return Consts.plb;
                case "flag p r t":
                    return Consts.prt;
                case "flag p r b":
                    return Consts.prb;

                default:
                    throw new Exception("invalid flag");
            }
        }

        private string GetDefFlag()
        {
            string flag;
            if (m_side == 'l')
            {
                if (m_DefSide == DefenderSide.LEFT)
                    flag = "flag p l t";
                else
                    flag = "flag p l b";
            }
            else
            {
                if (m_DefSide == DefenderSide.LEFT)
                    flag = "flag p r b";
                else
                    flag = "flag p r t";
            }
            return flag;
        }

        private bool BallInSideOurHalf(PointF? pos)
        {
            if (pos == null)
                return false;

            if (m_side == 'l' && pos.Value.X <= 0)
                return true;

            if (m_side == 'r' && pos.Value.X >= 0)
                return true;

            return false;
        }

        
        private SenseBodyInfo GetBodyInfo()
        {
            m_robot.SenseBody();
            SenseBodyInfo bodyInfo = null;
            while (bodyInfo == null)
            {
                Thread.Sleep(Consts.WAIT_FOR_MSG_TIME);
                bodyInfo = m_memory.getBodyInfo();
            }

            return bodyInfo;
        }

     }
}
