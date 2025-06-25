using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_play_edit_map")]
public class smsg_play_edit_map : IExtensible
{
	private byte[] _mapdata;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "mapdata", DataFormat = DataFormat.Default)]
	public byte[] mapdata
	{
		get
		{
			return _mapdata;
		}
		set
		{
			_mapdata = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
