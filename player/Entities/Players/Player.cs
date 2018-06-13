using System;
using System.Drawing;
using System.Threading;
using RoboCup;
using RoboCup.Entities;

namespace RoboCup
{

    public class Player : IPlayer, ISensorInput
    {
        // Protected members
        protected Robot m_robot;			    // robot which is controled by this brain
        protected Memory m_memory;				// place where all information is stored
        protected PointF m_startPosition;
        volatile protected bool m_timeOver;
        protected Thread m_strategy;

        public enum PlayerSide { LEFT, RIGHT };
        protected PlayerSide m_PlayerSide;

        protected int m_sideFactor
        {
            get
            {
                return m_side == 'r' ? 1 : -1;
            }
        }

        // Public members
        public int m_number;
        public char m_side;
        public String m_playMode;
        public Team m_team;
        public ICoach m_coach;

        public Player(Team team, ICoach coach)
        {
            m_coach = coach;
            m_memory = new Memory();
            m_team = team;
            m_robot = new Robot(m_memory);
            m_robot.Init(team.m_teamName, out m_side, out m_number, out m_playMode);

            Console.WriteLine("New Player - Team: " + m_team.m_teamName + " Side:" + m_side +" Num:" + m_number);

            m_strategy = new Thread(new ThreadStart(play));
            m_strategy.Start();
        }

        public virtual  void play()
        {
 
        }

        protected bool BallInSideOurHalf(PointF? pos)
        {
            if (pos == null)
                return false;

            if (m_side == 'l' && pos.Value.X <= 0)
                return true;

            if (m_side == 'r' && pos.Value.X >= 0)
                return true;

            return false;
        }

        public void see(VisualInfo info)
        {
            throw new NotImplementedException();
        }
        // This function receives hear information from player
        public void hear(int time, int direction, string message)
        {
            Console.WriteLine("team %d number %d time %d direction: %d message: %s ", m_team.m_teamName, m_number, time, direction, message);
            
        }

        public void hear(int time, string message)
        {
            throw new NotImplementedException();
        }
    }
}
