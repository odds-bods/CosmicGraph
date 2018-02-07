namespace CosmicGraph.Tester
{
    public class DogVertex : Vertex
    {
        public string Name { get { return Get(); } set { Set(value); } }
        public string Escapable { get { return Get(); } set { Set(value); } }
        public string SpecialChars { get { return Get(); } set { Set(value); } }
    }
}
