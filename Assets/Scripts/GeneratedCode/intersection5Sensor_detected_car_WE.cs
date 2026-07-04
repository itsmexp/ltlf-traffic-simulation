using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
using TrafficSimulation;
namespace ThinkEngine
{
	public class intersection5Sensor_detected_car_WE : Sensor
	{
		private int counter;
		private object specificValue;
		private Operation operation;
		private BasicTypeMapper mapper;
		private List<bool> values = new List<bool>();
		public override void Initialize(SensorConfiguration sensorConfiguration)
		{
			this.gameObject = sensorConfiguration.gameObject;
			ready = true;
			int index = gameObject.GetInstanceID();
			mapper = (BasicTypeMapper)MapperManager.GetMapper(typeof(bool));
			operation = mapper.OperationList()[0];
			counter = 0;
			mappingTemplate = "intersection5Sensor_detected_car_WE(intersection5,objectIndex("+index+"),{0})." + Environment.NewLine;
		}
		public override void Destroy()
		{
		}
		public override void Update()
		{
			if(!ready)
			{
				return;
			}
			if(!invariant || first)
			{
				first = false;
				Intersection Intersection_1 = gameObject.GetComponent<Intersection>();
				if(Intersection_1 == null)
				{
					values.Clear();
					return;
				}
				if(Intersection_1 == null)
				{
					values.Clear();
					return;
				}
				bool detected_car_WE_2 = Intersection_1.detected_car_WE;
				if (values.Count == 1)
				{
					values.RemoveAt(0);
				}
				values.Add(detected_car_WE_2);
			}
		}
		public override string Map()
		{
			object operationResult = operation(values, specificValue, counter);
			if(operationResult != null)
			{
				return string.Format(mappingTemplate, BasicTypeMapper.GetMapper(operationResult.GetType()).BasicMap(operationResult));
			}
			else
			{
				return "";
			}
		}
	}
}