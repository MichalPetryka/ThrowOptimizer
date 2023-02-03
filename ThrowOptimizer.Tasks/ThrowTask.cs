using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ThrowOptimizer.Configuration;

namespace ThrowOptimizer.Tasks
{
	public class ThrowTask : Task
	{
		[Required]
		public string AssemblyFile { get; set; } = null!;
		[Required]
		public string References { get; set; } = null!;

		public bool NoInline { get; set; } = false;

		public override bool Execute()
		{
			Log.LogWarning(References);
			using (ThrowProcessor throwProcessor = new(AssemblyFile, References.Split(';').Select(Path.GetDirectoryName).Distinct(),
				new LogHandler(this), new ProcessingConfiguration()
				{
					NoInline = NoInline
				}))
				throwProcessor.Process();
			return true;
		}
	}
}
