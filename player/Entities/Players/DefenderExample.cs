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
        
        public DefenderExample(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 0);
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

                while (ball == null || ball.Distance > Consts.KICKABLE_AREA)
                {
                    var ballPosByCoach = m_coach.GetSeenCoachObject("ball");
                    if (ballPosByCoach != null && ballPosByCoach.Pos != null)
                    {
                        Console.WriteLine($"Ball Position {ballPosByCoach.Pos.Value.X}, {ballPosByCoach.Pos.Value.Y}");
                    }

                    m_memory.waitForNewInfo();
                    ball = m_memory.GetSeenObject("ball");
                    if (ball == null)
                    {
                        // If you don't know where is ball then find it
                        m_robot.Turn(90);
                        m_memory.waitForNewInfo();
                    }
                    else if (ball.Distance.Value > Consts.KICKABLE_AREA )
                    {
                        // If ball is too far then
                        // turn to ball or 
                        // if we have correct direction then go to ball
                        if (Math.Abs((double)ball.Direction) > 2)
                        {
                            m_robot.Turn(ball.Direction.Value);

                        }
                        else
                        {
                            // if ball on our side , go to him, otherwise go to penalty area
                            if(ballPosByCoach!=null && BallInSideOurHalf(ballPosByCoach.Pos))
                                m_robot.Dash(10 * ball.Distance.Value);
                            else
                            {
                                var my_data = m_coach.GetSeenCoachObject("player "+m_team.m_teamName+" "+m_number);
                                var angleToTurn = GetRelativeAngle(my_data.BodyAngle, my_data.Pos,0,0);

                                m_robot.Turn(angleToTurn);
                                m_memory.waitForNewInfo();
                                m_robot.Dash(100);


                                // ball on opponent side - go to penalty area
                                //string flag;
                                //if (m_side == 'l')
                                //    flag = "flag p l c";
                                //else
                                //    flag = "flag p r c";

                                //int cnt1 = 0;
                                //goalArea = null;
                                //while(goalArea == null && cnt1 < 4)
                                //{
                                //    goalArea = m_memory.GetSeenObject(flag);
                                //    if (goalArea == null)
                                //    {
                                //        cnt1++;
                                //        m_robot.Turn(90);
                                //        m_memory.waitForNewInfo();
                                //    }
                                //}

                                //if (goalArea != null)
                                //{
                                //    m_robot.Turn(goalArea.Direction.Value);
                                //    m_memory.waitForNewInfo();

                                //    m_robot.Dash(100);
                                //}


                            }
                            //m_robot.Say("Running...");
                        }
                    }
                    //else if (ball.Distance.Value >= 30)
                    //{
                    //    // go to mid own rehave
                    //    string flag;
                    //    if (m_side == 'l')
                    //        flag = "flag p r c";
                    //    else
                    //        flag = "flag p l c";

                    //    goalArea = m_memory.GetSeenObject(flag);

                    //    if (goalArea == null)
                    //    {
                    //        m_coach.GetSeenCoachObject(flag);
                    //        //Get field information from god (coach).
                    //        var flagPos = m_coach.GetSeenCoachObject(flag);
                    //        if (flagPos != null && flagPos.Pos != null)
                    //        {
                    //            m_robot.Dash(flagPos.Direction.Value);
                    //        }
                    //    }
                    //    else
                    //        m_robot.Dash(goalArea.Direction.Value);
                    //}
                }

                // We know where is ball and we can kick it
                // so look for goal
                goal = null;
                int cnt = 0;
                while (goal==null && cnt < 4)
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

                if(goal == null) // if did not find goal -look for mid field
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

        private double GetRelativeAngle(float? bodyAngle, PointF? my_pos , int trg__pos_x, int trg__pos_y)
        {

            double direction = Math.Atan2(trg__pos_y - my_pos.Value.Y, trg__pos_x - my_pos.Value.X) / Math.PI * 360;
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
