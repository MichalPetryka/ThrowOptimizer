using McMaster.Extensions.CommandLineUtils;
using ThrowOptimizer.Configuration;

namespace ThrowOptimizer.Console
{
	internal class Program
	{
		[Option]
		public string Path { get; set; }
		[Option]
		public string[] References { get; set; }
		[Option]
		public bool NoInline { get; set; }

		private class LogHandler : Utils.LogHandler.ILogHandler
		{
			public void Log(string message, Utils.LogHandler.LogType logType)
			{
				System.Console.WriteLine(message);
			}
		}

		public int OnExecute()
		{
			using (ThrowProcessor throwProcessor = new(Path, References, new LogHandler(), new ProcessingConfiguration { NoInline = NoInline }))
				throwProcessor.Process();
			return 0;
		}

		private static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

		/*{
			RootCommand rootCommand = new();

			Option<string> path = new("--path", "Processed assembly path") { IsRequired = true };
			path.AddAlias("-p", "/path", "/p");
			rootCommand.AddOption(path);

			Option<string[]> references = new("--references", "Assembly references") { IsRequired = true };
			references.AddAlias("-r", "/references", "/r");
			rootCommand.AddOption(references);

			Option<bool> noInline = new("--noinline", "Emit NoInline on throw helpers") { IsRequired = false };
			noInline.AddAlias("/noinline");
			rootCommand.AddOption(noInline);

			Option<LocalsInit> localsInit = new("--localsinit", "Emit localsinit on throw helpers") { IsRequired = false };
			localsInit.AddAlias("/localsinit");
			rootCommand.AddOption(localsInit);

			rootCommand.SetHandler<string, string[], bool, LocalsInit, InvocationContext>(Main);

			return rootCommand.Invoke(args);
		}*/
	}
}
