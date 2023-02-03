namespace ThrowOptimizer.Configuration
{
	public sealed record ProcessingConfiguration(
		bool NoInline = false,
		bool IlVerify = false);
}