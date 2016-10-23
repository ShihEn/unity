//20140811
//minor update in section.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class file_io
{
	public static Quaternion[] Q_ref,Q_arm,Q_forearm;
	public static Int32[] train_section;
	
	//read csv to memory
	public static List<float[]> parseCSV(out int[] arr,string path)
	{
		List<float[]> parsedData = new List<float[]>();
		arr=new int[2]{0,0};
		using (StreamReader readFile = new StreamReader(path))
		{
	    	string line;
	    	string[] row;
			//readFile.ReadLine();
			
			line = readFile.ReadLine();
			if(string.Compare(line,"right")==0)
			{
				arr[0]=1;
			}
			else
			{
				arr[0]=0;
			}
			line=null;
			
			line= readFile.ReadLine();
			if(string.Compare(line,"withoutDevice")==0)
			{
				arr[1]=1;
			}
			else //with device
			{
				arr[1]=0;
			}
						
	    	while ((line = readFile.ReadLine()) != null)
	    	{
				row =line.Split(',');
				float[] tmp=new float[row.Length];
				for(int i=0;i<row.Length;i++)
				{					
					tmp[i]=Convert.ToSingle(row[i]);
				}				
				parsedData.Add(tmp);
			}
		}	
		return parsedData;
	}
	
	
//	public static List<float[]> parseCSV(string path)
//	{
//		List<float[]> parsedData = new List<float[]>();
//	
//		using (StreamReader readFile = new StreamReader(path))
//		{
//	    	string line;
//	    	string[] row;
//			readFile.ReadLine();
//			readFile.ReadLine();//檔頭
//			readFile.ReadLine();//跳兩行
//	    	while ((line = readFile.ReadLine()) != null)
//	    	{
//				row =line.Split(',');
//				float[] tmp=new float[row.Length];
//				for(int i=0;i<row.Length;i++)
//				{					
//					tmp[i]=Convert.ToSingle(row[i]);
//				}				
//				parsedData.Add(tmp);
//			}
//		}	
//		return parsedData;
//	}
	//read title
//	public static void readTitle(out int[] array2,string path)
//	{
//		array2=new int[2] {0,0};
//		
//		using (StreamReader readFile2 = new StreamReader(path))
//		{
////			string line;
////			line = readFile2.ReadLine();
////			if(string.Compare(line,"right")==0)
////			{
////				array2[0]=1;
////			}
////			else
////			{
////				array2[0]=0;
////			}
////			line=null;
////			
////			line= readFile2.ReadLine();
////			if(string.Compare(line,"withoutDevice")==0)
////			{
////				array2[0]=1;
////			}
////			else //with device
////			{
////				array2[0]=0;
////			}
//		}
//	}
	
	public static void clear_Qref()
	{
		Q_ref=null;
		Q_arm=null;
		Q_forearm=null;
	}
	
	public static int play(string path, int selectedItemIndex)
	{		
		if (File.Exists(path))
		{
			//List<float[]> testParse = parseCSV(Application.dataPath+@"\Resources\"+selectedItemIndex.ToString()+@"\ref_log.txt");
			int[] arr=new int[2];
			List<float[]> testParse = parseCSV(out arr,@path);
			float[][] a=testParse.ToArray();
			clear_Qref();
			Q_ref=new Quaternion[a.Length];
			Q_arm=new Quaternion[a.Length];
			Q_forearm=new Quaternion[a.Length];
			train_section=new Int32[a.Length];
			
			if(arr[0]==0&&arr[1]==0)//left with device
			{
				for(Int32 i=0;i<a.Length-1;i++)
				{
					file_io.Q_ref[i].Set(-a[i][0],a[i][2],a[i][1],a[i][3]);
					file_io.Q_arm[i].Set(a[i][4],a[i][6],-a[i][5],a[i][7]);
					file_io.Q_forearm[i].Set(a[i][8],a[i][10],-a[i][9],a[i][11]);
					train_section[i]=(Int32)a[i][12];
				}
			}
			else if(arr[0]==1&&arr[1]==0)//right with device
			{
				for(Int32 i=0;i<a.Length-1;i++)
				{
					file_io.Q_ref[i].Set(-a[i][0],a[i][2],a[i][1],a[i][3]);
					file_io.Q_arm[i].Set(-a[i][4],a[i][6],a[i][5],a[i][7]);
					file_io.Q_forearm[i].Set(-a[i][8],a[i][10],a[i][9],a[i][11]);
					train_section[i]=(Int32)a[i][12];
				}
			}
			else if(arr[0]==1&&arr[1]==1)//right without device
			{
				for(Int32 i=0;i<a.Length-1;i++)
				{
					file_io.Q_ref[i].Set(a[i][1],a[i][2],a[i][3],a[i][0]);
					file_io.Q_arm[i].Set(a[i][5],a[i][6],a[i][7],a[i][4]); 
					file_io.Q_forearm[i].Set(a[i][9],a[i][10],a[i][11],a[i][8]);
					train_section[i]=(Int32)a[i][12];
					//file_io.Q_ref[i].Set(-a[i][1],a[i][2],a[i][3],-a[i][0]);
					//file_io.Q_arm[i].Set(-a[i][5],a[i][6],a[i][7],-a[i][4]);
					//file_io.Q_forearm[i].Set(-a[i][9],a[i][11],a[i][10],-a[i][8]);
					//train_section[i]=(Int32)a[i][12];
				}
			}
			else//left without device
			{
				for(Int32 i=0;i<a.Length-1;i++)
				{
					//右手播出file_io檔的四元數表示方法，但但但但但不知道裝上IMU會不會一致?
					file_io.Q_ref[i].Set(a[i][1],a[i][2],a[i][3],a[i][0]);
					file_io.Q_arm[i].Set(-a[i][5],-a[i][6],-a[i][7],a[i][4]);
					file_io.Q_forearm[i].Set(a[i][9],-a[i][10],-a[i][11],a[i][8]);
					train_section[i]=(Int32)a[i][12];
				}
			}

			return a.Length-1; //read success
		}
		else
		{
			return 0; //read fail
		}
	}
	
	public static Vector3 QuaternionToEuler(Quaternion quaternion)
{
	float sqw = quaternion.w * quaternion.w;
	float sqx = quaternion.x * quaternion.x;
	float sqy = quaternion.y * quaternion.y;
	float sqz = quaternion.z * quaternion.z;
	float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
	float test = quaternion.x * quaternion.y + quaternion.z * quaternion.w;

	float heading, attitude, bank;

	if (test > 0.499f * unit)
	{ // singularity at north pole
		heading = 2.0f * (float)Math.Atan2(quaternion.x, quaternion.w);
		attitude = (float)Math.PI / 2.0f;
		bank = 0.0f;
	}
	else if (test < -0.499 * unit)
	{ // singularity at south pole
		heading = -2.0f * (float)Math.Atan2(quaternion.x, quaternion.w);
		attitude = -(float)Math.PI / 2.0f;
		bank = 0.0f;
	}
	else
	{
		heading = Mathf.Rad2Deg*(float)Math.Atan2(2.0f * quaternion.y * quaternion.w - 2.0f * quaternion.x * quaternion.z, sqx - sqy - sqz + sqw);
		attitude = Mathf.Rad2Deg*(float)Math.Asin(2.0f * test / unit);
		bank = Mathf.Rad2Deg*(float)Math.Atan2(2.0f * quaternion.x * quaternion.w - 2.0f * quaternion.y * quaternion.z, -sqx + sqy - sqz + sqw);
	}

	return new Vector3(bank, heading, attitude);//roll,yaw,pitch
}

//	public static int guide_direction (Vector3 point,Vector3 pivot, Quaternion data,Quaternion reference){
//		//data quaternion * inversed reference quaternion-->quaternion*vector3
//		//product map with x, y, z axis each time.
//		//pick up the max deviation and return direction: up/down/left/right/pro/sup
//		Vector3 v_data=new Vector3(0,0,0);
//		Vector3 v_reference=new Vector3(0,0,0);
//		
//		v_data=RotateAroundPoint(point,pivot,data);
//		v_reference=RotateAroundPoint(point,pivot,reference);
//		
//		float max=0;
//		float max2=0;
//		int max_index=0;
//		int axis_direction=0;
//		
//		max=v_data.x-v_reference.x;
//		max2=v_data.y-v_reference.y;
//		
//		if(max*max<max2*max2){
//			max=max2;
//			max_index=1;			
//		}
//		
//		max2=v_data.z-v_reference.z;
//		
//		if(max*max<max2*max2){
//			max=max2;
//			max_index=2;			
//		}
//		
//		switch (max_index){
//			case 0://z,pitch
//			axis_direction=(max>0)?1:2;
//			break;
//			case 1://x,roll
//			axis_direction=(max>0)?3:4;
//			break;
//			case 2://y,yaw
//			axis_direction=(max>0)?5:6;
//			break;
//		}		
//		/*table:
//		1 upward
//		2 downward
//		3 left
//		4 right
//		5 supination
//		6 pronation
//		*/
//		return axis_direction;
//	}	
	
		public static int guide_direction (Quaternion data,Quaternion reference){
		//data quaternion * inversed reference quaternion
		//product map with x, y, z axis each time.
		//pick up the max deviation and return direction: up/down/left/right/pro/sup
		float max=0;
		float max2=0;
		int max_index=0;
		int axis_direction=0;
		
		

		Quaternion Q_delta;
		Vector3 V_tmp=new Vector3();
		Vector3 V_axis=new Vector3();
		
		
		Q_delta=data*Quaternion.Inverse(reference);
		
		V_axis.Set(0,1,0);
		V_tmp=Q_delta*V_axis;
		max=V_tmp.z;
		
		//Y second, see change in Z
		V_axis.Set(0,0,1);
		V_tmp=Q_delta*V_axis;
		max2=V_tmp.x;		
		
		//compare x/y
		if(Mathf.Abs(max)<Mathf.Abs(max2)){
			max=max2;
			max_index=1;
		}
		
		//Z last, see change in X
		V_axis.Set(1,0,0);
		V_tmp=Q_delta*V_axis;
		max2=V_tmp.y;
		
		//compare z
		if(Mathf.Abs(max)<Mathf.Abs(max2)){
			max=max2;
			max_index=2;
		}
		
		switch (max_index){
			case 0://X is pitch, set Y, compare Z.
			axis_direction=(max>0)?1:2;
			break;
			case 1://Y is roll, set Z, compare X.
			axis_direction=(max>0)?3:4;
			break;
			case 2://Z is yaw, set X, compare Y.
			axis_direction=(max>0)?5:6;
			break;
		}
		
		/*table:
		1 upward
		2 downward
		3 left
		4 right
		5 supination
		6 pronation
		*/
		return axis_direction;
	}
	
	
	
	
	
	
//	public static int guide_direction (Quaternion data,Quaternion reference){
//		//data quaternion * inversed reference quaternion
//		//product map with x, y, z axis each time.
//		//pick up the max deviation and return direction: up/down/left/right/pro/sup
//		float max=0;
//		float max2=0;
//		int max_index=0;
//		int axis_direction=0;
//		
//		
//
//		Quaternion Q_delta;
//		Quaternion Q_axis=new Quaternion();
//		Quaternion Q_tmp;
//		Vector3 V_axis=new Vector3();
//		
//		Q_delta=data*Quaternion.Inverse(reference);
//		Q_axis.Set(0,1,0,0);
//		Q_tmp=Q_delta*Q_axis*Quaternion.Inverse(Q_delta);
//		
////		V_axis.Set(0,1,0);
////		max=RotateAroundPoint
//		
//		
//		max=Q_tmp.z;
//		
//		Q_axis.Set(0,0,1,0);
//		Q_tmp=Q_delta*Q_axis*Quaternion.Inverse(Q_delta);
//		max2=Q_tmp.x;
//		
//		//compare x/y
//		if(max*max<max2*max2){
//			max=max2;
//			max_index=1;
//		}
//		
//		Q_axis.Set(1,0,0,0);
//		Q_tmp=Q_delta*Q_axis*Quaternion.Inverse(Q_delta);
//		max2=Q_tmp.y;
//		
//		//compare z
//		if(max*max<max2*max2){
//			max=max2;
//			max_index=2;
//		}
//		
//		switch (max_index){
//			case 0://z,pitch
//			axis_direction=(max>0)?1:2;
//			break;
//			case 1://x,roll
//			axis_direction=(max>0)?3:4;
//			break;
//			case 2://y,yaw
//			axis_direction=(max>0)?5:6;
//			break;
//		}
//		
//		/*table:
//		1 upward
//		2 downward
//		3 left
//		4 right
//		5 supination
//		6 pronation
//		*/
//		return axis_direction;
//	}
	
	public static Quaternion forearm_origin(Quaternion arm_origin, Quaternion arm_destination,Quaternion forearm_destination){
		return arm_origin*Quaternion.Inverse(arm_destination)*forearm_destination;
	}
	
	//this funcion is equal to pqp-1 quaternion 
	static Vector3 RotateAroundPoint(Vector3 point,Vector3 pivot,Quaternion angle){
		Vector3 finalPos=point-pivot;
		finalPos=angle*finalPos+pivot;
		return finalPos;
	}
	
	public static bool angle_compare(Quaternion data, Quaternion reference, int bias){
		float tmp=Quaternion.Dot(data,reference);
//		Debug.Log(tmp.ToString());
//		float tmp2=Mathf.Rad2Deg*tmp;
//		bool tmp3=(tmp2<(float)bias);
//		return tmp3;
//		 tmp=Mathf.Acos(Quaternion.Dot(data,reference));
//		bool tmp2=(tmp<(float)bias);
//		return (tmp2);
		return (Mathf.Abs(Mathf.Rad2Deg*Mathf.Acos(Quaternion.Dot(data,reference)))<(float)bias);
	}

}
