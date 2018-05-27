using System.Collections.Generic;

namespace P7.Core.Settings
{
    public class ControllerNode
    {
        public string Controller { get; set; }
        public List<ActionNode> Actions { get; set; }
    }
}