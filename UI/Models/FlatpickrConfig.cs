using System;
using System.Collections.Generic;


namespace UI.Models;

public class FlatpickrConfig
{
    public string AltFormat { get; set; }
    public string AltInputClass { get; set; }
    public bool? AltInput { get; set; }
    public bool? AllowInput { get; set; }
    public bool? AllowInvalidPreload { get; set; }
    public bool? ClickOpens { get; set; }
    public string AriaDateFormat { get; set; }
    public string Conjunction { get; set; }
    public string DateFormat { get; set; }
    public object DefaultDate { get; set; }
    public int? DefaultHour { get; set; }
    public int? DefaultMinute { get; set; }
    public List<string> Disable { get; set; }
    public bool? DisableMobile { get; set; }
    public List<string> Enable { get; set; }
    public bool? EnableTime { get; set; }
    public bool? EnableSeconds { get; set; }
    public string FormatDate { get; set; }
    public int? HourIncrement { get; set; }
    public bool? Inline { get; set; }
    public DateTime? MaxDate { get; set; }
    public DateTime? MinDate { get; set; }
    public int? MinuteIncrement { get; set; }
    public string Mode { get; set; }
    public string Locale { get; set; }
    public bool? NoCalendar { get; set; }
    public bool? WeekNumbers { get; set; }
}