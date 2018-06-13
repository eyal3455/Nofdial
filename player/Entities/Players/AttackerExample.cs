using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Drawing;
using System.Threading;

namespace RoboCup
{
    public class AttackerExample : Player
    {
        public AttackerExample(Team team, ICoach coach, PlayerSide side)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 0);
            m_PlayerSide = side;
        }

        public override void play()
        {
            // first ,ove to start position
            //m_robot.Move(m_startPosition.X, m_startPosition.Y);
            while (!m_timeOver)
            {
                //Get current player's info:
                //var bodyInfo = GetBodyInfo();
                //Console.WriteLine($"Kicks so far : {bodyInfo.Kick}");

                var ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                if (ballPosByCoach == null)
                {
                    m_memory.waitForNewInfo();
                    ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                }
                if (ballPosByCoach != null && ballPosByCoach.Pos != null)
                {
                    Console.WriteLine($"Ball Position {ballPosByCoach.Pos.Value.X}, {ballPosByCoach.Pos.Value.Y}");
                }
                bool IsBallOnOurSide = BallInSideOurHalf(ballPosByCoach.Pos);

                if (!IsBallOnOurSide || (ballPosByCoach.Pos.Value.X == 0 && ballPosByCoach.Pos.Value.Y == 0))
                {
                    var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                    if (my_data == null)
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

                        var distToGoal = CommonTools.GetDistance(my_data.Pos, opponentGoal);
                        if (distToGoal < 20)
                        {
                            Console.WriteLine("before update angle {0}", angleToGoal);
                            var fix = (my_data.Pos.Value.Y > 0) ? 5 : -5;
                            angleToGoal = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, opponentGoal.X, opponentGoal.Y + fix);
                            Console.WriteLine("after update angle {0}", angleToGoal);
                        }
                        m_robot.Kick(100, angleToGoal);
                    }
                }
                else // ball on opponent side - go to defenders area
                {
                    string flag = GetAttFlag();

                    PointF flag_pos = GetFlagPos(flag);

                    var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                    var angleToTurn = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, flag_pos.X, flag_pos.Y);
                    var dist = CommonTools.GetDistance(my_data.Pos, flag_pos);

                    if (Math.Abs(angleToTurn) > 1)
                    {
                        m_robot.Turn(angleToTurn);
                        m_memory.waitForNewInfo();
                    }
                    m_robot.Dash(10 * dist);
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
                case "flag c l t":
                    return Consts.clt;
                case "flag c l b":
                    return Consts.clb;
                case "flag c r t":
                    return Consts.crt;
                case "flag c r b":
                    return Consts.crb;

                default:
                    throw new Exception("invalid flag");
            }
        }

        private string GetAttFlag()
        {
            string flag;
            if (m_side == 'l')
            {
                if (m_PlayerSide == PlayerSide.LEFT)
                    flag = "flag c r t";
                else
                    flag = "flag c r b";
            }
            else
            {
                if (m_PlayerSide == PlayerSide.LEFT)
                    flag = "flag c l b";
                else
                    flag = "flag c l t";
            }
            return flag;
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
