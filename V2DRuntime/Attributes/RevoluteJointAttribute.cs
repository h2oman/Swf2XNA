using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RevoluteJointAttribute : JointAttribute
	{
		/// <summary>
		/// The body2 angle minus body1 angle in the reference state (radians).
		/// </summary>
		public float ReferenceAngle;

		/// <summary>
		/// A flag to enable joint limits.
		/// </summary>
		public bool enableLimit;

		/// <summary>
		/// The lower angle for the joint limit (radians).
		/// </summary>
		public float lowerAngle;

		/// <summary>
		/// The upper angle for the joint limit (radians).
		/// </summary>
		public float upperAngle;

		/// <summary>
		/// A flag to enable the joint motor.
		/// </summary>
		public bool enableMotor;

		/// <summary>
		/// The desired motor speed. Usually in radians per second.
		/// </summary>
		public float motorSpeed;

		/// <summary>
		/// The maximum motor torque used to achieve the desired motor speed.
		/// Usually in N-m.
		/// </summary>
		public float maxMotorTorque;

		public RevoluteJointAttribute()
		{
			lowerAngle = 0.0f;
			upperAngle = 0.0f;
			maxMotorTorque = 0.0f;
			motorSpeed = 0.0f;
			enableLimit = false;
			enableMotor = false;
		}
		public void ApplyAttribtues(RevoluteJointDef j)
		{
			j.collideConnected = collideConnected;

			if (lowerAngle != 0.0f)
			{
				j.lowerAngle = lowerAngle;
			}
			if (upperAngle != 0.0f)
			{
				j.upperAngle = upperAngle;
			}
			if (maxMotorTorque != 0.0f)
			{
				j.maxMotorTorque = maxMotorTorque;
			}
			if (motorSpeed != 0.0f)
			{
				j.motorSpeed = motorSpeed;
			}
			if (enableLimit != false)
			{
				j.enableLimit = enableLimit;
			}
			if (enableMotor != false)
			{
				j.enableMotor = enableMotor;
			}
		}
	}
}