using Newtonsoft.Json;
using OxbridgeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using System.Net;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using System.Net.Http.Headers;

namespace OxbridgeApp.Services
{
    public delegate void MessageReceivedHandler(object sender, string message);

    //public delegate void ConnectionHandler(object sender);

    public class WebConnection
    {
        public bool Connected { get; set; }

        public Socket socket { get; set; }

        //public event MessageReceivedHandler NewMessageReceived;
        //public event ConnectionHandler ConnectedEvent;

        public event MessageReceivedHandler NewCoordReceived;

        public WebConnection()
        {



        }



        /// <summary>
        /// used for sending/receiving coordinates
        /// </summary>
        public void ConnectSocket() {
            socket = IO.Socket(Constants.HostName);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Connected = true;
                socket.On("coord", (data) =>
                {
                    Console.WriteLine("Received from server: " + data);
                    NewCoordReceived?.Invoke(null, data.ToString());
                });
            });
        }

        //doesnt work
        public void DisconnectSocket() {
            //socket.Disconnect();
            //socket.Off();
            //socket.Close();
            //socket = null;

            //socket.On(Socket.EVENT_DISCONNECT, () =>
            //{
            //    Connected = false;
            //    socket.On("disconnect", (data) =>
            //    {
                    

            //    });
            //});
        }



        /// <summary>
        /// used for sending this users coordinates to all other clients
        /// </summary>
        /// <param name="coordinate"></param>
        public void SendCoordinate(Coordinate coordinate) {
            if(socket != null) {
                string jsonCoordinate = JsonConvert.SerializeObject(coordinate);
                socket.Emit("coord", jsonCoordinate);
            }
            
        }

        
        /// <summary>
        /// This method will retrieve a list of upcoming races. It doesn't require a token, as anonymous users can also
        /// see this kind of information. 
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Race>> GetRaces()
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) })
            {
                //httpClient.DefaultRequestHeaders.Authorization =
                //new AuthenticationHeaderValue("Bearer", Preferences.Get(GlobalKeys.TokenKey, "null"));


                try {
                    var response = await httpClient.GetStringAsync("races").ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<ObservableCollection<Race>>(response.ToString());
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public async Task<bool> Login(string username, string password) {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
                var user = new { username, password };
                string jsonUser = JsonConvert.SerializeObject(user);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "authentication/login/");
                request.Content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                try {
                    var response = await httpClient.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.Created) {
                        string body = await response.Content.ReadAsStringAsync();
                        JObject jObj = JObject.Parse(body);
                        if (jObj != null) {
                            // Get the values out of the JSON 
                            string tokenValue = (string)jObj.SelectToken("token");
                            string fullNameValue = (string)jObj.SelectToken("fullName");
                            string usernameValue = (string)jObj.SelectToken("username");
                            bool isAdminValue = (bool)jObj.SelectToken("isAdmin");
                            bool isTeamLeaderValue = (bool)jObj.SelectToken("isTeamLeader");
                            string teamValue = (string)jObj.SelectToken("team");
                            // We store these preferences for later use
                            CurrentUser.SetCurrentUser(tokenValue, fullNameValue, usernameValue, isAdminValue, isTeamLeaderValue, teamValue);
                            CurrentUser.IsLoggedIn = true;

                            return true;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }

                CurrentUser.IsLoggedIn = false;
                return false;
            }
        }

        public async Task<bool> JoinRace(int raceID) {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
                httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Preferences.Get(CurrentUser.TokenKey, "null"));

                var obj = new { raceID };
                string jsonObj = JsonConvert.SerializeObject(obj);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "races/joinRace/");
                request.Content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

                try {
                    var response = await httpClient.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK) {
                        string body = await response.Content.ReadAsStringAsync();
                        JObject jObj = JObject.Parse(body);
                        if (jObj != null) {
                            // Get the values out of the JSON 
                            bool access = (bool)jObj.SelectToken("access");
                            string message = (string)jObj.SelectToken("message");
                            Console.WriteLine(message);
                            return access; //true or false depending on assignment to race
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("*** Either not logged in or something terrible");
                return false; //no response
            }
        }


        //public async Task<bool> ValidateToken(string token) {
        //    using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
        //        try {
        //            // Authorization
        //            httpClient.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", Preferences.Get(GlobalKeys.TokenKey, "null"));

        //            // var response = await client.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));
        //            var response = await httpClient.GetAsync("/validateToken");

        //            string body = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(body);


        //            JObject jObj = JObject.Parse(body);
        //            Console.WriteLine("Was parsing the body to a JObject successful: " + jObj != null);
        //            if (jObj != null) {
        //                bool auth = (bool)jObj.SelectToken("auth");
        //                if (auth == true) {
        //                    return true;
        //                }
        //            }
        //        }
        //        catch (Newtonsoft.Json.JsonReaderException ex) {
        //            Console.WriteLine(ex.Message);
        //            return false;
        //        }
        //        return false;
        //    }
        //}







        //public static async Task<ObservableCollection<Race>> GetRace(int raceNo) {
        //    using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
        //        var response = await httpClient.GetStringAsync("races/" + raceNo).ConfigureAwait(false);
        //        return JsonConvert.DeserializeObject<ObservableCollection<Race>>(response.ToString());
        //    }
        //}


    }
}

