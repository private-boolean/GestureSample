using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GestureSample.Backend.Data
{
	[DataContract(Name = "LabelType")]
	public enum LabelType
	{
		[EnumMember]
		none, 

		[EnumMember]
		TARDIGRADE_FOOT,

		[EnumMember]
		TARDIGRADE_BODY,

		[EnumMember]
		TARDIGRADE_HEAD
	}

	[DataContract]
	class FFLabel
	{
		[DataMember]
		public readonly long labelId;

		[DataMember]
		public readonly string email;

		[DataMember]
		public readonly long imageId;

		[DataMember]
		public readonly LabelType labelType;

		[DataMember]
		public readonly string data;

		[DataMember]
		public readonly DateTime date;

		[DataMember]
		public readonly int score;

		public FFLabel(): this(0, "", 0, LabelType.none, "", DateTime.Now, 0)
		{	}

		public FFLabel(long labelId, String email, long imageId, LabelType labelType, String data, DateTime date, int score)
		{
			this.labelId = labelId;
			this.email = email;
			this.imageId = imageId;
			this.labelType = labelType;
			this.data = data;
			this.date = date;
			this.score = score;
		}
	}
}
