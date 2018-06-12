using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RoboCup.Entities.Enums;

namespace RoboCup
{
    public class Formation_4_4_2 : IFormation 
    {
        public Formation_4_4_2() 
        {
        }

        public List<Player> InitAttackers(Team team, ICoach coach)
        {
            var players = new List<Player>();
            players.Add(new AttackerExample(team, coach));
            players.Add(new AttackerExample(team, coach));
            players.Add(new AttackerExample(team, coach));
            players.Add(new AttackerExample(team, coach));

            return players;
        }

        public List<Player> InitDefenders(Team team, ICoach coach)
        {
            var players = new List<Player>();
            players.Add(new DefenderExample(team, coach));
            players.Add(new DefenderExample(team, coach));
            players.Add(new DefenderExample(team, coach));
            players.Add(new DefenderExample(team, coach));

            return players;
        }

        public List<Player> InitTeam(Strategies stratgey, Team team, Coach coach)
        {
            switch (stratgey)
            {
                case Enums.Strategies.Attack:
                    return InitAttackers(team, coach);
                case Enums.Strategies.Defend:
                    return InitDefenders(team, coach);
            }

            var players = new List<Player>();
            players.Add(new DefenderExample(team, coach));
            players.Add(new AttackerExample(team, coach));

            return players;
        }
    }
}