using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Model
{
	public class AppConfigModule
	{
		public string ModuleName { get; set; }
		public bool	IsEnable { get; set; }
	}

	public class AppConfigModuleMapper
	{		
		public bool VideoConsult { get; set; }
		public bool VitalSigns { get; set; }
		public bool MicCough { get; set; }
		public bool VoiceRecognition { get; set; }
	}
}
