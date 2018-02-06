using System;
using System.Threading.Tasks;

namespace CosmicGraph.Tester
{
    public class Example
    {
        public async Task ExecuteAsync(ICosmicGraphClient cosmic)
        {
            await cosmic.ClearEntireGraphAsync();

            var person = await cosmic.AddVertexIfNotExistsAsync(
                new PersonVertex
                {
                    Id = Guid.Empty.ToString(),
                    Label = "Fred Smith",
                    FirstName = "Fred",
                    LastName = "Smith"
                }
            );

            var dog = await cosmic.AddChildAsync("owner", person,
                new DogVertex
                {
                    Label = "Jarvis",
                    Name = "Jarvis",
                    Age = "9"
                }
            );

            var tail = await cosmic.AddChildAsync("tail", dog,
                new TailVertex
                {
                    Name = "Primary Tail"
                }
            );

            //var tailFromPath = await client.GetVertexAtPathAsync<TailVertex>(person.Id, nameof(dog.Name), new string[] { "owner", "tail" });
        }
    }
}
