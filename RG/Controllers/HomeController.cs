using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RG.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RG.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;
        public HomeController(IMemoryCache cache)
        {
            this._cache = cache;
        }
        public IActionResult Home()
        {

            List<Person> ListOfPersons = new List<Person>();
            if (!_cache.TryGetValue("CacheTime", out List<Person> person))
            {
                string url = String.Format("https://hiring.rewardgateway.net/list"); //here I have the correct url for my API
                HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
                requestObj.Method = "Get";
                requestObj.PreAuthenticate = true;
                requestObj.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("hard:hard"));
                HttpWebResponse responseObj = null;
                responseObj = (HttpWebResponse)requestObj.GetResponse();
                string strresult = null;
                using (Stream stream = responseObj.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresult = sr.ReadToEnd();
                    person = JsonConvert.DeserializeObject<List<Person>>(strresult);
                    sr.Close();
                }

                _cache.Set("CacheTime", person, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(600)));
                return View(person);

            }

            ListOfPersons = _cache.Get<List<Person>>("CacheTime");
            return View(ListOfPersons);
        }
    }
}
