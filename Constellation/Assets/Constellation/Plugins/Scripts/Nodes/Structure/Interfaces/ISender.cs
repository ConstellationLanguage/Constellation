namespace Constellation
{
	public interface ISender
	{
		void Send (Ray value, Output _output);
		void Send(Ray value, int _output);
	}
}
