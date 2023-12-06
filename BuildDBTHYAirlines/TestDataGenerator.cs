using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDBTHYAirlines
{
    public static class TestDataGenerator
    {

        public static string GetRandomLoyaltyID()
        {
            List<string> loyaltyIDList = new List<string>() { "MP59011E", "MP11111F", "MP09211A", "MP39218S", "MP91133K", "MP21057Z" };
            Random random = new Random();

            return loyaltyIDList[random.Next(loyaltyIDList.Count)];
        }

        public static string GetRandomCustomerID()
        {
            List<string> customerIDList = new List<string>() { "V09222212", "E11022315", "B793215D", "Q12427291", "KK2192004", "JW9674817" };
            Random random = new Random();

            return customerIDList[random.Next(customerIDList.Count)];
        }

        public static string GetRandomDepartDate()
        {
            List<string> departDateList = new List<string>() { "01/10/2024", "12/12/2023", "12/23/2023", "03/01/2024", "02/21/2023", "11/20/2023" };
            Random random = new Random();

            return departDateList[random.Next(departDateList.Count)];
        }

        public static string GetRandomIataCode(string codeToExclude = "")
        {
            List<string> airportCodeList = new List<string>() { "IAH", "EWR", "LAX", "ORD", "CDG", "IST", "AYT", "ADA", "SAW" }.Where(x => !x.Equals(codeToExclude)).ToList();
            Random random = new Random();

            return airportCodeList[random.Next(airportCodeList.Count)];
        }

        public static string GetRandomDeDupeKey(string origin,string destination,string departDate)
        {
          
            string deDupeKey = $"{origin}{destination}{departDate}";


            return deDupeKey;
        }


        public static string GetRandomClientIP()
        {
            List<string> IPAdddressList = new List<string>() { "121.01.2.233", "175.34.11.221", "192.12.25.222", "192.123.121.225", "190.235.121.255", "192.190.12.122", "64.65.23.213" };
            Random random = new Random();

            return IPAdddressList[random.Next(IPAdddressList.Count)];

        }


        public static string GetRandomDeviceID()
        {
            List<string> deviceIDsList = new List<string>() { "a702870a-8f47-4d9d-a0c6-e84e841d7f97", "5fae3bfa-4104-4e44-9166-c67aa31df64f", "df66f9a0-e2d3-4743-ba54-b69095130366", "b2313403-6567-416c-a117-89f374aadd0c", "4b374a4e-d686-4cb3-a16c-7177a0555c9a", "9a8352a9-27ab-449c-a713-60959b97b222", "40d2ab86-66f6-4f24-ba83-a04d059e1521" };
            Random random = new Random();

            return deviceIDsList[random.Next(deviceIDsList.Count)];

        }


        public static string GetLogData()
        {
            return "{\"_id\":\"6552547c1c05cb0217d87074\",\"index\":0,\"guid\":\"bea77b3a-0f65-475f-97e0-b518e778e63e\",\"isActive\":false,\"balance\":\"$2,319.73\",\"picture\":\"http:\\\\placehold.it\\\\32x32\",\"age\":22,\"eyeColor\":\"green\",\"name\":\"Alice Leonard\",\"gender\":\"female\",\"company\":\"RECRISYS\",\"email\":\"aliceleonard@recrisys.com\",\"phone\":\"+1 (998) 460-2200\",\"address\":\"898 Luquer Street, Grenelefe, Connecticut, 6738\",\"about\":\"Cillum duis dolor anim occaecat dolore. Reprehenderit irure dolor ut velit aute exercitation velit dolor enim. Pariatur incididunt ullamco minim minim aliqua nisi magna sunt.\r\n\",\"registered\":\"2014-04-02T06:28:34 +05:00\",\"latitude\":67.97325,\"longitude\":99.190584,\"tags\":[\"culpa\",\"dolor\",\"culpa\",\"velit\",\"ipsum\",\"anim\",\"cillum\"],\"friends\":[{\"id\":0,\"name\":\"Sullivan Gaines\"},{\"id\":1,\"name\":\"Rebekah Freeman\"},{\"id\":2,\"name\":\"Forbes Meyer\"}],\"greeting\":\"Hello, Alice Leonard! You have 2 unread messages.\",\"favoriteFruit\":\"apple\"}";
        }
    }
}
