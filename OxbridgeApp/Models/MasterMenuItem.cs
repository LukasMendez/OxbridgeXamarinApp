using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class MasterMenuItem

    {
        public string Text { get; set; }
        public string Detail { get; set; }
        public string ImagePath { get; set; }
        public Type TargetViewModel { get; set; }
    }
}
