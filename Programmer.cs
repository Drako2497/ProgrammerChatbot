using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Timers;
using AIMLbot;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class Programmer : Bot, IFollowerPlugin
    {
        public enum Botstatus { Available, Away, Busy, Sleep };

        protected Botstate currentState;
        protected Botstate nextState;

        private string configFile;

        public TimeSpan wakeUpTime { get; private set; }
        public TimeSpan sleepTime { get; private set; }
        public TimeSpan workTime { get; private set; }
        public TimeSpan offWorkTime { get; private set; }
        public TimeSpan eatTime { get; private set; }
        public TimeSpan notEatTime { get; private set; }

        public SortedList<string, IChatSource> Sources { get; private set; }
        public SortedList<string, User> Users { get; private set; }

        private Timer stateTimer;

        public string Role { get; private set; }

        public string Nickname { get; private set; }

        public Color FontColor { get; protected set; }

        public Queue<ChatMessage> OutgoingMessage { get; private set; }

        public IProfile Profile { get; protected set; }

        public string AuthorName { get; private set; }

        public string PluginName { get; private set; }

        public string PluginVersion { get; private set; }

        private Botstatus status;
        private SortedList<string, UserAccount> accounts;

        public IChatSource PSource { get; private set; }

        class CProfile: IProfile
        {
            public CProfile()
            {
                Name = "Alexander";
            }

            public string Name
            {
                get; private set;
            }
        }

        public Programmer(string configFile)
        {
            this.configFile = configFile;
            initialize();

            OutgoingMessage = new Queue<ChatMessage>();

            status = Botstatus.Sleep;
            Users = new SortedList<string, User>();

            Profile = new Profile(GlobalSettings.grabSetting("name"));
            wakeUpTime = Convert.ToDateTime(GlobalSettings.grabSetting("waketime")).TimeOfDay;
            sleepTime = Convert.ToDateTime(GlobalSettings.grabSetting("sleeptime")).TimeOfDay;
            workTime = Convert.ToDateTime(GlobalSettings.grabSetting("worktime")).TimeOfDay;
            offWorkTime = Convert.ToDateTime(GlobalSettings.grabSetting("offworktime")).TimeOfDay;
            eatTime = Convert.ToDateTime(GlobalSettings.grabSetting("eattime")).TimeOfDay;
            notEatTime = Convert.ToDateTime(GlobalSettings.grabSetting("notEatTime")).TimeOfDay;

            currentState = Botstate.stateFactory.getBotState("Initial", this);

            stateTimer = new Timer();
            stateTimer.Interval = 2000; //2 seconds
            stateTimer.Elapsed += new ElapsedEventHandler(stateTimer_Elapsed);

            Sources = new SortedList<string, IChatSource>();

            accounts = new SortedList<string, UserAccount>();

            AuthorName = "Gordon Ngo";
            PluginName = "Programmer Chatbot";
            PluginVersion = "1.0";
            FontColor = Color.DarkSlateGray;
            Nickname = "Alex";
            Profile = new CProfile();
            Role = "Programmer";
        }

        /// <summary>
        /// Loads AIML files located in the AIML folder.
        /// </summary>
        private void initialize()
        {
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", configFile));
                loadSettings(path);
                isAcceptingUserInput = false;
                loadAIMLFromFiles();
                isAcceptingUserInput = true;
            }
            catch (FileNotFoundException)
            {
                //System.Windows.MessageBox.Show(ex.Message);
                //Terminate program
            }
        }

        //Elapsed the stateTimer to execute the stateloop
        public void stateTimer_Elapsed(object Sender, ElapsedEventArgs e)
        {
            stateLoop();
        }

        //Make the current state into the next state
        private void stateLoop()
        {
            if (nextState != null)
            {
                if (nextState is BusyState)
                {
                    Status = Botstatus.Busy;
                }

                else if (nextState is RestState)
                {
                    Status = Botstatus.Away;
                }

                else if (nextState is SleepState)
                {
                    Status = Botstatus.Sleep;
                }

                else
                {
                    Status = Botstatus.Available;
                }

                currentState = nextState;
                nextState = null;
            }
            currentState.doAction(this);
        }

        //Gets the status of the bot
        public Botstatus Status
        {
            get
            {
                return status;
            }
            protected set
            {
                if (status != value)
                {
                    status = value;

                    foreach (UserAccount account in accounts.Values)
                    {
                        switch (status)
                        {
                            case Botstatus.Busy:
                                account.Status = ChatStatus.Busy;
                                break;
                            case Botstatus.Away:
                                account.Status = ChatStatus.Idle;
                                break;
                            case Botstatus.Sleep:
                                account.Status = ChatStatus.Offline;
                                break;
                            default:
                                account.Status = ChatStatus.Available;
                                break;
                        }
                    }
                }
            }
        }

        //Setting the next state
        public void setNextState(Botstate newState)
        {
            nextState = newState;
        }

        //Start the statetimer
        public void start()
        {
            stateTimer.Start();
        }

        //To join the chat
        public void join(IChatSource source)
        {
            if (!Sources.ContainsKey(source.SourceName))
            {
                if (PSource == null)
                    PSource = source;
                Sources.Add(source.SourceName, source);
            }
        }

        public void loadDatabase()
        {

        }

        //Listens to the message being received from the sender
        public void listen(ChatMessage message)
        {
            string response;

            //If the message is send to a new sender in the chat, then it will get a a response
            if (message.Sender.Owner != null)
            {
                if (!Users.ContainsKey(message.Sender.Owner.Profile.Name))
                    Users.Add(message.Sender.Owner.Profile.Name, new User(message.Sender.Owner.Profile.Name, this));
                response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Owner.Profile.Name], this);
            }

            else
            {
                if (!Users.ContainsKey(message.Sender.Username))
                    Users.Add(message.Sender.Username, new User(message.Sender.Username, this));
                response = currentState.getAIMLResponse(message.Content.Message, Users[message.Sender.Username], this);
            }

            if (!response.Equals(""))
            {
                lock (OutgoingMessage)
                {
                    OutgoingMessage.Enqueue(new ChatMessage(message.Source, message.Recipient, message.Sender, new TextMessage(response)));
                }
            }
        }

        //Adding a new account to the accounts list
        public void add(UserAccount account)
        {
            accounts.Add(account.SourceName, account);
        }

          //Retrieves the source of an account
        public IList<UserAccount> retrieve(IChatSource source)
        {
            //Handles one account per chat source.
            List<UserAccount> list = new List<UserAccount>();
            list.Add(accounts[source.SourceName]);
            return list;
        }

        public void receive(ChatMessage message)
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
