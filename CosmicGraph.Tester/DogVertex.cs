namespace CosmicGraph.Tester
{
    public class DogVertex : Vertex
    {
        public string Name { get { return Get(); } set { Set(value); } }
        public string Age { get { return Get(); } set { Set(value); } }
    }
}
