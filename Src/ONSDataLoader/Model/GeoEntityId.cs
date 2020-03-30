using System;
using System.Diagnostics.CodeAnalysis;

namespace ONSLoader.Console.Model
{
    public class GeoEntityId : IEquatable<GeoEntityId>
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Reference { get; set; }

        public bool Equals([AllowNull] GeoEntityId other)
        {
            //Check whether the compared object is null. 
            if (other is null) return false;

            //Check whether the compared object references the same data. 
            if (ReferenceEquals(this, other)) return true;

            return Name.Equals(other.Name) && DataType.Equals(other.DataType);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = Name == null ? 0 : Name.GetHashCode();

            //Get hash code for the Code field. 
            int hashDataType = DataType.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashDataType;
        }
    }
}
