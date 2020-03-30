using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ONSLoader.Console.Extensions
{
    public static class ProjectionUtils
    {

        public static readonly Dictionary<string, CoordinateSystem> Transforms = new Dictionary<string, CoordinateSystem> {
            { "BNG", EPSG_27700() },
            { "EPSG:27700", EPSG_27700() },
            { "EPSG:4326", EPSG_4326() },
            { "WGS84", EPSG_4326() }
        };


        public static CoordinateSystem EPSG_27700()
        {
            var wkt = @"PROJCS[""OSGB 1936 / British National Grid"",GEOGCS[""OSGB 1936"",DATUM[""OSGB_1936"",SPHEROID[""Airy 1830"",6377563.396,299.3249646,AUTHORITY[""EPSG"",""7001""]],TOWGS84[446.448,-125.157,542.06,0.15,0.247,0.842,-20.489],AUTHORITY[""EPSG"",""6277""]],PRIMEM[""Greenwich"",0,AUTHORITY[""EPSG"",""8901""]],UNIT[""degree"",0.0174532925199433,AUTHORITY[""EPSG"",""9122""]],AUTHORITY[""EPSG"",""4277""]],PROJECTION[""Transverse_Mercator""],PARAMETER[""latitude_of_origin"",49],PARAMETER[""central_meridian"",-2],PARAMETER[""scale_factor"",0.9996012717],PARAMETER[""false_easting"",400000],PARAMETER[""false_northing"",-100000],UNIT[""metre"",1,AUTHORITY[""EPSG"",""9001""]],AXIS[""Easting"",EAST],AXIS[""Northing"",NORTH],AUTHORITY[""EPSG"",""27700""]]";
            return EPSGWkt(wkt);
        }

        public static CoordinateSystem EPSG_4326()
        {
            var wkt = @"GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0,AUTHORITY[""EPSG"",""8901""]],UNIT[""degree"",0.0174532925199433,AUTHORITY[""EPSG"",""9122""]],AUTHORITY[""EPSG"",""4326""]]";
            return EPSGWkt(wkt);
        }

        public static CoordinateSystem EPSGWkt(string wkt)
        {
            CoordinateSystemFactory csFact = new CoordinateSystemFactory();
            return csFact.CreateFromWkt(wkt);
        }

        public static double[] GetCoordinates(this Coordinate coord)
        {
            return new double[] { coord.X, coord.Y };
        }

        public static IList<double[]> GetCoordinates(this IEnumerable<Coordinate> coords)
        {
            return coords.Select(x => x.GetCoordinates()).ToList();
        }

        public static Coordinate Transform(this Coordinate coords, CoordinateSystem from, CoordinateSystem to)
        {
            CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();
            ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(from, to);
            return coords.Transform(trans);
        }

        public static Coordinate Transform(this Coordinate coords, ICoordinateTransformation transformer)
        {
            var c = transformer.MathTransform.Transform(coords.GetCoordinates());
            return new Coordinate
            {
                X = c[0],
                Y = c[1]
            };
        }

        public static ICoordinateTransformation GetTransformer(string from, string to)
        {
            CoordinateSystem f = Transforms[from];
            CoordinateSystem t = Transforms[to];
            return GetTransformer(f, t);
        }

        public static ICoordinateTransformation GetTransformer(CoordinateSystem from, CoordinateSystem to)
        {
            CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();
            ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(from, to);
            return trans;
        }


        public static IEnumerable<Coordinate> Transform(this IEnumerable<Coordinate> coords, string from, string to)
        {
            CoordinateSystem f = Transforms[from];
            CoordinateSystem t = Transforms[to];
            return coords.Transform(f, t);
        }

        public static IEnumerable<Coordinate> Transform(this IEnumerable<Coordinate> coords, CoordinateSystem from, CoordinateSystem to)
        {
            return coords.Select(x => x.Transform(from, to)).ToList();
        }


        public static Coordinate OSRefToWGS84(double easting, double northing)
        {
            return OSRefToWGS84(new Coordinate(easting, northing));
        }

        public static Coordinate OSRefToWGS84(this Coordinate coord)
        {
            var transformer = GetTransformer(EPSG_27700(), EPSG_4326());
            return coord.Transform(transformer);
        }

        public static Coordinate WGS84ToOSRef(double latitude, double longitude)
        {
            return WGS84ToOSRef(new Coordinate(longitude, latitude));
        }

        public static Coordinate WGS84ToOSRef(this Coordinate coord)
        {
            var transformer = GetTransformer(EPSG_4326(), EPSG_27700());
            return coord.Transform(transformer);
        }

        public static Geometry Transform(Geometry geometry, ICoordinateTransformation transformer)
        {
            if (geometry is null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (transformer is null)
            {
                throw new ArgumentNullException(nameof(transformer));
            }

            geometry = geometry.Copy();
            geometry.Apply(new MathTransformFilter(transformer.MathTransform));
            return geometry;
        }

        private sealed class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _transform;

            public MathTransformFilter(MathTransform transform) => _transform = transform;

            public bool Done => false;
            public bool GeometryChanged => true;

            public void Filter(CoordinateSequence seq, int i)
            {
                double x = seq.GetX(i);
                double y = seq.GetY(i);
                double z = seq.GetZ(i);
                _transform.Transform(ref x, ref y, ref z);
                seq.SetX(i, x);
                seq.SetY(i, y);
                seq.SetZ(i, z);
            }
        }
    }
}
