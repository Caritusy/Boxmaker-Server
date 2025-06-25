using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_challenge_play")]
public class smsg_challenge_play : IExtensible
{
	private int _user_head;

	private string _user_name = string.Empty;

	private string _user_country = string.Empty;

	private string _map_name = string.Empty;

	private byte[] _map_data;

	private readonly List<int> _x = new List<int>();

	private readonly List<int> _y = new List<int>();

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "user_head", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int user_head
	{
		get
		{
			return _user_head;
		}
		set
		{
			_user_head = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "user_name", DataFormat = DataFormat.Default)]
	public string user_name
	{
		get
		{
			return _user_name;
		}
		set
		{
			_user_name = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(3, IsRequired = false, Name = "user_country", DataFormat = DataFormat.Default)]
	public string user_country
	{
		get
		{
			return _user_country;
		}
		set
		{
			_user_country = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(4, IsRequired = false, Name = "map_name", DataFormat = DataFormat.Default)]
	public string map_name
	{
		get
		{
			return _map_name;
		}
		set
		{
			_map_name = value;
		}
	}

	[ProtoMember(5, IsRequired = false, Name = "map_data", DataFormat = DataFormat.Default)]
	[DefaultValue(null)]
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

	[ProtoMember(6, Name = "x", DataFormat = DataFormat.TwosComplement)]
	public List<int> x => _x;

	[ProtoMember(7, Name = "y", DataFormat = DataFormat.TwosComplement)]
	public List<int> y => _y;

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
