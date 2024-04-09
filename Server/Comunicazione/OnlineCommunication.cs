using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Comunicazione
{
    abstract class OnlineCommunication
    {
        abstract public void Run();
        abstract protected Task HandleRequests();
    }
}
