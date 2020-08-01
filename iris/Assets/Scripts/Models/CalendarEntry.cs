using System;
using System.Collections.Generic;

public class CalendarEntry
{
    public DateTime Start {get;set;}
    public DateTime End {get;set;}
    public List<string> Reminders {get;set;}
    public string Description {get;set;}
}
