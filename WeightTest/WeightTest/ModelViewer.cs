using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using WeightTest;

namespace BodyScanMulticamera
{
    
    public class ModelVisualizer
    {
        private Model3DGroup modelGroup;
        private ModelImporter mi;
        private ModelVisual3D model;
        private Dictionary<Range, DensityScale> densities;

        public ModelVisual3D Model
        {
            get
            {
                return model;
            }

            set
            {

            }
        }

        public float Weight { get; set; }

        public float DeltaWeight { get; set; }

        public ModelVisualizer(string ModelPath, int age)
        {
            InitDensities();

            #region Weight calculation
            modelGroup = new Model3DGroup();
            mi = new ModelImporter();
            mi.DefaultMaterial = MaterialHelper.CreateMaterial(Colors.White);
            modelGroup = mi.Load(ModelPath);

            GeometryModel3D personaGeometry = ((GeometryModel3D)modelGroup.Children[0]);
            MeshGeometry3D personaMesh = ((MeshGeometry3D)personaGeometry.Geometry);

            Point3D mCenter = CalculateModelCenter(personaMesh, KinectOrientation.Vertical);
        
            SphereVisual3D bolita = new SphereVisual3D();
            ((SphereVisual3D)bolita).Center = mCenter;
            ((SphereVisual3D)bolita).Radius = 0.05;
            
            CalculateWeight(personaMesh, mCenter, age);
            #endregion

            model = new ModelVisual3D();
            //model.Children.Add(bolita);
            Model.Content = modelGroup;
        }

        private void InitDensities()
        {
            // Human body density ( kilo/cubic meter )
            // This value was found in the following article  "HUMAN BODY DENSITY AND FAT OF AN ADULT MALE POPULATION AS MEASURED BY WATER DlSPLACEMENT" by
            // Harry J. Krzywicki, et al. 
            // This value varies between 1000 and 1600 depending on the measured (age, gender, etc.)  person
            densities = new Dictionary<Range, DensityScale>();
            densities.Add(new Range(1, 19), new DensityScale(1060, 16));
            densities.Add(new Range(20, 24), new DensityScale(1060, 13));
            densities.Add(new Range(25, 29), new DensityScale(1053, 17));
            densities.Add(new Range(30, 34), new DensityScale(1044, 13));
            densities.Add(new Range(35, 39), new DensityScale(1043, 12));
            densities.Add(new Range(40, 44), new DensityScale(1042, 12));
            densities.Add(new Range(45, 49), new DensityScale(1038, 10));
            densities.Add(new Range(50, 54), new DensityScale(1032, 26));
            densities.Add(new Range(55, 59), new DensityScale(1031, 21));
            densities.Add(new Range(60, 64), new DensityScale(1026, 10));
            densities.Add(new Range(65, 120), new DensityScale(1017, 1));
        }

        public void CalculateWeight(MeshGeometry3D _meshes, Point3D center, int age)
        {
            DensityScale density = GetDensity(age);
            float volume = CalculateVolume(_meshes, (float)center.X, (float)center.Y, (float)center.Z);

            Weight = density.DensityValue * volume;
            DeltaWeight = Weight - density.MinDensityValue() * volume;
        }

        /// <summary>
        /// Volume calculation found in the paper: "EFFICIENT FEATURE EXTRACTION FOR 2D/3D OBJECTS IN MESH REPRESENTATION" by
        /// Cha Zhang and Tsuhan Chen
        /// </summary>
        /// <param name="_meshes"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <returns></returns>
        float CalculateVolume(MeshGeometry3D _meshes, float x0, float y0, float z0)
        {
            float Volume = 0f;

            for (int i = 0; i < _meshes.Positions.Count - 3; i += 3)
            {
                float x1 = (float)_meshes.Positions[i + 0].X;
                float x2 = (float)_meshes.Positions[i + 1].X;
                float x3 = (float)_meshes.Positions[i + 2].X;

                float y1 = (float)_meshes.Positions[i + 0].Y;
                float y2 = (float)_meshes.Positions[i + 1].Y;
                float y3 = (float)_meshes.Positions[i + 2].Y;

                float z1 = (float)_meshes.Positions[i + 0].Z;
                float z2 = (float)_meshes.Positions[i + 1].Z;
                float z3 = (float)_meshes.Positions[i + 2].Z;

                Vector3D V1 = new Vector3D(x1 - x0, y1 - y0, z1 - z0);
                Vector3D V2 = new Vector3D(x2 - x0, y2 - y0, z2 - z0);
                Vector3D V3 = new Vector3D(x3 - x0, y3 - y0, z3 - z0);

                Vector3D crossResult = Vector3D.CrossProduct(V1, V2);
                float signedVolume = (float)Vector3D.DotProduct(crossResult, V3) / 6.0f;



                Volume += signedVolume;
            }


            return Math.Abs(Volume);
        }

        private DensityScale GetDensity(int age)
        {
            return densities[GetAgeRange(age)];
        }

        private Range GetAgeRange(int age)
        {
            foreach (var item in densities.Keys)
            {
                if (item.InsideRange(age))
                    return item;
            }

            return null;
        }

        enum KinectOrientation { Vertical, Horizontal}

        /// <summary>
        /// Finds the model´s center
        /// </summary>
        /// <param name="_meshes"></param>
        /// <returns></returns>
        Point3D CalculateModelCenter(MeshGeometry3D _meshes, KinectOrientation orientation)
        {
            Point3D center = new Point3D();
            
            float xmin = 1000;
            float xmax = -1000;
            float ymin  = 1000;
            float ymax = -1000;
            float zmin = 1000;
            float zmax = -1000;

            for (int i = 0; i < _meshes.Positions.Count - 3; i += 3)
            {
                float x1 = (float)_meshes.Positions[i + 0].X;
                float x2 = (float)_meshes.Positions[i + 1].X;
                float x3 = (float)_meshes.Positions[i + 2].X;

                float y1 = (float)_meshes.Positions[i + 0].Y;
                float y2 = (float)_meshes.Positions[i + 1].Y;
                float y3 = (float)_meshes.Positions[i + 2].Y;

                float z1 = (float)_meshes.Positions[i + 0].Z;
                float z2 = (float)_meshes.Positions[i + 1].Z;
                float z3 = (float)_meshes.Positions[i + 2].Z;

                if (x1 < xmin)
                    xmin = x1;
                if (x2 < xmin)
                    xmin = x1;
                if (x3 < xmin)
                    xmin = x1;

                if (x1 > xmax)
                    xmax = x1;
                if (x2 > xmax)
                    xmax = x2;
                if (x3 > xmax)
                    xmax = x3;

                if (y1 < ymin)
                    ymin = y1;
                if (y2 < ymin)
                    ymin = y1;
                if (y3 < ymin)
                    ymin = y1;

                if (y1 > ymax)
                    ymax = y1;
                if (y2 > ymax)
                    ymax = y2;
                if (y3 > ymax)
                    ymax = y3;

                if (z1 < zmin)
                    zmin = z1;
                if (z2 < zmin)
                    zmin = z1;
                if (z3 < zmin)
                    zmin = z1;

                if (z1 > zmax)
                    zmax = z1;
                if (z2 > zmax)
                    zmax = z2;
                if (z3 > zmax)
                    zmax = z3;


                
            }


            center.X = xmin +(xmax - xmin) / 2;
            center.Y = ymin + (ymax - ymin) / 2;
            center.Z = zmin + (zmax - zmin) / 2;

            if (orientation == KinectOrientation.Horizontal)
                center.Y = ymax;
            else
                center.X = xmax;

            return center;
        }

       
    }
}