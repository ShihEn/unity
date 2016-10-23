using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;
public class training : MonoBehaviour
{
		// Use this for initialization
		private GameObject test0;
		private GameObject test1;
		private GameObject test2;
		private GameObject test3;
		private GameObject test4;
		private GameObject test5;
		//-----------------------------------------------
		public Quaternion Q_arm_ref;
		public Quaternion Q_spine_ref;
		public Quaternion Q_forearm_ref;
		//-----------------------------------------------
		public Quaternion Q_ref;
		public Quaternion Q_spine;
		public Quaternion Q_arm;
		public Quaternion Q_forearm;
		private Quaternion angle_slide,angle_slide2,angle_slide3;
		private float armx=0F,army=0F,armz=0F,forearmx=0F,forearmy=0F,forearmz=0F;
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
		public GameObject trajectory1,trajectory2,trajectory3,trajectory4,trajectory5,trajectory6;
		private int traj=0;
		//--------------------------------------------------------------------------------------------------
		private float anglex, angley, anglez;
		public StreamWriter ref_sw;


		
		void Start()
		{
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources");				
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\0");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\1");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\2");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\3");
			System.IO.Directory.CreateDirectory(Application.dataPath+@"\Resources\4");
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
		}
		void OnGUI()
		{
				toggleRecforward = GUI.Toggle(new Rect(10, 45 , 200, 30),toggleRecforward,toggleRecforward?"Record":"forward");
				toggleRecside = GUI.Toggle(new Rect(10, 70 , 200, 30),toggleRecside,toggleRecside?"Record":"side");
				toggleRecupdown = GUI.Toggle(new Rect(10, 95 , 200, 30),toggleRecupdown,toggleRecupdown?"Record":"updown");
				toggleRec = GUI.Toggle(new Rect(10, 125, 200, 30), toggleRec, toggleRec?" Record":" Play");		

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
				if (GUI.Button (new Rect (120, 160, 100, 20), "Reback ")) {
						Application.LoadLevel(0);
				}
				if(GUI.Button (new Rect (10, 160, 100, 20), "Play Trial"))
				{
						//讀檔放入陣列中
						Count_array=file_io.play(Application.dataPath+@"\Resources\"+selectedItemIndex.ToString()+@"\ref_log.txt",selectedItemIndex);
						Flag_Play=(Count_array>0)?1:0;
						label_display=(Count_array>0)?"":"No file to Display";
						traj = 1;
						Q_count = 0;
				} 
				GUI.Label(new Rect (100, 125, 600, 200),label_display);
		}

		void Update()
		{
				//---------------------------------wedge feedback----------------------------------------------------------------
				if (traj == 1) {
						trajectory1.SetActive (true);
						trajectory2.SetActive (true);
						trajectory3.SetActive (true);
						trajectory4.SetActive (true);
						trajectory5.SetActive (true);
						trajectory6.SetActive (true);
				} else {
						trajectory1.SetActive (false);
						trajectory2.SetActive (false);
						trajectory3.SetActive (false);
						trajectory4.SetActive (false);
						trajectory5.SetActive (false);
						trajectory6.SetActive (false);
				}
				//-----------------------------cam section---------------------------------------------------------------------------
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
				//--------------------------------------play data--------------------------------------------------------------------
				if (Flag_Play == 1) {
						Q_spine=file_io.Q_ref[Q_count];
						Q_arm = file_io.Q_arm [Q_count];
						Q_forearm = file_io.Q_forearm [Q_count];
						label_display = "Play";
						if (Q_count < Count_array - 1) {
								Q_count++;
						} else {
								
								Flag_Play = 0;
						}
				} else {				
						if (Device_Sw == withoutDevice) { //without device
								if (string.Compare (st_sw_arm, "True") == 0) {//left hand
										Q_spine_ref.Set (0, 0, 0, 1);
										Q_arm_ref.Set (0, 0, 0, 1);
										Q_forearm_ref.Set (0, 0, 0, 1);
										//播放完動作會保持在最後的位置
										Q_spine=file_io.Q_ref[Count_array - 1];
										Q_arm = file_io.Q_arm [Count_array - 1];
										Q_forearm = file_io.Q_forearm [Count_array - 1];

								} else {//right hand
										Q_spine_ref.Set (0, 0, 0, 1);
										Q_arm_ref.Set (0, 0, 0, 1);
										Q_forearm_ref.Set (0, 0, 0, 1);
										//播放完動作會保持在最後的位置
										/*
										Q_spine=file_io.Q_ref[Q_count];
										Q_arm = file_io.Q_arm [Q_count];
										Q_forearm = file_io.Q_forearm [Q_count];
								*/}

						}
				}
				//----------------------------------------------------------------------------------------------------------------------
				test5.transform.rotation = (Quaternion.Inverse(Q_spine_ref)*Q_spine);
				test2.transform.rotation = (Quaternion.Inverse(Q_arm_ref)*Q_arm);
				test3.transform.rotation = (Quaternion.Inverse(Q_forearm_ref)*Q_forearm);
				if(string.Compare(st_sw_arm,"True")==0)
				{
						test1.transform.rotation = (Quaternion.Inverse (Q_arm_ref) * Q_arm * Quaternion.Euler (0, 180, 0));
						test0.transform.rotation = (Quaternion.Inverse (Q_forearm_ref) * Q_forearm * Quaternion.Euler (0, 180, 0));
						test4.transform.rotation = (Quaternion.Inverse (Q_forearm_ref) * Q_forearm * Quaternion.Euler (0, 180, 0));
				}
				else
				{
						test1.transform.rotation = (Quaternion.Inverse(Q_arm_ref)*Q_arm);
						test0.transform.rotation = (Quaternion.Inverse (Q_forearm_ref) * Q_forearm);
						test4.transform.rotation = (Quaternion.Inverse (Q_forearm_ref) * Q_forearm);
				}


		}
}
