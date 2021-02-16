using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T2.CLS.LogTransport.Config
{
	public class TransportConfiguration
	{
		#region properties

		public bool ValidateInputData{ get; set; }

		#endregion //properties

		public override string ToString()
		{
			return $" {nameof(TransportConfiguration)}.{nameof(ValidateInputData)} : {ValidateInputData} ";
		}
	}
}
