using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using PluginSDK;
using AIMLbot;

namespace ProgrammerChatbot
{
    public class Programmer : Profiler
    {
        public Programmer() : base()
        {
            FontColor = Color.DarkSlateGray;
        }

        public override void receive(ChatMessage message)
        {
            //If status of the bot is not sleeping or the user sends a message, then the bot will response
            if (Status != Botstatus.Sleep || message.Sender.Username.Equals("Boss"))
            {
                string response;

                //Makes a new chat sender if the sender is not active
                if (message.Sender.Owner != null)
                {
                    if (!Users.ContainsKey(message.Sender.Owner.Profile.Name))
                    {
                        Users.Add(message.Sender.Owner.Profile.Name, new User(message.Sender.Owner.Profile.Name, this));
                        response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Owner.Profile.Name], this);
                    }

                }

                else
                {
                    if (!Users.ContainsKey(message.Sender.Username))
                    {
                        Users.Add(message.Sender.Username, new User(message.Sender.Username, this));
                        response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Username], this);
                    }

                }

                //If the bot is not in the available state, make the current state the avaiable state
                if (!(currentState is AvailableState))
                {
                    nextState = Botstate.stateFactory.getBotState("Available", this);
                }
                
                //Else, reset the the attention counter on the current state
                else
                {
                    ((AvailableState)currentState).resetAttentionCounter();
                }
                    
            }
        }
    }
}
