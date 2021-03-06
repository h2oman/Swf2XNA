/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct ToNumber : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.ToNumber;}}
		public uint Version {get{return 5;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return 0; } }

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("tonumber");
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.ToNumber);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
