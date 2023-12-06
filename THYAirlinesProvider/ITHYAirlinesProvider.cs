using THYAirlines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EBBuildClient.Core;

namespace THYAirlines.Provider
{
    public interface ITHYAirlinesProvider
    {
        public Task<bool> InsertRecentSearchesFlightShopAsync(string customerId, string loyaltyId, string deDupKey, string transactionId, DateTime? searchTimeStamp, DateTime? loginStamp, string useToken, string sessionToken, string logDataValue, string clientip, string deviceid, string origin = null, string destination = null, string departDate = null);

        public List<LedgerListResponseDTO> GetLedgerStats();

        public Task<int> GetRecentSearchesCountByOriginDestDepartDateAsync(string origin, string destination, string departDate);
        public Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesDDoSByOriginDestDepartDateAsync(string deviceIDValue);
        public Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesFlightShopByDeviceOrLoyaltyIdAsync(string deviceId, string loyaltyId);
        public Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesFlightShopByFuzzyMatchingAsync(string iataCode);
        public Task<List<Recent_Searches_Flight_Shop>> GetHotMarketsByLoyaltyIdAsync(string loyaltyId, string origin);


        public Task<bool> UpdateRecentSearchesFlightShopByKeyDeviceIdAsync(string key, string deviceId, string isFavorite);
        public Task<bool> UpdateRecentSearchesFlightShopByKeyLoyaltyIdAsync(string key, string loyaltyId, string isFavorite);


        public Task<bool> DeleteRecentSearchesFlightShopByDeviceIdAsync(string deviceId);
        public Task<bool> DeleteRecentSearchesFlightShopByLoyaltyIdAsync(string loyaltyId);

    }
}
