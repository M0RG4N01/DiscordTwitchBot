using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class WebCalls
    {
        public class Datum
        {
            public string broadcaster_id { get; set; }
            public string broadcaster_name { get; set; }
            public bool is_gift { get; set; }
            public string plan_name { get; set; }
            public string tier { get; set; }
            public string user_id { get; set; }
            public string user_name { get; set; }
        }

        public class Pagination
        {
            public string cursor { get; set; }
        }

        public class RootObject
        {
            public List<Datum> data { get; set; }
            public Pagination pagination { get; set; }
        }

        public static RootObject GetRootObject()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/helix/subscriptions?broadcaster_id=109598326");
            request.Headers.Add("Authorization", "Bearer **Removed**");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string myResponse = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                myResponse = sr.ReadToEnd();
            }

            var subs = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(myResponse);
            return subs;

        }


    }
}
