using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_mission_finish")]
public class smsg_mission_finish : IExtensible
{
	private int _suc;

	private int _exp;

	private List<author_list> _authors = new List<author_list>();

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "suc", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int suc
	{
		get
		{
			return _suc;
		}
		set
		{
			_suc = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(2, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
	public int exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
		}
	}

	[ProtoMember(3, Name = "authors", DataFormat = DataFormat.Default)]
	public List<author_list> authors
	{
		get
		{
			return _authors;
		}
		set
		{
			_authors = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
