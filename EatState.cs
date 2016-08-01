using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerChatbot
{
    public class EatState: Botstate
    {
        private int eatCounter;

        public EatState() : base("Eat")
        {
            eatCounter = 0;
        }

        public override void doAction(Programmer bot)
        {
            eatCounter = eatCounter++ % 900;

            //If the bot is not eating, then the next state for the bot will be the available state
            if ((bot.notEatTime > bot.eatTime && (DateTime.Now.TimeOfDay >= bot.notEatTime || DateTime.Now.TimeOfDay < bot.eatTime)) || (bot.notEatTime < bot.eatTime && (DateTime.Now.TimeOfDay >= bot.notEatTime && DateTime.Now.TimeOfDay < bot.eatTime)))
            {
                bot.setNextState(stateFactory.getBotState("Available", bot));
            }

            //Else if the bot wants a break from eating, it will set the next state for the socialize state or the rest state
            else if (eatCounter == 0) //30 minutes elasped
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
