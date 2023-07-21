using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry.SpatialTrees;

using Rhino.Geometry;

namespace HelloGrasshopper
{
    public class RemoveDuplicatePoints : GH_Component
    {
        public RemoveDuplicatePoints()
            : base(
                name: "RemoveDuplicatePoints",
                nickname: "DupPt",
                description: "Remove similar points from a list",
                category: "liblaf",
                subCategory: "Point"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter(
                name: "Points",
                nickname: "P",
                description: "list of points to clean",
                access: GH_ParamAccess.list
            );
            pManager.AddNumberParameter(
                name: "Tolerance",
                nickname: "t",
                description: "If any points are less than this distance, they will be combined",
                access: GH_ParamAccess.item,
                @default: 0.01
            );
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter(
                name: "UniquePoints",
                nickname: "Q",
                description: "list of unique points",
                access: GH_ParamAccess.list
            );
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            double tolerance = 0.01;
            DA.GetDataList("Points", points);
            DA.GetData("Tolerance", ref tolerance);
            points.Sort((Point3d a, Point3d b) => a.CompareTo(b));
            HashSet<Point3d> point_set = new HashSet<Point3d>(points);
            Node3d<Point3d> tree = new Node3d<Point3d>(
                converter: (Point3d pt, out double x, out double y, out double z) =>
                {
                    x = y = z = 0.0;
                    TreeDelegates.Point3dCoordinates(pt, ref x, ref y, ref z);
                },
                region: new BoundingBox(points)
            );
            tree.AddRange(points);
            for (int i = 0; i < points.Count; i++)
            {
                if (!point_set.Contains(points[i]))
                    continue;
                while (true)
                {
                    List<Index3d<Point3d>> neighbors = tree.NearestItems(
                        locus: points[i],
                        groupSize: 2,
                        minimumDistance: 0.0,
                        maximumDistance: tolerance
                    );
                    if (neighbors.Count < 2)
                        break;
                    Index3d<Point3d> neighbor = neighbors[1];
                    if (points[i].DistanceTo(neighbor.Item) > tolerance)
                        break;
                    point_set.Remove(neighbor.Item);
                    tree.Remove(neighbor);
                }
            }
            DA.SetDataList("UniquePoints", point_set);
        }

        public override Guid ComponentGuid => new Guid("B0AA0E5D-0F03-4D5D-8E68-51D67791F03D");
    }
}
