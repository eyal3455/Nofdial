using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RoboCup
{
    public class Memory
    {
        // Private members
        volatile private VisualInfo m_info;	// place where all information is stored 
        volatile private SenseBodyInfo m_bodyInfo;	// place where all information is stored
        const int SIMULATOR_STEP = 100;
        const int WAIT_FOR_MSG_TIME = 10;

        //---------------------------------------------------------------------------
        // This constructor:
        // - initializes all variables
        public Memory()
        {
        }


        //---------------------------------------------------------------------------
        // This function puts see information into our memory
        public void store(VisualInfo info)
        {
            m_info = info;
            m_info.parse();
        }
        public void store(SenseBodyInfo bodyInfo)
        {
            m_bodyInfo = bodyInfo;
        }

        public SeenObject GetSeenObject(String name)
        {
            SeenObject result = null;
            if (m_info == null)
                waitForNewInfo();
            m_info?.m_seenObjects.TryGetValue(name, out result);
            return result;
        }

        public SenseBodyInfo getBodyInfo()
        {
            return m_bodyInfo;
        }


        //---------------------------------------------------------------------------
        // This function waits for new visual information
        public void waitForNewInfo()
        {
            // first remove old info
            m_info = null;
            // now wait until we get new copy
            while (m_info == null)
            {
                // We can get information faster then 75 miliseconds
                try
                {
                    Thread.Sleep(SIMULATOR_STEP);
                }
                catch (Exception e)
                {
                }
            }
        }
        
        public void clearBodyInfo()
        {
            m_bodyInfo = null;
        }
    }
}
