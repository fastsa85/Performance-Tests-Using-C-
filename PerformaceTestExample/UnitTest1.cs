using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PerformaceTestExample
{
    public class Tests
    {        
        //const string URL = "https://localhost:5001/api/v1/posts";
        const string URL = "https://www.google.com.ua";
        const int NUMBER_OF_REQUESTS = 300;
        const int NUMBER_OF_USERS = 3;

        const int MAX_TIMEOUT = 10; // minutes

        [Test]
        public void TestPerformance()
        {            
            var times = new List<long>();
            var users = new List<Task>();

            for (int i = 0; i < NUMBER_OF_USERS; i++)
            {
                var webClient = new HttpClient();
                webClient.Timeout = TimeSpan.FromMinutes(MAX_TIMEOUT);
                long time = 0;
                users.Add(new Task(() => 
                { 
                    ExecuteParallelRequest(webClient, URL, NUMBER_OF_REQUESTS, out time);
                    times.Add(time);
                }));                
            }

            foreach (var user in users)
            {
                user.Start();
            }

            Task.WaitAll(users.ToArray());
            
            var userNumber = 0;
            foreach (var time in times)
            {
                Console.WriteLine($"User_{userNumber} Reponse Time is: {time}");
                userNumber++;
            }

            var avrage = times.Sum()/times.Count;
            Console.WriteLine($"Average Response Time is: {avrage}");
        }

        private void ExecuteParallelRequest(HttpClient webClient, string url, int numberOfRequests, out long elapsedTime)
        {            
            var userRequests = new List<Task>();

            Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            for (int i = 0; i < numberOfRequests; i++)
            {
                userRequests.Add(webClient.GetAsync(new Uri(url)));               
            }
            Task.WaitAll(userRequests.ToArray());

            timer.Stop();

            elapsedTime = timer.ElapsedMilliseconds;
            webClient.Dispose();
        }
    }
}