namespace Apogee.OpenDDL.Ast
{
    public class Value
    {
        public object ParsedValue { get; set; }

        public static Value Parse(string raw)
        {
            raw = raw.Trim();
            
            var re = new Value();

            if (raw.StartsWith("\""))
            {
                re.ParsedValue = raw.Trim('"');
            }
            
            return re;
        }
    }
}