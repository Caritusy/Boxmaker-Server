using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "map_point_rank")]
public class map_point_rank : IExtensible
{
	private string _player_name = string.Empty;

	private int _player_point;

	private int _player_level;

	private string _player_country = string.Empty;

	private int _video_id;

	private int _user_id;

	private int _visitor;

	private IExtension extensionObject;

	[DefaultValue("")]
	[ProtoMember(1, IsRequired = false, Name = "player_name", DataFormat = DataFormat.Default)]
	public string player_name
	{
		get
		{
			return _player_name;
		}
		set
		{
			_player_name = value;
		}
	}

	[ProtoMember(2, IsRequired = false, Name = "player_point", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int player_point
	{
		get
		{
			return _player_point;
		}
		set
		{
			_player_point = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "player_level", DataFormat = DataFormat.TwosComplement)]
	public int player_level
	{
		get
		{
			return _player_level;
		}
		set
		{
			_player_level = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "player_country", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string player_country
	{
		get
		{
			return _player_country;
		}
		set
		{
			_player_country = value;
		}
	}

	[ProtoMember(5, IsRequired = false, Name = "video_id", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int video_id
	{
		get
		{
			return _video_id;
		}
		set
		{
			_video_id = value;
		}
	}

	[ProtoMember(6, IsRequired = false, Name = "user_id", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int user_id
	{
		get
		{
			return _user_id;
		}
		set
		{
			_user_id = value;
		}
	}

	[ProtoMember(7, IsRequired = false, Name = "visitor", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int visitor
	{
		get
		{
			return _visitor;
		}
		set
		{
			_visitor = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
