using MongoDB.Driver.GeoJsonObjectModel;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class coordinationDocument
    {
        public string _id { get; set; }
        public string name { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates>  location { get; set; }       
    }
    
   
}
