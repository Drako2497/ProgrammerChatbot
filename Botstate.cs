using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMLbot;
using PluginSDK;

namespace ProgrammerChatbot
{
    public abstract class Botstate
    {
        internal static Flyweightstatefactory stateFactory = new Flyweightstatefactory();

        public string Name
        {
            get;
            private set;
        }

        public Botstate(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Given an input string, return an output from the AIML bot.
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="user">User (so conversations can be tracked by the bot, per user)</param>
        /// <returns>string</returns>
        public string getAIMLResponse(string input, User user, Programmer bot, char punctuation = '.')
        {
            Request r = new Request(input, user, bot);
            Result res = bot.Chat(r);

            return res.Output;
        }

        public abstract void doAction(Programmer bot);

        //Find the user associated to the master account in the primary chat source
        protected User getPrimaryUser(Programmer bot)
        {
            if (bot.PSource.MasterAccount.Owner.Profile != null)
            {
                if (!bot.Users.ContainsKey(bot.PSource.MasterAccount.Owner.Profile.Name))
                    bot.Users.Add(bot.PSource.MasterAccount.Owner.Profile.Name, new User(bot.PSource.MasterAccount.Owner.Profile.Name, bot));
                return bot.Users[bot.PSource.MasterAccount.Owner.Profile.Name];
            }
            else
            {
                if (!bot.Users.ContainsKey(bot.PSource.MasterAccount.Username))
                    bot.Users.Add(bot.PSource.MasterAccount.Username, new User(bot.PSource.MasterAccount.Username, bot));
                return bot.Users[bot.PSource.MasterAccount.Username];
            }
        }
    }
}
