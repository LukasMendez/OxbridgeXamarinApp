using Newtonsoft.Json;
using OxbridgeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

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



        /// <summary>
        /// used for sending this users coordinates to all other clients
        /// </summary>
        /// <param name="coordinate"></param>
        public void SendCoordinate(Coordinate coordinate) {
            string jsonCoordinate = JsonConvert.SerializeObject(coordinate);
            socket.Emit("coord", jsonCoordinate);
        }

        

        public static async Task<ObservableCollection<Race>> GetRaces()
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Constants.HostName) })
            {
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

