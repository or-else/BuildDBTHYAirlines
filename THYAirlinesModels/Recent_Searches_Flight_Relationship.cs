using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THYAirlines.Models;

namespace THYAirlines.Models
{
    public static class Recent_Searches_Flight_Relationship
    {

        public static SchemaRelationshipDef GetHotMarketRelationshipDefinition(string filterField , string filterValue)
        {

            List<string> _childFilters = default(List<string>);
            _childFilters = EBIBuildAPIHelper.BuildFilter(_childFilters, filterField, FilterOperation.FUZZYMATCH, filterValue, BooleanOperation.AND);

            List<string> _childFilterFunctions = default(List<string>);
            _childFilterFunctions = EBIBuildAPIHelper.BuildFilterFunction(_childFilterFunctions, new List<string>() { "Destination" }, FilterFunctionOperation.GROUPBY);


            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "RecentSearches2ChildHotMarket",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(Recent_Searches_Flight_Shop_HotMarkets).AssemblyQualifiedName.Substring(0,typeof(Recent_Searches_Flight_Shop_HotMarkets).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(Recent_Searches_Flight_Shop_HotMarkets),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "PK",
                                        TargetSchemaKey = "ParentPK",
                                        Filters= _childFilters,
                                        FilterFunctions = _childFilterFunctions
                                },
                                ChildRelationship = null

                          }
                     }
                 }
            };

            return _schemaDefinition;
        }

    }
}
