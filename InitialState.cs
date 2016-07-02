using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class InitialState : Botstate
    {
        public InitialState() : base("Initial")
        {

        }

        public override void doAction(Chatbot bot)
        {
            //If the bot is not active, it will set the next state to the sleepstate
            if ((bot.wakeUpTime > bot.sleepTime && (DateTime.Now.TimeOfDay < bot.wakeUpTime && DateTime.Now.TimeOfDay >= bot.sleepTime)) || (bot.wakeUpTime < bot.sleepTime && (DateTime.Now.TimeOfDay < bot.wakeUpTime || DateTime.Now.TimeOfDay >= bot.sleepTime)))
            {
                bot.setNextState(stateFactory.getBotState("Sleep", bot));
            }

            //Else the bot will greet you
            else
            {
                bot.setNextState(stateFactory.getBotState("Greeting", bot));
            }
        }
    }
}
