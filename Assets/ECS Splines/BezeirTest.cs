using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BezeirTest : MonoBehaviour
{
    public Transform dude1;
    public Transform dude2;
    public Transform dude3;
    public Transform dude4;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < 100; i++)
        {
            //var tempposition = CalculateBezierPoint(i, dude1.position, dude2.position, dude3.position, dude4.position);
            
        }
    }
    
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u  = 1.0f - t;
        var tt  = t * t;
        var uu  = u * u;
        var uuu = uu * u;
        var ttt = tt * t;
 
        Vector3 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term
 
        return p;
    }
}
//Todo I need a dynamicbuffer or nativearry of some kind for t, the entities.foreach will iterate through the numbers of t then again no maybe not, maybe need an entity each with a component t
//I should probably just create an entity with a dynamic buffer then put the transform points into the buffer then get them in a job 
public struct TestMoveojbectTag : IComponentData { }

//This would be for the 
public struct BezierTransformComp : IComponentData
{
    public LocalToWorld LTWValue;
    //public Matrix4x4 Value;
}


//I have no clue why this systme doesnt work without this
//[AlwaysUpdateSystem]
public class BezeirtestJobComp : JobComponentSystem
{
    //EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    //public float placeonSpline;

    public float testvalue;

    public float timeaccumalator;

    protected override void OnCreate()
    {
        //m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
        //TODO Am disabling this temporarily here because it seems easier 
        Enabled = false;
        
    }


    protected override void OnStartRunning()
    {
        //Todo Use Math.select instead of an if statement in a job
        //        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();



        //        Entities.ForEach((ref DotPrefabinator prefabEntityComponent) =>
        //        {
        //            for (int i = 0; i < 20; i++)
        //            {
        //                var doots = ecb.Instantiate(prefabEntityComponent.Dootprefab);
        //                var tempvar = new float3(0, i, 0);
        //                ecb.SetComponent(doots, new Translation{Value = tempvar });
        //                
        //            }
        //
        //            ///ecb.AddComponent<PlayerTag>(currplayer);
        //
        //            //ecb.SetComponent(currplayer, new Translation { Value = new float3(0, 8, 0)});
        //            
        //            //ecb.SetComponent(currplayer, new PhysicsGravityFactor{Value = 4});
        //            //todo: This does not work currently seems to be a bug with Unity
        //            //float3 freezerots = new float3(0, 0, 0);
        //
        //            //ecb.SetComponent(currplayer, new PhysicsMass {InverseInertia = freezerots});
        //
        //        }).Run();

        //var boo = GetSingleton<DotPrefabinator>();

        //Entity timmy = EntityManager.Instantiate(boo.MovePrefab);
        timeaccumalator = 0f;
        //placeonSpline = 0;
    }
    
    
//todo:  I need to get this to work with splines as in more than 4 points 
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        if (!HasSingleton<BezierGraphSpawner>())
        {
            Debug.Log("There is no Bezeiergraphspawer");
            return default;
        }

        var dootprefab = GetSingleton<DotPrefabinator>();
            
        var baked = GetSingleton<BezierGraphSpawner>();
        //ref var booked = ref baked.BezzyGraphcomp.Value;

        var boo = GetSingleton<DotPrefabinator>();

        if (!HasSingleton<TestMoveojbectTag>())
        {
            Entity toto = EntityManager.Instantiate(boo.MovePrefab);
            EntityManager.AddComponent<TestMoveojbectTag>(toto);
            EntityManager.SetComponentData(toto, new Translation { Value = new float3(0, 0, 0) });
            EntityManager.SetName(toto, "TestMoveObject");
            //Debug.Log("Why the fuck isnt this working");
        }

        //var testoob = GetSingletonEntity<TestMoveojbectTag>();

        //var tumbly = GetComponentDataFromEntity<Translation>();

        timeaccumalator += Time.DeltaTime / 20f;
        
        var timedeelta = timeaccumalator;

        if(!HasSingleton<BezierTransformComp>())
        {
            Debug.Log("Theres is no BezeirTransformComp");
            return default;
        }

        //var BezzyTransform = GetSingleton<BezierTransformComp>();

        //Matrix4x4 bezzymatrix = new Matrix4x4();
        //bezzymatrix = BezzyTransform.LTWValue.Value;
        //bezzymatrix = BezzyTransform.Value;
        //var placeonSpline = 0f;// = this.placeonSpline;
        //Debug.Log(timedeelta);




        //testvalue += timedeelta / 3;
        //Debug.Log("Testvalue "  + testvalue);

        //Moves the object along the spline
        Entities.WithAll<TestMoveojbectTag>().ForEach((ref Translation moventy) =>
        {

            ref var booked = ref baked.BezzyGraphcomp.Value;

            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

            //Debug.Log(Curvecount);

            //math.@select();
            //for (int v = 0; v < Curvecount; v++)
            {
                //select(Double, Double, Boolean)
                //Returns b if c is true, a otherwise.


                // select(double a, double b, bool c)
                //These need to be increment by + 3 on each loop I think
                //math.any();
                //var tempbool = v == 0;

                //math.any(tempbool);
                float3 Bezpoint1 = new float3();
                float3 Bezpoint2 = new float3();
                float3 Bezpoint3 = new float3();
                float3 Bezpoint4 = new float3();

                //if (v == 0)
                //{//Each Bezier point on each curve is plus 3 to the last curve                     
                //Bezpoint1 = booked.Bezzypoint[0];
                //Bezpoint2 = booked.Bezzypoint[1];
                //Bezpoint3 = booked.Bezzypoint[2];
                //Bezpoint4 = booked.Bezzypoint[3];
                //}
                //else
                //{
                //    Bezpoint1 = booked.Bezzypoint[0 + 3 * v];
                //    Bezpoint2 = booked.Bezzypoint[1 + 3 * v];
                //    Bezpoint3 = booked.Bezzypoint[2 + 3 * v];
                //    Bezpoint4 = booked.Bezzypoint[3 + 3 * v];
                //}

                //TODO: This fucking works but it gives hairwire jumpy crazy results for some reason

                //return transform.TransformPoint(Bezier.GetPoint(
                //points[i], points[i + 1], points[i + 2], points[i + 3], t));

                //math.clamp


                //for (int i = 0; i < 100; i++)
                {
                    //Trying to test a move system but cant seem to fooking move thing brain is too tired currently

                    //progress += Time.deltaTime / duration;
                    //if (progress > 1f)
                    //{
                    //    progress = 1f;
                    //}
                    //transform.localPosition = spline.GetPoint(progress);



                    //I just need to fix what they have done because their code isnt quite working 
                    //todo Next I need to make this so I can move objects through it along it at speeds
                    //float t = i / 100f;
                    var placeonSpline = timedeelta;

                    //progress += Time.deltaTime / duration;

                    //placeonSpline = 0.1f;

                    //Debug.Log(placeonSpline);
                    //This part gets the curve basically converts the 0 to 1 value so its distributed through the whole spline
                    int i;
                    if (placeonSpline >= 1f)
                    {
                        placeonSpline = 1f;
                        i = booked.Bezzypoint.Length - 4;
                    }
                    else
                    {
                        //placeonSpline = Mathf.Clamp01(placeonSpline);// * Curvecount;
                        placeonSpline = math.saturate(placeonSpline) * Curvecount;
                        //Debug.Log("The placeonspline is" + placeonSpline);

                        i = (int)placeonSpline;
                        placeonSpline -= i;
                        i *= 3;
                    }

                    Bezpoint1 = booked.Bezzypoint[i];
                    Bezpoint2 = booked.Bezzypoint[i + 1];
                    Bezpoint3 = booked.Bezzypoint[i + 2];
                    Bezpoint4 = booked.Bezzypoint[i + 3];

                    //public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
                    //{
                    //    t = Mathf.Clamp01(t);
                    //    float oneMinusT = 1f - t;
                    //    return
                    //        oneMinusT * oneMinusT * oneMinusT * p0 +
                    //        3f * oneMinusT * oneMinusT * t * p1 +
                    //        3f * oneMinusT * t * t * p2 +
                    //        t * t * t * p3;
                    //}

                    placeonSpline = math.saturate(placeonSpline);
                    float oneminust = 1f - placeonSpline;

                    //Todo this is exactly from the Curve example
                    //var returnVector = oneminust * oneminust * oneminust * Bezpoint1 + 3f * oneminust * oneminust * placeonSpline * Bezpoint2 + 3f * oneminust * placeonSpline * placeonSpline + Bezpoint3 + placeonSpline * placeonSpline * placeonSpline * Bezpoint4;

                    //Debug.Log(returnVector);

                    //Matrix4x4 matrix4worldtest = new Matrix4x4 { };
                    //matrix4worldtest.SetTRS = BezzyTransform.Value
                    //BezzyTransform.Value.

                    //var tempval = math.transform(BezzyTransform.Value, moventy.Value);
                    //Vector3 worldPositon = matrix4x4.MultiplyPoint3x4(localPosition);



                    //var worldpos = bezzymatrix.LTWValue.MultiplyPoint3x4(returnVector - moventy.Value);
                    //Todo This is from the forum
                    //var worldpos = bezzymatrix.MultiplyPoint3x4(returnVector);
                    //Debug.Log(worldpos);

                    //var worldpos = math.transform(bezzymatrix, moventy.Value);
                    //TODO Why is this off the exact position?  It seems like the other version is as well apart from the other problems.  
                    float u = 1.0f - placeonSpline;
                    var tt = placeonSpline * placeonSpline;
                    var uu = u * u;
                    var uuu = uu * u;
                    var ttt = tt * placeonSpline;
                    float3 p = uuu * Bezpoint1; //first term
                    p += 3 * uu * placeonSpline * Bezpoint2; //second term
                    p += 3 * u * tt * Bezpoint3; //third term
                    p += ttt * Bezpoint4; //fourth term

                    //var munchy = ecb.Instantiate(dootprefab.Dootprefab);

                    //ecb.SetComponent(munchy, new Translation { Value =  p});

                    moventy.Value = p;

                }

            }
        }).Run();


        //Rotates the entity to look along the spline
        Entities.WithAll<TestMoveojbectTag>().ForEach((ref Rotation rutate) =>
        {
            //Clearly not 
            //Debug.Log("Is this shit even working");
            //Debug.Log("LocalWorldComp is" + worldcomp);

            ref var booked = ref baked.BezzyGraphcomp.Value;

            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;


            var placeonSpline = timedeelta;


            float3 Bezpoint0 = new float3();
            float3 Bezpoint1 = new float3();
            float3 Bezpoint2 = new float3();
            float3 Bezpoint3 = new float3();


            //placeonSpline = Mathf.Clamp01(placeonSpline);
            //placeonSpline = math.saturate(placeonSpline);

            int i;
            if (placeonSpline >= 1f)
            {
                placeonSpline = 1f;
                i = booked.Bezzypoint.Length - 4;
            }
            else
            {
                placeonSpline = math.saturate(placeonSpline) * Curvecount;
                i = (int)placeonSpline;
                placeonSpline -= i;
                i *= 3;
            }

            Bezpoint0 = booked.Bezzypoint[i];
            Bezpoint1 = booked.Bezzypoint[i + 1];
            Bezpoint2 = booked.Bezzypoint[i + 2];
            Bezpoint3 = booked.Bezzypoint[i + 3];

            placeonSpline = math.saturate(placeonSpline);


            float oneMinusT = 1f - placeonSpline;

            //3u ^ 2 * p0 + 6u * p1 + 6t* p2 +3t ^ 2 * p3

            //            public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            //                t = Mathf.Clamp01(t);
            //                float oneMinusT = 1f - t;
            //                return
            //                    3f * oneMinusT * oneMinusT * (p1 - p0) +
            //                    6f * oneMinusT * t * (p2 - p1) +
            //                    3f * t * t * (p3 - p2);
            //            }
            // return
            //GetFirstDerivative This is what I need to use to 
            float3 BezzyposVelocity = 3f * oneMinusT * oneMinusT * (Bezpoint1 - Bezpoint0) + 6f * oneMinusT * placeonSpline * (Bezpoint2 - Bezpoint1) + 3f * placeonSpline * placeonSpline * (Bezpoint3 - Bezpoint2);

            //var yourTRS = float4x4.TRS(thispos.Value, rutate.Value, 1);

            //var bezzyrotationmaybe = quaternion.identity;

            //var bezzyTRS = float4x4.TRS(bezzypositionmaybe, bezzyrotationmaybe, 1);

            //var resultPoint = math.mul(yourTRS, bezzyTRS);

            //var resultdirection = resultPoint.c3.xyz;

            //Todo What is this thing and how can I use it
            //var tempvar = worldcomp.Value[0];

            //Matrix4x4 matrix4test = worldcomp.Value;

            //var matrix = transform.localToWorldMatrix;


            //            if (lookForward) {
            //                transform.LookAt(position + spline.GetDirection(progress));
            //            }

            //Matrix4x4 matrix4x4 = Matrix4x4.identity;
            //matrix4x4 .SetTRS(translation, Rotation, Scale);

            //Vector3 worldPositon = matrix4x4.MultiplyPoint3x4(bezzypositionmaybe);

            //var Dooodey = bezzypositionmaybe - thispos.Value;

            //This is the getvelocitymethod who knows if it works
            //Vector3 worldpos = matrix4test.MultiplyPoint3x4(bezzypositionmaybe - thispos.Value);

            //transform.LookAt(position + spline.GetDirection(progress));

            //This is where I get the velocity
            //var tempworldpos = worldpos.normalized;


            //var templookat = thispos.Value + (float3)tempworldpos;

            //Todo I think I need to normalise the BezierVelocity          

            var tempval = math.normalize(BezzyposVelocity);
            //BezzyposVelocity
           
            rutate.Value = quaternion.LookRotation(tempval, math.up());

            //rutate.Value = resultPoint //quaternion.LookRotation(resultdirection, math.up());

            //Yes this is the translation
            //var tempvis = yourTRS.c3.xyz;
            //Debug.Log(tempvis);
            //var splineTRS = float4x4.TRS(

            //float4 resultPoint = math.mul(yourTRS, (wtfevenisthis, 1, 1));

            //var tempvar = 

            //math.transform()

            //            public Vector3 GetDirection (float t) {
            //                return GetVelocity(t).normalized;
            //            }

            //math.transform(LocalToWorld, myPoint)
            //            

            // var yourTRS = float4x4.TRS();
            // float4 resultPoint = math.mul(yourTRS ,(BezierResult.xyz,1))

            //            public Vector3 GetVelocity (float t) {
            //                return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t)) - transform.position;
            //            }

            //           Todo  I dont think I need to use this
            //            public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            //                return
            //                    2f * (1f - t) * (p1 - p0) +
            //                    2f * t * (p2 - p1);
            //            }

            //            public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            //                t = Mathf.Clamp01(t);
            //                float oneMinusT = 1f - t;
            //                return
            //                    3f * oneMinusT * oneMinusT * (p1 - p0) +
            //                    6f * oneMinusT * t * (p2 - p1) +
            //                    3f * t * t * (p3 - p2);
            //            }



        }).Run();
        
        

        //this.Enabled = false;

        return inputDeps;
    }


}



//public struct Vector3 GetPoint(float t)
//{
//    int i;
//    if (t >= 1f)
//    {
//        t = 1f;
//        i = points.Length - 4;
//    }
//    else
//    {
//        t = Mathf.Clamp01(t) * CurveCount;
//        i = (int)t;
//        t -= i;
//        i *= 3;
//    }
//    return transform.TransformPoint(Bezier.GetPoint(
//        points[i], points[i + 1], points[i + 2], points[i + 3], t));
//}

//todo I'm trying to figure out how to use structs to contain logic or code that is universal like maybe I don't just need to use the Bezier in the above maybe I might want to use it elsewhere for different things
//NativeArray<TestCompareUnit> testArray = new NativeArray<TestCompareUnit>(1000, Allocator.Temp);
//for (int i = 0; i < testArray.Length; i++)
//{
//TestCompareUnit item;
//item.isPlayer = System.Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
//item.speed = UnityEngine.Random.Range(0, 100f);
//testArray[i] = item;
//}
//testArray.Sort(new TestComparer());
//
//System.Text.StringBuilder sb = new System.Text.StringBuilder();
//
//for (int i = 0; i < testArray.Length; i++)
//{
//TestCompareUnit item = testArray[i];
//sb.AppendLine(item.ToString());
//}
//
//Debug.Log(sb.ToString());

public struct TestCompareUnit
{
    public bool isPlayer;
    public float speed;

    public override string ToString()
    {
        return isPlayer + " " + speed;
    }
}

public struct TestComparer : IComparer<TestCompareUnit>
{
    public int Compare(TestCompareUnit x, TestCompareUnit y)
    {
        if (x.isPlayer && !y.isPlayer) return -1;
        if (!x.isPlayer && y.isPlayer) return 1;
        if (x.speed > y.speed) return -1;
        if (x.speed < y.speed) return 1;
        return 0;
    }
}

public struct CurveCount
{
    public int[] points;

    public int GetCurveCount() 
    { var curves = ( - 1) / 3;

        return curves;
    }

}

public struct Coords
{
    public int x, y;

    public Coords(int p1, int p2)
    {
        x = p1;
        y = p2;
    }
}

public struct Employee
{
    public int EmpId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Employee(int empid, string fname, string lname)
    {
        EmpId = empid;
        FirstName = fname;
        LastName = lname;
    }

    public string GetFullName()
    {
        return FirstName + " " + LastName;
    }
}

// public struct BezierCalculate(float t)
//{
//    float u = 1.0f - t;
//    float tt = t * t;
//    float  uu = u * u;
//    float uuu = uu * u;
//    float ttt = tt * t;
//
//    //float3 p = uuu * wtf[0].Value.Value; //first term
//    //p += 3 * uu * t * wtf[1].Value.Value; //second term
//    //p += 3 * u * tt * wtf[2].Value.Value; //third term
//    //p += ttt * wtf[3].Value.Value; //fourth term
//    
//    return(pttt);
//
//}


//public float deltaTime;
//
//public void Execute(DynamicBuffer<MarketDataBuffer> marketDatas)
//{
//for (int i = 0; i < marketDatas.Length; i++)
//{
//    var marketData = marketDatas[i];
//                
//    //CalculateNewPrice
//               
//    var multiplier = (marketData.supply - marketData.demand) / (float)marketData.demand;
//
//    multiplier = math.select(math.abs(multiplier) * 10, 1/multiplier, multiplier >0);
//    multiplier = math.clamp(multiplier, 0.1f, 2.0f);
//
//    var targetPrice = marketData.basePrice * multiplier;
//
//    marketData.currentPrice = math.lerp(marketData.currentPrice, targetPrice, 0.04f * deltaTime);
//
//    //Calculate Level and Trend
//    var priceDifference = marketData.currentPrice / marketData.basePrice;
//    var trend = marketData.currentPrice / targetPrice;
//
//    MarketPriceLevel priceLevel =
//        (MarketPriceLevel)math.select(
//            math.select(
//                math.select(math.select((int)MarketPriceLevel.NORMAL, (int)MarketPriceLevel.BELOW, priceDifference <= 0.9f), (int)MarketPriceLevel.FARBELOW, priceDifference <= 0.5f), (int)MarketPriceLevel.ABOVE, priceDifference >= 1.1f) , (int)MarketPriceLevel.FARABOVE, priceDifference >= 1.5f);
//
//    MarketPriceTrend trendLevel =
//        (MarketPriceTrend)math.select(
//            math.select(
//                math.select(math.select((int)MarketPriceTrend.STILL, (int)MarketPriceTrend.INCREASING, trend <= 0.9f), (int)MarketPriceTrend.HIGHLYINCREASING, trend <= 0.5f), (int)MarketPriceTrend.DECREASING, trend >= 1.1f), (int)MarketPriceTrend.HIGHLYDECREASING, trend >= 1.5f);
//
//    marketData.priceLevel = priceLevel;
//    marketData.priceTrend = trendLevel;
//
//    marketDatas[i] = marketData;
//This should work better than the above math.selects
// float4 A = new float4(1.5f, 1.1f, 0.9f, 0.5f);
//        float4 B = new float4(priceDifference);
//        bool4 mask = A <= B;
//        int4 Y = math.select(new int4(0), new int4(1), mask);
//        int result = math.dot(Y, new int4(1)); // from 0 to 4