using System;
using ProtoBuf;

namespace protocol.map;

[Serializable]
[ProtoContract(Name = "map_url")]
public class map_url : IExtensible
{
	private int _map_theme;

	private byte[] _array;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "map_theme", DataFormat = DataFormat.TwosComplement)]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
