using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Apogee.OpenDDL
{
    public class Structure
    {
        public Structure OwnerStructure { get; set; }

        public bool IsValue { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }

        public Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        public List<Structure> Body { get; set; } = new List<Structure>();

        public string RawValue { get; set; }

        public object Value { get; set; }


        public bool IsPrimitive()
        {
            var primitive = new[]
            {
                "bool", "int8", "int16", "int32", "int64", "unsigned_int8", "unsigned_int16", "unsigned_int32",
                "unsigned_int64", "half", "float", "double", "float16", "float32", "float64", "string", "ref", "type",

                "b", "i8", "i16", "i32", "i64", "u8", "u16", "u32", "u64", "h", "f", "d", "f16", "f32", "f64", "s", "r",
                "t"
            };

            return primitive.Contains(Type.ToLower().Split('[')[0]);
        }

        public bool IsArray()
        {
            return Type.Contains('[');
        }


        public void ResolveValue()
        {
            IsValue = IsPrimitive();

            if (!IsValue)
            {
                return;
            }

            if (IsArray())
            {
                Value = ParseArray(RawValue.Trim());
            }
            else
            {
                Value = ParseValue(RawValue.Trim(), Type.Trim());
            }
        }

        private List<object> ParseArray(string value)
        {
            if (!IsArray()) return null;

            //break down the type

            var type = Type.Split('[')[0];
            var count = Type.Split('[').Last().Trim(']');

            //parse value
            var re = new List<object>();

            var segs = new List<string>();

            var tmp = new StringBuilder();
            int depth = 0;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (c == '{')
                {
                    depth++;
                }
                if (c == '}')
                {
                    depth--;
                }


                if (c == ',' && depth == 0)
                {
                    segs.Add(tmp.ToString().Trim().Trim(','));
                    tmp.Clear();
                }

                tmp.Append(c);
            }

            segs.Add(tmp.ToString().Trim().Trim(','));


            foreach (var seg in segs)
            {
                var ls = new List<object>();

                var val = seg.Trim().Remove(0, 1);
                val = val.Remove(val.Length - 1, 1);


                foreach (var s in val.Split(','))
                {
                    ls.Add(ParseValue(s.Trim(), type));
                }

                re.Add(ls);
            }


            return re;
        }


        private object ParseValue(string value, string type = "")
        {
            if (string.IsNullOrEmpty(type))
            {
                //non static tped first

                value = value.Trim();

                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    var ret = value.Remove(0, 1);

                    ret = ret.Remove(ret.Length - 1, 1);

                    return ret;
                }
            }
            else
            {
                var fmt = new NumberFormatInfo();
                fmt.NegativeSign = "-";
                fmt.NumberDecimalSeparator = ".";

                //static typed parsing
                switch (type)
                {
                    case "float":
                        return float.Parse(value, fmt);
                    case "unsigned_int32":
                        return uint.Parse(value);
                    case "ref":
                        return value;
                }
            }

            return null;
        }

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
                            Arguments.Add(name.ToString().Trim(), ParseValue(value.ToString()));
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