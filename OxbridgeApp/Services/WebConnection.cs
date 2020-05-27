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


        //public void ConnectAndTest()
        //{
        //    socket = IO.Socket(Constants.HostName);
        //    socket.On(Socket.EVENT_CONNECT, () =>
        //    {
        //        Connected = true;
        //        ConnectedEvent?.Invoke(null);
        //        //    socket.Emit("hi");
        //        socket.On("hi", (data) =>
        //        {
        //            Console.WriteLine("Received from server: " + data);
        //            NewMessageReceived?.Invoke(null, data.ToString());
        //        });
        //    });
        //}
        //public void SendMessage(string message) {
        //    socket.Emit("hi", message);

        //}

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

        public void DisconnectSocket() {
            socket.Disconnect();
            socket.Off();
            socket.Close();
            socket = null;

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

        

        public async Task<ObservableCollection<Race>> GetRaces()
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) })
            {
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
                            string token = (string)jObj.SelectToken("token");
                            // We store these preferences for later use
                            Preferences.Set(GlobalKeys.TokenKey, token);
                            return true;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                return false;
            }
        }

        public async Task<bool> ValidateToken(string token) {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) }) {
                try {
                    // Authorization
                    httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Preferences.Get(GlobalKeys.TokenKey, "null"));

                    // var response = await client.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));
                    var response = await httpClient.GetAsync("/validateToken");

                    string body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(body);


                    JObject jObj = JObject.Parse(body);
                    Console.WriteLine("Was parsing the body to a JObject successful: " + jObj != null);
                    if (jObj != null) {
                        bool auth = (bool)jObj.SelectToken("auth");
                        if (auth == true) {
                            return true;
                        }
                    }
                }
                catch (Newtonsoft.Json.JsonReaderException ex) {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                return false;
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

