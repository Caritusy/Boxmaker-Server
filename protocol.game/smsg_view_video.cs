using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_video")]
public class smsg_view_video : IExtensible
{
	private byte[] _video_data;

	private byte[] _map_data;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "video_data", DataFormat = DataFormat.Default)]
	public byte[] video_data
	{
		get
		{
			return _video_data;
		}
		set
		{
			_video_data = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "map_data", DataFormat = DataFormat.Default)]
	public byte[] map_data
	{
		get
		{
			return _map_data;
		}
		set
		{
			_map_data = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
