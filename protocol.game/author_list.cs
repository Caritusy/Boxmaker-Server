using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "author_list")]
public class author_list : IExtensible
{
	private int _user_head;

	private string _user_name = string.Empty;

	private string _user_country = string.Empty;

	private string _map_name = string.Empty;

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

	[ProtoMember(2, IsRequired = false, Name = "user_name", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
