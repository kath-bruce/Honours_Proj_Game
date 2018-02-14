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
        public string Event_Text { get; protected set; }
        
        public delegate void EventOption(string option_text, params object[] args); //note not sure what args would be needed

        private Dictionary<EventType, EventOption> event_choices;

        public Event()
        {
            Event_Text = "EVENT TEXT - lorem ipsum....";
            event_choices = new Dictionary<EventType, EventOption>();
        }

        public Event(string text)
        {
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

        public void AddChoice(EventType type, EventOption option, bool override_previous_option)
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
        }

        //todo could potentially add ability to remove an EventOption just using an EventOption variable
        public void RemoveChoice(EventType type, EventOption option)
        {
            if (event_choices.ContainsKey(type))
            {
                event_choices[type] -= option;
            }
        }

        public void RemoveChoice(EventType type)
        {
            event_choices.Remove(type);
        }
    }
}
