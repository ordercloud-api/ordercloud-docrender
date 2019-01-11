﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender
{
	public class JobRenderResponse
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public DateTime DateQueued { get; set; }
		public string JobPacket { get; set; }
		public string ServerJobResponse { get; set; }
	}
}
