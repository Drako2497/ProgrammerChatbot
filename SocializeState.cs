using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;
using AIMLbot;

namespace ProgrammerChatbot
{
    public class SocializeState: Botstate
    {
        public SocializeState() : base("Socialize")
        {

        }

        public override void doAction(Chatbot bot)
        {
            lock (bot.OutgoingMessage)
            {
                UserAccount user;

                do
                {
                    user = bot.PSource.Accounts[new Random().Next(0, bot.PSource.Accounts.Count)];
                } 
                
                //The bot will send a message to the user if the user hasn't send a message for a certain amount of time
                while (!(user.Owner is IFollower && user.Owner != bot));
                {
                    bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], user, new TextMessage(getAIMLResponse("SayStatement What are you doing", getPrimaryUser(bot), bot, '?'))));
                }
                
            }

            bot.setNextState(stateFactory.getBotState("Available", bot));
        }
    }
}
