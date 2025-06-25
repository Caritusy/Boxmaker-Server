using System;
using System.Collections.Generic;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_comment")]
public class smsg_view_comment : IExtensible
{
	private List<comment> _comments = new List<comment>();

	private map_info _infos;

	private IExtension extensionObject;

	[ProtoMember(1, Name = "comments", DataFormat = DataFormat.Default)]
	public List<comment> comments
	{ get { return _comments; }
		set { _comments = value; }	}


	[ProtoMember(2, IsRequired = true, Name = "infos", DataFormat = DataFormat.Default)]
	public map_info infos
	{
		get
		{
			return _infos;
		}
		set
		{
			_infos = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
