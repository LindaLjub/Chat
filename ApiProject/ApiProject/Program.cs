using System;
using System.Threading.Tasks;
using System.Net.Http; // api
using Newtonsoft.Json; // json

namespace ApiProject
{
    class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();
        static string uri = "https://sv443.net/jokeapi/category/Miscellaneous";

        // chuck norris joke https://api.chucknorris.io/jokes/random
        // random daily joke https://api.jokes.one/jod.json
        // random good jokes https://sv443.net/jokeapi/category/Miscellaneous

        static async Task Main()
        {
                // Call asynchronous network methods in a try/catch block to handle exceptions.
                try
                {
                    //HttpResponseMessage response = await client.GetAsync(uri);
                    //response.EnsureSuccessStatusCode();
                    //string responseBody = await response.Content.ReadAsStringAsync();

                    // Above three lines can be replaced with new helper method below
                    string responseBody = await client.GetStringAsync(uri);

                    // parse
                    dynamic stuff = JsonConvert.DeserializeObject(responseBody);

                    // String description = stuff.contents.jokes[0].description;
                    // String joke = stuff.contents.jokes[0].joke.text;
                    // string joke = stuff.value;
                    // Console.WriteLine(stuff);
                    // Console.WriteLine("***" + description + "***\n\n" + joke);

                    // To see if it is a 2 part joke
                    string type = stuff.type;

                    if(type == "twopart")
                    {
                        string jokeSetup = stuff.setup;
                        string jokeDelivery = stuff.delivery;
                        Console.WriteLine(jokeSetup + "\n" + jokeDelivery);
                    }
                    else
                    {
                        string joke = stuff.joke;
                        Console.WriteLine(joke);
                    }


                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
        }

       
    }
}
