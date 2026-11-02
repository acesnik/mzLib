using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using UsefulProteomicsDatabases;

namespace Test
{
    [SetUpFixture]
    [ExcludeFromCodeCoverage]
    public class MzPlotSetUpClass
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Loaders.SetCultureAsInvariantCulture();
            Assert.That(Thread.CurrentThread.CurrentCulture == CultureInfo.InvariantCulture);
            Assert.That(Thread.CurrentThread.CurrentUICulture == CultureInfo.InvariantCulture);
        }
    }
}
