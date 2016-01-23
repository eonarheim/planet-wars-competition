using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpointIndex = args.ToList().Select(a => a.ToLower()).ToList().IndexOf("-endpoint");
            var endpoint = args[endpointIndex + 1];
            try
            {
                var agent = new Agent("DemoAgent", endpoint);
                agent.Start().Wait();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Ooop! Something went wrong starting your agent {e.Message}");
            }

        }
    }
}
