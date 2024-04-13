using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Namespace
{
    public class FirebaseService
    {
        private readonly FirebaseClient _client;

        public FirebaseService()
        {
            string json = File.ReadAllText("./key.json");
            JObject jObject = JObject.Parse(json);
            string privateKey = jObject["private_key"].ToString();

            _client = new FirebaseClient("https://torantevoumou-default-rtdb.europe-west1.firebasedatabase.app", new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(privateKey)
            });
        }

        public async Task AddClient(Client client)
        {
            await _client
                .Child("users")
                .Child(client.Id.ToString())
                .PutAsync(client);
        }

        public async Task UpdateClient(Client client)
        {
            await _client
                .Child("users")
                .Child(client.Id.ToString())
                .PutAsync(client);
        }

        public async Task DeleteClient(string id)
        {
            await _client
                .Child("users")
                .Child(id)
                .DeleteAsync();
        }

        public async Task<List<Client>> GetClients()
        {
            using (var httpClient = new HttpClient())
            {
                var firebaseUrl = "https://torantevoumou-default-rtdb.europe-west1.firebasedatabase.app/users.json";
                var json = await httpClient.GetStringAsync(firebaseUrl);

                var clients = JsonConvert.DeserializeObject<List<Client>>(json);

                return clients;
            }
        }

        public async Task<bool> UsernameExists(string username)
        {
            var clients = await GetClients();
            return clients.Any(client => client.Username == username);
        }

        public async Task<bool> EmailExists(string email)
        {
            var clients = await GetClients();
            return clients.Any(client => client.Email == email);
        }

        public async Task<bool> PhoneNumberExists(string phoneNumber)
        {
            var clients = await GetClients();
            return clients.Any(client => client.PhoneNumber == phoneNumber);
        }

        public async Task AddUser(string firstName, string lastName, string username, string password, string email, string phoneNumber)
        {
            var client = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber
            };

            await AddClient(client);
        }


        public async Task<bool> UpdateClientAppointment(Appointment appointment)
        {
            var clients = await GetClients();
            Console.WriteLine("appid",appointment.Id.ToString());
            var client = clients.FirstOrDefault(client => client.Id == appointment.Id);
            Console.WriteLine("clientid",client.Id.ToString());
            if (client == null)
            {
                return false;
            }

            client.AppointmentDate += appointment.Date;
            client.AppointmentTime += appointment.Time;

            await UpdateClient(client);

            return true;
        }


    }
}