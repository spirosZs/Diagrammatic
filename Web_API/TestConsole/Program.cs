using Exercises;
using Exercises.Client;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            var diagramService = new DiagramService(client.BaseAddress, client);
            var diagramTestService = new DiagramTestService();

            var exercisesClient = new ExercisesClient(diagramService, diagramTestService);

            RunAsync(exercisesClient).Wait();

            Console.WriteLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }

        private static async Task RunAsync(IExercisesClient client)
        {
            var diagramsFilter = new DiagramsFilter();
            var diagrams = await client.Diagrams.GetAsync(diagramsFilter);
            Console.WriteLine(JsonConvert.SerializeObject(diagrams));

            var diagram = await client.Diagrams.GetAsync(diagrams.First().Id);
            Console.WriteLine(JsonConvert.SerializeObject(diagram));

            var createPayload = new CreateDiagramPayload
            {
                Name = "test from console",
                Definition = "just a json definition from syncfusion"
            };
            diagram = await client.Diagrams.CreateAsync(createPayload);
            Console.WriteLine($"CREATED: {JsonConvert.SerializeObject(diagram)}");

            diagram.Category = "just a category";
            diagram = await client.Diagrams.UpdateAsync(diagram);
            Console.WriteLine($"UPDATED: {JsonConvert.SerializeObject(diagram)}");

            diagram = await client.Diagrams.DeleteAsync(diagram.Id);
            Console.WriteLine($"DELETED: {JsonConvert.SerializeObject(diagram)}");

            try
            {
                diagram = await client.Diagrams.GetAsync(diagram.Id);
            }
            catch (Exception e)
            {
                Console.Write($"GETTING {diagram.Id}: {e.Message}");
            }
        }
    }
}
