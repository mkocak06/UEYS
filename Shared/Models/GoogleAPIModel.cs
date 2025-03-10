using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class GoogleAPIModel
    {
        public List<GoogleResult> Results { get; set; }
        public string Status { get; set; }
    }
    public class GoogleResult
    {
        public List<GoogleAddressComponent> Address_Components { get; set; }
        public string Formatted_Address { get; set; }
        public GoogleGeometry Geometry { get; set; }
        public bool Partial_Match { get; set; }
        public string Place_Id { get; set; }
        public GooglePlusCode Plus_Code { get; set; }
        public List<string> Types { get; set; }
    }
    public class GoogleAddressComponent
    {
        public string Long_Name { get; set; }
        public string Short_Name { get; set; }
        public List<string> Types { get; set; }
    }
    public class GoogleGeometry
    {
        public GoogleLocation Location { get; set; }
        public string Location_Type { get; set; }
        public Viewport Viewport { get; set; }
    }
    public class GooglePlusCode
    {
        public string Compound_Code { get; set; }
        public string Global_Code { get; set; }
    }
    
    public class Viewport
    {
        public GoogleLocation NorthEast { get; set; }
        public GoogleLocation SouthWest { get; set; }
    }
    public class GoogleLocation
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
    }
}
