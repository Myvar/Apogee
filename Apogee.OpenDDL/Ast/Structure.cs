using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Apogee.OpenDDL.Ast
{
    public class Structure
    {
        public Structure OwnerStructure { get; set; }

        public bool IsValue { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        
        public Dictionary<string, Value> Arguments { get; set; } = new Dictionary<string, Value>();

        public List<Structure> Body { get; set; } = new List<Structure>();

        public void ParseParameters(string raw)
        {
            //(key = "distance")
            raw = raw.Remove(0, 1);

            int state = 0;
            var name = new StringBuilder();
            var value = new StringBuilder();
            
            for (int i = 0; i < raw.Length; i++)
            {
                var c = raw[i];
                
                switch (state)
                {
                        case 0:
                            if (c == ' ')
                            {
                                state = 1;
                            }
                            else
                            {
                                name.Append(c);
                            }
                            break;
                        case 1:
                            if (c == '=')
                            {
                                state = 2;
                            }
                            break;
                        case 2:

                            if (c == ',' || c == ')')
                            {
                                Arguments.Add(name.ToString().Trim(), Value.Parse(value.ToString()));
                                name.Clear();
                                value.Clear();
                            }
                            else
                            {
                                value.Append(c);
                            }
                            
                            
                            break;
                            
                }
            }
            
        }
        
    }
}