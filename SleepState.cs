using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class SleepState : Botstate
    {
        public SleepState() : base("Sleep")
        {

        }

        public override void doAction(Programmer bot)
        {
            //If the bot is active, switch to the greetingstate
            if ((bot.wakeUpTime > bot.sleepTime && (DateTime.Now.TimeOfDay >= bot.wakeUpTime || DateTime.Now.TimeOfDay < bot.sleepTime)) || (bot.wakeUpTime < bot.sleepTime && (DateTime.Now.TimeOfDay >= bot.wakeUpTime && DateTime.Now.TimeOfDay < bot.sleepTime)))
            {
                bot.setNextState(stateFactory.getBotState("Greeting", bot));
            }
        }
    }
}
