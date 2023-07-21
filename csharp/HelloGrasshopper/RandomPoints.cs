using System;

using Grasshopper.Kernel;

using Rhino.Collections;
using Rhino.Geometry;

namespace HelloGrasshopper
{
    public class RandomPoints : GH_Component
    {
        public RandomPoints()
            : base(
                name: "RandomPoints",
                nickname: "RandPts",
                description: "Generate random points",
                category: "liblaf",
                subCategory: "Random"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter(
                name: "NumPoints",
                nickname: "N",
                description: "Number of points",
                access: GH_ParamAccess.item,
                @default: 100
            );
            foreach (int i in new int[] { 0, 1 })
            {
                foreach (string axis in new string[] { "X", "Y", "Z" })
                {
                    pManager.AddAngleParameter(
                        name: i == 0 ? $"Min{axis}" : $"Max{axis}",
                        nickname: $"{axis}{i}",
                        description: "",
                        access: GH_ParamAccess.item,
                        @default: Convert.ToDouble(i)
                    );
                }
            }
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter(
                name: "Points",
                nickname: "Pts",
                description: "Random points",
                access: GH_ParamAccess.list
            );
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int num_points = 0;
            double min_x = 0.0,
                min_y = 0.0,
                min_z = 0.0,
                max_x = 0.0,
                max_y = 0.0,
                max_z = 0.0;
            DA.GetData("NumPoints", ref num_points);
            DA.GetData("MinX", ref min_x);
            DA.GetData("MinY", ref min_y);
            DA.GetData("MinZ", ref min_z);
            DA.GetData("MaxX", ref max_x);
            DA.GetData("MaxY", ref max_y);
            DA.GetData("MaxZ", ref max_z);
            Point3dList points = new Point3dList();
            for (int i = 0; i < num_points; ++i)
            {
                points.Add(this.RandomPoint(min_x, min_y, min_z, max_x, max_y, max_z));
            }
            DA.SetDataList("Points", points);
        }

        public override Guid ComponentGuid => new Guid("166B24C0-A166-4E0D-BA85-692DD63765E5");

        protected double RandomDouble(double min, double max) =>
            this.rand.NextDouble() * (max - min) + min;

        protected Point3d RandomPoint(
            double min_x,
            double min_y,
            double min_z,
            double max_x,
            double max_y,
            double max_z
        )
        {
            double x = this.RandomDouble(min_x, max_x);
            double y = this.RandomDouble(min_y, max_y);
            double z = this.RandomDouble(min_z, max_z);
            return new Point3d(x, y, z);
        }

        private System.Random rand = new System.Random();
    }
}
