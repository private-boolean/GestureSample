using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureSample.Backend.Data
{
	class Label
	{
		public enum LabelType
		{
			TARDIGRADE_FOOT,
			TARDIGRADE_BODY,
			TARDIGRADE_HEAD,
			none
		}

		public readonly long labelId;
		public readonly string email;
		public readonly long imageId;
		public readonly LabelType labelType;
		public readonly string data;
		public readonly DateTime date;
		public readonly int score;
	}
}
