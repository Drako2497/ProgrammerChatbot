using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMLbot;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class Flyweightstatefactory
    {
        private SortedList<string, Botstate> flyweights;

        public Flyweightstatefactory()
        {
            flyweights = new SortedList<string, Botstate>();
        }

        //Gets the botstate from the bot
        public Botstate getBotState(string stateName, Programmer bot)
        {
            if (flyweights.ContainsKey(stateName))
            {
                 return flyweights[stateName];
            }  

            //If it goes to special state
            else if (flyweights.ContainsKey(bot.Profile.Name + stateName))
            {
                return flyweights[bot.Profile.Name + stateName];
            }
                
            //If flyweight is not found
            else
            {
                Botstate newState;
                
                switch (stateName)
                {
                    case "Initial":
                        newState = new InitialState();
                        break;
                    case "Greeting":
                        newState = new GreetingState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Available":
                        newState = new AvailableState();
                        flyweights.Add(bot.Profile.Name + stateName, newState);
                        break;
                    case "Socialize":
                        newState = new SocializeState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Busy":
                        newState = new BusyState();
                        flyweights.Add(bot.Profile.Name + stateName, newState);
                        break;
                    case "Rest":
                        newState = new RestState();
                        flyweights.Add(bot.Profile.Name + stateName, newState);
                        break;
                    case "Sleep":
                        newState = new SleepState();
                        flyweights.Add(stateName, newState);
                        break;
                    case "Eat":
                        newState = new EatState();
                        flyweights.Add(stateName, newState);
                        break;
                    default:
                        newState = null;
                        break;
                }

                if (newState != null)
                {
                    flyweights.Add(stateName, newState);
                }
                    
                return newState;
            }
        }
    }
}
