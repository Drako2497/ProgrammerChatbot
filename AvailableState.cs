﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSDK;
using AIMLbot;

namespace ProgrammerChatbot
{
    public class AvailableState: Botstate
    {
        private int attentionCounter;
        private int boredCounter;

        public AvailableState(): base("Available")
        {
            attentionCounter = 0;
        }

        //Reset the attention counter
        public void resetAttentionCounter()
        {
            attentionCounter = 0;
        }

        public override void doAction(Chatbot bot)
        {
            boredCounter = ++boredCounter % 450;

            //Go to the sleep state if the bot has its bedtime
            if ((bot.wakeUpTime > bot.sleepTime && (DateTime.Now.TimeOfDay < bot.wakeUpTime && DateTime.Now.TimeOfDay >= bot.sleepTime)) || (bot.wakeUpTime < bot.sleepTime && (DateTime.Now.TimeOfDay < bot.wakeUpTime || DateTime.Now.TimeOfDay >= bot.sleepTime)))
            {
                attentionCounter = ++attentionCounter % 150;

                if (attentionCounter == 0) //5 minutes elapsed
                {
                    //The bot says good night
                    lock (bot.OutgoingMessage)
                    {
                        bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatementPattern Good night", getPrimaryUser(bot), bot))));
                    }

                    bot.setNextState(stateFactory.getBotState("Sleep", bot));
                }
            }

            else if ((bot.offWorkTime > bot.workTime && (DateTime.Now.TimeOfDay < bot.offWorkTime && DateTime.Now.TimeOfDay >= bot.workTime)) || (bot.offWorkTime < bot.workTime && (DateTime.Now.TimeOfDay < bot.offWorkTime || DateTime.Now.TimeOfDay >= bot.workTime)))
            {
                attentionCounter = ++attentionCounter % 150;

                if (attentionCounter == 0) //5 minutes elapsed
                {
                    //The bot says it's time for work
                    lock (bot.OutgoingMessage)
                    {
                        bot.OutgoingMessage.Enqueue(new ChatMessage(bot.PSource, bot.retrieve(bot.PSource)[0], bot.PSource.MasterAccount, new TextMessage(getAIMLResponse("SayStatement Time for work", getPrimaryUser(bot), bot))));
                    }

                    bot.setNextState(stateFactory.getBotState("Busy", bot));
                }
            }

            //Go to socialize state if the bot gets bored
            else if (boredCounter == 0) //15 minutes elapsed
            {
                bot.setNextState(stateFactory.getBotState("Socialize", bot));
            }
        }
    }
}
