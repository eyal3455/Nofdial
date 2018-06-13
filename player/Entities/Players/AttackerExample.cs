using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using RoboCup.Infrastructure;


namespace RoboCup
{
    public class AttackerExample : Player
    {

        private const int WAIT_FOR_MSG_TIME = 10;

        public AttackerExample(Team team, ICoach coach) 
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 0);
        }

        public override void play()
        {

            // first ,ove to start position
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            SeenObject obj,obj_flag;
            
            while (!m_timeOver)
            {
                //var bodyInfo = GetBodyInfo();
                
                obj = m_memory.GetSeenObject("ball");
                if (obj == null)
                {
                    // If you don't know where is ball then find it
                    m_robot.Turn(80);
                    m_memory.waitForNewInfo();
                }
                else if (obj.Distance.Value > Consts.KICKABLE_AREA)
                {
                    // If ball is too far then
                    // turn to ball or 
                    // if we have correct direction then go to ball
                    if (obj.Direction.Value > Math.Abs(2))
                        m_robot.Turn(obj.Direction.Value);
                    else
                        m_robot.Dash(10 * obj.Distance.Value);
                }
                else
                {
                    // We know where is ball and we can kick it
                    // so look for goal
                    if (m_side == 'l')
                    {

                        obj = m_memory.GetSeenObject("goal r");
                        obj_flag = m_memory.GetSeenObject("flag p r c");
                    }
                    else{
                        obj = m_memory.GetSeenObject("goal l");
                        obj_flag = m_memory.GetSeenObject("flag p l c");

                    }

                    if (obj == null)
                    {
                        if(obj_flag != null)
                        {
                            m_robot.Kick(100, obj_flag.Direction.Value);
                        }
                        
                        m_robot.Turn(80);
                        m_memory.waitForNewInfo();
                    }
                    else
                        m_robot.Kick(100, obj.Direction.Value);
                }

                // sleep one step to ensure that we will not send
                // two commands in one cycle.
                try
                {
                    Thread.Sleep(2 * SoccerParams.simulator_step);
                }
                catch (Exception e) 
                {

                }
            }
        }

        private SenseBodyInfo GetBodyInfo()
        {
            m_robot.SenseBody();
            SenseBodyInfo bodyInfo = null;
            while (bodyInfo == null)
            {
                Thread.Sleep(WAIT_FOR_MSG_TIME);
                bodyInfo = m_memory.getBodyInfo();
            }

            return bodyInfo;
        }
    }
}
