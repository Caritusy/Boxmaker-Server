using System;
using System.Collections.Generic;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_player")]
public class smsg_view_player : IExtensible
{
	private player_data _data;

	private readonly List<map_recent> _recent = new List<map_recent>();

	private readonly List<map_upload> _upload = new List<map_upload>();

	private readonly List<map_top> _top = new List<map_top>();

	private readonly List<map_play> _play = new List<map_play>();

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "data", DataFormat = DataFormat.Default)]
	public player_data data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	[ProtoMember(2, Name = "recent", DataFormat = DataFormat.Default)]
	public List<map_recent> recent => _recent;

	[ProtoMember(3, Name = "upload", DataFormat = DataFormat.Default)]
	public List<map_upload> upload => _upload;

	[ProtoMember(4, Name = "top", DataFormat = DataFormat.Default)]
	public List<map_top> top => _top;

	[ProtoMember(5, Name = "play", DataFormat = DataFormat.Default)]
	public List<map_play> play => _play;

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
