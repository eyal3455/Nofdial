using RoboCup.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RoboCup.Entities.Enums;

namespace RoboCup
{
    public interface IFormation
    {
        List<Player> InitTeam(Strategies stratgey, Team team, Coach m_coach);
    }
}
