namespace ThrowOptimizer.Configuration
{
	public sealed record ProcessingConfiguration(LocalsInit LocalsInitMode = LocalsInit.KeepOriginal,
												bool NoInline = false,
												bool IlVerify = false);
}