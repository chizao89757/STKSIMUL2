using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKX;
using System.IO;
using System.Collections;
using AGI.STKObjects;
using AGI.STKUtil;

namespace STKSIMUL
{
    class SARSYS
    {
        #region 事件
        public delegate void myeventhandler();
        public event myeventhandler ScPathChange;//初始化完成事件

        #endregion

        #region 属性/成员


        #region 内部属性/成员
        private string scpath="";//sc工程路径

        private string simstarttime = "1 Nov 2015 00:00:00.00";//所有obj的仿真时间范围（其实也就是卫星）
        private string simstoptime = "2 Nov 2015 00:00:00.00";
        private double simstep = 60;//仿真间隔，好像也是卫星用


        private double sasem = 42164.17;//SemiMajorAxis半长轴
        private double saecc = 0.03;//Eccentricity离心率
        private double saaop = 270;//argument of perigee近地点幅角
        private double sainc = 40;//inclination轨道倾角
        private double salan = 92;//longitude of ascending node升交点赤经
        private double satra = 0;//true anomaly真近点角

        private double racoang = 0.5;//雷达张角
        private double raeleva = 86;//雷达仰角

        private IAgUnitPrefsDimCollection dimensions;//设定全局单位、偏好

        private IAgScenario scene;//设置仿真开始结束时间

        private IAgSatellite tdrs;//卫星
        private IAgVePropagatorTwoBody twobody;
        private IAgOrbitStateClassical classical;

        private IAgAircraft cruise;
        private IAgVePropagatorGreatArc greatArc;

        private IAgSensor radar_t;

        private IAgSensor radar_r;
        private IAgSensor target;
        //飞机waypoints,纬度，经度，高度，速度，转向速度

        private double[] plwaypoints = new double[]{
            -36.6,111.5,30,0.22,0,
            -22,118,30,0.22,0,
            14,113,30,0.22,0,
            40,110,30,0.22,0,
            34,122,30,0.22,0,
            -26,92,30,0.22,0
            };



        #endregion

        #region 内部属性/成员封装

        public double[] PlWayPoints
        {
            get
            {
                return plwaypoints;
            }
            set
            {
                plwaypoints = value;

                greatArc.Waypoints.RemoveAll();
                for (int ii = 0; ii < plwaypoints.Length;ii+=5 )
                {

                    AddWaypoint(greatArc.Waypoints,
                        plwaypoints[ii], 
                        plwaypoints[ii+1], 
                        plwaypoints[ii+2], 
                        plwaypoints[ii+3], 
                        plwaypoints[ii+4]
                        );
                }

                    
                greatArc.Propagate();
                root.Rewind();
            }
        }



        public string ScPath
        {
            get { return scpath; }//可用GetDirectory指令解决！！待处理
        }


        public DateTime SimStartTime//在哪里和通用时间格式转换？
        {
            get
            {
                return DateTime.Parse(simstarttime);
            }
            set
            {
                simstarttime = (value.ToString("r")).Substring(5, 20) + ".000";
                scene.StartTime = simstarttime;
                scene.Epoch = simstarttime;

                twobody.EphemerisInterval.SetStartAndStopTimes(simstarttime, simstoptime);
                twobody.Propagate();
                root.Rewind();

            }
        }
        public DateTime SimStopTime
        {
            get
            {
                return DateTime.Parse(simstoptime);
            }
            set
            {
                simstoptime = (value.ToString("r")).Substring(5, 20) + ".000";

                scene.StopTime = simstoptime;

                twobody.EphemerisInterval.SetStartAndStopTimes(simstarttime, simstoptime);
                twobody.Propagate();
                root.Rewind();
            }
        }
        public double SimStep
        {
            get
            {
                return simstep;
            }
            set
            {
                simstep = value;
                scene.Animation.AnimStepValue = simstep;
            }
        }

        public double SaSemiMajorAxis 
        {
            get
            {
                return sasem;
            }
            set
            {

                sasem = value;
                IAgClassicalSizeShapeSemimajorAxis semi = (IAgClassicalSizeShapeSemimajorAxis)classical.SizeShape;
                semi.SemiMajorAxis = sasem;

                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();
            }
        }
        public double SaEccentricity
        {
            get
            {
                return saecc;
            }
            set
            {
                saecc = value;
                IAgClassicalSizeShapeSemimajorAxis semi = (IAgClassicalSizeShapeSemimajorAxis)classical.SizeShape;
                semi.Eccentricity = saecc;

                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();
            }
        }

        public double SaArgOfPerigee
        {
            get
            {
                return saaop;
            }
            set
            {
                saaop = value;
                classical.Orientation.ArgOfPerigee = saaop;
                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();
            }
        }

        public double Sainclination
        {
            get
            {
                return sainc;
            }
            set
            {
                sainc = value;
                classical.Orientation.Inclination = sainc;
                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();

            }
        }
        public double SaLongAscendingNode
        {
            get
            {
                return salan;
            }
            set
            {
                salan = value;

                ((IAgOrientationAscNodeLAN)classical.Orientation.AscNode).Value = salan;
                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();

            }
        }
        public double SaTrueAnomaly
        {
            get
            {
                return satra;
            }
            set
            {
                satra = value;
                ((IAgClassicalLocationTrueAnomaly)classical.Location).Value = satra;
                twobody.InitialState.Representation.Assign(classical);
                twobody.Propagate();
                root.Rewind();

            }
        }


        public double RaConeAngle
        {
            get
            {
                return racoang;
            }
            set
            {
                racoang = value;

                ((IAgSnSimpleConicPattern)radar_t.Pattern).ConeAngle = racoang;
                root.Rewind();
            }
        }
        public double RaElevation
        {
            get
            {
                return raeleva;
            }
            set
            {
                raeleva = value;
                IAgSnPtFixed fixedPt = (IAgSnPtFixed)radar_t.Pointing;
                IAgOrientationAzEl azEl = (IAgOrientationAzEl)fixedPt.Orientation.ConvertTo(AgEOrientationType.eAzEl);
                azEl.Elevation = raeleva;
                azEl.AboutBoresight = AgEAzElAboutBoresight.eAzElAboutBoresightRotate;
                fixedPt.Orientation.Assign(azEl);
                root.Rewind();
            }
        }

        #endregion

        #region 静态属性/成员
        private static SARSYS defsys;//单例
        private static AgSTKXApplication rootapp;//stk操作入口
        private static AGI.STKObjects.AgStkObjectRoot root;//也是一个stk操作入口
        //但必须要上面那个new之后才能new下面这个，偶然发现的，好神奇
        #endregion

        #endregion

        #region 方法

        #region 静态公共方法
        /// <summary>
        /// 静态初始化方法
        /// </summary>
        static SARSYS()
        {
            rootapp = new AgSTKXApplication();
            root = new AGI.STKObjects.AgStkObjectRoot();
        }

        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static SARSYS createsys()
        {
            if (defsys == null)
            {
                defsys = new SARSYS();
            }
            return defsys;

        }

        #endregion

        #region 公开方法
        /// <summary>
        /// 新建一个SARSYS在当前sc文件中
        /// 需要建立最基础的系统
        /// 参数大多数有默认值
        /// </summary>
        public void newsarsys()
        {
            #region 写入信息
            AGI.STKX.IAgExecCmdResult resultmsg = rootapp.ExecuteCommand("SetDescription * long SARSYS");
            #endregion


            #region 设定单位、仿真时间
            // Reset the units to the STK defaults
            dimensions = root.UnitPreferences;
            dimensions.ResetUnits();

            // Set the date unit, acquire an interface to the scenario and use
            // it to set the time period and epoch
            dimensions.SetCurrentUnit("DateFormat", "UTCG");

            scene = (IAgScenario)root.CurrentScenario;
            scene.StartTime = simstarttime;
            scene.StopTime = simstoptime;
            scene.Epoch = simstarttime;

            //rootapp.ExecuteCommand("MapTracking * UTM");

            // Set new preference for Temperature
            dimensions.SetCurrentUnit("Temperature", "degC");
            #endregion

            #region 放置发射站
            //SATELLITE #1: TDRS
            //Assign a two-body propagator to propagate it
            tdrs = (IAgSatellite)root.CurrentScenario.Children.New(AgESTKObjectType.
                eSatellite, "TDRS");
            tdrs.SetPropagatorType(AgEVePropagatorType.ePropagatorTwoBody);
            twobody = (IAgVePropagatorTwoBody)tdrs.Propagator;

            //Define the TDRS satellite's orbit using
            //classical (Keplerian) orbital elements
            classical = (IAgOrbitStateClassical)twobody.InitialState.Representation.ConvertTo(AgEOrbitStateType.eOrbitStateClassical);

            //Set J2000 as the coordinate system
            //and set the time period and time step
            classical.CoordinateSystemType = AgECoordinateSystem.eCoordinateSystemJ2000;
            twobody.EphemerisInterval.SetStartAndStopTimes(simstarttime, simstoptime);
            twobody.Step = 60;

            //定义半主轴长度，离心率
            //Use period and eccentricity to define the size
            //and shape of the orbit
            classical.SizeShapeType = AgEClassicalSizeShape.eSizeShapeSemimajorAxis;
            IAgClassicalSizeShapeSemimajorAxis semi = (IAgClassicalSizeShapeSemimajorAxis)classical.SizeShape;
            semi.SemiMajorAxis = sasem;
            semi.Eccentricity = saecc;


            //定义轨道倾角，升交点赤经，近地点幅角
            //Use argument of perigee, inclination
            //and longitude of ascending node to
            //define the orientation of the orbit
            classical.Orientation.ArgOfPerigee = saaop;
            classical.Orientation.Inclination = sainc;
            classical.Orientation.AscNodeType = AgEOrientationAscNode.eAscNodeLAN;
            IAgOrientationAscNodeLAN lan = (IAgOrientationAscNodeLAN)classical.Orientation.AscNode;
            lan.Value = salan;

            //定义真近点角（？）来定义初始位置
            //Use true anomaly to specify the position of
            //the satellite in orbit
            classical.LocationType = AgEClassicalLocation.eLocationTrueAnomaly;
            IAgClassicalLocationTrueAnomaly trueAnomaly = (IAgClassicalLocationTrueAnomaly)classical.Location;
            trueAnomaly.Value = satra;


            //Assign the orbital elements to the TDRS
            //satellite's propagator and propagate the orbit	
            twobody.InitialState.Representation.Assign(classical);
            twobody.Propagate();

            root.Rewind();

            #endregion

            
            #region 放置接收站
            cruise = (IAgAircraft)root.CurrentScenario.Children.New(AgESTKObjectType.eAircraft, "Cruise");
            cruise.SetRouteType(AgEVePropagatorType.ePropagatorGreatArc);
            greatArc = (IAgVePropagatorGreatArc)cruise.Route;
            greatArc.EphemerisInterval.SetStartAndStopTimes(simstarttime, simstoptime);
            greatArc.Method = AgEVeWayPtCompMethod.eDetermineTimeAccFromVel;

            ((IAgVOModelFile)cruise.VO.Model.ModelData).Filename = @"\STKData\VO\Models\Air\rq-4a_globalhawk.mdl";
            cruise.VO.Offsets.Rotational.Enable = true;
            cruise.VO.Offsets.Rotational.X = 180;

            //Use the convenience method defined above
            //to add waypoints specifying the ship's route
            for (int ii = 0; ii < plwaypoints.Length; ii += 5)
            {
                AddWaypoint(greatArc.Waypoints, plwaypoints[ii], plwaypoints[ii + 1], plwaypoints[ii + 2], plwaypoints[ii + 3], plwaypoints[ii + 4]);
            }
              
            
            cruise.SetAttitudeType(AgEVeAttitude.eAttitudeStandard);
            IAgVeRouteAttitudeStandard attitude = (IAgVeRouteAttitudeStandard)cruise.Attitude;
            attitude.Basic.SetProfileType(AgEVeProfile.
                eProfileECFVelocityAlignmentWithRadialConstraint);
            cruise.Graphics.WaypointMarker.IsWaypointMarkersVisible = true;
            cruise.Graphics.WaypointMarker.IsTurnMarkersVisible = true;
            greatArc.Propagate();
            root.Rewind();


            #endregion

            #region 放置发射站雷达
            radar_t = (IAgSensor)root.CurrentScenario.Children["TDRS"].Children.New(AgESTKObjectType.eSensor, "radar_t");
            radar_t.SetPatternType(AgESnPattern.eSnSimpleConic);
            ((IAgSnSimpleConicPattern)radar_t.Pattern).ConeAngle = racoang;



            //Select a Fixed pointing type and the Az-El
            //orientation type, and set the elevation angle to
            //90 deg, so that the sensor points straight down
            //with reference to the satellite
            radar_t.SetPointingType(AgESnPointing.eSnPtFixed);
            IAgSnPtFixed fixedPt = (IAgSnPtFixed)radar_t.Pointing;
            IAgOrientationAzEl azEl = (IAgOrientationAzEl)fixedPt.Orientation.ConvertTo(AgEOrientationType.eAzEl);
            azEl.Elevation = raeleva;
            azEl.AboutBoresight = AgEAzElAboutBoresight.eAzElAboutBoresightRotate;
            fixedPt.Orientation.Assign(azEl);

            radar_t.Graphics.FillVisible = true;
            radar_t.VO.FillVisible = true;
            

            root.Rewind();


            #endregion


            #region 放置接收站雷达


            target = (IAgSensor)root.CurrentScenario.Children["TDRS"].Children.New(AgESTKObjectType.eSensor, "target");

            target.SetLocationType(AgESnLocation.eSnLocationCrdnPoint);
            IAgLocationCrdnPoint vgtPoint = target.LocationData as IAgLocationCrdnPoint;
            //vgtPoint.PointPath = "TDRS/radar_t BoresightIntersection(Terrain)";
            vgtPoint.PointPath = "Satellite/TDRS/Sensor/radar_t BoresightIntersection(Terrain)";
            target.SetPatternType(AgESnPattern.eSnSimpleConic);
            ((IAgSnSimpleConicPattern)target.Pattern).ConeAngle = 0.00001;
           
            

            IAgSnPtTargeted targetedSensor1 = target.CommonTasks.SetPointingTargetedTracking(
   AgETrackModeType.eTrackModeTranspond, AgEBoresightType.eBoresightRotate, "*/Aircraft/Cruise");


            target.SetPointingType(AgESnPointing.eSnPtTargeted);
            IAgSnPtTargeted rpt1 = (IAgSnPtTargeted)target.Pointing;
            rpt1.Boresight = AgESnPtTrgtBsightType.eSnPtTrgtBsightTracking;
            root.Rewind();
            rootapp.ExecuteCommand("Graphics */Satellite/TDRS/Sensor/target Show Off");


            /////////////////////////
            radar_r = (IAgSensor)root.CurrentScenario.Children["Cruise"].Children.New(AgESTKObjectType.eSensor, "radar_r");
            radar_r.SetPatternType(AgESnPattern.eSnSimpleConic);
            ((IAgSnSimpleConicPattern)radar_r.Pattern).ConeAngle = 3;
        

            IAgSnPtTargeted targetedSensor = radar_r.CommonTasks.SetPointingTargetedTracking(
    AgETrackModeType.eTrackModeTranspond, AgEBoresightType.eBoresightRotate, "*/Satellite/TDRS/Sensor/target");   

            
            radar_r.SetPointingType(AgESnPointing.eSnPtTargeted);
            IAgSnPtTargeted rpt = (IAgSnPtTargeted)radar_r.Pointing;
            rpt.Boresight = AgESnPtTrgtBsightType.eSnPtTrgtBsightTracking;
            

            root.Rewind();
            #endregion
        }
        public void focusearth()
        {
            string cmdstr = "VO * View FromTo FromRegName \"STK Object\" FromName \"Satellite/TDRS\" ToRegName \"View Central Body\" ToName \"Earth\"";
            rootapp.ExecuteCommand(cmdstr);
        }
        public void focussate()
        {
            string cmdstr = "VO * View FromTo FromRegName \"STK Object\" FromName \"Satellite/TDRS\" ToRegName \"STK Object\" ToName \"Satellite/TDRS\"";
            rootapp.ExecuteCommand(cmdstr);
        }
        public void focusplane()
        {
            string cmdstr = "VO * View FromTo FromRegName \"STK Object\" FromName \"Aircraft/Cruise\" ToRegName \"STK Object\" ToName \"Aircraft/Cruise\"";
            rootapp.ExecuteCommand(cmdstr);
        }
        /// <summary>
        /// 新建方法
        /// </summary>
        public void newsc()
        {
            rootapp.ExecuteCommand("Unload / *");
            rootapp.ExecuteCommand("New / Scenario newsys ");
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fp"></param>
        public void savesc(string fp)
        {
            if (File.Exists(fp))
            {
                File.Delete(fp);
            }
            rootapp.ExecuteCommand("SaveAs / * " + fp);
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="fp"></param>
        public void loadsc(string fp)
        {
            rootapp.ExecuteCommand("Unload / *");
            rootapp.ExecuteCommand("Load / Scenario " + fp);
            
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void unloadsc()
        {
            if (root.CurrentScenario != null)
            {
                root.CloseScenario();
            }
            //rootapp.ExecuteCommand("Unload / *");
        }

        /// <summary>
        /// 检查是否已有系统存在于打开的工程文件中
        /// </summary>
        /// <returns></returns>
        public bool checkifdefsys()
        {
            AGI.STKX.IAgExecCmdResult resultmsg= rootapp.ExecuteCommand("GetDescription * long");
            if ((resultmsg.IsSucceeded==true)&&(resultmsg.Count>=1))
            {
                string checkmsg=(string)resultmsg[0];
                if(checkmsg=="SARSYS")
                    return true;
            }

            return false ;
        }

        /// <summary>
        /// 读出当前sc文件中已经存在的SARSYS
        /// </summary>
        public void readsarsys()
        {

            AGI.STKX.IAgExecCmdResult resultmsg = rootapp.ExecuteCommand("GetDescription * long");


            dimensions = root.UnitPreferences;
            scene = (IAgScenario)root.CurrentScenario;
            tdrs = (IAgSatellite)root.CurrentScenario.Children["TDRS"];
            
            twobody = (IAgVePropagatorTwoBody)tdrs.Propagator;
            classical = (IAgOrbitStateClassical)twobody.InitialState.Representation.ConvertTo(AgEOrbitStateType.eOrbitStateClassical);

            cruise = (IAgAircraft)root.CurrentScenario.Children["Cruise"];
            greatArc = (IAgVePropagatorGreatArc)cruise.Route;

            radar_t = (IAgSensor)root.CurrentScenario.Children["TDRS"].Children["radar_t"];

            radar_r = (IAgSensor)root.CurrentScenario.Children["Cruise"].Children["radar_r"];

            target = (IAgSensor)root.CurrentScenario.Children["TDRS"].Children["target"];

            simstarttime = scene.StartTime;
            simstoptime = scene.StopTime;
            simstep = scene.Animation.AnimStepValue;
            IAgClassicalSizeShapeSemimajorAxis semi = (IAgClassicalSizeShapeSemimajorAxis)classical.SizeShape;
            sasem = semi.SemiMajorAxis;
            saecc = semi.Eccentricity;
            saaop = classical.Orientation.ArgOfPerigee;
            sainc = classical.Orientation.Inclination;
            salan = ((IAgOrientationAscNodeLAN)classical.Orientation.AscNode).Value;
            satra = ((IAgClassicalLocationTrueAnomaly)classical.Location).Value;
            racoang = ((IAgSnSimpleConicPattern)radar_t.Pattern).ConeAngle;

            IAgSnPtFixed fixedPt = (IAgSnPtFixed)radar_t.Pointing;
            IAgOrientationAzEl azEl = (IAgOrientationAzEl)fixedPt.Orientation.ConvertTo(AgEOrientationType.eAzEl);
            raeleva = azEl.Elevation;

            plwaypoints = new double[5 * greatArc.Waypoints.Count];
            for(int ii=0;ii<greatArc.Waypoints.Count;ii++)
            {
                plwaypoints[ii * 5] = greatArc.Waypoints[ii].Latitude;
                plwaypoints[ii * 5 + 1] = greatArc.Waypoints[ii].Longitude;
                plwaypoints[ii * 5 + 2] = greatArc.Waypoints[ii].Altitude;
                plwaypoints[ii * 5 + 3] = greatArc.Waypoints[ii].Speed;
                plwaypoints[ii * 5 + 4] = greatArc.Waypoints[ii].TurnRadius;
            }




        }


        public void playf()
        {
            root.PlayForward();
        }

        public void playb()
        {
            root.PlayBackward();
        }
        public void playp()
        {
            root.Pause();
        }


        #endregion
        
        #region 内部方法
        /// <summary>
        /// 构造方法
        /// </summary>
        private SARSYS()
        {

            rootapp.OnScenarioLoad += scfilepathchange;
            rootapp.OnScenarioNew += scfilepathchange;
            rootapp.OnScenarioSave += scfilepathchange;
            rootapp.OnScenarioClose += rootapp_OnScenarioClose;
            this.ScPathChange += SARSYS_SysInitComplete;

        }

        void rootapp_OnScenarioClose()
        {
            scpath = "";
            this.ScPathChange();//抛出sc目录改变事件
        }
        /// <summary>
        /// sc目录改变事件
        /// </summary>
        /// <param name="sp"></param>
        private void scfilepathchange(string sp)
        {
            scpath = sp;
            this.ScPathChange();//抛出sc目录改变事件
        }


        /// <summary>
        /// 默认事件方法
        /// </summary>
        private void SARSYS_SysInitComplete() { }
        private void AddWaypoint(IAgVeWaypointsCollection waypoints, object lat, object lon, double alt, double speed, double tr)
        {
            IAgVeWaypointsElement elem = waypoints.Add();
            elem.Latitude = (double)lat;
            elem.Longitude = (double)lon;
            elem.Altitude = alt;
            elem.Speed = speed;
            elem.TurnRadius = tr;
        }
        #endregion

        #endregion




    }
}
