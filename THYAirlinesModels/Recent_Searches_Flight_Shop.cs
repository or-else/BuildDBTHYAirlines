using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using EBBuildClient.Core.Interfaces;

namespace THYAirlines.Models
{
    
    public class Recent_Searches_Flight_Shop: CommonInterfaces
    {

        //Based on DevieID
        [JsonPropertyName("PK")]
        public string PK { get; set; }

        // Based on SessionID
        [JsonPropertyName("SK")]
        public string SK { get; set; }


        [JsonPropertyName("CUSTOMERID")] 
        public string Recent_Searches_FShop_Customer_Id { get; set; }


        [JsonPropertyName("LOYALTYID")]
        public string Recent_Searches_FShop_Loyalty_Id { get; set; }


        [JsonPropertyName("KEY")] // PK
        public string Recent_Searches_FShop_Key { get; set; }


        [JsonPropertyName("TRANSACTIONID")] // SK
        public string Recent_Searches_FShop_Transaction_Id { get; set; }


        [JsonPropertyName("TIMESTAMPT")]
        public DateTime Recent_Searches_FShop_Search_Time_Stamp { get; set; }


        [JsonPropertyName("LOGINTIMESTAMP")]
        public DateTime Recent_Searches_FShop_Login_Stamp { get; set; }


        [JsonPropertyName("USETOKEN")]
        public string Recent_Searches_FShop_Use_Token { get; set; }


        [JsonPropertyName("SESSIONTOKEN")]
        public string Recent_Searches_FShop_Session_Token { get; set; }


        [JsonPropertyName("LOGDATA")]
        public string Recent_Searches_FShop_LogData_Value { get; set; }


        [JsonPropertyName("CLIENTIP")]
        public string Recent_Searches_FShop_Client_IP { get; set; }


        [JsonPropertyName("DEVICEID")]
        public string Recent_Searches_FShop_Device_Id { get; set; }


        [JsonPropertyName("ISFAVORITE")]
        public Boolean Recent_Searches_FShop_IsFavorite { get; set; }

        [JsonPropertyName("COUNTER")]
        public string Recent_Searches_FShop_Counter { get; set; }


        [JsonPropertyName("ISACTIVE")]
        public Boolean IsActive { get; set; }


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
        public Dictionary<string, List<dynamic>> ChildRelationships { get; set; }
    }
}
