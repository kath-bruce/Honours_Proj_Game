using System.Collections;
using System.Collections.Generic;

namespace HonsProj
{
    public enum EventType
    {
        CONTINUE, FIRST_CHOICE, SECOND_CHOICE, THIRD_CHOICE
    }

    public class Event
    {
        public string Event_Name { get; protected set; }
        public string Event_Text { get; protected set; }
        public string Event_Continue_Text { get; protected set; }
        public string Event_First_Choice_Text { get; protected set; }
        public string Event_Second_Choice_Text { get; protected set; }
        public string Event_Third_Choice_Text { get; protected set; }
        
        public delegate void EventOption(/*params object[] args*/); //note not sure what args would be needed

        private Dictionary<EventType, EventOption> event_choices;

        public Event(string name)
        {
            Event_Name = name;
            Event_Text = "EVENT TEXT - lorem ipsum....";

            event_choices = new Dictionary<EventType, EventOption>();
        }

        public Event(string name, string text)
        {
            Event_Name = name;
            Event_Text = text;

            event_choices = new Dictionary<EventType, EventOption>();
        }

        public void SetEventText(string text)
        {
            Event_Text = text;
        }

        public int GetNumberOfSetChoices()
        {
            return event_choices.Count;
        }

        public Dictionary<EventType, EventOption> GetChoices()
        {
            return new Dictionary<EventType, EventOption>(event_choices);
        }

        public void AddChoice(EventType type, EventOption option, bool override_previous_option, string option_text)
        {
            if (event_choices.ContainsKey(type))
            {
                if (override_previous_option)
                {
                    event_choices[type] = option;
                }
                else
                {
                    event_choices[type] += option;
                }
            }
            else
            {
                event_choices.Add(type, option);
            }

            switch (type)
            {
                case EventType.CONTINUE:
                    Event_Continue_Text = option_text;
                    break;
                case EventType.FIRST_CHOICE:
                    Event_First_Choice_Text = option_text;
                    break;
                case EventType.SECOND_CHOICE:
                    Event_Second_Choice_Text = option_text;
                    break;
                case EventType.THIRD_CHOICE:
                    Event_Third_Choice_Text = option_text;
                    break;
                default:
                    break;
            }
        }
        
        public void RemoveChoice(EventType type, EventOption option)
        {
            if (event_choices.ContainsKey(type))
            {
                event_choices[type] -= option;
            }

            switch (type)
            {
                case EventType.CONTINUE:
                    Event_Continue_Text = null;
                    break;
                case EventType.FIRST_CHOICE:
                    Event_First_Choice_Text = null;
                    break;
                case EventType.SECOND_CHOICE:
                    Event_Second_Choice_Text = null;
                    break;
                case EventType.THIRD_CHOICE:
                    Event_Third_Choice_Text = null;
                    break;
                default:
                    break;
            }
        }

        public void RemoveChoice(EventType type)
        {
            event_choices.Remove(type);

            switch (type)
            {
                case EventType.CONTINUE:
                    Event_Continue_Text = null;
                    break;
                case EventType.FIRST_CHOICE:
                    Event_First_Choice_Text = null;
                    break;
                case EventType.SECOND_CHOICE:
                    Event_Second_Choice_Text = null;
                    break;
                case EventType.THIRD_CHOICE:
                    Event_Third_Choice_Text = null;
                    break;
                default:
                    break;
            }
        }
    }
}
