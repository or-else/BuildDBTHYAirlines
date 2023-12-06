using THYAirlines.Models;
using EBBuildClient.Core;
using ServiceStack;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ConfigurationRegistry;



namespace THYAirlines.Provider
{
    public class THYAirlinesProvider: ITHYAirlinesProvider
    {
        const Int32 TTL_Days = 90;
        const Int32 MAX_TRAN_REQ = 25;
        const Int32 MAX_RECORDS_RETURNED = 10;
        private readonly ConfigurationRegistry.IStartup _context;
                
        private IConfiguration _configuation { get; set; }
        private readonly EBBuildApiFactory _ebBuildDBApiServiceFactory;
        private readonly string _ebbuildDBApiBaseUri;
        private readonly string _ebbuildDBApiToken;
        private readonly string _ebbuildDBApiRoles;
        private readonly string _ebbuildDBUserEmail;
        private readonly string _ebbuildDBTenantId;
        private readonly string _ebbuildDBLedgerPreface;
        private readonly string _ebbuildDBLedgerEncryption;
        private readonly string _ebbuildDBLedgerName;
        private readonly bool _ebbuildDBUseWebSockets;
        private readonly Int32 _ebbuildDBTimeOut;
        private readonly Int32 _maxConnections;
        private readonly Int32 _maxRecordsReturned;
        private readonly Int32 _asyncScale;
        private readonly string _hostRegistration;



        public THYAirlinesProvider(List<string> Roles = null)
        {

            _context = new ConfigurationRegistry.StartupContext();
            _configuation = ConfigurationRegistry.StartupContext.Configuration;


            _ebbuildDBApiBaseUri = StartupContext.Configuration?.GetValue<string>("EbbuildApiBaseUri");
            _ebbuildDBApiToken = StartupContext.Configuration?.GetValue<string>("EbbuildApiToken");
            _ebbuildDBApiRoles = (Roles == null || Roles?.Count == 0 ? StartupContext.Configuration?.GetValue<string>("EbbuildApiRoles") : String.Join(", ", Roles?.ToArray()));
            _ebbuildDBUserEmail = StartupContext.Configuration?.GetValue<string>("EbbuildUserEmail");
            _ebbuildDBTenantId = StartupContext.Configuration?.GetValue<string>("EbbuildTenantId");
            _ebbuildDBLedgerEncryption = StartupContext.Configuration?.GetValue<string>("EbbuildLedgerEncryption");
            _ebbuildDBLedgerPreface = StartupContext.Configuration?.GetValue<string>("EbbuildLedgerPreface");
            _ebbuildDBLedgerName = string.Format("{0}", StartupContext.Configuration?.GetValue<string>("EbbuildLedgerName"));
            _ebbuildDBUseWebSockets = StartupContext.Configuration.GetValue<bool>("EbbuildUseWebSockets");
            _ebbuildDBTimeOut = StartupContext.Configuration.GetValue<Int32>("EbbuildTimeOut");
            _maxConnections = StartupContext.Configuration.GetValue<Int32>("EbbuildMaxConncetions");
            _maxRecordsReturned = StartupContext.Configuration.GetValue<Int32>("EbbuildMaxRecordsReturned");
            _hostRegistration = StartupContext.Configuration.GetValue<string>("EbbuildRegistrationHost");
            _asyncScale = StartupContext.Configuration.GetValue<Int32>("EbbuildAsyncScale");

            
            
            _ebBuildDBApiServiceFactory = new EBBuildApiFactory(_maxConnections, _configuation, _ebbuildDBApiToken, _ebbuildDBApiRoles, _ebbuildDBUserEmail, _ebbuildDBTenantId, _ebbuildDBLedgerPreface, _ebbuildDBLedgerName, _ebbuildDBApiBaseUri, _maxRecordsReturned, _ebbuildDBUseWebSockets, asyncScale: _asyncScale);
          
            
            InitializeAsync();
                       
        }


        public void InitializeAsync()
        {
            /*/
             * Create a client from the factory.
            /*/
            IEBBuildAPIService _client = _ebBuildDBApiServiceFactory.GetApiClient();


            /*/
             * Check if the user credentials exist.
            /*/
            AuthStatus authMessage =  _client.IsCredentialsValid().Result;

            if (authMessage != AuthStatus.Success && authMessage != AuthStatus.CredentialsNotFound)
            {
                if (authMessage == AuthStatus.TokenInvalid)
                {
                    Debug.WriteLine(string.Format("Your API Token is invalid!"));
                    throw new Exception(string.Format("Your API Token is invalid!"));
                }
                else
                {
                    if (authMessage == AuthStatus.TokenInvalidForEnvironment)
                    {
                        Debug.WriteLine(string.Format("Your API Token is invalid for this cluster environment!"));
                        throw new Exception(string.Format("Your API Token is invalid for this cluster environment!"));
                        
                    }
                    else
                    {
                        if (authMessage == AuthStatus.Failed)
                        {
                            Debug.WriteLine(string.Format("Authentication failed!  See Administrator immediately."));
                            throw new Exception(string.Format("Authentication failed!  See Administrator immediately."));
                        }
                        else
                        {
                            throw new Exception("There was an unhandled exception while attempting to authenticate!");
                        }
                    }
                }
            }


            /*/
             * Check if the desired table ledger exists.
            /*/
            var _recentSearchesLedger = _client.GetAllLedgersAsync(_client.GetLedgerName()).Result;

            /*/
             * Check to see if the table ledger exists in the list of available table ledgers.
            /*/
            if (_recentSearchesLedger.Item2 == null || _recentSearchesLedger.Item2?.ToList().Count == 0)
            {
                /*/
                 * Create the table ledger if it does not exist.
                /*/
                (string errorMessage, CreateBlockchainLedgerResponseDto ledgerResult) = _client.CreateLedgerAsync(_client.GetLedgerName()).Result;


                /*/
                 * If we are unable to create the table ledger on the fly then raise an exception.
                /*/
                if (string.IsNullOrEmpty(errorMessage) == false)
                {
                    throw new Exception(errorMessage);

                }

            }
        }


        public List<LedgerListResponseDTO> GetLedgerStats()
        {
            IEBBuildAPIService _client = _ebBuildDBApiServiceFactory.GetApiClient();


            var _einsteinLedger = _client.GetAllLedgersAsync(_client.GetLedgerName()).Result;



            return _einsteinLedger.Item2.ToList();
        }


        public async Task<bool> InsertRecentSearchesFlightShopAsync(string customerId, string loyaltyId, string deDupKey, string transactionId, DateTime? searchTimeStamp, DateTime? loginStamp, string useToken, string sessionToken, string logDataValue, string clientip, string deviceid, string origin = null, string destination = null, string departDate = null)
        {

            try
            {


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                string blockName = string.Empty;
                string childBlockName = string.Empty;


                List<List<string>> _queryPatterns = new List<List<string>>();
                List<List<string>> _childQueryPatterns = default(List<List<string>>);

                List<string> _queryPatternFunctions = default(List<string>);
                _queryPatternFunctions = EBIBuildAPIHelper.BuildFilterFunction(_queryPatternFunctions, new List<string>() { "DepartDate" }, FilterFunctionOperation.ORDERBY_DESC);


                List<string> _filters = new List<string>();
                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Key", FilterOperation.EQ, deDupKey);



                List<string> _filterFunctions = default(List<string>);




                blockName = String.Format(Guid.NewGuid().ToString());
                Recent_Searches_Flight_Shop rec = new Recent_Searches_Flight_Shop()
                {
                    PK = Guid.NewGuid().ToString(),
                    SK = Guid.NewGuid().ToString(),
                    Recent_Searches_FShop_Customer_Id = customerId,
                    Recent_Searches_FShop_Loyalty_Id = loyaltyId,
                    Recent_Searches_FShop_Key = deDupKey,
                    Recent_Searches_FShop_Transaction_Id = new Guid(transactionId).ToString(),
                    Recent_Searches_FShop_Search_Time_Stamp = (DateTime)searchTimeStamp,
                    Recent_Searches_FShop_Login_Stamp = (loginStamp.HasValue ? loginStamp.Value : default(DateTime)),
                    Recent_Searches_FShop_Use_Token = new Guid(useToken).ToString(),
                    Recent_Searches_FShop_Session_Token = new Guid(sessionToken).ToString(),
                    Recent_Searches_FShop_LogData_Value = this.CompressData(logDataValue),
                    Recent_Searches_FShop_Client_IP = clientip,
                    Recent_Searches_FShop_Device_Id = deviceid,
                    Recent_Searches_FShop_IsFavorite = false,
                    IsActive = false,
                    Origin = origin,
                    Destination = destination,
                    DepartDate = departDate,
                    Recent_Searches_FShop_Counter = "1",
                    TTL = this.GetTTL((string.IsNullOrEmpty(departDate) == false ? Convert.ToDateTime(departDate) : new Nullable<DateTime>())),
                    BlockName = blockName
                };



                childBlockName = String.Format(Guid.NewGuid().ToString());
                Recent_Searches_Flight_Shop_HotMarkets shopChild = new Recent_Searches_Flight_Shop_HotMarkets()
                {
                    PK = Guid.NewGuid().ToString(),
                    SK = Guid.NewGuid().ToString(),
                    ParentPK = rec.PK,
                    DepartDate = rec.DepartDate,
                    Origin = rec.Origin,
                    Destination = rec.Destination,
                    TTL = 90,
                    BlockName = childBlockName
                };

                Debug.WriteLine($"Parent Blockname: {blockName}");
                Debug.WriteLine($"Child Blockname: {childBlockName}");

                _queryPatternFunctions = EBIBuildAPIHelper.BuildFilterFunction(_queryPatternFunctions, new List<string>() { "DepartDate" }, FilterFunctionOperation.ORDERBY_DESC);


                _filters = new List<string>();
                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Key", FilterOperation.EQ, deDupKey);
                _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                if (!string.IsNullOrEmpty(deviceid))
                {
                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Device_Id", FilterOperation.EQ, deviceid);
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);
                }

                if (!string.IsNullOrEmpty(loyaltyId))
                {
                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Loyalty_Id", FilterOperation.EQ, loyaltyId);
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);
                }

                /*/
                 * Save parent.
                /*/
                EBBuildAPIService.SaveDataToLedgerWithNoResponse<Recent_Searches_Flight_Shop>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);



                string childBlockHashKey = string.Empty;


                /*/
                 * Save child.
                /*/
                List<string> _childFilters = default(List<string>);

                _childQueryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_childQueryPatterns, EBIBuildAPIHelper.BuildFilter(_childFilters, "ParentPK", FilterOperation.EQ, shopChild.ParentPK));
                EBBuildAPIService.SaveDataToLedgerWithNoResponse<Recent_Searches_Flight_Shop_HotMarkets>(shopChild, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _childQueryPatterns, _queryPatternFunctions, dBContext, childBlockName, childBlockHashKey);



                return await Task.FromResult(true).ConfigureAwait(false);

            }
            catch (Exception e) when (e is Exception)
            {
                var sb = new StringBuilder();
                sb.Append("Instrumentation Event");
                sb.AppendLine();
                sb.AppendFormat("TransactionId : {0}", transactionId);
                sb.AppendLine();
                sb.AppendFormat("customerId : {0}", customerId);
                sb.AppendLine();
                sb.AppendFormat("loyaltyId : {0}", loyaltyId);
                sb.AppendLine();
                sb.AppendFormat("deDupKey : {0}", deDupKey);
                sb.AppendLine();
                sb.AppendFormat("searchTimeStamp : {0}", searchTimeStamp);
                sb.AppendLine();
                sb.AppendFormat("loginStamp : {0}", loginStamp);
                sb.AppendLine();
                sb.AppendFormat("UseToken : {0}", useToken);
                sb.AppendLine();
                sb.AppendFormat("sessionToken : {0}", sessionToken);
                sb.AppendLine();
                sb.AppendFormat("logDataValue : {0}", logDataValue);
                sb.AppendLine();
                sb.AppendFormat("clientip : {0}", clientip);
                sb.AppendLine();
                sb.AppendFormat("deviceid : {0}", deviceid);
                sb.AppendLine();
                sb.AppendFormat(
                    "Class: RecentSearchProvider, Method: InsertRecentSearchesFlightShopByDeviceId, Params: Exception: {0}, StackTrace: {1}",
                    e, e.StackTrace);


                return false;
            }
        }


        public async Task<int> GetRecentSearchesCountByOriginDestDepartDateAsync(string origin, string destination, string departDate)
        {
            Int32 searchCount = 0;


            IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


            List<string> _filters = new List<string>();
            _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Origin", FilterOperation.EQ, origin);
            _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Destination", FilterOperation.EQ, destination);
            _filters = EBIBuildAPIHelper.BuildFilter(_filters, "DepartDate", FilterOperation.BEGINSWITH, departDate);

            /*/
             * Use the filterFunctions to set groupBy and/or sorting of data
            /*/
            List<string> _filterFunctions = new List<string>();
            _filterFunctions = EBIBuildAPIHelper.BuildFilterFunction(_filterFunctions, new List<string>() { "Origin", "Destination", "DepartDate" }, FilterFunctionOperation.GROUPBY);

            (AuthStatus authStatus, List<Recent_Searches_Flight_Shop> resultList, PaginationDTO paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
            asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
            parentToLazyLoadChildren: null,
            filterConditions: _filters,
            filterFunctions: _filterFunctions,
            relationship: null,
            servicContext: dBContext,            
            refreshCacheResults: false).ConfigureAwait(false);



            foreach (Recent_Searches_Flight_Shop result in resultList)
            {
                searchCount = Convert.ToInt32(result.GroupCount);
            }


            return searchCount;
        }
        public async Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesDDoSByOriginDestDepartDateAsync(string deviceIDValue)
        {
            List<Recent_Searches_Flight_Shop> shopListDoS = new List<Recent_Searches_Flight_Shop>();


            IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory.GetApiClient();


            (AuthStatus authStatus, shopListDoS, PaginationDTO paginationDataList) = await EBBuildAPIService.GetDDoSStatisticsAsync<Recent_Searches_Flight_Shop>(
             asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
             filterKey: "Recent_Searches_FShop_Device_Id",
             filterKeyValue: deviceIDValue,
             DateTimeKey: "Recent_Searches_FShop_Search_Time_Stamp",
             timeInterval: DoSTimeInterval.ELAPSEDMINUTES,
             DDoSTimeIntervalValue: "53100000",
             servicContext: dBContext,
             EnforceCacheResults: false,
             StringNameOfType: typeof(Recent_Searches_Flight_Shop).Name);

            return shopListDoS;
        }


        public async Task<List<Recent_Searches_Flight_Shop>> GetHotMarketsByLoyaltyIdAsync(string loyaltyId , string origin)
        {
            List<Recent_Searches_Flight_Shop> resultList = default(List<Recent_Searches_Flight_Shop>);


            try
            {

                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                /*/
                 * Change to create more granular partitions on the loyaltyID by adding searchdate
                 * This change should increase response times on loyaltyID and alleviate throttling on hot partitions.
                /*/

                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Loyalty_Id", FilterOperation.EQ, loyaltyId, BooleanOperation.AND);

                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: _filterFunctions,
                relationship: Recent_Searches_Flight_Relationship.GetHotMarketRelationshipDefinition("Origin", origin),
                servicContext: dBContext,               
                refreshCacheResults: false).ConfigureAwait(false);


            }
            catch (Exception e) when (e is Exception)
            {

                var logMessage =
                        string.Format(
                            "Class: RecentSearchProvider, Method: GetHotMarketsByLoyaltyIdAsync, Params: loyaltyId{0}, Exception: {1}, StackTrace: {2}",
                            loyaltyId, e, e.StackTrace);

                throw;


            }
            finally
            {

            }

            return resultList;
        }
        public async Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesFlightShopByDeviceOrLoyaltyIdAsync(string deviceId, string loyaltyId) 
        {
            List<Recent_Searches_Flight_Shop> resultList = default(List<Recent_Searches_Flight_Shop>);

            try
            {

                if (string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(loyaltyId))
                    throw new Exception("Either device ID or loyalty ID is required!");


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                List<string> _filters = new List<string>();

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = default(List<string>);

                AuthStatus authStatus = AuthStatus.Success;


                PaginationDTO paginationDataList = default(PaginationDTO);


                if (!string.IsNullOrEmpty(deviceId))
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Device_Id", FilterOperation.EQ, deviceId, BooleanOperation.AND);


                    (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                    asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                    parentToLazyLoadChildren: null,
                    filterConditions: _filters,
                    filterFunctions: _filterFunctions,
                    relationship: null,
                    servicContext: dBContext,                 
                    refreshCacheResults: false).ConfigureAwait(false);
                }
                else
                {
                    if (!string.IsNullOrEmpty(loyaltyId))
                    {
                        /*/
                         * Change to create more granular partitions on the loyaltyID by adding searchdate
                         * This change should increase response times on loyaltyID and alleviate throttling on hot partitions.
                        /*/

                        _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Loyalty_Id", FilterOperation.EQ, loyaltyId, BooleanOperation.AND);


                        (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                        asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                        parentToLazyLoadChildren: null,
                        filterConditions: _filters,
                        filterFunctions: _filterFunctions,
                        relationship: null,
                        servicContext: dBContext,                                               
                        refreshCacheResults: false).ConfigureAwait(false);

                    }
                }
            }
            catch (Exception e) when (e is Exception)
            {

                var logMessage =
                        string.Format(
                            "Class: RecentSearchProvider, Method: GetRecentSearchesFlightShopByDeviceOrLoyaltyIdAsync, Params: loyaltyId{0}, Exception: {1}, StackTrace: {2}",
                            loyaltyId, e, e.StackTrace);

                throw;


            }
            finally
            {

            }

            return resultList;
        }
        public async Task<List<Recent_Searches_Flight_Shop>> GetRecentSearchesFlightShopByFuzzyMatchingAsync(string iataCode)
        {
            List<Recent_Searches_Flight_Shop> resultList = default(List<Recent_Searches_Flight_Shop>);

            try
            {

                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                List<string> _filters = new List<string>();

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = default(List<string>);

                AuthStatus authStatus = AuthStatus.Success;



                PaginationDTO paginationDataList = default(PaginationDTO);


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Destinaiton", FilterOperation.FUZZYMATCH, iataCode, BooleanOperation.AND);


                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: _filterFunctions,
                relationship: null,
                servicContext: dBContext,               
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception e) when (e is Exception)
            {

                var logMessage =
                        string.Format(
                            "Class: RecentSearchProvider, Method: GetRecentSearchesFlightShopByFuzzyMatchingAsync, Params: loyaltyId{0}, Exception: {1}, StackTrace: {2}",
                            iataCode, e, e.StackTrace);

                throw;


            }
            finally
            {

            }

            return resultList;
        }


        public async Task<bool> UpdateRecentSearchesFlightShopByKeyDeviceIdAsync(string key, string deviceId, string isFavorite)
        {

            try
            {

                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filters = new List<string>();
                List<string> _filterFunctions = new List<string>();



                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Key", FilterOperation.EQ, key, BooleanOperation.AND);
                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Device_Id", FilterOperation.EQ, deviceId);



                (AuthStatus authStatus, List<Recent_Searches_Flight_Shop> resultsList, PaginationDTO paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: _filterFunctions,
                relationship: null,
                servicContext: dBContext,
                forceNewLedgerCheck: false,
                nameOfBlock: "",
                refreshCacheResults: false).ConfigureAwait(false);


                List<List<string>> _queryPatterns = new List<List<string>>();
                List<string> _queryPatternFunctions = default(List<string>);

                _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);
                _queryPatternFunctions = EBIBuildAPIHelper.BuildFilterFunction(_queryPatternFunctions, new List<string>() { "DepartDate" }, FilterFunctionOperation.ORDERBY_DESC);



                foreach (var result in resultsList)
                {

                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<Recent_Searches_Flight_Shop>(
                        new Recent_Searches_Flight_Shop()
                        {
                            PK = result.PK,
                            SK = result.SK,
                            Recent_Searches_FShop_Key = result.Recent_Searches_FShop_Key,
                            Recent_Searches_FShop_IsFavorite = (string.Compare(isFavorite, "True", StringComparison.OrdinalIgnoreCase) == 0 ? true : false),
                            DepartDate = result.DepartDate,
                            Destination = result.Destination,
                            IsActive = result.IsActive,
                            Origin = result.Origin,
                            Recent_Searches_FShop_Client_IP = result.Recent_Searches_FShop_Client_IP,
                            Recent_Searches_FShop_Counter = string.Format("{0}", Convert.ToInt32((string.IsNullOrEmpty(result.Recent_Searches_FShop_Counter) == false ? Convert.ToInt32(result.Recent_Searches_FShop_Counter) : 1))),

                            Recent_Searches_FShop_Customer_Id = result.Recent_Searches_FShop_Customer_Id,
                            Recent_Searches_FShop_Device_Id = result.Recent_Searches_FShop_Device_Id,
                            Recent_Searches_FShop_LogData_Value = result.Recent_Searches_FShop_LogData_Value,
                            Recent_Searches_FShop_Login_Stamp = result.Recent_Searches_FShop_Login_Stamp,
                            Recent_Searches_FShop_Loyalty_Id = result.Recent_Searches_FShop_Loyalty_Id,
                            Recent_Searches_FShop_Search_Time_Stamp = result.Recent_Searches_FShop_Search_Time_Stamp,
                            Recent_Searches_FShop_Session_Token = result.Recent_Searches_FShop_Session_Token,
                            Recent_Searches_FShop_Transaction_Id = result.Recent_Searches_FShop_Transaction_Id,
                            Recent_Searches_FShop_Use_Token = result.Recent_Searches_FShop_Use_Token,
                            TTL = (result.TTL != null ? result.TTL : this.GetTTL(null)),
                            BlockName = result.BlockName,
                            BlockHashCode = result.BlockHashCode
                        }, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, result.BlockName, result.BlockHashCode);

                }

                return await Task.FromResult(true).ConfigureAwait(false);

            }
            catch (Exception e) when (e is Exception)
            {
                var logMessage =
                       string.Format(
                           "Class: RecentSearchProvider, Method: UpdateRecentSearchesFlightShopByKeyDeviceId, Params: key{0}, Params: deviceId{1}, Params: isFavorite{2}, Exception: {3}, StackTrace: {4}",
                           key, deviceId, isFavorite, e.Message, e.StackTrace);



                throw new ArgumentNullException(e + logMessage);


            }
        }
        public async Task<bool> UpdateRecentSearchesFlightShopByKeyLoyaltyIdAsync(string key, string loyaltyId, string isFavorite)
        {
            try
            {

                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                List<string> _filters = new List<string>();
                List<string> _filterFunctions = new List<string>();

                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Loyalty_Id", FilterOperation.BEGINSWITH, loyaltyId);
                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Key", FilterOperation.EQ, key);




                (AuthStatus authStatus, List<Recent_Searches_Flight_Shop> resultsList, PaginationDTO paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<Recent_Searches_Flight_Shop>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: _filterFunctions,
                relationship: null,
                servicContext: dBContext,
                forceNewLedgerCheck: false,
                nameOfBlock: "",
                refreshCacheResults: false).ConfigureAwait(false);


                List<List<string>> _queryPatterns = new List<List<string>>();
                List<string> _queryPatternFunctions = default(List<string>);

                _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);
                _queryPatternFunctions = EBIBuildAPIHelper.BuildFilterFunction(_queryPatternFunctions, new List<string>() { "DepartDate" }, FilterFunctionOperation.ORDERBY_DESC);



                foreach (var result in resultsList)
                {

                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<Recent_Searches_Flight_Shop>(
                    new Recent_Searches_Flight_Shop()
                    {
                        PK = result.PK,
                        SK = result.SK,
                        Recent_Searches_FShop_Key = result.Recent_Searches_FShop_Key,
                        Recent_Searches_FShop_IsFavorite = (string.Compare(isFavorite, "True", StringComparison.OrdinalIgnoreCase) == 0 ? true : false),
                        DepartDate = result.DepartDate,
                        Destination = result.Destination,
                        IsActive = result.IsActive,
                        Origin = result.Origin,
                        Recent_Searches_FShop_Client_IP = result.Recent_Searches_FShop_Client_IP,
                        Recent_Searches_FShop_Counter = string.Format("{0}", Convert.ToInt32((string.IsNullOrEmpty(result.Recent_Searches_FShop_Counter) == false ? Convert.ToInt32(result.Recent_Searches_FShop_Counter) : 1))),

                        Recent_Searches_FShop_Customer_Id = result.Recent_Searches_FShop_Customer_Id,
                        Recent_Searches_FShop_Device_Id = result.Recent_Searches_FShop_Device_Id,
                        Recent_Searches_FShop_LogData_Value = result.Recent_Searches_FShop_LogData_Value,
                        Recent_Searches_FShop_Login_Stamp = result.Recent_Searches_FShop_Login_Stamp,
                        Recent_Searches_FShop_Loyalty_Id = result.Recent_Searches_FShop_Loyalty_Id,
                        Recent_Searches_FShop_Search_Time_Stamp = result.Recent_Searches_FShop_Search_Time_Stamp,
                        Recent_Searches_FShop_Session_Token = result.Recent_Searches_FShop_Session_Token,
                        Recent_Searches_FShop_Transaction_Id = result.Recent_Searches_FShop_Transaction_Id,
                        Recent_Searches_FShop_Use_Token = result.Recent_Searches_FShop_Use_Token,
                        TTL = (result.TTL != null ? result.TTL : this.GetTTL(null)),
                        BlockName = result.BlockName,
                        BlockHashCode = result.BlockHashCode


                    }, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, result.BlockName, result.BlockHashCode);

                }

                return await Task.FromResult(true).ConfigureAwait(false);

            }
            catch (Exception e) when (e is Exception)
            {
                var logMessage =
                       string.Format(
                           "Class: RecentSearchProvider, Method: UpdateRecentSearchesFlightShopByKeyloyaltyId, Params: key{0}, Params: loyaltyId{1}, Params: isFavorite{2}, Exception: {3}, StackTrace: {4}",
                           key, loyaltyId, isFavorite, e.Message, e.StackTrace);



                throw new ArgumentNullException(e + logMessage);


            }

        }


        public async Task<bool> DeleteRecentSearchesFlightShopByDeviceIdAsync(string deviceId)
        {

            IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


            List<string> _filters = new List<string>();
            _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Device_Id", FilterOperation.EQ, deviceId);


            EBBuildAPIService.DeleteDataFromLedgerWithNoResponse<Recent_Searches_Flight_Shop>(_filters, dBContext, _ebBuildDBApiServiceFactory.GetAsyncWrapper());


            return await Task.FromResult(true).ConfigureAwait(false);

        }
        public async Task<bool> DeleteRecentSearchesFlightShopByLoyaltyIdAsync(string loyaltyId)
        {

            IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory.GetApiClient();


            List<string> _filters = new List<string>();
            _filters = EBIBuildAPIHelper.BuildFilter(_filters, "Recent_Searches_FShop_Loyalty_Id", FilterOperation.EQ, loyaltyId);

            EBBuildAPIService.DeleteDataFromLedgerWithNoResponse<Recent_Searches_Flight_Shop>(_filters, dBContext, _ebBuildDBApiServiceFactory.GetAsyncWrapper());



            return await Task.FromResult(true).ConfigureAwait(false);

        }



        private double GetTTL(Nullable<DateTime> DepartureDate)
        {
            double ttl = TTL_Days;
           

            return ttl;
        }
        private string CompressData(string data)
        {
            ZstdSharp.Compressor compressor = new ZstdSharp.Compressor();
            string base64Encrypted = Convert.ToBase64String(compressor.Wrap(Encoding.UTF8.GetBytes(data)));

            return base64Encrypted;
        }
        private string DeCompressData(string data)
        {
            string decryptedData = data;

            /*/
             * Determine if the data is a base64 Encoded string that requires decompressing via ZStandard.  Otherwise, this is unencoded and we return the data as it came in.
            /*/
            if (this.IsBase64Encoded(data) == true)
            {
                ZstdSharp.Decompressor deCompressor = new ZstdSharp.Decompressor();
                decryptedData = System.Text.Encoding.UTF8.GetString(deCompressor.Unwrap(Convert.FromBase64String(data)));

                Int32 _decryptCnt = 0;

                while (this.IsBase64Encoded(decryptedData) == true && _decryptCnt < 10)
                {
                    decryptedData = System.Text.Encoding.UTF8.GetString(deCompressor.Unwrap(Convert.FromBase64String(decryptedData)));

                    _decryptCnt++;
                }
            }

            return decryptedData;
        }
        private bool IsBase64Encoded(string data)
        {
            Span<byte> buffer = new Span<byte>(new byte[data.Length]);

            bool retValue = Convert.TryFromBase64String(data, buffer, out int bytesParsed);

            return retValue;
        }
    }
}