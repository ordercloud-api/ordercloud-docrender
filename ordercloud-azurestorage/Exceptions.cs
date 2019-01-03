using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.AzureStorage
{

	public class Exceptions
	{
		public class NotFoundException : Exception
		{
		}

		public class MissingEnvironmentVar : Exception
		{
			private string _name;
			public MissingEnvironmentVar(string name)
			{
				_name = name;
			}
			public override string Message => $"{_name} is a required Environmental Variable";
		}
	}
}
