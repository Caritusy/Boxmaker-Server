using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_view_edit_single")]
public class cmsg_view_edit_single : IExtensible
{
	private msg_common _common;

	private int _map_id;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "common", DataFormat = DataFormat.Default)]
	public msg_common common
	{
		get
		{
			return _common;
		}
		set
		{
			_common = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "map_id", DataFormat = DataFormat.TwosComplement)]
	public int map_id
	{
		get
		{
			return _map_id;
		}
		set
		{
			_map_id = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
