using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using AIMLbot;
using PluginSDK;

namespace ProgrammerChatbot
{
    public class Profiler : Chatbot
    {
        protected SortedList<int, Profile> Profiles { get; private set; }
        
        protected SqlConnection connection;

        public Profiler() : base("ProgrammerSettings.xml")
        {
            Profiles = new SortedList<int, Profile>();
            connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\Programmer.mdf;Integrated Security=True");
        }
    }
}
