using System;

namespace Raven4.Indexing.Debug
{
    public class AnotherTestDocument
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string Description => $"Created: {Created}";
    }
}