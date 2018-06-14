using RoboCup;
using RoboCup.Entities;
using RoboCup.Infrastructure;
using System;
using System.Drawing;
using System.Threading;

namespace player.Entities.Players
{
    /// <summary>
    /// Also called goalie, or keeper
    /// The goalkeeper is simply known as the guy with gloves who keeps the opponents from scoring. He has a special position because only him can play the ball with his hands (provided that he is inside his own penalty area and the ball was not deliberately passed to him by a team mate).
    /// Aside from being the last line of defense, the goalkeeper is the first person in attack. That is why keepers who can make good goal kicks and strategic ball throws to team mates are valuable.
    /// The goalie has four main roles: saving, clearing, directing the defense, and distributing the ball. Saving is the act of preventing the ball from entering the net while clearing means keeping the ball far from the goal area.
    /// The goalkeeper has the role of directing the defense since he is the farthest player at the back and he can see where the defenders should position themselves.
    /// Distributing the ball happens when a goalkeeper decides whether to kick the ball or throw it after making a save. Where the keeper throws or kicks the ball is the first instance of attack
    /// </summary>
    public class Goalkeeper : Player
    {
        private const int WAIT_FOR_MSG_TIME = 10;
        private int currentGoalCounter = 0;

        public Goalkeeper(Team team, ICoach coach)
            : base(team, coach, true)
        {
            m_startPosition = new PointF(m_sideFactor * 48, 0);
        }

        public override void play()
        {
            m_robot.Move(m_startPosition.X, m_startPosition.Y);
            bool isInGoal = false;
            while (!m_timeOver)
            {
                m_memory.waitForNewInfo();
                SeenCoachObject ball = FindObjectInView("ball");
                while (ball == null)
                {
                    m_memory.waitForNewInfo();
                    ball = FindObjectInView("ball");
                }
                var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                while (my_data == null)
                {
                    m_memory.waitForNewInfo();
                    my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
                }
                var goalLocation = GetMyGoal();
                var distanceGoalBall = CommonTools.GetDistance(goalLocation, ball.Pos);

                var distanceToBall = CommonTools.GetDistance(my_data.Pos, ball.Pos);
                if (distanceGoalBall < 17)
                {
                    if (distanceToBall < Consts.KICKABLE_AREA)
                    {
                        // catch
                        var directionToBall = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos,
                            ball.Pos.Value.X, ball.Pos.Value.Y);
                        m_robot.Catch(directionToBall);
                        m_memory.waitForNewInfo();
                        
                        // move
                        m_robot.Move(m_startPosition.X, m_startPosition.Y);
                        m_memory.waitForNewInfo();

                        // turn
                        PointF opponentsGoal = GetOpponentsGoal();
                        var directionToOpponentsGoal = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos,
                            opponentsGoal.X, opponentsGoal.Y);
                        /*m_robot.Turn(directionToBall);
                        Thread.Sleep(100);
                        m_memory.waitForNewInfo();*/

                        m_robot.Kick(100, directionToOpponentsGoal);
                    }
                    else
                    {
                        var relativeAngle = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos,
                            ball.Pos.Value.X, ball.Pos.Value.Y);
                        if (Math.Abs(relativeAngle) > 2)
                        {
                            m_robot.Turn(relativeAngle);
                            m_memory.waitForNewInfo();
                        }
                        m_robot.Dash(10 * Math.Pow(distanceToBall,4));
                        isInGoal = false;
                    }
                }
                else
                {
                    MoveToMyGoal();
                }
            }

        }

        private PointF GetOpponentsGoal()
        {
            return m_side == 'l' ? Consts.goal_r : Consts.goal_l;
        }

        private bool IsKickoff()
        {
            if (m_robot.goalCounter_l + m_robot.goalCounter_r > currentGoalCounter)
            {
                currentGoalCounter = m_robot.goalCounter_l + m_robot.goalCounter_r;
                return true;
            }
            return false;
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

        private void MoveToMyGoal()
        {   
            var myGoal = GetMyGoal();
            PointF position = GetRelativePosition(myGoal);
            var my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
            while (my_data == null)
            {
                m_memory.waitForNewInfo();
                my_data = m_coach.GetSeenCoachObject("player " + m_team.m_teamName + " " + m_number);
            }

            double distance = CommonTools.GetDistance(my_data.Pos, position);
            if (distance > 0.1)
            {
                var relativeAngle = CommonTools.GetRelativeAngle(my_data.BodyAngle, my_data.Pos, position.X, position.Y);
                if (Math.Abs(relativeAngle) > 2) {
                    m_robot.Turn(relativeAngle);
                    m_memory.waitForNewInfo();
                }
                m_robot.Dash(10 * Math.Pow(distance,4));
            }
        }

        private PointF GetRelativePosition(PointF myGoal)
        {
            float distFromCenter = 5;
            var ball = m_coach.GetSeenCoachObject("ball");
            if (ball == null)
            {
                return myGoal;
            }
            double tetha = Math.Atan2(ball.Pos.Value.Y - myGoal.Y, ball.Pos.Value.X - myGoal.X);
            return new PointF(myGoal.X + distFromCenter * (float)Math.Cos(tetha),
                myGoal.Y + distFromCenter * (float)Math.Sin(tetha));
        }

        private PointF GetMyGoal()
        {
            return m_side == 'l' ? Consts.goal_l : Consts.goal_r;
        }

        private SeenCoachObject FindObjectInView(string objName)
        {
            return m_coach.GetSeenCoachObject(objName);
            //m_memory.waitForNewInfo();
            //SeenObject myObj = null;
            //while (myObj == null)
            //{
            //    myObj = m_memory.GetSeenObject(objName);
            //    if (myObj == null)
            //    {
            //        m_robot.Turn(10);
            //    }
            //}
            //return myObj;

        }

        private bool IsPointingAt(string objName, double offset)
        {
            m_memory.waitForNewInfo();
            SeenObject myObj = null;

            myObj = m_memory.GetSeenObject(objName);
            if (myObj == null)
            {
                return false;
            }
            return Math.Abs(myObj.Direction.Value) < offset;
        }
    }
}
