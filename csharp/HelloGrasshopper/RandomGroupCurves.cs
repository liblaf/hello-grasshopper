using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;

namespace HelloGrasshopper
{
    public class RandomGroupCurves : GH_Component
    {
        public RandomGroupCurves()
            : base(
                name: "RandomGroupCurves",
                nickname: "RandCurve",
                description: "Generate random groups of curves",
                category: "liblaf",
                subCategory: "Random"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter(
                name: "NumGroup",
                nickname: "N",
                description: "Number of groups of curves",
                access: GH_ParamAccess.item,
                @default: 2
            );
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(
                name: "Curves",
                nickname: "C",
                description: "Random groups of curves",
                access: GH_ParamAccess.tree
            );
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int num_group = 0;
            DA.GetData(name: "NumGroup", destination: ref num_group);
            GH_Structure<GH_Curve> curves = new GH_Structure<GH_Curve>();
            for (int i = 0; i < num_group; ++i)
            {
                int num_curve = this.rand.Next(2, 4);
                curves.AppendRange(
                    data: this.RandomGHCurves(num_curve, max_z: 0.0),
                    path: new GH_Path(0, i)
                );
            }
            DA.SetDataTree(0, curves);
        }

        public override Guid ComponentGuid => new Guid("BD94B659-1C9B-4A53-BFFB-2E9CF5BED79B");

        protected double RandomDouble(double min = 0.0, double max = 1.0) =>
            this.rand.NextDouble() * (max - min) + min;

        protected Point3d RandomPoint(
            double min_x = 0.0,
            double min_y = 0.0,
            double min_z = 0.0,
            double max_x = 1.0,
            double max_y = 1.0,
            double max_z = 1.0
        )
        {
            double x = this.RandomDouble(min_x, max_x);
            double y = this.RandomDouble(min_y, max_y);
            double z = this.RandomDouble(min_z, max_z);
            return new Point3d(x, y, z);
        }

        protected Curve RandomCurve(
            int min_num_point = 3,
            int max_num_point = 8,
            double min_x = 0.0,
            double min_y = 0.0,
            double min_z = 0.0,
            double max_x = 1.0,
            double max_y = 1.0,
            double max_z = 1.0
        )
        {
            List<Point3d> points = new List<Point3d>();
            int num_points = this.rand.Next(min_num_point, max_num_point);
            for (int i = 0; i < num_points; ++i)
                points.Add(this.RandomPoint(min_x, min_y, min_z, max_x, max_y, max_z));
            return Curve.CreateInterpolatedCurve(points, degree: 3);
        }

        protected GH_Curve RandomGHCurve(
            int min_num_point = 3,
            int max_num_point = 8,
            double min_x = 0.0,
            double min_y = 0.0,
            double min_z = 0.0,
            double max_x = 1.0,
            double max_y = 1.0,
            double max_z = 1.0
        )
        {
            return new GH_Curve(
                this.RandomCurve(
                    min_num_point,
                    max_num_point,
                    min_x,
                    min_y,
                    min_z,
                    max_x,
                    max_y,
                    max_z
                )
            );
        }

        protected List<GH_Curve> RandomGHCurves(
            int num_curve = 2,
            int min_num_point = 3,
            int max_num_point = 8,
            double min_x = 0.0,
            double min_y = 0.0,
            double min_z = 0.0,
            double max_x = 1.0,
            double max_y = 1.0,
            double max_z = 1.0
        )
        {
            List<GH_Curve> curves = new List<GH_Curve>();
            for (int i = 0; i < num_curve; ++i)
                curves.Add(
                    this.RandomGHCurve(
                        min_num_point,
                        max_num_point,
                        min_x,
                        min_y,
                        min_z,
                        max_x,
                        max_y,
                        max_z
                    )
                );
            return curves;
        }

        private System.Random rand = new System.Random();
    }
}
