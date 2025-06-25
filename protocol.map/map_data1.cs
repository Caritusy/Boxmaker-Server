using System;
using System.Collections.Generic;
using ProtoBuf;

namespace protocol.map;

[Serializable]
[ProtoContract(Name = "map_data1")]
public class map_data1 : IExtensible
{
	private int _map_ver;

	private int _mode;

	private int _time;

	private int _no_music;

	private int _end_area;

	private readonly List<map_data_sub> _maps = new List<map_data_sub>();

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "map_ver", DataFormat = DataFormat.TwosComplement)]
	public int map_ver
	{
		get
		{
			return _map_ver;
		}
		set
		{
			_map_ver = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "mode", DataFormat = DataFormat.TwosComplement)]
	public int mode
	{
		get
		{
			return _mode;
		}
		set
		{
			_mode = value;
		}
	}

	[ProtoMember(3, IsRequired = true, Name = "time", DataFormat = DataFormat.TwosComplement)]
	public int time
	{
		get
		{
			return _time;
		}
		set
		{
			_time = value;
		}
	}

	[ProtoMember(4, IsRequired = true, Name = "no_music", DataFormat = DataFormat.TwosComplement)]
	public int no_music
	{
		get
		{
			return _no_music;
		}
		set
		{
			_no_music = value;
		}
	}

	[ProtoMember(5, IsRequired = true, Name = "end_area", DataFormat = DataFormat.TwosComplement)]
	public int end_area
	{
		get
		{
			return _end_area;
		}
		set
		{
			_end_area = value;
		}
	}

	[ProtoMember(6, Name = "maps", DataFormat = DataFormat.Default)]
	public List<map_data_sub> maps => _maps;

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
