using Newtonsoft.Json;
using OxbridgeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OxbridgeApp.Services
{
    public class WebConnection
    {
        public WebConnection() {
        }

        public static async Task<ObservableCollection<Race>> GetRaces() {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
                var response = await httpClient.GetStringAsync("races").ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ObservableCollection<Race>>(response.ToString());
            }
        }

        //public static async Task<ObservableCollection<Race>> GetRace(int raceNo) {
        //    using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
        //        var response = await httpClient.GetStringAsync("races/" + raceNo).ConfigureAwait(false);
        //        return JsonConvert.DeserializeObject<ObservableCollection<Race>>(response.ToString());
        //    }
        //}


    }
}

