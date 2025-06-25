using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_create_map")]
public class smsg_create_map : IExtensible
{
	private edit_data _map;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "map", DataFormat = DataFormat.Default)]
	public edit_data map
	{
		get
		{
			return _map;
		}
		set
		{
			_map = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
