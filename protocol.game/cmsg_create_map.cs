using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_create_map")]
public class cmsg_create_map : IExtensible
{
	private msg_common _common;

	private int _index;

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

	[ProtoMember(2, IsRequired = true, Name = "index", DataFormat = DataFormat.TwosComplement)]
	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
