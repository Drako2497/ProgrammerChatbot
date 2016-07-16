using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerChatbot
{
    public class BusyState: Botstate
    {
        private int workCounter;

        public BusyState(): base("Busy")
        {
            workCounter = 0;
        }

        public override void doAction(Programmer bot)
        {
            workCounter = workCounter++ % 2700;

            //If the bot is off work, then the next state for the bot will be the available state
            if ((bot.offWorkTime > bot.workTime && (DateTime.Now.TimeOfDay >= bot.offWorkTime || DateTime.Now.TimeOfDay < bot.workTime)) || (bot.offWorkTime < bot.workTime && (DateTime.Now.TimeOfDay >= bot.offWorkTime && DateTime.Now.TimeOfDay < bot.workTime)))
            {
                bot.setNextState(stateFactory.getBotState("Available", bot));
            }

            //Else if the bot wants a break from work, it will set the next state for the socialize state or the rest state
            else if (workCounter == 0) //A 90 minute elasped
            {
                if (new Random().Next(0, 2) == 0)
                {
                    bot.setNextState(stateFactory.getBotState("Socialize", bot));
                }

                else
                {
                    bot.setNextState(stateFactory.getBotState("Rest", bot));
                }
                   
            }
        }
    }
}
