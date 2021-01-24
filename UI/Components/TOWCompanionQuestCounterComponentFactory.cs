using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(TOWQuestCounterComponentFactory))]

namespace LiveSplit.UI.Components
{
	public class TOWQuestCounterComponentFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "TOW Companion Quest Counter"; }
        }

        public string Description
        {
            get { return "The Outer Worlds companion quest completion counter."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Information; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new TOWQuestCounterComponent(state);
        }

        public string UpdateName
        {
            get { return this.ComponentName; }
        }

        public string UpdateURL
        {
            get { return null; }
        }

        public Version Version
        {
            get { return Version.Parse("1.0.0"); }
        }

        public string XMLURL
        {
            get { return null; }
        }
    }
}