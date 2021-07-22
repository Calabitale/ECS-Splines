# ECS-Splines

Just my implementation of a spline system converted from Catlike Codings Tutorials and other areas of the internet into a DOTs compatible code.  

The basic outline for how it works is:

You place the script titled BezeirSpline onto an empty gameobject in the scene, with a ConverttoEntity script.
You can then adjust the spline in the editor just click on the points and add more parts it should be pretty straighforward. 
On running the program tha data then will be autoconverted to a BlobAsset.
You can then get that using BezierGraphSpawner with an EntityQuery.
You can use the structs(CalcdistalongBezier, CalcPositiononBezier etc) to access that data in fully bursted jobs.

I did most of the messing around in BezierECSsystem it should have some examples in there(ignore the commented out code though thats old).  
