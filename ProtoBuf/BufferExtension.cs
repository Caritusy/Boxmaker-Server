using System.IO;

namespace ProtoBuf;

public sealed class BufferExtension : IExtension
{
	private byte[] buffer;

	int IExtension.GetLength()
	{
		return (buffer != null) ? buffer.Length : 0;
	}

	Stream IExtension.BeginAppend()
	{
		return new MemoryStream();
	}

	void IExtension.EndAppend(Stream stream, bool commit)
	{
		using (stream)
		{
			int num;
			if (commit && (num = (int)stream.Length) > 0)
			{
				MemoryStream memoryStream = (MemoryStream)stream;
				if (buffer == null)
				{
					buffer = memoryStream.ToArray();
					return;
				}
				int num2 = buffer.Length;
				byte[] to = new byte[num2 + num];
				Helpers.BlockCopy(buffer, 0, to, 0, num2);
				Helpers.BlockCopy(Helpers.GetBuffer(memoryStream), 0, to, num2, num);
				buffer = to;
			}
		}
	}

	Stream IExtension.BeginQuery()
	{
		return (buffer != null) ? new MemoryStream(buffer) : Stream.Null;
	}

	void IExtension.EndQuery(Stream stream)
	{
		using (stream)
		{
		}
	}
}
