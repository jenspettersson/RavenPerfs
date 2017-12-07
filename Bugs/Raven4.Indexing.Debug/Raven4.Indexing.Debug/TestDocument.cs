using System;

namespace Raven4.Indexing.Debug
{
    public class TestDocument
    {
        //Uses Key as Id via IdConvention
        public string Key { get; set; }
        public DateTime Created { get; set; }
        public string Description => $"Created: {Created}";
    }
}