using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace STKSIMUL
{
    class complex {
        public complex() {
            real = 0;
            imag = 0;
        }
        public double real;       //实部
        public double imag;       //虚部
    }

    class FFT
    {
        private static double PI = 3.1415926535897932384626433832795028841971;
        public static void conjugate_complex(int n,complex[] in1,complex[] out1) {
          int i = 0;
          for(i=0;i<n;i++) {
            out1[i].imag = -in1[i].imag;
            out1[i].real = in1[i].real;
          }
        }

        public static void c_abs(complex[] f,double[] out1,int n) {
          int i = 0;
          double t;
          for(i=0;i<n;i++) {
            t = f[i].real * f[i].real + f[i].imag * f[i].imag;
            out1[i] = Math.Sqrt(t);
          }
        }

        public static void c_plus(complex a, complex b, complex c)
        {
          c.real = a.real + b.real;
          c.imag = a.imag + b.imag;
        }

        public static void c_sub(complex a, complex b, complex c)
        {
          c.real = a.real - b.real;
          c.imag = a.imag - b.imag;
        }

        public static void c_mul(complex a, complex b, complex c)
        {
          c.real = a.real * b.real - a.imag * b.imag;
          c.imag = a.real * b.imag + a.imag * b.real;
        }

        public static void c_div(complex a, complex b, complex c)
        {
          c.real = (a.real * b.real + a.imag * b.imag)/(b.real * b.real +b.imag * b.imag);
          c.imag = (a.imag * b.real - a.real * b.imag)/(b.real * b.real +b.imag * b.imag);
        }

        public static void Wn_i(int n, int i, complex Wn, int flag)
        {
          Wn.real = Math.Cos(2*PI*i/n);
          if(flag == 1)
          Wn.imag = -Math.Sin(2*PI*i/n);
          else if(flag == 0)
          Wn.imag = -Math.Sin(2*PI*i/n);
        }

        //傅里叶变化
        public static void fft(int N, complex[] f)
        {
          complex t=new complex(),wn=new complex();//中间变量
          int i,j,k,m,n,l,r,M;
          int la,lb,lc;
          /*----计算分解的级数M=log2(N)----*/
          for(i=N,M=1;(i=i/2)!=1;M++);
          /*----按照倒位序重新排列原信号----*/
          for(i=1,j=N/2;i<=N-2;i++) {
            if(i<j)
            {
              t=f[j];
              f[j]=f[i];
              f[i]=t;
            }
            k=N/2;
            while(k<=j)
            {
              j=j-k;
              k=k/2;
            }
            j=j+k;
          }

          /*----FFT算法----*/
          for(m=1;m<=M;m++) {
            la=(int)Math.Pow(2,m); //la=2^m代表第m级每个分组所含节点数
            lb=la/2;    //lb代表第m级每个分组所含碟形单元数
                         //同时它也表示每个碟形单元上下节点之间的距离
            /*----碟形运算----*/
            for(l=1;l<=lb;l++) {
              r=(int)((l-1)*Math.Pow(2,M-m));
              for(n=l-1;n<N-1;n=n+la) //遍历每个分组，分组总数为N/la
              {
                lc=n+lb;  //n,lc分别代表一个碟形单元的上、下节点编号
                Wn_i(N,r,wn,1);//wn=Wnr
                c_mul(f[lc],wn,t);//t = f[lc] * wn复数运算
                c_sub(f[n],t,(f[lc]));//f[lc] = f[n] - f[lc] * Wnr
                c_plus(f[n],t,(f[n]));//f[n] = f[n] + f[lc] * Wnr
              }
            }
          }
        }

        //傅里叶逆变换
        public static void ifft(int N, complex[] f)
        {
          int i=0;
          conjugate_complex(N,f,f);
          fft(N,f);
          conjugate_complex(N,f,f);
          for(i=0;i<N;i++)
          {
            f[i].imag = (f[i].imag)/N;
            f[i].real = (f[i].real)/N;
          }
        }

    }

    class imgsim
    {
        class Matrix {
            public Matrix() {
                width = 0;
                height = 0;
                elements = null;
            }
            public int width;
            public int height;
            public complex[] elements;
        }
        private static double c=3e8;
        private static double PI=3.1415926535897932384626433832795028841971;
        private static long RanNum = 256;
        private static long AziNum = 256;
        private static long Nf = RanNum;
        private static long Ns = AziNum;
        private static double Tp=5e-6; //发射信号的时宽
        private static double BandWidth=100.0e6; //发射信号的带宽
        private static double Rate_Fs=1.4; //快时间域的过采样系数为3,频带模糊说明采样率不够，需要增大过采样系数以防止频带模糊
        private static double Kr=BandWidth/Tp; //发射信号的调频斜率
        private static double Fc=1.25e9; //载波频率
        private static double PRF=700.0; //脉冲重复频率
        private static double Tsar=4.0; //一个合成孔径时间
        private static double Vr = 600.0; //接收机飞行速度
        private static long BpAziNum=128;
        private static long BpRanNum=128;
        private static double deltaX=1.0;
        private static double deltaY=1.0;
        private static double Coaf = 4;
        private static double R_center;

        private static double geo_fuc=0.03;
        private static double geo_Omega=270.0*PI/180;
        private static double geo_i=53.0*PI/180;
        private static double geo_w=270.0*PI/180;
        private static double geo_gama=4.0*PI/180;
        private static double geo_thetaRR = 30.0; //接收站速度与目前坐标系y轴方向的夹角

        private static double[] TargetCenter = { 0, 0, 0 };
        private static double[] Location_R = { -5000, 0, 8000 };

        private static double SapRate = Rate_Fs*BandWidth;
        private static double SapTime = 1 / SapRate;
        private static double lambda = c / Fc;
        private static double PRI = 1 / PRF;
        private static double PRT = PRI;



        [DllImport("imgsim.dll")]
        public static extern int getpic();

        public static void createRanRef(complex[] ran_ref, double[] Tr)
        {
	        for (int i = 0; i<RanNum; i++) {
		        if (Math.Abs(Tr[i])<Tp / 2) {
			        ran_ref[i].real = Math.Cos(PI*Kr*Tr[i] * Tr[i]);
			        ran_ref[i].imag = Math.Sin(PI*Kr*Tr[i] * Tr[i]);
		        }
		        else {
			        ran_ref[i].real = 0;
			        ran_ref[i].imag = 0;
		        }
	        }
        }

        public static void fftshift(complex[] ran_ref, long len)
        {
	        double x, y;
	        for (int i = 0; i<len / 2; i++) {
		        x = ran_ref[i].real;
		        y = ran_ref[i].imag;
		        ran_ref[i].real = ran_ref[i + len / 2].real;
		        ran_ref[i].imag = ran_ref[i + len / 2].imag;
		        ran_ref[i + len / 2].real = x;
		        ran_ref[i + len / 2].imag = y;
	        }
        }

        public static void calculateData(complex[] data, double[] Tr, double tp, double[] R_path, double[] T_path, double[] Targets, double[] sigma, long TargetNum, double R_center, double ApertureRange_min, double ApertureRange_max)
        {
	        for (int j = 0; j<AziNum; j++) {
		        for (int i = 0; i<RanNum; i++) {
			        data[j*RanNum + i].real = 0;
			        data[j*RanNum + i].imag = 0;
			        for (int t = 0; t < TargetNum; t++) {
				        double TargetRange = Targets[t * 3 + 1] - R_path[3 * j + 1];
				        if ((TargetRange >= (ApertureRange_min)) && (TargetRange<(ApertureRange_max))) {
					        double Rt = Math.Sqrt(Math.Pow(T_path[3 * j + 0] - Targets[3 * t], 2) + Math.Pow(T_path[3 * j + 1] - Targets[3 * t + 1], 2) + Math.Pow(T_path[3 * j + 2] - Targets[3 * t + 2], 2));
					        double Rr = Math.Sqrt(Math.Pow(R_path[3 * j + 0] - Targets[3 * t], 2) + Math.Pow(R_path[3 * j + 1] - Targets[3 * t + 1], 2) + Math.Pow(R_path[3 * j + 2] - Targets[3 * t + 2], 2));
					        double R_target = Rt + Rr;
					        double delta_T = (R_target - (R_center)) / c;

					        //double R_target=Targets_Rs[j*(*TargetNum)+t];
					        //double delta_T=(R_target-(*R_center))/C;

					        //if (Math.Abs(Tr[i]-delta_T)<(*tp)/2) {
					        //	data[j*RanNum+i].x=data[j*RanNum+i].x+sigma[t]*Math.Cospi(-2*Fc*R_target/C+Kr*Math.Pow(Tr[i]-delta_T,2));
					        //	data[j*RanNum+i].y=data[j*RanNum+i].y+sigma[t]*Math.Sinpi(-2*Fc*R_target/C+Kr*Math.Pow(Tr[i]-delta_T,2));
					        //}
					        double StepLen = Math.Abs(Tr[1] - Tr[0]);
					        if (Math.Abs(Tr[i] - delta_T) <= Math.Abs(StepLen)) {
						        double new_sigma = sigma[t] * (StepLen - Math.Abs(Tr[i] - delta_T)) / StepLen;
						        data[j*RanNum + i].real = data[j*RanNum + i].real + new_sigma*Math.Cos(-2.0*PI*Fc*R_target / c);
						        data[j*RanNum + i].imag = data[j*RanNum + i].imag + new_sigma*Math.Sin(-2.0*PI*Fc*R_target / c);
					        }
				        }
			        }
		        }
	        }
        }

        public static void createRawData(complex[] dev_RawData, complex[] dev_data, complex[] RanRef)
        {
	        for (int j = 0; j<AziNum; j++) {
		        for (int i = 0; i<RanNum; i++) {
			        double real = dev_RawData[j*RanNum + i].real;
			        double imag = dev_RawData[j*RanNum + i].imag;
			        dev_RawData[j*RanNum + i].real = real*RanRef[i].real - imag*RanRef[i].imag;
			        dev_RawData[j*RanNum + i].imag = real*RanRef[i].imag + imag*RanRef[i].real;
		        }
	        }
        }

        public static void getRawData(complex[] data, double[] Tr, double[] R_path, double[] T_path, double[] Targets, double[] sigma, long TargetNum, double ApertureRange_min, double ApertureRange_max)
        {
	        double tp = Tp;

	        calculateData(data, Tr, tp, R_path, T_path, Targets, sigma, TargetNum, R_center, ApertureRange_min, ApertureRange_max);
	        /*
	        double *data_r,*data_i;
	        FILE *file_r,*file_i;
	        file_r=fopen("RawEcho_r.dat","wb");
	        file_i=fopen("RawEcho_i.dat","wb");
	        data_r=new double[AziNum*RanNum];
	        data_i=new double[AziNum*RanNum];
	        for (int i=0;i<AziNum*RanNum;i++) {
	        data_r[i]=data[i].real;
	        data_i[i]=data[i].imag;
	        }
	        fwrite(data_r,sizeof(double),AziNum*RanNum,file_r);
	        fwrite(data_i,sizeof(double),AziNum*RanNum,file_i);
	        fclose(file_r);
	        fclose(file_i);
	        delete[] data_r;
	        delete[] data_i;*/

	        //生成参考函数
	        complex[] ran_ref=new complex[RanNum];
            for (int i = 0; i < RanNum;i++ )
            {
                ran_ref[i] = new complex();
            }

                
            createRanRef(ran_ref, Tr);
	        fftshift(ran_ref, RanNum);

	        //回波生成
            FFT.fft((int)RanNum, ran_ref);
	        for (int i = 0; i<AziNum; i++) {
                complex[] tmpData = new complex[RanNum];
                for (int j = 0; j < RanNum; j++) {
                    tmpData[j] = data[RanNum * i + j];
                }
                FFT.fft((int)RanNum, tmpData);
	        }
	        createRawData(data, data, ran_ref);

	        for (int i = 0; i<AziNum; i++) {
                complex[] tmpData = new complex[RanNum];
                for (int j = 0; j < RanNum; j++)
                {
                    tmpData[j] = data[RanNum * i + j];
                }
                FFT.ifft((int)RanNum, tmpData);
	        }
        }

        public static double dot(double[] a, double[] b, int n)
        {
	        double sum = 0;
	        for (int i = 0; i<n; i++) {
		        sum += a[i] * b[i];
	        }
	        return sum;
        }

        public static double norm(double[] v, int n)
        {
	        double sum = 0;
	        for (int i = 0; i<n; i++) {
		        sum += v[i] * v[i];
	        }
	        return Math.Sqrt(sum);
        }

        public static double mod(double a, double b)
        {
	        return a - ((int)(a / b))*b;
        }

        public static void multiply(double[][] dst, double[][] src1, double[][] src2, int n, int m, int p) {
            double[][] srct = new double[3][]{ new double[3], new double[3], new double[3] };
	        //double srct[n][m];
	        for (int i = 0; i<n; i++) {
		        for (int j = 0; j<m; j++) {
			        srct[i][j] = src1[i][j];
		        }
	        }
	        for (int i = 0; i<n; i++) {
		        for (int j = 0; j<p; j++) {
			        dst[i][j] = 0;
			        for (int k = 0; k<m; k++) {
				        dst[i][j] += srct[i][k] * src2[k][j];
			        }
		        }
	        }
        }

        public static void calcSARPath(double[] R_path, double[] T_path) {
	        double fuc = geo_fuc;
	        double Omega = geo_Omega;
	        double w = geo_w;
	        double gama = geo_gama;
	        double thetaRR = geo_thetaRR;

	        double mu = 398600.44*1e9;
	        double a = 42164.17*1e3;
	        double tp = 0; //近地点时刻，tp=0则GEO卫星初始时刻位于近地点。
	        double thetay = 0 * PI / 180;
	        double thetap = 0 * PI / 180;
	        double thetar = 0 * PI / 180;
	        double k = 1; //right sight

	        //earth parameters
	        double Re = 6378.137*1e3;
	        double Rp = 6356.752*1e3;
	        double Tstart = 21600;
	        double n = 7.2921158*1e-5;
	        double Mtmid = n*(Tstart - tp);
	        double Ftmid = Mtmid + (2 * fuc - Math.Pow(fuc, 3) / 4)*Math.Sin((double)Mtmid) + 5.0 / 4 * Math.Pow(fuc, 2)*Math.Sin(2.0*Mtmid);
	        double Rsmid = a*(1 - Math.Pow(fuc, 2)) / (1 + fuc*Math.Cos(Ftmid));
	        double xsmid = Math.Cos(Omega)*Math.Cos(w + Ftmid)*Rsmid - Math.Sin(Omega)*Math.Cos(geo_i)*Math.Sin(w + Ftmid)*Rsmid;
	        double ysmid = Math.Sin(Omega)*Math.Cos(w + Ftmid)*Rsmid + Math.Cos(Omega)*Math.Cos(geo_i)*Math.Sin(w + Ftmid)*Rsmid;
	        double zsmid = Math.Sin(geo_i)*Math.Sin(w + Ftmid)*Rsmid;
	        double D3 = -Math.Cos(gama)*Math.Cos(thetap)*Math.Cos(thetar) + k*Math.Sin(gama)*Math.Cos(thetap)*Math.Sin(thetar);
	        double E3 = -Math.Cos(gama)*(-Math.Cos(thetay)*Math.Sin(thetap)*Math.Cos(thetar) + Math.Sin(thetay)*Math.Sin(thetar)) - k*Math.Sin(gama)*(Math.Cos(thetay)*Math.Sin(thetap)*Math.Sin(thetar) + Math.Sin(thetay)*Math.Cos(thetar));
	        double F3 = -Math.Cos(gama)*(Math.Sin(thetay)*Math.Sin(thetap)*Math.Cos(thetar) + Math.Cos(thetay)*Math.Sin(thetar)) - k*Math.Sin(gama)*(-Math.Sin(thetay)*Math.Sin(thetap)*Math.Sin(thetar) + Math.Cos(thetay)*Math.Cos(thetar));
	        double A3 = D3*(Math.Cos(Omega)*Math.Cos(w + Ftmid) - Math.Sin(Omega)*Math.Cos(geo_i)*Math.Sin(w + Ftmid)) + E3*(-Math.Cos(Omega)*Math.Sin(w + Ftmid) - Math.Sin(Omega)*Math.Cos(geo_i)*Math.Cos(w + Ftmid)) + F3*Math.Sin(Omega)*Math.Sin(geo_i);
	        double B3 = D3*(Math.Sin(Omega)*Math.Cos(w + Ftmid) + Math.Cos(Omega)*Math.Cos(geo_i)*Math.Sin(w + Ftmid)) + E3*(-Math.Sin(Omega)*Math.Sin(w + Ftmid) + Math.Cos(Omega)*Math.Cos(geo_i)*Math.Cos(w + Ftmid)) - F3*Math.Cos(Omega)*Math.Sin(geo_i);
	        double C3 = D3*Math.Sin(geo_i)*Math.Sin(w + Ftmid) + E3*Math.Sin(geo_i)*Math.Cos(w + Ftmid) + F3*Math.Cos(geo_i);
	        double K1 = Math.Pow(Rp, 2)*Math.Pow(A3, 2) + Math.Pow(Rp, 2)*Math.Pow(B3, 2) + Math.Pow(Re, 2)*Math.Pow(C3, 2);
	        double K2 = 2 * (Math.Pow(Rp, 2)*A3*xsmid + Math.Pow(Rp, 2)*B3*ysmid + Math.Pow(Re, 2)*C3*zsmid);
	        double K3 = Math.Pow(Rp, 2)*Math.Pow(xsmid, 2) + Math.Pow(Rp, 2)*Math.Pow(ysmid, 2) + Math.Pow(Re, 2)*Math.Pow(zsmid, 2) - Math.Pow(Re, 2)*Math.Pow(Rp, 2);
	        double Rmid = (-K2 - Math.Sqrt(Math.Pow(K2, 2) - 4 * K1*K3)) / 2 / K1;
	        double Xt = xsmid + A3*Rmid;
	        double Yt = ysmid + B3*Rmid;
	        double Zt = zsmid + C3*Rmid;
	        double[] rs_Tmid=new double[3]{xsmid, ysmid, zsmid };//GEO卫星在地球惯性坐标系下的位置坐标
	        double[] rt_Tmid=new double[3]{ Xt, Yt, Zt };//目标点在地球惯性坐标系下的位置坐标
	        double Beta_cen = Math.Acos(dot(rs_Tmid, rt_Tmid, 3) / norm(rs_Tmid, 3) / norm(rt_Tmid, 3));//目标点对应的地心角
	        double OmegaG = n*Tstart;//成像初始时刻Tstart时地球惯性坐标系与地球转动坐标系的角度差
	        double[][] Acr=new double[][]{  new double[3]{Math.Cos(OmegaG), Math.Sin(OmegaG), 0 }, new double[3]{ -Math.Sin(OmegaG), Math.Cos(OmegaG), 0 }, new double[3]{ 0, 0, 1 } };//地球惯性坐标系到地球转动坐标系的转换矩阵
	        double[] rt_TmidECR=new double[3]{ dot(Acr[0], rt_Tmid, 3), dot(Acr[1], rt_Tmid, 3), dot(Acr[2], rt_Tmid, 3) };
	        double lat_TmidECR = Math.Atan(rt_TmidECR[2] / Math.Sqrt(Math.Pow(rt_TmidECR[0], 2) + Math.Pow(rt_TmidECR[1], 2)));//目标点在地球转动坐标系下的纬度
	        //目标点在地球转动坐标系下的纬度
	        double long_TmidECR = 0;
	        if (rt_TmidECR[0] >= 0 && rt_TmidECR[1] >= 0) {
		        long_TmidECR = Math.Atan(rt_TmidECR[1] / rt_TmidECR[0]);
	        }
	        else if (rt_TmidECR[0]<0) {
		        long_TmidECR = Math.Atan(rt_TmidECR[1] / rt_TmidECR[0]) + PI;
	        }
	        else if (rt_TmidECR[0] >= 0 && rt_TmidECR[1]<0) {
		        long_TmidECR = Math.Atan(rt_TmidECR[1] / rt_TmidECR[0]) + 2 * PI;
	        }
	        double[][] T1= new double[3][]{new double[3]{ Math.Cos(long_TmidECR), Math.Sin(long_TmidECR), 0 }, new double[3]{ -Math.Sin(long_TmidECR), Math.Cos(long_TmidECR), 0 }, new double[3]{ 0, 0, 1 } };
	        double[][] T2 = new double[3][]{ new double[3]{ Math.Sin(lat_TmidECR), 0, -Math.Cos(lat_TmidECR) }, new double[3]{ 0, 1, 0 }, new double[3]{ Math.Cos(lat_TmidECR), 0, Math.Sin(lat_TmidECR) } };
	        double[][] A = new double[3][]{ new double[3]{ Math.Cos(w), -Math.Sin(w), 0 }, new double[3]{ Math.Sin(w), Math.Cos(w), 0 }, new double[3]{ 0, 0, 1 } };
	        double[][] B = new double[3][]{ new double[3]{ 1, 0, 0 }, new double[3]{ 0, Math.Cos(geo_i), -Math.Sin(geo_i) }, new double[3]{ 0, Math.Sin(geo_i), Math.Cos(geo_i) } };
	        double[][] C = new double[3][]{ new double[3]{ Math.Cos(Omega), -Math.Sin(Omega), 0 }, new double[3]{ Math.Sin(Omega), Math.Cos(Omega), 0 }, new double[3]{ 0, 0, 1 } };
	        double[][] Arv = new double[3][]{ new double[3]{ Math.Cos(Ftmid), -Math.Sin(Ftmid), 0 }, new double[3]{ Math.Sin(Ftmid), Math.Cos(Ftmid), 0 }, new double[3]{ 0, 0, 1 } };
	        double Ra_ref = norm(rt_Tmid, 3);
	        double Rsmid_di = Math.Sqrt(mu / a / (1 - Math.Pow(fuc, 2)))*fuc*Math.Sin(Ftmid);//GEO卫星距离的一阶导数
	        double AOL = mod((Ftmid + w), 2 * PI);//求解纬度幅角，如果纬度幅角超出2pi，求模。
	        double[] Vgp = new double[3]{ Ra_ref*n*Math.Sin(geo_i)*Math.Cos(AOL)*Math.Sin(Beta_cen), 0, Ra_ref*n*Math.Sin(geo_i)*Math.Cos(AOL)*Math.Cos(Beta_cen) };//求解初始时刻波束脚印速度的三个分量
	        double[] Vgr = new double[3]{ -Rsmid_di*Math.Tan(gama)*Math.Sin(Beta_cen), 0, -Rsmid_di*Math.Tan(gama)*Math.Cos(Beta_cen) };
	        double[] Vgt = new double[3]{ 0, Ra_ref*Math.Cos(Beta_cen) / Rsmid*(Math.Sqrt(mu / a / (1 - Math.Pow(fuc, 2)))*(1 + fuc*Math.Cos(Ftmid)) - Rsmid*n*Math.Cos(geo_i)), 0 };
	        double[] Vfoot_mid = new double[3]{ Vgp[0] + Vgr[0] + Vgt[0], Vgp[1] + Vgr[1] + Vgt[1], Vgp[2] + Vgr[2] + Vgt[2] };//星体坐标系下的波束脚印速度
            double[][] Martrix_ref = new double[3][] { new double[3], new double[3], new double[3] };//当前目标本地坐标系下波束脚印速度的矢量表示，波束脚印速度的z轴分量应该为零。
	        multiply(Martrix_ref, T2, T1, 3, 3, 3);
	        multiply(Martrix_ref, Martrix_ref, Acr, 3, 3, 3);
	        multiply(Martrix_ref, Martrix_ref, C, 3, 3, 3);
	        multiply(Martrix_ref, Martrix_ref, B, 3, 3, 3);
	        multiply(Martrix_ref, Martrix_ref, A, 3, 3, 3);
	        multiply(Martrix_ref, Martrix_ref, Arv, 3, 3, 3);
	        double[] Vfoot_ref = new double[3]{ dot(Martrix_ref[0], Vfoot_mid, 3), dot(Martrix_ref[1], Vfoot_mid, 3), dot(Martrix_ref[2], Vfoot_mid, 3) };
	        double thetaB = 0;
	        if (Vfoot_ref[0] >= 0 && Vfoot_ref[1] >= 0) {//波束脚印速度与y轴的夹角
		        thetaB = Math.Atan(Vfoot_ref[0] / Vfoot_ref[1]);
	        }
	        else if (Vfoot_ref[1]<0) {
		        thetaB = Math.Atan(Vfoot_ref[0] / Vfoot_ref[1]) + PI;
	        }
	        else if (Vfoot_ref[1] >= 0 && Vfoot_ref[0]<0) {
		        thetaB = Math.Atan(Vfoot_ref[0] / Vfoot_ref[1]) + 2 * PI;
	        }
	        double thetavRT = Math.Abs(thetaB - thetaRR);//接收站速度与发射站波束脚印速度的夹角
	        double[][] Avb = new double[3][]{ new double[3]{ Math.Cos(thetaRR), -Math.Sin(thetaRR), 0 }, new double[3]{ Math.Sin(thetaRR), Math.Cos(thetaRR), 0 }, new double[3]{ 0, 0, 1 } };//目标本地坐标系的转化矩阵
	        double theta_SQ = Math.Atan(Location_R[1] / Math.Sqrt(Math.Pow(Location_R[0], 2) + Math.Pow(Location_R[2], 2))) * 180 / PI;
	        double xR = Location_R[0];
	        double yR = Location_R[1];
	        double zR = Location_R[2];
	        double Rtmid = 0;//发射站初始时刻距离
	        for (int i = 0; i<3; i++) {
		        Rtmid += Math.Pow(rt_Tmid[i] - rs_Tmid[i], 2);
	        }
	        Rtmid = Math.Sqrt(Rtmid);
	        double Rrmid = 0;//接收站初始时刻距离
	        for (int i = 0; i<3; i++) {
		        Rrmid += Math.Pow(TargetCenter[i] - Location_R[i], 2);
	        }
	        Rrmid = Math.Sqrt(Rrmid);
	        double Rbimid = Rtmid + Rrmid;//初始时刻双基距离和
	        R_center = Rbimid;
	        double Fs = Rate_Fs*BandWidth;//距离维的采样率（双通道：实部＝I， 虚部＝Q）
	        double Ts = 1.0 / Fs;//快时间采样时间间隔
	        //long Nf = RanNum;//注意如果采样点数不够时是不能显示出很大的场景的，所以如果目标点之间间隔较远，可能无法显示出回波，此时需要增大采样点个数
	        double[] tf=new double[Nf];//快时间采样序列
	        double[] tf_org=new double[Nf];//理想快时间采样序列
	        double[] Fre_R=new double[Nf];//距离向的频率
	        for (int i = 0; i<Nf; i++) {
		        tf[i] = (i - 1.0*Nf / 2)*Ts + (Rbimid / c);
		        tf_org[i] = (i - 1.0*Nf / 2)*Ts;
		        Fre_R[i] = Fs / Nf*(i - 1.0*Nf / 2);
	        }
	        //long Ns = AziNum;//方位向采样点个数
	        double[] ts=new double[Ns];//慢时间采样序列
	        double[] ft=new double[Ns];//慢时间域频率变量
	        double[] ts_real=new double[Ns];
	        double[] M_t=new double[Ns];
	        double[] f_t=new double[Ns];
	        double[] Rs_t=new double[Ns];
	        double[] xs_t=new double[Ns];
	        double[] ys_t=new double[Ns];
	        double[] zs_t=new double[Ns];
	        double[][] Real_TECI=new double[Ns][];//卫星在地球惯性坐标系下的坐标
            for (int i=0;i<Ns;i++) {
                Real_TECI[i]=new double[3];
            }
	        double[] we_target=new double[Ns];
	        double[] xs_rECR=new double[Ns];
	        double[] ys_rECR=new double[Ns];
	        double[] zs_rECR=new double[Ns];
	        double[][] Real_TECR=new double[Ns][];//卫星在地球转动坐标系的位置
            for (int i=0;i<Ns;i++) {
                    Real_TECR[i]=new double[3];
            }
	        for (int i = 0; i<Ns; i++) {
		        ts[i] = (i - 1.0*Ns / 2)*PRT;
		        ft[i] = (i - 1.0*Ns / 2)*PRF / Ns;
		        R_path[i * 3 + 0] = Location_R[0];
		        R_path[i * 3 + 1] = Location_R[1] + ts[i] * Vr;
		        R_path[i * 3 + 2] = Location_R[2];

		        ts_real[i] = ts[i] + Tstart;
		        M_t[i] = n*(ts_real[i] - tp);
		        f_t[i] = M_t[i] + (2.0*fuc - Math.Pow(fuc, 3) / 4)*Math.Sin(M_t[i]) + 5.0 / 4 * Math.Pow(fuc, 2)*Math.Sin(2 * M_t[i]);
		        Rs_t[i] = a*(1 - Math.Pow(fuc, 2)) / (1 + fuc*Math.Cos(f_t[i]));
		        xs_t[i] = Math.Cos(Omega)*Math.Cos(w + f_t[i])*Rs_t[i] - Math.Sin(Omega)*Math.Cos(geo_i)*Math.Sin(w + f_t[i])*Rs_t[i];
		        ys_t[i] = Math.Sin(Omega)*Math.Cos(w + f_t[i])*Rs_t[i] + Math.Cos(Omega)*Math.Cos(geo_i)*Math.Sin(w + f_t[i])*Rs_t[i];
		        zs_t[i] = Math.Sin(geo_i)*Math.Sin(w + f_t[i])*Rs_t[i];
		        Real_TECI[i][0] = xs_t[i];
		        Real_TECI[i][1] = ys_t[i];
		        Real_TECI[i][2] = zs_t[i];
		        we_target[i] = n*ts_real[i];
		        xs_rECR[i] = Math.Cos(we_target[i])*xs_t[i] + Math.Sin(we_target[i])*ys_t[i];
		        ys_rECR[i] = -Math.Sin(we_target[i])*xs_t[i] + Math.Cos(we_target[i])*ys_t[i];
		        zs_rECR[i] = zs_t[i];
		        Real_TECR[i][0] = xs_rECR[i];
		        Real_TECR[i][1] = ys_rECR[i];
		        Real_TECR[i][2] = zs_rECR[i];
	        }
	        double[][] Tx=new double[3][]{new double[3],new double[3],new double[3]};
	        multiply(Tx, T2, T1, 3, 3, 3);
	        for (int i = 0; i<Ns; i++) {
		        double[] row=new double[3]{ dot(Tx[0], Real_TECR[i], 3), dot(Tx[1], Real_TECR[i], 3), dot(Tx[2], Real_TECR[i], 3) - Ra_ref };
		        T_path[i * 3 + 0] = dot(Avb[0], row, 3);
		        T_path[i * 3 + 1] = dot(Avb[1], row, 3);
		        T_path[i * 3 + 2] = dot(Avb[2], row, 3);
	        }
        }

        public static void getpic1()
        {
            byte[] data_r, data_i;

            double[] Tr=new double[RanNum];

	        for (int i = 0; i<RanNum; i++) {
		        Tr[i] = SapTime*(i - RanNum / 2);
	        }
	        double[] R_path= new double[3 * AziNum];
	        double[] T_path= new double[3 * AziNum];
	        calcSARPath(R_path, T_path);
	        double ApertureRange_min = TargetCenter[1] - Location_R[1] - Vr*Tsar / 2;
	        double ApertureRange_max = TargetCenter[1] - Location_R[1] + Vr*Tsar / 2;

	        const int TargetNum = 1;
	        double[] Targets=new double[3 * TargetNum]{ 0, 0, 0 };
	        double[] sigma =new double[1]{ 1 };

	        //回波生成
	        Matrix RawData=new Matrix();
	        RawData.elements = new complex[AziNum*RanNum];

	        //init value
	        for (int i = 0; i<AziNum*RanNum; i++) {
                RawData.elements[i] = new complex();
		        RawData.elements[i].real = 0;
		        RawData.elements[i].imag = 0;
	        }
            
	        getRawData(RawData.elements, Tr, R_path, T_path, Targets, sigma, TargetNum, ApertureRange_min, ApertureRange_max);

            
            			
            //BinaryReader file_r = new BinaryReader(File.Open("Echo_r.dat", FileMode.Open));
            FileStream file_r =File.Open("Echo_r.dat", FileMode.Open);
            FileStream file_i = File.Open("Echo_i.dat", FileMode.Open);

            data_r = new byte[AziNum * RanNum * sizeof(double)];
            data_i = new byte[AziNum * RanNum * sizeof(double)];
	        for (int i = 0; i<AziNum*RanNum; i++) {
                file_r.Write(BitConverter.GetBytes(RawData.elements[i].real), 0, sizeof(double));
                file_i.Write(BitConverter.GetBytes(RawData.elements[i].imag), 0, sizeof(double));
	        }
            file_r.Close();
            file_i.Close();


	        //距离向脉压
	        Matrix RancomImage=new Matrix();
	        RancomImage.elements = new complex[AziNum*RanNum];
            for (int i = 0; i < BpAziNum * BpRanNum; i++)
            {
                RancomImage.elements[i] = new complex();
            }
            /*
	        cout << "距离向脉压." << endl;
	        getRancomImage(RancomImage.elements, RawData.elements, Tr);
	        delete[] RawData.elements;

	        //file_r = fopen("Rancom_r.dat", "wb");
	        //file_i = fopen("Rancom_i.dat", "wb");
	        fopen_s(&file_r, "Rancom_r.dat", "wb");
	        fopen_s(&file_i, "Rancom_i.dat", "wb");

	        data_r = new double[AziNum*RanNum];
	        data_i = new double[AziNum*RanNum];
	        for (int i = 0; i<AziNum*RanNum; i++) {
		        data_r[i] = RancomImage.elements[i].real;
		        data_i[i] = RancomImage.elements[i].imag;
	        }
	        fwrite(data_r, sizeof(double), AziNum*RanNum, file_r);
	        fwrite(data_i, sizeof(double), AziNum*RanNum, file_i);
	        fclose(file_r);
	        fclose(file_i);
	        delete[] data_r;
	        delete[] data_i;*/

	        //BP算法
	        Matrix BPImage=new Matrix();
	        BPImage.elements = new complex[BpAziNum*BpRanNum];

            for (int i = 0; i < BpAziNum * BpRanNum; i++) {
                BPImage.elements[i] = new complex();
            }

	        double[] Xs = new double[BpRanNum];
	        for (int i = 0; i<BpRanNum; i++) {
		        Xs[i] = TargetCenter[0] + deltaX*(i - BpRanNum / 2 - 1);
	        }
	        double[] Ys = new double[BpAziNum];
	        for (int i = 0; i<BpAziNum; i++) {
		        Ys[i] = TargetCenter[1] + deltaY*(i - BpAziNum / 2 - 1);
	        }

	        double SapTime_u = SapTime / Coaf;
            /*
	        cout << "BP成像." << endl;

	        getBPImage(BPImage.elements, RancomImage.elements, Xs, Ys, T_path, R_path, &R_center, &SapTime_u, ApertureRange_min, ApertureRange_max);
	        delete[] RancomImage.elements;

	        //file_r = fopen("BpImage_r.dat", "wb");
	        //file_i = fopen("BpImage_i.dat", "wb");
	        fopen_s(&file_r, "BpImage_r.dat", "wb");
	        fopen_s(&file_i, "BpImage_i.dat", "wb");

	        data_r = new double[BpAziNum*BpRanNum];
	        data_i = new double[BpAziNum*BpRanNum];
	        for (int i = 0; i<BpAziNum*BpRanNum; i++) {
		        data_r[i] = BPImage.elements[i].real;
		        data_i[i] = BPImage.elements[i].imag;
	        }




	        fwrite(data_r, sizeof(double), BpAziNum*BpRanNum, file_r);
	        fwrite(data_i, sizeof(double), BpAziNum*BpRanNum, file_i);
	        fclose(file_r);
	        fclose(file_i);*/


        }



    }
}
