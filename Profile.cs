using PluginSDK;

namespace ProgrammerChatbot
{
    public class Profile : IProfile
    {
        public string Name { get; private set; }

        public bool Sex { get; set; }

        public Profile(string name)
        {
            Name = name;
        }
    }
}
