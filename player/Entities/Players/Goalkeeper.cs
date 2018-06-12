﻿using RoboCup;
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

        public Goalkeeper(Team team, ICoach coach)
            : base(team, coach)
        {
            m_startPosition = new PointF(m_sideFactor * 30, 20);
        }

        public override void play()
        {
            m_robot.Move(m_startPosition.X, m_startPosition.Y);

            while (!m_timeOver)
            {
                var myGoal = GetMyGoal();
                while (myGoal.Distance.Value > 10)
                {
                    //m_robot.Turn(myGoal.Direction.Value);
                    m_robot.Dash(100);
                    Thread.Sleep(100);
                    myGoal = GetMyGoal();
                }
            }
            // first ,ove to start position
            

            while (!m_timeOver)
            {
                SeenObject ball = null;
                SeenObject goal = null;

                //Get current player's info:
                var bodyInfo = GetBodyInfo();
                Console.WriteLine($"Kicks so far : {bodyInfo.Kick}");

                while (ball == null || ball.Distance > 1.5)
                {
                    //Get field information from god (coach).
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
                        m_robot.Turn(40);
                        m_memory.waitForNewInfo();
                    }
                    else if (ball.Distance > 1.5)
                    {
                        // If ball is too far then
                        // turn to ball or 
                        // if we have correct direction then go to ball
                        if (Math.Abs((double)ball.Direction) < 0)
                            m_robot.Turn(ball.Direction.Value);
                        else
                            m_robot.Dash(10 * ball.Distance.Value);
                    }
                }

                // We know where is ball and we can kick it
                // so look for goal

                while (goal == null)
                {
                    m_memory.waitForNewInfo();
                    if (m_side == 'l')
                        goal = m_memory.GetSeenObject("goal r");
                    else
                        goal = m_memory.GetSeenObject("goal l");

                    if (goal == null)
                    {
                        m_robot.Turn(40);
                    }
                }

                m_robot.Kick(100, goal.Direction.Value);
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

        private SeenObject GetMyGoal()
        {
            SeenObject goal = null;
            while (goal == null)
            {
                m_memory.waitForNewInfo();
                if (m_side == 'l')
                    goal = m_memory.GetSeenObject("goal l");
                else
                    goal = m_memory.GetSeenObject("goal r");

                if (goal == null)
                {
                    m_robot.Turn(10);
                }
            }
            return goal;
        }
    }
}