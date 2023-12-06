using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using EBBuildClient.Core.Interfaces;


namespace THYAirlines.Models
{
    public class Recent_Searches_Flight_Shop_HotMarkets: CommonInterfaces
    {
        //Based on DevieID
        [JsonPropertyName("PK")]
        public string PK { get; set; }

        // Based on SessionID
        [JsonPropertyName("SK")]
        public string SK { get; set; }


        [JsonPropertyName("ParentPK")]
        public string ParentPK { get; set; }
        

        [JsonPropertyName("ORIGIN")]
        public string Origin { get; set; }


        [JsonPropertyName("DESTINATION")]
        public string Destination { get; set; }


        [JsonPropertyName("DEPARTDATE")]
        public string DepartDate { get; set; }

        [JsonPropertyName("TTL")]
        public Nullable<double> TTL { get; set; }


        [JsonPropertyName("BlockName")]
        public string BlockName { get; set; }


        [JsonPropertyName("BlockHashCode")]
        public string BlockHashCode { get; set; }


        [JsonPropertyName("FuzzyMatchRatios")]
        public string FuzzyMatchRatios { get; set; }

        [JsonPropertyName("GroupCount")]
        public string GroupCount { get; set; }


        [JsonPropertyName("ChildRelationships")]       
        Dictionary<string, List<dynamic>> CommonInterfaces.ChildRelationships { get; set; }
    }
}
