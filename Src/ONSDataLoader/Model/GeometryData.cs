using NetTopologySuite.Geometries;

namespace ONSLoader.Console.Model
{
    public class GeometryData
    {
        public string Name;
        public string DataType;
        public string Reference;
        public Geometry Geom;
    }
}
