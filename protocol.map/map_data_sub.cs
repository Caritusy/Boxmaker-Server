using System;
using ProtoBuf;

namespace protocol.map;

[Serializable]
[ProtoContract(Name = "map_data_sub")]
public class map_data_sub : IExtensible
{
	private byte[] _array;

	private int _x_num;

	private int _y_num;

	private int _qd_y;

	private int _zd_y;

	private int _map_theme;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "array", DataFormat = DataFormat.Default)]
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

	[ProtoMember(2, IsRequired = true, Name = "x_num", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(3, IsRequired = true, Name = "y_num", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(4, IsRequired = true, Name = "qd_y", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(5, IsRequired = true, Name = "zd_y", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(6, IsRequired = true, Name = "map_theme", DataFormat = DataFormat.TwosComplement)]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
