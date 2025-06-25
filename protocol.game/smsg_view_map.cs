using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_map")]
public class smsg_view_map : IExtensible
{
	private List<map_show> _infos = new List<map_show>();

	private int _page;

	private IExtension extensionObject;

	[ProtoMember(1, Name = "infos", DataFormat = DataFormat.Default)]
	public List<map_show> infos
	{
		get { return _infos; }
		set { _infos = value; }
	}

	[ProtoMember(2, IsRequired = false, Name = "page", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int page
	{
		get
		{
			return _page;
		}
		set
		{
			_page = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
