namespace Constellation
{
	public interface ISender
	{
		void Send (global::Variable value, Output _output);
		void Send(global::Variable value, int _output);
	}
}
