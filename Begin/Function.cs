using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begin
{
    class Function
    {
        public string name { get; set; }
        public Dictionary<string, string> parameters;  //<name, type>
        public BeginParser.FunctionBodyContext body { get; set; }

        public Function(string name, Dictionary<string, string> parameters, BeginParser.FunctionBodyContext bodyContext)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = bodyContext;
        }
    }
}
