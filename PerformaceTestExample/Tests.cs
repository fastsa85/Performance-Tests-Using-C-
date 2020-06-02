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
        const string URL = "https://www.google.com.ua";
        //const string URL = "https://localhost:5001/api/v1/posts";
        const int NUMBER_OF_REQUESTS = 20;
        const int NUMBER_OF_USERS = 50;
        const int LIMIT = 3000; // milliseconds

        [Test]
        public void TestPerformance()
        {            
            var times = new List<long>();            

            for (int i = 0; i < NUMBER_OF_USERS; i++)
            {
                var webClient = new HttpClient();                
                long time = 0;
                var user = new Task(() => 
                { 
                    ExecuteParallelRequests(webClient, URL, NUMBER_OF_REQUESTS, out time);
                    times.Add(time);
                });

                user.Start();
                user.Wait();                
            }
            
            var userNumber = 0;
            foreach (var time in times)
            {
                Console.WriteLine($"User_{userNumber} Reponse Time is: {time}");
                userNumber++;
            }

            var avrage = times.Sum()/times.Count;
            Console.WriteLine($"Average Response Time is: {avrage}");

            var max = times.Max();
            Console.WriteLine($"Maximum Response Time is: {max}");

            var min = times.Min();
            Console.WriteLine($"Minimum Response Time is: {min}");

            var upperLimitCount = times.Where(x => x >= LIMIT).ToList().Count;
            Console.WriteLine($"Number of Responses Time more than {LIMIT} milliseconds is: {upperLimitCount}");
        }

        private void ExecuteParallelRequests(HttpClient webClient, string url, int numberOfRequests, out long elapsedTime)
        {            
            var userRequests = new List<Task>();

            Stopwatch timer = new Stopwatch();
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