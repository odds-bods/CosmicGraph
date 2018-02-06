namespace CosmicGraph.Tester
{
    public class PersonVertex : Vertex
    {
        public string FirstName { get { return Get(); } set { Set(value); } }
        public string LastName { get { return Get(); } set { Set(value); } }
    }
}
