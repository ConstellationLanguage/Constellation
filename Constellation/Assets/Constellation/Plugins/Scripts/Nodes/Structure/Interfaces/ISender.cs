namespace Constellation
{
	public interface ISender
	{
		void Send (Variable value, Output _output);
		void Send(Variable value, int _output);
	}
}
