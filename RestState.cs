using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;
using AIMLbot;

namespace ProgrammerChatbot
{
    public class RestState: Botstate
    {
        private int restCounter;

        public RestState() : base("Rest")
        {
            restCounter = 0;
        }

        public override void doAction(Chatbot bot)
        {
            restCounter = ++restCounter % 150;

            if (restCounter == 0) //5 minutes elapsed
            {
                //The bot will say back to work
                lock (bot.OutgoingMessage)
                {
                    bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatement Back to work", getPrimaryUser(bot), bot))));
                }

                bot.setNextState(stateFactory.getBotState("Busy", bot));
            }
        }
    }
}
