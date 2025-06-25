using System;
using ProtoBuf;

namespace protocol.map;

[Serializable]
[ProtoContract(Name = "map_data")]
public class map_data : IExtensible
{
	private int _map_ver;

	private byte[] _array;

	private int _x_num;

	private int _y_num;

	private int _qd_y;

	private int _zd_y;

	private int _mode;

	private int _time;

	private int _map_theme;

	private int _no_music;

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

	[ProtoMember(2, IsRequired = true, Name = "array", DataFormat = DataFormat.Default)]
	public byte[] array
	{
		get
		{
			return _array;
		}
		set
		{
			_array = value;
		}
	}

	[ProtoMember(3, IsRequired = true, Name = "x_num", DataFormat = DataFormat.TwosComplement)]
	public int x_num
	{
		get
		{
			return _x_num;
		}
		set
		{
			_x_num = value;
		}
	}

	[ProtoMember(4, IsRequired = true, Name = "y_num", DataFormat = DataFormat.TwosComplement)]
	public int y_num
	{
		get
		{
			return _y_num;
		}
		set
		{
			_y_num = value;
		}
	}

	[ProtoMember(5, IsRequired = true, Name = "qd_y", DataFormat = DataFormat.TwosComplement)]
	public int qd_y
	{
		get
		{
			return _qd_y;
		}
		set
		{
			_qd_y = value;
		}
	}

	[ProtoMember(6, IsRequired = true, Name = "zd_y", DataFormat = DataFormat.TwosComplement)]
	public int zd_y
	{
		get
		{
			return _zd_y;
		}
		set
		{
			_zd_y = value;
		}
	}

	[ProtoMember(7, IsRequired = true, Name = "mode", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(8, IsRequired = true, Name = "time", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(9, IsRequired = true, Name = "map_theme", DataFormat = DataFormat.TwosComplement)]
	public int map_theme
	{
		get
		{
			return _map_theme;
		}
		set
		{
			_map_theme = value;
		}
	}

	[ProtoMember(10, IsRequired = true, Name = "no_music", DataFormat = DataFormat.TwosComplement)]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
