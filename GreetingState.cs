using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class GreetingState : Botstate
    {
        public GreetingState(): base("Greeting")
        {

        }

        //Does the greeting to the user and the bot state will be set to the available state
        public override void doAction(Chatbot bot)
        {
            lock (bot.OutgoingMessage)
            {
                bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatementPattern Well hello", getPrimaryUser(bot), bot))));
            }

            bot.setNextState(stateFactory.getBotState("Available", bot));
        }
    }
}
