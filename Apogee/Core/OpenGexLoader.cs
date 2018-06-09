using System.Collections.Generic;
using Apogee.OpenDDL;

namespace Apogee.Core
{
    public class OpenGexLoader
    {
        private OpenDDLDocument _src;

        public OpenGexLoader(string file)
        {
            _src = new OpenDDLDocument(file);
        }

        public List<List<uint>> GetIndeciesData()
        {
            var re = new List<List<uint>>();

            var GeometryNode = _src.GetStructureByType("GeometryNode");

            Structure geometry = null;

            foreach (var node in GeometryNode.Body)
            {
                if (node.Type == "ObjectRef")
                {
                    var value = node.Body[0].Value;
                    geometry = _src.GetStructureByID(value.ToString());
                }
            }

            foreach (var structure in geometry.Body[0].Body)
            {
                if (structure.Type == "IndexArray")
                {
                    re.Add(IndexStructureToArray(structure));
                }
            }

            return re;
        }

        private List<uint> IndexStructureToArray(Structure r)
        {
            var re = new List<uint>();

            foreach (var x in r.Body[0].Value as List<object>)
            {
                var triangle = x as List<object>;
                foreach (var o in triangle)
                {
                    re.Add((uint) o);
                }
            }


            return re;
        }

        public Vertex[] GetVertexData()
        {
            var re = new List<Vertex>();

            var GeometryNode = _src.GetStructureByType("GeometryNode");

            Structure geometry = null;

            foreach (var node in GeometryNode.Body)
            {
                if (node.Type == "ObjectRef")
                {
                    var value = node.Body[0].Value;
                    geometry = _src.GetStructureByID(value.ToString());
                }
            }

            foreach (var structure in geometry.Body[0].Body)
            {
                if (structure.Type == "VertexArray")
                {
                    if (structure.Arguments["attrib"].ToString() == "position")
                    {
                        foreach (List<object> o in (List<object>) structure.Body[0].Value)
                        {
                            var p = new Vec3((float) o[0], (float) o[1], (float) o[2]);
                            re.Add(new Vertex(p));
                        }
                    }
                }
            }

            int c = 0;

            foreach (var structure in geometry.Body[0].Body)
            {
                if (structure.Type == "VertexArray")
                {
                    if (structure.Arguments["attrib"].ToString() == "texcoord")
                    {
                        foreach (List<object> o in (List<object>) structure.Body[0].Value)
                        {
                            var p = new Vec2((float) o[0], (float) o[1]);
                            re[c].TexCoord = p;
                            c++;
                        }
                    }
                }
            }


            return re.ToArray();
        }
    }
}