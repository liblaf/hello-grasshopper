using System;

using Grasshopper.Kernel;

namespace HelloGrasshopper
{
    public class HelloGrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name => "HelloGrasshopper";

        public override string Description => "Hello, Grasshopper!";

        public override Guid Id => new Guid("29B84659-974B-4C9F-A97D-8973AAC2F380");

        public override string AuthorName => "liblaf";

        public override string AuthorContact => "rhino@liblaf.anonaddy.me";
    }
}
