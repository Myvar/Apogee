using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Apogee.OpenDDL
{
    public class OpenDDLDocument
    {
        public Structure Root { get; set; } = new Structure();

        public OpenDDLDocument()
        {
        }


        public OpenDDLDocument(string fileName)
        {
            var root = new Structure();
            Parse(File.ReadAllText(fileName), ref root);
            Root = root;
        }


        public Structure GetStructureByType(string type)
        {
            return IterateByType(type, Root);
        }
        
        public Structure GetStructureByID(string type)
        {
            return IterateByID(type, Root);
        }

        private Structure IterateByID(string type, Structure r)
        {

            if (type == r.Name)
            {
                return r;
            }
            
            foreach (var structure in r.Body)
            {
                var z = IterateByID(type, structure);
                if (z != null) return z;
            }
            

            return null;
        }
        
        private Structure IterateByType(string type, Structure r)
        {

            if (type == r.Type)
            {
                return r;
            }
            
            foreach (var structure in r.Body)
            {
                var z = IterateByType(type, structure);
                if (z != null) return z;
            }
            

            return null;
        }
        
        
        public void Parse(string s, ref Structure owner)
        {
            var tmp = new StringBuilder();

            var tmpStruct = new Structure();

            var state = 0;
            var depth = 0;
            var inComment = false;

            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];

                if (s[i] == '/' && s[i + 1] == '/')
                {
                    inComment = true;
                }

                if (!inComment)
                {
                    switch (state)
                    {
                        case 0:
                            if (c == ' ' || c == '{')
                            {
                                tmpStruct.Type = tmp.ToString().Trim();
                                tmp.Clear();

                                state = 1;
                                if (c == '{') i--;
                            }
                            else
                            {
                                tmp.Append(c);
                            }

                            break;
                        case 1:
                            tmp.Append(c);

                            if (c == '{')
                            {
                                var x = tmp.ToString();

                                //we are done with header
                                //or we have data type
                                state = 2;
                                depth = 1;
                                tmp.Clear();
                            }
                            else if (c == ')')
                            {
                                //we have parameters
                                tmpStruct.ParseParameters(tmp.ToString());
                                tmp.Clear();
                            }
                            else if (c == '[')
                            {
                                //we have an array
                            }
                            else if (c == '$')
                            {
                                state = 3;
                            }
                            break;
                        case 2:
                            if (depth == 0)
                            {
                                if (tmpStruct.IsPrimitive())
                                {
                                    tmpStruct.RawValue = tmp.ToString();
                                    tmpStruct.ResolveValue();
                                    tmp.Clear();
                                }
                                else
                                {
                                    var inside = tmp.ToString();
                                    tmp.Clear();
                                    Parse(inside, ref tmpStruct);
                                }

                                owner.Body.Add(tmpStruct);

                                tmpStruct = new Structure();
                                state = 0;
                            }
                            if (c == '{')
                            {
                                depth++;
                            }
                            else if (c == '}')
                            {
                                depth--;
                            }

                            if (depth != 0) tmp.Append(c);

                            break;
                        case 3:
                            if (c == '{')
                            {
                                tmpStruct.Name = tmp.ToString().Trim();
                                state = 2;
                                depth = 1;
                                tmp.Clear();
                            }
                            else
                            {
                                tmp.Append(c);
                            }
                            break;
                    }
                }
                else
                {
                    if (c == '\n')
                    {
                        inComment = false;
                    }
                }
            }

            if (state == 2)
            {
                if (depth == 0)
                {
                    if (tmpStruct.IsPrimitive())
                    {
                        tmpStruct.RawValue = tmp.ToString();
                        tmpStruct.ResolveValue();
                        tmp.Clear();
                    }
                    else
                    {
                        var inside = tmp.ToString();
                        tmp.Clear();
                        Parse(inside, ref tmpStruct);
                    }

                    owner.Body.Add(tmpStruct);

                    tmpStruct = new Structure();
                    state = 0;
                }
            }
        }
    }
}