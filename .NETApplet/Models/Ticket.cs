using System;

namespace NETApplet
{
    public class Ticket
    {
        public decimal Exp { get; set; }
        public string App { get; set; }
        public string[] Scope { get; set; }
        public string Grant { get; set; }
        public string User { get; set; }
        public string Key { get; set; }
        public string Algorithm { get; set; }
        public object Ext { get; set; }
        public string Id { get; set; }
    }
}
