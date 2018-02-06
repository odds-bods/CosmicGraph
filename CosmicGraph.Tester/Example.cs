using System;
using System.Threading.Tasks;

namespace CosmicGraph.Tester
{
    public class Example
    {
        public async Task ExecuteAsync(ICosmicGraphClient cosmic)
        {
            await cosmic.ClearEntireGraphAsync();

            // fred...
            var fred = await cosmic.AddVertexIfNotExistsAsync(
                new PersonVertex
                {
                    // explicit known id.
                    Id = Guid.Empty.ToString(),
                    Label = "Fred Smith",
                    FirstName = "Fred",
                    LastName = "Smith"
                }
            );

            // has a dog.
            var dog = await cosmic.AddChildAsync("owner", fred,
                new DogVertex
                {
                    Label = "Jarvis",
                    Name = "Jarvis",
                    Age = "9"
                }
            );

            // and a tail.
            var tail = await cosmic.AddChildAsync("tail", dog,
                new TailVertex
                {
                    Name = "Primary Tail"
                }
            );

            // find the tail.
            var tailFromEdgePath = await cosmic.GetVertexAtEdgePathAsync<TailVertex>(
                fred,
                new string[] { "owner", "tail" }
            );

            // second gen.
            await AddDescendant(fred, "Bob", cosmic);
            await AddDescendant(fred, "Lynda", cosmic);
            var sam = await AddDescendant(fred, "Sam", cosmic);

            // third gen.
            var charlie = await AddDescendant(sam, "Charlie", cosmic);
            await AddDescendant(sam, "Timmy", cosmic);

            // fourth gen.
            var dash = await AddDescendant(charlie, "Dash", cosmic);

            // find dash.
            var personFromPropertyPath = await cosmic.GetVertexAtEdgePathAsync<PersonVertex>(
                fred,
                new string[] { "descendant" },
                nameof(fred.FirstName),
                new string[] { "Sam", "Charlie", "Dash" }
            );
        }

        private async Task<PersonVertex> AddDescendant(IVertex parent, string firstName, ICosmicGraphClient cosmic)
        {
            return await cosmic.AddChildAsync("descendant", parent,
                new PersonVertex
                {
                    FirstName = firstName,
                    LastName = "Smith",
                    Label = $"{firstName} Smith"
                }
            );
        }
    }
}
