using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    interface IMessage
    {
        string Header { get; set; }
        string UserName { get; set; }
    }
}
