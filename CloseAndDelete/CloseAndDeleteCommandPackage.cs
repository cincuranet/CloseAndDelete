using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace CloseAndDelete
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0.3", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(CloseAndDeleteCommandPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class CloseAndDeleteCommandPackage : AsyncPackage
	{
		public const string PackageGuidString = "653ed5b1-beb3-447e-b3ba-72f5ccb222b9";

		public CloseAndDeleteCommandPackage()
		{ }

		protected async override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await CloseAndDeleteCommand.InitializeAsync(this);
			await base.InitializeAsync(cancellationToken, progress);
		}
	}
}
