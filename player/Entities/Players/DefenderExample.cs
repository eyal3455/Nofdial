using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
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
                SeenObject goalArea = null;
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
                    // look for the ball
                    while (ball == null || ball.Distance > Consts.KICKABLE_AREA)
                    {
                        //m_memory.waitForNewInfo();
                        ball = m_memory.GetSeenObject("ball");
                        if (ball == null)
                        {
                            // If you don't know where is ball then find it
                            m_robot.Turn(90);
                            m_memory.waitForNewInfo();
                        }
                        else if (ball.Distance.Value > Consts.KICKABLE_AREA)
                        {
                            // If ball is too far then
                            // turn to ball or 
                            // if we have correct direction then go to ball
                            if (Math.Abs((double)ball.Direction) > 2)
                                m_robot.Turn(ball.Direction.Value);
                            else
                            {
                                // if ball on our side , go to him, otherwise go to penalty area
                                if (IsBallOnOurSide)
                                    m_robot.Dash(10 * ball.Distance.Value);
                                //m_robot.Say("Running...");
                            }
                        }
                    }

                    // We know where is ball and we can kick it
                    // so look for goal
                    goal = null;
                    int cnt = 0;
                    while (goal == null && cnt < 4)
                    {
                        m_memory.waitForNewInfo();
                        if (m_side == 'l')
                            goal = m_memory.GetSeenObject("goal r");
                        else
                            goal = m_memory.GetSeenObject("goal l");

                        if (goal == null)
                        {
                            cnt++;
                            m_robot.Turn(90);
                        }
                    }

                    // if did not find goal -look for mid field
                    if (goal == null) 
                    {
                        cnt = 0;
                        while (fieldCenter == null && cnt < 4)
                        {
                            fieldCenter = m_memory.GetSeenObject("flag c");

                            if (fieldCenter == null)
                            {
                                cnt++;
                                m_robot.Turn(90);

                            }
                        }
                    }

                    if (goal != null)
                        m_robot.Kick(100, goal.Direction.Value);
                    else
                        if (fieldCenter != null)
                        m_robot.Kick(100, fieldCenter.Direction.Value);

                }
                else // ball on opponent side - go to penalty area
                {
                    string flag = GetDefFlag();
                    
                    int cnt1 = 0;
                    goalArea = null;
                    while (goalArea == null && cnt1 < 4)
                    {
                        m_robot.SenseBody();
                        m_memory.waitForNewInfo();
                        Console.WriteLine("Look for Penalty, Direction {0}", this.m_memory.getBodyInfo().Turn);

                        goalArea = m_memory.GetSeenObject(flag);
                        if (goalArea == null)
                        {
                            cnt1++;
                            m_robot.Turn(90);
                            m_memory.waitForNewInfo();
                        }
                    }

                    if (goalArea != null)
                    {
                        m_robot.Turn(goalArea.Direction.Value);
                        m_memory.waitForNewInfo();

                        m_robot.Dash(100);

                        m_robot.SenseBody();
                        m_memory.waitForNewInfo();
                        Console.WriteLine("Move to Penalty, Direction {0}", this.m_memory.getBodyInfo().Turn);
                    }
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
