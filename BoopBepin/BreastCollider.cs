using System.Collections.Generic;
using UnityEngine;

namespace BoopBepin
{
	public class BreastCollider : MonoBehaviour
	{
		private Vector3 mousePos_Prev = Vector3.zero;

		private static bool enb_boop = true;
		
		private const int addBase = 350;
		private const int pleasureForceMin = 6;
		private const int distance = 100;
		
		private const float scaling = 0.0001f;
		private const float damping = 0.5f;

		private readonly CircularBuffer mouseVelocity = new CircularBuffer(5);
		private readonly List<DynamicBone_Ver02> dbv2s = new List<DynamicBone_Ver02>();
		private readonly List<DynamicBone> dbv0s = new List<DynamicBone>();
		
		private void Awake()
		{
			FindDBv2();
		}

		public void AddHuman(Human hum)
		{
			if(hum == null)
				return;

			foreach (var item in hum.transform.GetComponentsInChildren<DynamicBone_Ver02>(true))
				if (item != null && !dbv2s.Contains(item))
					dbv2s.Add(item);
				
			foreach (var item2 in hum.transform.GetComponentsInChildren<DynamicBone>(true))
				if (item2 != null && !dbv0s.Contains(item2))
					dbv0s.Add(item2);
		}
		
		public void FindDBv2()
		{
			dbv2s.Clear();
			dbv0s.Clear();
			
			foreach (var hum in FindObjectsOfType<Human>())
				AddHuman(hum);
		}

		private static void ApplyForce(DynamicBone_Ver02 dbc, Vector3 f)
		{
			dbc.Force += -f * 1.5f * 0.0001f;
		}

		private static void ApplyForce(DynamicBone dbc, Vector3 f)
		{
			dbc.m_Force += -f * 2f * 0.0001f;
		}
		
		private void Update()
		{
			if (BoopBepin.Toggle.Value.IsDown())
				enb_boop = !enb_boop;

			if (!enb_boop) 
				return;

			var mousePosition = Input.mousePosition;
			var obj = mousePos_Prev - mousePosition;
			
			mouseVelocity.Add(obj);
			mousePos_Prev = mousePosition;
			
			var vector = mouseVelocity.Average();
			var f = BoopBepin.cam.transform.rotation * vector;
			
			foreach (var dynamicBone_Ver in dbv2s)
			{
				if (dynamicBone_Ver == null || dynamicBone_Ver.Bones[3].transform == null)
					continue;
				
				var flag8 = Vector3.Distance(BoopBepin.cam.WorldToScreenPoint(dynamicBone_Ver.Bones[3].transform.position), mousePosition) < 100f;
				if (flag8)
					ApplyForce(dynamicBone_Ver, f);
				
				dynamicBone_Ver.Force *= 0.5f;
			}
			
			foreach (var dynamicBone in dbv0s)
			{
				if (dynamicBone == null || dynamicBone.m_Root == null)
					continue;
				
				var flag11 = Vector3.Distance(BoopBepin.cam.WorldToScreenPoint(dynamicBone.m_Root.position), mousePosition) < 150f;
				if (flag11)
					ApplyForce(dynamicBone, f);
				
				dynamicBone.m_Force *= 0.5f;
			}
		}
	}
}
