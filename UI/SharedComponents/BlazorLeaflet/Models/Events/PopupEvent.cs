using System;
using System.Collections.Generic;
using System.Text;

namespace UI.SharedComponents.BlazorLeaflet.Models.Events
{
    public class PopupEvent : Event
    {
        public Popup Popup { get; set; }
    }
}
