using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;
public class trajectory : MonoBehaviour
{
		// Use this for initialization
		private GameObject test0;
		private GameObject test1;
		private GameObject test2;
		private GameObject test3;
		private GameObject test4;
		private GameObject test5;
		private GameObject test6;
		//----------------------------------------------
		public Quaternion Q_arm_ref;
		public Quaternion Q_spine_ref;
		public Quaternion Q_forearm_ref;
		//----------------------------------------------
		public Quaternion Q_spine;
		public Quaternion Q_arm;
		public Quaternion Q_forearm;
		private Quaternion angle_slide,angle_slide2,angle_slide3;
		private float armx=0F,army=0F,armz=0F,forearmx=0F,forearmy=0F,forearmz=0F,x,y,z,xx,yy,zz,xxx,yyy,zzz;
		//-------------------save motion data define----------------------------------------------------
		GUIContent[] comboBoxList;
		private ComboBox comboBoxControl;// = new ComboBox();
		private GUIStyle listStyle = new GUIStyle();
		public static int selectedItemIndex=0; //get-set
		private string label_display="";
		public static bool toggleRec = false;
		public static bool toggleRecforward = false;
		public static bool toggleRecside = false;
		public static bool toggleRecupdown = false;
		public uint Rec_points=10;
		public static int ReadComLock=0;
		public int Flag_Rec=0;
		public int Flag_Play=0;
		Int32 Count_array;
		private int Device_Sw=1; //0=with device, 1=without device;
		private const int withDevice=0;
		private const int withoutDevice=1;
		private byte Condition=0; //condition=0 bypass ,condition=1 record, condition=3 save file
		private string st_sw_arm;
		private string st_sw_device;
		Int32 Q_count=0;
		//-----------------------------------------------------------------------------------------------
		public GameObject cam1, cam2, cam3; //兩個不同的攝影機
		public GameObject obj; //兩個不同的GameObject
		private int traj=0;
		//--------------------------------------------------------------------------------------------------
		public StreamWriter ref_sw;
		void Start()
		{
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources");				
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\0");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\1");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\2");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\3");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\4");
			st_sw_arm=PlayerPrefs.GetString("theToggleArm");
			st_sw_device=PlayerPrefs.GetString("theToggleManual");
			if(string.Compare(st_sw_arm,"True")==0)
			{
				test5 = GameObject.Find ("vincent:Spine2");
				test2 = GameObject.Find("vincent:LeftArm");
				test3 = GameObject.Find("vincent:LeftForeArm");
				test1 = GameObject.Find("wedgefeedback4");
				test0 = GameObject.Find("wedgefeedback5");
				test4 = GameObject.Find("wedgefeedback6");	
			}
			else
			{
				//第一顆看spine看代償動作
				test5 = GameObject.Find ("vincent:Spine2");
				test2 = GameObject.Find("vincent:RightArm");
				test3 = GameObject.Find("vincent:RightForeArm");
				test1 = GameObject.Find("wedgefeedback");
				test0 = GameObject.Find("wedgefeedback2");
				test4 = GameObject.Find("wedgefeedback3");
			}

				//------------------------------------------------save motion data define----------------------------------------------------
				comboBoxList = new GUIContent[5];
				comboBoxList[0] = new GUIContent("Action 1");
				comboBoxList[1] = new GUIContent("Action 2");
				comboBoxList[2] = new GUIContent("Action 3");
				comboBoxList[3] = new GUIContent("Action 4");
				comboBoxList[4] = new GUIContent("Action 5");

				listStyle.normal.textColor = Color.white; 
				listStyle.onHover.background =
						listStyle.hover.background = new Texture2D (2, 2);
				listStyle.padding.left =
						listStyle.padding.right =
								listStyle.padding.top =
										listStyle.padding.bottom = 4;

				comboBoxControl = new ComboBox(new Rect(10, 190, 80, 20), comboBoxList[0], comboBoxList, "button", "box", listStyle);
				//--------------------------------------------------------------------------------------------------------------------------
		}
		void OnGUI()
		{
				//GUI.HorizontalSlider(new Rect(50, 100, 180, 25), angle_arm_flexion, -30.0F, 120.0F);
				var armxstring = armx.ToString ();
				var armystring = army.ToString ();
				var armzstring = armz.ToString ();

				var forearmxstring = forearmx.ToString ();
				var forearmystring = forearmy.ToString ();
				var forearmzstring = forearmz.ToString ();

				GUI.TextArea (new Rect (580, 45, 100, 25), forearmxstring);
				GUI.TextArea (new Rect (580, 70, 100, 25), forearmystring);	
				GUI.TextArea (new Rect (580, 95, 100, 25), forearmzstring);

				GUI.TextArea (new Rect (300, 45, 100, 25), armxstring);
				GUI.TextArea (new Rect (300, 70, 100, 25), armystring);	
				GUI.TextArea (new Rect (300, 95, 100, 25), armzstring);

				armx = GUI.HorizontalSlider (new Rect (120, 45, 180, 25), armx, 0.0F, 360.0F);
				army = GUI.HorizontalSlider (new Rect (120, 70, 180, 25), army, 0.0F, 360.0F);
				armz = GUI.HorizontalSlider (new Rect (120, 95, 180, 25), armz, 0.0F, 360.0F);

				forearmx = GUI.HorizontalSlider (new Rect (400, 45, 180, 25), forearmx, 0.0F, 360.0F);
				forearmy = GUI.HorizontalSlider (new Rect (400, 70, 180, 25), forearmy, 0.0F, 360.0F);
				forearmz = GUI.HorizontalSlider (new Rect (400, 95, 180, 25), forearmz, 0.0F, 360.0F);
				//-------------------------------------camera--------------------------------------------------------
				toggleRecforward = GUI.Toggle(new Rect(10, 45 , 200, 30),toggleRecforward,toggleRecforward?"Record":"forward");
				toggleRecside = GUI.Toggle(new Rect(10, 70 , 200, 30),toggleRecside,toggleRecside?"Record":"side");
				toggleRecupdown = GUI.Toggle(new Rect(10, 95 , 200, 30),toggleRecupdown,toggleRecupdown?"Record":"updown");
				toggleRec = GUI.Toggle(new Rect(10, 125, 200, 30), toggleRec, toggleRec?" Record":" Play");	
				//-------------------------------------save data--------------------------------------------------------------------------
				if(Flag_Play==1)
				{
						GUI.enabled=false;
				}
				else
				{
						GUI.enabled=true;
				}

				//combo box
				selectedItemIndex = comboBoxControl.Show();
				//end of combo box
				if (GUI.Button (new Rect (120, 190, 100, 20), "next page")) {
						Application.LoadLevel(1);
				}
				if(GUI.Button (new Rect (10, 160, 100, 20), "Play Trial"))
				{
						//讀檔放入陣列中
						Count_array=file_io.play(Application.dataPath+@"\Resources\"+selectedItemIndex.ToString()+@"\ref_log.txt",selectedItemIndex);
						Flag_Play=(Count_array>0)?1:0;
						label_display=(Count_array>0)?"":"No file to Display";
						traj = 0;
				} 
				GUI.Label(new Rect (100, 125, 600, 200),label_display);
				//-----------------------------------------------------------------------------------------------------------------------


		}

		void Update()
		{
				//----------------------------------------------camera----------------------------------------------------
				if (toggleRecforward == true) {
						cam1.SetActive (true);
						obj.SetActive (true);
						cam2.SetActive (false);
						cam3.SetActive (false);
				} else if (toggleRecside == true) {
						cam1.SetActive (false);
						obj.SetActive (true);
						cam2.SetActive (true);
						cam3.SetActive (false);
				} else if (toggleRecupdown == true) {
						cam1.SetActive (false);
						obj.SetActive (true);
						cam2.SetActive (false);
						cam3.SetActive (true);
				}else{
						cam1.SetActive (true);
						obj.SetActive (true);
						cam2.SetActive (false);
						cam3.SetActive (false);

				}

				//------------------------------------------------------save data--------------------------------------------------------
				if(toggleRec==true)
				{
						if(Condition==0)
						{
								Condition=1;

								//initialize data
								ref_sw= new StreamWriter(Application.dataPath+@"\Resources\"+selectedItemIndex.ToString()+@"\ref_log.txt");
								file_io.clear_Qref();
								label_display="Prepare to save data";
								if(string.Compare(st_sw_arm,"True")==0)
								{
										ref_sw.WriteLine("left");
								}
								else
								{
										ref_sw.WriteLine("right");
								}
								if(string.Compare(st_sw_device,"True")==0)
								{					
										ref_sw.WriteLine("withDevice");
								}
								else
								{
										ref_sw.WriteLine("withoutDevice");
								}

						}
						else if(Condition==1)
						{
								//save data
								int tmp=0;// just for test! need modify

								if(Device_Sw==withoutDevice)
								{
										ref_sw.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
												1,0,0,0,
												angle_slide.w,angle_slide.x,angle_slide.y,angle_slide.z,
												angle_slide2.w,angle_slide2.x,angle_slide2.y,angle_slide2.z,tmp);
										
								}
								//Q1: ref, Q2: arm, Q3:forearm.

								//Q1: ref, Q2: arm, Q3:forearm.
								label_display="Save data to memory";
						}
						//move into record mode
				}
				else
				{
						//SW=0,
						if(Condition==0)
						{
								//bypass mode
								label_display="Standby";
						}
						else
						{
								//condition==1
								//save raw file
								ref_sw.Close(); //write to file
								Condition=0;//change to bypass mode
								label_display="Save file to disk";
						}
				}

				if (Flag_Play == 1) {
						
						//Q_ref=file_io.Q_ref[Q_count];
						Q_spine=file_io.Q_ref[Q_count];
						Q_arm = file_io.Q_arm [Q_count];
						Q_forearm = file_io.Q_forearm [Q_count];
						label_display = "Play";
						if (Q_count < Count_array - 1) {
								Q_count++;
						} else {
								Q_count = 0;
								Flag_Play = 0;
						}
				} else {				

						if (Device_Sw == withoutDevice) { //without device
								if(string.Compare(st_sw_arm,"True")==0)//left hand
								{
										Q_spine_ref.Set (0, 0, 0, 1);
										Q_arm_ref.Set (0, 0, 0, 1);
										Q_forearm_ref.Set (0, 0, 0, 1);
										traj = 1;
										angle_slide = Quaternion.Euler (armx, army, armz);
										angle_slide2 = Quaternion.Euler (forearmx, forearmy, forearmz);
										Q_arm.Set (-angle_slide.x, -angle_slide.y, -angle_slide.z, angle_slide.w);
										Q_forearm.Set (angle_slide2.x, -angle_slide2.y, -angle_slide2.z, angle_slide2.w);
								}
								else//right hand
								{
										Q_spine_ref.Set (0, 0, 0, 1);
										Q_arm_ref.Set (0, 0, 0, 1);
										Q_forearm_ref.Set (0, 0, 0, 1);
										traj = 0;//控制播放時顯不顯示軌跡
										angle_slide = Quaternion.Euler (armx, army, armz);
										angle_slide2 = Quaternion.Euler (forearmx, forearmy, forearmz);
										Q_arm.Set (angle_slide.x, angle_slide.y, angle_slide.z, angle_slide.w);
										Q_forearm.Set (angle_slide2.x, angle_slide2.y, angle_slide2.z, angle_slide2.w);
								}
						}
				}
				test5.transform.rotation = (Quaternion.Inverse(Q_spine_ref)*Q_spine);
				test2.transform.rotation = (Quaternion.Inverse(Q_arm_ref)*Q_arm);
				test3.transform.rotation = (Quaternion.Inverse(Q_forearm_ref)*Q_forearm);
		}
}
