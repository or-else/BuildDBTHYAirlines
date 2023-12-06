using EBBuildClient.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ConfigurationRegistry;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Data;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using static ServiceStack.LicenseUtils;
using System.Runtime.CompilerServices;
using THYAirlines.Provider;
using static System.Runtime.InteropServices.JavaScript.JSType;
using THYAirlines.Models;
using BuildDBTHYAirlines;
using Newtonsoft.Json.Linq;
using ServiceStack.Web;

public class Program
{    
    /*/
     * Prepared by : EverythingBlockchain, Inc  (C) All Rights Reserved 2023
     * Prepared for: Turkish Airlines Inc. (Turk Hava Yollari A.O.) THY via Olscan, Inc
     * License     : General Public License (GNU )
     * 
     * Purpose:     This test application is created to demonstrate the BuildDB document database with the following capabilities:
     *               1.	RBAC security
     *               2.	Advanced data filtering
     *               3.	Advanced data filtering with aggregate functions
     *               4.	Advanced data filtering with AI search
     *               5.	DDoS functions to combat throttling and denial of service attacks
     *               6.	Parent child relational data retrieval via lazy loading
     *               
     * Scope:       This test application simulates recent search functionality of guest and loyalty passengers for the airline industry.
     * Approach:    This test application uses manual synchronization to demonstrate two distinct test runs.
     * Test Run #1: utilizes a default role for all transactions.
     * Test Run #2: utilizes a foreign role to trigger RBAC protections.
    /*/
    public static void Main(string[] args)
    {

        Task task = Task.Run(async () =>
        {
            await Begin();
        });

        task.Wait();

    }





    public static async Task Begin()
    {

        bool results_01 = true;
        List<Task> tasks = new List<Task>();

       

        ITHYAirlinesProvider providerWithNoRBAC = new THYAirlinesProvider();


        /*/
         * Get ledger statistics
       / */
        List<LedgerListResponseDTO> _ledgerStats = providerWithNoRBAC.GetLedgerStats();




        /*/
         * 1.   BuildDB demos high data throughput with inserts of more than one type
        /*/
        for (Int32 _cnt = 0; _cnt < 2; _cnt++)
        {
            tasks.Add(InsertTestDataDeviceID(providerWithNoRBAC));
            tasks.Add(InsertTestDataLoyaltyID(providerWithNoRBAC));
        }       

        Task.WaitAll(tasks.ToArray());


        tasks.Clear();

        /*/
         * 2.	BuildDB demos hierarchical relationships (joins) without the aid of costly table indexes.
        /*/
        List<Recent_Searches_Flight_Shop> results_02 = providerWithNoRBAC.GetRecentSearchesFlightShopByDeviceOrLoyaltyIdAsync(deviceId: string.Empty, loyaltyId: "MP39218S").Result;


        /*/
         * 3.	BuildDB demos advanced aggregate functions and data filtering with AI search and semantic matching
        /*/
        Int32 results_03 =  providerWithNoRBAC.GetRecentSearchesCountByOriginDestDepartDateAsync(origin: string.Empty, destination: string.Empty, departDate: string.Empty).Result;
        List<Recent_Searches_Flight_Shop> results_03_fuzzyMatching = providerWithNoRBAC.GetRecentSearchesFlightShopByFuzzyMatchingAsync(iataCode: string.Empty).Result;
        List<Recent_Searches_Flight_Shop> results_03_hotMarkets = providerWithNoRBAC.GetHotMarketsByLoyaltyIdAsync(loyaltyId: string.Empty, origin: string.Empty).Result;


        /*/
         * 4.	BuildDB demos DDoS functions internally to combat throttling and potential denial of service attacks
        /*/
        List<Recent_Searches_Flight_Shop> results_04 = providerWithNoRBAC.GetRecentSearchesDDoSByOriginDestDepartDateAsync(deviceIDValue: string.Empty).Result;



     




        /*/
         * Test #2 with different role(s).
        /*/

        
        /*/
         * 5.	BuildDB demos enforcement of RBAC security 
         * Test with Guest role to trigger RBAC protections of the default role(s) from test run #1.
        /*/

        ITHYAirlinesProvider providerWithRBAC = new THYAirlinesProvider(new List<string>() { "Guest" });
        List<Recent_Searches_Flight_Shop> results_05 = providerWithRBAC.GetRecentSearchesFlightShopByDeviceOrLoyaltyIdAsync(deviceId: string.Empty, loyaltyId: TestDataGenerator.GetRandomLoyaltyID()).Result;


        
    }


    public static async Task<bool> InsertTestDataDeviceID(ITHYAirlinesProvider provider)
    {
       
        string customerId = TestDataGenerator.GetRandomCustomerID();
        string loyaltyId = string.Empty;        
        string transactionId = Guid.NewGuid().ToString();
        DateTime? searchTimeStamp = DateTime.Now;
        DateTime? loginStamp = DateTime.Now;
        string useToken = Guid.NewGuid().ToString();
        string sessionToken = Guid.NewGuid().ToString();
        string logDataValue = TestDataGenerator.GetLogData();
        string clientip = TestDataGenerator.GetRandomClientIP();
        string deviceid = TestDataGenerator.GetRandomDeviceID();
        string origin = TestDataGenerator.GetRandomIataCode();
        string destination = TestDataGenerator.GetRandomIataCode(origin);
        string departDate = TestDataGenerator.GetRandomDepartDate();
        string deDupKey = TestDataGenerator.GetRandomDeDupeKey(origin, destination, departDate);

        Debug.WriteLine($"Device ID: {deviceid}");
        Debug.WriteLine($"DeDup Key: {deDupKey}");                  



        await provider.InsertRecentSearchesFlightShopAsync(
            customerId,
            loyaltyId,
            deDupKey,
            transactionId,
            searchTimeStamp,
            loginStamp,
            useToken,
            sessionToken,
            logDataValue,
            clientip,
            deviceid,
            origin,
            destination, 
            departDate);




        return true;
    }



    

    public static async Task<bool> InsertTestDataLoyaltyID(ITHYAirlinesProvider provider)
    {
    
        string customerId = TestDataGenerator.GetRandomCustomerID();
        string loyaltyId = TestDataGenerator.GetRandomLoyaltyID();        
        string transactionId = Guid.NewGuid().ToString();
        DateTime? searchTimeStamp = DateTime.Now;
        DateTime? loginStamp = DateTime.Now;
        string useToken = Guid.NewGuid().ToString();
        string sessionToken = Guid.NewGuid().ToString();
        string logDataValue = TestDataGenerator.GetLogData();
        string clientip = TestDataGenerator.GetRandomClientIP();
        string deviceid = string.Empty;
        string origin = TestDataGenerator.GetRandomIataCode();
        string destination = TestDataGenerator.GetRandomIataCode(origin);
        string departDate = TestDataGenerator.GetRandomDepartDate();
        string deDupKey = TestDataGenerator.GetRandomDeDupeKey(origin, destination, departDate);

        Debug.WriteLine($"Loyalty ID: {loyaltyId}");
        Debug.WriteLine($"DeDup Key: {deDupKey}");

        await provider.InsertRecentSearchesFlightShopAsync(
            customerId,
            loyaltyId,
            deDupKey,
            transactionId,
            searchTimeStamp,
            loginStamp,
            useToken,
            sessionToken,
            logDataValue,
            clientip,
            deviceid,
            origin,
            destination,
            departDate);

       

        return true;
    }


}
